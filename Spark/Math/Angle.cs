namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a two dimensional vector of 32-bit floats.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Angle : IComparable<Angle>, IEquatable<Angle>, IFormattable, IPrimitiveValue
    {
        private float _radians;

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> struct.
        /// </summary>
        /// <param name="radians">The angle, in radians.</param>
        public Angle(float radians)
        {
            _radians = radians;
        }

        /// <summary>
        /// Gets an <see cref="Angle"/> that is 0°.
        /// </summary>
        public static Angle Zero => new Angle(0.0f);

        /// <summary>
        /// Gets an <see cref="Angle"/> that is 45°.
        /// </summary>
        public static Angle PiOverFour => new Angle(MathHelper.PiOverFour);

        /// <summary>
        /// Gets an <see cref="Angle"/> that is 90°.
        /// </summary>
        public static Angle PiOverTwo => new Angle(MathHelper.PiOverTwo);

        /// <summary>
        /// Gets an <see cref="Angle"/> that is 135°.
        /// </summary>
        public static Angle ThreePiOverFour => new Angle(MathHelper.ThreePiOverFour);

        /// <summary>
        /// Gets an <see cref="Angle"/> that is 180°.
        /// </summary>
        public static Angle Pi => new Angle(MathHelper.Pi);

        /// <summary>
        /// Gets an <see cref="Angle"/> that is 360°.
        /// </summary>
        public static Angle TwoPi => new Angle(MathHelper.TwoPi);

        /// <summary>
        /// Gets the size of the <see cref="Angle"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Angle>();

        /// <summary>
        /// Gets whether the angle is NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(_radians);

        /// <summary>
        /// Gets whether the angle is positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(_radians);

        /// <summary>
        /// Gets or sets the angle, in radians.
        /// </summary>
        public float Radians
        {
            get => _radians;
            set => _radians = value;
        }

        /// <summary>
        /// Gets or sets the angle, in degrees.
        /// </summary>
        public float Degrees
        {
            get => MathHelper.ToDegrees(_radians);
            set => _radians = MathHelper.ToRadians(value);
        }

        /// <summary>
        /// Gets the sine of the angle.
        /// </summary>
        public float Sin => (float)Math.Sin(_radians);

        /// <summary>
        /// Gets the cosine of the angle.
        /// </summary>
        public float Cos => (float)Math.Cos(_radians);

        /// <summary>
        /// Gets the tangent of the angle.
        /// </summary>
        public float Tan => (float)Math.Tan(_radians);

        /// <summary>
        /// Creates a new <see cref="Angle"/> from an angle in radians.
        /// </summary>
        /// <param name="angleInRadians">Angle in radians</param>
        /// <returns>The angle</returns>
        public static Angle FromRadians(float angleInRadians)
        {
            return new Angle(angleInRadians);
        }

        /// <summary>
        /// Creates a new <see cref="Angle"/> from an angle in degrees.
        /// </summary>
        /// <param name="angleInDegrees">Angle in degrees</param>
        /// <returns>The angle</returns>
        public static Angle FromDegrees(float angleInDegrees)
        {
            return new Angle(MathHelper.ToRadians(angleInDegrees));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {

            return _radians.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Angle)
            {
                return Equals((Angle)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the angle and another angle.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Angle other)
        {
            return MathHelper.IsApproxEquals(other._radians, _radians);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero 
        /// This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. 
        /// Greater than zero This object is greater than <paramref name="other" />.</returns>
        public int CompareTo(Angle other)
        {
            return _radians.CompareTo(other._radians);
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
            
            return _radians.ToString(format, formatProvider);
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("Angle", _radians);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            _radians = input.ReadSingle();
        }
        
        /// <summary>
        /// Performs an explicit cast from a number to an angle.
        /// </summary>
        /// <param name="value">Value, in radians</param>
        /// <returns>Value as an angle</returns>
        public static explicit operator Angle(double value)
        {
            return new Angle((float)value);
        }

        /// <summary>
        /// Performs an explicit cast from an angle to a number.
        /// </summary>
        /// <param name="value">Angle value</param>
        /// <returns>Value a double number, in radians</returns>
        public static explicit operator double(Angle value)
        {
            return value._radians;
        }

        /// <summary>
        /// Performs an explicit cast from a number to an angle.
        /// </summary>
        /// <param name="value">Value, in radians</param>
        /// <returns>Value as an angle</returns>
        public static explicit operator Angle(float value)
        {
            return new Angle(value);
        }

        /// <summary>
        /// Performs an explicit cast from an angle to a number.
        /// </summary>
        /// <param name="value">Angle value</param>
        /// <returns>Value as a floating number, in radians.</returns>
        public static explicit operator float(Angle value)
        {
            return value._radians;
        }

        /// <summary>
        /// Tests if angle a is greater than b.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>True if a is greater than b, false otherwise.</returns>
        public static bool operator >(Angle a, Angle b)
        {
            return a._radians > b._radians;
        }

        /// <summary>
        /// Tests if angle a is less than b.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>True if a is less than b, false otherwise.</returns>
        public static bool operator <(Angle a, Angle b)
        {
            return a._radians < b._radians;
        }

        /// <summary>
        /// Tests if angle a is greater than or equal to b.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>True if a is greater than or equal to b, false otherwise.</returns>
        public static bool operator >=(Angle a, Angle b)
        {
            return a._radians >= b._radians;
        }

        /// <summary>
        /// Tests if angle a is less than or equal to b.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>True if a is less than or equal to b, false otherwise.</returns>
        public static bool operator <=(Angle a, Angle b)
        {
            return a._radians <= b._radians;
        }

        /// <summary>
        /// Determines if this <see cref="Angle"/> is equal to another
        /// </summary>
        /// <param name="lhs">Left side operand</param>
        /// <param name="rhs">Right side operand</param>
        /// <returns>True if the values are equal, false if otherwise</returns>
        public static bool operator ==(Angle lhs, Angle rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines if this <see cref="Angle"/> is not equal to another
        /// </summary>
        /// <param name="lhs">Left side operand</param>
        /// <param name="rhs">Right side operand</param>
        /// <returns>True if the values are not equal, false if otherwise</returns>
        public static bool operator !=(Angle lhs, Angle rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
