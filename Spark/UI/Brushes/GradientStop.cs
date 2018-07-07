namespace Spark.UI
{
    using Math;

    public class GradientStop
    {
        public GradientStop()
        {
        }

        public GradientStop(float offset, Color color)
        {
            Offset = offset;
            Color = color;
        }

        public float Offset { get; set; }
        public Color Color { get; set; }

        public override bool Equals(object obj)
        {
            GradientStop gs = obj as GradientStop;
            return gs != null && 
                   MathHelper.IsApproxEquals(gs.Offset, Offset) &&
                   gs.Color == Color;
        }

        public override int GetHashCode()
        {
            return Offset.GetHashCode() + Color.GetHashCode();
        }
    }
}
