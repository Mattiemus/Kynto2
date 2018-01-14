namespace Spark.Direct3D11.Graphics
{
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Defines an object that holds a native D3D11 sampler state.
    /// </summary>
    public interface ID3D11SamplerState
    {
        /// <summary>
        /// Gets the native D3D11 sampler state.
        /// </summary>
        D3D11.SamplerState D3DSamplerState { get; }
    }
}
