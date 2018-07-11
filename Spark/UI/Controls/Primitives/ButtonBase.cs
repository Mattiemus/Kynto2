namespace Spark.UI.Controls.Primitives
{
    using Input;
    using Math;

    public class ButtonBase : ContentControl
    {
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register(
                nameof(IsPressed),
                typeof(bool),
                typeof(ButtonBase));

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Click),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ButtonBase));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            protected set => SetValue(IsPressedProperty, value);
        }

        protected virtual void OnClick()
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsPressed = true;
            e.Handled = true;
            CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
            ReleaseMouseCapture();

            if (IsPressed)
            {
                IsPressed = false;
                OnClick();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                IsPressed = WithinBounds(e.GetPosition(this));
            }
        }

        private bool WithinBounds(Vector2 p)
        {
            return p.X > 0 && p.X < ActualWidth && p.Y > 0 && p.Y < ActualHeight;
        }
    }
}
