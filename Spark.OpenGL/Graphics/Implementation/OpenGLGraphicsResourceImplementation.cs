namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics.Implementation;

    /// <summary>
    /// OpenGL implementation of a graphics resource
    /// </summary>
    public abstract class OpenGLGraphicsResourceImplementation : GraphicsResourceImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLGraphicsResourceImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        protected OpenGLGraphicsResourceImplementation(OpenGLRenderSystem renderSystem)
            : base(renderSystem, renderSystem.GetNextUniqueResourceId())
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            OpenGLRenderSystem = renderSystem;
        }

        /// <summary>
        /// Gets the parent render system
        /// </summary>
        protected OpenGLRenderSystem OpenGLRenderSystem { get; }
    }
}
