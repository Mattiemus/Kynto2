namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Geometry;
    using Spark.Graphics.Implementation;

    using Core;

    /// <summary>
    /// Factory for creating <see cref="OpenGLIndexBufferImplementation"/> instances
    /// </summary>
    public sealed class OpenGLIndexBufferImplementationFactory : OpenGLGraphicsResourceImplementationFactory, IIndexBufferImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLIndexBufferImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLIndexBufferImplementationFactory(OpenGLRenderSystem renderSystem)
            : base(renderSystem, typeof(IndexBuffer))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="format">The index format.</param>
        /// <param name="indexCount">Number of indices the buffer will contain.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <returns>The buffer implementation.</returns>
        public IIndexBufferImplementation CreateImplementation(IndexFormat format, int indexCount, ResourceUsage resourceUsage)
        {
            return new OpenGLIndexBufferImplementation(OpenGLRenderSystem, format, indexCount, resourceUsage);
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="format">The index format.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">Index data to initialize the buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        public IIndexBufferImplementation CreateImplementation(IndexFormat format, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
        {
            return new OpenGLIndexBufferImplementation(OpenGLRenderSystem, format, resourceUsage, data);
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the buffer should use.</param>
        /// <param name="data">The index data to initialize the buffer with.</param>
        /// <returns>The buffer implementation.</returns>
        public IIndexBufferImplementation CreateImplementation(ResourceUsage resourceUsage, IndexData data)
        {
            return new OpenGLIndexBufferImplementation(OpenGLRenderSystem, resourceUsage, data);
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<IIndexBufferImplementationFactory>(this);
        }
    }
}
