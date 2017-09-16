namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// A linear  128-bit floating point color using red, green, blue, and alpha components (in RGBA order).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LinearColor : IEquatable<LinearColor>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// Red component of the color.
        /// </summary>
        public float R;

        /// <summary>
        /// Green component of the color.
        /// </summary>
        public float G;

        /// <summary>
        /// Blue component of the color.
        /// </summary>
        public float B;

        /// <summary>
        /// Alpha component of the color.
        /// </summary>
        public float A;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearColor"/> struct.
        /// </summary>
        /// <param name="value">Value to initialize each component to</param>
        public LinearColor(float value)
        {
            R = G = B = A = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4"/> struct.
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        public LinearColor(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Gets a color with the value R: 0 G: 0 B: 0 A: 0.
        /// </summary>
        public static LinearColor Black => new LinearColor(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a color with the value R: 1 G: 1 B: 1 A: 0.
        /// </summary>
        public static LinearColor White => new LinearColor(1.0f, 1.0f, 1.0f, 0.0f);

        /// <summary>
        /// Gets a color with the value R: 1 G: 0 B: 0 A: 0.
        /// </summary>
        public static LinearColor Red => new LinearColor(1.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a color with the value R: 0 G: 1 B: 0 A: 0.
        /// </summary>
        public static LinearColor Green => new LinearColor(0.0f, 1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a color with the value R: 0 G: 0 B: 1 A: 0.
        /// </summary>
        public static LinearColor Blue => new LinearColor(0.0f, 0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Gets the size of the <see cref="LinearColor"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<LinearColor>();

        /// <summary>
        /// Gets whether any of the components of the linear color are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(R) || float.IsNaN(G) || float.IsNaN(B) || float.IsNaN(A);

        /// <summary>
        /// Gets whether any of the components of the linear color are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(R) || float.IsInfinity(G) || float.IsInfinity(B) || float.IsInfinity(A);

        /// <summary>
        /// Gets or sets individual components of the linear color in the order that the components are declared (RGBA).
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
                        return R;
                    case 1:
                        return G;
                    case 2:
                        return B;
                    case 3:
                        return A;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        R = value;
                        break;
                    case 1:
                        G = value;
                        break;
                    case 2:
                        B = value;
                        break;
                    case 3:
                        A = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return R.GetHashCode() + G.GetHashCode() + B.GetHashCode() + A.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is LinearColor)
            {
                return Equals((LinearColor)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the linear color and another linear color.
        /// </summary>
        /// <param name="other">Linear color to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(LinearColor other)
        {
            return MathHelper.IsApproxEquals(other.R, R) &&
                MathHelper.IsApproxEquals(other.G, G) &&
                MathHelper.IsApproxEquals(other.B, B) &&
                MathHelper.IsApproxEquals(other.A, A);
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
                R.ToString(format, formatProvider),
                G.ToString(format, formatProvider),
                B.ToString(format, formatProvider),
                A.ToString(format, formatProvider)
            };

            return string.Format(formatProvider, "R: {0} G: {1} B: {2} A: {3}", components);
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("R", R);
            output.Write("G", G);
            output.Write("B", B);
            output.Write("A", A);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            R = input.ReadSingle();
            G = input.ReadSingle();
            B = input.ReadSingle();
            A = input.ReadSingle();
        }

        /// <summary>
        /// Determines if this <see cref="LinearColor"/> is equal to another
        /// </summary>
        /// <param name="lhs">Left side operand</param>
        /// <param name="rhs">Right side operand</param>
        /// <returns>True if the values are equal, false if otherwise</returns>
        public static bool operator ==(LinearColor lhs, LinearColor rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines if this <see cref="LinearColor"/> is not equal to another
        /// </summary>
        /// <param name="lhs">Left side operand</param>
        /// <param name="rhs">Right side operand</param>
        /// <returns>True if the values are not equal, false if otherwise</returns>
        public static bool operator !=(LinearColor lhs, LinearColor rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
