namespace Spark.UI.Controls
{
    public abstract class ContentControl : Control
    {
        private FrameworkElement _contentControl;

        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.Register(nameof(Content), typeof(object), typeof(ContentControl), new PropertyMetadata(OnContentPropertyChanged));

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentControl control = (ContentControl)d;
            if (e.NewValue is UIElement uiElement)
            {
                uiElement.Parent = control;
            }

            control._contentControl = e.NewValue as FrameworkElement;

            control.OnContentChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
            // No-op by default
        }
    }
}
