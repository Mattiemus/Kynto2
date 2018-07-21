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
                nameof(BorderBrush),
                typeof(Brush),
                typeof(Border),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                nameof(BorderThickness),
                typeof(Thickness),
                typeof(Border),
                new FrameworkPropertyMetadata(
                    new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(Border),
                new FrameworkPropertyMetadata(
                    new CornerRadius(),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
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
            Pen pen = null;

            if (BorderBrush != null && !BorderThickness.IsEmpty)
            {
                pen = new Pen(BorderBrush, BorderThickness.Left);
            }

            if (CornerRadius.TopLeft > 0 || CornerRadius.TopRight > 0 || CornerRadius.BottomLeft > 0 || CornerRadius.BottomRight > 0)
            {
                drawingContext.DrawRoundedRectangle(
                    Background,
                    pen,
                    brushRect,
                    CornerRadius);
            }
            else
            {
                drawingContext.DrawRectangle(Background, pen, brushRect);
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
