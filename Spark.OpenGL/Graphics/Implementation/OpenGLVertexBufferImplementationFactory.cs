namespace Spark.Graphics.Implementation
{
    using Core;

    /// <summary>
    /// Factory for creating <see cref="OpenGLVertexBufferImplementation"/> instances
    /// </summary>
    public sealed class OpenGLVertexBufferImplementationFactory : OpenGLGraphicsResourceImplementationFactory, IVertexBufferImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLVertexBufferImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLVertexBufferImplementationFactory(OpenGLRenderSystem renderSystem)
            : base(renderSystem, typeof(VertexBuffer))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        public IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, int vertexCount)
        {
            return new OpenGLVertexBufferImplementation(OpenGLRenderSystem, vertexLayout, vertexCount);
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="data">The interleaved vertex data to initialize the vertex buffer with.</param>
        public IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, IReadOnlyDataBuffer data)
        {
            return new OpenGLVertexBufferImplementation(OpenGLRenderSystem, vertexLayout, data);
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<IVertexBufferImplementationFactory>(this);
        }
    }
}
