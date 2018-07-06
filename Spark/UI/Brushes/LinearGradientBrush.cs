namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;
    using Utilities;

    public sealed class LinearGradientBrush : GradientBrush
    {
        public LinearGradientBrush()
        {
        }

        public LinearGradientBrush(IRenderSystem renderSystem)
            : base(renderSystem)
        {
        }

        protected override Texture2D CreateTexture()
        {
            int width = 128;
            int height = 128;

            using (DataBuffer<Color> colorBuffer = new DataBuffer<Color>(GetTextureData(width, height)))
            {
                return new Texture2D(RenderSystem, width, height, SurfaceFormat.Color, colorBuffer);
            }
        }

        private Color GetPixel(int x, int y)
        {
            float deltaX = EndPoint.X - StartPoint.X;
            float deltaY = EndPoint.Y - StartPoint.Y;

            float denom = 1.0f / ((deltaX * deltaX) + (deltaY * deltaY));
            float t = (deltaX * (x - StartPoint.X) + deltaY * (y - StartPoint.Y)) * denom;

            return GetGradientColor(t / 128.0f);
        }

        private Color[] GetTextureData(int width, int height)
        {
            Color[] pixels = new Color[width * height];
            bool copyHorizontal = false;
            bool copyVertical = false;

            if (StartPoint.X == EndPoint.X)
            {
                copyVertical = true;
            }

            if (EndPoint.Y == StartPoint.Y)
            {
                copyHorizontal = true;
            }

            if (copyVertical)
            {
                Color lastValue = Color.Black;
                Color[] srcLine = null;

                for (var y = 0; y < height; y++)
                {
                    Color value = GetPixel(0, y);
                    if (srcLine == null || (lastValue != value))
                    {
                        if (srcLine == null)
                        {
                            srcLine = new Color[width];
                        }

                        srcLine.Fill(value);
                        lastValue = value;
                    }

                    Array.Copy(srcLine, 0, pixels, y * width, width);
                }
            }
            else if (copyHorizontal)
            {
                for (int x = 0; x < width; x++)
                {
                    Color value = GetPixel(x, 0);
                    for (var y = 0; y < height; y++)
                    {
                        pixels[y * width + x] = value;
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[y * width + x] = GetPixel(x, y);
                    }
                }
            }

            return pixels;
        }
    }
}
