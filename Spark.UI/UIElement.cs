namespace Spark.UI
{
    using Math;
    using Media;

    public class UIElement : Visual
    {
        private Size _desiredSize;

        public static readonly DependencyProperty ParentProperty 
            = DependencyProperty.Register(nameof(Parent), typeof(UIElement), typeof(UIElement), new PropertyMetadata(OnParentChanged));

        internal static readonly DependencyPropertyKey IsVisiblePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsVisible), typeof(bool), typeof(UIElement), new PropertyMetadata(OnIsVisibleChanged));
        public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;

        public static readonly DependencyProperty VisibilityProperty 
            = DependencyProperty.Register(nameof(Visibility), typeof(bool), typeof(UIElement), new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));

        public UIElement()
        {
            _desiredSize = Size.Empty;
        }
        
        public event DependencyPropertyChangedEventHandler IsVisibleChanged;

        public UIElement Parent
        {
            get => (UIElement)GetValue(ParentProperty);
            set => SetValue(ParentProperty, value);
        }

        public bool IsVisible => (bool)GetValue(IsVisibleProperty);

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


        private static void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)sender).UpdateIsVisible();
        }

        private static void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uie = sender as UIElement;
            uie.IsVisibleChanged?.Invoke(uie, e);
        }

        private static void OnParentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement uie && e.NewValue != null)
            {
                uie.UpdateIsVisible();

                UIElement parent = e.NewValue as UIElement;
                parent.IsVisibleChanged += 
                    (s, ea) => 
                    {
                        uie.UpdateIsVisible();
                    };
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

        private void UpdateIsVisible()
        {
            SetValue(IsVisiblePropertyKey, Visibility == Visibility.Visible && (Parent == null || Parent.IsVisible));
        }
    }
}
