﻿namespace Spark.Math
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
