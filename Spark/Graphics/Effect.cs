namespace Spark.Graphics
{
    using System;

    using Effects;
    using Content;
    using Implementation;

    /// <summary>
    /// Represents an effect that is used to render an object. This completely describes how to configure the graphics pipeline - what render states, resources,
    /// and shaders are bound to the GPU. An effect is necessary to render any object.
    /// </summary>
    public sealed class Effect : GraphicsResource, ISavable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        private Effect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="effectData">Compiled effect data that represents this effect.</param>
        public Effect(IRenderSystem renderSystem, EffectData effectData)
        {
            CreateImplementation(renderSystem, effectData);
        }

        /// <summary>
        /// Occurs when an effect shader group that is contained in this effect is about to be applied to a render context.
        /// </summary>
        public event OnApplyDelegate OnShaderGroupApply
        {
            add => EffectImplementation.OnShaderGroupApply += value;
            remove => EffectImplementation.OnShaderGroupApply -= value;
        }

        /// <summary>
        /// Gets the effect data
        /// </summary>
        public EffectData EffectData => EffectImplementation.EffectData;

        /// <summary>
        /// Gets the effect sort key, used to compare effects as a first step in sorting objects to render. The sort key is the same
        /// for cloned effects. Further sorting can then be done using the indices of the contained techniques, and the indices of their passes.
        /// </summary>
        public int SortKey => EffectImplementation.SortKey;

        /// <summary>
        /// Gets or sets the currently active shader group.
        /// </summary>
        public IEffectShaderGroup CurrentShaderGroup
        {
            get => EffectImplementation.CurrentShaderGroup;
            set
            {
                if (value == null || !value.IsPartOf(this))
                {
                    return;
                }

                EffectImplementation.CurrentShaderGroup = value;
            }
        }

        /// <summary>
        /// Gets the shader groups contained in this effect.
        /// </summary>
        public EffectShaderGroupCollection ShaderGroups => EffectImplementation.ShaderGroups;

        /// <summary>
        /// Gets all effect parameters contained in this effect.
        /// </summary>
        public EffectParameterCollection Parameters => EffectImplementation.Parameters;

        /// <summary>
        /// Gets all constant buffers that contain all value type parameters used by all passes.
        /// </summary>
        public EffectConstantBufferCollection ConstantBuffers => EffectImplementation.ConstantBuffers;
        
        /// <summary>
        /// Gets the effect implementation
        /// </summary>
        private IEffectImplementation EffectImplementation
        {
            get => Implementation as IEffectImplementation;
            set => BindImplementation(value);
        }

        /// <summary>
        /// Clones the effect, and possibly sharing relevant underlying resources. Cloned instances are guaranteed to be
        /// completely separate from the source effect in terms of parameters, changing one will not change the other. But unlike
        /// creating a new effect from the same compiled byte code, native resources can still be shared more effectively.
        /// </summary>
        /// <returns>Cloned effect instance.</returns>
        public Effect Clone()
        {
            ThrowIfDisposed();

            IEffectImplementation impl = EffectImplementation.Clone();

            return new Effect()
            {
                EffectImplementation = impl,
                Name = Name
            };
        }

        /// <summary>
        /// Sets the effect's current shader group by name.
        /// </summary>
        /// <param name="shaderGroupName">Name of the shader group to set to.</param>
        /// <returns>True if the shader group was found and set as the current active shader group, false otherwise.</returns>
        public bool SetCurrentShaderGroup(string shaderGroupName)
        {
            if (string.IsNullOrEmpty(shaderGroupName))
            {
                return false;
            }

            IEffectShaderGroup shaderGroup = ShaderGroups[shaderGroupName];
            if (shaderGroup == null)
            {
                return false;
            }

            EffectImplementation.CurrentShaderGroup = shaderGroup;
            return true;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            IRenderSystem renderSystem = GraphicsHelpers.GetRenderSystem(input.ServiceProvider);

            string name = input.ReadString();
            byte[] effectByteCode = input.ReadByteArray();
            EffectData effectData = EffectData.Read(effectByteCode);

            CreateImplementation(renderSystem, effectData);
            Name = name;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            byte[] effectBytes = EffectData.Write(EffectData);

            output.Write("Name", EffectImplementation.Name);
            output.Write("EffectByteCode", effectBytes);
        }

        /// <summary>
        /// Creates the underlying implementation
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        /// <param name="effectData">Compiled effect data</param>
        private void CreateImplementation(IRenderSystem renderSystem, EffectData effectData)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (effectData == null)
            {
                throw new ArgumentNullException(nameof(effectData));
            }
            
            if (!renderSystem.TryGetImplementationFactory(out IEffectImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                EffectImplementation = factory.CreateImplementation(effectData);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }
    }
}
