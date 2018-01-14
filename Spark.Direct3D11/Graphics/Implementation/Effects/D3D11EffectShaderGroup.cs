namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Graphics;

    public sealed class D3D11EffectShaderGroup : IEffectShaderGroup
    {
        private static readonly int NumberOfStages = Enum.GetValues(typeof(ShaderStage)).Length;

        private readonly D3D11EffectImplementation _impl;
        private readonly EffectData.ShaderGroup _groupData;
        private readonly ShaderBlock[] _shaderBlocks;
        private readonly InputLayoutCache _localLayoutCache;

        internal D3D11EffectShaderGroup(D3D11EffectImplementation impl, EffectData.ShaderGroup groupData, int groupIndex) 
            : this(impl, groupData, groupIndex, null)
        {
        }

        internal D3D11EffectShaderGroup(D3D11EffectImplementation impl, EffectData.ShaderGroup groupData, int groupIndex, D3D11EffectShaderGroup groupToCloneFrom)
        {
            _impl = impl;
            Name = groupData.Name;
            ShaderGroupIndex = groupIndex;
            _groupData = groupData;
            _shaderBlocks = new ShaderBlock[NumberOfStages];

            if (groupData.ShaderIndices != null && groupData.ShaderIndices.Length > 0)
            {
                EffectData effectData = impl.EffectData;
                D3D11EffectResourceManager resourceManager = impl.Resourcemanager;
                int[] shaderIndices = groupData.ShaderIndices;

                for (int i = 0; i < shaderIndices.Length; i++)
                {
                    int shaderIndex = shaderIndices[i];
                    EffectData.Shader shader = effectData.Shaders[shaderIndex];
                    _shaderBlocks[(int)shader.ShaderType] = resourceManager.GetShaderBlock(impl.D3DDevice, shader);

                    if (shader.ShaderType == ShaderStage.VertexShader)
                    {
                        if (groupToCloneFrom == null)
                        {
                            var inputSig = new HashedShaderSignature(shader.InputSignature.ByteCode, shader.InputSignature.HashCode);
                            _localLayoutCache = new InputLayoutCache(impl.D3DDevice, inputSig);
                        }
                        else
                        {
                            _localLayoutCache = groupToCloneFrom._localLayoutCache;
                        }
                    }

                    _shaderBlocks[(int)shader.ShaderType] = resourceManager.GetShaderBlock(impl.D3DDevice, effectData.Shaders[shaderIndex]);
                }
            }
        }

        /// <summary>
        /// Gets the name of the effect part.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the index of this shader group in the collection it is contained in.
        /// </summary>
        public int ShaderGroupIndex { get; }

        public void Apply(IRenderContext renderContext)
        {
            if (renderContext == null)
            {
                return;
            }

            var d3dRenderContext = renderContext as D3D11RenderContext;

            // Setting of render states is done at a higher level (e.g. material, or possibly
            // by hooking into the OnApply event)
            _impl.OnPreApply(d3dRenderContext, this);

            // Announce input signature
            d3dRenderContext.SetCurrentInputLayoutCache(_localLayoutCache);

            // Apply shader blocks
            for (int i = 0; i < _shaderBlocks.Length; i++)
            {
                ShaderBlock shaderBlock = _shaderBlocks[i];
                if (shaderBlock != null)
                {
                    shaderBlock.Apply(d3dRenderContext);
                }
                else
                {
                    // Apply NULL shader
                    var stage = (ShaderStage)i;
                    var shaderStage = d3dRenderContext.GetShaderStage(stage) as D3D11ShaderStage;
                    shaderStage.SetShader(HashedShader.NullShader(stage));
                }
            }
        }

        /// <summary>
        /// Queries the shader group if it contains a shader used by the specified shader stage.
        /// </summary>
        /// <param name="shaderStage">Shader stage to query.</param>
        /// <returns>True if the group contains a shader that will be bound to the shader stage, false otherwise.</returns>
        public bool ContainsShader(ShaderStage shaderStage)
        {
            for (int i = 0; i < _shaderBlocks.Length; i++)
            {
                if (_shaderBlocks[i].Shader.ShaderType == shaderStage)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if this part belongs to the given effect.
        /// </summary>
        /// <param name="effect">Effect to check against</param>
        /// <returns>True if the effect is the parent of this part, false otherwise.</returns>
        public bool IsPartOf(Effect effect)
        {
            if (effect == null)
            {
                return false;
            }

            return ReferenceEquals(effect.Implementation, _impl);
        }
    }
}
