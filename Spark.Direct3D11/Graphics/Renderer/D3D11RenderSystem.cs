namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    using Spark.Direct3D11.Graphics.Implementation;
    using Spark.Utilities;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;
    using DXGI = SharpDX.DXGI;

    /// <summary>
    /// Direct3D11 render system implementation.
    /// </summary>
    public sealed class D3D11RenderSystem : Disposable, IRenderSystem
    {
        private readonly D3D11GraphicsAdapter _adapter;
        private D3D11RenderContext _immediateContext;
        private readonly ImplementationFactoryCollection _implFactories;
        private readonly PredefinedRenderStateProvider _prebuiltRenderStates;
        private readonly SamplerStateCache _samplerStateCache;

        private int _currentResourceId;
        private int _currentEffectSortKey;

        public D3D11RenderSystem()
            : this(null, D3D11CreationFlags.None, D3D11FeatureLevel.Level_11_0)
        {
        }

        public D3D11RenderSystem(D3D11CreationFlags creationFlags)
            : this(null, creationFlags, D3D11FeatureLevel.Level_11_0)
        {
        }

        public D3D11RenderSystem(D3D11CreationFlags creationFlags, D3D11FeatureLevel featureLevel)
            : this(null, creationFlags, featureLevel)
        {
        }

        public D3D11RenderSystem(D3D11GraphicsAdapter adapter)
            : this(adapter, D3D11CreationFlags.None, D3D11FeatureLevel.Level_11_0)
        {
        }

        public D3D11RenderSystem(D3D11GraphicsAdapter adapter, D3D11CreationFlags creationFlags)
            : this(adapter, creationFlags, D3D11FeatureLevel.Level_11_0)
        {
        }

        public D3D11RenderSystem(D3D11GraphicsAdapter adapter, D3D11CreationFlags creationFlags, D3D11FeatureLevel featureLevel)
        {
            DXGIFactory = new DXGI.Factory1();

            if (adapter == null)
            {
                adapter = new D3D11GraphicsAdapter(DXGIFactory.GetAdapter(0), 0);
            }

            try
            {
                D3DDevice = new D3D11.Device(DXGIFactory.GetAdapter(adapter.AdapterIndex), (D3D11.DeviceCreationFlags)creationFlags, (D3D.FeatureLevel)featureLevel);
                _adapter = adapter;
                _adapter.SetDevice(D3DDevice);
            }
            catch (Exception e)
            {
                Dispose(true);
                throw new SparkGraphicsException("Error creating graphics device", e);
            }

            _implFactories = new ImplementationFactoryCollection();
            
            AreCommandListsSupported = CheckCommandListSupport(D3DDevice);
            _currentResourceId = 0;
            _currentEffectSortKey = 0;

            InitializeFactories();

            GlobalInputLayoutManager = new InputLayoutManager(D3DDevice);
            _prebuiltRenderStates = new PredefinedRenderStateProvider(this);
            StandardEffects = new StandardEffectLibrary(this);
            StandardEffects.LoadProvider(new D3D11EffectByteCodeProvider());
            _samplerStateCache = new SamplerStateCache(this);

            _immediateContext = new D3D11RenderContext(this, adapter, D3DDevice.ImmediateContext, true);
        }

        /// <summary>
        /// Event for when the render system is in the process of being disposed, where all resources will get cleaned up.
        /// </summary>
        public event TypedEventHandler<IRenderSystem> Disposing;
        
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string Name => "Direct3D11_RenderSystem";

        /// <summary>
        /// Gets the identifier that describes the render system platform.
        /// </summary>
        public string Platform => "Direct3D11";

        /// <summary>
        /// Gets the immediate render context.
        /// </summary>
        public IRenderContext ImmediateContext => _immediateContext;

        /// <summary>
        /// Gets the graphics adapter the render system was created with.
        /// </summary>
        public IGraphicsAdapter Adapter => _adapter;

        /// <summary>
        /// Gets if command lists are supported or not. If not, then creating deferred render contexts will fail.
        /// </summary>
        public bool AreCommandListsSupported { get; }

        /// <summary>
        /// Gets the provider for prebuilt blend states.
        /// </summary>
        public IPredefinedBlendStateProvider PredefinedBlendStates => _prebuiltRenderStates;

        /// <summary>
        /// Gets the provider for prebuilt depthstencil states.
        /// </summary>
        public IPredefinedDepthStencilStateProvider PredefinedDepthStencilStates => _prebuiltRenderStates;

        /// <summary>
        /// Gets the provider for prebuilt rasterizer states.
        /// </summary>
        public IPredefinedRasterizerStateProvider PredefinedRasterizerStates => _prebuiltRenderStates;

        /// <summary>
        /// Gets the provider for prebuilt sampler states.
        /// </summary>
        public IPredefinedSamplerStateProvider PredefinedSamplerStates => _prebuiltRenderStates;

        /// <summary>
        /// Gets the standard effect library for the render system.
        /// </summary>
        public StandardEffectLibrary StandardEffects { get; }

        /// <summary>
        /// Gets the global cache of input layouts, shared between all render contexts.
        /// </summary>
        public InputLayoutManager GlobalInputLayoutManager { get; private set; }

        /// <summary>
        /// Gets the native D3D11 Device.
        /// </summary>
        public D3D11.Device D3DDevice { get; private set; }

        /// <summary>
        /// Gets the native DXGI Factory that created the D3D11 device.
        /// </summary>
        public DXGI.Factory DXGIFactory { get; private set; }

        /// <summary>
        /// Gets the native D3D11 immediate context.
        /// </summary>
        public D3D11.DeviceContext D3DImmediateContext => D3DDevice.ImmediateContext;

        /// <summary>
        /// Gets the sampler state cache used for the effects system.
        /// </summary>
        public SamplerStateCache SamplerCache => _samplerStateCache;

        public IDeferredRenderContext CreateDeferredRenderContext()
        {
            if (!AreCommandListsSupported)
            {
                throw new SparkGraphicsException("Deferred render context is not avaliable");
            }

            return new D3D11RenderContext(this, _adapter, new D3D11.DeviceContext(D3DDevice), false);
        }

        public T GetImplementationFactory<T>() where T : IGraphicsResourceImplementationFactory
        {
            return _implFactories.GetImplementationFactory<T>();
        }

        public bool TryGetImplementationFactory<T>(out T implementationFactory) where T : IGraphicsResourceImplementationFactory
        {
            return _implFactories.TryGetImplementationFactory<T>(out implementationFactory);
        }

        public bool IsSupported<T>() where T : GraphicsResource
        {
            return _implFactories.IsSupported<T>();
        }

        public void Initialize(SparkEngine engine)
        {
            // No-op
        }

        public IEnumerator<IGraphicsResourceImplementationFactory> GetEnumerator()
        {
            return _implFactories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _implFactories.GetEnumerator();
        }

        internal bool AddImplementationFactory<T>(T implFactory) where T : IGraphicsResourceImplementationFactory
        {
            return _implFactories.AddImplementationFactory(implFactory);
        }

        internal bool RemoveImplementationFactory<T>(T implFactory) where T : IGraphicsResourceImplementationFactory
        {
            return _implFactories.RemoveImplementationFactory(implFactory);
        }

        internal int GetNextUniqueResourceId()
        {
            return Interlocked.Increment(ref _currentResourceId);
        }

        internal int GetNextUniqueEffectSortKey()
        {
            return Interlocked.Increment(ref _currentEffectSortKey);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            Disposing?.Invoke(this, EventArgs.Empty);

            if (isDisposing)
            {
                if (_immediateContext != null)
                {
                    _immediateContext.Dispose();
                    _immediateContext = null;
                }

                if (D3DDevice != null)
                {
                    D3DDevice.Dispose();
                    D3DDevice = null;
                }

                if (DXGIFactory != null)
                {
                    DXGIFactory.Dispose();
                    DXGIFactory = null;
                }

                if (GlobalInputLayoutManager != null)
                {
                    GlobalInputLayoutManager.Dispose();
                    GlobalInputLayoutManager = null;
                }

                base.Dispose(isDisposing);
            }
        }

        private bool CheckCommandListSupport(D3D11.Device device)
        {
            device.CheckThreadingSupport(out bool concurrentResources, out bool commandLists);

            return commandLists;
        }

        private void InitializeFactories()
        {
            AddImplementationFactory<IBlendStateImplementationFactory>(new D3D11BlendStateImplementationFactory(this));
            AddImplementationFactory<IDepthStencilStateImplementationFactory>(new D3D11DepthStencilStateImplementationFactory(this));
            AddImplementationFactory<IRasterizerStateImplementationFactory>(new D3D11RasterizerStateImplementationFactory(this));
            AddImplementationFactory<ISamplerStateImplementationFactory>(new D3D11SamplerStateImplementationFactory(this));
            
            AddImplementationFactory<IIndexBufferImplementationFactory>(new D3D11IndexBufferImplementationFactory(this));
            AddImplementationFactory<IVertexBufferImplementationFactory>(new D3D11VertexBufferImplementationFactory(this));
            //new D3D11StreamOutputImplFactory().Initialize(this);

            AddImplementationFactory<ITexture1DImplementationFactory>(new D3D11Texture1DImplementationFactory(this));
            // TODO: Texutre1DArray
            AddImplementationFactory<ITexture2DImplementationFactory>(new D3D11Texture2DImplementationFactory(this));
            // TODO: Texutre2DArray
            // TODO: Texture3D
            // TODO: TextureCube

            AddImplementationFactory<IRenderTarget2DImplementationFactory>(new D3D11RenderTarget2DImplementationFactory(this));
            AddImplementationFactory<IRenderTarget2DArrayImplementationFactory>(new D3D11RenderTarget2DArrayImplementationFactory(this));
            AddImplementationFactory<IRenderTargetCubeImplementationFactory>(new D3D11RenderTargetCubeImplementationFactory(this));

            //new D3D11OcclusionQueryImplFactory().Initialize(this);
            AddImplementationFactory<ISwapChainImplementationFactory>(new D3D11SwapChainImplementationFactory(this));
            AddImplementationFactory<IEffectImplementationFactory>(new D3D11EffectImplementationFactory(this));
        }

        #region ImplementationFactoryCollection

        private sealed class ImplementationFactoryCollection : IEnumerable<IGraphicsResourceImplementationFactory>
        {
            private readonly Dictionary<Type, IGraphicsResourceImplementationFactory> _graphicResourceTypeToFactory;
            private readonly Dictionary<Type, IGraphicsResourceImplementationFactory> _factoryTypeToFactory;

            public ImplementationFactoryCollection()
            {
                _graphicResourceTypeToFactory = new Dictionary<Type, IGraphicsResourceImplementationFactory>();
                _factoryTypeToFactory = new Dictionary<Type, IGraphicsResourceImplementationFactory>();
            }

            public bool AddImplementationFactory<T>(T implFactory) where T : IGraphicsResourceImplementationFactory
            {
                if (implFactory == null || _graphicResourceTypeToFactory.ContainsKey(implFactory.GraphicsResourceType))
                {
                    return false;
                }

                _graphicResourceTypeToFactory.Add(implFactory.GraphicsResourceType, implFactory);
                _factoryTypeToFactory.Add(typeof(T), implFactory);

                return true;
            }

            public bool RemoveImplementationFactory<T>(T implFactory) where T : IGraphicsResourceImplementationFactory
            {
                if (implFactory == null || !_graphicResourceTypeToFactory.ContainsKey(implFactory.GraphicsResourceType))
                {
                    return false;
                }

                return _graphicResourceTypeToFactory.Remove(implFactory.GraphicsResourceType) && _factoryTypeToFactory.Remove(typeof(T));
            }

            public T GetImplementationFactory<T>() where T : IGraphicsResourceImplementationFactory
            {
                if (_factoryTypeToFactory.TryGetValue(typeof(T), out IGraphicsResourceImplementationFactory factory) && factory is T)
                {
                    return (T)factory;
                }

                return default(T);
            }

            public bool TryGetImplementationFactory<T>(out T implementationFactory) where T : IGraphicsResourceImplementationFactory
            {
                implementationFactory = default(T);
                
                if (_factoryTypeToFactory.TryGetValue(typeof(T), out IGraphicsResourceImplementationFactory factory) && factory is T)
                {
                    implementationFactory = (T)factory;
                    return true;
                }

                return false;
            }

            public bool IsSupported<T>() where T : GraphicsResource
            {
                return _graphicResourceTypeToFactory.ContainsKey(typeof(T));
            }

            public IEnumerator<IGraphicsResourceImplementationFactory> GetEnumerator()
            {
                return _factoryTypeToFactory.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _factoryTypeToFactory.Values.GetEnumerator();
            }
        }

        #endregion

        #region SamplerState caching

        public sealed class SamplerStateCache : Disposable
        {
            private readonly Dictionary<RenderStateKey, SamplerState> _cache;
            private readonly IRenderSystem _renderSystem;

            public SamplerStateCache(IRenderSystem renderSystem)
            {
                _cache = new Dictionary<RenderStateKey, SamplerState>();
                _renderSystem = renderSystem;

                // Set our default ones into the cache
                AddSamplerState(renderSystem.PredefinedSamplerStates.AnisotropicClamp);
                AddSamplerState(renderSystem.PredefinedSamplerStates.AnisotropicWrap);
                AddSamplerState(renderSystem.PredefinedSamplerStates.LinearClamp);
                AddSamplerState(renderSystem.PredefinedSamplerStates.LinearWrap);
                AddSamplerState(renderSystem.PredefinedSamplerStates.PointClamp);
                AddSamplerState(renderSystem.PredefinedSamplerStates.PointWrap);
            }

            public SamplerState GetOrCreateSamplerState(EffectData.SamplerStateData data)
            {
                RenderStateKey key = ComputeRenderStateKey(ref data);

                lock (_cache)
                {
                    if (!_cache.TryGetValue(key, out SamplerState ss))
                    {
                        ss = new SamplerState(_renderSystem)
                        {
                            AddressU = data.AddressU,
                            AddressV = data.AddressV,
                            AddressW = data.AddressW,
                            Filter = data.Filter,
                            MaxAnisotropy = data.MaxAnisotropy,
                            MinMipMapLevel = data.MinMipMapLevel,
                            MaxMipMapLevel = data.MaxMipMapLevel,
                            MipMapLevelOfDetailBias = data.MipMapLevelOfDetailBias,
                            BorderColor = data.BorderColor
                        };
                        ss.BindRenderState();

                        _cache.Add(key, ss);
                    }

                    return ss;
                }
            }

            private void AddSamplerState(SamplerState ss)
            {
                _cache.Add(ss.RenderStateKey, ss);
            }

            private RenderStateKey ComputeRenderStateKey(ref EffectData.SamplerStateData data)
            {
                // Taken from SamplerState...if that changes, this has to change!!!
                unchecked
                {
                    int hash = 17;

                    hash = (hash * 31) + RenderStateType.SamplerState.GetHashCode();
                    hash = (hash * 31) + data.AddressU.GetHashCode();
                    hash = (hash * 31) + data.AddressV.GetHashCode();
                    hash = (hash * 31) + data.AddressW.GetHashCode();
                    hash = (hash * 31) + data.Filter.GetHashCode();
                    hash = (hash * 31) + data.MaxAnisotropy;
                    hash = (hash * 31) + data.MinMipMapLevel;
                    hash = (hash * 31) + (data.MaxMipMapLevel % int.MaxValue);
                    hash = (hash * 31) + data.MipMapLevelOfDetailBias.GetHashCode();
                    hash = (hash * 31) + data.BorderColor.GetHashCode();

                    // Do not include ComparisonFunction into hash computation, we don't use it...yet?

                    return new RenderStateKey(RenderStateType.SamplerState, hash);
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
                    foreach (KeyValuePair<RenderStateKey, SamplerState> kv in _cache)
                    {
                        kv.Value.Dispose();
                    }

                    _cache.Clear();
                }

                base.Dispose(isDisposing);
            }
        }

        #endregion
    }
}
