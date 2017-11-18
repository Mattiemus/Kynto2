namespace Spark.Graphics
{
    /// <summary>
    /// Addressing mode for textures.
    /// </summary>
    public enum TextureAddressMode
    {
        /// <summary>
        /// Tile the texture at every integer junction. E.g. For u values
        /// between 0 and 2, the texture is repeated twice. No mirroring is performed.
        /// </summary>
        Wrap = 0,

        /// <summary>
        /// Texture coordinates are clamped to the range [0.0, 1.0]
        /// </summary>
        Clamp = 1,

        /// <summary>
        /// Texture coordinates outside the range [0.0, 1.0] are set to the border color specified by a currently bound sampler state.
        /// </summary>
        Border = 2,

        /// <summary>
        /// Similar to wrap except the texture is flipped at every integer junction. E.g. for
        /// u values between 0 and 1, the texture is addressed normally but between 1 and 2,
        /// it is flipped and so on.
        /// </summary>
        Mirror = 3,

        /// <summary>
        /// Similar to mirror and clamp. Takes the absolute value of the texture coordinate (thus mirroring around 0) 
        /// and clamps to the maximum value
        /// </summary>
        MirrorOnce = 4
    }
}
