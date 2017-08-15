namespace Spark.Graphics
{
    using System;

    using Core;
    
    /// <summary>
    /// General graphics helper
    /// </summary>
    public static class GraphicsHelpers
    {
        /// <summary>
        /// Gets a render system from the engine service providers.
        /// </summary>
        /// <returns>Render system in the engine service provider.</returns>
        public static IRenderSystem GetRenderSystem()
        {
            if (!Engine.IsInitialized)
            {
                throw new SparkException("Engine is not initialized");
            }

            IRenderSystem renderSystem = Engine.Instance.Services.GetService<IRenderSystem>();
            if (renderSystem == null)
            {
                throw new SparkGraphicsException("No render system has been registered with the engine");
            }

            return renderSystem;
        }

        /// <summary>
        /// Gets a render system from the service provider.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <returns>Render system in the service provider.</returns>
        public static IRenderSystem GetRenderSystem(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            IRenderSystem renderSystem = serviceProvider.GetService(typeof(IRenderSystem)) as IRenderSystem;
            if (renderSystem == null)
            {
                throw new SparkGraphicsException("No render system has been registered with the engine");
            }

            return renderSystem;
        }
    }
}
