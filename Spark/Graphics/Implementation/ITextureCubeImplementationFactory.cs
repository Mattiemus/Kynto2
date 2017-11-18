namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="ITextureCubeImplementation"/>.
    /// </summary>
    public interface ITextureCubeImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="size">Size of the texture (width/height), in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <returns>The texture implementation.</returns>
        ITextureCubeImplementation CreateImplementation(int size, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage);

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="size">Size of the texture (width/height), in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of mip levels and each data buffer must not exceed the size of the corresponding mip level, and is
        /// permitted to be null.</param>
        /// <returns>The texture implementation.</returns>
        ITextureCubeImplementation CreateImplementation(int size, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data);
    }
}
