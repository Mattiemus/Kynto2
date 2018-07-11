namespace Spark.UI
{
    using System.ComponentModel;

    [TypeConverter(typeof(CornerRadiusConverter))]
    public struct CornerRadius
    {
        public CornerRadius(float uniformRadius)
            : this()
        {
            BottomLeft = BottomRight = TopLeft = TopRight = uniformRadius;
        }

        public CornerRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
            : this()
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }

        public float BottomLeft { get; set; }

        public float BottomRight { get; set; }

        public float TopLeft { get; set; }

        public float TopRight { get; set; }
    }
}
