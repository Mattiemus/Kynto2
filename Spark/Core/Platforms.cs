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
        public static IPlatformInitializer Windows
            => new PlatformInitializer(new []
                {
                    new ServiceDescriptor("Spark.Direct3D11", "Spark.Graphics.IRenderSystem", "Spark.Direct3D11.Graphics.D3D11RenderSystem"),
                    new ServiceDescriptor("Spark.Windows", "Spark.Application.IApplicationSystem", "Spark.Windows.Application.WinFormsApplicationSystem"),
                    new ServiceDescriptor("Spark.Windows", "Spark.Input.IMouseInputSystem", "Spark.Windows.Input.Win32MouseInputSystem"),
                    new ServiceDescriptor("Spark.Windows", "Spark.Input.IKeyboardInputSystem", "Spark.Windows.Input.Win32KeyboardInputSystem")
                });
    }
}
