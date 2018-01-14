namespace Spark.Direct3D11.Graphics
{
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Defines an object that holds a native D3D11 buffer.
    /// </summary>
    public interface ID3D11Buffer
    {
        /// <summary>
        /// Gets the native D3D11 buffer.
        /// </summary>
        D3D11.Buffer D3DBuffer { get; }
    }
}
