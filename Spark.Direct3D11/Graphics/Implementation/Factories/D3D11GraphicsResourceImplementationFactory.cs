namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    
    /// <summary>
    /// Base class for a graphics resource implementation factory
    /// </summary>
    public abstract class D3D11GraphicsResourceImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11GraphicsResourceImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        /// <param name="resourceType">Type of graphics resource this factory constructs</param>
        protected D3D11GraphicsResourceImplementationFactory(D3D11RenderSystem renderSystem, Type resourceType)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem));
            }

            if (resourceType == null)
            {
                throw new ArgumentNullException(nameof(resourceType));
            }

            D3DRenderSystem = renderSystem;
            GraphicsResourceType = resourceType;
        }

        /// <summary>
        /// Gets the render system this factory binds resources to.
        /// </summary>
        public IRenderSystem RenderSystem => D3DRenderSystem;

        /// <summary>
        /// Gets the type of graphics resource this implementation factory creates implementations for.
        /// </summary>
        public Type GraphicsResourceType { get; }

        /// <summary>
        /// Gets the render system this factory binds resources to
        /// </summary>
        protected D3D11RenderSystem D3DRenderSystem { get; }

        /// <summary>
        /// Gets the next available resource id from the render system.
        /// </summary>
        /// <returns>Next unique resource id.</returns>
        protected int GetNextUniqueResourceId()
        {
            return D3DRenderSystem.GetNextUniqueResourceId();
        }
    }
}
