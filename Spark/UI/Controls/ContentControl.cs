namespace Spark.UI.Controls
{
    using System.Windows.Markup;

    using Math;

    [ContentProperty(nameof(Content))]
    public class ContentControl : Control
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(ContentControl),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    ContentPropertyChanged));

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                nameof(ContentTemplate),
                typeof(DataTemplate),
                typeof(ContentControl),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        static ContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ContentControl),
                new FrameworkPropertyMetadata(typeof(ContentControl)));
        }

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (VisualChildrenCount > 0)
            {
                UIElement ui = GetVisualChild(0) as UIElement;

                if (ui != null)
                {
                    ui.Measure(constraint);
                    return ui.DesiredSize;
                }
            }

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (VisualChildrenCount > 0)
            {
                UIElement ui = GetVisualChild(0) as UIElement;
                FrameworkElement fe = ui as FrameworkElement;

                if (ui != null)
                {
                    ui.Arrange(new RectangleF(Vector2.Zero, finalSize));
                    return finalSize;
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        private static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentControl)d).IsInitialized = true;
        }
    }
}
