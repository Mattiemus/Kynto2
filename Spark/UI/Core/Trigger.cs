namespace Spark.UI
{
    using System;

    public class Trigger
    {
        public Trigger()
        {
            Setters = new SetterCollection();
        }

        public Trigger(DependencyProperty property, object value)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            Property = property;
            Value = value;
            Setters = new SetterCollection();
        }

        public DependencyProperty Property { get; set; }

        public object Value { get; set; }

        public SetterCollection Setters { get; }
    }
}
