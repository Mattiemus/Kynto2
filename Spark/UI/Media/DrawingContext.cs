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
        private readonly Stack<Matrix4x4> _transformStack;
        private IRenderContext _renderContext;
        private readonly SpriteBatch _spriteBatch;
        private RenderTarget2D _uiRenderTarget;
        private Rectangle _bounds;
        private Matrix4x4 _currentTransform;
        private bool _inBeginEnd;
        
        public DrawingContext(IRenderSystem renderSystem, Rectangle bounds)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            _renderSystem = renderSystem;
            _transformStack = new Stack<Matrix4x4>();
            _spriteBatch = new SpriteBatch(renderSystem);
            _uiRenderTarget = new RenderTarget2D(renderSystem, bounds.Width, bounds.Height);
            _bounds = bounds;
            _currentTransform = Matrix4x4.Identity;
        }

        internal Rectangle Bounds
        {
            get
            {
                ThrowIfDisposed();

                return _bounds;
            }
            set
            {
                ThrowIfDisposed();

                _bounds = value;
                BoundsUpdated();
            }
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

            _renderContext = renderContext;
            _inBeginEnd = true;

            renderContext.SetRenderTarget(_uiRenderTarget);

            _spriteBatch.Begin(_renderContext);
        }

        internal void End()
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("End called before begin");
            }

            _spriteBatch.End();

            _renderContext.SetRenderTarget(null);

            _spriteBatch.Begin(_renderContext);
            _spriteBatch.Draw(_uiRenderTarget, _bounds, Color.White);
            _spriteBatch.End();

            _renderContext = null;
            _inBeginEnd = false;
        }

        //public abstract void DrawGeometry(Brush brush, Pen pen, Geometry geometry);

        //public abstract void DrawImage(ImageSource imageSource, RectangleF rectangle);

        //public abstract void DrawImage(ImageSource imageSource, float opacity, RectangleF sourceRectangle, RectangleF destinationRectangle);

        public void DrawLine(Pen pen, Vector2 point0, Vector2 point1)
        {

        }

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
                _spriteBatch.Draw(
                    brush.GetTexture(_renderSystem),
                    new RectangleF(
                        rectangle.X + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        rectangle.Width,
                        rectangle.Height),
                    Color.White);
            }

            if (pen != null && pen.Brush != null && !MathHelper.IsApproxZero(pen.Thickness))
            {
                _spriteBatch.Draw(
                    pen.Brush.GetTexture(_renderSystem),
                    new RectangleF(
                        rectangle.X + pen.Thickness + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        rectangle.Width - pen.Thickness - pen.Thickness,
                        pen.Thickness),
                    Color.White);

                _spriteBatch.Draw(
                    pen.Brush.GetTexture(_renderSystem),
                    new RectangleF(
                        rectangle.X + pen.Thickness + _currentTransform.Translation.X,
                        rectangle.Y + rectangle.Height - pen.Thickness + _currentTransform.Translation.Y,
                        rectangle.Width - pen.Thickness - pen.Thickness,
                        pen.Thickness),
                    Color.White);

                _spriteBatch.Draw(
                    pen.Brush.GetTexture(_renderSystem),
                    new RectangleF(
                        rectangle.X + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        pen.Thickness,
                        rectangle.Height),
                    Color.White);

                _spriteBatch.Draw(
                    pen.Brush.GetTexture(_renderSystem),
                    new RectangleF(
                        rectangle.X + rectangle.Width - pen.Thickness + _currentTransform.Translation.X,
                        rectangle.Y + _currentTransform.Translation.Y,
                        pen.Thickness,
                        rectangle.Height),
                    Color.White);
            }
        }

        public void DrawRoundedRectangle(Brush brush, Pen pen, RectangleF rectangle, float radiusX, float radiusY)
        {

        }

        //public abstract void DrawText(FormattedText formattedText, Vector2 origin);

        public void PushOpacity(float opacity)
        {

        }
        
        public void PushTranslation(Vector2 translation)
        {
            _transformStack.Push(_currentTransform);
            _currentTransform *= Matrix4x4.FromTranslation(translation.X, translation.Y, 0.0f);
        }

        public void Pop()
        {
            if (_transformStack.Count == 0)
            {
                return;
            }

            _currentTransform = _transformStack.Pop();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _uiRenderTarget.Dispose();
                _spriteBatch.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private void BoundsUpdated()
        {
            _uiRenderTarget.Dispose();
            _uiRenderTarget = new RenderTarget2D(_renderSystem, Bounds.Width, Bounds.Height);
        }
    }
}
