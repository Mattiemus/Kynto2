namespace Spark.UI
{
    using System;

    public delegate void DependencyPropertyChangedEventHandler(object sender, DependencyPropertyChangedEventArgs e);

    public class DependencyPropertyChangedEventArgs : EventArgs
    {
        public DependencyPropertyChangedEventArgs(DependencyProperty property, object oldValue, object newValue)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }
        
        public DependencyProperty Property { get; private set; }

        public object OldValue { get; private set; }

        public object NewValue { get; private set; }
    }
}
