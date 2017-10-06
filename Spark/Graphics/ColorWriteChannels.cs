namespace Spark.Graphics
{
    using System;

    /// <summary>
    /// Defines the channels that can be written to
    /// a render target's color buffer.
    /// </summary>
    [Flags]
    public enum ColorWriteChannels
    {
        /// <summary>
        /// Write to no channels.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only write to the red (R) channel.
        /// </summary>
        Red = 1,

        /// <summary>
        /// Only write to the green (G) channel.
        /// </summary>
        Green = 2,

        /// <summary>
        /// Only write to the blue (B) channel.
        /// </summary>
        Blue = 4,

        /// <summary>
        /// Only write to the alpha (A) channel.
        /// </summary>
        Alpha = 8,

        /// <summary>
        /// Write to all (RGBA) channels.
        /// </summary>
        All = 15
    }
}
