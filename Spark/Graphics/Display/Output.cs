namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Math;

    /// <summary>
    /// Description of an output of a graphics adapter, such as a monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Output : IEquatable<Output>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Output"/> struct.
        /// </summary>
        /// <param name="monitorHandle">Handle of the output</param>
        /// <param name="name">Name of the output</param>
        /// <param name="bounds">Output's bounds in desktop coordinates</param>
        public Output(IntPtr monitorHandle, string name, Rectangle bounds)
        {
            if (name == null)
            {
                name = string.Empty;
            }

            MonitorHandle = monitorHandle;
            Name = name;
            DesktopBounds = bounds;
        }

        /// <summary>
        /// Gets the handle to the output.
        /// </summary>
        public IntPtr MonitorHandle { get; }

        /// <summary>
        /// Gets the name of the output.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the output's bounds in desktop coordinate.
        /// </summary>
        public Rectangle DesktopBounds { get; }

        /// <summary>
        /// Tests equality between two outputs
        /// </summary>
        /// <param name="a">First output</param>
        /// <param name="b">Second output</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(Output a, Output b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two outputs
        /// </summary>
        /// <param name="a">First output</param>
        /// <param name="b">Second output</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(Output a, Output b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Tests equality betwen this output and another.
        /// </summary>
        /// <param name="other">Other output to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(Output other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality betwen this output and another.
        /// </summary>
        /// <param name="other">Other output to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref Output other)
        {
            return (MonitorHandle == other.MonitorHandle) && Name.Equals(other.Name) && DesktopBounds.Equals(other.DesktopBounds);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(Object obj)
        {
            if (obj is DisplayMode)
            {
                Output other = (Output)obj;
                return Equals(ref other);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return MonitorHandle.GetHashCode() + Name.GetHashCode() + DesktopBounds.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "MonitorHandle: {0}, Name: {1}, DesktopBounds: {2}", new object[] { MonitorHandle.ToString(), Name, DesktopBounds.ToString() });
        }
    }
}
