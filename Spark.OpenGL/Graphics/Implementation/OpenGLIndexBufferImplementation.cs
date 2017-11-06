namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Geometry;
    using Spark.Graphics.Implementation;

    using Core;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// OpenGL implementation for <see cref="IndexBuffer"/>
    /// </summary>
    public sealed class OpenGLIndexBufferImplementation  : OpenGLGraphicsResourceImplementation, IIndexBufferImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLIndexBufferImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="format">The index format.</param>
        /// <param name="indexCount">Number of indices the buffer will contain.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <returns>The buffer implementation.</returns>
        public OpenGLIndexBufferImplementation(OpenGLRenderSystem renderSystem, IndexFormat format, int indexCount, ResourceUsage resourceUsage)
            : base(renderSystem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLIndexBufferImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="format">The index format.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        public OpenGLIndexBufferImplementation(OpenGLRenderSystem renderSystem, IndexFormat format, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
            : base(renderSystem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLIndexBufferImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">The index data to initialize the buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        public OpenGLIndexBufferImplementation(OpenGLRenderSystem renderSystem, ResourceUsage resourceUsage, IndexData data)
            : base(renderSystem)
        {
            OpenGLBufferId = OGL.GL.GenBuffer();

            IndexCount = data.Length;
            IndexFormat = data.IndexFormat;
            ResourceUsage = resourceUsage;

            OGL.GL.BindBuffer(OGL.BufferTarget.ElementArrayBuffer, OpenGLBufferId);
            using (MappedDataBuffer mappedData = data.Map())
            {
                OGL.GL.NamedBufferData(OpenGLBufferId, data.SizeInBytes, mappedData.Pointer, OGL.BufferUsageHint.StaticDraw);
            }
        }

        /// <summary>
        /// Gets the OpenGL buffer id
        /// </summary>
        internal int OpenGLBufferId { get; }

        /// <summary>
        /// Gets the number of indices in the buffer.
        /// </summary>
        public int IndexCount { get; }

        /// <summary>
        /// Gets the index format, 32-bit (int) or 16-bit (short).
        /// </summary>
        public IndexFormat IndexFormat { get; }

        /// <summary>
        /// Gets the resource usage of the buffer.
        /// </summary>
        public ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Reads data from the index buffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the index buffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the index buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        /// <param name="elementCount">Number of elements to read.</param>
        /// <param name="offsetInBytes">Offset from the start of the index buffer at which to start copying from</param>
        public void GetData<T>(IDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes) where T : struct
        {
            throw new NotImplementedException();           
        }

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
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, DataWriteOptions writeOptions) where T : struct
        {
            throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            base.Dispose(isDisposing);
        }
    }
}
