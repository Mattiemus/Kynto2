namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="ITexture1DImplementation"/>.
    /// </summary>
    public interface ITexture1DImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <returns>The texture implementation.</returns>
        ITexture1DImplementation CreateImplementation(int width, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage);

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of mip levels and each data buffer must not exceed the size of the corresponding mip level, and is
        /// permitted to be null.</param>
        /// <returns>The texture implementation.</returns>
        ITexture1DImplementation CreateImplementation(int width, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data);
    }
}
