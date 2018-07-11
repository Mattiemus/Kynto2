namespace Spark.UI
{
    public interface IObservableDependencyObject
    {
        void AttachPropertyChangedHandler(
            string propertyName,
            DependencyPropertyChangedEventHandler handler);

        void RemovePropertyChangedHandler(
            string propertyName,
            DependencyPropertyChangedEventHandler handler);
    }
}
