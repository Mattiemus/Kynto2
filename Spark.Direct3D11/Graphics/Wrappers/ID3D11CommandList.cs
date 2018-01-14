namespace Spark.Direct3D11.Graphics
{
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Defines an object that holds a native D3D11 command list.
    /// </summary>
    public interface ID3D11CommandList
    {
        /// <summary>
        /// Gets the native D3D11 command list.
        /// </summary>
        D3D11.CommandList D3DCommandList { get; }
    }
}
