namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Math;
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D11 = SharpDX.Direct3D11;
    using SDX = SharpDX;
    using SDXM = SharpDX.Mathematics.Interop;
    using DXGI = SharpDX.DXGI;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="SwapChain"/>.
    /// </summary>
    public sealed class D3D11SwapChainImplementation : GraphicsResourceImplementation, ISwapChainImplementation, ID3D11SwapChain
    {
        private DXGI.SwapChain _nativeSwapChain;
        private D3D11.RenderTargetView _renderTargetView;
        private D3D11.DepthStencilView _depthStencilView;
        private D3D11.Texture2D _backBuffer;
        private IntPtr _windowHandle;
        private PresentationParameters _presentParams;
        private bool _resetting;

        internal D3D11SwapChainImplementation(D3D11RenderSystem renderSystem, int resourceID, IntPtr windowHandle, PresentationParameters presentParams)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;

            _windowHandle = windowHandle;
            _presentParams = presentParams;
            _resetting = false;

            CreateSwapChain(_windowHandle, _presentParams);
            CreateViews();

            Name = string.Empty;
        }

        /// <summary>
        /// Occurs when the swapchain views are destroyed during a resize or reset event.
        /// </summary>
        public event TypedEventHandler<ID3D11Backbuffer, EventArgs> OnResetResize;

        /// <summary>
        /// Gets the presentation parameters the swap chain is initialized to.
        /// </summary>
        public PresentationParameters PresentationParameters => _presentParams;

        /// <summary>
        /// Gets the current display mode of the swap chain.
        /// </summary>
        public DisplayMode CurrentDisplayMode
        {
            get
            {
                DXGI.ModeDescription modeDesc = _nativeSwapChain.Description.ModeDescription;
                return new DisplayMode(modeDesc.Width, modeDesc.Height, modeDesc.RefreshRate.Numerator, Direct3DHelper.FromD3DSurfaceFormat(modeDesc.Format));
            }
        }

        /// <summary>
        /// Gets the handle of the window the swap chain presents to.
        /// </summary>
        public IntPtr WindowHandle => _windowHandle;

        /// <summary>
        /// Gets the handle to the monitor that contains the majority of the output.
        /// </summary>
        public IntPtr MonitorHandle
        {
            get
            {
                using (DXGI.Output output = _nativeSwapChain.ContainingOutput)
                {
                    return output.Description.MonitorHandle;
                }
            }
        }

        /// <summary>
        /// Gets if the swap chain is in full screen or not. By default, swap chains are not in full screen mode.
        /// </summary>
        public bool IsFullScreen => _presentParams.IsFullScreen;

        /// <summary>
        /// Gets if the current display mode is in wide screen or not.
        /// </summary>
        public bool IsWideScreen => CurrentDisplayMode.AspectRatio > 1.6f;

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Gets the native DXGI device.
        /// </summary>
        public DXGI.SwapChain DXGISwapChain => _nativeSwapChain;

        /// <summary>
        /// Gets the native D3D11 texture that represents the backbuffer.
        /// </summary>
        public D3D11.Texture2D D3DTexture2D => _backBuffer;

        /// <summary>
        /// Gets the native D3D11 render target view.
        /// </summary>
        public D3D11.RenderTargetView D3DRenderTargetView => _renderTargetView;

        /// <summary>
        /// Gets the native D3D11 depth stencil view.
        /// </summary>
        public D3D11.DepthStencilView D3DDepthStencilView => _depthStencilView;

        /// <summary>
        /// Gets the native read-only D3D11 depth stencil view (if not readable, then this should be null).
        /// </summary>
        D3D11.DepthStencilView ID3D11DepthStencilView.D3DReadOnlyDepthStencilView => null;

        public void Clear(IRenderContext renderContext, ClearOptions clearOptions, Color color, float depth, int stencil)
        {
            D3D11.DeviceContext nativeContext = Direct3DHelper.GetD3DDeviceContext(renderContext);
            Clear(nativeContext, clearOptions, color, depth, stencil);
        }

        public void GetBackBufferData<T>(IDataBuffer<T> data, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            D3D11.DeviceContext nativeContext = D3DDevice.ImmediateContext;
            D3D11.Texture2D resource = _backBuffer;
            bool createdResolve = false;

            // If MSAA, need to first resolve it into a temporary texture before copying the data.
            if (_presentParams.MultiSampleCount > 1)
            {
                D3D11.Texture2DDescription desc = _backBuffer.Description;
                desc.SampleDescription.Count = 1;
                desc.SampleDescription.Quality = 0;
                desc.BindFlags = D3D11.BindFlags.None;
                desc.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                desc.Usage = D3D11.ResourceUsage.Default;

                resource = new D3D11.Texture2D(D3DDevice, desc);
                createdResolve = true;

                nativeContext.ResolveSubresource(_backBuffer, 0, resource, 0, desc.Format);
            }

            ResourceRegion3D? region;
            if (subimage.HasValue)
            {
                region = new ResourceRegion3D(subimage.Value);
            }
            else
            {
                region = new ResourceRegion3D(0, _presentParams.BackBufferWidth, 0, _presentParams.BackBufferHeight, 0, 1);
            }

            ResourceHelper.ReadTextureData(resource, nativeContext, _presentParams.BackBufferWidth, _presentParams.BackBufferHeight, 1, 1, 1, _presentParams.BackBufferFormat, ResourceUsage.Static, data, 0, 0, ref region, startIndex);

            if (createdResolve)
            {
                resource.Dispose();
            }
        }

        public void Present()
        {
            if (_resetting)
            {
                return;
            }

            _nativeSwapChain.Present((int)_presentParams.PresentInterval, DXGI.PresentFlags.None);
        }

        public void Reset(IntPtr windowHandle, PresentationParameters presentParams)
        {
            if (_resetting)
            {
                return;
            }

            _resetting = true;

            DestroyViews();
            DestroySwapChain();
            _windowHandle = windowHandle;
            _presentParams = presentParams;
            CreateSwapChain(_windowHandle, _presentParams);
            CreateViews();

            // Be sure to set the name to the new objects
            if (!string.IsNullOrEmpty(Name))
            {
                OnNameChange(Name);
            }

            OnResetResize?.Invoke(this, EventArgs.Empty);

            _resetting = false;
        }

        public void Resize(int width, int height)
        {
            if (_resetting || _presentParams.BackBufferWidth == width && _presentParams.BackBufferHeight == height)
            {
                return;
            }

            _presentParams.BackBufferWidth = width;
            _presentParams.BackBufferHeight = height;

            var flags = DXGI.SwapChainFlags.None;
            if (_presentParams.IsFullScreen)
            {
                flags = DXGI.SwapChainFlags.AllowModeSwitch;
            }

            DestroyViews();
            _nativeSwapChain.ResizeBuffers(1, width, height, Direct3DHelper.ToD3DSurfaceFormat(_presentParams.BackBufferFormat), flags);
            CreateViews();

            // Be sure to set the name to the new objects
            if (!string.IsNullOrEmpty(Name))
            {
                OnNameChange(Name);
            }

            OnResetResize?.Invoke(this, EventArgs.Empty);
        }

        public bool ToggleFullScreen()
        {
            _presentParams.IsFullScreen = !_presentParams.IsFullScreen;

            var modeDesc = new DXGI.ModeDescription
            {
                Format = Direct3DHelper.ToD3DSurfaceFormat(_presentParams.BackBufferFormat),
                Width = _presentParams.BackBufferWidth,
                Height = _presentParams.BackBufferHeight,
                RefreshRate = new DXGI.Rational(60, 1)
            };

            if (_presentParams.IsFullScreen)
            {
                // Set full screen mode
                _nativeSwapChain.ResizeTarget(ref modeDesc);

                using (DXGI.Output output = _nativeSwapChain.ContainingOutput)
                {
                    _nativeSwapChain.SetFullscreenState(true, output);
                }
            }
            else
            {
                // Set to windowed
                modeDesc.RefreshRate = new DXGI.Rational(0, 0);
                _nativeSwapChain.IsFullScreen = false;
                
                _nativeSwapChain.ResizeTarget(ref modeDesc);
            }

            return _presentParams.IsFullScreen;
        }

        public void Clear(D3D11.DeviceContext deviceContext, ClearOptions options, float depth, int stencil)
        {
            Clear(deviceContext, options, Color.Black, depth, stencil);
        }

        public void Clear(D3D11.DeviceContext deviceContext, ClearOptions options, Color color, float depth, int stencil)
        {
            if (_renderTargetView != null && ((options & ClearOptions.Target) == ClearOptions.Target))
            {
                Direct3DHelper.ConvertColor(ref color, out SDXM.RawColor4 sdxColor);
                deviceContext.ClearRenderTargetView(_renderTargetView, sdxColor);
            }

            bool clearDepth = (options & ClearOptions.Depth) == ClearOptions.Depth;
            bool clearStencil = (options & ClearOptions.Stencil) == ClearOptions.Stencil;

            if (_depthStencilView != null && (clearDepth || clearStencil))
            {
                D3D11.DepthStencilClearFlags clearFlags;

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
        }

        public void NotifyOnFirstBind()
        {
            //No-op
        }

        public void ResolveResource(D3D11.DeviceContext deviceContext)
        {
            //No-op
        }

        protected override void OnNameChange(string name)
        {
            if (_backBuffer != null)
            {
                _backBuffer.DebugName = name;
            }

            if (_renderTargetView != null)
            {
                _renderTargetView.DebugName = (string.IsNullOrEmpty(name)) ? name : name + "_RTV";
            }

            if (_depthStencilView != null)
            {
                _depthStencilView.DebugName = (string.IsNullOrEmpty(name)) ? name : name + "_DSV";
            }

            if (_nativeSwapChain != null)
            {
                _nativeSwapChain.DebugName = (string.IsNullOrEmpty(name)) ? name : name + "_Backbuffer";
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
                DestroyViews();
                DestroySwapChain();
            }

            base.Dispose(isDisposing);
        }

        private void CreateSwapChain(IntPtr windowHandle, PresentationParameters presentParams)
        {

            DXGI.SwapEffect swapEffect;
            DXGI.Format format = Direct3DHelper.ToD3DSurfaceFormat(presentParams.BackBufferFormat);
            DXGI.SampleDescription sampleDesc = new DXGI.SampleDescription(presentParams.MultiSampleCount, presentParams.MultiSampleQuality);
            bool multiSampleEnabled = sampleDesc.Count > 1;

            switch (presentParams.RenderTargetUsage)
            {
                case RenderTargetUsage.PreserveContents:
                    if (multiSampleEnabled)
                    {
                        swapEffect = DXGI.SwapEffect.Discard;
                    }
                    else
                    {
                        swapEffect = DXGI.SwapEffect.Sequential;
                    }
                    break;
                default:
                    swapEffect = DXGI.SwapEffect.Discard;
                    break;
            }

            var modeDesc = new DXGI.ModeDescription(presentParams.BackBufferWidth, presentParams.BackBufferHeight, new DXGI.Rational(60, 1), format);

            var swapDesc = new DXGI.SwapChainDescription
            {
                BufferCount = 1,
                Flags = DXGI.SwapChainFlags.AllowModeSwitch,
                IsWindowed = true,
                ModeDescription = modeDesc,
                OutputHandle = windowHandle,
                SampleDescription = sampleDesc,
                SwapEffect = swapEffect,
                Usage = DXGI.Usage.RenderTargetOutput
            };

            DXGI.Factory factory = (RenderSystem as D3D11RenderSystem).DXGIFactory;
            factory.MakeWindowAssociation(windowHandle, DXGI.WindowAssociationFlags.IgnoreAll | DXGI.WindowAssociationFlags.IgnoreAltEnter);
            _nativeSwapChain = new DXGI.SwapChain(factory, D3DDevice, swapDesc);

            if (_presentParams.IsFullScreen)
            {
                _nativeSwapChain.ResizeTarget(ref modeDesc);

                using (DXGI.Output output = _nativeSwapChain.ContainingOutput)
                {
                    _nativeSwapChain.SetFullscreenState(true, output);
                }

                _nativeSwapChain.ResizeBuffers(1, modeDesc.Width, modeDesc.Height, modeDesc.Format, DXGI.SwapChainFlags.AllowModeSwitch);
            }
        }

        private void CreateViews()
        {
            _backBuffer = D3D11.Resource.FromSwapChain<D3D11.Texture2D>(_nativeSwapChain, 0);

            D3D11.RenderTargetViewDescription rtvDesc = new D3D11.RenderTargetViewDescription();
            rtvDesc.Dimension = (_presentParams.MultiSampleCount > 1) ? D3D11.RenderTargetViewDimension.Texture2DMultisampled : D3D11.RenderTargetViewDimension.Texture2D;
            rtvDesc.Format = Direct3DHelper.ToD3DSurfaceFormat(_presentParams.BackBufferFormat);
            rtvDesc.Texture2D.MipSlice = 0;

            _renderTargetView = new D3D11.RenderTargetView(D3DDevice, _backBuffer, rtvDesc);

            if (_presentParams.DepthStencilFormat != DepthFormat.None)
            {
                var depthBufferDesc = new D3D11.Texture2DDescription
                {
                    ArraySize = 1,
                    BindFlags = D3D11.BindFlags.DepthStencil,
                    CpuAccessFlags = D3D11.CpuAccessFlags.None,
                    Format = Direct3DHelper.ToD3DDepthFormat(_presentParams.DepthStencilFormat),
                    Width = _presentParams.BackBufferWidth,
                    Height = _presentParams.BackBufferHeight,
                    MipLevels = 1,
                    OptionFlags = D3D11.ResourceOptionFlags.None,
                    SampleDescription = _backBuffer.Description.SampleDescription,
                    Usage = D3D11.ResourceUsage.Default
                };

                using (var depthBuffer = new D3D11.Texture2D(D3DDevice, depthBufferDesc))
                {
                    _depthStencilView = new D3D11.DepthStencilView(D3DDevice, depthBuffer);
                }
            }
        }

        private void DestroySwapChain()
        {
            if (_nativeSwapChain != null)
            {
                if (_presentParams.IsFullScreen)
                {
                    _nativeSwapChain.SetFullscreenState(false, null);
                }

                _nativeSwapChain.Dispose();
                _nativeSwapChain = null;
            }
        }

        private void DestroyViews()
        {
            if (_renderTargetView != null)
            {
                _renderTargetView.Dispose();
                _renderTargetView = null;
            }

            if (_depthStencilView != null)
            {
                _depthStencilView.Dispose();
                _depthStencilView = null;
            }

            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
                _backBuffer = null;
            }
        }
    }
}
