namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D11 = SharpDX.Direct3D11;
    using SDX = SharpDX;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="Texture3D"/>.
    /// </summary>
    public sealed class D3D11Texture3DImplementation : GraphicsResourceImplementation, ITexture3DImplementation
    {
        private D3D11.Texture3D _nativeTexture;
        private D3D11.ShaderResourceView _shaderResourceView;

        internal D3D11Texture3DImplementation(D3D11RenderSystem renderSystem, int resourceID, int width, int height, int depth, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            Width = width;
            Height = height;
            Depth = depth;
            MipCount = mipMapCount;
            Format = format;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.Texture3DDescription
            {
                MipLevels = MipCount,
                Width = Width,
                Height = Height,
                Depth = Depth,
                Usage = Direct3DHelper.ToD3DResourceUsage(ResourceUsage),
                Format = Direct3DHelper.ToD3DSurfaceFormat(Format),
                BindFlags = D3D11.BindFlags.ShaderResource,
                CpuAccessFlags = (ResourceUsage == ResourceUsage.Dynamic) ? D3D11.CpuAccessFlags.Write : D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None
            };

            _nativeTexture = new D3D11.Texture3D(D3DDevice, desc);
            _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _nativeTexture);

            Name = string.Empty;
        }

        internal D3D11Texture3DImplementation(D3D11RenderSystem renderSystem, int resourceID, int width, int height, int depth, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            Width = width;
            Height = height;
            Depth = depth;
            MipCount = mipMapCount;
            Format = format;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.Texture3DDescription
            {
                MipLevels = MipCount,
                Width = Width,
                Height = Height,
                Depth = Depth,
                Usage = Direct3DHelper.ToD3DResourceUsage(ResourceUsage),
                Format = Direct3DHelper.ToD3DSurfaceFormat(Format),
                BindFlags = D3D11.BindFlags.ShaderResource,
                CpuAccessFlags = (ResourceUsage == ResourceUsage.Dynamic) ? D3D11.CpuAccessFlags.Write : D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None
            };

            SDX.DataBox[] ptrs = ResourceHelper.MapDataBuffers(Format, 1, MipCount, Width, Height, data);

            try
            {
                _nativeTexture = new D3D11.Texture3D(D3DDevice, desc, ptrs);
                _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _nativeTexture);
            }
            finally
            {
                ResourceHelper.UnmapDataBuffers(data);
            }

            Name = string.Empty;
        }

        /// <summary>
        /// Gets the format of the texture resource.
        /// </summary>
        public SurfaceFormat Format { get; }

        /// <summary>
        /// Gets the number of mip map levels in the texture resource. Mip levels may be indexed in the range of [0, MipCount).
        /// </summary>
        public int MipCount { get; }

        /// <summary>
        /// Gets the texture width, in texels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the texture height, in texels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the texture depth, in texels.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Gets the resource usage of the texture.
        /// </summary>
        public ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Gets the native D3D11 texture.
        /// </summary>
        public D3D11.Texture3D D3DTexture3D => _nativeTexture;

        /// <summary>
        /// Gets the native D3D11 shader resource view.
        /// </summary>
        public D3D11.ShaderResourceView D3DShaderResourceView => _shaderResourceView;

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 3D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, int mipLevel, ResourceRegion3D? subimage, int startIndex) where T : struct
        {
            D3D11.DeviceContext d3dContext = D3DDevice.ImmediateContext;

            ResourceHelper.ReadTextureData(_nativeTexture, d3dContext, Width, Height, Depth, 1, MipCount, Format, ResourceUsage, data, 0, mipLevel, ref subimage, startIndex);
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 3D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int mipLevel, ResourceRegion3D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            D3D11.DeviceContext d3dContext = Direct3DHelper.GetD3DDeviceContext(renderContext);

            ResourceHelper.WriteTextureData(_nativeTexture, d3dContext, Width, Height, Depth, 1, MipCount, Format, ResourceUsage, data, 0, mipLevel, ref subimage, startIndex, writeOptions);
        }

        /// <summary>
        /// Called when the name of the graphics resource is changed, useful if the implementation wants to set the name to
        /// be used as a debug name.
        /// </summary>
        /// <param name="name">New name of the resource</param>
        protected override void OnNameChange(string name)
        {
            if (_nativeTexture != null)
            {
                _nativeTexture.DebugName = name;
            }

            if (_shaderResourceView != null)
            {
                _shaderResourceView.DebugName = (string.IsNullOrEmpty(name)) ? name : name + "_SRV";
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
                if (_nativeTexture != null)
                {
                    _nativeTexture.Dispose();
                    _nativeTexture = null;
                }

                if (_shaderResourceView != null)
                {
                    _shaderResourceView.Dispose();
                    _shaderResourceView = null;
                }
            }

            base.Dispose(isDisposing);
        }
    }
}
