namespace Spark.Math
{
    using System;
    
    public struct RectangleF : IEquatable<RectangleF>
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(Vector2 topLeft, Size size)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public RectangleF(Vector2 topLeft, float width, float height)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = width;
            Height = height;
        }
        
        public static RectangleF Empty => new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);
                
        /// <summary>
        /// Gets the center of the rectangle.
        /// </summary>
        public Vector2 Center => new Vector2(X + (Width / 2), Y + (Height / 2));

        /// <summary>
        /// Gets the top left point of the rectangle.
        /// </summary>
        public Vector2 TopLeftPoint => new Vector2(X, Y);

        /// <summary>
        /// Gets the top right point of the rectangle.
        /// </summary>
        public Vector2 TopRightPoint => new Vector2(X + Width, Y);

        /// <summary>
        /// Gets the bottom left point of the rectangle.
        /// </summary>
        public Vector2 BottomLeftPoint => new Vector2(X, Y + Height);

        /// <summary>
        /// Gets the bottom right point of the rectangle.
        /// </summary>
        public Vector2 BottomRightPoint => new Vector2(X + Width, Y + Height);

        /// <summary>
        /// Gets the size of the rectangle
        /// </summary>
        public Size Size => new Size(Width, Height);

        /// <summary>
        /// Gets the left-most X coordinate.
        /// </summary>
        public float Left => X;

        /// <summary>
        /// Gets the right-most X coordinate (Left + Width).
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        /// Gets the top-most Y coordinate.
        /// </summary>
        public float Top => Y;

        /// <summary>
        /// Gets the bottom-most Y coordinate (Top + Height).
        /// </summary>
        public float Bottom => Y + Height;

        /// <summary>
        /// Gets if the current rectangle is the empty rectangle, where the top left coordinate and width/height are all zero, and thus
        /// define a rectangle that has no area.
        /// </summary>
        public bool IsEmpty => MathHelper.IsApproxZero(X) && MathHelper.IsApproxZero(Y) && MathHelper.IsApproxZero(Width) && MathHelper.IsApproxZero(Height);

        /// <summary>
        /// Gets whether any of the components of the rectangle are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Width) || float.IsNaN(Height);

        /// <summary>
        /// Gets whether any of the components of the rectangle are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsNegativeInfinity(X) || float.IsPositiveInfinity(X) || float.IsNegativeInfinity(Y) || float.IsPositiveInfinity(Y) ||
                                  float.IsNegativeInfinity(Width) || float.IsPositiveInfinity(Width) || float.IsNegativeInfinity(Height) || float.IsNegativeInfinity(Height);


        public static RectangleF Inflate(RectangleF rect, float width, float height)
        {
            if (width < rect.Width * -2)
            {
                return Empty;
            }

            if (height < rect.Height * -2)
            {
                return Empty;
            }

            RectangleF result = rect;
            result.X -= width;
            result.Y -= height;
            result.Width += 2 * width;
            result.Height += 2 * height;

            return result;
        }

        public static RectangleF Inflate(RectangleF rect, Size size)
        {
            return Inflate(rect, size.Width, size.Height);
        }

        public void Inflate(float width, float height)
        {
            RectangleF result = this;
            Inflate(this, width, height);

            X = result.X;
            Y = result.Y;
            Width = result.Width;
            Height = result.Height;
        }

        public void Inflate(Size size)
        {
            Inflate(size.Width, size.Height);
        }

        /// <summary>
        /// Tests if the point is inside the rectangle.
        /// </summary>
        /// <param name="pt">Point to test</param>
        /// <returns>True if the point is contained by the rectangle, false otherwise.</returns>
        public bool Contains(Vector2 pt)
        {
            Contains(ref pt, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the point is inside the rectangle.
        /// </summary>
        /// <param name="pt">Point to test</param>
        /// <param name="result">True if the point is contained by the rectangle, false otherwise.</param>
        public void Contains(ref Vector2 pt, out bool result)
        {
            result = (X <= pt.X) && (pt.X < (X + Width)) && (Y <= pt.Y) && (pt.Y < (Y + Height));
        }

        /// <summary>
        /// Tests of the XY point is inside the rectangle.
        /// </summary>
        /// <param name="x">X coordinate of the point</param>
        /// <param name="y">Y coordinate of the point</param>
        /// <returns>True if the point is contained by the rectangle, false otherwise.</returns>
        public bool Contains(float x, float y)
        {
            return (X <= x) && (x < (X + Width)) && (Y <= y) && (y < (Y + Height));
        }

        /// <summary>
        /// Tests if the specified rectangle is contained inside the current rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to test</param>
        /// <returns>True if the rectangle is contained by the rectangle, false otherwise.</returns>
        public bool Contains(RectangleF rect)
        {
            Contains(ref rect, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the specified rectangle is contained inside the current rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to test</param>
        /// <param name="result">True if the rectangle is contained by the rectangle, false otherwise.</param>
        public void Contains(ref RectangleF rect, out bool result)
        {
            result = (X <= rect.X) && ((rect.X + rect.Width) <= (X + Width)) && (Y <= rect.Y) && ((rect.Y + rect.Height) <= Y + Height);
        }

        public override bool Equals(object o)
        {
            if (!(o is RectangleF))
            {
                return false;
            }

            return Equals((RectangleF)o);
        }

        public bool Equals(RectangleF other)
        {
            return MathHelper.IsApproxEquals(X, other.X) &&
                   MathHelper.IsApproxEquals(Y, other.Y) &&
                   MathHelper.IsApproxEquals(Width, other.Width) &&
                   MathHelper.IsApproxEquals(Height, other.Height);
        }

        public static RectangleF operator -(RectangleF lhs, Thickness rhs)
        {
            return new RectangleF(
                lhs.Left + rhs.Left,
                lhs.Top + rhs.Top,
                Math.Max(0.0f, lhs.Width - rhs.Left - rhs.Right),
                Math.Max(0.0f, lhs.Height - rhs.Top - rhs.Bottom));
        }
        
        public static bool operator ==(RectangleF lhs, RectangleF rhs)
        {
            return MathHelper.IsApproxEquals(lhs.X, rhs.X) &&
                   MathHelper.IsApproxEquals(lhs.Y, rhs.Y) &&
                   MathHelper.IsApproxEquals(lhs.Width, rhs.Width) &&
                   MathHelper.IsApproxEquals(lhs.Height, rhs.Height);
        }

        public static bool operator !=(RectangleF lhs, RectangleF rhs)
        {
            return !MathHelper.IsApproxEquals(lhs.X, rhs.X) ||
                   !MathHelper.IsApproxEquals(lhs.Y, rhs.Y) ||
                   !MathHelper.IsApproxEquals(lhs.Width, rhs.Width) ||
                   !MathHelper.IsApproxEquals(lhs.Height, rhs.Height);
        }
    }
}
