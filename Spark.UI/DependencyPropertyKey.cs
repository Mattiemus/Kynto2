namespace Spark.UI
{
    public sealed class DependencyPropertyKey
    {
        internal DependencyPropertyKey(DependencyProperty property)
        {
            DependencyProperty = property;
        }

        public DependencyProperty DependencyProperty { get; }
    }
}
