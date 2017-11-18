namespace Spark.Graphics
{
    /// <summary>
    /// Defines a surface's encoding format.
    /// </summary>
    public enum SurfaceFormat
    {
        /// <summary>
        /// Unsigned 32-Bit RGB pixel format with alpha, 8 bits per channel.
        /// </summary>
        Color = 0,

        /// <summary>
        /// Unsigned 32-bit BGR pixel format with alpha, 8 bits per channel. This differs from the default color format where 
        /// the R and B components are swapped.
        /// </summary>
        BGRColor = 1,

        /// <summary>
        /// Unsigned 16-bit BGR pixel format with 5 bits for blue, 6 bits for green, and 5 bits for red.
        /// </summary>
        BGR565 = 2,

        /// <summary>
        /// Unsigned 16-bit BGRA pixel format with 5 bits for r,g,b channels and 1 bit for alpha.
        /// </summary>
        BGRA5551 = 3,

        /// <summary>
        /// DXT1 compression texture format. Surface dimensions must be in multiples of 4.
        /// </summary>
        DXT1 = 4,

        /// <summary>
        /// DXT3 compression texture format. Surface dimensions must be in multiples of 4.
        /// </summary>
        DXT3 = 5,

        /// <summary>
        /// DXT5 compression texture format. Surface dimensions must be in multiples of 4.
        /// </summary>
        DXT5 = 6,

        /// <summary>
        /// Unsigned 32-bit RGBA pixel format with 10 bits for r,g,b, channels and 2 bits for alpha.
        /// </summary>
        RGBA1010102 = 7,

        /// <summary>
        /// Unsigned 32-bit RG pixel format using 16 bits for red and green.
        /// </summary>
        RG32 = 8,

        /// <summary>
        /// Unsigned 64-bit RGBA pixel format using 16 bit for r,g,b,a channels.
        /// </summary>
        RGBA64 = 9,

        /// <summary>
        /// Unsigned 8-bit alpha.
        /// </summary>
        Alpha8 = 10,

        /// <summary>
        /// IEEE 32-bit float format using 32 bits for the red channel.
        /// </summary>
        Single = 11,

        /// <summary>
        /// IEEE 64-bit float format using 32 bits for red and green.
        /// </summary>
        Vector2 = 12,

        /// <summary>
        /// IEEE 96-bit float format using 32 bits for red, green, blue.
        /// </summary>
        Vector3 = 13,

        /// <summary>
        /// IEEE 128-bit float format using 32 bits for r,g,b,a channels.
        /// </summary>
        Vector4 = 14
    }

    /// <summary>
    /// Extensions to the <see cref="SurfaceFormat"/> enum type
    /// </summary>
    public static class SurfaceFormatExtensions
    {
        /// <summary>
        /// Gets the size of the surface format in bytes.
        /// </summary>
        /// <param name="format">Surface format</param>
        /// <returns>Size of the format, in bytes.</returns>
        public static int SizeInBytes(this SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Vector4:
                    return 16;
                case SurfaceFormat.Vector3:
                    return 12;
                case SurfaceFormat.Vector2:
                    return 8;
                case SurfaceFormat.Single:
                case SurfaceFormat.Color:
                case SurfaceFormat.BGRColor:
                case SurfaceFormat.RGBA1010102:
                case SurfaceFormat.RG32:
                    return 4;
                case SurfaceFormat.BGR565:
                case SurfaceFormat.BGRA5551:
                case SurfaceFormat.DXT3:
                case SurfaceFormat.DXT5:
                    return 2;
                case SurfaceFormat.DXT1:
                case SurfaceFormat.Alpha8:
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets if the format is compressed or not.
        /// </summary>
        /// <param name="format">Surface format</param>
        /// <returns>True if the format is compressed, false otherwise.</returns>
        public static bool IsCompressedFormat(this SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.DXT1:
                case SurfaceFormat.DXT3:
                case SurfaceFormat.DXT5:
                    return true;
                default:
                    return false;
            }
        }
    }
}
