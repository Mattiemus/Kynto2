namespace Spark.Math
{
    using System;

    /// <summary>
    /// Utility class that contains math constants and useful math functions.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Represents a "close to zero" value.
        /// </summary> 
        public const float ZeroTolerance = 1E-06f;

        /// <summary>
        /// Determines if two values are approximately equal
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <returns>True if the two values are equal</returns>
        public static bool IsApproxEquals(float a, float b)
        {
            return Math.Abs(a - b) <= ZeroTolerance;
        }

        /// <summary>
        /// Determines if a value is nearly equal to zero
        /// </summary>
        /// <param name="value">Floating point value</param>
        /// <returns>True if the input is nearly equal to zero</returns>
        public static bool IsApproxZero(float value)
        {
            return value <= ZeroTolerance;
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
    }
}
