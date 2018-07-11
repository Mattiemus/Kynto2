namespace Spark.Math
{
    using System;

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

        public static Size operator +(Size lhs, Thickness rhs)
        {
            return new Size(
                lhs.Width + rhs.Left + rhs.Right,
                lhs.Height + rhs.Top + rhs.Bottom);
        }

        public static Size operator -(Size lhs, Thickness rhs)
        {
            return new Size(
                Math.Max(lhs.Width - rhs.Left - rhs.Right, 0.0f),
                Math.Max(lhs.Height - rhs.Top - rhs.Bottom, 0.0f));
        }
    }
}
