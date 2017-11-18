namespace Spark.Graphics
{
    /// <summary>
    /// Defines the different options for setting render targets/depth buffers.
    /// </summary>
    public enum SetTargetOptions
    {
        /// <summary>
        /// Default behavior, set targets and a depth buffer of the first target (if any).
        /// </summary>
        None = 0,

        /// <summary>
        /// Set no depth buffer when setting render targets.
        /// </summary>
        NoDepthBuffer = 1,

        /// <summary>
        /// Set the depth buffer as read-only so it can be simultaneously set as a shader resource.
        /// </summary>
        ReadOnlyDepthBuffer = 2
    }
}
