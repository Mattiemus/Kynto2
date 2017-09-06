namespace Spark.Core
{
    /// <summary>
    /// Defines groups of platform initializers.
    /// </summary>
    public static class Platforms
    {
        /// <summary>
        /// Gets the platform which uses OpenTK to provide cross platform support
        /// </summary>
        public static IPlatformInitializer GeneralCrossPlatformInitializer 
            => new PlatformInitializer(new []
                {
                    new ServiceDescriptor("Spark.OpenGL", "Spark.Graphics.IRenderSystem", "Spark.OpenGL.Graphics.OpenGLRenderSystem")
                });
    }
}
