namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    
    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="ITexture2DImplementation"/>.
    /// </summary>
    public sealed class D3D11Texture2DImplementationFactory : D3D11GraphicsResourceImplementationFactory, ITexture2DImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11Texture2DImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11Texture2DImplementationFactory(D3D11RenderSystem renderSystem) 
            : base(renderSystem, typeof(Texture2D))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <returns>The texture implementation.</returns>
        public ITexture2DImplementation CreateImplementation(int width, int height, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage)
        {
            return new D3D11Texture2DImplementation(D3DRenderSystem, GetNextUniqueResourceId(), width, height, 1, mipMapCount, format, resourceUsage);
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of mip levels and each data buffer must not exceed the size of the corresponding mip level, and is
        /// permitted to be null.</param>
        /// <returns>The texture implementation.</returns>
        public ITexture2DImplementation CreateImplementation(int width, int height, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            return new D3D11Texture2DImplementation(D3DRenderSystem, GetNextUniqueResourceId(), width, height, 1, mipMapCount, format, resourceUsage, data);
        }
    }
}
