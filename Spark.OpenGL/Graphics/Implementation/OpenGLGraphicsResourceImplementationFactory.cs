namespace Spark.Graphics.Implementation
{
    using System;

    /// <summary>
    /// Base class for an OpenGL graphics resource factory
    /// </summary>
    public abstract class OpenGLGraphicsResourceImplementationFactory : GraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsResourceImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        /// <param name="resourceType">Type of graphics resource this factory constructs</param>
        protected OpenGLGraphicsResourceImplementationFactory(OpenGLRenderSystem renderSystem, Type resourceType)
            : base(renderSystem, resourceType)
        {
            OpenGLRenderSystem = renderSystem;
        }

        /// <summary>
        /// Gets the concrete render system implementation
        /// </summary>
        protected OpenGLRenderSystem OpenGLRenderSystem { get; }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public abstract void Initialize();
    }
}
