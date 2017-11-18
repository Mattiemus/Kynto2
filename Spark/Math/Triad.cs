namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    using Content;

    /// <summary>
    /// Defines a grouping of three orthonormal axes that make a coordinate system.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Triad : IEquatable<Triad>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// X-Axis of the coordinate system.
        /// </summary>
        public Vector3 XAxis;

        /// <summary>
        /// Y-Axis of the coordinate system.
        /// </summary>
        public Vector3 YAxis;

        /// <summary>
        /// Z-Axis of the coordinate system.
        /// </summary>
        public Vector3 ZAxis;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triad"/> struct.
        /// </summary>
        /// <param name="xAxis">X-Axis of the coordinate system.</param>
        /// <param name="yAxis">Y-Axis of the coordinate system.</param>
        /// <param name="zAxis">Z-Axis of the coordinate system.</param>
        public Triad(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;
        }

        /// <summary>
        /// Creates a triad from a set of three axes.
        /// </summary>
        /// <param name="xAxis">X-Axis of the coordinate system.</param>
        /// <param name="yAxis">Y-Axis of the coordinate system.</param>
        /// <param name="zAxis">Z-Axis of the coordinate system.</param>
        /// <returns>The triad.</returns>
        public static Triad FromAxes(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            Triad triad;
            triad.XAxis = xAxis;
            triad.YAxis = yAxis;
            triad.ZAxis = zAxis;

            return triad;
        }

        /// <summary>
        /// Creates a triad from a set of three axes.
        /// </summary>
        /// <param name="xAxis">X-Axis of the coordinate system.</param>
        /// <param name="yAxis">Y-Axis of the coordinate system.</param>
        /// <param name="zAxis">Z-Axis of the coordinate system.</param>
        /// <param name="triad">The triad.</param>
        public static void FromAxes(ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, out Triad triad)
        {
            triad.XAxis = xAxis;
            triad.YAxis = yAxis;
            triad.ZAxis = zAxis;
        }

        /// <summary>
        /// Gets the standard or canonical basis where the XAxis = {1, 0, 0}, YAxis = {0, 1, 0}, ZAxis = {0, 0, 1}.
        /// </summary>
        public static Triad UnitAxes => new Triad(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);

        /// <summary>
        /// Gets the size of the <see cref="Triad"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Triad>();

        /// <summary>
        /// Gets or sets individual axes of the triad in the order of {X, Y, Z} axes.
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <returns>The value of the specified axis.</returns>
        public Vector3 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return XAxis;
                    case 1:
                        return YAxis;
                    case 2:
                        return ZAxis;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        XAxis = value;
                        break;
                    case 1:
                        YAxis = value;
                        break;
                    case 2:
                        ZAxis = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
        }

        /// <summary>
        /// Gets whether the axes are normalized.
        /// </summary>
        public bool IsNormalized => XAxis.IsNormalized && YAxis.IsNormalized && ZAxis.IsNormalized;

        /// <summary>
        /// Gets whether any of the axes are degenerate (equal to zero).
        /// </summary>
        public bool IsDegenerate => XAxis.Equals(Vector3.Zero) || YAxis.Equals(Vector3.Zero) || ZAxis.Equals(Vector3.Zero);

        /// <summary>
        /// Gets whether any of the components of the axes are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => XAxis.IsNaN || YAxis.IsNaN || ZAxis.IsNaN;

        /// <summary>
        /// Gets whether any of the components of the axes are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => XAxis.IsInfinity || YAxis.IsInfinity || ZAxis.IsInfinity;

        /// <summary>
        /// Computes an orthonormal basis from a single vector.
        /// </summary>
        /// <param name="zAxis">Z axis to form an orthonormal basis to.</param>
        /// <returns>Orthonormal basis</returns>
        public static Triad FromZComplementBasis(Vector3 zAxis)
        {
            FromZComplementBasis(ref zAxis, out Triad result);
            return result;
        }

        /// <summary>
        /// Computes an orthonormal basis from a single vector.
        /// </summary>
        /// <param name="zAxis">Z axis to form an orthonormal basis to.</param>
        /// <param name="result">Orthonormal basis</param>
        public static void FromZComplementBasis(ref Vector3 zAxis, out Triad result)
        {
            result.ZAxis = zAxis;
            Vector3.ComplementBasis(ref result.ZAxis, out result.XAxis, out result.YAxis);
        }

        /// <summary>
        /// Computes an orthonormal basis from a single vector.
        /// </summary>
        /// <param name="yAxis">Y axis to form an orthonormal basis to.</param>
        /// <returns>Orthonormal basis</returns>
        public static Triad FromYComplementBasis(Vector3 yAxis)
        {
            FromYComplementBasis(ref yAxis, out Triad result);
            return result;
        }

        /// <summary>
        /// Computes an orthonormal basis from a single vector.
        /// </summary>
        /// <param name="yAxis">Y axis to form an orthonormal basis to.</param>
        /// <param name="result">Orthonormal basis</param>
        public static void FromYComplementBasis(ref Vector3 yAxis, out Triad result)
        {
            result.YAxis = yAxis;
            Vector3.ComplementBasis(ref result.YAxis, out result.XAxis, out result.ZAxis);
        }

        /// <summary>
        /// Transfroms the specified axes by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="axes">Axes to transform.</param>
        /// <param name="rotation">Rotation quaternion.</param>
        /// <returns>Transformed axes.</returns>
        public static Triad Transform(Triad axes, Quaternion rotation)
        {
            Transform(ref axes, ref rotation, out Triad result);
            return result;
        }

        /// <summary>
        /// Transfroms the specified axes by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="axes">Axes to transform.</param>
        /// <param name="rotation">Rotation quaternion.</param>
        /// <param name="result">Transformed axes.</param>
        public static void Transform(ref Triad axes, ref Quaternion rotation, out Triad result)
        {
            Vector3.Transform(ref axes.XAxis, ref rotation, out result.XAxis);
            Vector3.Transform(ref axes.YAxis, ref rotation, out result.YAxis);
            Vector3.Transform(ref axes.ZAxis, ref rotation, out result.ZAxis);
        }

        /// <summary>
        /// Transforms the specified axes by the given <see cref="Matrix"/>. This does not use the fourth
        /// row and column, meaning the translation component is ignored. If scaling, then axes will need to be
        /// renormalized.
        /// </summary>
        /// <param name="axes">Axes to transform.</param>
        /// <param name="scaleRotation">Scale-Rotation matrix.</param>
        /// <returns>Transformed axes.</returns>
        public static Triad Transform(Triad axes, Matrix4x4 scaleRotation)
        {
            Transform(ref axes, ref scaleRotation, out Triad result);
            return result;
        }

        /// <summary>
        /// Transforms the specified axes by the given <see cref="Matrix"/>. This does not use the fourth
        /// row and column, meaning the translation component is ignored. If scaling, then axes will need to be
        /// renormalized.
        /// </summary>
        /// <param name="axes">Axes to transform.</param>
        /// <param name="scaleRotation">Scale-Rotation matrix.</param>
        /// <param name="result">Transformed axes.</param>
        public static void Transform(ref Triad axes, ref Matrix4x4 scaleRotation, out Triad result)
        {
            Vector3.TransformNormal(ref axes.XAxis, ref scaleRotation, out result.XAxis);
            Vector3.TransformNormal(ref axes.YAxis, ref scaleRotation, out result.YAxis);
            Vector3.TransformNormal(ref axes.ZAxis, ref scaleRotation, out result.ZAxis);
        }

        /// <summary>
        /// Normalizes the axes.
        /// </summary>
        /// <param name="axes">Axes to normalize.</param>
        /// <returns>Normalized axes.</returns>
        public static Triad Normalize(Triad axes)
        {
            Triad result;
            Normalize(ref axes, out result);
            return result;
        }

        /// <summary>
        /// Normalizes the axes.
        /// </summary>
        /// <param name="axes">Axes to normalize.</param>
        /// <param name="result">Normalized axes.</param>
        public static void Normalize(ref Triad axes, out Triad result)
        {
            Vector3.Normalize(ref axes.XAxis, out result.XAxis);
            Vector3.Normalize(ref axes.YAxis, out result.YAxis);
            Vector3.Normalize(ref axes.ZAxis, out result.ZAxis);
        }

        /// <summary>
        /// Normalizes the axes.
        /// </summary>
        public void Normalize()
        {
            XAxis.Normalize();
            YAxis.Normalize();
            ZAxis.Normalize();
        }

        /// <summary>
        /// Get individual axes of the triad in the order of {X, Y, Z} axes.
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <param name="axis">The value of the specified axis.</param>
        public void GetAxis(int index, out Vector3 axis)
        {
            switch (index)
            {
                case 0:
                    axis = XAxis;
                    break;
                case 1:
                    axis = YAxis;
                    break;
                case 2:
                    axis = ZAxis;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
            }
        }

        /// <summary>
        /// Set individual axes of the triad in the order of {X, Y, Z} axes.
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <param name="axis">The value of the specified axis to set.</param>
        public void SetAxis(int index, ref Vector3 axis)
        {
            switch (index)
            {
                case 0:
                    XAxis = axis;
                    break;
                case 1:
                    YAxis = axis;
                    break;
                case 2:
                    ZAxis = axis;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
            }
        }

        /// <summary>
        /// Computes the determinant of the basis.
        /// </summary>
        /// <returns>Determinant</returns>
        public float ComputeDeterminant()
        {
            float m11 = XAxis.X;
            float m12 = XAxis.Y;
            float m13 = XAxis.Z;
            float m21 = YAxis.X;
            float m22 = YAxis.Y;
            float m23 = YAxis.Z;
            float m31 = ZAxis.X;
            float m32 = ZAxis.Y;
            float m33 = ZAxis.Z;

            float h1 = m33;
            float h2 = m32;
            float h4 = m31;

            return ((((m11 * (((m22 * h1) - (m23 * h2)))) - (m12 * (((m21 * h1) - (m23 * h4))))) + (m13 * (((m21 * h2) - (m22 * h4))))));
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is Triad)
            {
                return Equals((Triad)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between this triad and another.
        /// </summary>
        /// <param name="other">triad</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(Triad other)
        {
            return XAxis.Equals(ref other.XAxis) && YAxis.Equals(ref other.YAxis) && ZAxis.Equals(ref other.ZAxis);
        }

        /// <summary>
        /// Tests equality between this triad and another.
        /// </summary>
        /// <param name="other">triad</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(Triad other, float tolerance)
        {
            return XAxis.Equals(ref other.XAxis, tolerance) && YAxis.Equals(ref other.YAxis, tolerance) && ZAxis.Equals(ref other.ZAxis, tolerance);
        }

        /// <summary>
        /// Tests equality between this triad and another.
        /// </summary>
        /// <param name="other">triad</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(ref Triad other)
        {
            return XAxis.Equals(ref other.XAxis) && YAxis.Equals(ref other.YAxis) && ZAxis.Equals(ref other.ZAxis);
        }

        /// <summary>
        /// Tests equality between this triad and another.
        /// </summary>
        /// <param name="other">triad</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(ref Triad other, float tolerance)
        {
            return XAxis.Equals(ref other.XAxis, tolerance) && YAxis.Equals(ref other.YAxis, tolerance) && ZAxis.Equals(ref other.ZAxis, tolerance);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return XAxis.GetHashCode() + YAxis.GetHashCode() + ZAxis.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
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

            return string.Format(formatProvider, "XAxis: [{0}], YAxis: [{1}], ZAxis: [{2}]", new object[] { XAxis.ToString(format, formatProvider), YAxis.ToString(format, formatProvider), ZAxis.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Reads the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            XAxis = input.Read<Vector3>();
            YAxis = input.Read<Vector3>();
            ZAxis = input.Read<Vector3>();
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("XAxis", XAxis);
            output.Write("YAxis", YAxis);
            output.Write("ZAxis", ZAxis);
        }
    }
}
