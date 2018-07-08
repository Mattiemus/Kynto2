namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;
    using Utilities;

    public sealed class LinearGradientBrush : GradientBrush
    {
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + base.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is LinearGradientBrush linGradBrush)
            {
                return Equals(linGradBrush);
            }

            return false;
        }

        public override bool Equals(Brush other)
        {
            if (other is LinearGradientBrush linGradBrush)
            {
                return base.Equals(other);
            }

            return false;
        }

        protected override Texture2D CreateTexture(IRenderSystem renderSystem)
        {
            int side = 128;

            using (DataBuffer<Color> colorBuffer = new DataBuffer<Color>(GetTextureData(side)))
            {
                return new Texture2D(renderSystem, side, side, SurfaceFormat.Color, colorBuffer);
            }
        }

        private Color GetPixel(int x, int y, int side)
        {
            float deltaX = EndPoint.X - StartPoint.X;
            float deltaY = EndPoint.Y - StartPoint.Y;

            float denom = 1.0f / ((deltaX * deltaX) + (deltaY * deltaY));
            float t = (deltaX * (x - StartPoint.X) + deltaY * (y - StartPoint.Y)) * denom;

            return GetGradientColor(t / side);
        }

        private Color[] GetTextureData(int side)
        {
            Color[] pixels = new Color[side * side];
            bool copyHorizontal = false;
            bool copyVertical = false;

            if (MathHelper.IsApproxEquals(EndPoint.X, StartPoint.X))
            {
                copyVertical = true;
            }

            if (MathHelper.IsApproxEquals(EndPoint.Y, StartPoint.Y))
            {
                copyHorizontal = true;
            }

            if (copyVertical)
            {
                Color lastValue = Color.Black;
                Color[] srcLine = null;

                for (var y = 0; y < side; y++)
                {
                    Color value = GetPixel(0, y, side);
                    if (srcLine == null || (lastValue != value))
                    {
                        if (srcLine == null)
                        {
                            srcLine = new Color[side];
                        }

                        srcLine.Fill(value);
                        lastValue = value;
                    }

                    Array.Copy(srcLine, 0, pixels, y * side, side);
                }
            }
            else if (copyHorizontal)
            {
                for (int x = 0; x < side; x++)
                {
                    Color value = GetPixel(x, 0, side);
                    for (var y = 0; y < side; y++)
                    {
                        pixels[y * side + x] = value;
                    }
                }
            }
            else
            {
                for (int y = 0; y < side; y++)
                {
                    for (int x = 0; x < side; x++)
                    {
                        pixels[y * side + x] = GetPixel(x, y, side);
                    }
                }
            }

            return pixels;
        }
    }
}
