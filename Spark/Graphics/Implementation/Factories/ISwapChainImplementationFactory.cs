namespace Spark.Graphics.Implementation
{
    using System;

    /// <summary>
    /// Defines a factory that creates platform-specific implementation of type <see cref="ISwapChainImplementation"/>.
    /// </summary>
    public interface ISwapChainImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="windowHandle">The handle to the window the swap chain is bounded to and which the swap chain presents to.</param>
        /// <param name="presentParams">Presentation parameters defining how the swap chain should be setup.</param>
        /// <returns>The swap chain implementation.</returns>
        ISwapChainImplementation CreateImplementation(IntPtr windowHandle, PresentationParameters presentParams);
    }
}
