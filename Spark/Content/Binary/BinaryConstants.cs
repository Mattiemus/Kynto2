namespace Spark.Content.Binary
{
    using System.Text;
    using System.Collections.Generic;

    /// <summary>
    /// Static binary reader and writer constants
    /// </summary>
    public static class BinaryConstants
    {
        /// <summary>
        /// Savable version number
        /// </summary>
        public const short VERSION_NUMBER = 12;

        /// <summary>
        /// Size of the header block in bytes
        /// </summary>
        public const int HEADER_SIZE_IN_BYTES = 32;

        /// <summary>
        /// Magic word which identifies the file as a readable binary file
        /// </summary>
        public static readonly IReadOnlyList<byte> MAGIC_WORD = Encoding.ASCII.GetBytes("SPRK");

        /// <summary>
        /// Standard identifier for a null object.
        /// </summary>
        public const sbyte NULL_OBJECT = -1;

        /// <summary>
        /// Standard identifier for a non-null object.
        /// </summary>
        public const sbyte A_OK = 1;

        /// <summary>
        /// Identifies if a group header does not have an array count.
        /// </summary>
        public const sbyte GROUP_HEADER_NOARRAY = 2;
    }
}
