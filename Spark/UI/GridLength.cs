namespace Spark.UI
{
    using System;
    using System.ComponentModel;

    using Math;

    [TypeConverter(typeof(GridLengthConverter))]
    public struct GridLength : IEquatable<GridLength>
    {
        private readonly GridUnitType _type;
        private readonly float _value;

        public GridLength(float value)
            : this(value, GridUnitType.Pixel)
        {
        }

        public GridLength(float value, GridUnitType type)
        {
            if (value < 0 || float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException("Invalid value", nameof(value));
            }

            if (type < GridUnitType.Auto || type > GridUnitType.Star)
            {
                throw new ArgumentException("Invalid value", nameof(type));
            }

            _type = type;
            _value = value;
        }

        public static GridLength Auto => new GridLength(0.0f, GridUnitType.Auto);

        public GridUnitType GridUnitType => _type;

        public bool IsAbsolute => _type == GridUnitType.Pixel;

        public bool IsAuto => _type == GridUnitType.Auto;

        public bool IsStar => _type == GridUnitType.Star;

        public float Value => _value;

        public static bool operator ==(GridLength a, GridLength b)
        {
            return (a.IsAuto && b.IsAuto) || (MathHelper.IsApproxEquals(a._value, b._value) && a._type == b._type);
        }

        public static bool operator !=(GridLength gl1, GridLength gl2)
        {
            return !(gl1 == gl2);
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (!(o is GridLength))
            {
                return false;
            }

            return this == (GridLength)o;
        }

        public bool Equals(GridLength gridLength)
        {
            return this == gridLength;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode() ^ _type.GetHashCode();
        }

        public override string ToString()
        {
            if (IsAuto)
            {
                return "Auto";
            }

            string s = _value.ToString();
            return IsStar ? s + "*" : s;
        }
    }
}
