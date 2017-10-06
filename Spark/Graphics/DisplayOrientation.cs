namespace Spark.Graphics
{
    using System;

    /// <summary>
    /// Defines the display orientation.
    /// </summary>
    [Flags]
    public enum DisplayOrientation
    {
        /// <summary>
        /// Default orientation.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Display is rotated CCW 90 degrees into a landscape orientation (width > height).
        /// </summary>
        LandscapeLeft = 1,

        /// <summary>
        /// Display is rotated CW 90 degrees into a landscape orientation (width > height).
        /// </summary>
        LandscapeRight = 2,

        /// <summary>
        /// Display is oriented where height is greater than width.
        /// </summary>
        Portrait = 4
    }
}
