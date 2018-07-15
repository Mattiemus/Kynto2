namespace Spark.UI.Controls
{
    public class DefinitionBase : FrameworkContentElement
    {
        public static readonly DependencyProperty SharedSizeGroupProperty =
            DependencyProperty.Register(
                nameof(SharedSizeGroup),
                typeof(string),
                typeof(DefinitionBase),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.Inherits));

        public string SharedSizeGroup
        {
            get => (string)GetValue(SharedSizeGroupProperty);
            set => SetValue(SharedSizeGroupProperty, value);
        }
    }
}
