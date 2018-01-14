namespace Spark.Direct3D11.Graphics
{
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Defines an object that holds a native D3D11 blend state.
    /// </summary>
    public interface ID3D11BlendState
    {
        /// <summary>
        /// Gets the native D3D11 blend state.
        /// </summary>
        D3D11.BlendState D3DBlendState { get; }
    }
}
