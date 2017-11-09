namespace Spark.Math
{
    public struct Size
    {
        public float Width;

        public float Height;

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public static Size Empty => new Size(0.0f, 0.0f);
    }
}
