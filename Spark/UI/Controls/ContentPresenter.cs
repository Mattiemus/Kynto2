namespace Spark.UI.Controls
{
    using System;

    using Media;
    using Math;

    public class ContentPresenter : FrameworkElement
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(ContentPresenter),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    ContentChanged));

        public static readonly DependencyProperty ContentTemplateProperty =
            ContentControl.ContentTemplateProperty.AddOwner(
                typeof(ContentPresenter),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        private Visual _visualChild;

        public ContentPresenter()
        {
        }

        internal ContentPresenter(ContentControl templatedParent)
        {
            Content = templatedParent.Content;
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

        protected internal override int VisualChildrenCount => (_visualChild != null) ? 1 : 0;

        public override bool ApplyTemplate()
        {
            if (IsInitialized && _visualChild == null)
            {
                Visual visual = Content as Visual;

                if (visual == null && Content != null)
                {
                    DataTemplate template = ContentTemplate;

                    if (template == null)
                    {
                        DataTemplateKey key = new DataTemplateKey(Content.GetType());
                        template = TryFindResource(key) as DataTemplate;
                    }

                    if (template != null)
                    {
                        visual = template.CreateVisualTree(this);

                        FrameworkElement fe = visual as FrameworkElement;

                        if (fe != null)
                        {
                            fe.DataContext = Content;
                        }
                    }
                    else
                    {
                        // TODO:
                        //visual = new TextBlock
                        //{
                        //    Text = Content.ToString(),
                        //};
                    }
                }

                if (visual != null)
                {
                    _visualChild = visual;
                    AddVisualChild(_visualChild);
                    OnApplyTemplate();
                    return true;
                }
            }

            return false;
        }

        protected internal override Visual GetVisualChild(int index)
        {
            if (index > 0 || _visualChild == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _visualChild;
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

                if (ui != null)
                {
                    ui.Arrange(new RectangleF(Vector2.Zero, finalSize));
                    return finalSize;
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        private static void ContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((ContentPresenter)sender).ContentChanged(e.OldValue, e.NewValue);
        }

        private void ContentChanged(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                RemoveLogicalChild(oldValue);
                RemoveVisualChild(_visualChild);
                _visualChild = null;
            }

            if (newValue != null)
            {
                AddLogicalChild(newValue);
            }

            ApplyTemplate();
        }
    }
}
