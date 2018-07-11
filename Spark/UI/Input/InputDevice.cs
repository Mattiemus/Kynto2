namespace Spark.UI.Input
{
    public abstract class InputDevice : DependencyObject
    {
        public PresentationSource ActiveSource { get; internal set; }

        public abstract IInputElement Target { get; }
    }
}
