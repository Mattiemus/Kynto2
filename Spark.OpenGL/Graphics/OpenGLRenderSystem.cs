namespace Spark.Graphics
{
    using System.Collections;
    using System.Collections.Generic;

    using Core;
    using Utilities;
    using Implementation;
    
    /// <summary>
    /// OpenGL render system implementation
    /// </summary>
    public sealed class OpenGLRenderSystem : BaseDisposable, IRenderSystem
    {
        private readonly ImplementationFactoryCollection _implementationFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLRenderSystem"/> class
        /// </summary>
        public OpenGLRenderSystem()
        {
            _implementationFactories = new ImplementationFactoryCollection();
            InitializeFactories();

            OpenGLImmediateContext = new OpenGLRenderContext(this);
        }

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
        /// Gets the implementation of the immediate context
        /// </summary>
        internal OpenGLRenderContext OpenGLImmediateContext { get; }

        /// <summary>
        /// Initializes the service. This is called by the engine when a service is newly registered.
        /// </summary>
        /// <param name="engine">Engine instance</param>
        public void Initialize(Engine engine)
        {
            // No-op
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
        /// Performs the dispose action
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void DisposeInternal(bool isDisposing)
        {
            // No-op
        }

        /// <summary>
        /// Initializes the various graphics resource creation factories
        /// </summary>
        private void InitializeFactories()
        {
            new OpenGLVertexBufferImplementationFactory(this).Initialize();

            new OpenGLEffectImplementationFactory(this).Initialize();
        }
    }
}
