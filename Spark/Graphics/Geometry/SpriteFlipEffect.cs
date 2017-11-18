namespace Spark.Graphics
{
    using System;

    /// <summary>
    /// Defines different flip effects for sprites.
    /// </summary>
    [Flags]
    public enum SpriteFlipEffect
    {
        /// <summary>
        /// No rotation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Rotate 180 degrees about the Y-axis, mirroring horizontally.
        /// </summary>
        FlipHorizontally = 1,

        /// <summary>
        /// Rotate 180 degrees about the X-Axis, mirroring vertically.
        /// </summary>
        FlipVertically = 2
    }
}
