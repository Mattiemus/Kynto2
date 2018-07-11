namespace Spark.UI.Controls
{
    using System;
    using System.Windows.Markup;

    using Input;
    using Media;
    using Math;

    public class Control : FrameworkElement
    {
        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(Color.White),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BorderBrushProperty =
            Border.BorderBrushProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(Color.White),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BorderThicknessProperty =
            Border.BorderThicknessProperty.AddOwner(
                typeof(Control),
                new FrameworkPropertyMetadata(
                    new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalContentAlignment),
                typeof(HorizontalAlignment),
                typeof(Control),
                new FrameworkPropertyMetadata(HorizontalAlignment.Left));

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(Control),
                new FrameworkPropertyMetadata(
                    new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(
                nameof(Template),
                typeof(ControlTemplate),
                typeof(Control),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(
                "VerticalContentAlignment",
                typeof(VerticalAlignment),
                typeof(Control),
                new FrameworkPropertyMetadata(VerticalAlignment.Top));

        private Visual _child;

        static Control()
        {
            FocusableProperty.OverrideMetadata(typeof(Control), new PropertyMetadata(true));
        }

        public Control()
        {
            Background = new SolidColorBrush(Color.White);
            AddHandler(KeyDownEvent, (KeyEventHandler)((s, e) => OnKeyDown(e)));
        }

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

        public HorizontalAlignment HorizontalContentAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty);
            set => SetValue(HorizontalContentAlignmentProperty, value);
        }

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public ControlTemplate Template
        {
            get => (ControlTemplate)GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get => (VerticalAlignment)GetValue(VerticalContentAlignmentProperty);
            set => SetValue(VerticalContentAlignmentProperty, value);
        }

        protected internal override int VisualChildrenCount => (_child != null) ? 1 : 0;

        public sealed override bool ApplyTemplate()
        {
            if (IsInitialized && _child == null)
            {
                if (Template == null)
                {
                    ApplyTheme();
                }

                if (Template != null)
                {
                    _child = Template.CreateVisualTree(this);
                    AddVisualChild(_child);
                    OnApplyTemplate();
                    return true;
                }
            }

            return false;
        }

        protected internal sealed override DependencyObject GetTemplateChild(string childName)
        {
            if (_child != null)
            {
                INameScope nameScope = NameScope.GetNameScope(_child);
                if (nameScope != null)
                {
                    return nameScope.FindName(childName) as DependencyObject;
                }
            }

            return null;
        }

        protected internal override Visual GetVisualChild(int index)
        {
            if (_child != null && index == 0)
            {
                return _child;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UIElement ui = _child as UIElement;
            if (ui != null)
            {
                ui.Measure(constraint);
                return ui.DesiredSize;
            }

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement ui = _child as UIElement;

            if (ui != null)
            {
                ui.Arrange(new RectangleF(Vector2.Zero, finalSize));
                return finalSize;
            }

            return base.ArrangeOverride(finalSize);
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
        }

        private void ApplyTheme()
        {
            object defaultStyleKey = DefaultStyleKey;
            if (defaultStyleKey == null)
            {
                throw new InvalidOperationException("DefaultStyleKey must be set.");
            }

            Style style = (Style)FindResource(DefaultStyleKey);
            style.Attach(this);
        }
    }
}
