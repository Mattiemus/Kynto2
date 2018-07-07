namespace Spark.UI
{
    using Graphics;
    using Math;

    public abstract class GradientBrush : Brush
    {
        private Vector2 _startPoint;
        private Vector2 _endPoint;

        protected GradientBrush()
        {
            GradientStops = new GradientStopCollection();
            GradientStops.BrushInvalidate = InvalidateTexture;

            _startPoint = new Vector2(0, 0);
            _endPoint = new Vector2(1, 1);
        }

        protected GradientBrush(IRenderSystem renderSystem)
            : base(renderSystem)
        {
            GradientStops = new GradientStopCollection();
            GradientStops.BrushInvalidate = InvalidateTexture;

            _startPoint = new Vector2(0, 0);
            _endPoint = new Vector2(1, 1);
        }

        public GradientStopCollection GradientStops { get; }

        public Vector2 StartPoint
        {
            get => _startPoint;
            set
            {
                _startPoint = value;
                InvalidateTexture();
            }
        }
        
        public Vector2 EndPoint
        {
            get => _endPoint;
            set
            {
                _endPoint = value;
                InvalidateTexture();
            }
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
