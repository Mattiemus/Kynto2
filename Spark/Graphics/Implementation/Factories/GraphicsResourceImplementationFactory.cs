namespace Spark.Graphics.Implementation
{
    using System;

    /// <summary>
    /// Base class for a graphics resource implementation factory
    /// </summary>
    public abstract class GraphicsResourceImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsResourceImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        /// <param name="resourceType">Type of graphics resource this factory constructs</param>
        protected GraphicsResourceImplementationFactory(IRenderSystem renderSystem, Type resourceType)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem));
            }

            if (resourceType == null)
            {
                throw new ArgumentNullException(nameof(resourceType));
            }

            RenderSystem = renderSystem;
            GraphicsResourceType = resourceType;
        }

        /// <summary>
        /// Gets the render system this factory binds resources to.
        /// </summary>
        public IRenderSystem RenderSystem { get; }

        /// <summary>
        /// Gets the type of graphics resource this implementation factory creates implementations for.
        /// </summary>
        public Type GraphicsResourceType { get; }
    }
}
