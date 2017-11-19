namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// Factory for creating <see cref="OpenGLDepthStencilStateImplementationFactory"/> instances
    /// </summary>
    public sealed class OpenGLDepthStencilStateImplementationFactory : OpenGLGraphicsResourceImplementationFactory, IDepthStencilStateImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLDepthStencilStateImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLDepthStencilStateImplementationFactory(OpenGLRenderSystem renderSystem)
            : base(renderSystem, typeof(DepthStencilState))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        public IDepthStencilStateImplementation CreateImplementation()
        {
            return new OpenGLDepthStencilStateImplementation(OpenGLRenderSystem, OpenGLRenderSystem.GetNextUniqueResourceId());
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<IDepthStencilStateImplementationFactory>(this);
        }
    }
}
