namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;

    public sealed class RadialGradientBrush : GradientBrush
    {
        private float _radius;
        private Vector2 _center;
        private Vector2 _gradientOrigin;

        public RadialGradientBrush()
        {
            _radius = 0.5f;
            _center = new Vector2(0.5f, 0.5f);
            _gradientOrigin = new Vector2(0.5f, 0.5f);
        }

        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                InvalidateTexture();
            }
        }

        public Vector2 Center
        {
            get => _center;
            set
            {
                _center = value;
                InvalidateTexture();
            }
        }

        public Vector2 GradientOrigin
        {
            get => _gradientOrigin;
            set
            {
                _gradientOrigin = value;
                InvalidateTexture();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Radius.GetHashCode();
                hash = (hash * 7) + Center.GetHashCode();
                hash = (hash * 7) + GradientOrigin.GetHashCode();                
                hash = (hash * 7) + base.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is RadialGradientBrush radGradBrush)
            {
                return Equals(radGradBrush);
            }

            return false;
        }

        public override bool Equals(Brush other)
        {
            if (other is RadialGradientBrush radGradBrush)
            {
                return MathHelper.IsApproxEquals(Radius, radGradBrush.Radius) &&
                       Center == radGradBrush.Center &&
                       GradientOrigin == radGradBrush.GradientOrigin &&
                       base.Equals(other);
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
            float fx = GradientOrigin.X - Center.X;
            float fy = GradientOrigin.Y - Center.Y;

            float radius2 = Radius * Radius;

            float denom = 1.0f / (radius2 - ((fx * fx) + (fy * fy)));

            float dx = x - (GradientOrigin.X * side);
            float dy = y - (GradientOrigin.Y * side);

            float dx2 = dx * dx;
            float dy2 = dy * dy;

            float grad = (radius2 * (dx2 + dy2)) - ((dx * fy - dy * fx) * (dx * fy - dy * fx));
            grad = ((dx * fx + dy * fy) + (float)Math.Sqrt(grad));
            grad *= denom;

            return GetGradientColor(grad / side);
        }

        private Color[] GetTextureData(int side)
        {
            Color[] data = new Color[side * side];

            for (int y = 0; y < side; y++)
            {
                for (int x = 0; x < side; x++)
                {
                    data[y * side + x] = GetPixel(x, y, side);
                }
            }

            return data;
        }
    }
}
