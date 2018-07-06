namespace Spark.UI
{
    using Graphics;
    using Math;
    using Utilities;

    public sealed class SolidColorBrush : Brush
    {
        private Texture2D _brushTexture;

        public SolidColorBrush()
        {
        }

        public SolidColorBrush(IRenderSystem renderSystem)
            : base(renderSystem)
        {
        }

        public SolidColorBrush(IRenderSystem renderSystem, Color color)
            : base(renderSystem)
        {
            Color = color;
        }

        public SolidColorBrush(Color color)
        {
            Color = color;
        }

        public Color Color { get; set; }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            SolidColorBrush solidColorBrush = obj as SolidColorBrush;
            if (solidColorBrush != null)
            {
                return solidColorBrush.Color == Color;
            }

            return false;
        }

        public override void Draw(IRenderContext context, SpriteBatch batch, Rectangle bounds, Matrix4x4 transform, float alpha)
        {
            if (_brushTexture == null)
            {
                _brushTexture = CreateTexture();
            }

            batch.Begin(
                context,
                SpriteSortMode.Texture,
                BlendState.AlphaBlendNonPremultiplied,
                RasterizerState.CullNone,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                transform);

            batch.Draw(_brushTexture, bounds, Color.White * alpha);

            batch.End();
        }

        private Texture2D CreateTexture()
        {
            using (DataBuffer<Color> colorBuffer = new DataBuffer<Color>(Color))
            {
                return new Texture2D(RenderSystem, 1, 1, SurfaceFormat.Color, colorBuffer);
            }
        }
    }
}
