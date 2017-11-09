namespace Spark.UI
{
    using Math;

    public class FrameworkElement : UIElement
    {
        internal static readonly DependencyPropertyKey ActualWidthKey 
            = DependencyProperty.RegisterReadOnly(nameof(ActualWidth), typeof(float), typeof(FrameworkElement), new FrameworkPropertyMetadata(float.NaN));
        public static readonly DependencyProperty ActualWidthProperty = ActualWidthKey.DependencyProperty;

        internal static readonly DependencyPropertyKey ActualHeightKey 
            = DependencyProperty.RegisterReadOnly(nameof(ActualHeight), typeof(float), typeof(FrameworkElement), new FrameworkPropertyMetadata(float.NaN));
        public static readonly DependencyProperty ActualHeightProperty = ActualHeightKey.DependencyProperty;

        public static readonly DependencyProperty HorizontalAlignmentProperty
            = DependencyProperty.Register(nameof(HorizontalAlignment), typeof(HorizontalAlignment), typeof(FrameworkElement), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch) { AffectsMeasure = true, AffectsArrange = true });

        public static readonly DependencyProperty VerticalAlignmentProperty
            = DependencyProperty.Register(nameof(HorizontalAlignment), typeof(VerticalAlignment), typeof(FrameworkElement), new FrameworkPropertyMetadata(VerticalAlignment.Stretch) { AffectsMeasure = true, AffectsArrange = true });

        public static readonly DependencyProperty MarginProperty
            = DependencyProperty.Register(nameof(Margin), typeof(Thickness), typeof(FrameworkElement), new FrameworkPropertyMetadata(Thickness.Zero) { AffectsMeasure = true, AffectsArrange = true });

        public float ActualWidth => (float)GetValue(ActualWidthProperty);

        public float ActualHeight => (float)GetValue(ActualHeightProperty);

        public HorizontalAlignment HorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalAlignmentProperty);
            set => SetValue(HorizontalAlignmentProperty, value);
        }

        public VerticalAlignment VerticalAlignment
        {
            get => (VerticalAlignment)GetValue(VerticalAlignmentProperty);
            set => SetValue(VerticalAlignmentProperty, value);
        }

        public Thickness Margin
        {
            get => (Thickness)GetValue(MarginProperty);
            set => SetValue(MarginProperty, value);
        }

        public DependencyObject Parent { get; }

        public bool IsInitialized { get; }
        
        protected sealed override Size MeasureCore(Size availableSize)
        {
            return MeasureOverride(availableSize);
        }

        protected sealed override void ArrangeCore(RectangleF finalRect)
        {
            ArrangeOverride(finalRect.Size);
        }

        protected virtual Size MeasureOverride(Size availableSize)
        {
            return Size.Empty;
        }

        protected virtual Size ArrangeOverride(Size finalSize)
        {
            return Size.Empty;
        }
    }
}
