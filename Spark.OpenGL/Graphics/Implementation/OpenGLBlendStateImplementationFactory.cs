namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// Factory for creating <see cref="OpenGLBlendStateImplementationFactory"/> instances
    /// </summary>
    public sealed class OpenGLBlendStateImplementationFactory : OpenGLGraphicsResourceImplementationFactory, IBlendStateImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLBlendStateImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLBlendStateImplementationFactory(OpenGLRenderSystem renderSystem)
            : base(renderSystem, typeof(BlendState))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        public IBlendStateImplementation CreateImplementation()
        {
            return new OpenGLBlendStateImplementation(OpenGLRenderSystem, OpenGLRenderSystem.GetNextUniqueResourceId());
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<IBlendStateImplementationFactory>(this);
        }
    }
}
