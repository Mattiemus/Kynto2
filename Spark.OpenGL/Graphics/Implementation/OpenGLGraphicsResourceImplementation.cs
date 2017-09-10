namespace Spark.OpenGL.Graphics.Implementation
{
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
            RenderSystem = renderSystem;
        }

        /// <summary>
        /// Gets the parent render system
        /// </summary>
        protected OpenGLRenderSystem RenderSystem { get; }
    }
}
