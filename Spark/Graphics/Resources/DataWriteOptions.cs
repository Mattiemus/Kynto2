namespace Spark.Graphics
{
    /// <summary>
    /// Options for writing data to a hardware resource.
    /// </summary>
    public enum DataWriteOptions
    {
        /// <summary>
        /// Default writing mode. Buffer may be overwritten using this.
        /// </summary>
        None = 0,

        /// <summary>
        /// This writing mode will discard all contents of the buffer. Only applicable for Dynamic 
        /// buffers.
        /// </summary>
        Discard = 1,

        /// <summary>
        /// This writing mode will not overwrite existing contents of the buffer. Only applicable
        /// for dynamic buffers.
        /// </summary>
        NoOverwrite = 2
    }
}
