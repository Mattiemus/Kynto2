namespace Spark.Direct3D11.Graphics
{
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Defines an object that holds a native D3D11 shader resource view.
    /// </summary>
    public interface ID3D11ShaderResourceView
    {
        /// <summary>
        /// Gets the native D3D11 shader resource view.
        /// </summary>
        D3D11.ShaderResourceView D3DShaderResourceView { get; }
    }
}
