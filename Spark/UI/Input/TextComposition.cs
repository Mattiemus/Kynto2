namespace Spark.UI.Input
{
    public class TextComposition 
    {
        public TextComposition(InputManager inputManager, IInputElement source, string resultText)
        {
            Text = resultText;
        }

        public string Text { get; set; }
    }
}
