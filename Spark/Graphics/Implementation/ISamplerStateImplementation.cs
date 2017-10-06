namespace Spark.Graphics.Implementation
{
    using Math;

    /// <summary>
    /// Defines an implementation for <see cref="SamplerState"/>.
    /// </summary>
    public interface ISamplerStateImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        bool IsBound { get; }

        /// <summary>
        /// Gets the number of anisotropy levels supported. This can vary by implementation.
        /// </summary>
        int SupportedAnisotropyLevels { get; }

        /// <summary>
        /// Gets or sets the addressing mode for the U coordinate. By default, this value is <see cref="TextureAddressMode.Clamp"/>.
        /// </summary>
        TextureAddressMode AddressU { get; set; }

        /// <summary>
        /// Gets or sets the addressing mode for the V coordinate. By default, this value is <see cref="TextureAddressMode.Clamp"/>.
        /// </summary>
        TextureAddressMode AddressV { get; set; }

        /// <summary>
        /// Gets or sets the addressing mode for the W coordinate. By default, this value is <see cref="TextureAddressMode.Clamp"/>.
        /// </summary>
        TextureAddressMode AddressW { get; set; }

        /// <summary>
        /// Gets or sets the filtering used during texture sampling. By default, this value is <see cref="TextureFilter.Linear"/>.
        /// </summary>
        TextureFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets the maximum anisotropy. This is used to clamp values when the filter is set to anisotropic. By default, this value is
        /// <see cref="SupportedAnisotropyLevels"/> as it can vary by implementation. If a higher or lower value is set than supported, it is clamped.
        /// </summary>
        int MaxAnisotropy { get; set; }

        /// <summary>
        /// Gets or sets the mipmap LOD bias. This is the offset from the calculated mipmap level that is actually used (e.g. sampled at mipmap level 3 with offset 2, then the
        /// mipmap at level 5 is sampled). By default, this value is zero.
        /// </summary>
        float MipMapLevelOfDetailBias { get; set; }

        /// <summary>
        /// Gets or sets the lower bound of the mipmap range [0, n-1] to clamp access to, where zero is the largest and most detailed mipmap level. The level n-1 is the least detailed mipmap level.
        /// By default, this value is zero.
        /// </summary>
        int MinMipMapLevel { get; set; }

        /// <summary>
        /// Gets or sets the upper bound of the mipmap range [0, n-1] to clamp access to, where zero is the largest and most detailed mipmap level. The level n-1 is the least detailed mipmap level.
        /// By default, this value is <see cref="int.MaxValue"/>.
        /// </summary>
        int MaxMipMapLevel { get; set; }

        /// <summary>
        /// Gets or sets the border color if the texture addressing is set to border. By default, this value is <see cref="Color.TransparentBlack"/>.
        /// </summary>
        Color BorderColor { get; set; }

        /// <summary>
        /// Checks if the specified texture addressing mode is supported by the graphics platform.
        /// </summary>
        /// <param name="mode">Texture addressing mode</param>
        /// <returns>True if supported, false otherwise.</returns>
        bool IsAddressingModeSupported(TextureAddressMode mode);

        /// <summary>
        /// Binds the implementation, creating the underlying state. Once bound the state is read-only. If unbound, this will happen
        /// automatically when the state is first used during rendering. It is best practice to do this ahead of time.
        /// </summary>
        void BindSamplerState();
    }
}
