namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines an infinite plane at an origin with a normal. The origin of the plane is represented by a distance value from zero, along the normal vector.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Plane : IEquatable<Plane>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// The normal vector of the plane.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The plane constant, which is the (negative) distance from the origin (0, 0, 0) to the origin of the plane along its normal.
        /// </summary>
        public float D;

        /// <summary>
        /// Constructs a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="x">X component of the plane normal</param>
        /// <param name="y">Y component of the plane normal</param>
        /// <param name="z">Z component of the plane normal</param>
        /// <param name="d">Plane constant, (negative) distance from origin (0, 0, 0) to plane origin.</param>
        public Plane(float x, float y, float z, float d)
        {
            Normal = new Vector3(x, y, z);
            D = d;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="normal">Plane normal</param>
        /// <param name="d">Plane constant, (negative) distance from origin (0, 0, 0) to plane origin.</param>
        public Plane(Vector3 normal, float d)
        {
            Normal = normal;
            D = d;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="plane">XYZ contains the plane normal vector, and W contains the plane constant.</param>
        public Plane(Vector4 plane)
        {
            Normal = new Vector3(plane.X, plane.Y, plane.Z);
            D = plane.W;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="normal">Plane normal.</param>
        /// <param name="origin">Plane origin.</param>
        public Plane(Vector3 normal, Vector3 origin)
        {
            Vector3.Dot(ref normal, ref origin, out float dot);

            Normal = normal;
            D = -dot;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Plane"/> struct from three points.
        /// </summary>
        /// <param name="p1">First position</param>
        /// <param name="p2">Second position</param>
        /// <param name="p3">Third position</param>
        public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Compute first vector
            Vector3 v1;
            v1.X = p2.X - p1.X;
            v1.Y = p2.Y - p1.Y;
            v1.Z = p2.Z - p1.Z;

            // Compute second vector
            Vector3 v2;
            v2.X = p3.X - p1.X;
            v2.Y = p3.Y - p1.Y;
            v2.Z = p3.Z - p1.Z;

            // Take cross product
            Vector3.NormalizedCross(ref v1, ref v2, out Normal);

            D = -((p1.X * Normal.X) + (p1.Y * Normal.Y) + (p1.Z * Normal.Z));
        }

        /// <summary>
        /// Gets a unit <see cref="Plane"/> that has its origin at zero and normal set to (1, 0, 0).
        /// </summary>
        public static Plane UnitX => new Plane(Vector3.UnitX, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Plane"/> that has its origin at zero and normal set to (0, 1, 0).
        /// </summary>
        public static Plane UnitY => new Plane(Vector3.UnitY, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Plane"/> that has its origin at zero and normal set to (0, 0, 1).
        /// </summary>
        public static Plane UnitZ => new Plane(Vector3.UnitZ, 0.0f);

        /// <summary>
        /// Gets or sets the plane's origin.
        /// </summary>
        public Vector3 Origin
        {
            get
            {
                Vector3.Multiply(ref Normal, -D, out Vector3 origin);
                return origin;
            }
            set
            {
                Vector3.Dot(ref Normal, ref value, out float dot);
                D = -dot;
            }
        }

        /// <summary>
        /// Gets the size of Plane structure in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Plane>();

        /// <summary>
        /// Gets if the plane is degenerate (normal is zero).
        /// </summary>
        public bool IsDegenerate => Normal.Equals(Vector3.Zero);

        /// <summary>
        /// Gets whether any of the components of the plane are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(D) || Normal.IsNaN;

        /// <summary>
        /// Gets whether any of the components of the plane are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsNegativeInfinity(D) || float.IsPositiveInfinity(D) || Normal.IsInfinity;







        /// <summary>
        /// Normalizes the plane's normal to be unit length.
        /// </summary>
        public void Normalize()
        {
            float lengthSquared = Normal.LengthSquared();
            if (!MathHelper.IsApproxZero(lengthSquared))
            {
                float invLength = 1.0f / (float)Math.Sqrt(lengthSquared);
                Vector3.Multiply(ref Normal, invLength, out Normal);
                D *= invLength;
            }
        }










        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Plane other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(ref Plane other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(Plane other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Checks inequality between the plane and another plane.
        /// </summary>
        /// <param name="other">Other plane</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the planes are equal, false otherwise.</returns>
        public bool Equals(ref Plane other, float tolerance)
        {
            return Normal.Equals(ref other.Normal, tolerance) &&
                   (Math.Abs(D - other.D) <= tolerance);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Plane)
            {
                return Equals((Plane)obj);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Normal.GetHashCode() + D.GetHashCode();
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

            return string.Format(formatProvider, "Normal: {0}, D: {1}, Origin: {2}", new object[] { Normal.ToString(format, formatProvider), D.ToString(format, formatProvider), Origin.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("Normal", Normal);
            output.Write("D", D);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            Normal = input.Read<Vector3>();
            D = input.ReadSingle();
        }
    }
}
