namespace Spark.Graphics
{
    using System.Collections.Generic;
    
    using Implementation;

    /// <summary>
    /// Defines the render system service manager, which is responsible for managing and creating all graphics objects. A render system can create render contexts and supports
    /// factories to create individual graphic objects such as buffers, textures, and shaders. Graphic resource creation is always guaranteed to be thread safe, either through
    /// concurrent creation or through locking schemes.
    /// </summary>
    public interface IRenderSystem : IDisposableEngineService, IEnumerable<IGraphicsResourceImplementationFactory>
    {
        /// <summary>
        /// Gets the identifier that describes the render system platform.
        /// </summary>
        string Platform { get; }

        /// <summary>
        /// Gets the immediate render context.
        /// </summary>
        IRenderContext ImmediateContext { get; }

        /// <summary>
        /// Gets the graphics adapter the render system was created with.
        /// </summary>
        IGraphicsAdapter Adapter { get; }

        /// <summary>
        /// Gets if command lists are supported or not. If not, then creating deferred render contexts will fail.
        /// </summary>
        bool AreCommandListsSupported { get; }

        /// <summary>
        /// Gets the provider for predefined blend states.
        /// </summary>
        IPredefinedBlendStateProvider PredefinedBlendStates { get; }

        /// <summary>
        /// Gets the provider for predefined depthstencil states.
        /// </summary>
        IPredefinedDepthStencilStateProvider PredefinedDepthStencilStates { get; }

        /// <summary>
        /// Gets the provider for predefined rasterizer states.
        /// </summary>
        IPredefinedRasterizerStateProvider PredefinedRasterizerStates { get; }

        /// <summary>
        /// Gets the provider for predefined sampler states.
        /// </summary>
        IPredefinedSamplerStateProvider PredefinedSamplerStates { get; }

        /// <summary>
        /// Gets the standard effect library for the render system.
        /// </summary>
        StandardEffectLibrary StandardEffects { get; }

        /// <summary>
        /// Creates a deferred render context. A deferred context is a thread-safe context that can be used to record graphics commands on a different
        /// thread other than the main rendering one.
        /// </summary>
        /// <returns>A newly created deferred render context.</returns>
        IDeferredRenderContext CreateDeferredRenderContext();

        /// <summary>
        /// Gets the implementation factory of the specified type.
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <returns>The registered implementation factory, if it exists. Otherwise, null is returned.</returns>
        T GetImplementationFactory<T>() where T : IGraphicsResourceImplementationFactory;

        /// <summary>
        /// Tries to get the implementation factory of the specified type.
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <param name="implementationFactory">The registered implementation factory, if it exists.</param>
        /// <returns>True if the factory was registered and found, false otherwise.</returns>
        bool TryGetImplementationFactory<T>(out T implementationFactory) where T : IGraphicsResourceImplementationFactory;

        /// <summary>
        /// Queries if the graphics resource type (e.g. VertexBuffer) is supported by any of the registered implementation factories.
        /// </summary>
        /// <typeparam name="T">Graphics resource type</typeparam>
        /// <returns>True if the type is supported by an implementation factory, false otherwise.</returns>
        bool IsSupported<T>() where T : GraphicsResource;
    }
}
