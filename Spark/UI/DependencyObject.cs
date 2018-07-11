namespace Spark.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Media;
    using Data;

    public class DependencyObject : IObservableDependencyObject
    {
        private static Dictionary<Type, Dictionary<string, DependencyProperty>> PropertyDeclarations = 
            new Dictionary<Type, Dictionary<string, DependencyProperty>>();

        private readonly Dictionary<DependencyProperty, object> _properties;
        private readonly Dictionary<DependencyProperty, BindingExpressionBase> _propertyBindings;
        private readonly Dictionary<string, List<DependencyPropertyChangedEventHandler>> _propertyChangedHandlers;
        private DependencyObject dependencyParent;

        public DependencyObject()
        {
            _properties = new Dictionary<DependencyProperty, object>();
            _propertyBindings = new Dictionary<DependencyProperty, BindingExpressionBase>();
            _propertyChangedHandlers = new Dictionary<string, List<DependencyPropertyChangedEventHandler>>();
        }

        public bool IsSealed => false;

        public DependencyObjectType DependencyObjectType => DependencyObjectType.FromSystemType(GetType());

        internal DependencyObject DependencyParent
        {
            get => dependencyParent;
            set
            {
                if (dependencyParent == value)
                {
                    return;
                }

                DependencyProperty[] inheriting = GetInheritingProperties().ToArray();
                Dictionary<DependencyProperty, object> oldValues = new Dictionary<DependencyProperty, object>();

                foreach (DependencyProperty dp in inheriting)
                {
                    oldValues[dp] = GetValue(dp);
                }

                dependencyParent = value;

                foreach (DependencyProperty dp in inheriting)
                {
                    object oldValue = oldValues[dp];
                    object newValue = GetValue(dp);

                    if (!AreEqual(oldValues[dp], newValue))
                    {
                        DependencyPropertyChangedEventArgs e 
                            = new DependencyPropertyChangedEventArgs(
                                dp,
                                oldValue,
                                newValue);

                        OnPropertyChanged(e);
                    }
                }                
            }
        }

        public void AttachPropertyChangedHandler(
            string propertyName,
            DependencyPropertyChangedEventHandler handler)
        {
            List<DependencyPropertyChangedEventHandler> handlers;

            if (!_propertyChangedHandlers.TryGetValue(propertyName, out handlers))
            {
                handlers = new List<DependencyPropertyChangedEventHandler>();
                _propertyChangedHandlers.Add(propertyName, handlers);
            }

            handlers.Add(handler);
        }

        public void RemovePropertyChangedHandler(
            string propertyName,
            DependencyPropertyChangedEventHandler handler)
        {
            List<DependencyPropertyChangedEventHandler> handlers;
            if (_propertyChangedHandlers.TryGetValue(propertyName, out handlers))
            {
                handlers.Remove(handler);
            }
        }

        public void ClearValue(DependencyProperty dp)
        {
            if (IsSealed)
            {
                throw new InvalidOperationException("Cannot manipulate property values on a sealed DependencyObject");
            }

            _properties.Remove(dp);
        }

        public void ClearValue(DependencyPropertyKey key)
        {
            ClearValue(key.DependencyProperty);
        }

        public void CoerceValue(DependencyProperty dp)
        {
            PropertyMetadata pm = dp.GetMetadata(this);
            pm.CoerceValueCallback?.Invoke(this, GetValue(dp));
        }

        public LocalValueEnumerator GetLocalValueEnumerator()
        {
            return new LocalValueEnumerator(_properties);
        }

        public object GetValue(DependencyProperty dp)
        {
            object val;

            if (!_properties.TryGetValue(dp, out val))
            {
                val = GetDefaultValue(dp);

                if (val == null && dp.PropertyType.IsValueType)
                {
                    val = Activator.CreateInstance(dp.PropertyType);
                }
            }

            return val;
        }

        public void InvalidateProperty(DependencyProperty dp)
        {
            BindingExpressionBase binding;

            if (_propertyBindings.TryGetValue(dp, out binding))
            {
                object oldValue = GetValue(dp);
                object newValue = binding.GetValue();
                SetValueInternal(dp, oldValue, newValue);
            }
        }

        public object ReadLocalValue(DependencyProperty dp)
        {
            object val = _properties[dp];
            return val == null ? DependencyProperty.UnsetValue : val;
        }

        public void SetBinding(DependencyProperty dp, string path)
        {
            SetBinding(dp, new Binding(path));
        }

        public void SetBinding(DependencyProperty dp, BindingBase binding)
        {
            Binding b = binding as Binding;

            if (b == null)
            {
                throw new NotSupportedException("Unsupported binding type.");
            }

            SetBinding(dp, b);
        }

        public BindingExpression SetBinding(DependencyProperty dp, Binding binding)
        {
            PropertyPathParser pathParser = new PropertyPathParser();
            BindingExpression expression = new BindingExpression(pathParser, this, dp, binding);
            object oldValue = GetValue(dp);
            object newValue = expression.GetValue();

            _propertyBindings.Add(dp, expression);
            SetValueInternal(dp, oldValue, newValue);

            return expression;
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            if (IsSealed)
            {
                throw new InvalidOperationException("Cannot manipulate property values on a sealed DependencyObject.");
            }

            if (value != DependencyProperty.UnsetValue && !dp.IsValidType(value))
            {
                throw new ArgumentException("Value is not of the correct type for this DependencyProperty.");
            }

            if (dp.ValidateValueCallback != null && !dp.ValidateValueCallback(value))
            {
                throw new Exception("Value does not validate.");
            }

            object oldValue = GetValue(dp);
            _propertyBindings.Remove(dp);
            SetValueInternal(dp, oldValue, value);
        }

        public void SetValue(DependencyPropertyKey key, object value)
        {
            SetValue(key.DependencyProperty, value);
        }

        internal static IEnumerable<DependencyProperty> GetAllProperties(Type type)
        {
            Type t = type;

            while (t != null)
            {
                Dictionary<string, DependencyProperty> list;

                if (PropertyDeclarations.TryGetValue(t, out list))
                {
                    foreach (DependencyProperty dp in list.Values)
                    {
                        yield return dp;
                    }
                }

                t = t.BaseType;
            }
        }

        internal static DependencyProperty GetPropertyFromName(Type type, string name)
        {
            Dictionary<string, DependencyProperty> list;
            DependencyProperty result;
            Type t = type;

            while (t != null)
            {
                if (PropertyDeclarations.TryGetValue(t, out list) && list.TryGetValue(name, out result))
                {
                    return result;
                }

                t = t.BaseType;
            }

            throw new KeyNotFoundException(string.Format(
                "Dependency property '{0}' could not be found on type '{1}'.",
                name,
                type.FullName));
        }

        internal static void Register(Type t, DependencyProperty dp)
        {
            Dictionary<string, DependencyProperty> typeDeclarations;
            if (!PropertyDeclarations.TryGetValue(t, out typeDeclarations))
            {
                typeDeclarations = new Dictionary<string, DependencyProperty>();
                PropertyDeclarations.Add(t, typeDeclarations);
            }

            if (!typeDeclarations.ContainsKey(dp.Name))
            {
                typeDeclarations[dp.Name] = dp;
            }
            else
            {
                throw new ArgumentException("A property named " + dp.Name + " already exists on " + t.Name);
            }
        }

        internal bool IsRegistered(Type t, DependencyProperty dp)
        {
            return GetAllProperties(t).Contains(dp);
        }

        internal bool IsUnset(DependencyProperty dependencyProperty)
        {
            return !_properties.ContainsKey(dependencyProperty);
        }

        protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            PropertyMetadata pm = e.Property.GetMetadata(this);

            if (pm != null)
            {
                pm.PropertyChangedCallback?.Invoke(this, e);
            }

            List<DependencyPropertyChangedEventHandler> handlers;

            if (_propertyChangedHandlers.TryGetValue(e.Property.Name, out handlers))
            {
                foreach (DependencyPropertyChangedEventHandler handler in handlers.ToArray())
                {
                    handler(this, e);
                }
            }

            FrameworkPropertyMetadata metadata = e.Property.GetMetadata(this) as FrameworkPropertyMetadata;
            UIElement uiElement = this as UIElement;

            if (metadata != null && uiElement != null)
            {
                if (metadata.AffectsArrange)
                {
                    uiElement.InvalidateArrange();
                }

                if (metadata.AffectsMeasure)
                {
                    uiElement.InvalidateMeasure();
                }

                if (metadata.AffectsRender)
                {
                    uiElement.InvalidateVisual();
                }

                if (metadata.Inherits)
                {
                    foreach (DependencyObject child in VisualTreeHelper.GetChildren(this))
                    {
                        child.InheritedValueChanged(e);
                    }
                }
            }
        }

        protected virtual bool ShouldSerializeProperty(DependencyProperty dp)
        {
            throw new NotImplementedException();
        }

        private bool AreEqual(object a, object b)
        {
            return Equals(a, b);
        }

        private object GetDefaultValue(DependencyProperty dp)
        {
            PropertyMetadata metadata = dp.GetMetadata(this);
            FrameworkPropertyMetadata frameworkMetadata = metadata as FrameworkPropertyMetadata;
            object result = metadata.DefaultValue;

            if (frameworkMetadata != null && frameworkMetadata.Inherits && dependencyParent != null)
            {
                result = dependencyParent.GetValue(dp);
            }

            return result;
        }

        private IEnumerable<DependencyProperty> GetInheritingProperties()
        {
            foreach (DependencyProperty dp in GetAllProperties(GetType()))
            {
                FrameworkPropertyMetadata metadata = dp.GetMetadata(GetType()) as FrameworkPropertyMetadata;

                if (metadata != null && metadata.Inherits)
                {
                    yield return dp;
                }
            }
        }

        private void InheritedValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsRegistered(GetType(), e.Property) && !_properties.ContainsKey(e.Property))
            {
                OnPropertyChanged(e);
            }
            else
            {
                foreach (DependencyObject child in VisualTreeHelper.GetChildren(this))
                {
                    child.InheritedValueChanged(e);
                }
            }
        }

        private void SetValueInternal(DependencyProperty dp, object oldValue, object newValue)
        {
            PropertyMetadata metadata = dp.GetMetadata(this);

            if (metadata.CoerceValueCallback != null)
            {
                newValue = metadata.CoerceValueCallback(this, newValue);
            }

            if (newValue != DependencyProperty.UnsetValue && dp.IsValidValue(newValue))
            {
                _properties[dp] = newValue;
            }
            else
            {
                _properties.Remove(dp);
                newValue = GetValue(dp);
            }

            if (!AreEqual(oldValue, newValue))
            {
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(dp, oldValue, newValue));
            }
        }
    }
}
