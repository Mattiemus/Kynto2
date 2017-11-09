namespace Spark.UI
{
    using Math;
    using Media;

    public class UIElement : Visual
    {
        private Size _desiredSize;

        public static readonly DependencyProperty VisibilityProperty 
            = DependencyProperty.Register(nameof(Visibility), typeof(bool), typeof(UIElement), new PropertyMetadata(Visibility.Visible));

        public UIElement()
        {
            _desiredSize = Size.Empty;
        }

        public Visibility Visibility
        {
            get => (Visibility)GetValue(VisibilityProperty);
            set => SetValue(VisibilityProperty, value);
        }

        public bool IsMeasureValid { get; private set; }

        public bool IsArrangeValid { get; private set; }

        public Size DesiredSize
        {
            get
            {
                if (Visibility == Visibility.Collapsed)
                {
                    return Size.Empty;
                }

                return _desiredSize;
            }
        }
        
        public void UpdateLayout()
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        public void InvalidateMeasure()
        {
            IsMeasureValid = false;
        }

        public void InvalidateArrange()
        {
            IsArrangeValid = false;
        }

        public void Measure(Size availableSize)
        {
            if (!IsMeasureValid)
            {
                _desiredSize = MeasureCore(availableSize);
                IsMeasureValid = true;
            }
        }

        public void Arrange(RectangleF finalRect)
        {
            ArrangeCore(finalRect);
            IsArrangeValid = true;
        }

        protected virtual Size MeasureCore(Size availableSize)
        {
            return Size.Empty;
        }

        protected virtual void ArrangeCore(RectangleF finalRect)
        {
            // No-op
        }

        protected override void OnPropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            if (dp.DefaultMetadata is FrameworkPropertyMetadata metadata)
            {
                if (metadata.AffectsMeasure)
                {
                    InvalidateMeasure();
                }

                if (metadata.AffectsArrange)
                {
                    InvalidateArrange();
                }
            }

            base.OnPropertyChanged(dp, oldValue, newValue);
        }
    }
}
