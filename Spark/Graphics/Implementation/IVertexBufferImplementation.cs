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
        /// Gets the resource usage of the buffer.
        /// </summary>
        ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Reads interleaved vertex data from the vertexbuffer and stores it in an array of data buffers. Each data buffer represents
        /// a single vertex element (in the order declared by the vertex declaration). All data is read from the vertex buffer starting
        /// from the beginning of the vertex buffer and each data buffer to the length of each data buffer. Each data buffer must have the same length.
        /// The data buffers must match the vertex declaration in format size and cannot exceed the length of the vertex buffer.
        /// </summary>
        /// <param name="data">Array of databuffers representing each vertex attribute that will contain data read from the vertex buffer.</param>
        void GetInterleavedData(params IDataBuffer[] data);

        /// <summary>
        /// Writes interleaved vertex data from an array of data buffers into the vertex buffer. Each data buffer represents a single vertex
        /// element (in the order declared by the vertex declaration). All data is written to the vertex buffer starting from the beginning of the
        /// vertex buffer and each data buffer to the length of each data buffer. Each data buffer must have the same length. The data buffers 
        /// must match the vertex declaration in format size and cannot exceed the length of the vertex buffer. For dynamic vertex buffers, this always
        /// uses the <see cref="DataWriteOptions.Discard"/> flag.
        /// </summary>
        /// <param name="renderContext">The current render context.</param> 
        /// <param name="data">Array of databuffers representing each vertex attribute whose data will be written to the vertex buffer.</param>
        void SetInterleavedData(IRenderContext renderContext, params IReadOnlyDataBuffer[] data);

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
        /// <param name="writeOptions">Writing options, valid only for dynamic vertex buffers.</param>
        void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride, DataWriteOptions writeOptions) where T : struct;
    }
}
