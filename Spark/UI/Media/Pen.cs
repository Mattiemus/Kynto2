namespace Spark.UI.Media
{
    using Animation;

    public class Pen : Animatable
    {
        public Pen(Brush brush, float thickness)
        {
            Brush = brush;
            Thickness = thickness;
        }

        public Brush Brush { get; set; }

        public float Thickness { get; set; }
    }
}
