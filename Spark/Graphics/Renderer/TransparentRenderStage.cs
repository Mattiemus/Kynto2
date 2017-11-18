namespace Spark.Graphics.Renderer
{
    using Graphics.Materials;

    /// <summary>
    /// Defines logic for rendering transparent objects.
    /// </summary>
    public sealed class TransparentRenderStage : IRenderStage
    {
        private RasterizerState _pass1RS;
        private DepthStencilState _pass1DSS;
        private RasterizerState _pass2RS;

        private RasterizerState _defaultPass1RS;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransparentRenderStage"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system.</param>
        public TransparentRenderStage(IRenderSystem renderSystem) 
            : this(renderSystem, RenderBucketId.Transparent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransparentRenderStage"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system.</param>
        /// <param name="transparentBucketID">The transparent bucket identifier.</param>
        public TransparentRenderStage(IRenderSystem renderSystem, RenderBucketId transparentBucketID)
        {
            TransparentBucketId = transparentBucketID;

            InitRenderStates(renderSystem);
        }

        /// <summary>
        /// Gets or sets the transparent render bucket ID. By default this is <see cref="RenderBucketID.Transparent"/>.
        /// </summary>
        public RenderBucketId TransparentBucketId { get; set; }

        /// <summary>
        /// Gets or sets the rasterizer state for rendering back-faces of geometry. Default is a state set to <see cref="CullMode.Front"/>
        /// and <see cref="VertexWinding.CounterClockwise"/>. Setting this to null sets the default state.
        /// </summary>
        public RasterizerState BackPassRasterizerState
        {
            get => _pass1RS;
            set
            {
                _pass1RS = value;
                if (value == null)
                {
                    _pass1RS = _defaultPass1RS;
                }
            }
        }

        /// <summary>
        /// Gets or sets the depth stencil state for rendering back-faces of geometry. Default is a state set to <see cref="DepthStencilState.DepthWriteOff"/>.
        /// Setting this to null sets the default state.
        /// </summary>
        public DepthStencilState BackPassDepthStencilState
        {
            get => _pass1DSS;
            set
            {
                _pass1DSS = value;
                if (value == null)
                {
                    _pass1DSS = DepthStencilState.DepthWriteOff;
                }
            }
        }

        /// <summary>
        /// Gets or sets the rasterizer state for rendering the front-faces of geometry. Default is a state set to <see cref="RasterizerState.CullBackClockwiseFront"/>.
        /// Setting this to null sets the default state.
        /// </summary>
        public RasterizerState FrontPassRasterizerState
        {
            get => _pass2RS;
            set
            {
                _pass2RS = value;
                if (value == null)
                {
                    _pass2RS = RasterizerState.CullBackClockwiseFront;
                }
            }
        }

        /// <summary>
        /// Executes the draw logic of the stage.
        /// </summary>
        /// <param name="queue">Render queue of objects that are to be drawn by the renderer.</param>
        /// <param name="renderContext">Render context of the renderer.</param>
        public void Execute(IRenderContext renderContext, RenderQueue queue)
        {
            RenderBucket bucket = queue[TransparentBucketId];
            if (bucket == null)
            {
                return;
            }

            // See if we have an enforced RS state already that is wireframe or no culling, since mostly likely these are meant to be enforced
            // for debug purposes. If so, then we do single-pass rendering and don't enforce any other render states.
            bool isDebugRendering = IsDebugRendering(renderContext);
            RasterizerState oldEnRs = null;
            DepthStencilState oldEnDs = null;

            if (!isDebugRendering)
            {
                GetPreviousEnforcedRenderState(renderContext, out oldEnRs, out oldEnDs);
            }

            for (int i = 0; i < bucket.Count; i++)
            {
                RenderBucketEntry RenderBucketEntry = bucket[i];
                IRenderable renderable = RenderBucketEntry.Renderable;
                Material material = RenderBucketEntry.Material;

                TransparencyMode transMode = material.TransparencyMode;

                if (material != null)
                {
                    if (material.Passes.Count != 1)
                    {
                        transMode = TransparencyMode.OneSided;
                    }

                    if (transMode == TransparencyMode.OneSided || isDebugRendering)
                    {
                        DrawRenderable(renderContext, renderable, material);
                        continue;
                    }

                    // Do two pass rendering

                    // Enforce first pass states (back rendering)
                    renderContext.RasterizerState = _pass1RS;
                    renderContext.DepthStencilState = _pass1DSS;
                    renderContext.EnforcedRenderState |= EnforcedRenderState.DepthStencilState | EnforcedRenderState.RasterizerState;

                    DrawRenderable(renderContext, renderable, material);

                    // Enforce second pass states (front rendering)
                    renderContext.EnforcedRenderState &= ~EnforcedRenderState.DepthStencilState;
                    renderContext.RasterizerState = _pass2RS;

                    DrawRenderable(renderContext, renderable, material);

                    renderContext.EnforcedRenderState &= ~EnforcedRenderState.RasterizerState;
                }
            }

            if (!isDebugRendering)
            {
                HonorPreviousEnforcedRenderState(renderContext, oldEnRs, oldEnDs);
            }
        }

        /// <summary>
        /// Draws a renderable
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="renderable">Renderable to draw</param>
        /// <param name="material">Material to use when drawing the renderable</param>
        private void DrawRenderable(IRenderContext renderContext, IRenderable renderable, Material material)
        {
            material.ApplyMaterial(renderContext, renderable.RenderProperties);

            MaterialPassCollection passes = material.Passes;
            for (int passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                MaterialPass pass = passes[passIndex];
                pass.Apply(renderContext);
                renderable.SetupDrawCall(renderContext, TransparentBucketId, pass);
            }
        }

        /// <summary>
        /// Determines if debug rendering is being used
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <returns>True if debug rendering is being used, false otherwise</returns>
        private bool IsDebugRendering(IRenderContext renderContext)
        {
            if ((renderContext.EnforcedRenderState & EnforcedRenderState.RasterizerState) == EnforcedRenderState.RasterizerState)
            {
                RasterizerState oldEnRS = renderContext.RasterizerState;
                if (oldEnRS.Cull == CullMode.None || oldEnRS.Fill == FillMode.WireFrame)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the previous enforced render state
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="rsState">Previous rasterizer state</param>
        /// <param name="dsState">Previous depth stencil state</param>
        private void GetPreviousEnforcedRenderState(IRenderContext renderContext, out RasterizerState rsState, out DepthStencilState dsState)
        {
            rsState = null;
            dsState = null;

            if ((renderContext.EnforcedRenderState & EnforcedRenderState.RasterizerState) == EnforcedRenderState.RasterizerState)
            {
                rsState = renderContext.RasterizerState;
                renderContext.EnforcedRenderState &= ~EnforcedRenderState.RasterizerState;
            }

            if ((renderContext.EnforcedRenderState & EnforcedRenderState.DepthStencilState) == EnforcedRenderState.DepthStencilState)
            {
                dsState = renderContext.DepthStencilState;
                renderContext.EnforcedRenderState &= ~EnforcedRenderState.DepthStencilState;
            }
        }

        /// <summary>
        /// Honors the previously enforced render state
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="rsState">Previous rasterizer state</param>
        /// <param name="dsState">Previous depth stencil state</param>
        private void HonorPreviousEnforcedRenderState(IRenderContext renderContext, RasterizerState rsState, DepthStencilState dsState)
        {
            if (rsState != null)
            {
                renderContext.RasterizerState = rsState;
                renderContext.EnforcedRenderState |= EnforcedRenderState.RasterizerState;
            }

            if (dsState != null)
            {
                renderContext.DepthStencilState = dsState;
                renderContext.EnforcedRenderState |= EnforcedRenderState.DepthStencilState;
            }
        }

        /// <summary>
        /// Initializes the render states
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        private void InitRenderStates(IRenderSystem renderSystem)
        {
            _pass1RS = _defaultPass1RS = new RasterizerState(renderSystem);
            _pass1RS.Cull = CullMode.Front;
            _pass1RS.VertexWinding = VertexWinding.CounterClockwise;
            _pass1RS.BindRenderState();

            _pass1DSS = DepthStencilState.DepthWriteOff;

            _pass2RS = RasterizerState.CullBackClockwiseFront;
        }
    }
}
