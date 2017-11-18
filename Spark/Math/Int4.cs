namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    using Content;

    /// <summary>
    /// Defines a four dimensional vector where each component is a 32-bit integer.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Int4 : IEquatable<Int4>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// X component of the vector.
        /// </summary>
        public int X;

        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public int Y;

        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public int Z;

        /// <summary>
        /// W component of the vector.
        /// </summary>
        public int W;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Int4"/> struct.
        /// </summary>
        /// <param name="value">Value to initialize each component to</param>
        public Int4(int value)
        {
            X = Y = Z = W = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int4"/> struct.
        /// </summary>
        /// <param name="xy">Vector that contains XY components</param>
        /// <param name="z">Z component</param>
        /// <param name="w">W component</param>
        public Int4(Int2 xy, int z, int w)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int4"/> struct.
        /// </summary>
        /// <param name="xyz">Vector that contains XYZ components</param>
        /// <param name="w">W component</param>
        public Int4(Int3 xyz, int w)
        {
            X = xyz.X;
            Y = xyz.Y;
            Z = xyz.Z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int4"/> struct.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        /// <param name="w">W component</param>
        public Int4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Gets an <see cref="Int4"/> set to (0, 0, 0, 0).
        /// </summary>
        public static Int4 Zero => new Int4(0, 0, 0, 0);

        /// <summary>
        /// Gets an <see cref="Int4"/> set to (1, 1, 1, 1).
        /// </summary>
        public static Int4 One => new Int4(1, 1, 1, 1);

        /// <summary>
        /// Gets a unit <see cref="Int4"/> set to (1, 0, 0, 0).
        /// </summary>
        public static Int4 UnitX => new Int4(1, 0, 0, 0);

        /// <summary>
        /// Gets a unit <see cref="Int4"/> set to (0, 1, 0, 0).
        /// </summary>
        public static Int4 UnitY => new Int4(0, 1, 0, 0);

        /// <summary>
        /// Gets a unit <see cref="Int4"/> set to (0, 0, 1, 0).
        /// </summary>
        public static Int4 UnitZ => new Int4(0, 0, 1, 0);

        /// <summary>
        /// Gets a unit <see cref="Int4"/> set to (0, 0, 0, 1).
        /// </summary>
        public static Int4 UnitW => new Int4(0, 0, 0, 1);

        /// <summary>
        /// Gets the size of the <see cref="Int4"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Int4>();

        /// <summary>
        /// Gets or sets individual components of the vector in the order that the components are declared (XYZW).
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <returns>The value of the specified component.</returns>
        public int this[int index]
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
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Int4 Add(Int4 a, Int4 b)
        {
            Add(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Sum of the two vectors</param>
        public static void Add(ref Int4 a, ref Int4 b, out Int4 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
            result.W = a.W + b.W;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Int4 Subtract(Int4 a, Int4 b)
        {
            Subtract(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Difference of the two vectors</param>
        public static void Subtract(ref Int4 a, ref Int4 b, out Int4 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
            result.W = a.W - b.W;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Int4 Divide(Int4 a, Int4 b)
        {
            Divide(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <param name="result">Quotient of the two vectors</param>
        public static void Divide(ref Int4 a, ref Int4 b, out Int4 result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
            result.Z = a.Z / b.Z;
            result.W = a.W / b.W;
        }

        /// <summary>
        /// Divides the components of one vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Int4 Divide(Int4 value, int divisor)
        {
            Divide(ref value, divisor, out Int4 result);
            return result;
        }

        /// <summary>
        /// Divides the components of one vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <param name="result">Divided Vector</param>
        public static void Divide(ref Int4 value, int divisor, out Int4 result)
        {
            result.X = value.X / divisor;
            result.Y = value.Y / divisor;
            result.Z = value.Z / divisor;
            result.W = value.W / divisor;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Scaled vector</returns>
        public static Int4 Multiply(Int4 value, int scale)
        {
            Multiply(ref value, scale, out Int4 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <param name="result">Scaled vector</param>
        public static void Multiply(ref Int4 value, int scale, out Int4 result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
            result.Z = value.Z * scale;
            result.W = value.W * scale;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vectorr</returns>
        public static Int4 Multiply(Int4 a, Int4 b)
        {
            Multiply(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Multiplied vector</param>
        public static void Multiply(ref Int4 a, ref Int4 b, out Int4 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
            result.W = a.W * b.W;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <returns>Clamped vector</returns>
        public static Int4 Clamp(Int4 value, Int4 min, Int4 max)
        {
            Clamp(ref value, ref min, ref max, out Int4 result);
            return result;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">Clamped vector</param>
        public static void Clamp(ref Int4 value, ref Int4 min, ref Int4 max, out Int4 result)
        {
            int x = value.X;
            int y = value.Y;
            int z = value.Z;
            int w = value.W;

            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            w = (w > max.W) ? max.W : w;
            w = (w < min.W) ? min.W : w;

            result.X = x;
            result.Y = y;
            result.Z = z;
            result.W = w;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Minimum vector</returns>
        public static Int4 Min(Int4 a, Int4 b)
        {
            Min(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Minimum vector</param>
        public static void Min(ref Int4 a, ref Int4 b, out Int4 result)
        {
            result.X = (a.X < b.X) ? a.X : b.X;
            result.Y = (a.Y < b.Y) ? a.Y : b.Y;
            result.Z = (a.Z < b.Z) ? a.Z : b.Z;
            result.W = (a.W < b.W) ? a.W : b.W;
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Maximum vector</returns>
        public static Int4 Max(Int4 a, Int4 b)
        {
            Max(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Maximum vector</param>
        public static void Max(ref Int4 a, ref Int4 b, out Int4 result)
        {
            result.X = (a.X > b.X) ? a.X : b.X;
            result.Y = (a.Y > b.Y) ? a.Y : b.Y;
            result.Z = (a.Z > b.Z) ? a.Z : b.Z;
            result.W = (a.W > b.W) ? a.W : b.W;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Int4 Negate(Int4 value)
        {
            Negate(ref value, out Int4 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="result">Negated vector</param>
        public static void Negate(ref Int4 value, out Int4 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
        }

        /// <summary>
        /// Adds the two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Int4 operator +(Int4 a, Int4 b)
        {
            Add(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Int4 operator -(Int4 a, Int4 b)
        {
            Subtract(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Int4 operator -(Int4 value)
        {
            Negate(ref value, out Int4 result);
            return result;
        }

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vector</returns>
        public static Int4 operator *(Int4 a, Int4 b)
        {
            Multiply(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Multiplied vector</returns>
        public static Int4 operator *(Int4 value, int scale)
        {
            Multiply(ref value, scale, out Int4 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="scale">Amount to scale</param>
        /// <param name="value">Source vector</param>
        /// <returns>Multiplied vector</returns>
        public static Int4 operator *(int scale, Int4 value)
        {
            Multiply(ref value, scale, out Int4 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Int4 operator /(Int4 a, Int4 b)
        {
            Divide(ref a, ref b, out Int4 result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Int4 operator /(Int4 value, int divisor)
        {
            Divide(ref value, divisor, out Int4 result);
            return result;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Int4"/> to <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">Int4 value</param>
        /// <returns>Converted Vector4</returns>
        public static explicit operator Vector4(Int4 value)
        {
            return new Vector4(value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector4"/> to <see cref="Int4"/>.
        /// </summary>
        /// <param name="value">Vector4 value</param>
        /// <returns>Converted Int4</returns>
        public static explicit operator Int4(Vector4 value)
        {
            return new Int4((int)value.X, (int)value.Y, (int)value.Z, (int)value.W);
        }

        /// <summary>
        /// Checks equality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(Int4 a, Int4 b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Checks inequality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(Int4 a, Int4 b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Flips the signs of the components of the vector.
        /// </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
            W = -W;
        }

        /// <summary>
        /// Checks equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Other vector</param>
        /// <returns>True if the vectors are equal, false otherwise.</returns>
        public bool Equals(Int4 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Checks equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Other vector</param>
        /// <returns>True if the vectors are equal, false otherwise.</returns>
        public bool Equals(ref Int4 other)
        {
            return (X == other.X) && (Y == other.Y) && (Z == other.Z) && (W == other.W);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, False.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Int4)
            {
                return Equals((Int4)obj);
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
                return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
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
            return string.Format(formatProvider, "X: {0} Y: {1} Z: {2} W: {3}", new object[] { X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider), W.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
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
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            X = input.ReadInt32();
            Y = input.ReadInt32();
            Z = input.ReadInt32();
            W = input.ReadInt32();
        }
    }
}
