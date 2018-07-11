namespace Spark.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Markup;

    [ContentProperty(nameof(Setters))]
    public class Trigger : TriggerBase
    {
        private readonly Dictionary<DependencyObject, FrameworkElement> _attached;
        private readonly List<FrameworkElement> _applied;

        public Trigger()
        {
            _attached = new Dictionary<DependencyObject, FrameworkElement>();
            _applied = new List<FrameworkElement>();

            Setters = new SetterBaseCollection();
        }

        public DependencyProperty Property { get; set; }

        public SetterBaseCollection Setters { get; private set; }

        [Ambient]
        public string SourceName { get; set; }

        public object Value { get; set; }

        internal override void Attach(FrameworkElement target, DependencyObject parent)
        {
            DependencyObject source = parent;

            if (SourceName != null)
            {
                source = (DependencyObject)target.FindName(SourceName);
            }

            _attached.Add(source, target);

            if (CheckCondition(source))
            {
                Setters.Attach(target);
                _applied.Add(target);
            }

            IObservableDependencyObject observable = source;
            observable.AttachPropertyChangedHandler(Property.Name, ValueChanged);
        }

        private static object ConvertToType(object value, Type type)
        {
            // Convert.ChangeType doesn't handle nullables. 
            Type u = Nullable.GetUnderlyingType(type);

            if (u != null)
            {
                if (value == null)
                {
                    return Activator.CreateInstance(type);
                }

                return Convert.ChangeType(value, u);
            }

            return Convert.ChangeType(value, type);
        }

        private bool CheckCondition(DependencyObject source)
        {
            object value = source.GetValue(Property);
            return value != null && value.Equals(ConvertToType(Value, Property.PropertyType));
        }

        private void ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DependencyObject source = (DependencyObject)sender;
            FrameworkElement target = _attached[source];

            if (CheckCondition(source))
            {
                if (!_applied.Contains(target))
                {
                    Setters.Attach(target);
                    _applied.Add(target);
                }
            }
            else
            {
                if (_applied.Contains(target))
                {
                    Setters.Detach(target);
                    _applied.Remove(target);
                }
            }
        }
    }
}
