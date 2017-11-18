namespace Spark
{
    /// <summary>
    /// Defines groups of platform initializers.
    /// </summary>
    public static class Platforms
    {
        /// <summary>
        /// Gets the platform which uses OpenGL to provide cross platform support
        /// </summary>
        public static IPlatformInitializer WindowsOpenGLBasicInputNoSound
            => new PlatformInitializer(new []
                {
                    new ServiceDescriptor("Spark.OpenGL", "Spark.Application.IApplicationSystem", "Spark.OpenGL.Application.OpenGLApplicationSystem"),
                    new ServiceDescriptor("Spark.OpenGL", "Spark.Graphics.IRenderSystem", "Spark.OpenGL.Graphics.OpenGLRenderSystem"),
                    new ServiceDescriptor("Spark.Windows", "Spark.Input.IMouseInputSystem", "Spark.Windows.Input.Win32MouseInputSystem"),
                    new ServiceDescriptor("Spark.Windows", "Spark.Input.IKeyboardInputSystem", "Spark.Windows.Input.Win32KeyboardInputSystem")
                });
    }
}
