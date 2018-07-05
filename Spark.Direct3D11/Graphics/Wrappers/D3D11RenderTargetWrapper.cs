namespace Spark.Direct3D11.Graphics
{
    using System;

    using Spark.Math;
    using Spark.Graphics;
    using Spark.Utilities;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;
    using DXGI = SharpDX.DXGI;
    using SDXM = SharpDX.Mathematics.Interop;

    public sealed class D3D11RenderTargetWrapper : Disposable, IRenderTarget, ID3D11RenderTargetView, ID3D11ShaderResourceView
    {
        private D3D11.RenderTargetView _renderTargetView;
        private D3D11.ShaderResourceView _shaderResourceView;
        private D3D11.Resource _nativeTexture;
        private D3D11.Resource _resolveTexture;
        private string _name;
        private readonly bool _ownsTexture;
        private D3D11DepthStencilBufferWrapper _parentDepthBuffer;
        private D3D11RenderTargetWrapper[] _subTargets;

        public D3D11RenderTargetWrapper(D3D11.Device device, int width, int arrayCount, int mipCount, SurfaceFormat format, MSAADescription msaaDesc, DepthFormat depthFormat, bool readableDepth, bool optimizeDepthForSingleSurface, RenderTargetUsage targetUsage)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            D3DDevice = device;
            _ownsTexture = true;
            MultisampleDescription = msaaDesc;
            Format = format;
            TargetUsage = targetUsage;
            IsArrayResource = false;
            _parentDepthBuffer = null;
            Parent = null;
            SubResourceIndex = -1;
            MipCount = mipCount;

            Initialize1DRenderTarget(width, arrayCount);
            InitializeDepthBuffer(depthFormat, readableDepth, optimizeDepthForSingleSurface);

            Name = string.Empty;
        }

        public D3D11RenderTargetWrapper(D3D11.Device device, int width, int height, bool isCube, int arrayCount, int mipCount, SurfaceFormat format, MSAADescription msaaDesc, DepthFormat depthFormat, bool readableDepth, bool optimizeDepthForSingleSurface, RenderTargetUsage targetUsage)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            D3DDevice = device;
            _ownsTexture = true;
            MultisampleDescription = msaaDesc;
            Format = format;
            TargetUsage = targetUsage;
            IsArrayResource = isCube;
            _parentDepthBuffer = null;
            Parent = null;
            SubResourceIndex = -1;
            MipCount = mipCount;

            Initialize2DRenderTarget(width, height, arrayCount);
            InitializeDepthBuffer(depthFormat, readableDepth, optimizeDepthForSingleSurface);

            Name = string.Empty;
        }

        public D3D11RenderTargetWrapper(D3D11.Device device, int width, int height, int depth, int mipCount, SurfaceFormat format, MSAADescription msaaDesc, DepthFormat depthFormat, bool readableDepth, bool optimizeDepthForSingleSurface, RenderTargetUsage targetUsage)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            D3DDevice = device;
            _ownsTexture = true;
            MultisampleDescription = msaaDesc;
            Format = format;
            TargetUsage = targetUsage;
            IsArrayResource = false;
            _parentDepthBuffer = null;
            Parent = null;
            SubResourceIndex = -1;
            MipCount = mipCount;

            Initialize3DRenderTarget(width, height, depth);
            InitializeDepthBuffer(depthFormat, readableDepth, optimizeDepthForSingleSurface);

            Name = string.Empty;
        }

        public D3D11RenderTargetWrapper(SurfaceFormat surfaceFormat, int mipCount, D3D11DepthStencilBufferWrapper depthBuffer)
        {
            if (depthBuffer == null)
            {
                throw new ArgumentNullException(nameof(depthBuffer));
            }

            if (depthBuffer.Parent != null)
            {
                depthBuffer = depthBuffer.Parent;
            }

            D3DDevice = depthBuffer.D3DDevice;
            _ownsTexture = true;
            MultisampleDescription = depthBuffer.MultisampleDescription;
            TargetUsage = depthBuffer.TargetUsage;
            MipCount = mipCount;
            Width = depthBuffer.Width;
            Height = depthBuffer.Height;
            Format = surfaceFormat;
            IsArrayResource = depthBuffer.IsArrayResource;
            IsArrayResource = depthBuffer.IsCubeResource;
            ResourceType = depthBuffer.ResourceType;
            SubResourceIndex = -1;
            _parentDepthBuffer = depthBuffer;
            Parent = null;

            if (depthBuffer.ResourceType == ShaderResourceType.Texture3D)
            {
                ArrayCount = 1;
                Depth = depthBuffer.ArrayCount;
            }
            else
            {
                ArrayCount = depthBuffer.ArrayCount;
                Depth = 1;
            }

            depthBuffer.AddRef();

            switch (ResourceType)
            {
                case ShaderResourceType.Texture3D:
                    Initialize3DRenderTarget(Width, Height, Depth);
                    break;
                case ShaderResourceType.TextureCubeMSArray:
                case ShaderResourceType.TextureCubeMS:
                case ShaderResourceType.TextureCubeArray:
                case ShaderResourceType.TextureCube:
                case ShaderResourceType.Texture2DMSArray:
                case ShaderResourceType.Texture2DMS:
                case ShaderResourceType.Texture2DArray:
                case ShaderResourceType.Texture2D:
                    Initialize2DRenderTarget(Width, Height, ArrayCount);
                    break;
                case ShaderResourceType.Texture1DArray:
                case ShaderResourceType.Texture1D:
                    Initialize1DRenderTarget(Width, ArrayCount);
                    break;
            }

            Name = string.Empty;
        }

        private D3D11RenderTargetWrapper(D3D11RenderTargetWrapper parentTarget, int arraySlice)
        {
            D3DDevice = parentTarget.D3DDevice;
            _ownsTexture = false;
            MultisampleDescription = parentTarget.MultisampleDescription;
            TargetUsage = parentTarget.TargetUsage;
            ArrayCount = parentTarget.ArrayCount;
            MipCount = parentTarget.MipCount;
            Width = parentTarget.Width;
            Height = parentTarget.Height;
            Depth = parentTarget.Depth;
            Format = parentTarget.Format;
            IsArrayResource = parentTarget.IsArrayResource;
            IsArrayResource = parentTarget.IsArrayResource;
            _nativeTexture = parentTarget._nativeTexture;
            _resolveTexture = parentTarget._resolveTexture;
            ResourceType = parentTarget.ResourceType;
            SubResourceIndex = arraySlice;
            Parent = parentTarget;
            _parentDepthBuffer = null;

            if (_nativeTexture is D3D11.Texture1D)
            {
                Initialize1DRenderTargetViewsOnly(arraySlice);
            }
            else if (_nativeTexture is D3D11.Texture2D)
            {
                Initialize2DRenderTargetViewsOnly(arraySlice);
            }
            else if (_nativeTexture is D3D11.Texture3D)
            {
                Initialize3DRenderTargetViewsOnly(arraySlice);
            }

            Name = parentTarget._name + "[" + GetSubName() + "]";
        }

        public ShaderResourceType ResourceType { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;

                if (_renderTargetView != null)
                {
                    _renderTargetView.DebugName = (string.IsNullOrEmpty(value)) ? value : value + "_RTV";
                }

                if (_shaderResourceView != null)
                {
                    _shaderResourceView.DebugName = (string.IsNullOrEmpty(value)) ? value : value + "_SRV";
                }

                if (_ownsTexture && _nativeTexture != null)
                {
                    _nativeTexture.DebugName = value;
                }

                if (_ownsTexture && _resolveTexture != null)
                {
                    _resolveTexture.DebugName = (string.IsNullOrEmpty(value)) ? value : value + "_RESOLVE";
                }
            }
        }

        public IDepthStencilBuffer DepthStencilBuffer => GetDepthBuffer();

        public DepthFormat DepthStencilFormat
        {
            get
            {
                D3D11DepthStencilBufferWrapper depthBuffer = GetDepthBuffer();
                if (depthBuffer == null)
                {
                    return DepthFormat.None;
                }

                return depthBuffer.DepthStencilFormat;
            }
        }

        public SurfaceFormat Format { get; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Depth { get; private set; }

        public int MipCount { get; }

        public int ArrayCount { get; private set; }

        public bool IsArrayResource { get; private set; }

        public bool IsCubeResource { get; }

        /// <summary>
        /// Gets if the resource is a sub-resource, representing an individual array slice if the main resource is an array resource or an
        /// individual face if its a cube resource. If this is a sub resource, then its sub resource index indicates the position in the array/cube.
        /// </summary>
        public bool IsSubResource => SubResourceIndex != -1;

        /// <summary>
        /// Gets the array index if the resource is a sub resource in an array. If not, then the index is -1.
        /// </summary>
        public int SubResourceIndex { get; }

        public MSAADescription MultisampleDescription { get; }

        public RenderTargetUsage TargetUsage { get; }

        public D3D11.Device D3DDevice { get; }

        public D3D11.RenderTargetView D3DRenderTargetView => _renderTargetView;

        public D3D11.ShaderResourceView D3DShaderResourceView => _shaderResourceView;

        public D3D11.Resource D3DTexture => _nativeTexture;

        public D3D11.Resource D3DResolveTexture => _resolveTexture;

        public D3D11RenderTargetWrapper Parent { get; }

        private bool ReallyWantResolve => MultisampleDescription.Count > 1 && MultisampleDescription.ResolveShaderResource;

        public IRenderTarget GetSubRenderTarget(int arrayIndex)
        {
            // Either array or cube and has to not be sub
            if (!IsArrayResource && IsArrayResource && IsSubResource)
            {
                return null;
            }

            int totalSubResources = ArrayCount;
            int actualArraySlice = arrayIndex;

            // Account for cube arrays, and if not an array but a cube map, we still have the 6 faces
            if (IsArrayResource && IsArrayResource)
            {
                actualArraySlice *= 6;
            }
            else if (IsArrayResource)
            {
                totalSubResources *= 6;
            }

            if (_subTargets == null)
            {
                _subTargets = new D3D11RenderTargetWrapper[totalSubResources];
            }

            if (arrayIndex < 0 || arrayIndex >= _subTargets.Length)
            {
                return null;
            }

            D3D11RenderTargetWrapper subTarget = _subTargets[arrayIndex];
            if (subTarget == null)
            {
                subTarget = new D3D11RenderTargetWrapper(this, actualArraySlice);
                _subTargets[arrayIndex] = subTarget;
            }

            return subTarget;
        }

        public void NotifyOnFirstBind()
        {
            if (IsSubResource)
            {
                return;
            }

            // For array/cube resources, we may have a depth buffer that has been optimized, where it uses one face that is
            // shared among everyone - but now we're binding the "parent" which may be the full array or cube resource, so we need
            // to "expand" the shared depth buffer. All resources that share this guy will come along, since we validate against
            // the size of the full resource anyways, any sub resources sharing this will eventually have individual views
            // created as necessary when/if they get bound.
            //
            // If you never bind the "full" array/cube resource, which probably will be common, you'll go along in life merrily, 
            // and consume less resource memory to boot,  this will only kick in if you want to do something fancy.
            if (_parentDepthBuffer != null && _parentDepthBuffer.IsOptimizedForSingleSurface)
            {
                _parentDepthBuffer.MakeOptimizedToFull();
            }
        }

        public void ResolveResource(D3D11.DeviceContext deviceContext)
        {
            if (_resolveTexture != null)
            {
                DXGI.Format resolveFormat = Direct3DHelper.ToD3DSurfaceFormat(Format);
                if (IsSubResource)
                {
                    deviceContext.ResolveSubresource(_nativeTexture, SubResourceIndex, _resolveTexture, SubResourceIndex, resolveFormat);
                }
                else
                {
                    for (int i = 0; i < ArrayCount; i++)
                    {
                        deviceContext.ResolveSubresource(_nativeTexture, i, _resolveTexture, i, resolveFormat);
                    }
                }

                // Hmm, only call if not a sub resource?
                if (MipCount > 1)
                {
                    deviceContext.GenerateMips(_shaderResourceView);
                }
            }
        }

        public void Clear(D3D11.DeviceContext deviceContext, ClearOptions options, Color color, float depth, int stencil)
        {
            if ((options & ClearOptions.Target) == ClearOptions.Target)
            {
                Direct3DHelper.ConvertColor(ref color, out SDXM.RawColor4 sdxColor);
                deviceContext.ClearRenderTargetView(_renderTargetView, sdxColor);
            }

            if ((options & ClearOptions.Depth) == ClearOptions.Depth || (options & ClearOptions.Stencil) == ClearOptions.Stencil)
            {
                D3D11DepthStencilBufferWrapper depthBuffer = GetDepthBuffer();
                depthBuffer?.Clear(deviceContext, options, depth, stencil);
            }
        }
        
        #region 1D Render Target Initialization

        private void Initialize1DRenderTarget(int width, int arrayCount)
        {
            // Set fields
            ArrayCount = arrayCount;
            Width = width;
            Height = 1;
            Depth = 1;
            IsArrayResource = arrayCount > 1;
            ResourceType = (IsArrayResource) ? ShaderResourceType.Texture1DArray : ShaderResourceType.Texture1D;

            // Setup texture resource

            var texDesc = new D3D11.Texture1DDescription();
            texDesc.Format = Direct3DHelper.ToD3DSurfaceFormat(Format);
            texDesc.Width = width;
            texDesc.MipLevels = MipCount;
            texDesc.ArraySize = arrayCount;
            texDesc.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            texDesc.OptionFlags = (MipCount > 1) ? D3D11.ResourceOptionFlags.GenerateMipMaps : D3D11.ResourceOptionFlags.None;
            texDesc.Usage = D3D11.ResourceUsage.Default;
            texDesc.BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource;

            _nativeTexture = new D3D11.Texture1D(D3DDevice, texDesc);

            // Can't multisample, so don't bother creating resolve texture

            Initialize1DRenderTargetViewsOnly(-1);
        }

        private void Initialize1DRenderTargetViewsOnly(int arraySlice)
        {
            // Setup render target view
            var rtvDesc = new D3D11.RenderTargetViewDescription();
            rtvDesc.Format = Direct3DHelper.ToD3DSurfaceFormat(Format);
            rtvDesc.Dimension = (IsArrayResource) ? D3D11.RenderTargetViewDimension.Texture1DArray : D3D11.RenderTargetViewDimension.Texture1D;

            // For creating views of sub textures
            int numArraySlices = (arraySlice < 0) ? ArrayCount : 1;
            arraySlice = (arraySlice < 0) ? 0 : arraySlice;

            if (IsArrayResource)
            {
                rtvDesc.Texture1DArray.FirstArraySlice = arraySlice;
                rtvDesc.Texture1DArray.ArraySize = numArraySlices;
                rtvDesc.Texture1DArray.MipSlice = 0;
            }
            else
            {
                rtvDesc.Texture1D.MipSlice = 0;
            }

            _renderTargetView = new D3D11.RenderTargetView(D3DDevice, _nativeTexture, rtvDesc);

            // Setup shader resource view

            var srvDesc = new D3D11.ShaderResourceViewDescription();
            srvDesc.Format = rtvDesc.Format;

            if (IsArrayResource)
            {
                srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture1DArray;
                srvDesc.Texture1DArray.FirstArraySlice = arraySlice;
                srvDesc.Texture1DArray.ArraySize = numArraySlices;
                srvDesc.Texture1DArray.MipLevels = MipCount;
                srvDesc.Texture1DArray.MostDetailedMip = 0;
            }
            else
            {
                srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture1D;
                srvDesc.Texture1D.MipLevels = MipCount;
                srvDesc.Texture1D.MostDetailedMip = 0;
            }

            // Can't multisample this texture, so don't bother checking for resolve texture

            _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _nativeTexture, srvDesc);
        }

        #endregion

        #region 2D Render Target Initialization

        private void Initialize2DRenderTarget(int width, int height, int arrayCount)
        {
            // Set fields
            ArrayCount = arrayCount;
            Width = width;
            Height = height;
            Depth = 1;
            IsArrayResource = arrayCount > 1;

            if (MultisampleDescription.IsMultisampled)
            {
                if (IsArrayResource)
                {
                    ResourceType = (IsArrayResource) ? ShaderResourceType.TextureCubeMSArray : ShaderResourceType.TextureCubeMS;
                }
                else
                {
                    ResourceType = (IsArrayResource) ? ShaderResourceType.Texture2DMSArray : ShaderResourceType.Texture2DMS;
                }
            }
            else
            {
                if (IsArrayResource)
                {
                    ResourceType = (IsArrayResource) ? ShaderResourceType.TextureCubeArray : ShaderResourceType.TextureCube;
                }
                else
                {
                    ResourceType = (IsArrayResource) ? ShaderResourceType.Texture2DArray : ShaderResourceType.Texture2D;
                }
            }

            // Setup texture resource

            var texDesc = new D3D11.Texture2DDescription();
            texDesc.Width = width;
            texDesc.Height = height;
            texDesc.MipLevels = (!MultisampleDescription.IsMultisampled) ? MipCount : 1;
            texDesc.ArraySize = (IsArrayResource) ? arrayCount * 6 : arrayCount;
            texDesc.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            texDesc.Usage = D3D11.ResourceUsage.Default;
            texDesc.SampleDescription = new DXGI.SampleDescription(MultisampleDescription.Count, MultisampleDescription.QualityLevel);
            texDesc.OptionFlags = (IsArrayResource && !MultisampleDescription.IsMultisampled) ? D3D11.ResourceOptionFlags.TextureCube : D3D11.ResourceOptionFlags.None;
            texDesc.OptionFlags |= (MipCount > 1) ? D3D11.ResourceOptionFlags.GenerateMipMaps : D3D11.ResourceOptionFlags.None;
            texDesc.BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource;
            texDesc.Format = Direct3DHelper.ToD3DSurfaceFormat(Format);

            _nativeTexture = new D3D11.Texture2D(D3DDevice, texDesc);

            // Check if want to create a non-msaa resolve texture
            if (ReallyWantResolve)
            {
                texDesc.SampleDescription = new DXGI.SampleDescription(1, 0);
                texDesc.MipLevels = MipCount;
                texDesc.OptionFlags = (IsArrayResource) ? D3D11.ResourceOptionFlags.TextureCube : D3D11.ResourceOptionFlags.None;
                _resolveTexture = new D3D11.Texture2D(D3DDevice, texDesc);
            }

            Initialize2DRenderTargetViewsOnly(-1);
        }

        private void Initialize2DRenderTargetViewsOnly(int arraySlice)
        {
            // Setup render target view
            var rtvDesc = new D3D11.RenderTargetViewDescription();
            rtvDesc.Format = Direct3DHelper.ToD3DSurfaceFormat(Format);

            if (MultisampleDescription.IsMultisampled)
            {
                rtvDesc.Dimension = (IsArrayResource || IsArrayResource) ? D3D11.RenderTargetViewDimension.Texture2DMultisampledArray : D3D11.RenderTargetViewDimension.Texture2DMultisampled;
            }
            else
            {
                rtvDesc.Dimension = (IsArrayResource || IsArrayResource) ? D3D11.RenderTargetViewDimension.Texture2DArray : D3D11.RenderTargetViewDimension.Texture2D;
            }

            // For creating views of sub textures
            int numArraySlices = (arraySlice < 0) ? (IsArrayResource) ? ArrayCount * 6 : ArrayCount : (IsArrayResource && IsArrayResource) ? 6 : 1;
            int cubeCount = (arraySlice < 0) ? ArrayCount : 1; // Only valid with cube resource array
            arraySlice = (arraySlice < 0) ? 0 : arraySlice;

            if (IsArrayResource || IsArrayResource)
            {
                if (MultisampleDescription.IsMultisampled)
                {
                    rtvDesc.Dimension = D3D11.RenderTargetViewDimension.Texture2DMultisampledArray;
                    rtvDesc.Texture2DMSArray.FirstArraySlice = arraySlice;
                    rtvDesc.Texture2DMSArray.ArraySize = numArraySlices;
                }
                else
                {
                    rtvDesc.Dimension = D3D11.RenderTargetViewDimension.Texture2DArray;
                    rtvDesc.Texture2DArray.FirstArraySlice = arraySlice;
                    rtvDesc.Texture2DArray.ArraySize = numArraySlices;
                    rtvDesc.Texture2DArray.MipSlice = 0;
                }
            }
            else
            {
                if (MultisampleDescription.IsMultisampled)
                {
                    rtvDesc.Dimension = D3D11.RenderTargetViewDimension.Texture2DMultisampled;
                }
                else
                {
                    rtvDesc.Dimension = D3D11.RenderTargetViewDimension.Texture2D;
                    rtvDesc.Texture2D.MipSlice = 0;
                }
            }

            _renderTargetView = new D3D11.RenderTargetView(D3DDevice, _nativeTexture, rtvDesc);

            // Setup shader resource view
            var srvDesc = new D3D11.ShaderResourceViewDescription();
            srvDesc.Format = rtvDesc.Format;

            if (IsArrayResource)
            {
                if (MultisampleDescription.IsMultisampled)
                {
                    srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture2DMultisampledArray;
                    srvDesc.Texture2DMSArray.FirstArraySlice = arraySlice;
                    srvDesc.Texture2DMSArray.ArraySize = numArraySlices;
                }
                else
                {
                    if (IsArrayResource)
                    {
                        srvDesc.Dimension = D3D.ShaderResourceViewDimension.TextureCubeArray;
                        srvDesc.TextureCubeArray.CubeCount = cubeCount;
                        srvDesc.TextureCubeArray.First2DArrayFace = arraySlice;
                        srvDesc.TextureCubeArray.MipLevels = 1;
                        srvDesc.TextureCubeArray.MostDetailedMip = 0;
                    }
                    else
                    {
                        srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture2DArray;
                        srvDesc.Texture2DArray.FirstArraySlice = arraySlice;
                        srvDesc.Texture2DArray.ArraySize = numArraySlices;
                        srvDesc.Texture2DArray.MipLevels = 1;
                        srvDesc.Texture2DArray.MostDetailedMip = 0;
                    }
                }
            }
            else
            {
                if (IsArrayResource)
                {
                    if (MultisampleDescription.IsMultisampled)
                    {
                        srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture2DMultisampledArray;
                        srvDesc.Texture2DMSArray.FirstArraySlice = arraySlice;
                        srvDesc.Texture2DMSArray.ArraySize = numArraySlices;
                    }
                    else
                    {
                        srvDesc.Dimension = D3D.ShaderResourceViewDimension.TextureCube;
                        srvDesc.TextureCube.MipLevels = 1;
                        srvDesc.TextureCube.MostDetailedMip = 0;
                    }
                }
                else
                {
                    if (MultisampleDescription.IsMultisampled)
                    {
                        srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture2DMultisampled;
                    }
                    else
                    {
                        srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture2D;
                        srvDesc.Texture2D.MipLevels = 1;
                        srvDesc.Texture2D.MostDetailedMip = 0;
                    }
                }
            }

            if (_resolveTexture != null && ReallyWantResolve)
            {
                _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _resolveTexture, srvDesc);
            }
            else
            {
                _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _nativeTexture, srvDesc);
            }
        }

        #endregion

        #region 3D Render Target Initialization

        private void Initialize3DRenderTarget(int width, int height, int depth)
        {
            // Set fields
            Width = width;
            Height = height;
            Depth = depth;
            ResourceType = ShaderResourceType.Texture3D;

            // Setup texture resource

            var texDesc = new D3D11.Texture3DDescription();
            texDesc.Width = width;
            texDesc.Height = height;
            texDesc.Depth = depth;
            texDesc.MipLevels = MipCount;
            texDesc.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            texDesc.OptionFlags = (MipCount > 1) ? D3D11.ResourceOptionFlags.GenerateMipMaps : D3D11.ResourceOptionFlags.None;
            texDesc.Usage = D3D11.ResourceUsage.Default;
            texDesc.BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource;
            texDesc.Format = Direct3DHelper.ToD3DSurfaceFormat(Format);

            _nativeTexture = new D3D11.Texture3D(D3DDevice, texDesc);

            // Cant multisample, so don't bother to create resolve texture

            Initialize3DRenderTargetViewsOnly(-1);
        }

        private void Initialize3DRenderTargetViewsOnly(int depthSlice)
        {
            // Setup render target view
            var rtvDesc = new D3D11.RenderTargetViewDescription();
            rtvDesc.Format = Direct3DHelper.ToD3DSurfaceFormat(Format);
            rtvDesc.Dimension = D3D11.RenderTargetViewDimension.Texture3D;

            // For creating views of sub textures
            int numDepthSlices = (depthSlice < 0) ? Depth : 1;
            depthSlice = (depthSlice < 0) ? 0 : depthSlice;

            rtvDesc.Texture3D.DepthSliceCount = numDepthSlices;
            rtvDesc.Texture3D.FirstDepthSlice = depthSlice;
            rtvDesc.Texture3D.MipSlice = 0;

            _renderTargetView = new D3D11.RenderTargetView(D3DDevice, _nativeTexture, rtvDesc);

            // Setup shader resource view
            var srvDesc = new D3D11.ShaderResourceViewDescription();
            srvDesc.Format = rtvDesc.Format;
            srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture3D;
            srvDesc.Texture3D.MipLevels = MipCount;
            srvDesc.Texture3D.MostDetailedMip = 0;

            // Can't multisample this texture, so don't bother checking for resolve texture

            _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _nativeTexture, srvDesc);
        }

        #endregion

        private void InitializeDepthBuffer(DepthFormat depthFormat, bool createShaderView, bool optimizeForSingleSurface)
        {
            if (depthFormat == DepthFormat.None)
            {
                return;
            }

            switch (ResourceType)
            {
                case ShaderResourceType.Texture1D:
                case ShaderResourceType.Texture1DArray:
                    _parentDepthBuffer = new D3D11DepthStencilBufferWrapper(D3DDevice, Width, depthFormat, ArrayCount, createShaderView, optimizeForSingleSurface, MultisampleDescription, TargetUsage);
                    _parentDepthBuffer.AddRef();
                    break;
                case ShaderResourceType.Texture2D:
                case ShaderResourceType.Texture2DArray:
                case ShaderResourceType.Texture2DMS:
                case ShaderResourceType.Texture2DMSArray:
                case ShaderResourceType.TextureCube:
                case ShaderResourceType.TextureCubeMS:
                case ShaderResourceType.TextureCubeArray:
                case ShaderResourceType.TextureCubeMSArray:
                    _parentDepthBuffer = new D3D11DepthStencilBufferWrapper(D3DDevice, Width, Height, IsArrayResource, depthFormat, ArrayCount, createShaderView, optimizeForSingleSurface, MultisampleDescription, TargetUsage);
                    _parentDepthBuffer.AddRef();
                    break;
                case ShaderResourceType.Texture3D:
                    _parentDepthBuffer = new D3D11DepthStencilBufferWrapper(D3DDevice, Width, Height, Depth, depthFormat, createShaderView, optimizeForSingleSurface, MultisampleDescription, TargetUsage);
                    _parentDepthBuffer.AddRef();
                    break;
            }
        }

        private D3D11DepthStencilBufferWrapper GetDepthBuffer()
        {
            if (IsSubResource)
            {
                D3D11DepthStencilBufferWrapper parentDepthBuffer = Parent._parentDepthBuffer;

                if (parentDepthBuffer == null)
                {
                    return null;
                }

                if (parentDepthBuffer.IsOptimizedForSingleSurface)
                {
                    return parentDepthBuffer;
                }

                return parentDepthBuffer.GetSubBuffer(SubResourceIndex);
            }
            else
            {
                return _parentDepthBuffer;
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }
            
            if (isDisposing)
            {
                if (_renderTargetView != null)
                {
                    _renderTargetView.Dispose();
                    _renderTargetView = null;
                }

                if (_shaderResourceView != null)
                {
                    _shaderResourceView.Dispose();
                    _shaderResourceView = null;
                }

                if (_ownsTexture && _resolveTexture != null)
                {
                    _resolveTexture.Dispose();
                    _resolveTexture = null;
                }

                if (_ownsTexture && _nativeTexture != null)
                {
                    _nativeTexture.Dispose();
                    _nativeTexture = null;
                }

                if (_parentDepthBuffer != null)
                {
                    _parentDepthBuffer.Release();
                }
            }

            base.Dispose(isDisposing);
        }

        private string GetSubName()
        {
            if (IsArrayResource)
            {
                if (IsArrayResource)
                {
                    // Cube arrays in multiples of 6, so all we can do is get the "array index" of the cube
                    int cubeIndex = SubResourceIndex / 6;
                    return cubeIndex.ToString();
                }

                return "Face: " + ((CubeMapFace)SubResourceIndex).ToString();
            }

            return SubResourceIndex.ToString();
        }
    }
}
