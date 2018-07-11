namespace Spark.UI
{
    public abstract class TriggerBase : DependencyObject
    {
        internal abstract void Attach(FrameworkElement element, DependencyObject parent);
    }
}
