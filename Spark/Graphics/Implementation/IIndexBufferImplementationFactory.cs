namespace Spark.Graphics.Implementation
{
    using Core;
    using Graphics.Geometry;

    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="IIndexBufferImplementation"/>.
    /// </summary>
    public interface IIndexBufferImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="format">The index format.</param>
        /// <param name="indexCount">Number of indices the buffer will contain.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <returns>The buffer implementation.</returns>
        IIndexBufferImplementation CreateImplementation(IndexFormat format, int indexCount, ResourceUsage resourceUsage);

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="format">The index format.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        IIndexBufferImplementation CreateImplementation(IndexFormat format, ResourceUsage resourceUsage, IReadOnlyDataBuffer data);

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">The index data to initialize the buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        IIndexBufferImplementation CreateImplementation(ResourceUsage resourceUsage, IndexData data);
    }
}
