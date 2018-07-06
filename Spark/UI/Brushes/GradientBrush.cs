namespace Spark.UI
{
    using Graphics;
    using Math;

    public abstract class GradientBrush : Brush
    {
        private Texture2D _brushTexture;
        private BlendState _blendState;

        protected GradientBrush()
        {
            GradientStops = new GradientStopCollection();
            StartPoint = new Vector2(0, 0);
            EndPoint = new Vector2(1, 1);
        }

        protected GradientBrush(IRenderSystem renderSystem)
            : base(renderSystem)
        {
            GradientStops = new GradientStopCollection();
            StartPoint = new Vector2(0, 0);
            EndPoint = new Vector2(1, 1);
        }

        public GradientStopCollection GradientStops { get; }

        public Vector2 StartPoint { get; set; }

        public Vector2 EndPoint { get; set; }

        public override void Draw(IRenderContext context, SpriteBatch batch, Rectangle bounds, Matrix4x4 transform, float alpha)
        {
            if (_brushTexture == null)
            {
                _brushTexture = CreateTexture();
            }

            if (_blendState == null)
            {
                _blendState = ContainsAlpha() ? BlendState.AlphaBlendNonPremultiplied : BlendState.Opaque;
            }

            var state = _blendState;
            if (state == BlendState.Opaque && alpha < 1.0f)
            {
                state = BlendState.AlphaBlendNonPremultiplied;
            }

            batch.Begin(
                context,
                SpriteSortMode.Texture,
                state,
                RasterizerState.CullNone,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                transform);

            batch.Draw(_brushTexture, bounds, Color.White * alpha);

            batch.End();
        }

        protected abstract Texture2D CreateTexture();

        protected bool ContainsAlpha()
        {
            foreach (GradientStop s in GradientStops)
            {
                if (s.Color.A < 255)
                {
                    return true;
                }
            }

            return false;
        }

        protected Color GetGradientColor(float offset)
        {
            offset = MathHelper.Clamp(offset, 0.0f, 1.0f);

            var prevTimeMillis = GetPrevOffset(offset);
            var nextTimeMillis = GetNextOffset(offset);

            var prevValue = GetPrevColor(offset);
            var nextValue = GetNextColor(offset);

            if (offset - prevTimeMillis <= 0.0)
            {
                return prevValue;
            }

            if ((offset - prevTimeMillis) >= nextTimeMillis)
            {
                return nextValue;
            }

            float v = 1.0f / ((nextTimeMillis - prevTimeMillis) / (offset - prevTimeMillis));

            return Color.Lerp(prevValue, nextValue, v);
        }

        private Color GetPrevColor(float offset)
        {
            GradientStop frame = null;
            foreach (GradientStop f in GradientStops)
            {
                if (f.Offset <= offset && (frame == null || f.Offset > frame.Offset))
                {
                    frame = f;
                }
            }

            if (frame != null)
            {
                return frame.Color;
            }

            return Color.TransparentBlack;
        }

        private Color GetNextColor(float offset)
        {
            GradientStop frame = null;
            foreach (GradientStop f in GradientStops)
            {
                if (f.Offset > offset && (frame == null || f.Offset < frame.Offset))
                {
                    frame = f;
                }
            }

            if (frame != null)
            {
                return frame.Color;
            }

            return Color.TransparentBlack;
        }

        private float GetPrevOffset(float offset)
        {
            GradientStop frame = null;
            foreach (GradientStop f in GradientStops)
            {
                if (f.Offset <= offset && (frame == null || f.Offset > frame.Offset))
                {
                    frame = f;
                }
            }

            if (frame != null)
            {
                return frame.Offset;
            }

            return float.NaN;
        }

        private float GetNextOffset(float offset)
        {
            GradientStop frame = null;
            foreach (GradientStop f in GradientStops)
            {
                if (f.Offset > offset && (frame == null || f.Offset < frame.Offset))
                {
                    frame = f;
                }
            }

            if (frame != null)
            {
                return frame.Offset;
            }

            return float.NaN;
        }
    }
}
