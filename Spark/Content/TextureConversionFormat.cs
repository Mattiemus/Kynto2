namespace Spark.Content
{
    /// <summary>
    /// Enumerates the options to tell the image importer what texture format the image should be converted to.
    /// </summary>
    public enum TextureConversionFormat
    {
        /// <summary>
        /// Keep the image's format, the importer will try and match to supported SurfaceFormats, whether its Color, DXT,
        /// or some other texture data. This is the default value.
        /// </summary>
        NoChange,

        /// <summary>
        /// Convert to the Color ARGB format.
        /// </summary>
        Color,

        /// <summary>
        /// Convert to DXT1.
        /// </summary>
        DXT1,

        /// <summary>
        /// Convert to DXT3.
        /// </summary>
        DXT3,

        /// <summary>
        /// Convert to DXT5.
        /// </summary>
        DXT5
    }
}
