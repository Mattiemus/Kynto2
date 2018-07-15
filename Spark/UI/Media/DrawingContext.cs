namespace Spark.UI.Media
{
    using System;
    using System.Collections.Generic;

    using Graphics;
    using Math;
    using Utilities;

    public class DrawingContext : Disposable
    {
        private readonly IRenderSystem _renderSystem;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        private bool _applyCameraViewProjection;
        private Matrix4x4 _worldMatrix;
        private Effect _spriteEffect;
        private IEffectParameter _matrixParam;
        private IEffectParameter _samplerParam;
        private readonly IEffectShaderGroup _pass;

        private IRenderContext _renderContext;
        private IShaderStage _pixelStage;
        private SamplerState _ss;

        private readonly Stack<DrawState> _drawStateStack;
        private float _currentOpacity;
        private Matrix4x4 _currentTransform;
        
        private bool _inBeginEnd;
        
        public DrawingContext(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            _renderSystem = renderSystem;
            _primitiveBatch = new PrimitiveBatch<VertexPositionColor>(renderSystem);

            _spriteEffect = renderSystem.StandardEffects.CreateEffect("Sprite");
            _matrixParam = _spriteEffect.Parameters["SpriteTransform"];
            _samplerParam = _spriteEffect.Parameters["SpriteMapSampler"];
            _pass = _spriteEffect.ShaderGroups["SpriteNoTexture"];

            _drawStateStack = new Stack<DrawState>();
            _currentOpacity = 1.0f;
            _currentTransform = Matrix4x4.Identity;
        }

        internal void Begin(IRenderContext renderContext)
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
            
            renderContext.BlendState = BlendState.AlphaBlendNonPremultiplied;
            renderContext.RasterizerState = RasterizerState.CullBackClockwiseFront;
            renderContext.DepthStencilState = DepthStencilState.None;
            _ss = SamplerState.LinearClamp;

            _worldMatrix = Matrix4x4.Identity;
            _inBeginEnd = true;
            _applyCameraViewProjection = false;

            _primitiveBatch.Begin(renderContext);

            ApplyEffect();
        }

        internal void End()
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

        public void DrawGeometry(Brush brush, Pen pen, Geometry geometry, Matrix4x4 matrix)
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("DrawRectangle called before begin");
            }

            if (brush == null && pen == null)
            {
                return;
            }

            if (brush != null)
            {
                Color fillBrushColor = GetBrushColor(brush);
                fillBrushColor.A = (byte)(fillBrushColor.A * _currentOpacity);

                Matrix4x4 translationMatrix = matrix * _currentTransform;
                
                DataBuffer<VertexPositionColor> vertexData = (DataBuffer<VertexPositionColor>)geometry.VertexData.Clone();
                for (int i = 0; i < vertexData.Length; i++)
                {
                    VertexPositionColor current = vertexData[i];
                    vertexData[i] = new VertexPositionColor(Vector3.Transform(current.Position, translationMatrix), fillBrushColor);
                }

                _primitiveBatch.DrawIndexed(PrimitiveBatchTopology.TriangleList, vertexData, geometry.IndexData);
            }
        }

        //public abstract void DrawImage(ImageSource imageSource, RectangleF rectangle);

        //public abstract void DrawImage(ImageSource imageSource, float opacity, RectangleF sourceRectangle, RectangleF destinationRectangle);

        //public void DrawLine(Pen pen, Vector2 point0, Vector2 point1);

        public void DrawRectangle(Brush brush, Pen pen, RectangleF rectangle)
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("DrawRectangle called before begin");
            }

            if (brush == null && pen == null)
            {
                return;
            }

            if (brush != null)
            {
                Color fillBrushColor = GetBrushColor(brush);
                fillBrushColor.A = (byte)(fillBrushColor.A * _currentOpacity);

                DrawRectangleInternal(
                    new RectangleF(
                        rectangle.X + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        rectangle.Width,
                        rectangle.Height),
                    fillBrushColor);
            }

            if (pen != null && pen.Brush != null && !MathHelper.IsApproxZero(pen.Thickness))
            {
                Color penBrushColor = GetBrushColor(pen.Brush);
                penBrushColor.A = (byte)(penBrushColor.A * _currentOpacity);

                DrawRectangleInternal(
                    new RectangleF(
                        rectangle.X + pen.Thickness + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        rectangle.Width - pen.Thickness - pen.Thickness,
                        pen.Thickness),
                    penBrushColor);

                DrawRectangleInternal(
                    new RectangleF(
                        rectangle.X + pen.Thickness + _currentTransform.Translation.X,
                        rectangle.Y + rectangle.Height - pen.Thickness + _currentTransform.Translation.Y,
                        rectangle.Width - pen.Thickness - pen.Thickness,
                        pen.Thickness),
                    penBrushColor);

                DrawRectangleInternal(
                    new RectangleF(
                        rectangle.X + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        pen.Thickness,
                        rectangle.Height),
                    penBrushColor);

                DrawRectangleInternal(
                    new RectangleF(
                        rectangle.X + rectangle.Width - pen.Thickness + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        pen.Thickness,
                        rectangle.Height),
                    penBrushColor);
            }
        }

        public void DrawRoundedRectangle(Brush brush, Pen pen, RectangleF rectangle, float radiusX, float radiusY)
        {
            // TODO
        }

        //public abstract void DrawText(FormattedText formattedText, Vector2 origin);

        public void PushOpacity(float opacity)
        {
            _drawStateStack.Push(new DrawState { Transform = _currentTransform, Opacity = _currentOpacity });
            _currentOpacity *= opacity;
        }
        
        public void PushTranslation(Vector2 translation)
        {
            _drawStateStack.Push(new DrawState { Transform = _currentTransform, Opacity = _currentOpacity });
            _currentTransform *= Matrix4x4.FromTranslation(translation.X, translation.Y, 0.0f);
        }

        public void Pop()
        {
            DrawState newDrawState = _drawStateStack.Pop();
            _currentOpacity = newDrawState.Opacity;
            _currentTransform = newDrawState.Transform;
        }

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

        private void DrawRectangleInternal(RectangleF rect, Color color)
        {
            _primitiveBatch.DrawQuad(
                new VertexPositionColor(new Vector3(rect.TopLeftPoint, 0.0f), color),
                new VertexPositionColor(new Vector3(rect.TopRightPoint, 0.0f), color),
                new VertexPositionColor(new Vector3(rect.BottomRightPoint, 0.0f), color),
                new VertexPositionColor(new Vector3(rect.BottomLeftPoint, 0.0f), color));
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

        private Color GetBrushColor(Brush brush)
        {
            if (brush is SolidColorBrush solidColorBrush)
            {
                return solidColorBrush.Color;
            }

            throw new NotSupportedException("Only solid color brushes are currently supported");
        }

        private struct DrawState
        {
            public Matrix4x4 Transform;
            public float Opacity;
        }
    }
}
