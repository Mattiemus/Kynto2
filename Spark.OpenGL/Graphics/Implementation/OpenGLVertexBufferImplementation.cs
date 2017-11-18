namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    
    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// OpenGL implementation for <see cref="VertexBuffer"/>
    /// </summary>
    public sealed class OpenGLVertexBufferImplementation : OpenGLGraphicsResourceImplementation, IVertexBufferImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLVertexBufferImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        public OpenGLVertexBufferImplementation(OpenGLRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount, ResourceUsage resourceUsage)
            : base(renderSystem)
        {
            OpenGLBufferId = OGL.GL.GenBuffer();

            VertexLayout = vertexLayout;
            ResourceUsage = resourceUsage;
            VertexCount = vertexCount;

            OGL.GL.BindBuffer(OGL.BufferTarget.ArrayBuffer, OpenGLBufferId);
            OGL.GL.NamedBufferData(OpenGLBufferId, vertexCount * vertexLayout.VertexStride, IntPtr.Zero, OGL.BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLVertexBufferImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">The interleaved vertex data to initialize the vertex buffer with.</param>
        public OpenGLVertexBufferImplementation(OpenGLRenderSystem renderSystem, VertexLayout vertexLayout, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
            : base(renderSystem)
        {
            OpenGLBufferId = OGL.GL.GenBuffer();

            VertexLayout = vertexLayout;
            ResourceUsage = resourceUsage;
            VertexCount = data.SizeInBytes / vertexLayout.VertexStride;

            OGL.GL.BindBuffer(OGL.BufferTarget.ArrayBuffer, OpenGLBufferId);
            using (MappedDataBuffer mappedData = data.Map())
            {
                OGL.GL.NamedBufferData(OpenGLBufferId, data.SizeInBytes, mappedData.Pointer, OGL.BufferUsageHint.StaticDraw);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLVertexBufferImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="resourceUsage">The resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">Array of databuffers to initialize the vertex buffer with, each databuffer corresponds to a single vertex element.</param>
        /// <returns>The buffer implementation.</returns>
        public OpenGLVertexBufferImplementation(OpenGLRenderSystem renderSystem, VertexLayout vertexLayout, ResourceUsage resourceUsage, IReadOnlyDataBuffer[] data)
            : base(renderSystem)
        {
            if (data.Length != 1)
            {
                throw new NotImplementedException();
            }

            OpenGLBufferId = OGL.GL.GenBuffer();

            VertexLayout = vertexLayout;
            ResourceUsage = resourceUsage;
            VertexCount = data[0].SizeInBytes / vertexLayout.VertexStride;

            OGL.GL.BindBuffer(OGL.BufferTarget.ArrayBuffer, OpenGLBufferId);
            using (MappedDataBuffer mappedData = data[0].Map())
            {
                OGL.GL.NamedBufferData(OpenGLBufferId, data[0].SizeInBytes, mappedData.Pointer, OGL.BufferUsageHint.StaticDraw);
            }
        }

        /// <summary>
        /// Gets the OpenGL buffer id
        /// </summary>
        internal int OpenGLBufferId { get; }

        /// <summary>
        /// Gets the vertex layout that describes the structure of vertex data contained in the buffer.
        /// </summary>
        public VertexLayout VertexLayout { get; }

        /// <summary>
        /// Gets the number of vertices contained in the buffer.
        /// </summary>
        public int VertexCount { get; }

        /// <summary>
        /// Gets the resource usage of the buffer.
        /// </summary>
        public ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Reads interleaved vertex data from the vertexbuffer and stores it in an array of data buffers. Each data buffer represents
        /// a single vertex element (in the order declared by the vertex declaration). All data is read from the vertex buffer starting
        /// from the beginning of the vertex buffer and each data buffer to the length of each data buffer. Each data buffer must have the same length.
        /// The data buffers must match the vertex declaration in format size and cannot exceed the length of the vertex buffer.
        /// </summary>
        /// <param name="data">Array of databuffers representing each vertex attribute that will contain data read from the vertex buffer.</param>
        public void GetInterleavedData(params IDataBuffer[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes interleaved vertex data from an array of data buffers into the vertex buffer. Each data buffer represents a single vertex
        /// element (in the order declared by the vertex declaration). All data is written to the vertex buffer starting from the beginning of the
        /// vertex buffer and each data buffer to the length of each data buffer. Each data buffer must have the same length. The data buffers 
        /// must match the vertex declaration in format size and cannot exceed the length of the vertex buffer. For dynamic vertex buffers, this always
        /// uses the <see cref="DataWriteOptions.Discard"/> flag.
        /// </summary>
        /// <param name="renderContext">The current render context.</param> 
        /// <param name="data">Array of databuffers representing each vertex attribute whose data will be written to the vertex buffer.</param>
        public void SetInterleavedData(IRenderContext renderContext, params IReadOnlyDataBuffer[] data)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

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
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();

            using (MappedDataBuffer mappedData = data.Map())
            {
                OGL.GL.NamedBufferSubData(OpenGLBufferId, new IntPtr(offsetInBytes), elementCount * data.ElementSizeInBytes, mappedData.Pointer);
            }
        }

        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (OTK.GraphicsContext.CurrentContext != null && !OTK.GraphicsContext.CurrentContext.IsDisposed)
            {
                OGL.GL.DeleteBuffer(OpenGLBufferId);
            }

            base.Dispose(isDisposing);
        }
    }
}
