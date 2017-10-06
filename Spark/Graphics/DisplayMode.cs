namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Description for a display mode, that provide details of a display that the device is rendering to.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayMode : IEquatable<DisplayMode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayMode"/> struct
        /// </summary>
        /// <param name="width">Width of the display in pixels</param>
        /// <param name="height">Height of the display in pixels</param>
        /// <param name="refreshRate">Refresh rate of the display</param>
        /// <param name="format">Format of the display</param>
        public DisplayMode(int width, int height, int refreshRate, SurfaceFormat format)
        {
            Width = width;
            Height = height;
            RefreshRate = refreshRate;
            SurfaceFormat = format;
        }

        /// <summary>
        /// Gets the display width in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the display height in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the aspect ratio (width / height).
        /// </summary>
        public float AspectRatio
        {
            get
            {
                if (Width != 0 && Height != 0)
                {
                    return Width / (float)Height;
                }

                return 0.0f;
            }
        }

        /// <summary>
        /// Gets the refresh rate of the display mode.
        /// </summary>
        public float RefreshRate { get; }

        /// <summary>
        /// Gets the surface format supported by this display mode.
        /// </summary>
        public SurfaceFormat SurfaceFormat { get; }

        /// <summary>
        /// Tests equality between two display modes.
        /// </summary>
        /// <param name="a">First display mode</param>
        /// <param name="b">Second display mode</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(DisplayMode a, DisplayMode b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Tests inequality between two display modes.
        /// </summary>
        /// <param name="a">First display mode</param>
        /// <param name="b">Second display mode</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(DisplayMode a, DisplayMode b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Tests equality betwen this display mode and another.
        /// </summary>
        /// <param name="other">Other display mode to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(DisplayMode other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality betwen this display mode and another.
        /// </summary>
        /// <param name="other">Other display mode to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref DisplayMode other)
        {
            return (Width == other.Width) && (Height == other.Height) && (RefreshRate == other.RefreshRate) && (SurfaceFormat == other.SurfaceFormat);
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
                DisplayMode other = (DisplayMode)obj;
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
                return Width.GetHashCode() + Height.GetHashCode() + RefreshRate.GetHashCode() + SurfaceFormat.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name. </returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "Width: {0}, Height: {1}, AspectRatio: {2}, RefreshRate: {3},  SurfaceFormat: {4}", new object[] { Width.ToString(), Height.ToString(), AspectRatio.ToString(), RefreshRate.ToString(), SurfaceFormat.ToString() });
        }
    }
}
