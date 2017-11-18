namespace Spark.Graphics
{
    using Math;

    /// <summary>
    /// Enumeration of possible vertex formats
    /// </summary>
    public enum VertexFormat
    {
        /// <summary>
        /// A vector containing four 8-bit unsigned bytes in normalized format (interpreted as normalized floating-point value in range of [0, 1]). Commonly used for RGBA color. 
        /// </summary>
        Color = 0,

        /// <summary>
        /// A 16-bit unsigned integer.
        /// </summary>
        UShort = 1,

        /// <summary>
        /// A vector containing two 16-bit unsigned integers for a total size of 32 bits.
        /// </summary>
        UShort2 = 2,

        /// <summary>
        /// A vector containing four 16-bit unsigned integers for a total size of 64 bits.
        /// </summary>
        UShort4 = 3,

        /// <summary>
        /// A 16-bit unsigned integer in normalized format (interpreted as normalized floating-point value in range of [0, 1]).
        /// </summary>
        NormalizedUShort = 4,

        /// <summary>
        /// A vector containing two 16-bit unsigned integers in normalized format (interpreted as normalized floating-point value in range of [0, 1]) for a total size of 32 bits.
        /// </summary>
        NormalizedUShort2 = 5,

        /// <summary>
        /// A vector containing four 16-bit unsigned integers in normalized format (interpreted as normalized floating-point value in range of [0, 1]) for a total size of 64 bits.
        /// </summary>
        NormalizedUShort4 = 6,

        /// <summary>
        /// A 16-bit integer.
        /// </summary>
        Short = 7,

        /// <summary>
        /// A vector containing two 16-bit integers for a total size of 32 bits.
        /// </summary>
        Short2 = 8,

        /// <summary>
        /// A vector containing four 16-bit integers for a total size of 64 bits.
        /// </summary>
        Short4 = 9,

        /// <summary>
        /// A 16-bit integer in normalized format (interpreted as normalized floating-point value in range of [-1, 1]).
        /// </summary>
        NormalizedShort = 10,

        /// <summary>
        /// A vector containing two 16-bit integers in normalized format (interpreted as normalized floating-point value in range of [-1, 1]) for a total size of 32 bits.
        /// </summary>
        NormalizedShort2 = 11,

        /// <summary>
        /// A vector containing four 16-bit integers in normalized format (interpreted as normalized floating-point value in range of [-1, 1]) for a total size of 64 bits.
        /// </summary>
        NormalizedShort4 = 12,

        /// <summary>
        /// A 32-bit unsigned integer.
        /// </summary>
        UInt = 13,

        /// <summary>
        /// A vector containing two 32-bit unsigned integers for a total size of 64 bits.
        /// </summary>
        UInt2 = 14,

        /// <summary>
        /// A vector containing three 32-bit unsigned integers for a total size of 96 bits.
        /// </summary>
        UInt3 = 15,

        /// <summary>
        /// A vector containing four 32-bit unsigned integers for a total size of 128 bits.
        /// </summary>
        UInt4 = 16,

        /// <summary>
        /// A 32-bit integer.
        /// </summary>
        Int = 17,

        /// <summary>
        /// A vector containing two 32-bit integers for a total size of 64 bits.
        /// </summary>
        Int2 = 18,

        /// <summary>
        /// A vector containing three 32-bit integers for a total size of 96 bits.
        /// </summary>
        Int3 = 19,

        /// <summary>
        /// A vector containing four 32-bit integers for a total size of 128 bits.
        /// </summary>
        Int4 = 20,

        /// <summary>
        /// A 16-bit float.
        /// </summary>
        Half = 21,

        /// <summary>
        /// A vector containing two 16-bit floats for a total size of 32 bits.
        /// </summary>
        Half2 = 22,

        /// <summary>
        /// A vector containing four 16-bit floats for a total size of 64 bits.
        /// </summary>
        Half4 = 23,

        /// <summary>
        /// A 32-bit float.
        /// </summary>
        Float = 24,

        /// <summary>
        /// A vector containing two 32-bit floats for a total size of 64 bits.
        /// </summary>
        Float2 = 25,

        /// <summary>
        /// A vector containing three 32-bit floats for a total size of 96 bits.
        /// </summary>
        Float3 = 26,

        /// <summary>
        /// A vector containing four 32-bit floats for a total size of 128 bits.
        /// </summary>
        Float4 = 27
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
                case VertexFormat.Color:
                    return Color.SizeInBytes;
                case VertexFormat.UShort:
                case VertexFormat.NormalizedUShort:
                case VertexFormat.Short:
                case VertexFormat.NormalizedShort:
                    return sizeof(short);
                case VertexFormat.UShort2:
                case VertexFormat.NormalizedUShort2:
                case VertexFormat.Short2:
                case VertexFormat.NormalizedShort2:
                    return sizeof(short) * 2;
                case VertexFormat.UShort4:
                case VertexFormat.NormalizedUShort4:
                case VertexFormat.Short4:
                case VertexFormat.NormalizedShort4:
                    return sizeof(short) * 4;
                case VertexFormat.UInt:
                case VertexFormat.Int:
                    return sizeof(int);
                case VertexFormat.UInt2:
                case VertexFormat.Int2:
                    return Int2.SizeInBytes;
                case VertexFormat.UInt3:
                case VertexFormat.Int3:
                    return Int3.SizeInBytes;
                case VertexFormat.UInt4:
                case VertexFormat.Int4:
                    return Int4.SizeInBytes;
                case VertexFormat.Half:
                    return 2; // TODO
                case VertexFormat.Half2:
                    return 4; //TODO
                case VertexFormat.Half4:
                    return 8; // TODO
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
