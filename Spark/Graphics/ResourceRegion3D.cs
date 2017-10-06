namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    /// <summary>
    /// Region representing a section of a 3D texture resource.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ResourceRegion3D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRegion3D"/> struct.
        /// </summary>
        /// <param name="left">The left most position in the 3D resource at which to access (0 or greater).</param>
        /// <param name="right">The right most position in the 3D resource at which to access (Width or less).</param>
        /// <param name="top">The top most position in the 3D resource at which to access (0 or greater).</param>
        /// <param name="bottom">The bottom most position in the 2D resource at which to access (Height or less).</param>
        /// <param name="front">The front most position in the 3D resource at which to access (0 or greater).</param>
        /// <param name="back">The back most position in the 3D resource at which to access (Depth or less).</param>
        public ResourceRegion3D(int left, int right, int top, int bottom, int front, int back)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
            Front = front;
            Back = back;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRegion3D"/> struct.
        /// </summary>
        /// <param name="region">One dimensional region to populate from.</param>
        public ResourceRegion3D(ResourceRegion1D region)
        {
            Left = region.Left;
            Right = region.Right;
            Top = 0;
            Bottom = 1;
            Front = 0;
            Back = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRegion3D"/> struct.
        /// </summary>
        /// <param name="region">Two dimensional region to populate from.</param>
        public ResourceRegion3D(ResourceRegion2D region)
        {
            Left = region.Left;
            Right = region.Right;
            Top = region.Top;
            Bottom = region.Bottom;
            Front = 0;
            Back = 1;
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
        /// Front most position in the resource at which to access (0 or greater).
        /// </summary>
        public int Front;

        /// <summary>
        /// Back most position in the resource at which to access (Depth or less).
        /// </summary>
        public int Back;

        /// <summary>
        /// Gets the width of the region.
        /// </summary>
        public int Width => Right - Left;

        /// <summary>
        /// Gets the height of the region.
        /// </summary>
        public int Height => Bottom - Top;

        /// <summary>
        /// Gets the depth of the region.
        /// </summary>
        public int Depth => Back - Front;

        /// <summary>
        /// Gets the number of texel elements encompassed in the region.
        /// </summary>
        public int ElementCount => (Right - Left) * (Bottom - Top) * (Back - Front);

        /// <summary>
        /// Validates the resource region, ensuring that the region is in fact within bounds of the resource. Upon completion of the check, the resource
        /// dimensions will hold the dimensions of the subimage.
        /// </summary>
        /// <param name="width">Width of the resource, in texels. This will then hold the subimage width.</param>
        /// <param name="height">Height of the resource, in texels. This will then hold the subimage height.</param>
        /// <param name="depth">Depth of the resource, in texels. This will then hold the subimage depth.</param>
        public void ValidateRegion(ref int width, ref int height, ref int depth)
        {
            if (Left < 0 || Right <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Region left or right side is out of range");
            }

            if (Top < 0 || Bottom <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Region top or bottom side is out of range");
            }

            if (Front < 0 || Back <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Region front or back side is out of range");
            }

            if (width < 0 || height < 0 || depth < 0 || Left > width || Right > width || Left == Right || Top > height || Bottom > height || Top == Bottom || Front > depth || Back > depth || Front == Back)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Resource region is invalid");
            }

            width = Right - Left;
            height = Bottom - Top;
            depth = Back - Front;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Left: {0}, Right: {1}, Top: {2}, Bottom: {3}, Front: {4}, Back {5} - Width: {6}, Height: {7}, Depth: {8}", Left.ToString(), Right.ToString(), Top.ToString(), Bottom.ToString(), Front.ToString(), Back.ToString(), Width.ToString(), Height.ToString(), Depth.ToString());
        }
    }
}
