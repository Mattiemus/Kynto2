namespace Spark.UI
{
    using System;
    using System.Collections.Generic;

    using Graphics;
    using Math;
    using Utilities;

    public sealed class DrawingContext : Disposable
    {
        private readonly IRenderSystem _renderSystem;
        private readonly SpriteBatch _spriteBatch;
        private bool _inBeginEnd;
        private IRenderContext _renderContext;
        private RenderTarget2D _uiRenderTarget;
        private Rectangle _bounds;
        
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

        public Rectangle Bounds
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

        public void Begin(IRenderContext renderContext)
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

        public void End()
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
            throw new NotImplementedException();
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
