namespace Spark.Content
{
    using System;

    /// <summary>
    /// Enumerates what access other resource streams can have on an open file.
    /// </summary>
    [Flags]
    public enum ResourceFileShare
    {
        /// <summary>
        /// Declines sharing of the current file.
        /// </summary>
        None = 0,

        /// <summary>
        /// Allows subsequent opening of the file for reading.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Allows subsequent opening of the file for writing.
        /// </summary>
        Write = 2,

        /// <summary>
        /// Allows subsequent opening of the file for reading or writing.
        /// </summary>
        ReadWrite = 3,

        /// <summary>
        /// Allows subsequent deleting of a file.
        /// </summary>
        Delete = 4
    }
}
