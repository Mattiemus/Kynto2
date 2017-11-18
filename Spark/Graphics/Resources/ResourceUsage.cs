namespace Spark.Graphics
{
    /// <summary>
    /// Resource usage enumeration, defines how the resource can be accessed by the GPU/CPU.
    /// </summary>
    public enum ResourceUsage
    {
        /// <summary>
        /// Resource is GPU-accessible (read/write) only, the default usage.
        /// </summary>
        Static = 0,

        /// <summary>
        /// Resource is write-accessible by CPU and read-accessible by GPU. Typically for
        /// resources that update every frame.
        /// </summary>
        Dynamic = 1,

        /// <summary>
        /// Resource is GPU-read only, meaning once created it can never change.
        /// </summary>
        Immutable = 2
    }
}
