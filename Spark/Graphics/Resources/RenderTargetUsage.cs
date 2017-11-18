namespace Spark.Graphics
{
    /// <summary>
    /// Determines how data is used when a render target is activated.
    /// </summary>
    public enum RenderTargetUsage
    {
        /// <summary>
        /// Always clear the contents of the target.
        /// </summary>
        DiscardContents = 0,

        /// <summary>
        /// Always keep the contents of the target
        /// </summary>
        PreserveContents = 1,

        /// <summary>
        /// Use the platform default, which is either discard or preserve.
        /// </summary>
        PlatformDefault = 2
    }
}
