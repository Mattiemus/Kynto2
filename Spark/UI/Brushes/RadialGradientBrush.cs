namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;

    public sealed class RadialGradientBrush : GradientBrush
    {
        public RadialGradientBrush()
        {
            Radius = 1.0f;
            Center = new Vector2(0.5f, 0.5f);
            GradientOrigin = new Vector2(0.5f, 0.5f);
        }

        public RadialGradientBrush(IRenderSystem renderSystem)
            : base(renderSystem)
        {
            Radius = 1.0f;
            Center = new Vector2(0.5f, 0.5f);
            GradientOrigin = new Vector2(0.5f, 0.5f);
        }

        public float Radius { get; set; }

        public Vector2 Center { get; set; }

        public Vector2 GradientOrigin { get; set; }

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
            float fx = GradientOrigin.X - Center.X;
            float fy = GradientOrigin.Y - Center.Y;

            float radius2 = Radius * Radius;

            float denom = 1.0f / (radius2 - ((fx * fx) + (fy * fy)));

            float dx = x - (GradientOrigin.X * 128.0f);
            float dy = y - (GradientOrigin.Y * 128.0f);

            float dx2 = dx * dx;
            float dy2 = dy * dy;

            float grad = (radius2 * (dx2 + dy2)) - ((dx * fy - dy * fx) * (dx * fy - dy * fx));
            grad = ((dx * fx + dy * fy) + (float)Math.Sqrt(grad));
            grad *= denom;

            return GetGradientColor(grad / 128.0f);
        }

        private Color[] GetTextureData(int width, int height)
        {
            Color[] data = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    data[y * width + x] = GetPixel(x, y);
                }
            }

            return data;
        }
    }
}
