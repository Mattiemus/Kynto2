namespace Spark.UI.Media
{
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
            set => _color = value;
        }

        public override string ToString()
        {
            return Color.ToString();
        }
    }
}
