namespace Spark.Core
{
    /// <summary>
    /// Defines groups of platform initializers.
    /// </summary>
    public static class Platforms
    {
        /// <summary>
        /// Gets the platform which uses OpenGL to provide cross platform support
        /// </summary>
        public static IPlatformInitializer OpenGLPlatformInitializer 
            => new PlatformInitializer(new []
                {
                    new ServiceDescriptor("Spark.OpenGL", "Spark.Application.IApplicationSystem", "Spark.OpenGL.Application.OpenGLApplicationSystem"),
                    new ServiceDescriptor("Spark.OpenGL", "Spark.Graphics.IRenderSystem", "Spark.OpenGL.Graphics.OpenGLRenderSystem")
                });
    }
}
