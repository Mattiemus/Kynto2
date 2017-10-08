namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

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
        /// <param name="vertexCount">The number of vertices the buffer will contain.</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        /// <returns>The buffer implementation.</returns>
        public IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, int vertexCount, ResourceUsage resourceUsage)
        {
            return new OpenGLVertexBufferImplementation(OpenGLRenderSystem, vertexLayout, vertexCount, resourceUsage);
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">The vertex data to initialize the vertex buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        public IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
        {
            return new OpenGLVertexBufferImplementation(OpenGLRenderSystem, vertexLayout, resourceUsage, data);
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">Array of databuffers to initialize the vertex buffer with, each databuffer corresponds to a single vertex element.</param>
        /// <returns>The buffer implementation.</returns>
        public IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            return new OpenGLVertexBufferImplementation(OpenGLRenderSystem, vertexLayout, resourceUsage, data);
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
