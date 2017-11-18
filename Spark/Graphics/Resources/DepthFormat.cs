namespace Spark.Graphics
{
    /// <summary>
    /// Defines the format of a RenderTarget's depth buffer.
    /// </summary>
    public enum DepthFormat
    {
        /// <summary>
        /// No depth buffer to be created.
        /// </summary>
        None = 0,

        /// <summary>
        /// A depth buffer that holds 16-bit depth data.
        /// </summary>
        Depth16 = 1,

        /// <summary>
        /// A depth buffer that holds 32-bit data, where 24 bits are for depth and 8 are for stencil.
        /// </summary>
        Depth24Stencil8 = 2,

        /// <summary>
        /// A depth buffer that holds 32-bit floating point depth data.
        /// </summary>
        Depth32 = 3,

        /// <summary>
        /// A depth buffer that holds 32-bit floating point depth data with an additional 8 bits for stencil.
        /// </summary>
        Depth32Stencil8 = 4
    }
}
