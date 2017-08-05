namespace Spark.Graphics
{
    using System;

    /// <summary>
    /// Options to clear rendertargets and the back buffer.
    /// </summary>
    [Flags]
    public enum ClearOptions
    {
        /// <summary>
        /// No clear value
        /// </summary>
        None = 0,

        /// <summary>
        /// Clear depth buffer.
        /// </summary>
        Depth = 1,

        /// <summary>
        /// Clear stencil buffer.
        /// </summary>
        Stencil = 2,

        /// <summary>
        /// Clear the render target/color buffer.
        /// </summary>
        Target = 4,

        /// <summary>
        /// Clears the render target, depth, and stencil buffers.
        /// </summary>
        All = Target | Depth | Stencil
    }
}
