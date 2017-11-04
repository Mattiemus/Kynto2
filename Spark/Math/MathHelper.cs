namespace Spark.Math
{
    using System;

    /// <summary>
    /// Utility class that contains math constants and useful math functions.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Represents the math constant e.
        /// </summary>
        public const float E = (float)Math.E;

        /// <summary>
        /// Represents the math constant log base ten of e.
        /// </summary>
        public const float Log10E = (float)0.43429448190325182d;

        /// <summary>
        /// Represents the log base two of e.
        /// </summary>
        public const float Log2E = (float)1.4426950408889634d;

        /// <summary>
        /// Represents Pi/2 (90 degrees).
        /// </summary>
        public const float PiOverTwo = (float)(Math.PI / 2.0d);

        /// <summary>
        /// Represents Pi/4 (45 degrees).
        /// </summary>
        public const float PiOverFour = (float)(Math.PI / 4.0d);

        /// <summary>
        /// Represents 3Pi/4 (135 degrees).
        /// </summary>
        public const float ThreePiOverFour = (float)((3.0d * Math.PI) / 4.0d);

        /// <summary>
        /// Represents the value of pi (180 degrees).
        /// </summary>
        public const float Pi = (float)Math.PI;

        /// <summary>
        /// Represents 2Pi (360 degrees).
        /// </summary>
        public const float TwoPi = (float)(Math.PI * 2.0d);

        /// <summary>
        /// Value to multiply degrees by to obtain radians.
        /// </summary>
        public const float DegreesToRadians = (float)(Math.PI / 180.0d);

        /// <summary>
        /// Value to multiply radians by to obtain degrees.
        /// </summary>
        public const float RadiansToDegrees = (float)(180.0d / Math.PI);

        /// <summary>
        /// Represents Pi^2
        /// </summary>
        public const float PiSquared = (float)(Math.PI * Math.PI);

        /// <summary>
        /// One thirds constant (1/3).
        /// </summary>
        public const float OneThird = (float)(1.0d / 3.0d);

        /// <summary>
        /// Two thirds constant (2/3).
        /// </summary>
        public const float TwoThird = (float)(2.0d / 3.0d);

        /// <summary>
        /// Four thirds constant (4/3).
        /// </summary>
        public const float FourThirds = (float)(2.0d / 3.0d);

        /// <summary>
        /// Represents a "close to zero" value.
        /// </summary>
        public const float ZeroTolerance = 1E-06f;

        /// <summary>
        /// Represents a very tight "close to zero" value.
        /// </summary>
        public const float TightZeroTolerance = 1E-12f;

        /// <summary>
        /// Represents a "close to zero" value, the smallest float value possible.
        /// </summary>
        public const float Epsilon = float.Epsilon;

        /// <summary>
        /// Converts an angle in radians to the corresponding angle in degrees.
        /// </summary>
        /// <param name="radians">Angle in radians</param>
        /// <returns>Angle in degrees</returns>
        public static float ToDegrees(float radians)
        {
            return radians * RadiansToDegrees;
        }

        /// <summary>
        /// Converts an angle in degrees to the corresponding angle in radians.
        /// </summary>
        /// <param name="degrees">Angle in degrees</param>
        /// <returns>Angle in radians</returns>
        public static float ToRadians(float degrees)
        {
            return degrees * DegreesToRadians;
        }

        /// <summary>
        /// Determines if two values are approximately equal
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <returns>True if the two values are equal</returns>
        public static bool IsApproxEquals(float a, float b)
        {
            return IsApproxEquals(a, b, ZeroTolerance);
        }

        /// <summary>
        /// Checks equality between a and b using the specified tolerance
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <param name="tolerance">Tolerance that a and b should be within</param>
        /// <returns>True if a is nearly equal to b</returns>
        public static bool IsApproxEquals(float a, float b, float tolerance)
        {
            return Math.Abs(a - b) <= tolerance;
        }

        /// <summary>
        /// Determines if a value is nearly equal to zero
        /// </summary>
        /// <param name="value">Floating point value</param>
        /// <returns>True if the input is nearly equal to zero</returns>
        public static bool IsApproxZero(float value)
        {
            return Math.Abs(value) <= ZeroTolerance;
        }

        /// <summary>
        /// Clamps a value within the specified range.
        /// </summary>
        /// <param name="value">Source value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped value</returns>
        public static int Clamp(int value, int min, int max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        /// <summary>
        /// Clamps a value within the specified range.
        /// </summary>
        /// <param name="value">Source value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped value</returns>
        public static float Clamp(float value, float min, float max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        /// <summary>
        /// Clamps an integer to the byte range 0-255.
        /// </summary>
        /// <param name="value">Integer to be clamped</param>
        /// <returns>Clamped value</returns>
        public static int ClampToByte(int value)
        {
            return Clamp(value, 0, 255);
        }

        /// <summary>
        /// Clamps and rounds a value within the specified range.
        /// </summary>
        /// <param name="value">Source value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped and rounded value in double precision</returns>
        public static double ClampAndRound(float value, float min, float max)
        {
            if (float.IsNaN(value))
            {
                return 0.0;
            }

            if (float.IsInfinity(value))
            {
                return (float.IsNegativeInfinity(value) ? min : max);
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return Math.Round(value);
        }

        /// <summary>
        /// Checks if a value is within the specified range.
        /// </summary>
        /// <param name="value">Source value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>True if the value is within the interval false otherwise.</returns>
        public static bool InInterval(float value, float min, float max)
        {
            if (value > max || value < min)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Solves the quadratic where a is the quadratic term, b the linear term, and c the constant term. A*x^2 + B*X + C = 0.
        /// </summary>
        /// <param name="a">Quadratic term</param>
        /// <param name="b">Linear term</param>
        /// <param name="c">Constant term</param>
        /// <param name="result1">First solution</param>
        /// <param name="result2">Second solution</param>
        /// <returns>True if a real solution was found, false if imaginary.</returns>
        public static bool SolveQuadratic(float a, float b, float c, out float result1, out float result2)
        {
            float sqrtpart = b * b - 4 * a * c;

            if (sqrtpart > 0)
            {
                // Two real solutions
                result1 = (-b + (float)Math.Sqrt(sqrtpart)) / (2 * a);
                result2 = (-b - (float)Math.Sqrt(sqrtpart)) / (2 * a);

                return true;
            }
            else if (sqrtpart < 0)
            {
                // Two imaginary solutions
                result1 = result2 = 0;
                result2 = result1;

                return false;
            }
            else
            {
                //One real solution
                result1 = (-b + (float)Math.Sqrt(sqrtpart)) / (2 * a);
                result2 = result1;

                return true;
            }
        }
    }
}
