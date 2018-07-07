namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;
    using Utilities;

    public abstract class Brush : Disposable, IEquatable<Brush>
    {
        private float _opacity;
        private Texture2D _brushTexture;

        protected Brush()
        {
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

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Opacity.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            Brush other = obj as Brush;
            if (other != null)
            {
                return Equals(other);
            }

            return false;
        }

        public virtual bool Equals(Brush other)
        {
            return other != null &&
                   MathHelper.IsApproxEquals(other.Opacity, Opacity);
        }

        internal Texture2D GetTexture(IRenderSystem renderSystem)
        {
            if (_brushTexture == null)
            {
                _brushTexture = CreateTexture(renderSystem);
            }

            return _brushTexture;
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
