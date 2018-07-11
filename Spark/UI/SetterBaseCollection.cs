namespace Spark.UI
{
    using System.Collections.ObjectModel;

    public sealed class SetterBaseCollection : Collection<SetterBase>
    {
        internal void Attach(FrameworkElement element)
        {
            foreach (SetterBase setter in this)
            {
                setter.Attach(element);
            }
        }

        internal void Detach(FrameworkElement element)
        {
            foreach (SetterBase setter in this)
            {
                setter.Detach(element);
            }
        }
    }
}
