namespace Spark.Core
{
    /// <summary>
    /// Defines groups of platform initializers.
    /// </summary>
    public static class Platforms
    {
        /// <summary>
        /// Gets the platform which uses OpenGL under windows
        /// </summary>
        public static IPlatformInitializer WindowsOpenGLPlatform
        {
            get
            {
                return new PlatformInitializer(new[]
                {
                    new ServiceDescriptor("Spark.OpenGL", "Spark.Graphics.IRenderSystem", "Spark.Graphics.OpenGLRenderSystem")
                });
            }
        }
    }
}
