namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;
    using Utilities;

    public class InterfaceHost : Disposable
    {
        private readonly IRenderSystem _renderSystem;
        private RenderTarget2D _uiRenderTarget;
        private Rectangle _bounds;

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private SolidColorBrush _solidBrush;
        private LinearGradientBrush _gradientBrush;
        private RadialGradientBrush _radGradientBrush;

        public InterfaceHost(IRenderSystem renderSystem, Rectangle bounds, SpriteFont font)
        {
            _spriteBatch = new SpriteBatch(renderSystem);
            _font = font;
            _solidBrush = new SolidColorBrush(renderSystem, Color.Blue);
            _gradientBrush = new LinearGradientBrush(renderSystem);
            _gradientBrush.StartPoint = Vector2.Zero;
            _gradientBrush.EndPoint = Vector2.UnitX;
            _gradientBrush.GradientStops.Add(new GradientStop(0.0f, Color.Blue));
            _gradientBrush.GradientStops.Add(new GradientStop(1.0f, Color.Red));
            _radGradientBrush = new RadialGradientBrush(renderSystem);
            _radGradientBrush.GradientStops.Add(new GradientStop(0.0f, Color.Blue));
            _radGradientBrush.GradientStops.Add(new GradientStop(1.0f, Color.Red));

            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            _renderSystem = renderSystem;
            _bounds = bounds;

            _uiRenderTarget = new RenderTarget2D(renderSystem, _bounds.Width, _bounds.Height);
        }
        
        public Rectangle Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                BoundsUpdated();
            }
        }
        
        public void ProcessVisibleSet(IRenderContext context)
        {
            context.SetRenderTarget(_uiRenderTarget);

            _spriteBatch.Begin(context);
            _spriteBatch.DrawString(_font, "Hello, world!", Vector2.Zero, Color.Green);
            _spriteBatch.End();


            context.SetRenderTarget(null);

            _spriteBatch.Begin(context);
            _spriteBatch.Draw(_uiRenderTarget, _bounds, Color.White);
            _spriteBatch.End();

            _solidBrush.Draw(context, _spriteBatch, new Rectangle(30, 300, 250, 250), Matrix4x4.Identity, 1.0f);
            _gradientBrush.Draw(context, _spriteBatch, new Rectangle(300, 300, 250, 250), Matrix4x4.Identity, 1.0f);
            _radGradientBrush.Draw(context, _spriteBatch, new Rectangle(570, 300, 250, 250), Matrix4x4.Identity, 1.0f);
        }

        private void BoundsUpdated()
        {
            _uiRenderTarget.Dispose();
            _uiRenderTarget = new RenderTarget2D(_renderSystem, Bounds.Width, Bounds.Height);
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
            }

            base.Dispose(isDisposing);
        }
    }
}
