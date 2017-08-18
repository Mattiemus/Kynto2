namespace Spark.Content
{
    /// <summary>
    /// Enumerates how a resource file can be accessed.
    /// </summary>
    public enum ResourceFileAccess
    {
        /// <summary>
        /// The resource stream is open for read only.
        /// </summary>
        Read = 1,

        /// <summary>
        /// The resource stream is open for write only.
        /// </summary>
        Write = 2,

        /// <summary>
        /// The resource stream is open for both reading and writing.
        /// </summary>
        ReadWrite = 3
    }
}
