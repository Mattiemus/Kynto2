namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D11 = SharpDX.Direct3D11;
    
    /// <summary>
    /// A Direct3D11 implementation for <see cref="VertexBuffer"/>.
    /// </summary>
    public sealed class D3D11VertexBufferImplementation : GraphicsResourceImplementation, IVertexBufferImplementation, IStreamOutputBufferImplementation, ID3D11Buffer
    {
        private D3D11.Buffer _nativeBuffer;

        internal D3D11VertexBufferImplementation(D3D11RenderSystem renderSystem, int resourceID, VertexLayout vertexLayout, int vertexCount, bool streamOutput)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            VertexLayout = vertexLayout;
            VertexCount = vertexCount;
            ResourceUsage = ResourceUsage.Static;

            var desc = new D3D11.BufferDescription
            {
                BindFlags = (streamOutput) ? D3D11.BindFlags.VertexBuffer | D3D11.BindFlags.StreamOutput | D3D11.BindFlags.ShaderResource : D3D11.BindFlags.VertexBuffer,
                Usage = D3D11.ResourceUsage.Default,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                SizeInBytes = vertexLayout.VertexStride * VertexCount,
                StructureByteStride = 0
            };

            _nativeBuffer = new D3D11.Buffer(D3DDevice, desc);

            Name = string.Empty;
        }

        internal D3D11VertexBufferImplementation(D3D11RenderSystem renderSystem, int resourceID, VertexLayout vertexLayout, int vertexCount, ResourceUsage resourceUsage)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            VertexLayout = vertexLayout;
            VertexCount = vertexCount;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.BufferDescription
            {
                BindFlags = D3D11.BindFlags.VertexBuffer,
                Usage = Direct3DHelper.ToD3DResourceUsage(ResourceUsage),
                CpuAccessFlags = (ResourceUsage == ResourceUsage.Dynamic) ? D3D11.CpuAccessFlags.Write : D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                SizeInBytes = vertexLayout.VertexStride * VertexCount,
                StructureByteStride = 0
            };

            _nativeBuffer = new D3D11.Buffer(D3DDevice, desc);

            Name = string.Empty;
        }

        internal D3D11VertexBufferImplementation(D3D11RenderSystem renderSystem, int resourceID, VertexLayout vertexLayout, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            VertexLayout = vertexLayout;
            VertexCount = data.SizeInBytes / vertexLayout.VertexStride;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.BufferDescription
            {
                BindFlags = D3D11.BindFlags.VertexBuffer,
                Usage = Direct3DHelper.ToD3DResourceUsage(ResourceUsage),
                CpuAccessFlags = (ResourceUsage == ResourceUsage.Dynamic) ? D3D11.CpuAccessFlags.Write : D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                SizeInBytes = data.SizeInBytes,
                StructureByteStride = 0
            };

            using (MappedDataBuffer ptr = data.Map())
            {
                _nativeBuffer = new D3D11.Buffer(D3DDevice, ptr, desc);
            }

            Name = string.Empty;
        }

        internal D3D11VertexBufferImplementation(D3D11RenderSystem renderSystem, int resourceID, VertexLayout vertexLayout, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
            : base(renderSystem, resourceID)
        {
            // Collate all the attribute buffers into a single interleaved buffer (rather than using a staging resource - to be thread safe!)
            using (IDataBuffer interleavedBuffer = ResourceHelper.CreatedInterleavedVertexBuffer(vertexLayout, data))
            {
                D3DDevice = renderSystem.D3DDevice;
                VertexLayout = vertexLayout;
                VertexCount = interleavedBuffer.SizeInBytes / vertexLayout.VertexStride;
                ResourceUsage = resourceUsage;

                var desc = new D3D11.BufferDescription
                {
                    BindFlags = D3D11.BindFlags.VertexBuffer,
                    Usage = Direct3DHelper.ToD3DResourceUsage(ResourceUsage),
                    CpuAccessFlags = (ResourceUsage == ResourceUsage.Dynamic) ? D3D11.CpuAccessFlags.Write : D3D11.CpuAccessFlags.None,
                    OptionFlags = D3D11.ResourceOptionFlags.None,
                    SizeInBytes = interleavedBuffer.SizeInBytes,
                    StructureByteStride = 0
                };

                using (MappedDataBuffer ptr = interleavedBuffer.Map())
                {
                    _nativeBuffer = new D3D11.Buffer(D3DDevice, ptr, desc);
                }
            }

            Name = string.Empty;
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
        /// Gets the resource usage of the buffer.
        /// </summary>
        public ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Ges the native D3D11 buffer.
        /// </summary>
        public D3D11.Buffer D3DBuffer => _nativeBuffer;

        /// <summary>
        /// Reads interleaved vertex data from the vertexbuffer and stores it in an array of data buffers. Each data buffer represents
        /// a single vertex element (in the order declared by the vertex declaration). All data is read from the vertex buffer starting
        /// from the beginning of the vertex buffer and each data buffer to the length of each data buffer. Each data buffer must have the same length.
        /// The data buffers must match the vertex declaration in format size and cannot exceed the length of the vertex buffer.
        /// </summary>
        /// <param name="data">Array of databuffers representing each vertex attribute that will contain data read from the vertex buffer.</param>
        public void GetInterleavedData(params IDataBuffer[] data)
        {
            D3D11.DeviceContext d3dContext = D3DDevice.ImmediateContext;

            ResourceHelper.ReadVertexData(_nativeBuffer, d3dContext, VertexCount, VertexLayout, ResourceUsage, data);
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
            D3D11.DeviceContext d3dContext = Direct3DHelper.GetD3DDeviceContext(renderContext);

            ResourceHelper.WriteVertexData(_nativeBuffer, d3dContext, VertexCount, VertexLayout, ResourceUsage, data);
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
            D3D11.DeviceContext d3dContext = D3DDevice.ImmediateContext;

            ResourceHelper.ReadVertexData(_nativeBuffer, d3dContext, VertexCount, VertexLayout, ResourceUsage, data, startIndex, elementCount, offsetInBytes, vertexStride);
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
            D3D11.DeviceContext d3dContext = Direct3DHelper.GetD3DDeviceContext(renderContext);

            ResourceHelper.WriteVertexData(_nativeBuffer, d3dContext, VertexCount, VertexLayout, ResourceUsage, data, startIndex, elementCount, offsetInBytes, vertexStride, writeOptions);
        }

        /// <summary>
        /// Called when the name of the graphics resource is changed, useful if the implementation wants to set the name to
        /// be used as a debug name.
        /// </summary>
        /// <param name="name">New name of the resource</param>
        protected override void OnNameChange(string name)
        {
            if (_nativeBuffer != null)
            {
                _nativeBuffer.DebugName = name;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (_nativeBuffer != null)
                {
                    _nativeBuffer.Dispose();
                    _nativeBuffer = null;
                }
            }

            base.Dispose(isDisposing);
        }
    }
}
