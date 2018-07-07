namespace Spark.UI
{
    using Math;

    public class UIElement : DependencyObject
    {
        public static readonly DependencyProperty ParentProperty
            = DependencyProperty.Register(
                 nameof(Parent),
                 typeof(UIElement),
                 typeof(UIElement),
                 new PropertyMetadata(new PropertyChangedCallback(OnParentChanged)));

        public static readonly DependencyProperty IsVisibleProperty 
            = DependencyProperty.Register(
                 nameof(IsVisible),
                 typeof(bool),
                 typeof(UIElement),
                 new PropertyMetadata(true, new PropertyChangedCallback(OnIsVisibleChanged)));

        public static readonly DependencyProperty VisibilityProperty 
            = DependencyProperty.Register(
                nameof(Visibility),
                typeof(Visibility),
                typeof(UIElement),
                new FrameworkPropertyMetadata(
                    Visibility.Visible, 
                    new PropertyChangedCallback(OnVisibilityChanged), 
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MarginProperty 
            = DependencyProperty.Register(
                nameof(Margin),
                typeof(Thickness),
                typeof(UIElement));

        public static readonly DependencyProperty PaddingProperty
            = DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(UIElement));

        public static readonly DependencyProperty HorizontalAlignmentProperty 
            = DependencyProperty.Register(
                nameof(HorizontalAlignment),
                typeof(HorizontalAlignment),
                typeof(UIElement),
                new PropertyMetadata(HorizontalAlignment.Stretch));

        public static readonly DependencyProperty VerticalAlignmentProperty
            = DependencyProperty.Register(
                nameof(VerticalAlignment),
                typeof(VerticalAlignment),
                typeof(UIElement),
                new PropertyMetadata(VerticalAlignment.Stretch));

        public static readonly DependencyProperty WidthProperty 
            = DependencyProperty.Register(
                nameof(Width),
                typeof(float?),
                typeof(UIElement),
                new PropertyMetadata(
                    new PropertyChangedCallback(
                        (s, e) => 
                        {
                            UIElement uie = s as UIElement;
                            uie?.InvalidateMeasure();
                        })));

        public static readonly DependencyProperty HeightProperty 
            = DependencyProperty.Register(
                nameof(Height),
                typeof(float?),
                typeof(UIElement),
                new PropertyMetadata(
                    new PropertyChangedCallback(
                        (s, e) => 
                        {
                            UIElement uie = s as UIElement;
                            uie?.InvalidateMeasure();
                        })));
        
        private float? _actualWidth;
        private float? _actualHeight;
        private Size _desiredSize;

        public UIElement()
        {
        }

        public event DependencyPropertyChangedEventHandler IsVisibleChanged;

        public UIElement Parent
        {
            get { return (UIElement)GetValue(ParentProperty); }
            set { SetValue(ParentProperty, value); }
        }

        public bool IsInitialized { get; protected set; }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            private set { SetValue(IsVisibleProperty, value); }
        }

        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

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

        public bool IsMeasureValid { get; private set; }

        public bool IsArrangeValid { get; private set; }

        public float? Width
        {
            get { return (float?)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public float? Height
        {
            get { return (float?)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public RectangleF Bounds 
            => new RectangleF(
                GetAbsoluteLeft(),
                GetAbsoluteTop(),
                ActualWidth,
                ActualHeight);

        public float ActualWidth
        {
            get
            {
                if (_actualWidth.HasValue)
                {
                    return _actualWidth.Value;
                }

                float result = 0.0f;
                if (Width != null)
                {
                    result = Width.Value;
                }
                else if (Parent != null)
                {
                    result = Parent.MeasureWidth(this) - Margin.Left - Margin.Right;
                }

                _actualWidth = result;
                return result;
            }
        }

        public float ActualHeight
        {
            get
            {
                if (_actualHeight.HasValue)
                {
                    return _actualHeight.Value;
                }

                float result = 0.0f;
                if (Height != null)
                {
                    result = Height.Value;
                }
                else if (Parent != null)
                {
                    result = Parent.MeasureHeight(this) - Margin.Top - Margin.Bottom;
                }

                _actualHeight = result;
                return result;
            }
        }

        private static void OnParentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uie = sender as UIElement;
            if (uie != null && e.NewValue != null)
            {
                var parent = e.NewValue as UIElement;
                uie.UpdateIsVisible();
                parent.IsVisibleChanged += (s, ea) =>
                {
                    uie.UpdateIsVisible();
                };
            }
        }

        private static void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uie = sender as UIElement;
            uie?.IsVisibleChanged?.Invoke(uie, e);
        }

        private static void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uie = sender as UIElement;
            uie?.UpdateIsVisible();
        }

        public virtual void Initialize()
        {
            IsInitialized = true;
            UpdateIsVisible();
        }

        public void InvalidateMeasure()
        {
            IsMeasureValid = false;

            _actualWidth = null;
            _actualHeight = null;
        }

        public void InvalidateArrange()
        {
            IsArrangeValid = false;
        }

        public void UpdateLayout()
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        public void Measure(Size availableSize)
        {
            if (!IsMeasureValid)
            {
                _desiredSize = new Size(MeasureWidth(availableSize.Width), MeasureHeight(availableSize.Height));
                IsMeasureValid = true;
            }
        }

        public void Arrange(RectangleF finalRect)
        {
            IsArrangeValid = true;
        }

        public virtual void Draw(DrawingContext drawingContext)
        {
        }

        public virtual float GetAbsoluteLeft()
        {
            if (Parent != null)
            {
                return Parent.GetAbsoluteLeft(this);
            }

            return Margin.Left;
        }

        public virtual float GetAbsoluteTop()
        {
            if (Parent != null)
            {
                return Parent.GetAbsoluteTop(this);
            }

            return Margin.Top;
        }

        protected virtual float GetAbsoluteLeft(UIElement child)
        {
            var result = 0.0f;

            if (child.HorizontalAlignment == HorizontalAlignment.Left || child.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                result = child.Margin.Left;
                if (child.Parent != null)
                {
                    result += child.Parent.GetAbsoluteLeft();
                }
            }
            else if (child.HorizontalAlignment == HorizontalAlignment.Right)
            {
                result = child.Parent.ActualWidth - child.Margin.Right - child.ActualWidth + child.Parent.GetAbsoluteLeft();
            }
            else if (child.HorizontalAlignment == HorizontalAlignment.Center)
            {
                result = (child.Parent.ActualWidth / 2.0f) - (child.ActualWidth / 2.0f) + child.Margin.Left - child.Margin.Right;
                if (child.Parent != null)
                {
                    result += child.Parent.GetAbsoluteLeft();
                }
            }

            return result;
        }

        protected virtual float GetAbsoluteTop(UIElement child)
        {
            var result = 0.0f;

            if (child.VerticalAlignment == VerticalAlignment.Top || child.VerticalAlignment == VerticalAlignment.Stretch)
            {
                result = child.Margin.Top;
                if (child.Parent != null)
                {
                    result += child.Parent.GetAbsoluteTop();
                }
            }
            else if (child.VerticalAlignment == VerticalAlignment.Bottom)
            {
                result = child.Parent.ActualHeight - child.Margin.Bottom - child.ActualHeight + child.Parent.GetAbsoluteTop();
            }
            else if (child.VerticalAlignment == VerticalAlignment.Center)
            {
                result = (child.Parent.ActualHeight / 2.0f) - (child.ActualHeight / 2.0f) + child.Margin.Top - child.Margin.Bottom;
                if (child.Parent != null)
                {
                    result += child.Parent.GetAbsoluteTop();
                }
            }

            return result;
        }

        protected virtual float MeasureWidth(float availableWidth)
        {
            return 0.0f;
        }

        protected virtual float MeasureHeight(float availableHeight)
        {
            return 0.0f;
        }

        protected virtual float MeasureWidth(UIElement child)
        {
            float result = 0.0f;
            if (child.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                result = ActualWidth;
            }
            else
            {
                result = child.MeasureWidth(ActualWidth);
            }

            return result;
        }

        protected virtual float MeasureHeight(UIElement child)
        {
            float result = 0.0f;
            if (child.VerticalAlignment == VerticalAlignment.Stretch)
            {
                result = ActualHeight;
            }
            else
            {
                result = child.MeasureHeight(ActualHeight);
            }

            return result;
        }

        private void UpdateIsVisible()
        {
            IsVisible = Visibility == Visibility.Visible && (Parent == null || Parent.IsVisible);
        }
    }
}
