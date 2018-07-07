namespace Spark.UI
{
    using System;

    using Graphics;
    using Utilities;

    public abstract class Brush : Disposable
    {
        private readonly IRenderSystem _renderSystem;
        private float _opacity;
        private Texture2D _brushTexture;

        protected Brush()
            : this(SparkEngine.Instance.Services.GetService<IRenderSystem>())
        {
        }

        protected Brush(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            _renderSystem = renderSystem;

            _opacity = 1.0f;
        }

        public float Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                InvalidateTexture();
            }
        }

        internal Texture2D BrushTexture
        {
            get
            {
                if (_brushTexture == null)
                {
                    _brushTexture = CreateTexture(_renderSystem);
                }

                return _brushTexture;
            }
        }
        
        protected abstract Texture2D CreateTexture(IRenderSystem renderSystem);

        protected void InvalidateTexture()
        {
            _brushTexture?.Dispose();
            _brushTexture = null;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing && _brushTexture != null)
            {
                _brushTexture.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}
