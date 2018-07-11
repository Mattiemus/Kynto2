namespace Spark.UI.Input
{
    public class KeyboardEventArgs : InputEventArgs
    {
        public KeyboardEventArgs(KeyboardDevice keyboard)
            : base(keyboard)
        {
        }

        public KeyboardDevice KeyboardDevice => (KeyboardDevice)Device;
    }
}
