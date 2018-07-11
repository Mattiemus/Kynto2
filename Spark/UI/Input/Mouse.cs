namespace Spark.UI.Input
{
    public static class Mouse
    {
        public static readonly RoutedEvent MouseEnterEvent =
            EventManager.RegisterRoutedEvent(
                "MouseEnter",
                RoutingStrategy.Direct,
                typeof(MouseEventHandler),
                typeof(Mouse));

        public static readonly RoutedEvent MouseLeaveEvent =
            EventManager.RegisterRoutedEvent(
                "MouseLeave",
                RoutingStrategy.Direct,
                typeof(MouseEventHandler),
                typeof(Mouse));

        public static readonly RoutedEvent MouseLeftButtonDownEvent =
            EventManager.RegisterRoutedEvent(
                "MouseLeftButtonDown",
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(Mouse));

        public static readonly RoutedEvent MouseLeftButtonUpEvent =
            EventManager.RegisterRoutedEvent(
                "MouseLeftButtonUp",
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(Mouse));

        public static readonly RoutedEvent MouseMoveEvent =
            EventManager.RegisterRoutedEvent(
                "MouseMove",
                RoutingStrategy.Bubble,
                typeof(MouseEventHandler),
                typeof(Mouse));

        static Mouse()
        {
            PrimaryDevice = new MouseDevice();
        }

        public static IInputElement Captured => PrimaryDevice.Captured;

        public static MouseDevice PrimaryDevice { get; }

        public static void Capture(IInputElement element)
        {
            PrimaryDevice.Capture(element);
        }
    }
}
