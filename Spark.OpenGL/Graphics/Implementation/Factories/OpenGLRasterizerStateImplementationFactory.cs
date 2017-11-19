namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// Factory for creating <see cref="OpenGLRasterizerStateImplementationFactory"/> instances
    /// </summary>
    public sealed class OpenGLRasterizerStateImplementationFactory : OpenGLGraphicsResourceImplementationFactory, IRasterizerStateImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLRasterizerStateImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLRasterizerStateImplementationFactory(OpenGLRenderSystem renderSystem)
            : base(renderSystem, typeof(RasterizerState))
        {
        }
        
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        public IRasterizerStateImplementation CreateImplementation()
        {
            return new OpenGLRasterizerStateImplementation(OpenGLRenderSystem, OpenGLRenderSystem.GetNextUniqueResourceId());
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<IRasterizerStateImplementationFactory>(this);
        }
    }
}
