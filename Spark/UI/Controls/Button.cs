namespace Spark.UI.Controls
{
    using Primitives;

    public class Button : ButtonBase
    {
        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
        }
    }
}
