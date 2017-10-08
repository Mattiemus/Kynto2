namespace Spark.Graphics.Implementation
{
    using Core;

    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="IVertexBufferImplementation"/>.
    /// </summary>
    public interface IVertexBufferImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="vertexCount">The number of vertices the buffer will contain.</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        /// <returns>The buffer implementation.</returns>
        IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, int vertexCount, ResourceUsage resourceUsage);

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">The vertex data to initialize the vertex buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, ResourceUsage resourceUsage, IReadOnlyDataBuffer data);

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">Array of databuffers to initialize the vertex buffer with, each databuffer corresponds to a single vertex element.</param>
        /// <returns>The buffer implementation.</returns>
        IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data);
    }
}
