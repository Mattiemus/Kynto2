namespace Spark.Core.Interop
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Native methods for interacting with raw memory
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// Copies memory from one location to another
        /// </summary>
        /// <param name="pDest">Destination pointer</param>
        /// <param name="pSrc">Source pointer</param>
        /// <param name="count">Number of bytes</param>
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr pDest, IntPtr pSrc, [param: MarshalAs(UnmanagedType.SysUInt)] IntPtr count);

        /// <summary>
        /// Fills a block of memory with a single value
        /// </summary>
        /// <param name="pDest">Destination pointer</param>
        /// <param name="length">Number of bytes</param>
        /// <param name="value">Value to set</param>
        [DllImport("kernel32.dll", EntryPoint = "RtlFillMemory")]
        internal static extern void ClearMemory(IntPtr pDest, [param: MarshalAs(UnmanagedType.SysUInt)] IntPtr count, byte value);
    }
}
