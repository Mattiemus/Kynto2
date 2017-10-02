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
        /// Gets the angle which completes the full circle (360°) with the same sweep direction.
        /// </summary>
        public Angle ForwardSweepToFullCircle
        {
            get
            {
                Angle result;
                if (_radians > 0.0f)
                {
                    result._radians = MathHelper.TwoPi - _radians;
                }
                else
                {
                    result._radians = -MathHelper.TwoPi - _radians;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the angle which completes the full circle (360°) with the opposite sweep direction.
        /// </summary>
        public Angle ReverseSweepToFullCircle
        {
            get
            {
                Angle result;
                if (_radians > 0.0)
                {
                    result._radians = _radians - MathHelper.TwoPi;
                }
                else
                {
                    result._radians = _radians + MathHelper.TwoPi;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the angle that complements this angle, where both angles add to 90°.
        /// </summary>
        public Angle Complement => new Angle(MathHelper.PiOverTwo - _radians);

        /// <summary>
        /// Gets the angle that supplements this angle, where both angles add to 180°.
        /// </summary>
        public Angle Supplement => new Angle(MathHelper.Pi - _radians);
        
        /// <summary>
        /// Gets if the angle is a full circle, that is if it is 360°.
        /// </summary>
        public bool IsFullCircle => MathHelper.IsApproxEquals(Math.Abs(_radians), MathHelper.TwoPi);

        /// <summary>
        /// Gets if the angle is zero, that is if it is 0°.
        /// </summary>
        public bool IsZeroAngle => MathHelper.IsApproxEquals(Math.Abs(_radians), 0.0f);

        /// <summary>
        /// Gets if the angle is an acute angle, where it is less than 90° but greater than 0°.
        /// </summary>
        public bool IsAcuteAngle => Math.Abs(_radians) > 0.0f && Math.Abs(_radians) < MathHelper.PiOverTwo;

        /// <summary>
        /// Gets if the angle is an obtuse angle, where it is is gerater than 90° but less than 180°.
        /// </summary>
        public bool IsObtuseAngle => Math.Abs(_radians) > MathHelper.PiOverTwo && Math.Abs(_radians) < MathHelper.Pi;

        /// <summary>
        /// Gets if the angle is a right angle, where it is 90°.
        /// </summary>
        public bool IsRightAngle => MathHelper.IsApproxEquals(Math.Abs(_radians), MathHelper.PiOverTwo);

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
        /// Returns the angle whose cosine is the specified number.
        /// </summary>
        /// <param name="num">Cosine of the angle where the value must be greater than or equal to -1, but less than or equal to 1.</param>
        /// <returns>The angle in the range [0, π] or NaN if the number is out of range. </returns>
        public static Angle Asin(float num)
        {
            Asin(num, out Angle result);
            return result;
        }

        /// <summary>
        /// Returns the angle whose cosine is the specified number.
        /// </summary>
        /// <param name="num">Cosine of the angle where the value must be greater than or equal to -1, but less than or equal to 1.</param>
        /// <param name="result">The angle in the range [0, π] or NaN if the number is out of range.</param>
        public static void Asin(float num, out Angle result)
        {
            result._radians = (float)Math.Asin(num);
        }
        
        /// <summary>
        /// Returns the angle whose sine is the specified number.
        /// </summary>
        /// <param name="num">Sine of the angle where the value must be greater than or equal to -1, but less than or equal to 1.</param>
        /// <returns>The angle in the range [-π/2, π/2] or NaN if the number is out of range.</returns>
        public static Angle Acos(float num)
        {
            Acos(num, out Angle result);
            return result;
        }

        /// <summary>
        /// Returns the angle whose sine is the specified number.
        /// </summary>
        /// <param name="num">Sine of the angle where the value must be greater than or equal to -1, but less than or equal to 1.</param>
        /// <param name="result">The angle in the range [-π/2, π/2] or NaN if the number is out of range.</param>
        public static void Acos(float num, out Angle result)
        {
            result._radians = (float)Math.Acos(num);
        }

        /// <summary>
        /// Returns the angle whose tangent is the specified number.
        /// </summary>
        /// <param name="num">The tangent</param>
        /// <returns>The angle in the range [-π/2, π/2] or NaN if the number is out of range.</returns>
        public static Angle Atan(float num)
        {
            Atan(num, out Angle result);
            return result;
        }

        /// <summary>
        /// Returns the angle whose tangent is the specified number.
        /// </summary>
        /// <param name="num">The tangent</param>
        /// <param name="result">The angle in the range [-π/2, π/2] or NaN if the number is out of range.</param>
        public static void Atan(float num, out Angle result)
        {
            result._radians = (float)Math.Atan(num);
        }

        /// <summary>
        /// Returns the angle whose tangent is the quotient of two coordinates of a cartesian point.
        /// </summary>
        /// <param name="y">Y coordinate of the point</param>
        /// <param name="x">X coordinate of the point</param>
        /// <returns>The angle in the range [-π, π] or NaN if the coordinates are out of range.</returns>
        public static Angle Atan2(float y, float x)
        {
            Atan2(x, y, out Angle result);
            return result;
        }

        /// <summary>
        /// Returns the angle whose tangent is the quotient of two coordinates of a cartesian point.
        /// </summary>
        /// <param name="y">Y coordinate of the point</param>
        /// <param name="x">X coordinate of the point</param>
        /// <param name="result">The angle in the range [-π, π] or NaN if the coordinates are out of range.</param>
        public static void Atan2(float y, float x, out Angle result)
        {
            result._radians = (float)Math.Atan2(x, y);
        }

        /// <summary>
        /// Adds two angles together.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>Sum of the two angles</returns>
        public static Angle Add(Angle a, Angle b)
        {
            Add(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Adds two angles together.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <param name="result">Sum of the two angles</param>
        public static void Add(ref Angle a, ref Angle b, out Angle result)
        {
            result._radians = a._radians + b._radians;
        }

        /// <summary>
        /// Subtracts angle b from angle a.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>Difference of the two angles</returns>
        public static Angle Subtract(Angle a, Angle b)
        {
            Subtract(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Subtracts angle b from angle a.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <param name="result">Difference of the two angles</param>
        public static void Subtract(ref Angle a, ref Angle b, out Angle result)
        {
            result._radians = a._radians - b._radians;
        }

        /// <summary>
        /// Multiplies two angles together.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>Product of the two angles</returns>
        public static Angle Multiply(Angle a, Angle b)
        {
            Multiply(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Multiplies two angles together.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <param name="result">Product of the two angles</param>
        public static void Multiply(ref Angle a, ref Angle b, out Angle result)
        {
            result._radians = a._radians * b._radians;
        }

        /// <summary>
        /// Multiplies the angle by a scalar value.
        /// </summary>
        /// <param name="value">Angle</param>
        /// <param name="scale">Scalar</param>
        /// <returns>Multiplied angle</returns>
        public static Angle Multiply(Angle value, float scale)
        {
            Multiply(ref value, scale, out Angle result);
            return result;
        }

        /// <summary>
        /// Multiplies the angle by a scalar value.
        /// </summary>
        /// <param name="value">Angle</param>
        /// <param name="scale">Scalar</param>
        /// <param name="result">Multiplied angle</param>
        public static void Multiply(ref Angle value, float scale, out Angle result)
        {
            result._radians = value._radians * scale;
        }

        /// <summary>
        /// Divides angle a by angle b.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Divisor angle</param>
        /// <returns>Quotient of the two angles</returns>
        public static Angle Divide(Angle a, Angle b)
        {
            Divide(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Divides angle a by angle b.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Divisor angle</param>
        /// <param name="result">Quotient of the two angles</param>
        public static void Divide(ref Angle a, ref Angle b, out Angle result)
        {
            result._radians = a._radians / b._radians;
        }

        /// <summary>
        /// Divides an angle by a divisor.
        /// </summary>
        /// <param name="value">Angle</param>
        /// <param name="divisor">Divisor</param>
        /// <returns>Divided angle</returns>
        public static Angle Divide(Angle value, float divisor)
        {
            Divide(ref value, divisor, out Angle result);
            return result;
        }

        /// <summary>
        /// Divides an angle by a divisor.
        /// </summary>
        /// <param name="value">Angle</param>
        /// <param name="divisor">Divisor</param>
        /// <param name="result">Divided angle</param>
        public static void Divide(ref Angle value, float divisor, out Angle result)
        {
            result._radians = value._radians / divisor;
        }

        /// <summary>
        /// Initializes a new <see cref="Angle"/> from an angle in degrees.
        /// </summary>
        /// <param name="angleInDegrees">Angle in degrees</param>
        /// <returns>The angle</returns>
        public static Angle FromDegrees(float angleInDegrees)
        {
            FromDegrees(angleInDegrees, out Angle result);
            return result;
        }

        /// <summary>
        /// Initializes a new <see cref="Angle"/> from an angle in degrees.
        /// </summary>
        /// <param name="angleInDegrees">Angle in degrees</param>
        /// <param name="result">The angle</param>
        public static void FromDegrees(float angleInDegrees, out Angle result)
        {
            result._radians = MathHelper.ToRadians(angleInDegrees);
        }

        /// <summary>
        /// Initializes a new <see cref="Angle"/> from an angle in radians.
        /// </summary>
        /// <param name="angleInRadians">Angle in radians</param>
        /// <returns>The angle</returns>
        public static Angle FromRadians(float angleInRadians)
        {
            FromRadians(angleInRadians, out Angle result);
            return result;
        }

        /// <summary>
        /// Initializes a new <see cref="Angle"/> from an angle in radians.
        /// </summary>
        /// <param name="angleInRadians">Angle in radians</param>
        /// <param name="result">The angle</param>
        public static void FromRadians(float angleInRadians, out Angle result)
        {
            result._radians = angleInRadians;
        }

        /// <summary>
        /// Tests if the angle is within the specified sweep.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <param name="startAngle">Starting angle of the sweep.</param>
        /// <param name="sweepAngle">Sweep angle</param>
        /// <returns>True if the angle is within the sweep, false otherwise.</returns>
        public static bool IsInSweep(Angle value, Angle startAngle, Angle sweepAngle)
        {
            IsInSweep(ref value, ref startAngle, ref sweepAngle, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the angle is within the specified sweep.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <param name="startAngle">Starting angle of the sweep.</param>
        /// <param name="sweepAngle">Sweep angle</param>
        /// <param name="result">True if the angle is within the sweep, false otherwise.</param>
        public static void IsInSweep(ref Angle value, ref Angle startAngle, ref Angle sweepAngle, out bool result)
        {
            float diffAngle = value._radians - startAngle._radians;
            float sweepRads = sweepAngle._radians;

            if (diffAngle < 0.0)
            {
                diffAngle = -diffAngle;
                sweepRads = -sweepRads;
            }

            if (diffAngle >= -MathHelper.ZeroTolerance && diffAngle <= (MathHelper.ZeroTolerance + sweepRads))
            {
                result = true;
                return;
            }

            Angle test;
            test._radians = diffAngle;
            test.WrapToPositive();

            result = test._radians <= (MathHelper.ZeroTolerance + value._radians) || test._radians >= (MathHelper.TwoPi - MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Returns the larger of the two angles.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>Larger angle</returns>
        public static Angle Max(Angle a, Angle b)
        {
            Max(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Returns the larger of the two angles.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <param name="result">Larger angle</param>
        public static void Max(ref Angle a, ref Angle b, out Angle result)
        {
            result._radians = (a._radians > b._radians) ? a._radians : b._radians;
        }

        /// <summary>
        /// Returns the smaller of the two angles.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>Smaller angle</returns>
        public static Angle Min(Angle a, Angle b)
        {
            Min(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Returns the smaller of the two angles.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <param name="result">Smaller angle</param>
        public static void Min(ref Angle a, ref Angle b, out Angle result)
        {
            result._radians = (a._radians < b._radians) ? a._radians : b._radians;
        }

        /// <summary>
        /// Restricts the source angle in the range of the minimum and maximum angles.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <param name="min">Minimum angle</param>
        /// <param name="max">Maximum angle</param>
        /// <returns>Clamped angle</returns>
        public static Angle Clamp(Angle value, Angle min, Angle max)
        {
            Clamp(ref value, ref min, ref max, out Angle result);
            return result;
        }

        /// <summary>
        /// Restricts the source angle in the range of the minimum and maximum angles.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <param name="min">Minimum angle</param>
        /// <param name="max">Maximum angle</param>
        /// <param name="result">Clamped angle</param>
        public static void Clamp(ref Angle value, ref Angle min, ref Angle max, out Angle result)
        {
            float rads = value._radians;

            rads = (rads > max._radians) ? max._radians : rads;
            rads = (rads < min._radians) ? min._radians : rads;

            result._radians = rads;
        }

        /// <summary>
        /// Flips the sign of the angle.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <returns>Negated angle</returns>
        public static Angle Negate(Angle value)
        {
            Negate(ref value, out Angle result);
            return result;
        }

        /// <summary>
        /// Flips the sign of the angle.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <param name="result">Negated angle</param>
        public static void Negate(ref Angle value, out Angle result)
        {
            result._radians = -value._radians;
        }

        /// <summary>
        /// Wraps the angle to be in the range of [0, 2π).
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <returns>Angle in [0, 2π) range</returns>
        public static Angle WrapToPositive(Angle value)
        {
            WrapToPositive(ref value, out Angle result);
            return result;
        }

        /// <summary>
        /// Wraps the angle to be in the range of [0, 2π).
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <param name="result">Angle in [0, 2π) range</param>
        public static void WrapToPositive(ref Angle value, out Angle result)
        {
            result._radians = value._radians % MathHelper.TwoPi;
            if (result._radians < 0.0f)
            {
                result._radians += MathHelper.TwoPi;
            }
        }

        /// <summary>
        /// Wraps the angle to be in the range of [π, -π], hence about zero.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <returns>Angle in [π, -π] range</returns>
        public static Angle WrapAroundZero(Angle value)
        {
            WrapAroundZero(ref value, out Angle result);
            return result;
        }

        /// <summary>
        /// Wraps the angle to be in the range of [π, -π], hence about zero.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <param name="result">Angle in [π, -π] range</param>
        public static void WrapAroundZero(ref Angle value, out Angle result)
        {
            result._radians = (float)Math.IEEERemainder(value._radians, Math.PI * 2.0d);
            if (result._radians <= -MathHelper.Pi)
            {
                result._radians += MathHelper.TwoPi;
            }
            else if (result._radians > MathHelper.Pi)
            {
                result._radians -= MathHelper.TwoPi;
            }
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
        /// Adds two angles together.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>Sum of the two angles</returns>
        public static Angle operator +(Angle a, Angle b)
        {
            Add(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Subtracts angle b from angle a.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>Difference of the two angles</returns>
        public static Angle operator -(Angle a, Angle b)
        {
            Subtract(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Multiplies two angles together.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>Product of the two angles</returns>
        public static Angle operator *(Angle a, Angle b)
        {
            Multiply(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Multiplies the angle by a scalar.
        /// </summary>
        /// <param name="value">Angle</param>
        /// <param name="scale">Scalar</param>
        /// <returns>Multiplied angle</returns>
        public static Angle operator *(Angle value, float scale)
        {
            Multiply(ref value, scale, out Angle result);
            return result;
        }

        /// <summary>
        /// Multiplies the angle by a scalar.
        /// </summary>
        /// <param name="scale">Scalar</param>
        /// <param name="value">Angle</param>
        /// <returns>Multiplied angle</returns>        
        public static Angle operator *(float scale, Angle value)
        {
            Multiply(ref value, scale, out Angle result);
            return result;
        }

        /// <summary>
        /// Divides angle a by angle b.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Divisor angle</param>
        /// <returns>Quotient of the two angles</returns>
        public static Angle operator /(Angle a, Angle b)
        {
            Divide(ref a, ref b, out Angle result);
            return result;
        }

        /// <summary>
        /// Divides angle by a divisor.
        /// </summary>
        /// <param name="value">Angle</param>
        /// <param name="divisor">Divisor</param>
        /// <returns>Divided angle</returns>
        public static Angle operator /(Angle value, float divisor)
        {
            Divide(ref value, divisor, out Angle result);
            return result;
        }

        /// <summary>
        /// Flips the sign of the angle.
        /// </summary>
        /// <param name="value">Source angle</param>
        /// <returns>Negated angle</returns>
        public static Angle operator -(Angle value)
        {
            Negate(ref value, out Angle result);
            return result;
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
        /// Tests equality between two angles.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>True if both angles are equal, false otherwise</returns>
        public static bool operator ==(Angle a, Angle b)
        {
            return MathHelper.IsApproxEquals(a._radians, b._radians);
        }

        /// <summary>
        /// Tests inequality between two angles.
        /// </summary>
        /// <param name="a">First angle</param>
        /// <param name="b">Second angle</param>
        /// <returns>True if both angles are not equal, false otherwise</returns>
        public static bool operator !=(Angle a, Angle b)
        {
            return !MathHelper.IsApproxEquals(a._radians, b._radians);
        }

        /// <summary>
        /// Tests if the angle is within the specified sweep.
        /// </summary>
        /// <param name="startAngle">Starting angle of the sweep.</param>
        /// <param name="sweepAngle">Sweep angle</param>
        /// <returns>True if the angle is within the sweep, false otherwise.</returns>
        public bool IsInSweep(Angle startAngle, Angle sweepAngle)
        {
            float diffAngle = _radians - startAngle._radians;
            float sweepRads = sweepAngle._radians;

            if (diffAngle < 0.0)
            {
                diffAngle = -diffAngle;
                sweepRads = -sweepRads;
            }

            if (diffAngle >= -MathHelper.ZeroTolerance && diffAngle <= (MathHelper.ZeroTolerance + sweepRads))
            {
                return true;
            }

            Angle test;
            test._radians = diffAngle;
            test.WrapToPositive();

            return test._radians <= (MathHelper.ZeroTolerance + _radians) || test._radians >= (MathHelper.TwoPi - MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Flips the sign of the angle.
        /// </summary>
        public void Negate()
        {
            _radians = -_radians;
        }

        /// <summary>
        /// Wraps the angle to be in the range of [0, 2π).
        /// </summary>
        public void WrapToPositive()
        {
            _radians = _radians % MathHelper.TwoPi;
            if (_radians < 0.0f)
            {
                _radians += MathHelper.TwoPi;
            }
        }

        /// <summary>
        /// Wraps the angle to be in the range of [π, -π], hence about zero.
        /// </summary>
        public void WrapAroundZero()
        {
            _radians = (float)Math.IEEERemainder(_radians, Math.PI * 2.0d);
            if (_radians <= -MathHelper.Pi)
            {
                _radians += MathHelper.TwoPi;
            }
            else if (_radians > MathHelper.Pi)
            {
                _radians -= MathHelper.TwoPi;
            }
        }

        /// <summary>
        /// Tests equality between the angle and another angle.
        /// </summary>
        /// <param name="other">Angle to test against</param>
        /// <returns>True if angles are equal, false otherwise.</returns>
        public bool Equals(Angle other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the angle and another angle.
        /// </summary>
        /// <param name="other">Angle to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if angles are equal within tolerance, false otherwise.</returns>
        public bool Equals(Angle other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the angle and another angle.
        /// </summary>
        /// <param name="other">Angle to test against</param>
        /// <returns>True if angles are equal, false otherwise.</returns>
        public bool Equals(ref Angle other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the angle and another angle.
        /// </summary>
        /// <param name="other">Angle to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if angles are equal within tolerance, false otherwise.</returns>
        public bool Equals(ref Angle other, float tolerance)
        {
            return Math.Abs(_radians - other._radians) <= tolerance;
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
                return MathHelper.IsApproxEquals(((Angle)obj)._radians, _radians);
            }

            return false;
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
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return _radians.GetHashCode();
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

            return string.Format(formatProvider, "Radians: {0} Degrees: {1}", new object[] { Radians.ToString(format, formatProvider), Degrees.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("Radians", _radians);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            _radians = input.ReadSingle();
        }
    }
}
