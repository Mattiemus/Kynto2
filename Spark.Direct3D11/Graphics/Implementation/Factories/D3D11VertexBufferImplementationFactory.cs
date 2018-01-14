namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// A factory that creates Direct3D11 implementation of type <see cref="IVertexBufferImplementation"/>.
    /// </summary>
    public sealed class D3D11VertexBufferImplementationFactory : D3D11GraphicsResourceImplementationFactory, IVertexBufferImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11VertexBufferImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11VertexBufferImplementationFactory(D3D11RenderSystem renderSystem)
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
            return new D3D11VertexBufferImplementation(D3DRenderSystem, GetNextUniqueResourceId(), vertexLayout, vertexCount, resourceUsage);
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
            return new D3D11VertexBufferImplementation(D3DRenderSystem, GetNextUniqueResourceId(), vertexLayout, resourceUsage, data);
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
            return new D3D11VertexBufferImplementation(D3DRenderSystem, GetNextUniqueResourceId(), vertexLayout, resourceUsage, data);
        }
    }
}
