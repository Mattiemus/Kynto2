namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.Threading;

    using Spark.Graphics;
    using Spark.Utilities;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;
    using DXGI = SharpDX.DXGI;

    public sealed class D3D11DepthStencilBufferWrapper : Disposable, IDepthStencilBuffer, ID3D11DepthStencilView, ID3D11ShaderResourceView
    {
        private D3D11.DepthStencilView _depthStencilView;
        private D3D11.DepthStencilView _readOnlyDepthStencilView;
        private D3D11.ShaderResourceView _shaderResourceView;
        private D3D11.Resource _nativeTexture;
        private string _name;
        private readonly bool _ownsTexture;
        private int _refCount;
        private D3D11DepthStencilBufferWrapper[] _subBuffers;

        public D3D11DepthStencilBufferWrapper(D3D11.Device device, int width, DepthFormat depthFormat, int arrayCount, bool createShaderView, bool optimizeForSingleSurface, MSAADescription msaaDesc, RenderTargetUsage targetUsage)
        {
            Guard.Against.NullArgument(device, nameof(device));
            
            D3DDevice = device;
            _ownsTexture = true;
            IsOptimizedForSingleSurface = optimizeForSingleSurface;
            IsReadable = createShaderView;
            MultisampleDescription = new MSAADescription(1, 0, msaaDesc.ResolveShaderResource);
            TargetUsage = targetUsage;
            IsCubeResource = false;
            DepthStencilFormat = depthFormat;
            Parent = null;
            SubResourceIndex = -1;

            Initialize1DDepthBuffer(width, arrayCount);

            Name = string.Empty;
        }

        public D3D11DepthStencilBufferWrapper(D3D11.Device device, int width, int height, bool isCube, DepthFormat depthFormat, int arrayCount, bool createShaderView, bool optimizeForSingleSurface, MSAADescription msaaDesc, RenderTargetUsage targetUsage)
        {
            Guard.Against.NullArgument(device, nameof(device));

            D3DDevice = device;
            _ownsTexture = true;
            IsOptimizedForSingleSurface = optimizeForSingleSurface;
            IsReadable = createShaderView;
            MultisampleDescription = msaaDesc;
            TargetUsage = targetUsage;
            IsCubeResource = isCube;
            DepthStencilFormat = depthFormat;
            Parent = null;
            SubResourceIndex = -1;

            Initialize2DDepthBuffer(width, height, arrayCount);

            Name = string.Empty;
        }

        public D3D11DepthStencilBufferWrapper(D3D11.Device device, int width, int height, int depth, DepthFormat depthFormat, bool createShaderView, bool optimizeForSingleSurface, MSAADescription msaaDesc, RenderTargetUsage targetUsage)
        {
            Guard.Against.NullArgument(device, nameof(device));

            D3DDevice = device;
            _ownsTexture = true;
            IsOptimizedForSingleSurface = optimizeForSingleSurface;
            IsReadable = createShaderView;
            MultisampleDescription = new MSAADescription(1, 0, msaaDesc.ResolveShaderResource);
            TargetUsage = targetUsage;
            IsCubeResource = false;
            IsArrayResource = true;
            DepthStencilFormat = depthFormat;
            Parent = null;
            SubResourceIndex = -1;

            Initialize3DDepthBuffer(width, height, depth);

            Name = string.Empty;
        }

        private D3D11DepthStencilBufferWrapper(D3D11DepthStencilBufferWrapper parentBuffer, int arraySlice)
        {
            D3DDevice = parentBuffer.D3DDevice;
            _ownsTexture = false;
            IsOptimizedForSingleSurface = false;
            IsReadable = parentBuffer.IsReadable;
            MultisampleDescription = parentBuffer.MultisampleDescription;
            TargetUsage = parentBuffer.TargetUsage;
            ArrayCount = parentBuffer.ArrayCount;
            Width = parentBuffer.Width;
            Height = parentBuffer.Height;
            IsArrayResource = parentBuffer.IsArrayResource;
            IsCubeResource = parentBuffer.IsCubeResource;
            _nativeTexture = parentBuffer._nativeTexture;
            ResourceType = parentBuffer.ResourceType;
            DepthStencilFormat = parentBuffer.DepthStencilFormat;
            Parent = parentBuffer;
            SubResourceIndex = arraySlice;

            if (_nativeTexture is D3D11.Texture1D)
            {
                Initialize1DDepthBufferViewsOnly(arraySlice);
            }
            else if (_nativeTexture is D3D11.Texture2D || _nativeTexture is D3D11.Texture3D)
            {
                Initialize2DDepthBufferViewsOnly(arraySlice);
            }

            // Adjust for cube arrays
            if (IsCubeResource && IsArrayResource)
            {
                SubResourceIndex /= 6;
            }

            Name = parentBuffer._name + "[" + GetSubName() + "]";
        }

        public ShaderResourceType ResourceType { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;

                if (_depthStencilView != null)
                {
                    _depthStencilView.DebugName = value ?? String.Format("{0}_DSV", value);
                }

                if (_readOnlyDepthStencilView != null)
                {
                    _readOnlyDepthStencilView.DebugName = value ?? String.Format("{0}_DSV [READONLY]", value);
                }

                if (_shaderResourceView != null)
                {
                    _shaderResourceView.DebugName = value ?? String.Format("{0}_SRV", value);
                }

                if (_ownsTexture && _nativeTexture != null)
                {
                    _nativeTexture.DebugName = value;
                }
            }
        }

        public bool IsReadable { get; }

        public bool IsShareable => true;

        public DepthFormat DepthStencilFormat { get; }

        public int Width { get; private set; }

        public int Height { get; private set; }

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

        public D3D11.DepthStencilView D3DDepthStencilView => _depthStencilView;

        public D3D11.DepthStencilView D3DReadOnlyDepthStencilView => _readOnlyDepthStencilView;

        public D3D11.ShaderResourceView D3DShaderResourceView => _shaderResourceView;

        public D3D11.Resource D3DTexture => _nativeTexture;

        public D3D11DepthStencilBufferWrapper Parent { get; }

        public bool IsOptimizedForSingleSurface { get; private set; }

        public D3D11DepthStencilBufferWrapper GetSubBuffer(int subResourceIndex)
        {
            // Either array or cube and has to not be sub -- or if we're optimized, then we'll only ever have one surface
            if ((!IsArrayResource && !IsCubeResource && IsSubResource) || IsOptimizedForSingleSurface)
            {
                return null;
            }

            int totalSubResources = ArrayCount;
            int actualArraySlice = subResourceIndex;

            // Account for cube arrays, and if not an array but a cube map, we still have the 6 faces.
            if (IsCubeResource && IsArrayResource)
            {
                actualArraySlice *= 6;
            }
            else if (IsCubeResource)
            {
                totalSubResources *= 6;
            }

            if (_subBuffers == null)
            {
                _subBuffers = new D3D11DepthStencilBufferWrapper[totalSubResources];
            }

            if (subResourceIndex < 0 || subResourceIndex >= _subBuffers.Length)
            {
                return null;
            }

            D3D11DepthStencilBufferWrapper subBuffer = _subBuffers[subResourceIndex];
            if (subBuffer == null)
            {
                subBuffer = new D3D11DepthStencilBufferWrapper(this, actualArraySlice);
                _subBuffers[subResourceIndex] = subBuffer;
            }

            return subBuffer;
        }

        public void Clear(D3D11.DeviceContext deviceContext, ClearOptions options, float depth, int stencil)
        {
            if (options == ClearOptions.Target)
            {
                return;
            }

            D3D11.DepthStencilClearFlags clearFlags;
            bool clearDepth = false;
            bool clearStencil = false;

            if ((options & ClearOptions.Depth) == ClearOptions.Depth)
            {
                clearDepth = true;
            }

            if ((options & ClearOptions.Stencil) == ClearOptions.Stencil)
            {
                clearStencil = true;
            }

            if (clearDepth && clearStencil)
            {
                clearFlags = D3D11.DepthStencilClearFlags.Depth | D3D11.DepthStencilClearFlags.Stencil;
            }
            else if (clearDepth)
            {
                clearFlags = D3D11.DepthStencilClearFlags.Depth;
            }
            else
            {
                clearFlags = D3D11.DepthStencilClearFlags.Stencil;
            }

            deviceContext.ClearDepthStencilView(_depthStencilView, clearFlags, depth, (byte)stencil);
        }

        internal void AddRef()
        {
            Interlocked.Increment(ref _refCount);
        }

        internal void Release()
        {
            Interlocked.Decrement(ref _refCount);

            if (_refCount == 0)
            {
                Dispose();
            }
        }

        public void MakeOptimizedToFull()
        {
            if (IsOptimizedForSingleSurface)
            {
                IsOptimizedForSingleSurface = false;

                if (_depthStencilView != null)
                {
                    _depthStencilView.Dispose();
                    _depthStencilView = null;
                }

                if (_readOnlyDepthStencilView != null)
                {
                    _readOnlyDepthStencilView.Dispose();
                    _readOnlyDepthStencilView = null;
                }

                if (_shaderResourceView != null)
                {
                    _shaderResourceView.Dispose();
                    _shaderResourceView = null;
                }

                if (_nativeTexture != null)
                {
                    _nativeTexture.Dispose();
                    _nativeTexture = null;
                }

                switch (ResourceType)
                {
                    case ShaderResourceType.Texture3D:
                        Initialize3DDepthBuffer(Width, Height, ArrayCount);
                        break;
                    case ShaderResourceType.TextureCubeMSArray:
                    case ShaderResourceType.TextureCubeMS:
                    case ShaderResourceType.TextureCubeArray:
                    case ShaderResourceType.TextureCube:
                    case ShaderResourceType.Texture2DMSArray:
                    case ShaderResourceType.Texture2DArray:
                    case ShaderResourceType.Texture2DMS:
                    case ShaderResourceType.Texture2D:
                        Initialize2DDepthBuffer(Width, Height, ArrayCount);
                        break;
                    case ShaderResourceType.Texture1DArray:
                    case ShaderResourceType.Texture1D:
                        Initialize1DDepthBuffer(Width, ArrayCount);
                        break;
                }
            }
        }

        private string GetSubName()
        {
            if (IsCubeResource)
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

        #region 1D Depth Buffer Initialization

        private void Initialize1DDepthBuffer(int width, int arrayCount)
        {
            // Set fields
            ArrayCount = arrayCount;
            Width = width;
            Height = 1;
            IsArrayResource = arrayCount > 1;
            ResourceType = (IsArrayResource) ? ShaderResourceType.Texture1DArray : ShaderResourceType.Texture1D;

            // Setup texture resource
            var texDesc = new D3D11.Texture1DDescription
            {
                Width = width,
                MipLevels = 1,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                Usage = D3D11.ResourceUsage.Default
            };

            if (!IsOptimizedForSingleSurface)
            {
                texDesc.ArraySize = arrayCount;
            }
            else
            {
                texDesc.ArraySize = 1;
            }

            if (IsReadable)
            {
                texDesc.BindFlags = D3D11.BindFlags.DepthStencil | D3D11.BindFlags.ShaderResource;
                texDesc.Format = Direct3DHelper.ToD3DTextureFormatFromDepthFormat(DepthStencilFormat);
            }
            else
            {
                texDesc.BindFlags = D3D11.BindFlags.DepthStencil;
                texDesc.Format = Direct3DHelper.ToD3DDepthFormat(DepthStencilFormat);
            }

            _nativeTexture = new D3D11.Texture1D(D3DDevice, texDesc);

            Initialize1DDepthBufferViewsOnly(-1);
        }

        private void Initialize1DDepthBufferViewsOnly(int arraySlice)
        {
            // Setup depth stencil view
            var dsvDesc = new D3D11.DepthStencilViewDescription
            {
                Flags = D3D11.DepthStencilViewFlags.None,
                Format = Direct3DHelper.ToD3DDepthFormat(DepthStencilFormat)
            };

            // For creating views of sub textures
            int numArraySlices = (arraySlice < 0) ? ArrayCount : 1;
            arraySlice = (arraySlice < 0) ? 0 : arraySlice;

            if (!IsOptimizedForSingleSurface)
            {
                if (IsArrayResource)
                {
                    dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture1DArray;
                    dsvDesc.Texture1DArray.FirstArraySlice = arraySlice;
                    dsvDesc.Texture1DArray.ArraySize = numArraySlices;
                    dsvDesc.Texture1DArray.MipSlice = 0;
                }
                else
                {
                    dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture1D;
                    dsvDesc.Texture1D.MipSlice = 0;
                }
            }
            else
            {
                dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture1D;
                dsvDesc.Texture1D.MipSlice = 0;
            }

            _depthStencilView = new D3D11.DepthStencilView(D3DDevice, _nativeTexture, dsvDesc);

            // Set up shader resource view
            if (IsReadable)
            {
                var srvDesc = new D3D11.ShaderResourceViewDescription();
                srvDesc.Format = Direct3DHelper.ToD3DShaderResourceFormatFromDepthFormat(DepthStencilFormat);

                if (!IsOptimizedForSingleSurface)
                {
                    if (IsArrayResource)
                    {
                        srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture1DArray;
                        srvDesc.Texture1DArray.FirstArraySlice = arraySlice;
                        srvDesc.Texture1DArray.ArraySize = numArraySlices;
                        srvDesc.Texture1DArray.MipLevels = 1;
                        srvDesc.Texture1DArray.MostDetailedMip = 0;
                    }
                    else
                    {
                        srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture1D;
                        srvDesc.Texture1D.MipLevels = 1;
                        srvDesc.Texture1D.MostDetailedMip = 0;
                    }
                }
                else
                {
                    srvDesc.Dimension = D3D.ShaderResourceViewDimension.Texture1D;
                    srvDesc.Texture1D.MipLevels = 1;
                    srvDesc.Texture1D.MostDetailedMip = 0;
                }

                _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _nativeTexture, srvDesc);
            }
        }

        #endregion

        #region 2D Depth Buffer Initialization

        private void Initialize2DDepthBuffer(int width, int height, int arrayCount)
        {
            // Set fields
            ArrayCount = arrayCount;
            Width = width;
            Height = height;
            IsArrayResource = arrayCount > 1;

            if (MultisampleDescription.IsMultisampled)
            {
                if (IsCubeResource)
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
                if (IsCubeResource)
                {
                    ResourceType = (IsArrayResource) ? ShaderResourceType.TextureCubeArray : ShaderResourceType.TextureCube;
                }
                else
                {
                    ResourceType = (IsArrayResource) ? ShaderResourceType.Texture2DArray : ShaderResourceType.Texture2D;
                }
            }

            // Setup texture resource
            var texDesc = new D3D11.Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                Usage = D3D11.ResourceUsage.Default,
                SampleDescription = new DXGI.SampleDescription(MultisampleDescription.Count, MultisampleDescription.QualityLevel)
            };

            if (!IsOptimizedForSingleSurface)
            {
                texDesc.ArraySize = (IsCubeResource) ? arrayCount * 6 : arrayCount;
                texDesc.OptionFlags = (IsCubeResource && !MultisampleDescription.IsMultisampled) ? D3D11.ResourceOptionFlags.TextureCube : D3D11.ResourceOptionFlags.None;
            }
            else
            {
                texDesc.ArraySize = 1;
                texDesc.OptionFlags = D3D11.ResourceOptionFlags.None;
            }

            if (IsReadable)
            {
                texDesc.BindFlags = D3D11.BindFlags.DepthStencil | D3D11.BindFlags.ShaderResource;
                texDesc.Format = Direct3DHelper.ToD3DTextureFormatFromDepthFormat(DepthStencilFormat);
            }
            else
            {
                texDesc.BindFlags = D3D11.BindFlags.DepthStencil;
                texDesc.Format = Direct3DHelper.ToD3DDepthFormat(DepthStencilFormat);
            }

            _nativeTexture = new D3D11.Texture2D(D3DDevice, texDesc);

            Initialize2DDepthBufferViewsOnly(-1);
        }

        private void Initialize2DDepthBufferViewsOnly(int arraySlice)
        {
            // Setup depth stencil view
            var dsvDesc = new D3D11.DepthStencilViewDescription();
            dsvDesc.Flags = D3D11.DepthStencilViewFlags.None;
            dsvDesc.Format = Direct3DHelper.ToD3DDepthFormat(DepthStencilFormat);

            // For creating views of sub textures
            int numArraySlices = (arraySlice < 0) ? (IsCubeResource) ? ArrayCount * 6 : ArrayCount : (IsCubeResource && IsArrayResource) ? 6 : 1;
            int cubeCount = (arraySlice < 0) ? ArrayCount : 1; // Only valid with cube resource array
            arraySlice = (arraySlice < 0) ? 0 : arraySlice;

            if (!IsOptimizedForSingleSurface)
            {
                if (IsArrayResource || IsCubeResource)
                {
                    if (MultisampleDescription.IsMultisampled)
                    {
                        dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture2DMultisampledArray;
                        dsvDesc.Texture2DMSArray.FirstArraySlice = arraySlice;
                        dsvDesc.Texture2DMSArray.ArraySize = numArraySlices;
                    }
                    else
                    {
                        dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture2DArray;
                        dsvDesc.Texture2DArray.FirstArraySlice = arraySlice;
                        dsvDesc.Texture2DArray.ArraySize = numArraySlices;
                        dsvDesc.Texture2DArray.MipSlice = 0;
                    }
                }
                else
                {
                    if (MultisampleDescription.IsMultisampled)
                    {
                        dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture2DMultisampled;
                    }
                    else
                    {
                        dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture2D;
                        dsvDesc.Texture2D.MipSlice = 0;
                    }
                }
            }
            else
            {
                if (MultisampleDescription.IsMultisampled)
                {
                    dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture2DMultisampled;
                }
                else
                {
                    dsvDesc.Dimension = D3D11.DepthStencilViewDimension.Texture2D;
                    dsvDesc.Texture2D.MipSlice = 0;
                }
            }

            _depthStencilView = new D3D11.DepthStencilView(D3DDevice, _nativeTexture, dsvDesc);

            // Set up shader resource view
            if (IsReadable)
            {
                dsvDesc.Flags = SharpDX.Direct3D11.DepthStencilViewFlags.ReadOnlyDepth | SharpDX.Direct3D11.DepthStencilViewFlags.ReadOnlyStencil;
                _readOnlyDepthStencilView = new SharpDX.Direct3D11.DepthStencilView(D3DDevice, _nativeTexture, dsvDesc);

                var srvDesc = new D3D11.ShaderResourceViewDescription();
                srvDesc.Format = Direct3DHelper.ToD3DShaderResourceFormatFromDepthFormat(DepthStencilFormat);

                if (!IsOptimizedForSingleSurface)
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
                            if (IsCubeResource)
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
                        if (IsCubeResource)
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

                _shaderResourceView = new D3D11.ShaderResourceView(D3DDevice, _nativeTexture, srvDesc);
            }
        }

        #endregion

        #region 3D Depth Buffer Initialization

        private void Initialize3DDepthBuffer(int width, int height, int depth)
        {
            // Set fields
            ArrayCount = depth;
            Width = width;
            Height = height;
            ResourceType = ShaderResourceType.Texture3D;

            // Setup texture resource
            var texDesc = new D3D11.Texture2DDescription();
            texDesc.Width = width;
            texDesc.Height = height;
            texDesc.MipLevels = 1;
            texDesc.CpuAccessFlags = D3D11.CpuAccessFlags.None;
            texDesc.OptionFlags = D3D11.ResourceOptionFlags.None;
            texDesc.Usage = D3D11.ResourceUsage.Default;
            texDesc.SampleDescription = new DXGI.SampleDescription(1, 0);

            if (!IsOptimizedForSingleSurface)
            {
                texDesc.ArraySize = depth;
            }
            else
            {
                texDesc.ArraySize = 1;
            }

            if (IsReadable)
            {
                texDesc.BindFlags = D3D11.BindFlags.DepthStencil | D3D11.BindFlags.ShaderResource;
                texDesc.Format = Direct3DHelper.ToD3DTextureFormatFromDepthFormat(DepthStencilFormat);
            }
            else
            {
                texDesc.BindFlags = D3D11.BindFlags.DepthStencil;
                texDesc.Format = Direct3DHelper.ToD3DDepthFormat(DepthStencilFormat);
            }

            _nativeTexture = new D3D11.Texture2D(D3DDevice, texDesc);

            // Basically created a non-multisampled 2D texture array where array size is the depth of the 3D texture, from here on
            // its the same setup as a 2D texture array
            Initialize2DDepthBufferViewsOnly(-1);
        }

        #endregion

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (_depthStencilView != null)
                {
                    _depthStencilView.Dispose();
                    _depthStencilView = null;
                }

                if (_readOnlyDepthStencilView != null)
                {
                    _readOnlyDepthStencilView.Dispose();
                    _readOnlyDepthStencilView = null;
                }

                if (_shaderResourceView != null)
                {
                    _shaderResourceView.Dispose();
                    _shaderResourceView = null;
                }

                if (_ownsTexture && _nativeTexture != null)
                {
                    _nativeTexture.Dispose();
                    _nativeTexture = null;
                }

                if (_subBuffers != null)
                {
                    for (int i = 0; i < _subBuffers.Length; i++)
                    {
                        D3D11DepthStencilBufferWrapper subBuffer = _subBuffers[i];
                        subBuffer?.Dispose();
                    }

                    _subBuffers = null;
                }
            }

            base.Dispose(isDisposing);
        }
    }
}
