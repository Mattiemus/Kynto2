namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Content;

    /// <summary>
    /// Represents an intersection result between a bounding volume and line, ray, or segment.
    /// </summary>
    /// <remarks>
    /// Three possible results can occur:
    /// <list type="number">
    /// <item>
    /// <description>No intersection (e.g. linea misses the volume completely)</description>
    /// </item>
    /// <item>
    /// <description>One point intersection (e.g. line origin is inside and it exits, or the line touches at exactly one point externally)</description>
    /// </item>
    /// <item>
    /// <description>Two point intersection (e.g. line enters and exits the volume</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundingIntersectionResult : IEquatable<BoundingIntersectionResult>, IFormattable, IPrimitiveValue
    {
        private int _count;
        private LineIntersectionResult? _closest;
        private LineIntersectionResult? _farthest;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingIntersectionResult"/> struct.
        /// </summary>
        /// <param name="record">An intersection record</param>
        public BoundingIntersectionResult(LineIntersectionResult record)
        {
            _closest = record;
            _farthest = null;
            _count = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingIntersectionResult"/> struct.
        /// </summary>
        /// <param name="first">First intersection record</param>
        /// <param name="second">Second intersection record</param>
        public BoundingIntersectionResult(LineIntersectionResult first, LineIntersectionResult second)
        {
            if (first.Distance <= second.Distance)
            {
                _closest = first;
                _farthest = second;
            }
            else
            {
                _closest = second;
                _farthest = first;
            }

            _count = 2;
        }

        /// <summary>
        /// Gets the number of intersections in the result (0, 1, or 2).
        /// </summary>
        public int IntersectionCount => _count;

        /// <summary>
        /// Gets the closest intersection record, if it exists.
        /// </summary>
        public LineIntersectionResult? ClosestIntersection => _closest;

        /// <summary>
        /// Gets the farthest intersection record, if it is exists.
        /// </summary>
        public LineIntersectionResult? FarthestIntersection => _farthest;

        /// <summary>
        /// Gets the intersection record by index. 0 = Closest, 1 = Farthest.
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <returns>The corresponding intersection record.</returns>
        public LineIntersectionResult this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return (index == 0) ? _closest.Value : _farthest.Value;
            }
        }

        /// <summary>
        /// Constructs a bounding intersection result from potentially two line intersection results.
        /// </summary>
        /// <param name="first">First result which may be null.</param>
        /// <param name="second">Second result which may be null.</param>
        /// <returns>Bounding intersection result.</returns>
        public static BoundingIntersectionResult FromResults(LineIntersectionResult? first, LineIntersectionResult? second)
        {
            FromResults(ref first, ref second, out BoundingIntersectionResult result);
            return result;
        }

        /// <summary>
        /// Constructs a bounding intersection result from potentially two line intersection results.
        /// </summary>
        /// <param name="first">First result which may be null.</param>
        /// <param name="second">Second result which may be null.</param>
        /// <param name="result">Bounding intersection result</param>
        public static void FromResults(ref LineIntersectionResult? first, ref LineIntersectionResult? second, out BoundingIntersectionResult result)
        {
            if (first == null && second == null)
            {
                result = new BoundingIntersectionResult();
            }
            else if (first != null && second == null)
            {
                result = new BoundingIntersectionResult(first.Value);
            }
            else if (first == null && second != null)
            {
                result = new BoundingIntersectionResult(second.Value);
            }
            else
            {
                result = new BoundingIntersectionResult(first.Value, second.Value);
            }
        }

        /// <summary>
        /// Tests equality between two bounding intersection results.
        /// </summary>
        /// <param name="a">First bounding intersection result</param>
        /// <param name="b">Second bounding intersection result</param>
        /// <returns>True if the results are equal, false otherwise.</returns>
        public static bool operator ==(BoundingIntersectionResult a, BoundingIntersectionResult b)
        {
            return a.Equals(ref b, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests inequality between two bounding intersection results.
        /// </summary>
        /// <param name="a">First bounding intersection result</param>
        /// <param name="b">Second bounding intersection result</param>
        /// <returns>True if the results are not equal, false otherwise.</returns>
        public static bool operator !=(BoundingIntersectionResult a, BoundingIntersectionResult b)
        {
            return !a.Equals(ref b, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is BoundingIntersectionResult)
            {
                BoundingIntersectionResult result = (BoundingIntersectionResult)obj;
                return Equals(ref result, MathHelper.ZeroTolerance);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the bounding intersection result and another.
        /// </summary>
        /// <param name="other">Other result to compare to</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(BoundingIntersectionResult other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the bounding intersection result and another.
        /// </summary>
        /// <param name="other">Other result to compare to</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(BoundingIntersectionResult other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the bounding intersection result and another.
        /// </summary>
        /// <param name="other">Other result to compare to</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref BoundingIntersectionResult other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the bounding intersection result and another.
        /// </summary>
        /// <param name="other">Other result to compare to</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref BoundingIntersectionResult other, float tolerance)
        {
            if (_count != other._count)
            {
                return false;
            }

            bool closestEqual = false;
            bool farthestEqual = false;

            if (_closest.HasValue && other._closest.HasValue)
            {
                LineIntersectionResult cR = other._closest.Value;
                closestEqual = _closest.Value.Equals(ref cR, tolerance);
            }
            else
            {
                closestEqual = !_closest.HasValue && !other._closest.HasValue;
            }

            if (_farthest.HasValue && other._farthest.HasValue)
            {
                LineIntersectionResult fR = other._farthest.Value;
                farthestEqual = _farthest.Value.Equals(ref fR, tolerance);
            }
            else
            {
                farthestEqual = !_farthest.HasValue && !other._farthest.HasValue;
            }

            return closestEqual && farthestEqual;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int closestHash = (_closest.HasValue) ? _closest.Value.GetHashCode() : 0;
                int farthestHash = (_farthest.HasValue) ? _farthest.Value.GetHashCode() : 0;
                return _count.GetHashCode() + closestHash + farthestHash;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString("G");
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format)
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return ToString();
            }

            return ToString("G", formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return ToString();
            }

            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(formatProvider, "{{Count: {0} HasClosest: {1} HasFarthest: {2}}}", new object[] { _count.ToString(format, formatProvider), _closest.HasValue.ToString(formatProvider), _farthest.HasValue.ToString(formatProvider) });
        }
        
        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("Count", _count);
            output.WriteNullable("Closest", _closest);
            output.WriteNullable("Farthest", _farthest);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            _count = input.ReadInt32();
            _closest = input.ReadNullable<LineIntersectionResult>();
            _farthest = input.ReadNullable<LineIntersectionResult>();
        }
    }
}
