namespace Spark.Direct3D11.Graphics
{
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Defines an object that holds a native D3D11 depth stencil state.
    /// </summary>
    public interface ID3D11DepthStencilState
    {
        /// <summary>
        /// Gets the native D3D11 depth stencil state.
        /// </summary>
        D3D11.DepthStencilState D3DDepthStencilState { get; }
    }
}
