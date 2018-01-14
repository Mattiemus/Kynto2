namespace Spark.Direct3D11.Graphics.Implementation
{
    using System.Collections.Generic;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="Effect"/>.
    /// </summary>
    public sealed class D3D11EffectImplementation : GraphicsResourceImplementation, IEffectImplementation
    {
        private D3D11EffectShaderGroup _currentShaderGroup;

        internal D3D11EffectImplementation(D3D11RenderSystem renderSystem, int resourceID, D3D11EffectImplementation implToCloneFrom)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            EffectData = implToCloneFrom.EffectData;
            Resourcemanager = implToCloneFrom.Resourcemanager.Clone();
            SortKey = implToCloneFrom.SortKey;

            Initialize(implToCloneFrom);
        }

        internal D3D11EffectImplementation(D3D11RenderSystem renderSystem, int resourceID, EffectData effectData)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            EffectData = effectData;
            Resourcemanager = new D3D11EffectResourceManager(EffectData);
            SortKey = renderSystem.GetNextUniqueEffectSortKey();

            Initialize();
        }

        /// <summary>
        /// Occurs when an effect shader group that is contained by this effect is about to be applied to a render context.
        /// </summary>
        public event OnApplyDelegate OnShaderGroupApply;

        /// <summary>
        /// Gets the effect sort key, used to compare effects as a first step in sorting objects to render. The sort key is the same
        /// for cloned effects. Further sorting can then be done using the indices of the contained techniques, and the indices of their passes.
        /// </summary>
        public int SortKey { get; }

        /// <summary>
        /// Gets or sets the currently active shader group.
        /// </summary>
        public IEffectShaderGroup CurrentShaderGroup
        {
            get => _currentShaderGroup;
            set => _currentShaderGroup = value as D3D11EffectShaderGroup;
        }

        /// <summary>
        /// Gets the shader groups contained in this effect.
        /// </summary>
        public EffectShaderGroupCollection ShaderGroups { get; private set; }

        /// <summary>
        /// Gets all effect parameters used by all passes.
        /// </summary>
        public EffectParameterCollection Parameters { get; private set; }

        /// <summary>
        /// Gets all constant buffers that contain all value type parameters used by all passes.
        /// </summary>
        public EffectConstantBufferCollection ConstantBuffers { get; private set; }

        /// <summary>
        /// Gets the effect data
        /// </summary>
        public EffectData EffectData { get; }

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        internal D3D11EffectResourceManager Resourcemanager { get; }
        
        /// <summary>
        /// Clones the effect, and possibly sharing relevant underlying resources. Cloned instances are guaranteed to be
        /// completely separate from the source effect in terms of parameters, changing one will not change the other. But unlike
        /// creating a new effect from the same compiled byte code, native resources can still be shared more effectively.
        /// </summary>
        /// <returns>Cloned effect implementation.</returns>
        public IEffectImplementation Clone()
        {
            D3D11RenderSystem renderSystem = RenderSystem as D3D11RenderSystem;
            return new D3D11EffectImplementation(renderSystem, renderSystem.GetNextUniqueResourceId(), this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                // Call release on the shader manager, last one turns off the lights and dispose of shared shader objects
                Resourcemanager.ShaderManager.Release();

                // Release constant buffers
                for (int i = 0; i < ConstantBuffers.Count; i++)
                {
                    (ConstantBuffers[i] as D3D11EffectConstantBuffer).Dispose();
                }
            }

            base.Dispose(isDisposing);
        }

        internal void OnPreApply(IRenderContext renderContext, IEffectShaderGroup shaderGroup)
        {
            OnShaderGroupApply?.Invoke(renderContext, shaderGroup);
        }

        private void Initialize(D3D11EffectImplementation implToCloneFrom = null)
        {
            // Setup constant buffers
            if (EffectData.ConstantBuffers != null && EffectData.ConstantBuffers.Length > 0)
            {
                var cbs = new D3D11EffectConstantBuffer[EffectData.ConstantBuffers.Length];

                for (int i = 0; i < cbs.Length; i++)
                {
                    D3D11EffectConstantBuffer cb;
                    if ((implToCloneFrom == null))
                    {
                        cb = new D3D11EffectConstantBuffer(D3DDevice, this, EffectData.ConstantBuffers[i]);
                    }
                    else
                    {
                        cb = new D3D11EffectConstantBuffer(D3DDevice, this, EffectData.ConstantBuffers[i], implToCloneFrom.ConstantBuffers[i] as D3D11EffectConstantBuffer);
                    }

                    cb.SetDebugName(string.Format("{0}:{1}", ResourceId, cb.Name));
                    cbs[i] = cb;
                }

                ConstantBuffers = new EffectConstantBufferCollection(cbs);
                Resourcemanager.SetConstantBuffers(ConstantBuffers);
            }
            else
            {
                ConstantBuffers = EffectConstantBufferCollection.EmptyCollection;
            }

            // Setup shader resources
            if (EffectData.ResourceVariables != null && EffectData.ResourceVariables.Length > 0)
            {
                var variables = new List<D3D11EffectParameter>(EffectData.ResourceVariables.Length);
                for (int i = 0, startResourceIndex = 0; i < EffectData.ResourceVariables.Length; i++)
                {
                    EffectData.ResourceVariable rvar = EffectData.ResourceVariables[i];

                    if (rvar.ResourceType != D3DShaderInputType.ConstantBuffer && rvar.ResourceType != D3DShaderInputType.TextureBuffer)
                    {
                        D3D11EffectParameter param = D3D11EffectParameter.CreateResourceVariable(this, Resourcemanager, startResourceIndex, rvar);

                        // ONLY set the default sampler state if we are NOT being cloned, otherwise cloning the resource manager already took care of this!
                        if (implToCloneFrom == null && param.ParameterType == EffectParameterType.SamplerState && rvar.SamplerData != null)
                        {
                            SetDefaultSamplerState(param, rvar);
                        }

                        variables.Add(param);
                    }

                    startResourceIndex += (rvar.ElementCount == 0) ? 1 : rvar.ElementCount;
                }

                Parameters = new EffectParameterCollection(new CompositeEffectParameterCollection(ConstantBuffers, variables));
            }
            else
            {
                Parameters = new EffectParameterCollection(new CompositeEffectParameterCollection(ConstantBuffers, EffectParameterCollection.EmptyCollection));
            }

            // Setup shader groups
            if (EffectData.ShaderGroups != null && EffectData.ShaderGroups.Length > 0)
            {
                var groups = new D3D11EffectShaderGroup[EffectData.ShaderGroups.Length];

                if (implToCloneFrom == null)
                {
                    for (int i = 0; i < groups.Length; i++)
                    {
                        groups[i] = new D3D11EffectShaderGroup(this, EffectData.ShaderGroups[i], i);
                    }
                }
                else
                {
                    for (int i = 0; i < groups.Length; i++)
                    {
                        groups[i] = new D3D11EffectShaderGroup(this, EffectData.ShaderGroups[i], i, implToCloneFrom.ShaderGroups[i] as D3D11EffectShaderGroup);
                    }
                }

                ShaderGroups = new EffectShaderGroupCollection(groups);
                _currentShaderGroup = ShaderGroups[0] as D3D11EffectShaderGroup;
            }
            else
            {
                ShaderGroups = EffectShaderGroupCollection.EmptyCollection;
                _currentShaderGroup = null;
            }
        }

        private void SetDefaultSamplerState(D3D11EffectParameter param, EffectData.ResourceVariable data)
        {
            // Sampler arrays are not supported for the default value setting...maybe one day
            if (param.IsArray)
            {
                return;
            }

            var renderSystem = RenderSystem as D3D11RenderSystem;
            param.SetResource(renderSystem.SamplerCache.GetOrCreateSamplerState(data.SamplerData));
        }
    }
}
