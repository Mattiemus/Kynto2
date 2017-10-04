namespace Spark.Graphics
{
    using System;

    using Core;
    using Content;
    using Graphics.Geometry;
    using Graphics.Implementation;

    /// <summary>
    /// Represents a buffer of indices that exists on the GPU. This allows for vertex data to be-reused, where duplicate vertices can
    /// be indexed multiple times, allowing for a smaller vertex buffer.
    /// </summary>
    public class IndexBuffer : GraphicsResource, ISavable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuffer"/> class.
        /// </summary>
        protected IndexBuffer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="indexFormat">The index format.</param>
        /// <param name="indexCount">Number of indices the buffer will contain.</param>
        public IndexBuffer(IRenderSystem renderSystem, IndexFormat indexFormat, int indexCount)
        {
            CreateImplementation(renderSystem, indexFormat, indexCount, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="indexFormat">The index format.</param>
        /// <param name="indexCount">Number of indices the buffer will contain.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        public IndexBuffer(IRenderSystem renderSystem, IndexFormat indexFormat, int indexCount, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, indexFormat, indexCount, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="indexFormat">The index format.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        public IndexBuffer(IRenderSystem renderSystem, IndexFormat indexFormat, IReadOnlyDataBuffer data)
        {
            CreateImplementation(renderSystem, indexFormat, data, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="indexFormat">The index format.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        public IndexBuffer(IRenderSystem renderSystem, IndexFormat indexFormat, IReadOnlyDataBuffer data, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, indexFormat, data, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        public IndexBuffer(IRenderSystem renderSystem, IndexData data)
        {
            CreateImplementation(renderSystem, data, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        public IndexBuffer(IRenderSystem renderSystem, IndexData data, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, data, resourceUsage);
        }

        /// <summary>
        /// Gets the number of indices in the buffer.
        /// </summary>
        public int IndexCount => IndexBufferImplementation.IndexCount;

        /// <summary>
        /// Gets the index format, 32-bit (int) or 16-bit (short).
        /// </summary>
        public IndexFormat IndexFormat => IndexBufferImplementation.IndexFormat;

        /// <summary>
        /// Gets the resource usage of the buffer.
        /// </summary>
        public ResourceUsage ResourceUsage => IndexBufferImplementation.ResourceUsage;

        /// <summary>
        /// Gets or sets the index buffer implementation
        /// </summary>
        private IIndexBufferImplementation IndexBufferImplementation
        {
            get => Implementation as IIndexBufferImplementation;
            set => BindImplementation(value);
        }
        
        /// <summary>
        /// Reads data from the index buffer into the data buffer
        /// </summary>
        /// <typeparam name="T">Type of data to read from the index buffer.</typeparam>
        /// <param name="data">Data buffer to copy data into.</param>
        public void GetData<T>(IDataBuffer<T> data) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                IndexBufferImplementation.GetData<T>(data, 0, (data != null) ? data.Length : 0, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Reads data from the index buffer into the data buffer
        /// </summary>
        /// <typeparam name="T">Type of data to read from the index buffer.</typeparam>
        /// <param name="data">Data buffer to copy data into.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start writing to</param>
        /// <param name="elementCount">Number of indices to read</param>
        public void GetData<T>(IDataBuffer<T> data, int startIndex, int elementCount) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                IndexBufferImplementation.GetData(data, startIndex, elementCount, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Reads data from the index buffer into the data buffer
        /// </summary>
        /// <typeparam name="T">Type of data to read from the index buffer.</typeparam>
        /// <param name="data">Data buffer to copy data into.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start writing to</param>
        /// <param name="elementCount">Number of indices to read</param>
        /// <param name="offsetInBytes">Offset from the start of the index buffer at which to start copying from</param>
        public void GetData<T>(IDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                IndexBufferImplementation.GetData<T>(data, startIndex, elementCount, offsetInBytes);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the index buffer.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                IndexBufferImplementation.SetData<T>(renderContext, data, 0, (data != null) ? data.Length : 0, 0, DataWriteOptions.Discard);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        public void SetData(IRenderContext renderContext, IndexData data)
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                switch (data.IndexFormat)
                {
                    case IndexFormat.ThirtyTwoBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsIntDataBuffer(), 0, (data.IsValid) ? data.Length : 0, 0, DataWriteOptions.Discard);
                        break;
                    case IndexFormat.SixteenBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsShortDataBuffer(), 0, (data.IsValid) ? data.Length : 0, 0, DataWriteOptions.Discard);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the index buffer.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="writeOptions">Write options, used only if this is a dynamic buffer. None, discard, no overwrite</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                IndexBufferImplementation.SetData(renderContext, data, 0, (data != null) ? data.Length : 0, 0, writeOptions);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="writeOptions">Write options, used only if this is a dynamic buffer. None, discard, no overwrite</param>
        public void SetData(IRenderContext renderContext, IndexData data, DataWriteOptions writeOptions)
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                switch (data.IndexFormat)
                {
                    case IndexFormat.ThirtyTwoBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsIntDataBuffer(), 0, (data.IsValid) ? data.Length : 0, 0, writeOptions);
                        break;
                    case IndexFormat.SixteenBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsShortDataBuffer(), 0, (data.IsValid) ? data.Length : 0, 0, writeOptions);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the index buffer.</typeparam>
        /// <param name="renderContext">The current render context.</param> 
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start copying from</param>
        /// <param name="elementCount">Number of indices to write</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                IndexBufferImplementation.SetData(renderContext, data, startIndex, elementCount, 0, DataWriteOptions.Discard);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <param name="renderContext">The current render context.</param> 
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start copying from</param>
        /// <param name="elementCount">Number of indices to write</param>
        public void SetData(IRenderContext renderContext, IndexData data, int startIndex, int elementCount)
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                switch (data.IndexFormat)
                {
                    case IndexFormat.ThirtyTwoBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsIntDataBuffer(), startIndex, elementCount, 0, DataWriteOptions.Discard);
                        break;
                    case IndexFormat.SixteenBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsShortDataBuffer(), startIndex, elementCount, 0, DataWriteOptions.Discard);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the index buffer.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start copying from</param>
        /// <param name="elementCount">Number of indices to write</param>
        /// <param name="writeOptions">Write options, used only if this is a dynamic buffer. None, discard, no overwrite</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                IndexBufferImplementation.SetData(renderContext, data, startIndex, elementCount, 0, writeOptions);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start copying from</param>
        /// <param name="elementCount">Number of indices to write</param>
        /// <param name="writeOptions">Write options, used only if this is a dynamic buffer. None, discard, no overwrite</param>
        public void SetData(IRenderContext renderContext, IndexData data, int startIndex, int elementCount, DataWriteOptions writeOptions)
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                switch (data.IndexFormat)
                {
                    case IndexFormat.ThirtyTwoBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsIntDataBuffer(), startIndex, elementCount, 0, writeOptions);
                        break;
                    case IndexFormat.SixteenBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsShortDataBuffer(), startIndex, elementCount, 0, writeOptions);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the index buffer.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start copying from</param>
        /// <param name="elementCount">Number of indices to write</param>
        /// <param name="offsetInBytes">Offset from the start of the index buffer at which to start writing at</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                IndexBufferImplementation.SetData(renderContext, data, startIndex, elementCount, offsetInBytes, DataWriteOptions.Discard);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start copying from</param>
        /// <param name="elementCount">Number of indices to write</param>
        /// <param name="offsetInBytes">Offset from the start of the index buffer at which to start writing at</param>
        public void SetData(IRenderContext renderContext, IndexData data, int startIndex, int elementCount, int offsetInBytes)
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                switch (data.IndexFormat)
                {
                    case IndexFormat.ThirtyTwoBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsIntDataBuffer(), startIndex, elementCount, offsetInBytes, DataWriteOptions.Discard);
                        break;
                    case IndexFormat.SixteenBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsShortDataBuffer(), startIndex, elementCount, offsetInBytes, DataWriteOptions.Discard);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the index buffer.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start copying from</param>
        /// <param name="elementCount">Number of indices to write</param>
        /// <param name="offsetInBytes">Offset from the start of the index buffer at which to start writing at</param>
        /// <param name="writeOptions">Write options, used only if this is a dynamic buffer. None, discard, no overwrite</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                IndexBufferImplementation.SetData(renderContext, data, startIndex, elementCount, offsetInBytes, writeOptions);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data from the data buffer into the index buffer.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer to copy data from.</param>
        /// <param name="startIndex">Starting index in the data buffer at which to start copying from</param>
        /// <param name="elementCount">Number of indices to write</param>
        /// <param name="offsetInBytes">Offset from the start of the index buffer at which to start writing at</param>
        /// <param name="writeOptions">Write options, used only if this is a dynamic buffer. None, discard, no overwrite</param>
        public void SetData(IRenderContext renderContext, IndexData data, int startIndex, int elementCount, int offsetInBytes, DataWriteOptions writeOptions)
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                switch (data.IndexFormat)
                {
                    case IndexFormat.ThirtyTwoBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsIntDataBuffer(), startIndex, elementCount, offsetInBytes, writeOptions);
                        break;
                    case IndexFormat.SixteenBits:
                        IndexBufferImplementation.SetData(renderContext, data.AsShortDataBuffer(), startIndex, elementCount, offsetInBytes, writeOptions);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }
        
        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            IRenderSystem renderSystem = GraphicsHelpers.GetRenderSystem(input.ServiceProvider);

            string name = input.ReadString();
            int indexCount = input.ReadInt32();
            IndexFormat indexFormat = input.ReadEnum<IndexFormat>();
            ResourceUsage usage = input.ReadEnum<ResourceUsage>();
            IDataBuffer<byte> byteBuffer = input.ReadArrayData<byte>();

            CreateImplementation(renderSystem, indexFormat, byteBuffer, usage);
            Name = name;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            IIndexBufferImplementation impl = IndexBufferImplementation;
            output.Write("Name", Name);
            output.Write("IndexCount", IndexBufferImplementation.IndexCount);
            output.WriteEnum("IndexFormat", IndexBufferImplementation.IndexFormat);
            output.WriteEnum("BufferUsage", IndexBufferImplementation.ResourceUsage);

            IDataBuffer<byte> byteBuffer = DataBuffer<byte>.Create(IndexBufferImplementation.IndexCount * IndexBufferImplementation.IndexFormat.SizeInBytes());
            impl.GetData(byteBuffer, 0, byteBuffer.Length, 0);
            byteBuffer.Position = 0;

            output.Write<byte>("IndexData", byteBuffer);
        }
        
        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="indexCount">The number of indices the buffer will contain.</param>
        protected void ValidateCreationParameters(int indexCount)
        {
            if (indexCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCount), "Resource size must be greater than zero");
            }
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="indexFormat">The index format.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        protected void ValidateCreationParameters(IndexFormat indexFormat, IReadOnlyDataBuffer data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer is null or empty");
            }

            data.Position = 0;
            int totalSize = data.SizeInBytes;
            int indexSize = indexFormat.SizeInBytes();

            if (totalSize % indexSize != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Index format mismatch");
            }
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="data">Index data to initialize the buffer with.</param>
        protected void ValidateCreationParameters(IndexData data)
        {
            // IndexData may be constructed with default ctor and not contain either an int or short data buffer
            if (!data.IsValid || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer is null or empty");
            }

            data.Position = 0;
        }

        /// <summary>
        /// Creates the index buffer implementation 
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="indexFormat">The index format.</param>
        /// <param name="indexCount">Number of indices the buffer will contain.</param>
        /// <param name="bufferUsage">Resource usage specifying the type of memory the buffer should use.</param>
        private void CreateImplementation(IRenderSystem renderSystem, IndexFormat indexFormat, int indexCount, ResourceUsage bufferUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (bufferUsage == ResourceUsage.Immutable)
            {
                throw new SparkGraphicsException("Must supply data for immutable buffer");
            }

            ValidateCreationParameters(indexCount);
            
            if (!renderSystem.TryGetImplementationFactory(out IIndexBufferImplFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                IndexBufferImplementation = factory.CreateImplementation(indexFormat, indexCount, bufferUsage);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        /// <summary>
        /// Creates the index buffer implementation 
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="indexFormat">The index format.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        /// <param name="bufferUsage">Resource usage specifying the type of memory the buffer should use.</param>
        private void CreateImplementation(IRenderSystem renderSystem, IndexFormat indexFormat, IReadOnlyDataBuffer data, ResourceUsage bufferUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            ValidateCreationParameters(indexFormat, data);
            
            if (!renderSystem.TryGetImplementationFactory(out IIndexBufferImplFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                IndexBufferImplementation = factory.CreateImplementation(indexFormat, bufferUsage, data);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        /// <summary>
        /// Creates the index buffer implementation 
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        /// <param name="bufferUsage">Resource usage specifying the type of memory the buffer should use.</param>
        private void CreateImplementation(IRenderSystem renderSystem, IndexData data, ResourceUsage bufferUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            ValidateCreationParameters(data);
            
            if (!renderSystem.TryGetImplementationFactory(out IIndexBufferImplFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                IndexBufferImplementation = factory.CreateImplementation(bufferUsage, data);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }
    }
}
