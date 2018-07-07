namespace Spark.UI
{
    using Graphics;
    using Math;

    public sealed class SolidColorBrush : Brush
    {
        private Color _color;

        public SolidColorBrush()
        {
        }

        public SolidColorBrush(Color color)
        {
            _color = color;
        }

        public SolidColorBrush(IRenderSystem renderSystem)
            : base(renderSystem)
        {
        }

        public SolidColorBrush(IRenderSystem renderSystem, Color color)
            : base(renderSystem)
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

        protected override Texture2D CreateTexture(IRenderSystem renderSystem)
        {
            using (DataBuffer<Color> colorBuffer = new DataBuffer<Color>(Color))
            {
                return new Texture2D(renderSystem, 1, 1, SurfaceFormat.Color, colorBuffer);
            }
        }
    }
}
