namespace Spark.UI.Media
{
    using System;

    using Graphics;
    using Math;
    using Utilities;

    public class DrawingContext : Disposable
    {
        private readonly IRenderSystem _renderSystem;
        private IRenderContext _renderContext;
        private readonly SpriteBatch _spriteBatch;
        private RenderTarget2D _uiRenderTarget;
        private Rectangle _bounds;
        private bool _inBeginEnd;

        public DrawingContext(IRenderSystem renderSystem, Rectangle bounds)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            _renderSystem = renderSystem;
            _spriteBatch = new SpriteBatch(renderSystem);
            _uiRenderTarget = new RenderTarget2D(renderSystem, bounds.Width, bounds.Height);
            _bounds = bounds;
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
                    new Rectangle(
                        (int)rectangle.X,
                        (int)rectangle.Y,
                        (int)rectangle.Width,
                        (int)rectangle.Height),
                    Color.White);
            }

            if (pen != null && pen.Brush != null && !MathHelper.IsApproxZero(pen.Thickness))
            {
                _spriteBatch.Draw(
                    pen.Brush.GetTexture(_renderSystem),
                    new Rectangle(
                        (int)(rectangle.X + pen.Thickness),
                        (int)rectangle.Y,
                        (int)(rectangle.Width - pen.Thickness - pen.Thickness),
                        (int)pen.Thickness),
                    Color.White);

                _spriteBatch.Draw(
                    pen.Brush.GetTexture(_renderSystem),
                    new Rectangle(
                        (int)(rectangle.X + pen.Thickness),
                        (int)(rectangle.Y + rectangle.Height - pen.Thickness),
                        (int)(rectangle.Width - pen.Thickness - pen.Thickness),
                        (int)pen.Thickness),
                    Color.White);

                _spriteBatch.Draw(
                    pen.Brush.GetTexture(_renderSystem),
                    new Rectangle(
                        (int)(rectangle.X),
                        (int)(rectangle.Y),
                        (int)(pen.Thickness),
                        (int)(rectangle.Height)),
                    Color.White);

                _spriteBatch.Draw(
                    pen.Brush.GetTexture(_renderSystem),
                    new Rectangle(
                        (int)(rectangle.X + rectangle.Width - pen.Thickness),
                        (int)(rectangle.Y),
                        (int)(pen.Thickness),
                        (int)(rectangle.Height)),
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

        public void PushTransform(Transform transform)
        {

        }

        public void Pop()
        {

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
