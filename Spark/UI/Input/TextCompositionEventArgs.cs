namespace Spark.UI.Input
{
    public delegate void TextCompositionEventHandler(object sender, TextCompositionEventArgs e);

    public class TextCompositionEventArgs : InputEventArgs
    {
        public TextCompositionEventArgs(InputDevice inputDevice, TextComposition composition)
            : base(inputDevice)
        {
            Text = composition.Text;
        }

        public string Text { get; private set; }
    }
}
