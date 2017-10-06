namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Math;

    /// <summary>
    /// Region representing a section of a 2D resource.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ResourceRegion2D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRegion2D"/> struct.
        /// </summary>
        /// <param name="left">The left most position in the 2D resource at which to access (0 or greater).</param>
        /// <param name="right">The right most position in the 2D resource at which to access (Width or less).</param>
        /// <param name="top">The top most position in the 2D resource at which to access (0 or greater).</param>
        /// <param name="bottom">The bottom most position in the 2D resource at which to access (Height or less).</param>
        public ResourceRegion2D(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRegion2D"/> struct.
        /// </summary>
        /// <param name="region">One dimensional region to populate from.</param>
        public ResourceRegion2D(ResourceRegion1D region)
        {
            Left = region.Left;
            Right = region.Right;
            Top = 0;
            Bottom = 1;
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
        /// Top most position in the resource at which to access (0 or greater).
        /// </summary>
        public int Top;

        /// <summary>
        /// Bottom most position in the resource at which to to access (Height of the resource or less).
        /// </summary>
        public int Bottom;

        /// <summary>
        /// Gets the width of the region.
        /// </summary>
        public int Width => Right - Left;

        /// <summary>
        /// Gets the height of the region.
        /// </summary>
        public int Height => Bottom - Top;

        /// <summary>
        /// Gets the number of texel elements encompassed in the region.
        /// </summary>
        public int ElementCount => (Right - Left) * (Bottom - Top);

        /// <summary>
        /// Implicitly converts the rectangle to the resource region.
        /// </summary>
        /// <param name="rectangle">Rectangle to convert</param>
        /// <returns>The resource region</returns>
        public static implicit operator ResourceRegion2D(Rectangle rectangle)
        {
            return new ResourceRegion2D(rectangle.X, rectangle.Width, rectangle.Y, rectangle.Height);
        }

        /// <summary>
        /// Implicitly converts the resource region to a rectangle.
        /// </summary>
        /// <param name="region">Resource region to convert</param>
        /// <returns>The rectangle</returns>
        public static implicit operator Rectangle(ResourceRegion2D region)
        {
            return new Rectangle(region.Left, region.Top, region.Right, region.Bottom);
        }

        /// <summary>
        /// Validates the resource region, ensuring that the region is in fact within bounds of the resource. Upon completion of the check, the resource
        /// dimensions will hold the dimensions of the subimage.
        /// </summary>
        /// <param name="width">Width of the resource, in texels. This will then hold the subimage width.</param>
        /// <param name="height">Height of the resource, in texels. This will then hold the subimage height.</param>
        public void ValidateRegion(ref int width, ref int height)
        {
            if (Left < 0 || Right <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Region left or right side is out of range");
            }

            if (Top < 0 || Bottom <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Region top or bottom side is out of range");
            }

            if (width < 0 || height < 0 || Left > width || Right > width || Left == Right || Top > height || Bottom > height || Top == Bottom)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Resource region is invalid");
            }

            width = Right - Left;
            height = Bottom - Top;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Left: {0}, Right: {1}, Top: {2}, Bottom: {3} - Width: {4}, Height: {5}", Left.ToString(), Right.ToString(), Top.ToString(), Bottom.ToString(), Width.ToString(), Height.ToString());
        }
    }
}
