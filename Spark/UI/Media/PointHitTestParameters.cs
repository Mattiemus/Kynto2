namespace Spark.UI.Media
{
    using Math;

    public class PointHitTestParameters : HitTestParameters
    {
        public PointHitTestParameters(Vector2 point)
        {
            HitPoint = point;
        }

        public Vector2 HitPoint { get; private set; }
    }
}
