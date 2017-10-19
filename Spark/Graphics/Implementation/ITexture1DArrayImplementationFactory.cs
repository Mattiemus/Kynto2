namespace Spark.Graphics.Implementation
{
    using Core;

    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="ITexture1DArrayImplementation"/>.
    /// </summary>
    public interface ITexture1DArrayImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <returns>The texture implementation.</returns>
        ITexture1DArrayImplementation CreateImplementation(int width, int arrayCount, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage);

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of array slices * number of mip map levels. Data is assumed to be in the order of the array slices, 
        /// where the entire mip map chain of the first array slice comes first, then the next slice's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        /// <returns>The texture implementation.</returns>
        ITexture1DArrayImplementation CreateImplementation(int width, int arrayCount, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data);
    }
}
