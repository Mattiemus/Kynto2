namespace Spark.Graphics
{
    /// <summary>
    /// Format for the index buffer.
    /// </summary>
    public enum IndexFormat
    {
        /// <summary>
        /// Short format type.
        /// </summary>
        SixteenBits,

        /// <summary>
        /// Int format type.
        /// </summary>
        ThirtyTwoBits
    }

    /// <summary>
    /// Extensions to the <see cref="IndexFormat"/> enum type
    /// </summary>
    public static class IndexFormatExtensions
    {
        /// <summary>
        /// Gets the size of the index format in bytes.
        /// </summary>
        /// <param name="format">Index format.</param>
        /// <returns>Size of the format, in bytes.</returns>
        public static int SizeInBytes(this IndexFormat format)
        {
            switch (format)
            {
                case IndexFormat.ThirtyTwoBits:
                    return sizeof(int);
                case IndexFormat.SixteenBits:
                    return sizeof(short);
                default:
                    return 0;
            }
        }
    }
}
