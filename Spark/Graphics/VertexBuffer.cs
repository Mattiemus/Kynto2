namespace Spark.Graphics
{
    using System;

    using Core;
    using Implementation;

    /// <summary>
    /// Represents a buffer of vertices that exists on the GPU. A vertex is made up of individual attributes such as a position
    /// and other properties like colors, texture coordinates, or normals.
    /// </summary>
    public class VertexBuffer : GraphicsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        protected VertexBuffer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        public VertexBuffer(VertexLayout vertexLayout, int vertexCount)
        {
            CreateImplementation(GetRenderSystem(), vertexLayout, vertexCount);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        public VertexBuffer(IRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount)
        {
            CreateImplementation(renderSystem, vertexLayout, vertexCount);
        }

        /// <summary>
        /// Gets the vertex layout that describes the structure of vertex data contained in the buffer.
        /// </summary>
        public VertexLayout VertexLayout => VertexBufferImplementation.VertexLayout;

        /// <summary>
        /// Gets the number of vertices contained in the buffer.
        /// </summary>
        public int VertexCount => VertexBufferImplementation.VertexCount;

        /// <summary>
        /// Gets the vertex buffer implementation
        /// </summary>
        private IVertexBufferImplementation VertexBufferImplementation
        {
            get
            {
                return Implementation as IVertexBufferImplementation;
            }
            set
            {
                BindImplementation(value);
            }
        }

        /// <summary>
        /// Reads data from the vertex buffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the vertex buffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the vertex buffer.</param>
        public void GetData<T>(IDataBuffer<T> data) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                VertexBufferImplementation.GetData(data, 0, (data != null) ? data.Length : 0, 0, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Reads data from the vertex buffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the vertex buffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        /// <param name="elementCount">Number of elements to read.</param>
        public void GetData<T>(IDataBuffer<T> data, int startIndex, int elementCount) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                VertexBufferImplementation.GetData(data, startIndex, elementCount, 0, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Reads data from the vertex buffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the vertex buffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        /// <param name="elementCount">Number of elements to read.</param>
        /// <param name="offsetInBytes">Offset in bytes from the beginning of the vertex buffer to the data.</param>
        /// <param name="vertexStride">Size of an element in bytes</param>
        public void GetData<T>(IDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                VertexBufferImplementation.GetData(data, startIndex, elementCount, offsetInBytes, vertexStride);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Writes data to the vertex buffer from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the vertex buffer.</typeparam>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        public void SetData<T>(IReadOnlyDataBuffer<T> data) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                VertexBufferImplementation.SetData(data, 0, (data != null) ? data.Length : 0, 0, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }
        
        /// <summary>
        /// Writes data to the vertex buffer from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the vertex buffer.</typeparam>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="elementCount">Number of elements to write.</param>
        public void SetData<T>(IReadOnlyDataBuffer<T> data, int startIndex, int elementCount) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                VertexBufferImplementation.SetData(data, startIndex, elementCount, 0, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data to the vertex buffer from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the vertex buffer.</typeparam>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="elementCount">Number of elements to write.</param>
        /// <param name="offsetInBytes">Offset in bytes from the beginning of the vertex buffer to the data.</param>
        /// <param name="vertexStride">Size of an element in bytes.</param>
        public void SetData<T>(IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                VertexBufferImplementation.SetData<T>(data, startIndex, elementCount, offsetInBytes, vertexStride);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Creates the vertex buffer implementation
        /// </summary>
        /// <param name="renderSystem">Render system to use when creating the implementation</param>
        /// <param name="vertexLayout">Vertex layout</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        private void CreateImplementation(IRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (vertexLayout == null)
            {
                throw new ArgumentNullException(nameof(vertexLayout), "Vertex layout cannot be null");
            }

            IVertexBufferImplementationFactory factory;
            if (!renderSystem.TryGetImplementationFactory(out factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                VertexBufferImplementation = factory.CreateImplementation(vertexLayout, vertexCount);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }
    }
}
