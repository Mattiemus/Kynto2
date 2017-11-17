namespace Spark.UI
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    public abstract class DependencyObject
    {
        private Dictionary<DependencyProperty, object> _properties;
        private HashSet<DependencyProperty> _propertySet;
        
        internal event DependencyPropertyChangedEventHandler PropertyChanged;
        
        public object GetValue(DependencyProperty dp)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            object defaultValue = dp.DefaultMetadata != null ? dp.DefaultMetadata.DefaultValue : null;
            if (defaultValue == null)
            {
                defaultValue = dp.PropertyType.IsValueType ? Activator.CreateInstance(dp.PropertyType) : null;
            }

            // Check whether the value inherits from its parent
            bool inherits = (dp.DefaultMetadata is FrameworkPropertyMetadata) && ((FrameworkPropertyMetadata)dp.DefaultMetadata).Inherits;

            object currentValue = default(object);
            if (_properties != null && _properties.ContainsKey(dp))
            {
                currentValue = _properties[dp];
            }

            if (currentValue == null && inherits)
            {
                FrameworkElement frameworkElement = this as FrameworkElement;
                if (frameworkElement != null && frameworkElement.Parent != null)
                {
                    currentValue = frameworkElement.Parent.GetValue(dp);
                }
            }

            if (currentValue == null)
            {
                currentValue = defaultValue;
            }

            return currentValue;
        }

        public void SetValue(DependencyPropertyKey propertyKey, object value)
        {
            FrameworkElement frameworkElement = this as FrameworkElement;
            bool setAsAssigned = frameworkElement != null && !frameworkElement.IsInitialized;
            
            SetValue(propertyKey.DependencyProperty, value, setAsAssigned);
        }

        public void SetValue(DependencyProperty property, object value)
        {
            FrameworkElement frameworkElement = this as FrameworkElement;
            bool setAsAssigned = frameworkElement != null && !frameworkElement.IsInitialized;
            
            SetValue(property, value, setAsAssigned);
        }

        protected void SetValue(DependencyProperty property, object value, bool setAsAssigned)
        {
            // TODO: throw exception if read onlys

            if (_properties == null)
            {
                _properties = new Dictionary<DependencyProperty, object>();
            }
            
            if (value != null && !property.PropertyType.IsInstanceOfType(value))
            {
                throw new InvalidCastException();
            }

            object oldValue = _properties.ContainsKey(property) ? _properties[property] : null;
            if ((oldValue == null && value != null) || 
                (oldValue != null && value == null) || 
                (oldValue != null && !oldValue.Equals(value)))
            {

                OnPropertyChanging(property, oldValue, value);
                _properties[property] = value;
                OnPropertyChanged(property, oldValue, value);

                if (setAsAssigned)
                {
                    SetPropertyAsAssigned(property);
                }

                // TODO DefaultValue Metadata...

                if (property.DefaultMetadata != null && property.DefaultMetadata.PropertyChangedCallback != null)
                {
                    property.DefaultMetadata.PropertyChangedCallback(this, new DependencyPropertyChangedEventArgs(property, oldValue, value));
                }
            }
        }

        internal bool IsAssignedProperty(DependencyProperty dp)
        {
            return _propertySet != null && _propertySet.Contains(dp);
        }

        protected virtual void OnPropertyChanging(DependencyProperty dp, object oldValue, object newValue)
        {
            // No-op
        }

        protected virtual void OnPropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            PropertyChanged?.Invoke(this, new DependencyPropertyChangedEventArgs(dp, oldValue, newValue));
        }
                
        protected object GetCurrentValue(string propertyName)
        {
            PropertyInfo propertyInfo = GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(this, null);
            }

            return null;
        }

        protected DependencyProperty GetPropertyOrNull(string name)
        {
            return _properties != null ? _properties.Where(p => p.Key.Name == name).Select(p => p.Key).FirstOrDefault() : null;
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
