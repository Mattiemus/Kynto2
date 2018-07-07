namespace Spark.UI
{
    using System;

    using Math;

    public struct Thickness : IEquatable<Thickness>
    {
        private readonly float _left;
        private readonly float _top;
        private readonly float _right;
        private readonly float _bottom;

        public Thickness(float thickness)
        {
            _left = thickness;
            _top = thickness;
            _right = thickness;
            _bottom = thickness;
        }

        public Thickness(float left, float top, float right, float bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        public Thickness(Thickness thickness)
        {
            _left = thickness.Left;
            _top = thickness.Top;
            _right = thickness.Right;
            _bottom = thickness.Bottom;
        }

        public float Left => _left;

        public float Top => _top;

        public float Right => _right;

        public float Bottom => _bottom;

        public bool IsEmpty => 
            MathHelper.IsApproxZero(Left) && 
            MathHelper.IsApproxZero(Top) && 
            MathHelper.IsApproxZero(Right) && 
            MathHelper.IsApproxZero(Bottom);

        public override int GetHashCode()
        {
            return Left.GetHashCode() + 
                   Top.GetHashCode() + 
                   Right.GetHashCode() + 
                   Bottom.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Thickness)
            {
                return Equals((Thickness)obj);
            }

            return false;
        }

        public bool Equals(Thickness t)
        {
            return MathHelper.IsApproxEquals(Left, t.Left) && 
                   MathHelper.IsApproxEquals(Top, t.Top) && 
                   MathHelper.IsApproxEquals(Right, t.Right) && 
                   MathHelper.IsApproxEquals(Bottom, t.Bottom);
        }
    }
}
