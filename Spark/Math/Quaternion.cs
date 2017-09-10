namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a 4 dimensional vector that represents a rotation, where the XYZ components is the axis that an object is rotated about 
    /// by the angle theta. The W component is equal to cos(theta/2.0).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Quaternion : IEquatable<Quaternion>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// X component of the vector part of the quaternion.
        /// </summary>
        public float X;

        /// <summary>
        /// Y component of the vector part of the quaternion.
        /// </summary>
        public float Y;

        /// <summary>
        /// Z component of the vector part of the quaternion.
        /// </summary>
        public float Z;

        /// <summary>
        /// Rotation component of the quaternion.
        /// </summary>
        public float W;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="xyz">Vector part</param>
        /// <param name="w">Rotation component</param>
        public Quaternion(Vector3 xyz, float w)
        {
            X = xyz.X;
            Y = xyz.Y;
            Z = xyz.Z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="xy">Vector part containing XY components</param>
        /// <param name="z">Z component of the vector part</param>
        /// <param name="w">Rotation component</param>
        public Quaternion(Vector3 xy, float z, float w)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new Quaternion.
        /// </summary>
        /// <param name="x">X component of the vector part</param>
        /// <param name="y">Y component of the vector part</param>
        /// <param name="z">Z component of the vector part</param>
        /// <param name="w">Rotation component</param>
        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Gets the identity <see cref="Quaternion"/> set to (0, 0, 0, 1).
        /// </summary>
        public static Quaternion Identity => new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets a <see cref="Quaternion"/> set to (1, 1, 1, 1).
        /// </summary>
        public static Quaternion One => new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Gets the size of the <see cref="Quaternion"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Quaternion>();

        /// <summary>
        /// Gets if the quaternion is equal to the identity quaternion.
        /// </summary>
        public bool IsIdentity => Equals(Identity);

        /// <summary>
        /// Gets whether the quaternion is normalized or not.
        /// </summary>
        public bool IsNormalized => MathHelper.IsApproxEquals(LengthSquared(), 1.0f);

        /// <summary>
        /// Gets whether any of the components of the quaternion are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z) || float.IsNaN(W);

        /// <summary>
        /// Gets whether any of the components of the quaternion are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(X) || float.IsInfinity(Y) || float.IsInfinity(Z) || float.IsInfinity(W);

        /// <summary>
        /// Gets the angle of the quaternion.
        /// </summary>
        public Angle Angle
        {
            get
            {
                float length = (X * X) + (Y * Y) + (Z * Z);
                if (length < MathHelper.ZeroTolerance)
                {
                    return Angle.Zero;
                }

                return Angle.FromRadians((float)(2.0 * Math.Acos(MathHelper.Clamp(W, -1.0f, 1.0f))));
            }
        }

        /// <summary>
        /// Gets the axis of the quaternion.
        /// </summary>
        public Vector3 Axis
        {
            get
            {
                float length = (X * X) + (Y * Y) + (Z * Z);
                if (length < MathHelper.ZeroTolerance)
                {
                    return Vector3.UnitX;
                }

                float invLength = 1.0f / length;                
                return new Vector3(X * invLength, Y * invLength, Z * invLength);
            }
        }

        /// <summary>
        /// Gets or sets individual components of the quaternion in the order that the components are declared (XYZW).
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
                    case 3:
                        return W;
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
                    case 3:
                        W = value;
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
            return (X * X) + (Y * Y) + (Z * Z) + (W * W);
        }

        /// <summary>
        /// Compute the length (magnitude) of the quaternion.
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
                return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
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
        /// Tests equality between the quaternion and another quaternion.
        /// </summary>
        /// <param name="other">Quaternion to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return MathHelper.IsApproxEquals(other.X, X) && 
                MathHelper.IsApproxEquals(other.Y, Y) && 
                MathHelper.IsApproxEquals(other.Z, Z) && 
                MathHelper.IsApproxEquals(other.W, W);
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
                Z.ToString(format, formatProvider),
                W.ToString(format, formatProvider)
            };

            return string.Format(formatProvider, "X: {0} Y: {1} Z: {2} W: {3}", components);
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
            output.Write("W", W);
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
            W = input.ReadSingle();
        }

        /// <summary>
        /// Determines if this <see cref="Quaternion"/> is equal to another
        /// </summary>
        /// <param name="lhs">Left side operand</param>
        /// <param name="rhs">Right side operand</param>
        /// <returns>True if the values are equal, false if otherwise</returns>
        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines if this <see cref="Quaternion"/> is not equal to another
        /// </summary>
        /// <param name="lhs">Left side operand</param>
        /// <param name="rhs">Right side operand</param>
        /// <returns>True if the values are not equal, false if otherwise</returns>
        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
