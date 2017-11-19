namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    
    /// <summary>
    /// Factory for creating <see cref="OpenGLSwapChainImplementationFactory"/> instances
    /// </summary>
    public sealed class OpenGLSwapChainImplementationFactory : OpenGLGraphicsResourceImplementationFactory, ISwapChainImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLSwapChainImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLSwapChainImplementationFactory(OpenGLRenderSystem renderSystem)
            : base(renderSystem, typeof(SwapChain))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="windowHandle">The handle to the window the swap chain is bounded to and which the swap chain presents to.</param>
        /// <param name="presentParams">Presentation parameters defining how the swap chain should be setup.</param>
        /// <returns>The swap chain implementation.</returns>
        public ISwapChainImplementation CreateImplementation(IntPtr windowHandle, PresentationParameters presentParams)
        {
            return new OpenGLSwapChainImplementation(OpenGLRenderSystem, windowHandle, presentParams);
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<ISwapChainImplementationFactory>(this);
        }
    }
}
