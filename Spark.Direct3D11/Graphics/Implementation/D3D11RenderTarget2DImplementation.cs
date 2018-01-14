namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Math;
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="RenderTarget2D"/> and <see cref="RenderTarget2DArray"/>.
    /// </summary>
    public sealed class D3D11RenderTarget2DImplementation : GraphicsResourceImplementation, IRenderTarget2DImplementation, IRenderTarget2DArrayImplementation, ID3D11RenderTargetView, ID3D11ShaderResourceView
    {
        private readonly D3D11RenderTargetWrapper _targetWrapper;

        internal D3D11RenderTarget2DImplementation(D3D11RenderSystem renderSystem, int resourceID, int width, int height, int arrayCount, int mipMapCount, SurfaceFormat format, MSAADescription preferredMSAA, DepthFormat depthFormat, bool preferReadableDepth, RenderTargetUsage targetUsage)
            : base(renderSystem, resourceID)
        {
            _targetWrapper = new D3D11RenderTargetWrapper(renderSystem.D3DDevice, width, height, false, arrayCount, mipMapCount, format, preferredMSAA, depthFormat, preferReadableDepth, arrayCount > 1, targetUsage);

            Name = string.Empty;
        }

        internal D3D11RenderTarget2DImplementation(D3D11RenderSystem renderSystem, int resourceID, SurfaceFormat format, int mipMapCount, D3D11DepthStencilBufferWrapper depthBuffer)
            : base(renderSystem, resourceID)
        {

            _targetWrapper = new D3D11RenderTargetWrapper(format, mipMapCount, depthBuffer);

            Name = string.Empty;
        }

        /// <summary>
        /// Gets the depth stencil buffer associated with the render target, if no buffer is associated then this will be null.
        /// </summary>
        public IDepthStencilBuffer DepthStencilBuffer => _targetWrapper.DepthStencilBuffer;

        /// <summary>
        /// Gets the multisample settings for the resource. The MSAA count, quality, and if the resource should be resolved to
        /// a non-MSAA resource for shader input. MSAA targets that do not resolve to a non-MSAA resource will only ever have one mip map per array slice.
        /// </summary>
        public MSAADescription MultisampleDescription => _targetWrapper.MultisampleDescription;

        /// <summary>
        /// Gets the target usage, specifying how the target should be handled when it is bound to the pipeline. Generally this is
        /// set to discard by default.
        /// </summary>
        public RenderTargetUsage TargetUsage => _targetWrapper.TargetUsage;

        /// <summary>
        /// Gets the format of the texture resource.
        /// </summary>
        public SurfaceFormat Format => _targetWrapper.Format;

        /// <summary>
        /// Gets the number of array slices in the texture. Slices may be indexed in the range [0, ArrayCount).
        /// </summary>
        public int ArrayCount => _targetWrapper.ArrayCount;

        /// <summary>
        /// Gets the number of mip map levels in the texture resource. Mip levels may be indexed in the range of [0, MipCount).
        /// </summary>
        public int MipCount => _targetWrapper.MipCount;

        /// <summary>
        /// Gets the texture width, in texels.
        /// </summary>
        public int Width => _targetWrapper.Width;

        /// <summary>
        /// Gets the texture height, in texels.
        /// </summary>
        public int Height => _targetWrapper.Height;

        /// <summary>
        /// Gets the resource usage of the texture.
        /// </summary>
        public ResourceUsage ResourceUsage => ResourceUsage.Static;

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice => _targetWrapper.D3DDevice;

        /// <summary>
        /// Gets the native D3D render target view.
        /// </summary>
        public D3D11.RenderTargetView D3DRenderTargetView => _targetWrapper.D3DRenderTargetView;

        /// <summary>
        /// Gets the native D3D11 shader resource view. If multisampled and set to resolve,
        /// this is a view of the resolve texture, otherwise its a view of the non-resolve texture.
        /// </summary>
        public D3D11.ShaderResourceView D3DShaderResourceView => _targetWrapper.D3DShaderResourceView;

        /// <summary>
        /// Gets the native D3D11 texture (may or may not be multisampled).
        /// </summary>
        public D3D11.Resource D3DTexture => _targetWrapper.D3DTexture;

        /// <summary>
        /// Gets the native D3D11 resolve texture (never multisampled), if it exists.
        /// </summary>
        public D3D11.Resource D3DResolveTexture => _targetWrapper.D3DResolveTexture;

        /// <summary>
        /// Gets the generic render target implementation.
        /// </summary>
        /// <returns></returns>
        public D3D11RenderTargetWrapper GetD3D11RenderTargetWrapper()
        {
            return _targetWrapper;
        }

        /// <summary>
        /// Gets a sub render target at the specified array index.
        /// </summary>
        /// <param name="arrayIndex">Zero-based index of the sub render target.</param>
        /// <returns>The sub render target.</returns>
        public IRenderTarget GetSubRenderTarget(int arrayIndex)
        {
            return _targetWrapper.GetSubRenderTarget(arrayIndex);
        }

        /// <summary>
        /// Gets a sub texture at the specified array index.
        /// </summary>
        /// <param name="arraySlice">Zero-based index of the sub texture.</param>
        /// <returns>The sub texture.</returns>
        public IShaderResource GetSubTexture(int arraySlice)
        {
            return _targetWrapper.GetSubRenderTarget(arraySlice);
        }

        /// <summary>
        /// Called on the first render target in the group before the group is bound to the context.
        /// </summary>
        public void NotifyOnFirstBind()
        {
            _targetWrapper.NotifyOnFirstBind();
        }

        /// <summary>
        /// Resolves the resource if its multisampled and does any mip map generation.
        /// </summary>
        /// <param name="deviceContext">Device context</param>
        public void ResolveResource(D3D11.DeviceContext deviceContext)
        {
            _targetWrapper.ResolveResource(deviceContext);
        }

        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="deviceContext">Device context</param>
        /// <param name="options">Clear options</param>
        /// <param name="color">Color to clear to.</param>
        /// <param name="depth">Depth to clear to.</param>
        /// <param name="stencil">Stencil to clear to</param>
        public void Clear(D3D11.DeviceContext deviceContext, ClearOptions options, Color color, float depth, int stencil)
        {
            _targetWrapper.Clear(deviceContext, options, color, depth, stencil);
        }

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, int mipLevel, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            D3D11.DeviceContext d3dContext = _targetWrapper.D3DDevice.ImmediateContext;

            ResourceRegion3D? region;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }
            else
            {
                region = new ResourceRegion3D(0, _targetWrapper.Width, 0, _targetWrapper.Height, 0, 1);
            }

            D3D11.Resource resource = _targetWrapper.D3DResolveTexture ?? _targetWrapper.D3DTexture;

            ResourceHelper.ReadTextureData(resource, d3dContext, _targetWrapper.Width, _targetWrapper.Height, 1, _targetWrapper.ArrayCount, _targetWrapper.MipCount, _targetWrapper.Format, ResourceUsage.Static, data, 0, mipLevel, ref region, startIndex);
        }

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            D3D11.DeviceContext d3dContext = _targetWrapper.D3DDevice.ImmediateContext;

            ResourceRegion3D? region;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }
            else
            {
                region = new ResourceRegion3D(0, _targetWrapper.Width, 0, _targetWrapper.Height, 0, 1);
            }

            D3D11.Resource resource = _targetWrapper.D3DResolveTexture ?? _targetWrapper.D3DTexture;

            ResourceHelper.ReadTextureData(resource, d3dContext, _targetWrapper.Width, _targetWrapper.Height, 1, _targetWrapper.ArrayCount, _targetWrapper.MipCount, _targetWrapper.Format, ResourceUsage.Static, data, arraySlice, mipLevel, ref region, startIndex);
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int mipLevel, ResourceRegion2D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            D3D11.DeviceContext d3dContext = Direct3DHelper.GetD3DDeviceContext(renderContext);

            ResourceRegion3D? region;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }
            else
            {
                region = new ResourceRegion3D(0, _targetWrapper.Width, 0, _targetWrapper.Height, 0, 1);
            }

            D3D11.Resource resource = _targetWrapper.D3DResolveTexture ?? _targetWrapper.D3DTexture;

            ResourceHelper.WriteTextureData(resource, d3dContext, _targetWrapper.Width, _targetWrapper.Height, 1, _targetWrapper.ArrayCount, _targetWrapper.MipCount, _targetWrapper.Format, ResourceUsage.Static, data, 0, mipLevel, ref region, startIndex, writeOptions);
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion2D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            D3D11.DeviceContext d3dContext = Direct3DHelper.GetD3DDeviceContext(renderContext);

            ResourceRegion3D? region;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }
            else
            {
                region = new ResourceRegion3D(0, _targetWrapper.Width, 0, _targetWrapper.Height, 0, 1);
            }

            D3D11.Resource resource = _targetWrapper.D3DResolveTexture ?? _targetWrapper.D3DTexture;

            ResourceHelper.WriteTextureData(resource, d3dContext, _targetWrapper.Width, _targetWrapper.Height, 1, _targetWrapper.ArrayCount, _targetWrapper.MipCount, _targetWrapper.Format, ResourceUsage.Static, data, arraySlice, mipLevel, ref region, startIndex, writeOptions);
        }

        /// <summary>
        /// Called when the name of the graphics resource is changed, useful if the implementation wants to set the name to
        /// be used as a debug name.
        /// </summary>
        /// <param name="name">New name of the resource</param>
        protected override void OnNameChange(string name)
        {
            _targetWrapper.Name = name;
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
                _targetWrapper.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}
