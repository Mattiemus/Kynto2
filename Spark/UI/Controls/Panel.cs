namespace Spark.UI.Controls
{
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Markup;

    using Media;

    [ContentProperty(nameof(Children))]
    public abstract class Panel : FrameworkElement
    {
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                nameof(Background),
                typeof(Brush),
                typeof(Panel),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsItemsHostProperty =
            DependencyProperty.Register(
                nameof(IsItemsHost),
                typeof(bool),
                typeof(Panel));

        protected Panel()
        {
            InternalChildren = new UIElementCollection(this, this);
        }

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public UIElementCollection Children => IsItemsHost ? null : InternalChildren;

        [Bindable(false)]
        public bool IsItemsHost
        {
            get => (bool)GetValue(IsItemsHostProperty);
            set => SetValue(IsItemsHostProperty, value);
        }

        protected internal override IEnumerator LogicalChildren => InternalChildren.GetEnumerator();

        protected internal UIElementCollection InternalChildren { get; }

        protected internal override int VisualChildrenCount => InternalChildren.Count;

        protected internal override Visual GetVisualChild(int index)
        {
            return InternalChildren[index];
        }
    }
}
