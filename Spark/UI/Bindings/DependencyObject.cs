namespace Spark.UI
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public abstract class DependencyObject
    {
        private Dictionary<DependencyProperty, object> _properties;
        private HashSet<DependencyProperty> _propertySet;

        internal event DependencyPropertyChangedEventHandler PropertyChanged;

        private static object GetDefaultValueOfType(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public object GetValue(DependencyProperty dp)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            var defaultValue = dp.DefaultMetadata != null ? dp.DefaultMetadata.DefaultValue : null;
            if (defaultValue == null)
            {
                defaultValue = GetDefaultValueOfType(dp.PropertyType);
            }
            
            var currentValue = default(Object);
            if (_properties != null && _properties.ContainsKey(dp))
            {
                currentValue = _properties[dp];
            }

            if (currentValue == null)
            {
                var inherits = (dp.DefaultMetadata is FrameworkPropertyMetadata) && ((FrameworkPropertyMetadata)dp.DefaultMetadata).Inherits;
                if (inherits)
                {
                    var uielement = this as UIElement;
                    if (uielement != null && uielement.Parent != null)
                    {
                        currentValue = uielement.Parent.GetValue(dp);
                    }
                }
            }

            if (currentValue == null)
            {
                currentValue = defaultValue;
            }

            return currentValue;
        }
        
        public void SetValue(DependencyProperty property, object value)
        {
            var frameworkElement = this as FrameworkElement;
            var setAsAssigned = frameworkElement != null && !frameworkElement.IsInitialized;

            SetValue(property, value, setAsAssigned);
        }

        protected void SetValue(DependencyProperty property, object value, bool setAsAssigned)
        {
            if (_properties == null)
            {
                _properties = new Dictionary<DependencyProperty, object>();
            }

            if (value != null && !property.PropertyType.IsAssignableFrom(value.GetType()))
            {
                throw new InvalidCastException();
            }

            var oldValue = _properties.ContainsKey(property) ? _properties[property] : null;
            if ((oldValue == null && value != null) || 
                (oldValue != null && value == null) || 
                (oldValue != null && value != null && !oldValue.Equals(value)))
            {
                OnPropertyChanging(property, oldValue, value);
                _properties[property] = value;
                OnPropertyChanged(property, oldValue, value);

                if (setAsAssigned)
                {
                    SetPropertyAsAssigned(property);
                }

                // TODO DefaultValue Metadata

                if (property.DefaultMetadata != null && property.DefaultMetadata.PropertyChangedCallback != null)
                {
                    property.DefaultMetadata.PropertyChangedCallback(this, new DependencyPropertyChangedEventArgs(property, oldValue, value));
                }
            }
        }

        protected virtual void OnPropertyChanging(DependencyProperty dp, object oldValue, object newValue)
        {
        }

        protected virtual void OnPropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            PropertyChanged?.Invoke(this, new DependencyPropertyChangedEventArgs(dp, oldValue, newValue));
        }

        internal bool IsAssignedProperty(DependencyProperty dp)
        {
            return _propertySet != null && _propertySet.Contains(dp);
        }

        protected object GetCurrentValue(string propertyName)
        {
            var propertyInfo = GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(this, null);
            }

            return null;
        }

        protected DependencyProperty GetPropertyOrNull(string name)
        {
            return _properties != null
                ? _properties
                    .Where(p => p.Key.Name == name)
                    .Select(p => p.Key)
                    .FirstOrDefault()
                : null;
        }

        protected void SetPropertyAsAssigned(DependencyProperty property)
        {
            if (_propertySet == null)
            {
                _propertySet = new HashSet<DependencyProperty>();
            }

            if (!_propertySet.Contains(property))
            {
                _propertySet.Add(property);
            }
        }
    }
}
