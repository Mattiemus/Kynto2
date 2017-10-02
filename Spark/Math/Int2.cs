namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a two dimensional vector where each component is a 32-bit integer.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Int2 : IEquatable<Int2>, IFormattable, IPrimitiveValue
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
        /// Initializes a new instance of the <see cref="Int2"/> struct.
        /// </summary>
        /// <param name="value">Value to initialize each component to</param>
        public Int2(int value)
        {
            X = Y = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int2"/> struct.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets an <see cref="Int2"/> set to (0, 0).
        /// </summary>
        public static Int2 Zero => new Int2(0, 0);

        /// <summary>
        /// Gets an <see cref="Int2"/> set to (1, 1).
        /// </summary>
        public static Int2 One => new Int2(1, 1);

        /// <summary>
        /// Gets a unit <see cref="Int2"/> set to (1, 0).
        /// </summary>
        public static Int2 UnitX => new Int2(1, 0);

        /// <summary>
        /// Gets a unit <see cref="Int2"/> set to (0, 1).
        /// </summary>
        public static Int2 UnitY => new Int2(0, 1);

        /// <summary>
        /// Gets the size of the <see cref="Int2"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Int2>();

        /// <summary>
        /// Gets or sets individual components of the vector in the order that the components are declared (XY).
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
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
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
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
                }
            }
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Int2 Add(Int2 a, Int2 b)
        {
            Add(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Sum of the two vectors</param>
        public static void Add(ref Int2 a, ref Int2 b, out Int2 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Int2 Subtract(Int2 a, Int2 b)
        {
            Subtract(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Difference of the two vectors</param>
        public static void Subtract(ref Int2 a, ref Int2 b, out Int2 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Int2 Divide(Int2 a, Int2 b)
        {
            Divide(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <param name="result">Quotient of the two vectors</param>
        public static void Divide(ref Int2 a, ref Int2 b, out Int2 result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
        }

        /// <summary>
        /// Divides the components of one vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Int2 Divide(Int2 value, int divisor)
        {
            Divide(ref value, divisor, out Int2 result);
            return result;
        }

        /// <summary>
        /// Divides the components of one vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <param name="result">Divided Vector</param>
        public static void Divide(ref Int2 value, int divisor, out Int2 result)
        {
            result.X = value.X / divisor;
            result.Y = value.Y / divisor;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Scaled vector</returns>
        public static Int2 Multiply(Int2 value, int scale)
        {
            Multiply(ref value, scale, out Int2 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <param name="result">Scaled vector</param>
        public static void Multiply(ref Int2 value, int scale, out Int2 result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vectorr</returns>
        public static Int2 Multiply(Int2 a, Int2 b)
        {
            Multiply(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Multiplied vector</param>
        public static void Multiply(ref Int2 a, ref Int2 b, out Int2 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <returns>Clamped vector</returns>
        public static Int2 Clamp(Int2 value, Int2 min, Int2 max)
        {
            Clamp(ref value, ref min, ref max, out Int2 result);
            return result;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">Clamped vector</param>
        public static void Clamp(ref Int2 value, ref Int2 min, ref Int2 max, out Int2 result)
        {
            int x = value.X;
            int y = value.Y;

            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            result.X = x;
            result.Y = y;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Minimum vector</returns>
        public static Int2 Min(Int2 a, Int2 b)
        {
            Min(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Minimum vector</param>
        public static void Min(ref Int2 a, ref Int2 b, out Int2 result)
        {
            result.X = (a.X < b.X) ? a.X : b.X;
            result.Y = (a.Y < b.Y) ? a.Y : b.Y;
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Maximum vector</returns>
        public static Int2 Max(Int2 a, Int2 b)
        {
            Max(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Maximum vector</param>
        public static void Max(ref Int2 a, ref Int2 b, out Int2 result)
        {
            result.X = (a.X > b.X) ? a.X : b.X;
            result.Y = (a.Y > b.Y) ? a.Y : b.Y;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Int2 Negate(Int2 value)
        {
            Negate(ref value, out Int2 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="result">Negated vector</param>
        public static void Negate(ref Int2 value, out Int2 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        /// <summary>
        /// Adds the two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Int2 operator +(Int2 a, Int2 b)
        {
            Add(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Int2 operator -(Int2 a, Int2 b)
        {
            Subtract(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Int2 operator -(Int2 value)
        {
            Int2 result;
            result.X = -value.X;
            result.Y = -value.Y;

            return result;
        }

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vector</returns>
        public static Int2 operator *(Int2 a, Int2 b)
        {
            Multiply(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Multiplied vector</returns>
        public static Int2 operator *(Int2 value, int scale)
        {
            Multiply(ref value, scale, out Int2 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="scale">Amount to scale</param>
        /// <param name="value">Source vector</param>
        /// <returns>Multiplied vector</returns>
        public static Int2 operator *(int scale, Int2 value)
        {
            Multiply(ref value, scale, out Int2 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Int2 operator /(Int2 a, Int2 b)
        {
            Divide(ref a, ref b, out Int2 result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Int2 operator /(Int2 value, int divisor)
        {
            Divide(ref value, divisor, out Int2 result);
            return result;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Int2"/> to <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value"><see cref="Int2"/> value</param>
        /// <returns>Converted <see cref="Vector2"/></returns>
        public static explicit operator Vector2(Int2 value)
        {
            return new Vector2(value.X, value.Y);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector2"/> to <see cref="Int2"/>.
        /// </summary>
        /// <param name="value"><see cref="Vector2"/> value</param>
        /// <returns>Converted <see cref="Int2"/></returns>
        public static explicit operator Int2(Vector2 value)
        {
            return new Int2((int)value.X, (int)value.Y);
        }

        /// <summary>
        /// Checks equality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(Int2 a, Int2 b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Checks inequality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(Int2 a, Int2 b)
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
        }

        /// <summary>
        /// Checks equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Other vector</param>
        /// <returns>True if the vectors are equal, false otherwise.</returns>
        public bool Equals(Int2 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Checks equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Other vector</param>
        /// <returns>True if the vectors are equal, false otherwise.</returns>
        public bool Equals(ref Int2 other)
        {
            return (X == other.X) && (Y == other.Y);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, False.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Int2)
            {
                return Equals((Int2)obj);
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
                return X.GetHashCode() + Y.GetHashCode();
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

            return string.Format(formatProvider, "X: {0} Y: {1}", new object[] { X.ToString(format, formatProvider), Y.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("X", X);
            output.Write("Y", Y);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            X = input.ReadInt32();
            Y = input.ReadInt32();
        }
    }
}
