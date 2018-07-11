namespace Spark.Math
{
    using System.ComponentModel;
    
    [TypeConverter(typeof(ThicknessConverter))]
    public struct Thickness
    {
        public Thickness(float uniformLength)
            : this()
        {
            Left = Top = Right = Bottom = uniformLength;
        }

        public Thickness(float left, float top, float right, float bottom)
            : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public float Left { get; set; }

        public float Top { get; set; }

        public float Right { get; set; }

        public float Bottom { get; set; }

        internal bool IsEmpty => MathHelper.IsApproxZero(Left) && 
                                 MathHelper.IsApproxZero(Top) && 
                                 MathHelper.IsApproxZero(Right) && 
                                 MathHelper.IsApproxZero(Bottom);

        public override bool Equals(object obj)
        {
            if (obj is Thickness)
            {
                Thickness other = (Thickness)obj;
                return MathHelper.IsApproxEquals(Left, other.Left) &&
                       MathHelper.IsApproxEquals(Top, other.Top) &&
                       MathHelper.IsApproxEquals(Right, other.Right) &&
                       MathHelper.IsApproxEquals(Bottom, other.Bottom);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + Left.GetHashCode();
                hash = (hash * 23) + Top.GetHashCode();
                hash = (hash * 23) + Right.GetHashCode();
                hash = (hash * 23) + Bottom.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Thickness a, Thickness b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Thickness a, Thickness b)
        {
            return !a.Equals(b);
        }

        public static Thickness operator +(Thickness a, Thickness b)
        {
            return new Thickness(
                a.Left + b.Left,
                a.Top + b.Top,
                a.Right + b.Right,
                a.Bottom + b.Bottom);
        }
    }
}
