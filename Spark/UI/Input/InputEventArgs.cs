namespace Spark.UI.Input
{
    public class InputEventArgs : RoutedEventArgs
    {
        public InputEventArgs(InputDevice inputDevice)
        {
            Device = inputDevice;
        }

        public InputDevice Device { get; }
    }
}
