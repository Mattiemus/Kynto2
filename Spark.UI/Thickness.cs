namespace Spark.UI
{
    using System;

    using Math;

    public struct Thickness : IEquatable<Thickness>
    {
        public Thickness(float thickness)
        {
            Left = thickness;
            Top = thickness;
            Right = thickness;
            Bottom = thickness;
        }

        public Thickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Thickness(Thickness thickness)
        {
            Left = thickness.Left;
            Top = thickness.Top;
            Right = thickness.Right;
            Bottom = thickness.Bottom;
        }

        public static Thickness Zero => new Thickness();

        public float Left { get; }

        public float Top { get; }

        public float Right { get; }

        public float Bottom { get; }

        public bool IsEmpty => MathHelper.IsApproxZero(Left) && 
                               MathHelper.IsApproxZero(Top) && 
                               MathHelper.IsApproxZero(Right) && 
                               MathHelper.IsApproxZero(Bottom);

        public bool Equals(Thickness t)
        {
            return MathHelper.IsApproxEquals(Left, t.Left) && 
                   MathHelper.IsApproxEquals(Top, t.Top) && 
                   MathHelper.IsApproxEquals(Right, t.Right) && 
                   MathHelper.IsApproxEquals(Bottom, t.Bottom);
        }
    }
}
