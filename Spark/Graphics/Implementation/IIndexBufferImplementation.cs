namespace Spark.Graphics.Implementation
{
    using Core;

    /// <summary>
    /// Defines an implementation for <see cref="IndexBuffer"/>.
    /// </summary>
    public interface IIndexBufferImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets the number of indices in the buffer.
        /// </summary>
        int IndexCount { get; }

        /// <summary>
        /// Gets the index format, 32-bit (int) or 16-bit (short).
        /// </summary>
        IndexFormat IndexFormat { get; }

        /// <summary>
        /// Gets the resource usage of the buffer.
        /// </summary>
        ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Reads data from the index buffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the index buffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the index buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        /// <param name="elementCount">Number of elements to read.</param>
        /// <param name="offsetInBytes">Offset from the start of the index buffer at which to start copying from</param>
        void GetData<T>(IDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes) where T : struct;

        /// <summary>
        /// Writes data to the index buffer from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the index buffer.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the index buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="elementCount">Number of elements to write.</param>
        /// <param name="offsetInBytes">Offset from the start of the index buffer at which to start writing at</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic index buffers.</param>
        void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, DataWriteOptions writeOptions) where T : struct;
    }
}
