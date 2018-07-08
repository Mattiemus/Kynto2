namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;

    public sealed class SolidColorBrush : Brush
    {
        private Color _color;

        public SolidColorBrush()
        {
            _color = Color.TransparentBlack;
        }

        public SolidColorBrush(Color color)
        {
            _color = color;
        }
        
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                InvalidateTexture();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Color.GetHashCode();
                hash = (hash * 7) + base.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is SolidColorBrush solidBrush)
            {
                return Equals(solidBrush);
            }

            return false;
        }

        public override bool Equals(Brush other)
        {
            if (other is SolidColorBrush solidBrush)
            {
                return Color == solidBrush.Color &&
                       base.Equals(other);
            }

            return false;
        }

        protected override Texture2D CreateTexture(IRenderSystem renderSystem)
        {
            using (DataBuffer<Color> colorBuffer = new DataBuffer<Color>(Color))
            {
                return new Texture2D(renderSystem, 1, 1, SurfaceFormat.Color, colorBuffer);
            }
        }
    }
}
