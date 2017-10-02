namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a 2D rectangle in a plane where the positive X axis is right and positive Y axis is down.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle : IEquatable<Rectangle>, IPrimitiveValue
    {
        /// <summary>
        /// Top left X coordinate of the rectangle.
        /// </summary>
        public int X;

        /// <summary>
        /// Top left Y coordinate of the rectangle.
        /// </summary>
        public int Y;

        /// <summary>
        /// Width of the rectangle.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public int Height;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate of the top left point of the rectangle.</param>
        /// <param name="y">The Y coordinate of the top left point of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="topLeft">The top left point of the rectangle</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rectangle(Int2 topLeft, int width, int height)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the empty rectangle, which is a rectangle that has no area.
        /// </summary>
        public static Rectangle Empty => new Rectangle();

        /// <summary>
        /// Gets the size of a rectangle type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Rectangle>();

        /// <summary>
        /// Gets the center of the rectangle.
        /// </summary>
        public Int2 Center => new Int2(X + (Width / 2), Y + (Height / 2));

        /// <summary>
        /// Gets the top left point of the rectangle.
        /// </summary>
        public Int2 TopLeftPoint => new Int2(X, Y);

        /// <summary>
        /// Gets the top right point of the rectangle.
        /// </summary>
        public Int2 TopRightPoint => new Int2(X + Width, Y);

        /// <summary>
        /// Gets the bottom left point of the rectangle.
        /// </summary>
        public Int2 BottomLeftPoint => new Int2(X, Y + Height);

        /// <summary>
        /// Gets the bottom right point of the rectangle.
        /// </summary>
        public Int2 BottomRightPoint => new Int2(X + Width, Y + Height);

        /// <summary>
        /// Gets the left-most X coordinate.
        /// </summary>
        public int Left => X;

        /// <summary>
        /// Gets the right-most X coordinate (Left + Width).
        /// </summary>
        public int Right => X + Width;

        /// <summary>
        /// Gets the top-most Y coordinate.
        /// </summary>
        public int Top => Y;

        /// <summary>
        /// Gets the bottom-most Y coordinate (Top + Height).
        /// </summary>
        public int Bottom => Y + Height;

        /// <summary>
        /// Gets if t he current rectangle is the empty rectangle, where the top left coordinate and width/height are all zero, and thus
        /// define a rectangle that has no area.
        /// </summary>
        public bool IsEmpty => X == 0 && Y == 0 && Width == 0 && Height == 0;

        /// <summary>
        /// Gets whether any of the components of the rectangle are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Width) || float.IsNaN(Height);

        /// <summary>
        /// Gets whether any of the components of the rectangle are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsNegativeInfinity(X) || float.IsPositiveInfinity(X) || float.IsNegativeInfinity(Y) || float.IsPositiveInfinity(Y) ||
                                  float.IsNegativeInfinity(Width) || float.IsPositiveInfinity(Width) || float.IsNegativeInfinity(Height) || float.IsNegativeInfinity(Height);

        /// <summary>
        /// Creates a rectangle that represents the intersection (overlap) of two rectangles. This may return an empty triangle.
        /// </summary>
        /// <param name="a">First rectangle.</param>
        /// <param name="b">Second rectangle.</param>
        /// <returns>The rectangle that represents the intersection.</returns>
        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            Intersect(ref a, ref b, out Rectangle result);
            return result;
        }

        /// <summary>
        /// Creates a rectangle that represents the intersection (overlap) of two rectangles. This may return an empty triangle.
        /// </summary>
        /// <param name="a">First rectangle.</param>
        /// <param name="b">Second rectangle.</param>
        /// <param name="result">The rectangle that represents the intersection.</param>
        public static void Intersect(ref Rectangle a, ref Rectangle b, out Rectangle result)
        {
            int x = (a.X > b.X) ? a.X : b.X;
            int y = (a.Y > b.Y) ? a.Y : b.Y;

            int right1 = a.Right;
            int right2 = b.Right;

            int bot1 = a.Bottom;
            int bot2 = b.Bottom;

            int right = (right1 < right2) ? right1 : right2;
            int bottom = (bot1 < bot2) ? bot1 : bot2;

            if ((right > x) && (bottom > y))
            {
                result = new Rectangle(x, y, right - x, bottom - y);
            }
            else
            {
                result = Empty;
            }
        }

        /// <summary>
        /// Creates a rectangle that is the union of the two rectangles.
        /// </summary>
        /// <param name="a">First rectangle.</param>
        /// <param name="b">Second rectangle.</param>
        /// <returns>The union of the two rectangles.</returns>
        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            Union(ref a, ref b, out Rectangle result);
            return result;
        }

        /// <summary>
        /// Creates a rectangle that is the union of the two rectangles.
        /// </summary>
        /// <param name="a">First rectangle.</param>
        /// <param name="b">Second rectangle.</param>
        /// <param name="result">The union of the two rectangles</param>
        public static void Union(ref Rectangle a, ref Rectangle b, out Rectangle result)
        {
            int x = (a.X < b.X) ? a.X : b.X;
            int y = (a.Y < b.Y) ? a.Y : b.Y;
            int right1 = a.Right;
            int right2 = b.Right;

            int bot1 = a.Height;
            int bot2 = b.Height;

            int right = (right1 > right2) ? right1 : right2;
            int bot = (bot1 > bot2) ? bot1 : bot2;

            result = new Rectangle(x, y, right - x, bot - y);
        }

        /// <summary>
        /// Translates the rectangle's top left location by the supplied amount.
        /// </summary>
        /// <param name="amt">Amount to translate along X,Y axis.</param>
        public void Translate(Int2 amt)
        {
            Translate(amt.X, amt.Y);
        }

        /// <summary>
        /// Translates the rectangle's top left location by the supplied
        /// amount.
        /// </summary>
        /// <param name="xAmt">Amount to translate along X axis.</param>
        /// <param name="yAmt">Amount to translate along Y axis.</param>
        public void Translate(int xAmt, int yAmt)
        {
            X += xAmt;
            Y += yAmt;
        }

        /// <summary>
        /// Scales the rectangle by the supplied amount.
        /// </summary>
        /// <param name="amt">Amount to scale along X, Y axis</param>
        public void Scale(Int2 amt)
        {
            Scale(amt.X, amt.Y);
        }

        /// <summary>
        /// Scales the rectangle by the supplied amount.
        /// </summary>
        /// <param name="xAmt">Amount to scale along X axis</param>
        /// <param name="yAmt">Amount to scale along Y axis</param>
        public void Scale(int xAmt, int yAmt)
        {
            X -= xAmt;
            Y -= yAmt;

            Width += xAmt * 2;
            Height += yAmt * 2;
        }

        /// <summary>
        /// Tests if the point is inside the rectangle.
        /// </summary>
        /// <param name="pt">Point to test</param>
        /// <returns>True if the point is contained by the rectangle, false otherwise.</returns>
        public bool Contains(Int2 pt)
        {
            Contains(ref pt, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the point is inside the rectangle.
        /// </summary>
        /// <param name="pt">Point to test</param>
        /// <param name="result">True if the point is contained by the rectangle, false otherwise.</param>
        public void Contains(ref Int2 pt, out bool result)
        {
            result = (X <= pt.X) && (pt.X < (X + Width)) && (Y <= pt.Y) && (pt.Y < (Y + Height));
        }

        /// <summary>
        /// Tests of the XY point is inside the rectangle.
        /// </summary>
        /// <param name="x">X coordinate of the point</param>
        /// <param name="y">Y coordinate of the point</param>
        /// <returns>True if the point is contained by the rectangle, false otherwise.</returns>
        public bool Contains(int x, int y)
        {
            return (X <= x) && (x < (X + Width)) && (Y <= y) && (y < (Y + Height));
        }

        /// <summary>
        /// Tests if the specified rectangle is contained inside the current rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to test</param>
        /// <returns>True if the rectangle is contained by the rectangle, false otherwise.</returns>
        public bool Contains(Rectangle rect)
        {
            Contains(ref rect, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the specified rectangle is contained inside the current rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to test</param>
        /// <param name="result">True if the rectangle is contained by the rectangle, false otherwise.</param>
        public void Contains(ref Rectangle rect, out bool result)
        {
            result = (X <= rect.X) && ((rect.X + rect.Width) <= (X + Width)) && (Y <= rect.Y) && ((rect.Y + rect.Height) <= Y + Height);
        }

        /// <summary>
        /// Tests if the specified rectangle intersects with the current rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to test</param>
        /// <returns>True if the rectangle intersects with the current rectangle, false otherwise.</returns>
        public bool Intersects(Rectangle rect)
        {
            Intersects(ref rect, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the specified rectangle intersects with the current rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to test.</param>
        /// <param name="result">True if the rectangle intersects with the current rectangle, false otherwise.</param>
        public void Intersects(ref Rectangle rect, out bool result)
        {
            result = (rect.X < (X + Width)) && (X < (rect.X + rect.Width)) && (rect.Y < (Y + Height)) && (Y < (rect.Y + rect.Height));
        }

        /// <summary>
        /// Tests equality between two rectangles.
        /// </summary>
        /// <param name="a">First rectangle</param>
        /// <param name="b">Second rectangle</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two rectangles.
        /// </summary>
        /// <param name="a">First rectangle</param>
        /// <param name="b">Second rectangle</param>
        /// <returns>True if not equal, false otherwise.</returns>
        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Rectangle)
            {
                return Equals((Rectangle)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the rectangle and another rectangle.
        /// </summary>
        /// <param name="other">Rectangle to test</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(Rectangle other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the rectangle and another rectangle.
        /// </summary>
        /// <param name="other">Rectangle to test</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(ref Rectangle other)
        {
            return (X == other.X) && (Y == other.Y) && (Width == other.Width) && (Height == other.Height);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() + Y.GetHashCode() + Width.GetHashCode() + Height.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "X:{0} Y:{1} Width:{2} Height:{3}", new object[] { X.ToString(info), Y.ToString(info), Width.ToString(info), Height.ToString(info) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("X", X);
            output.Write("Y", Y);
            output.Write("Width", Width);
            output.Write("Height", Height);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            X = input.ReadInt32();
            Y = input.ReadInt32();
            Width = input.ReadInt32();
            Height = input.ReadInt32();
        }
    }
}
