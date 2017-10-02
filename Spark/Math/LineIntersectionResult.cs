namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Content;

    /// <summary>
    /// Represents an intersection result from a line-object, ray-object, or segment-object test. The object may be a simple bounding volume
    /// or a complex triangle mesh.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LineIntersectionResult : IEquatable<LineIntersectionResult>, IComparable<LineIntersectionResult>, IFormattable, IPrimitiveValue
    {
        private Vector3 _point;
        private float _distance;
        private Vector3? _normal;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineIntersectionResult"/> struct.
        /// </summary>
        /// <param name="point">Point of intersection</param>
        /// <param name="distance">Distance from the ray/line's origin.</param>
        public LineIntersectionResult(Vector3 point, float distance)
        {
            _point = point;
            _distance = distance;

            _normal = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineIntersectionResult"/> struct.
        /// </summary>
        /// <param name="point">Point of intersection</param>
        /// <param name="distance">Distance from the ray/line's origin.</param>
        /// <param name="normal">Normal vector at the point of intersection.</param>
        public LineIntersectionResult(Vector3 point, float distance, Vector3 normal)
        {
            _point = point;
            _distance = distance;

            _normal = normal;
        }

        /// <summary>
        /// Gets the point of intersection.
        /// </summary>
        public Vector3 Point => _point;

        /// <summary>
        /// Gets the distance from the point of intersection to line origin (if segment, usually the Start point). This is used to sort
        /// intersections records.
        /// </summary>
        public float Distance => _distance;

        /// <summary>
        /// Gets the optional normal vector at the point of intersection.
        /// </summary>
        public Vector3? Normal => _normal;

        /// <summary>
        /// Tests if the first intersection result's distance is greater than the second.
        /// </summary>
        /// <param name="a">First intersection result</param>
        /// <param name="b">Second intersection result</param>
        /// <returns>True if the first distance is greater. false otherwise.</returns>
        public static bool operator >(LineIntersectionResult a, LineIntersectionResult b)
        {
            return a._distance > b._distance;
        }

        /// <summary>
        /// Tests if the first intersection result's distance is greater than or equal to the second.
        /// </summary>
        /// <param name="a">First intersection result</param>
        /// <param name="b">Second intersection result</param>
        /// <returns>True if the first distance is greater than or equal, false otherwise.</returns>
        public static bool operator >=(LineIntersectionResult a, LineIntersectionResult b)
        {
            return a._distance >= b._distance;
        }

        /// <summary>
        /// Tests if the first intersection result's distance is less than the second.
        /// </summary>
        /// <param name="a">First intersection result</param>
        /// <param name="b">Second intersection result</param>
        /// <returns>True if the first distance is less than, false otherwise.</returns>
        public static bool operator <(LineIntersectionResult a, LineIntersectionResult b)
        {
            return a._distance < b._distance;
        }

        /// <summary>
        /// Tests if the first intersection result's distance is less than or equal to the second.
        /// </summary>
        /// <param name="a">First intersection result</param>
        /// <param name="b">Second intersection result</param>
        /// <returns>True if the first distance is less than or equal, false otherwise.</returns>
        public static bool operator <=(LineIntersectionResult a, LineIntersectionResult b)
        {
            return a._distance <= b._distance;
        }

        /// <summary>
        /// Tests equality between two intersection results.
        /// </summary>
        /// <param name="a">First intersection</param>
        /// <param name="b">Second intersection</param>
        /// <returns>True if the two intersections are equal, false otherwise.</returns>
        public static bool operator ==(LineIntersectionResult a, LineIntersectionResult b)
        {
            return a.Equals(ref b, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests inequality between two intersection results.
        /// </summary>
        /// <param name="a">First intersection</param>
        /// <param name="b">Second intersection</param>
        /// <returns>True if the two intersections are not equal, false otherwise.</returns>
        public static bool operator !=(LineIntersectionResult a, LineIntersectionResult b)
        {
            return !a.Equals(ref b, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        /// Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. 
        /// Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(LineIntersectionResult other)
        {
            if (MathHelper.IsApproxEquals(_distance, other._distance))
            {
                return 0;
            }

            if (_distance < other._distance)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is LineIntersectionResult)
            {
                LineIntersectionResult result = (LineIntersectionResult)obj;
                return Equals(ref result, MathHelper.ZeroTolerance);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the intersection result and another.
        /// </summary>
        /// <param name="other">Other result to test</param>
        /// <returns>True if the results are equal, false otherwise.</returns>
        public bool Equals(LineIntersectionResult other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the intersection result and another.
        /// </summary>
        /// <param name="other">Other result to test</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the results are equal, false otherwise.</returns>
        public bool Equals(LineIntersectionResult other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the intersection result and another.
        /// </summary>
        /// <param name="other">Other result to test</param>
        /// <returns>True if the results are equal, false otherwise.</returns>
        public bool Equals(ref LineIntersectionResult other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the intersection result and another.
        /// </summary>
        /// <param name="other">Other result to test</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the results are equal, false otherwise.</returns>
        public bool Equals(ref LineIntersectionResult other, float tolerance)
        {
            if (!_point.Equals(ref other._point, tolerance) || !MathHelper.IsApproxEquals(_distance, other._distance, tolerance))
            {
                return false;
            }

            bool normalsEqual = false;
            bool trianglesEqual = false;

            if (_normal.HasValue && other._normal.HasValue)
            {
                Vector3 n = other._normal.Value;
                normalsEqual = _normal.Value.Equals(ref n, tolerance);
            }
            else
            {
                normalsEqual = !_normal.HasValue && !other._normal.HasValue;
            }

            return normalsEqual && trianglesEqual;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int normalHash = _normal.HasValue ? _normal.Value.GetHashCode() : 0;
                return _point.GetHashCode() + _distance.GetHashCode() + normalHash;
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

            return string.Format(formatProvider, "Point: {0} Distance: {1} Normal: {2}", new object[] { _point.ToString(format, formatProvider), _distance.ToString(format, formatProvider), (_normal.HasValue) ? _normal.Value.ToString(format, formatProvider) : "null" });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("Point", _point);
            output.Write("Distance", _distance);
            output.WriteNullable("Normal", _normal);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            _point = input.Read<Vector3>();
            _distance = input.ReadSingle();
            _normal = input.ReadNullable<Vector3>();
        }
    }
}
