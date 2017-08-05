namespace Spark.Graphics.Implementation
{
    using System;

    /// <summary>
    /// Defines a factory that creates platform-specific implementations that are bound to a specific graphics resource. These factories are registered to a 
    /// render system, and work in tandem with the specific graphics resource object to initialize its implementation.
    /// </summary>
    public interface IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Gets the render system this factory binds resources to.
        /// </summary>
        IRenderSystem RenderSystem { get; }

        /// <summary>
        /// Gets the type of graphics resource this implementation factory creates implementations for.
        /// </summary>
        Type GraphicsResourceType { get; }
    }
}
