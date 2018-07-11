namespace Spark.UI.Input
{
    public static class TextCompositionManager
    {
        public static readonly RoutedEvent TextInputEvent =
            EventManager.RegisterRoutedEvent(
                "TextInput",
                RoutingStrategy.Bubble,
                typeof(TextCompositionEventHandler),
                typeof(TextCompositionManager));

        static TextCompositionManager()
        {
            InputManager.Current.PreProcessInput += PreProcessKeyboardInput;
        }

        private static void PreProcessKeyboardInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.Input.Device == Keyboard.PrimaryDevice)
            {
                KeyEventArgs keyEventArgs = e.Input as KeyEventArgs;

                if (keyEventArgs != null)
                {
                    // TODO:
                    //string text = keyEventArgs.KeyboardDevice.KeyToString(keyEventArgs.Key);

                    string text = "a";

                    if (text != string.Empty && !char.IsControl(text[0]))
                    {
                        TextComposition composition = new TextComposition(
                            InputManager.Current,
                            keyEventArgs.Device.Target,
                            text);

                        TextCompositionEventArgs ev = new TextCompositionEventArgs(keyEventArgs.Device, composition)
                        {
                            RoutedEvent = TextInputEvent
                        };

                        InputManager.Current.ProcessInput(ev);
                    }
                }
            }
        }
    }
}
