namespace Spark.Graphics
{
    using System;

    using Core;
    using Content;
    using Implementation;

    /// <summary>
    /// Represents a buffer of vertices that exists on the GPU. A vertex is made up of individual attributes such as a position
    /// and other properties like colors, texture coordinates, or normals.
    /// </summary>
    public class VertexBuffer : GraphicsResource, ISavable
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
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data this buffer will contain</param>
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        public VertexBuffer(IRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount)
        {
            CreateImplementation(renderSystem, vertexLayout, vertexCount, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data this buffer will contain</param>
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        public VertexBuffer(IRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, vertexLayout, vertexCount, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="data">The interleaved vertex data to initialize the vertex buffer with.</param>
        public VertexBuffer(IRenderSystem renderSystem, VertexLayout vertexLayout, IReadOnlyDataBuffer data)
        {
            CreateImplementation(renderSystem, vertexLayout, ResourceUsage.Static, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">The interleaved vertex data to initialize the vertex buffer with.</param>
        public VertexBuffer(IRenderSystem renderSystem, VertexLayout vertexLayout, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
        {
            CreateImplementation(renderSystem, vertexLayout, resourceUsage, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="data">Array of databuffers to initialize the vertex buffer with, each databuffer corresponds to a single vertex element.</param>
        public VertexBuffer(IRenderSystem renderSystem, VertexLayout vertexLayout, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, vertexLayout, ResourceUsage.Static, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data of this buffer</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">Array of databuffers to initialize the vertex buffer with, each databuffer corresponds to a single vertex element.</param>
        public VertexBuffer(IRenderSystem renderSystem, VertexLayout vertexLayout, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, vertexLayout, resourceUsage, data);
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
        /// Gets the resource usage of the buffer.
        /// </summary>
        public ResourceUsage ResourceUsage => VertexBufferImplementation.ResourceUsage;

        /// <summary>
        /// Gets or sets the vertex buffer implementation
        /// </summary>
        private IVertexBufferImplementation VertexBufferImplementation
        {
            get => Implementation as IVertexBufferImplementation;
            set => BindImplementation(value);
        }

        /// <summary>
        /// Reads interleaved vertex data from the vertexbuffer and stores it in an array of data buffers. Each data buffer represents
        /// a single vertex element (in the order declared by the vertex declaration). All data is read from the vertex buffer starting
        /// from the beginning of the vertex buffer and each data buffer to the length of each data buffer. Each data buffer must have the same length.
        /// The data buffers must match the vertex declaration in format size and cannot exceed the length of the vertex buffer.
        /// </summary>
        /// <param name="data">Array of databuffers representing each vertex attribute that will contain data read from the vertex buffer.</param>
        public void GetInterleavedData(params IDataBuffer[] data)
        {
            ThrowIfDisposed();

            try
            {
                VertexBufferImplementation.GetInterleavedData(data);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
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
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                VertexBufferImplementation.SetInterleavedData(renderContext, data);
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
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                VertexBufferImplementation.SetData(renderContext, data, 0, (data != null) ? data.Length : 0, 0, 0, DataWriteOptions.Discard);
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
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic vertex buffers.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                VertexBufferImplementation.SetData(renderContext, data, 0, (data != null) ? data.Length : 0, 0, 0, writeOptions);
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
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="elementCount">Number of elements to write.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                VertexBufferImplementation.SetData(renderContext, data, startIndex, elementCount, 0, 0, DataWriteOptions.Discard);
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
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="elementCount">Number of elements to write.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic vertex buffers.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                VertexBufferImplementation.SetData(renderContext, data, startIndex, elementCount, 0, 0, writeOptions);
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
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the vertex buffer.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="elementCount">Number of elements to write.</param>
        /// <param name="offsetInBytes">Offset in bytes from the beginning of the vertex buffer to the data.</param>
        /// <param name="vertexStride">Size of an element in bytes.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                VertexBufferImplementation.SetData(renderContext, data, startIndex, elementCount, offsetInBytes, vertexStride, DataWriteOptions.Discard);
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
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                VertexBufferImplementation.SetData(renderContext, data, startIndex, elementCount, offsetInBytes, vertexStride, writeOptions);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Reads the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        public void Read(ISavableReader input)
        {
            IRenderSystem renderSystem = GraphicsHelper.GetRenderSystem(input.ServiceProvider);

            string name = input.ReadString();
            VertexLayout decl = input.ReadSavable<VertexLayout>();
            int vertexCount = input.ReadInt32();
            ResourceUsage usage = input.ReadEnum<ResourceUsage>();
            IDataBuffer<byte> byteBuffer = input.ReadArrayData<byte>();

            CreateImplementation(renderSystem, decl, usage, byteBuffer);
            Name = name;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            IVertexBufferImplementation impl = VertexBufferImplementation;
            output.Write("Name", Name);
            output.WriteSavable("VertexLayout", VertexBufferImplementation.VertexLayout);
            output.Write("VertexCount", VertexBufferImplementation.VertexCount);
            output.WriteEnum("BufferUsage", VertexBufferImplementation.ResourceUsage);

            IRenderContext renderContext = RenderSystem.ImmediateContext;

            IDataBuffer<byte> byteBuffer = DataBuffer<byte>.Create(VertexBufferImplementation.VertexLayout.VertexStride * VertexBufferImplementation.VertexCount);
            impl.GetData(byteBuffer, 0, byteBuffer.Length, 0, 0);
            byteBuffer.Position = 0;

            output.Write<byte>("VertexData", byteBuffer);
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="vertexCount">The number of vertices the buffer will contain.</param>
        protected void ValidateCreationParameters(VertexLayout vertexLayout, int vertexCount)
        {
            if (vertexLayout == null)
            {
                throw new ArgumentNullException(nameof(vertexLayout), "Vertex layout cannot be null");
            }

            if (vertexCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCount), "Resource size must be greater than zero");
            }
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="data">The interleaved vertex data to initialize the vertex buffer with.</param>
        protected void ValidateCreationParameters(VertexLayout vertexLayout, IReadOnlyDataBuffer data)
        {
            if (vertexLayout == null)
            {
                throw new ArgumentNullException(nameof(vertexLayout), "Vertex layout cannot be null");
            }

            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer cannot be null");
            }

            // Get the vertex count
            data.Position = 0;
            int totalSize = data.SizeInBytes;
            int vertexStride = vertexLayout.VertexStride;

            // Vertex stride should divide evenly into the total buffer size
            if (totalSize % vertexStride != 0)
            {
                throw new ArgumentOutOfRangeException("data", "Vertex layout stride does not match the side of the data buffer");
            }
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout that describes the data.</param>
        /// <param name="data">Array of databuffers to initialize the vertex buffer with, each databuffer corresponds to a single vertex element.</param>
        protected void ValidateCreationParameters(VertexLayout vertexLayout, IReadOnlyDataBuffer[] data)
        {
            if (vertexLayout == null)
            {
                throw new ArgumentNullException(nameof(vertexLayout), "Vertex layout cannot be null");
            }

            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer cannot be null");
            }

            // Verify the incoming vertex attribute databuffers match the declaration
            if (data.Length != vertexLayout.ElementCount)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Vertex layout element count mismatch");
            }

            int totalSizeInBytes = 0;
            int vertexStride = 0;
            int vertexCount = 0;

            for (int i = 0; i < data.Length; i++)
            {
                IReadOnlyDataBuffer db = data[i];

                if (db == null)
                {
                    throw new ArgumentNullException(nameof(data), "Data buffer is null");
                }

                VertexElement element = vertexLayout[i];
                int vSizeInBytes = db.ElementSizeInBytes;
                int vCount = db.SizeInBytes / vSizeInBytes;

                if (i == 0)
                {
                    vertexCount = vCount;

                    if (vertexCount <= 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(data), "Resource size must be greater than or equal to zero");
                    }
                }
                else
                {
                    if (vCount != vertexCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(data), "Vertex count mismatch");
                    }
                }

                if (vSizeInBytes != element.Format.SizeInBytes())
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Vertex element mismatch");
                }

                totalSizeInBytes += db.SizeInBytes;
                vertexStride += vSizeInBytes;
                db.Position = 0;
            }

            if (totalSizeInBytes > vertexLayout.VertexStride * vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Vertex buffer size mismatch");
            }
        }

        /// <summary>
        /// Creates the vertex buffer implementation
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="vertexLayout">Vertex layout</param>
        /// <param name="vertexCount">Vertex count</param>
        /// <param name="bufferUsage">Vertex buffer usage</param>
        private void CreateImplementation(IRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount, ResourceUsage bufferUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (bufferUsage == ResourceUsage.Immutable)
            {
                throw new SparkGraphicsException("Must supply data for immutable buffer");
            }

            ValidateCreationParameters(vertexLayout, vertexCount);
            
            if (!renderSystem.TryGetImplementationFactory(out IVertexBufferImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                VertexBufferImplementation = factory.CreateImplementation(vertexLayout, vertexCount, bufferUsage);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        /// <summary>
        /// Creates the vertex buffer implementation
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="vertexLayout">Vertex layout</param>
        /// <param name="bufferUsage">Vertex buffer usage</param>
        /// <param name="data">Vertex buffer data</param>
        private void CreateImplementation(IRenderSystem renderSystem, VertexLayout vertexLayout, ResourceUsage bufferUsage, IReadOnlyDataBuffer data)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            ValidateCreationParameters(vertexLayout, data);

            if (!renderSystem.TryGetImplementationFactory(out IVertexBufferImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                VertexBufferImplementation = factory.CreateImplementation(vertexLayout, bufferUsage, data);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        /// <summary>
        /// Creates the vertex buffer implementation
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="vertexLayout">Vertex layout</param>
        /// <param name="bufferUsage">Vertex buffer usage</param>
        /// <param name="data">Vertex buffer data</param>
        private void CreateImplementation(IRenderSystem renderSystem, VertexLayout vertexLayout, ResourceUsage bufferUsage, params IReadOnlyDataBuffer[] data)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            ValidateCreationParameters(vertexLayout, data);

            if (!renderSystem.TryGetImplementationFactory(out IVertexBufferImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                VertexBufferImplementation = factory.CreateImplementation(vertexLayout, bufferUsage, data);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }
    }
}
