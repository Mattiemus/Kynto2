namespace Spark.Graphics
{
    /// <summary>
    /// Enumeration for texture dimensions.
    /// </summary>
    public enum TextureDimension
    {
        /// <summary>
        /// One dimensional texture.
        /// </summary>
        One = 0,

        /// <summary>
        /// Two dimensional texture.
        /// </summary>
        Two = 1,

        /// <summary>
        /// Three dimensional texture.
        /// </summary>
        Three = 2,

        /// <summary>
        /// An array of 6 two dimensional textures where each texture is a face of a cube.
        /// </summary>
        Cube = 3
    }
}
