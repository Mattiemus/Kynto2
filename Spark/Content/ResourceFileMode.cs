namespace Spark.Content
{
    /// <summary>
    /// Enumerates how a resource/repository file should be opened.
    /// </summary>
    public enum ResourceFileMode
    {
        /// <summary>
        /// A new file is always created at the path specified, but if there is an existing file an exception gets thrown instead of
        /// it being overwritten.
        /// </summary>
        CreateNew = 1,

        /// <summary>
        /// A new file is always created at the path specified, overwriting an existing file.
        /// </summary>
        Create = 2,

        /// <summary>
        /// The file is opened, if it exists.
        /// </summary>
        Open = 3,

        /// <summary>
        /// If the file cannot be found, it will be created at the path specified and then opened. If it is found, it is simply opened.
        /// </summary>
        OpenOrCreate = 4
    }
}
