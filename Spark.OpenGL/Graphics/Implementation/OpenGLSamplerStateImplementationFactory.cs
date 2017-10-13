namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// Factory for creating <see cref="OpenGLSamplerStateImplementationFactory"/> instances
    /// </summary>
    public sealed class OpenGLSamplerStateImplementationFactory : OpenGLGraphicsResourceImplementationFactory, ISamplerStateImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLSamplerStateImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLSamplerStateImplementationFactory(OpenGLRenderSystem renderSystem)
            : base(renderSystem, typeof(SamplerState))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        public ISamplerStateImplementation CreateImplementation()
        {
            return new OpenGLSamplerStateImplementation(OpenGLRenderSystem, OpenGLRenderSystem.GetNextUniqueResourceId());
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<ISamplerStateImplementationFactory>(this);
        }
    }
}
