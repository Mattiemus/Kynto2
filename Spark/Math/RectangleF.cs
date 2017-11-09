namespace Spark.Math
{
    public struct RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public RectangleF(Vector2 topLeft, float width, float height)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = width;
            Height = height;
        }
        
        public static RectangleF Empty => new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);

        public Size Size => new Size(Width, Height);
    }
}
