namespace Spark.Graphics
{
    using Math;

    /// <summary>
    /// Enumeration of possible vertex formats
    /// </summary>
    public enum VertexFormat
    { 
        Float = 1,
        Float2 = 2,
        Float3 = 3,
        Float4 = 4
    }

    /// <summary>
    /// Extensions to the <see cref="VertexFormat"/> enum type
    /// </summary>
    public static class VertexFormatExtensions
    {
        /// <summary>
        /// Gets the size of the vertex format in bytes.
        /// </summary>
        /// <param name="format">Vertex format</param>
        /// <returns>Size of the format, in bytes.</returns>
        public static int SizeInBytes(this VertexFormat format)
        {
            switch (format)
            {
                case VertexFormat.Float:
                    return sizeof(float);
                case VertexFormat.Float2:
                    return Vector2.SizeInBytes;
                case VertexFormat.Float3:
                    return Vector3.SizeInBytes;
                case VertexFormat.Float4:
                    return Vector4.SizeInBytes;
                default:
                    return 0;
            }
        }
    }
}
