namespace Spark.UI
{
    public sealed class Pen
    {
        public Pen()
        {

        }

        public Pen(Brush brush, float thickness)
        {
            Brush = brush;
            Thickness = thickness;
        }

        public Brush Brush { get; set; }

        public float Thickness { get; set; }
    }
}
