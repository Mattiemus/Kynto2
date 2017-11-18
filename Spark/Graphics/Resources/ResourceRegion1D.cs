namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Region representing a section of a 1D resource.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ResourceRegion1D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRegion1D"/> struct.
        /// </summary>
        /// <param name="left">The left most position in the 1D resource at which to access (0 or greater).</param>
        /// <param name="right">The right most position in the 1D resource at which to access (Width or less).</param>
        public ResourceRegion1D(int left, int right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Left most position in the resource at which to access (0 or greater).
        /// </summary>
        public int Left;

        /// <summary>
        /// Right most position in the resource at which to access (Width of the resource or less).
        /// </summary>
        public int Right;

        /// <summary>
        /// Gets the width of the region.
        /// </summary>
        public int Width => Right - Left;

        /// <summary>
        /// Gets the number of texel elements encompassed in the region.
        /// </summary>
        public int ElementCount => Right - Left;

        /// <summary>
        /// Validates the resource region, ensuring that the region is in fact within bounds of the resource. Upon completion of the check, the resource
        /// dimensions will hold the dimensions of the subimage.
        /// </summary>
        /// <param name="width">Width of the resource, in texels. This will then hold the subimage width.</param>
        public void ValidateRegion(ref int width)
        {
            if (Left < 0 || Right <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Region left or right side is out of range");
            }

            if (width < 0 || Left > width || Right > width || Left == Right)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Resource region is invalid");
            }

            width = Right - Left;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Left: {0}, Right: {1} - Width: {2}", Left.ToString(), Right.ToString(), Width.ToString());
        }
    }
}
