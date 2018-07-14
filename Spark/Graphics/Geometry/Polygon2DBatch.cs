namespace Spark.Graphics
{
    using System;

    using Math;
    using Utilities;

    public sealed class Polygon2DBatch : Disposable
    {
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        private bool _inBeginEnd;

        private bool _applyCameraViewProjection;
        private Matrix4x4 _worldMatrix;
        private Effect _spriteEffect;
        private IEffectParameter _matrixParam;
        private IEffectParameter _samplerParam;
        private readonly IEffectShaderGroup _pass;

        private IRenderContext _renderContext;
        private IShaderStage _pixelStage;
        private SamplerState _ss;

        public Polygon2DBatch(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem));
            }

            _primitiveBatch = new PrimitiveBatch<VertexPositionColor>(renderSystem);

            _spriteEffect = renderSystem.StandardEffects.CreateEffect("Sprite");
            _matrixParam = _spriteEffect.Parameters["SpriteTransform"];
            _samplerParam = _spriteEffect.Parameters["SpriteMapSampler"];
            _pass = _spriteEffect.ShaderGroups["SpriteNoTexture"];
        }

        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        public void Begin(IRenderContext renderContext)
        {
            Begin(renderContext, null, null, null, null, Matrix4x4.Identity, false);
        }

        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        public void Begin(IRenderContext renderContext, Matrix4x4 worldMatrix)
        {
            Begin(renderContext, null, null, null, null, worldMatrix, false);
        }

        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        /// <param name="applyCameraViewProj">Applies the view-projection matrix from the camera, otherwise creates a projection to draw in screen space.</param>
        public void Begin(IRenderContext renderContext, Matrix4x4 worldMatrix, bool applyCameraViewProj)
        {
            Begin(renderContext, null, null, null, null, worldMatrix, applyCameraViewProj);
        }

        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        public void Begin(IRenderContext renderContext, BlendState blendState)
        {
            Begin(renderContext, blendState, null, null, null, Matrix4x4.Identity, false);
        }

        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        public void Begin(IRenderContext renderContext, BlendState blendState, Matrix4x4 worldMatrix)
        {
            Begin(renderContext, blendState, null, null, null, worldMatrix, false);
        }

        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        /// <param name="applyCameraViewProj">Applies the view-projection matrix from the camera, otherwise creates a projection to draw in screen space.</param>
        public void Begin(IRenderContext renderContext, BlendState blendState, Matrix4x4 worldMatrix, bool applyCameraViewProj)
        {
            Begin(renderContext, blendState, null, null, null, worldMatrix, applyCameraViewProj);
        }

        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="rasterizerState">Rasterizer state to use, if null then <see cref="RasterizerState.CullBackClockwiseFront"/> is used.</param>
        /// <param name="samplerState">Sampler state to use, if null then <see cref="SamplerState.LinearClamp"/> is used.</param>
        /// <param name="depthStencilState">Depth stencil state to use, if null then <see cref="DepthStencilState.None"/> is used.</param>
        public void Begin(IRenderContext renderContext, BlendState blendState, RasterizerState rasterizerState, SamplerState samplerState, DepthStencilState depthStencilState)
        {
            Begin(renderContext, blendState, rasterizerState, samplerState, depthStencilState, Matrix4x4.Identity, false);
        }

        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="rasterizerState">Rasterizer state to use, if null then <see cref="RasterizerState.CullBackClockwiseFront"/> is used.</param>
        /// <param name="samplerState">Sampler state to use, if null then <see cref="SamplerState.LinearClamp"/> is used.</param>
        /// <param name="depthStencilState">Depth stencil state to use, if null then <see cref="DepthStencilState.None"/> is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        public void Begin(IRenderContext renderContext, BlendState blendState, RasterizerState rasterizerState, SamplerState samplerState, DepthStencilState depthStencilState, Matrix4x4 worldMatrix)
        {
            Begin(renderContext, blendState, rasterizerState, samplerState, depthStencilState, worldMatrix, false);
        }
        
        /// <summary>
        /// Begins a polygon batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="rasterizerState">Rasterizer state to use, if null then <see cref="RasterizerState.CullBackClockwiseFront"/> is used.</param>
        /// <param name="samplerState">Sampler state to use, if null then <see cref="SamplerState.LinearClamp"/> is used.</param>
        /// <param name="depthStencilState">Depth stencil state to use, if null then <see cref="DepthStencilState.None"/> is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        /// <param name="applyCameraViewProj">Applies the view-projection matrix from the camera, otherwise creates a projection to draw in screen space.</param>
        public void Begin(IRenderContext renderContext, BlendState blendState, RasterizerState rasterizerState, SamplerState samplerState, DepthStencilState depthStencilState, Matrix4x4 worldMatrix, bool applyCameraViewProj)
        {
            ThrowIfDisposed();

            if (_inBeginEnd)
            {
                throw new InvalidOperationException("Cannot nest begin calls");
            }

            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext), "Render context cannot be null");
            }

            _pixelStage = renderContext.GetShaderStage(ShaderStage.PixelShader);
            _renderContext = renderContext;

            if (blendState == null)
            {
                blendState = BlendState.AlphaBlendNonPremultiplied;
            }

            if (rasterizerState == null)
            {
                rasterizerState = RasterizerState.CullBackClockwiseFront;
            }

            if (samplerState == null)
            {
                samplerState = SamplerState.LinearClamp;
            }

            if (depthStencilState == null)
            {
                depthStencilState = DepthStencilState.None;
            }

            renderContext.BlendState = blendState;
            renderContext.RasterizerState = rasterizerState;
            renderContext.DepthStencilState = depthStencilState;
            _ss = samplerState;

            _worldMatrix = worldMatrix;
            _inBeginEnd = true;
            _applyCameraViewProjection = applyCameraViewProj;

            _primitiveBatch.Begin(renderContext);
            
            ApplyEffect();
        }

        public void End()
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("End called before begin");
            }

            _primitiveBatch.End();

            // Make sure we don't hold onto references...
            _samplerParam.SetResource((SamplerState)null);

            _renderContext = null;
            _pixelStage = null;
            _applyCameraViewProjection = false;
            _inBeginEnd = false;
        }

        public void DrawRaw(IReadOnlyDataBuffer<VertexPositionColor> vertices, IndexData indices)
        {
            _primitiveBatch.DrawIndexed(PrimitiveBatchTopology.TriangleList, vertices, indices);
        }

        public void DrawRectangle(RectangleF rect, Color color)
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("Draw called before begin");
            }
            
            _primitiveBatch.DrawQuad(
                new VertexPositionColor(new Vector3(rect.TopLeftPoint, 0.0f), color),
                new VertexPositionColor(new Vector3(rect.TopRightPoint, 0.0f), color),
                new VertexPositionColor(new Vector3(rect.BottomRightPoint, 0.0f), color),
                new VertexPositionColor(new Vector3(rect.BottomLeftPoint, 0.0f), color));
        }




        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (_primitiveBatch != null)
                {
                    _primitiveBatch.Dispose();
                    _primitiveBatch = null;
                }
                
                if (_spriteEffect != null)
                {
                    _spriteEffect.Dispose();
                    _spriteEffect = null;
                    _matrixParam = null;
                    _samplerParam = null;
                }
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Applies the effect to being used when rendering
        /// </summary>
        private void ApplyEffect()
        {
            Matrix4x4 matrix = Matrix4x4.Identity;

            if (_applyCameraViewProjection && _renderContext.Camera != null)
            {
                matrix = _renderContext.Camera.ViewProjectionMatrix;
            }
            else
            {
                Viewport viewport = (_renderContext.Camera == null) ? new Viewport(0, 0, 800, 600) : _renderContext.Camera.Viewport;

                SpriteBatch.ComputeSpriteProjection(ref viewport, out matrix);
            }

            Matrix4x4.Multiply(ref _worldMatrix, ref matrix, out Matrix4x4 spriteTransform);
            _matrixParam.SetValue(ref spriteTransform);
            _samplerParam.SetResource(_ss);

            _pass.Apply(_renderContext);
        }
    }
}
