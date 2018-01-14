namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="IndexBuffer"/>.
    /// </summary>
    public sealed class D3D11IndexBufferImplementation : GraphicsResourceImplementation, IIndexBufferImplementation, ID3D11Buffer
    {
        private D3D11.Buffer _nativeBuffer;

        internal D3D11IndexBufferImplementation(D3D11RenderSystem renderSystem, int resourceID, IndexFormat format, int indexCount, ResourceUsage resourceUsage)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            IndexCount = indexCount;
            IndexFormat = format;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.BufferDescription
            {
                BindFlags = D3D11.BindFlags.IndexBuffer,
                Usage = Direct3DHelper.ToD3DResourceUsage(ResourceUsage),
                CpuAccessFlags = (ResourceUsage == ResourceUsage.Dynamic) ? D3D11.CpuAccessFlags.Write : D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                SizeInBytes = IndexCount * IndexFormat.SizeInBytes(),
                StructureByteStride = 0
            };

            _nativeBuffer = new D3D11.Buffer(D3DDevice, desc);

            Name = String.Empty;
        }

        internal D3D11IndexBufferImplementation(D3D11RenderSystem renderSystem, int resourceID, IndexFormat format, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            IndexCount = data.SizeInBytes / format.SizeInBytes();
            IndexFormat = format;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.BufferDescription
            {
                BindFlags = D3D11.BindFlags.IndexBuffer,
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

            Name = String.Empty;
        }

        internal D3D11IndexBufferImplementation(D3D11RenderSystem renderSystem, int resourceID, ResourceUsage resourceUsage, IndexData data)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            IndexCount = data.SizeInBytes / data.IndexFormat.SizeInBytes();
            IndexFormat = data.IndexFormat;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.BufferDescription
            {
                BindFlags = D3D11.BindFlags.IndexBuffer,
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
        }

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
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Ges the native D3D11 buffer.
        /// </summary>
        public D3D11.Buffer D3DBuffer => _nativeBuffer;

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
            D3D11.DeviceContext d3dContext = D3DDevice.ImmediateContext;
            int bufferSizeInBytes = IndexCount * IndexFormat.SizeInBytes();

            ResourceHelper.ReadBufferData(_nativeBuffer, d3dContext, bufferSizeInBytes, ResourceUsage, data, startIndex, elementCount, offsetInBytes);
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
            D3D11.DeviceContext d3dContext = Direct3DHelper.GetD3DDeviceContext(renderContext);
            int bufferSizeInBytes = IndexCount * IndexFormat.SizeInBytes();

            ResourceHelper.WriteBufferData(_nativeBuffer, d3dContext, bufferSizeInBytes, ResourceUsage, data, startIndex, elementCount, offsetInBytes, writeOptions);
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

            if (isDisposing && _nativeBuffer != null)
            {
                _nativeBuffer.Dispose();
                _nativeBuffer = null;
            }

            base.Dispose(isDisposing);
        }
    }
}
