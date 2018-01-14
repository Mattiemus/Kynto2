namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="ISwapChainImplementation"/>.
    /// </summary>
    public sealed class D3D11SwapChainImplementationFactory : D3D11GraphicsResourceImplementationFactory, ISwapChainImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11SwapChainImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11SwapChainImplementationFactory(D3D11RenderSystem renderSystem)
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
            return new D3D11SwapChainImplementation(D3DRenderSystem, GetNextUniqueResourceId(), windowHandle, presentParams);
        }
    }
}
