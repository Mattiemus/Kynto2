namespace Spark.UI.Controls
{
    using Media;
    using Math;

    public class Border : Decorator
    {
        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(
                typeof(Border),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                "BorderBrush",
                typeof(Brush),
                typeof(Border),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness",
                typeof(Thickness),
                typeof(Border),
                new FrameworkPropertyMetadata(
                    new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(Border),
                new FrameworkPropertyMetadata(
                    new CornerRadius(),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                "Padding",
                typeof(Thickness),
                typeof(Border),
                new FrameworkPropertyMetadata(
                    new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        protected internal override void OnRender(DrawingContext drawingContext)
        {
            RectangleF brushRect = new RectangleF(Vector2.Zero, new Size(ActualWidth, ActualHeight));
            RectangleF penRect = brushRect;
            Pen pen = null;

            if (BorderBrush != null && !BorderThickness.IsEmpty)
            {
                pen = new Pen(BorderBrush, BorderThickness.Left);

                float penOffset = -(pen.Thickness / 2);
                brushRect.Inflate(-pen.Thickness, -pen.Thickness);
                penRect.Inflate(penOffset, penOffset);
            }

            if (CornerRadius.TopLeft > 0 || CornerRadius.BottomLeft > 0)
            {
                drawingContext.DrawRoundedRectangle(
                    Background,
                    pen,
                    brushRect,
                    CornerRadius.TopLeft,
                    CornerRadius.BottomLeft);
            }
            else
            {
                if (Background != null)
                {
                    drawingContext.DrawRectangle(Background, null, brushRect);
                }

                if (pen != null)
                {
                    drawingContext.DrawRectangle(null, pen, penRect);
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (Child != null)
            {
                constraint -= Padding + BorderThickness;
                Child.Measure(constraint);
                return Child.DesiredSize + Padding + BorderThickness;
            }

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child != null)
            {
                RectangleF rect = new RectangleF(Vector2.Zero, finalSize) - Padding - BorderThickness;
                Child.Arrange(rect);
            }

            return finalSize;
        }
    }
}
