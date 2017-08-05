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
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, int vertexCount);

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="data">The interleaved vertex data to initialize the vertex buffer with.</param>
        IVertexBufferImplementation CreateImplementation(VertexLayout vertexLayout, IReadOnlyDataBuffer data);
    }
}
