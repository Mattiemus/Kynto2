namespace Spark.Direct3D11.Graphics
{
    using DXGI = SharpDX.DXGI;

    /// <summary>
    /// Defines an object that holds a native DXGI Swapchain.
    /// </summary>
    public interface ID3D11SwapChain : ID3D11Backbuffer
    {
        /// <summary>
        /// Gets the native DXGI swap chain.
        /// </summary>
        DXGI.SwapChain DXGISwapChain { get; }
    }
}
