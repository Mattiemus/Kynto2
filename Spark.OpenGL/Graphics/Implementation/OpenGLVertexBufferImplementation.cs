namespace Spark.Graphics.Implementation
{
    using System;

    using Core;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    public sealed class OpenGLVertexBufferImplementation : GraphicsResourceImplementation, IVertexBufferImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLVertexBufferImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        public OpenGLVertexBufferImplementation(OpenGLRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount)
            : base(renderSystem, OGL.GL.GenBuffer())
        {
            OGL.GL.BindBuffer(OGL.BufferTarget.ArrayBuffer, ResourceId);
            OGL.GL.BufferData(OGL.BufferTarget.ArrayBuffer, vertexCount * vertexLayout.VertexStride, IntPtr.Zero, OGL.BufferUsageHint.StaticDraw);

            VertexLayout = vertexLayout;
            VertexCount = vertexCount;
        }

        /// <summary>
        /// Gets the vertex layout that describes the structure of vertex data contained in the buffer.
        /// </summary>
        public VertexLayout VertexLayout { get; }

        /// <summary>
        /// Gets the number of vertices contained in the buffer.
        /// </summary>
        public int VertexCount { get; }

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
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="elementCount">Number of elements to write.</param>
        /// <param name="offsetInBytes">Offset in bytes from the beginning of the vertex buffer to the data.</param>
        /// <param name="vertexStride">Size of an element in bytes.</param>
        public void SetData<T>(IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride) where T : struct
        {
            ThrowIfDisposed();

            using (MappedDataBuffer mappedData = data.Map())
            {
                OGL.GL.NamedBufferSubData(ResourceId, new IntPtr(offsetInBytes), elementCount * data.ElementSizeInBytes, mappedData.Pointer);
            }
        }

        /// <summary>
        /// Performs the dispose action
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void DisposeInternal(bool isDisposing)
        {
            if (OTK.GraphicsContext.CurrentContext == null || OTK.GraphicsContext.CurrentContext.IsDisposed)
            {
                return;
            }

            OGL.GL.DeleteBuffer(ResourceId);
        }
    }
}
