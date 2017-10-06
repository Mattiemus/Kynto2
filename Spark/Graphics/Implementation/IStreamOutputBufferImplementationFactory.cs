namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="IStreamOutputBufferImplementation"/>.
    /// </summary>
    public interface IStreamOutputBufferImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="vertexCount">The number of vertices the buffer will contain.</param>
        /// <returns>The buffer implementation.</returns>
        IStreamOutputBufferImplementation CreateImplementation(VertexLayout vertexLayout, int vertexCount);
    }
}
