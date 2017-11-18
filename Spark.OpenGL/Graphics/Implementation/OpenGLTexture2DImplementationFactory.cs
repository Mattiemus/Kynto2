namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    
    /// <summary>
    /// Factory for creating <see cref="OpenGLTexture2DImplementation"/> instances
    /// </summary>
    public sealed class OpenGLTexture2DImplementationFactory : OpenGLGraphicsResourceImplementationFactory, ITexture2DImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLTexture2DImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLTexture2DImplementationFactory(OpenGLRenderSystem renderSystem)
            : base(renderSystem, typeof(VertexBuffer))
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
            return null;
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
            return null;
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<ITexture2DImplementationFactory>(this);
        }
    }
}
