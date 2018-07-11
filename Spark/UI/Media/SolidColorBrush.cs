namespace Spark.UI.Media
{
    using Graphics;
    using Math;

    public class SolidColorBrush : Brush
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

        public override string ToString()
        {
            return Color.ToString();
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
