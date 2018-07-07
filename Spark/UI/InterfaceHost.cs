namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;
    using Utilities;

    public class InterfaceHost : Disposable
    {
        private readonly IRenderSystem _renderSystem;
        private readonly SpriteBatch _spriteBatch;
        private readonly DrawingContext _drawingContext;
        private UIElement _uiElement;

        public InterfaceHost(IRenderSystem renderSystem, Rectangle bounds)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            _renderSystem = renderSystem;
            _spriteBatch = new SpriteBatch(renderSystem);
            _drawingContext = new DrawingContext(renderSystem, bounds);
        }
        
        public Rectangle Bounds
        {
            get
            {

                ThrowIfDisposed();

                return _drawingContext.Bounds;
            }
            set
            {
                ThrowIfDisposed();

                _drawingContext.Bounds = value;
                RemeasureContent();
            }
        }

        public UIElement Content
        {
            get => _uiElement;
            set
            {

                ThrowIfDisposed();

                _uiElement = value;
                RemeasureContent();
            }
        }
        
        public void Render(IRenderContext context)
        {
            ThrowIfDisposed();

            if (_uiElement == null)
            {
                return;
            }

            _drawingContext.Begin(context);
            _uiElement.Draw(_drawingContext);
            _drawingContext.End();
        }
        
        private void RemeasureContent()
        {
            if (_uiElement == null)
            {
                return;
            }

            _uiElement.Width = Bounds.Width;
            _uiElement.Height = Bounds.Height;

            _uiElement.UpdateLayout();
            _uiElement.Measure(new Size(Bounds.Width, Bounds.Height));
            _uiElement.Arrange(new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height));
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _drawingContext.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}
