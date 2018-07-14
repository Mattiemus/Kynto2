namespace Spark.UI.Shapes
{
    using System;

    using Media;
    using Math;

    public abstract class Shape : FrameworkElement
    {
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                nameof(Fill),
                typeof(Brush),
                typeof(Shape),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                nameof(Stretch),
                typeof(Stretch),
                typeof(Shape),
                new FrameworkPropertyMetadata(
                    Stretch.None,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                nameof(Stroke),
                typeof(Brush),
                typeof(Shape),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                nameof(StrokeThickness),
                typeof(float),
                typeof(Shape),
                new FrameworkPropertyMetadata(
                    1.0f,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public virtual Geometry RenderedGeometry => DefiningGeometry;

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public float StrokeThickness
        {
            get => (float)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        protected abstract Geometry DefiningGeometry { get; }

        protected internal override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = (Stroke != null) ? new Pen(Stroke, StrokeThickness) : null;
            RectangleF shapeBounds = RenderedGeometry.GetRenderBounds(pen);
            Matrix4x4 matrix = Matrix4x4.Identity;

            if (Stretch != Stretch.None)
            {
                float scaleX = ActualWidth / shapeBounds.Width;
                float scaleY = ActualHeight / shapeBounds.Height;
                switch (Stretch)
                {
                    case Stretch.Uniform:
                        scaleX = scaleY = Math.Min(scaleX, scaleY);
                        break;

                    case Stretch.UniformToFill:
                        // Hmm, in WPF appears to be the same as Uniform. This can't be right...
                        scaleX = scaleY = Math.Min(scaleX, scaleY);
                        break;
                }

                matrix *= Matrix4x4.FromTranslation(-shapeBounds.X, -shapeBounds.Y, 0.0f);
                matrix *= Matrix4x4.FromScale(scaleX, scaleY, 1.0f);
                matrix *= Matrix4x4.FromTranslation(
                    (ActualWidth - (shapeBounds.Width * scaleX)) / 2.0f,
                    (ActualHeight - (shapeBounds.Height * scaleY)) / 2.0f,
                    0.0f);
            }

            drawingContext.DrawGeometry(Fill, pen, RenderedGeometry, matrix);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Pen pen = (Stroke != null) ? new Pen(Stroke, StrokeThickness) : null;
            RectangleF shapeBounds = RenderedGeometry.GetRenderBounds(pen);
            Size desired = constraint;
            float sx = 0.0f;
            float sy = 0.0f;

            if (Stretch == Stretch.None)
            {
                return new Size(
                    shapeBounds.X + shapeBounds.Width,
                    shapeBounds.Y + shapeBounds.Height);
            }

            if (float.IsInfinity(constraint.Width))
            {
                desired.Width = shapeBounds.Width;
            }

            if (float.IsInfinity(constraint.Height))
            {
                desired.Height = shapeBounds.Height;
            }

            if (shapeBounds.Width > 0.0f)
            {
                sx = desired.Width / shapeBounds.Width;
            }

            if (shapeBounds.Height > 0.0f)
            {
                sy = desired.Height / shapeBounds.Height;
            }

            if (float.IsInfinity(constraint.Width))
            {
                sx = sy;
            }

            if (float.IsInfinity(constraint.Height))
            {
                sy = sx;
            }

            switch (Stretch)
            {
                case Stretch.Uniform:
                    sx = sy = Math.Min(sx, sy);
                    break;

                case Stretch.UniformToFill:
                    sx = sy = Math.Max(sx, sy);
                    break;

                case Stretch.Fill:
                    if (float.IsInfinity(constraint.Width))
                    {
                        sx = 1.0f;
                    }

                    if (float.IsInfinity(constraint.Height))
                    {
                        sy = 1.0f;
                    }
                    break;
            }

            return new Size(shapeBounds.Width * sx, shapeBounds.Height * sy);
        }
    }
}
