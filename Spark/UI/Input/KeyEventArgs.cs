namespace Spark.UI.Input
{
    using Spark.Input;

    public delegate void KeyEventHandler(object sender, KeyEventArgs e);

    public class KeyEventArgs : KeyboardEventArgs
    {
        public KeyEventArgs(KeyboardDevice keyboard, Keys key)
            : base(keyboard)
        {
            Key = key;
        }

        public Keys Key { get; }
    }
}
