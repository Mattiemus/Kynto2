namespace Spark.Direct3D11.Graphics
{
    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Defines an object that holds a native D3D11 depth stencil view and which can resolve the resource.
    /// </summary>
    public interface ID3D11DepthStencilView
    {
        /// <summary>
        /// Gets the native D3D11 depth stencil view.
        /// </summary>
        D3D11.DepthStencilView D3DDepthStencilView { get; }

        /// <summary>
        /// Gets the native read-only D3D11 depth stencil view (if not readable, then this should be null).
        /// </summary>
        D3D11.DepthStencilView D3DReadOnlyDepthStencilView { get; }

        /// <summary>
        /// Clears the depth stencil buffer.
        /// </summary>
        /// <param name="deviceContext">Device context</param>
        /// <param name="options">Clear options</param>
        /// <param name="depth">Depth to clear to.</param>
        /// <param name="stencil">Stencil to clear to</param>
        void Clear(D3D11.DeviceContext deviceContext, ClearOptions options, float depth, int stencil);
    }
}
