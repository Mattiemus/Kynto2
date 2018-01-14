namespace Spark.Direct3D11.Graphics
{
    using System;
    
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Enumerates supported Diret3D11 device creation flags.
    /// </summary>
    [Flags]
    public enum D3D11CreationFlags
    {
        /// <summary>
        /// None.
        /// </summary>
        None = D3D11.DeviceCreationFlags.None,

        /// <summary>
        /// Create device with the debug layer.
        /// </summary>
        Debug = D3D11.DeviceCreationFlags.Debug,

        /// <summary>
        /// Create device with BGRA support, for WPF and other Direct2D interop.
        /// </summary>
        BgraSupport = D3D11.DeviceCreationFlags.BgraSupport
    }
}
