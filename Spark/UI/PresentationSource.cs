namespace Spark.UI
{
    using Media;
    using Math;

    public class PresentationSource
    {
        public Visual RootVisual { get; set; }

        public Size ClientSize { get; set; }
        
        public Vector2 PointToScreen(Vector2 p)
        {
            return Vector2.Zero;
        }
    }
}
