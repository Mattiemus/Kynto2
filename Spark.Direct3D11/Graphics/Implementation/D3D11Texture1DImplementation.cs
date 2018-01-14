namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="Texture1D" /> and <see cref="Texture1DArray" />.
    /// </summary>
    public sealed class D3D11Texture1DImplementation : GraphicsResourceImplementation, ITexture1DImplementation, ITexture1DArrayImplementation, ID3D11ShaderResourceView
    {
        private D3D11.Texture1D _nativeTexture;
        private D3D11.ShaderResourceView _shaderResourceView;
        private D3D11ShaderResourceWrapper[] _subTextures;

        internal D3D11Texture1DImplementation(D3D11RenderSystem renderSystem, int resourceID, int width, int arrayCount, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            Width = width;
            ArrayCount = arrayCount;
            MipCount = mipMapCount;
            Format = format;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.Texture1DDescription
            {
                ArraySize = ArrayCount,
                MipLevels = MipCount,
                Width = Width,
                Usage = Direct3DHelper.ToD3DResourceUsage(ResourceUsage),
                Format = Direct3DHelper.ToD3DSurfaceFormat(Format),
                BindFlags = D3D11.BindFlags.ShaderResource,
                CpuAccessFlags = (ResourceUsage == ResourceUsage.Dynamic) ? D3D11.CpuAccessFlags.Write : D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None
            };

            _nativeTexture = new D3D11.Texture1D(D3DDevice, desc);
            _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _nativeTexture);

            Name = string.Empty;
        }

        internal D3D11Texture1DImplementation(D3D11RenderSystem renderSystem, int resourceID, int width, int arrayCount, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            Width = width;
            ArrayCount = arrayCount;
            MipCount = mipMapCount;
            Format = format;
            ResourceUsage = resourceUsage;

            var desc = new D3D11.Texture1DDescription
            {
                ArraySize = ArrayCount,
                MipLevels = MipCount,
                Width = Width,
                Usage = Direct3DHelper.ToD3DResourceUsage(ResourceUsage),
                Format = Direct3DHelper.ToD3DSurfaceFormat(Format),
                BindFlags = D3D11.BindFlags.ShaderResource,
                CpuAccessFlags = (ResourceUsage == ResourceUsage.Dynamic) ? D3D11.CpuAccessFlags.Write : D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None
            };

            IntPtr[] ptrs = ResourceHelper.MapDataBuffers(data);

            try
            {
                _nativeTexture = new D3D11.Texture1D(D3DDevice, desc, ptrs);
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
        /// Gets the number of array slices in the texture. Slices may be indexed in the range [0, ArrayCount).
        /// </summary>
        public int ArrayCount { get; }

        /// <summary>
        /// Gets the number of mip map levels in the texture resource. Mip levels may be indexed in the range of [0, MipCount).
        /// </summary>
        public int MipCount { get; }

        /// <summary>
        /// Gets the texture width, in texels.
        /// </summary>
        public int Width { get; }

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
        public D3D11.Texture1D D3DTexture1D => _nativeTexture;

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
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, int mipLevel, ResourceRegion1D? subimage, int startIndex) where T : struct
        {
            var d3dContext = D3DDevice.ImmediateContext;

            ResourceRegion3D? region = null;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }

            ResourceHelper.ReadTextureData(_nativeTexture, d3dContext, Width, 1, 1, ArrayCount, MipCount, Format, ResourceUsage, data, 0, mipLevel, ref region, startIndex);
        }


        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion1D? subimage, int startIndex) where T : struct
        {
            var d3dContext = D3DDevice.ImmediateContext;

            ResourceRegion3D? region = null;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }

            ResourceHelper.ReadTextureData(_nativeTexture, d3dContext, Width, 1, 1, ArrayCount, MipCount, Format, ResourceUsage, data, arraySlice, mipLevel, ref region, startIndex);
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int mipLevel, ResourceRegion1D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            var d3dContext = Direct3DHelper.GetD3DDeviceContext(renderContext);

            ResourceRegion3D? region = null;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }

            ResourceHelper.WriteTextureData(_nativeTexture, d3dContext, Width, 1, 1, ArrayCount, MipCount, Format, ResourceUsage, data, 0, mipLevel, ref region, startIndex, writeOptions);
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion1D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            D3D11.DeviceContext d3dContext = Direct3DHelper.GetD3DDeviceContext(renderContext);

            ResourceRegion3D? region = null;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }

            ResourceHelper.WriteTextureData(_nativeTexture, d3dContext, Width, 1, 1, ArrayCount, MipCount, Format, ResourceUsage, data, arraySlice, mipLevel, ref region, startIndex, writeOptions);
        }

        /// <summary>
        /// Gets a sub texture at the specified array index.
        /// </summary>
        /// <param name="arrayIndex">Zero-based index of the sub texture.</param>
        /// <returns>The sub texture.</returns>
        public IShaderResource GetSubTexture(int arrayIndex)
        {
            // Always return null if not an array resource
            if (ArrayCount == 1)
            {
                return null;
            }

            // Return null if out of bounds
            if (arrayIndex < 0 || arrayIndex >= ArrayCount)
            {
                return null;
            }

            if (_subTextures == null)
            {
                _subTextures = new D3D11ShaderResourceWrapper[ArrayCount];
            }

            D3D11ShaderResourceWrapper subTexture = _subTextures[arrayIndex];

            // Create the SRV as needed
            if (subTexture == null)
            {
                var desc = new D3D11.ShaderResourceViewDescription();
                desc.Texture1DArray.FirstArraySlice = arrayIndex;
                desc.Texture1DArray.ArraySize = 1;
                desc.Texture1DArray.MipLevels = MipCount;
                desc.Texture1DArray.MostDetailedMip = 0;
                desc.Format = Direct3DHelper.ToD3DSurfaceFormat(Format);
                desc.Dimension = D3D.ShaderResourceViewDimension.Texture1DArray;

                subTexture = new D3D11ShaderResourceWrapper(new D3D11.ShaderResourceView(D3DDevice, _nativeTexture, desc), true);
                _subTextures[arrayIndex] = subTexture;
            }

            return subTexture;
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

                if (_subTextures != null)
                {
                    for (int i = 0; i < _subTextures.Length; i++)
                    {
                        D3D11ShaderResourceWrapper subTexture = _subTextures[i];
                        subTexture?.Dispose();
                    }
                }

                _subTextures = null;
            }

            base.Dispose(isDisposing);
        }
    }
}
