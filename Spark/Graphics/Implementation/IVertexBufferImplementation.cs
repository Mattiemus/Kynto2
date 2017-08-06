namespace Spark.Graphics.Implementation
{
    using Core;

    /// <summary>
    /// Defines an implementation for <see cref="VertexBuffer"/>.
    /// </summary>
    public interface IVertexBufferImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets the vertex layout that describes the structure of vertex data contained in the buffer.
        /// </summary>
        VertexLayout VertexLayout { get; }

        /// <summary>
        /// Gets the number of vertices contained in the buffer.
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// Reads data from the vertex buffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the vertex buffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        /// <param name="elementCount">Number of elements to read.</param>
        /// <param name="offsetInBytes">Offset in bytes from the beginning of the vertex buffer to the data.</param>
        /// <param name="vertexStride">Size of an element in bytes</param>
        void GetData<T>(IDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride) where T : struct;

        /// <summary>
        /// Writes data to the vertex buffer from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the vertex buffer.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="elementCount">Number of elements to write.</param>
        /// <param name="offsetInBytes">Offset in bytes from the beginning of the vertex buffer to the data.</param>
        /// <param name="vertexStride">Size of an element in bytes.</param>
        void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride) where T : struct;
    }
}
