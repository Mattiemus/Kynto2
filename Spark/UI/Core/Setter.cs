namespace Spark.UI
{
    using System;

    public class Setter
    {
        public Setter()
        {
        }

        public Setter(DependencyProperty property, object value)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            Property = property;
            Value = value;
        }

        public string TargetName { get; set; }

        public DependencyProperty Property { get; set; }

        public object Value { get; set; }
    }
}
