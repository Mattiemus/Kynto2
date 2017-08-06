namespace Spark.Graphics
{
    using System.Collections.Generic;

    using Core;
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
