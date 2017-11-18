namespace Spark.Graphics
{
    /// <summary>
    /// Enumeration for valid filtering schemes involving shrinking (minify), expanding (magnify),
    /// or filtering between mipmap levels.
    /// </summary>
    public enum TextureFilter
    {
        /// <summary>
        /// Use point filtering.
        /// </summary>
        Point = 0,

        /// <summary>
        /// Use point filtering for min/mag and linear filtering for mip.
        /// </summary>
        PointMipLinear = 1,

        /// <summary>
        /// Use linear filtering.
        /// </summary>
        Linear = 2,

        /// <summary>
        /// Use linear filtering for min/mag and point filtering for mip.
        /// </summary>
        LinearMipPoint = 3,

        /// <summary>
        /// Use linear filtering for min and mip, but point for mag.
        /// </summary>
        MinLinearMagPointMipLinear = 4,

        /// <summary>
        /// Use linear filtering for min, and point filtering for both mag and mip.
        /// </summary>
        MinLinearMagPointMipPoint = 5,

        /// <summary>
        /// Use point filtering for min, and linear filtering for both mag and mip.
        /// </summary>
        MinPointMagLinearMipLinear = 6,

        /// <summary>
        /// Use point filtering for min and mip, but point for mip.
        /// </summary>
        MinPointMagLinearMipPoint = 7,

        /// <summary>
        /// Use anisotropic filtering.
        /// </summary>
        Anisotropic = 8
    }
}
