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
        private readonly Polygon2DBatch _polygonBatch;

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
            _polygonBatch = new Polygon2DBatch(renderSystem);

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

            _inBeginEnd = true;

            _polygonBatch.Begin(renderContext);
        }

        internal void End()
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("End called before begin");
            }

            _polygonBatch.End();
            
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

                _polygonBatch.DrawRaw(vertexData, geometry.IndexData);
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

                _polygonBatch.DrawRectangle(
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

                _polygonBatch.DrawRectangle(
                    new RectangleF(
                        rectangle.X + pen.Thickness + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        rectangle.Width - pen.Thickness - pen.Thickness,
                        pen.Thickness),
                    penBrushColor);

                _polygonBatch.DrawRectangle(
                    new RectangleF(
                        rectangle.X + pen.Thickness + _currentTransform.Translation.X,
                        rectangle.Y + rectangle.Height - pen.Thickness + _currentTransform.Translation.Y,
                        rectangle.Width - pen.Thickness - pen.Thickness,
                        pen.Thickness),
                    penBrushColor);

                _polygonBatch.DrawRectangle(
                    new RectangleF(
                        rectangle.X + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        pen.Thickness,
                        rectangle.Height),
                    penBrushColor);

                _polygonBatch.DrawRectangle(
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
                _polygonBatch.Dispose();
            }

            base.Dispose(isDisposing);
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
