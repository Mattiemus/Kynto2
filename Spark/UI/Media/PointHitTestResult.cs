namespace Spark.UI.Media
{
    using Math;

    public class PointHitTestResult : HitTestResult
    {
        public PointHitTestResult(Visual visualHit, Vector2 pointHit)
        {
            VisualHit = visualHit;
            PointHit = pointHit;
        }

        public Vector2 PointHit { get; private set; }
    }
}
