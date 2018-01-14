namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="IIndexBufferImplementation" />.
    /// </summary>
    public sealed class D3D11IndexBufferImplementationFactory : D3D11GraphicsResourceImplementationFactory, IIndexBufferImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11IndexBufferImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11IndexBufferImplementationFactory(D3D11RenderSystem renderSystem)
            : base(renderSystem, typeof(IndexBuffer))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="format">The index format.</param>
        /// <param name="indexCount">Number of indices the buffer will contain.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <returns>The buffer implementation.</returns>
        public IIndexBufferImplementation CreateImplementation(IndexFormat format, int indexCount, ResourceUsage resourceUsage)
        {
            return new D3D11IndexBufferImplementation(D3DRenderSystem, GetNextUniqueResourceId(), format, indexCount, resourceUsage);
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="format">The index format.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        public IIndexBufferImplementation CreateImplementation(IndexFormat format, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
        {
            return new D3D11IndexBufferImplementation(D3DRenderSystem, GetNextUniqueResourceId(), format, resourceUsage, data);
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">The index data to initialize the buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        public IIndexBufferImplementation CreateImplementation(ResourceUsage resourceUsage, IndexData data)
        {
            return new D3D11IndexBufferImplementation(D3DRenderSystem, GetNextUniqueResourceId(), resourceUsage, data);
        }
    }
}
