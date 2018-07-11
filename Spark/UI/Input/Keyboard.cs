namespace Spark.UI.Input
{
    public static class Keyboard
    {
        public static readonly RoutedEvent GotKeyboardFocusEvent =
            EventManager.RegisterRoutedEvent(
                "GotKeyboardFocus",
                RoutingStrategy.Bubble,
                typeof(KeyboardFocusChangedEventHandler),
                typeof(Keyboard));

        public static readonly RoutedEvent LostKeyboardFocusEvent =
            EventManager.RegisterRoutedEvent(
                "LostKeyboardFocus",
                RoutingStrategy.Bubble,
                typeof(KeyboardFocusChangedEventHandler),
                typeof(Keyboard));

        public static readonly RoutedEvent KeyDownEvent =
            EventManager.RegisterRoutedEvent(
                "KeyDown",
                RoutingStrategy.Bubble,
                typeof(KeyEventHandler),
                typeof(Keyboard));

        static Keyboard()
        {
            PrimaryDevice = new KeyboardDevice();
        }

        public static KeyboardDevice PrimaryDevice { get; }

        public static IInputElement FocusedElement => PrimaryDevice.FocusedElement;

        public static IInputElement Focus(IInputElement element)
        {
            return PrimaryDevice.Focus(element);
        }
    }
}
