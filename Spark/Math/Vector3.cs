namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a three dimensional vector of 32-bit floats.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector3 : IEquatable<Vector3>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// X component of the vector.
        /// </summary>
        public float X;

        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public float Y;

        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public float Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> struct.
        /// </summary>
        /// <param name="value">Value to initialize each component to</param>
        public Vector3(float value)
        {
            X = Y = Z = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> struct.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> set to (0, 0, 0).
        /// </summary>
        public static Vector3 Zero => new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a <see cref="Vector3"/> set to (1, 1, 1).
        /// </summary>
        public static Vector3 One => new Vector3(1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector3"/> set to (1, 0, 0).
        /// </summary>
        public static Vector3 UnitX => new Vector3(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector3"/> set to (0, 1, 0).
        /// </summary>
        public static Vector3 UnitY => new Vector3(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector3"/> set to (0, 0, 1).
        /// </summary>
        public static Vector3 UnitZ => new Vector3(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets the size of the <see cref="Vector3"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Vector3>();

        /// <summary>
        /// Gets whether the vector is normalized or not.
        /// </summary>
        public bool IsNormalized => MathHelper.IsApproxEquals(LengthSquared(), 1.0f);

        /// <summary>
        /// Gets whether any of the components of the vector are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z);

        /// <summary>
        /// Gets whether any of the components of the vector are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(X) || float.IsInfinity(Y) || float.IsInfinity(Z);

        /// <summary>
        /// Gets or sets individual components of the vector in the order that the components are declared (XYZ).
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <returns>The value of the specified component.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
        }

        /// <summary>
        /// Compute the length (magnitude) squared of the vector.
        /// </summary>
        /// <returns>Length squared</returns>
        public float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        /// <summary>
        /// Compute the length (magnitude) of the vector.
        /// </summary>
        /// <returns>Length</returns>
        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector4)
            {
                return Equals((Vector4)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Vector3 other)
        {
            return MathHelper.IsApproxEquals(other.X, X) && 
                MathHelper.IsApproxEquals(other.Y, Y) && 
                MathHelper.IsApproxEquals(other.Z, Z);
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

            string[] components = new[]
{
                X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider),
                Z.ToString(format, formatProvider)
            };

            return string.Format(formatProvider, "X: {0} Y: {1} Z: {2}", components);
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("X", X);
            output.Write("Y", Y);
            output.Write("Z", Z);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            X = input.ReadSingle();
            Y = input.ReadSingle();
            Z = input.ReadSingle();
        }

        /// <summary>
        /// Determines if this <see cref="Vector3"/> is equal to another
        /// </summary>
        /// <param name="lhs">Left side operand</param>
        /// <param name="rhs">Right side operand</param>
        /// <returns>True if the values are equal, false if otherwise</returns>
        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines if this <see cref="Vector3"/> is not equal to another
        /// </summary>
        /// <param name="lhs">Left side operand</param>
        /// <param name="rhs">Right side operand</param>
        /// <returns>True if the values are not equal, false if otherwise</returns>
        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
