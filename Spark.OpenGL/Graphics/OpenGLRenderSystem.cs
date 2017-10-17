namespace Spark.OpenGL.Graphics
{
    using System;
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;

    using Spark.Utilities;
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using Core;
    using Implementation;

    using OTK = OpenTK.Graphics;

    /// <summary>
    /// OpenGL render system implementation
    /// </summary>
    public sealed class OpenGLRenderSystem : BaseDisposable, IRenderSystem
    {
        private readonly ImplementationFactoryCollection _implementationFactories;
        private readonly PredefinedRenderStateProvider _prebuiltRenderStates;
        private int _currentEffectSortKey;
        private int _currentResourceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLRenderSystem"/> class
        /// </summary>
        public OpenGLRenderSystem()
        {
            CreateOpenGLContext();
            
            _implementationFactories = new ImplementationFactoryCollection();
            _currentEffectSortKey = 0;
            _currentResourceId = 0;

            Adapter = new OpenGLGraphicsAdapter();

            InitializeFactories();
            
            _prebuiltRenderStates = new PredefinedRenderStateProvider(this);
            
            OpenGLImmediateContext = new OpenGLRenderContext(this);
        }
        
        /// <summary>
        /// Gets the implementation of the immediate context
        /// </summary>
        internal OpenGLRenderContext OpenGLImmediateContext { get; }
        
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string Name => "OpenGL Render System";

        /// <summary>
        /// Gets the identifier that describes the render system platform.
        /// </summary>
        public string Platform => "OpenGL 4.5";

        /// <summary>
        /// Gets the immediate render context.
        /// </summary>
        public IRenderContext ImmediateContext => OpenGLImmediateContext;

        /// <summary>
        /// Gets the graphics adapter the render system was created with.
        /// </summary>
        public IGraphicsAdapter Adapter { get; }

        /// <summary>
        /// Gets if command lists are supported or not. If not, then creating deferred render contexts will fail.
        /// </summary>
        public bool AreCommandListsSupported
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the provider for predefined blend states.
        /// </summary>
        public IPredefinedBlendStateProvider PredefinedBlendStates => _prebuiltRenderStates;

        /// <summary>
        /// Gets the provider for predefined depthstencil states.
        /// </summary>
        public IPredefinedDepthStencilStateProvider PredefinedDepthStencilStates => _prebuiltRenderStates;

        /// <summary>
        /// Gets the provider for predefined rasterizer states.
        /// </summary>
        public IPredefinedRasterizerStateProvider PredefinedRasterizerStates => _prebuiltRenderStates;

        /// <summary>
        /// Gets the provider for predefined sampler states.
        /// </summary>
        public IPredefinedSamplerStateProvider PredefinedSamplerStates => _prebuiltRenderStates;

        /// <summary>
        /// Initializes the service. This is called by the engine when a service is newly registered.
        /// </summary>
        /// <param name="engine">Engine instance</param>
        public void Initialize(Engine engine)
        {
            // No-op
        }

        /// <summary>
        /// Creates a deferred render context. A deferred context is a thread-safe context that can be used to record graphics commands on a different
        /// thread other than the main rendering one.
        /// </summary>
        /// <returns>A newly created deferred render context.</returns>
        public IDeferredRenderContext CreateDeferredRenderContext()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds an implementation factory of the specified type
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <param name="implFactory">Implementation factory to add</param>
        /// <returns>True if the factory was added, false if it was not</returns>
        public bool AddImplementationFactory<T>(T implFactory) where T : IGraphicsResourceImplementationFactory
        {
            return _implementationFactories.AddImplementationFactory(implFactory);
        }

        /// <summary>
        /// Removes an implementation factory of the specified type
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <param name="implFactory">Implementation factory to remove</param>
        /// <returns>True if the factory was removed, false if it was not</returns>
        public bool RemoveImplementationFactory<T>(T implFactory) where T : IGraphicsResourceImplementationFactory
        {
            return _implementationFactories.RemoveImplementationFactory(implFactory);
        }

        /// <summary>
        /// Gets the implementation factory of the specified type.
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <returns>The registered implementation factory, if it exists. Otherwise, null is returned.</returns>
        public T GetImplementationFactory<T>() where T : IGraphicsResourceImplementationFactory
        {
            return _implementationFactories.GetImplementationFactory<T>();
        }

        /// <summary>
        /// Tries to get the implementation factory of the specified type.
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <param name="implementationFactory">The registered implementation factory, if it exists.</param>
        /// <returns>True if the factory was registered and found, false otherwise.</returns>
        public bool TryGetImplementationFactory<T>(out T implementationFactory) where T : IGraphicsResourceImplementationFactory
        {
            return _implementationFactories.TryGetImplementationFactory(out implementationFactory);
        }

        /// <summary>
        /// Queries if the graphics resource type (e.g. VertexBuffer) is supported by any of the registered implementation factories.
        /// </summary>
        /// <typeparam name="T">Graphics resource type</typeparam>
        /// <returns>True if the type is supported by an implementation factory, false otherwise.</returns>
        public bool IsSupported<T>() where T : GraphicsResource
        {
            return _implementationFactories.IsSupported<T>();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<IGraphicsResourceImplementationFactory> GetEnumerator()
        {
            return _implementationFactories.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _implementationFactories.GetEnumerator();
        }

        /// <summary>
        /// Gets the next unique resource id
        /// </summary>
        /// <returns>Unique resource id</returns>
        internal int GetNextUniqueResourceId()
        {
            return Interlocked.Increment(ref _currentResourceId);
        }

        /// <summary>
        /// Gets the next unique effect sorting key
        /// </summary>
        /// <returns>Unique effect sorting key</returns>
        internal int GetNextUniqueEffectSortKey()
        {
            return Interlocked.Increment(ref _currentEffectSortKey);
        }
        
        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            ImmediateContext.Dispose();

            base.Dispose(isDisposing);
        }

        public static OpenTK.NativeWindow window;
        public static OTK.GraphicsContext context;

        /// <summary>
        /// Creates the OpenGL context if it has not already been created
        /// </summary>
        private void CreateOpenGLContext()
        {
            if (OTK.GraphicsContext.CurrentContext == null)
            {
                window = new OpenTK.NativeWindow(1024, 768, "Test", OpenTK.GameWindowFlags.Default, OTK.GraphicsMode.Default, OpenTK.DisplayDevice.Default);
                
                context = new OTK.GraphicsContext(OTK.GraphicsMode.Default, window.WindowInfo, 3, 3, OTK.GraphicsContextFlags.Default);
                context.MakeCurrent(window.WindowInfo);
                context.LoadAll();
            }
        }

        /// <summary>
        /// Initializes the various graphics resource creation factories
        /// </summary>
        private void InitializeFactories()
        {
            // Render state objects
            new OpenGLBlendStateImplementationFactory(this).Initialize();
            new OpenGLDepthStencilStateImplementationFactory(this).Initialize();
            new OpenGLRasterizerStateImplementationFactory(this).Initialize();
            new OpenGLSamplerStateImplementationFactory(this).Initialize();

            // Buffer objects
            new OpenGLIndexBufferImplementationFactory(this).Initialize();
            new OpenGLVertexBufferImplementationFactory(this).Initialize();

            // Swap chain
            new OpenGLSwapChainImplementationFactory(this).Initialize();

            // Effects
            new OpenGLEffectImplementationFactory(this).Initialize();
        }
    }
}
