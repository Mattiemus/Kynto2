namespace Spark.Graphics
{
    /// <summary>
    /// A material pass contains all the necessary state information to configure the render pipeline before a draw call is issued.
    /// </summary>
    public sealed class MaterialPass : INamable
    {
        private IEffectShaderGroup _shaderGroup;
        private string _name;
        private BlendState _blendState;
        private RasterizerState _rasterizerState;
        private DepthStencilState _depthStencilState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPass"/> class.
        /// </summary>
        /// <param name="parent">Parent material</param>
        /// <param name="shaderGroup">Shader group</param>
        /// <param name="passName">Pass name</param>
        internal MaterialPass(Material parent, IEffectShaderGroup shaderGroup, string passName)
        {
            Parent = parent;
            _name = passName;
            _shaderGroup = shaderGroup;
            PassIndex = 0;

            RenderStatesToApply = EnforcedRenderState.All;
            _blendState = BlendState.Opaque;
            _rasterizerState = RasterizerState.CullBackClockwiseFront;
            _depthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Gets the material that owns this pass.
        /// </summary>
        public Material Parent { get; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                _name = value;

                // Everytime the pass name changes, make sure our fast look up stays updated. Hopefully this won't be called frequently
                Parent.Passes.UpdateFastLookup();
            }
        }

        /// <summary>
        /// Gets the group of shaders that will be set when this pass is applied.
        /// </summary>
        public IEffectShaderGroup ShaderGroup
        {
            get => _shaderGroup;
            set
            {
                if (_shaderGroup != null && _shaderGroup.IsPartOf(Parent.Effect))
                {
                    _shaderGroup = value;
                }
            }
        }

        /// <summary>
        /// Gets the index of this pass in the collection it is contained in.
        /// </summary>
        public int PassIndex { get; internal set; }

        /// <summary>
        /// Gets or sets the render states that will be applied in this pass. By default, none are. When a render state object is set to the pass, it will
        /// cause the pass to always apply that particular state type. This behavior can be disabled by removing the corresponding flag bit.
        /// </summary>
        public EnforcedRenderState RenderStatesToApply { get; set; }

        /// <summary>
        /// Gets or sets the blend state for the pass. By default, this is <see cref="Graphics.BlendState.Opaque" />.
        /// </summary>
        public BlendState BlendState
        {
            get => _blendState;
            set
            {
                if (value == null)
                {
                    value = BlendState.Opaque;
                }

                _blendState = value;
                RenderStatesToApply |= EnforcedRenderState.BlendState;
            }
        }

        /// <summary>
        /// Gets or sets the rasterizer state for the pass. By default, this is <see cref="Graphics.RasterizerState.CullBackClockwiseFront" />.
        /// </summary>
        public RasterizerState RasterizerState
        {
            get => _rasterizerState;
            set
            {
                if (value == null)
                {
                    value = RasterizerState.CullBackClockwiseFront;
                }

                _rasterizerState = value;
                RenderStatesToApply |= EnforcedRenderState.RasterizerState;
            }
        }

        /// <summary>
        /// Gets or sets the depth stencil state for the pass. By default, this is <see cref="Graphics.DepthStencilState.Default" />.
        /// </summary>
        public DepthStencilState DepthStencilState
        {
            get => _depthStencilState;
            set
            {
                if (value == null)
                {
                    value = DepthStencilState.Default;
                }

                _depthStencilState = value;
                RenderStatesToApply |= EnforcedRenderState.DepthStencilState;
            }
        }

        /// <summary>
        /// Applies the pass to the context which will configure the graphics pipeline by setting render states and binding shaders/resources.
        /// </summary>
        /// <param name="renderContext">Render context to apply to.</param>
        public void Apply(IRenderContext renderContext)
        {
            if (renderContext == null)
            {
                return;
            }

            // Apply render states
            if (RenderStatesToApply.HasFlag(EnforcedRenderState.BlendState))
            {
                renderContext.BlendState = _blendState;
            }

            if (RenderStatesToApply.HasFlag(EnforcedRenderState.RasterizerState))
            {
                renderContext.RasterizerState = _rasterizerState;
            }

            if (RenderStatesToApply.HasFlag(EnforcedRenderState.DepthStencilState))
            {
                renderContext.DepthStencilState = _depthStencilState;
            }

            // Apply shaders and bind resources
            _shaderGroup.Apply(renderContext);
        }

        /// <summary>
        /// Sets the shader group to be used by the pass by its name.
        /// </summary>
        /// <param name="name">Name of the shader group contained in the parent material's effect.</param>
        /// <returns>True if the shader group was set, false if it could not be found.</returns>
        public bool SetShaderGroup(string name)
        {
            Effect e = Parent.Effect;
            IEffectShaderGroup grp = e.ShaderGroups[name];
            if (grp == null)
            {
                return false;
            }

            _shaderGroup = grp;
            return true;
        }
    }
}
