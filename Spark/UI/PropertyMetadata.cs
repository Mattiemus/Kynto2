namespace Spark.UI
{
    using System;

    public delegate void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e);

    public class PropertyMetadata
    {
        private object _defaultValue;
        private bool _isSealed;
        private PropertyChangedCallback _propertyChangedCallback;
        private CoerceValueCallback _coerceValueCallback;

        public PropertyMetadata()
            : this(null, null, null)
        {
        }

        public PropertyMetadata(object defaultValue)
            : this(defaultValue, null, null)
        {
        }

        public PropertyMetadata(PropertyChangedCallback propertyChangedCallback)
            : this(null, propertyChangedCallback, null)
        {
        }

        public PropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
            : this(defaultValue, propertyChangedCallback, null)
        {
        }

        public PropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
        {
            CheckNotUnset(defaultValue);

            _defaultValue = defaultValue;
            _propertyChangedCallback = propertyChangedCallback;
            _coerceValueCallback = coerceValueCallback;
        }

        public object DefaultValue
        {
            get => _defaultValue;
            set
            {
                CheckNotSealed();
                CheckNotUnset(value);
                _defaultValue = value;
            }
        }

        public PropertyChangedCallback PropertyChangedCallback
        {
            get => _propertyChangedCallback;
            set
            {
                CheckNotSealed();
                _propertyChangedCallback = value;
            }
        }

        public CoerceValueCallback CoerceValueCallback
        {
            get => _coerceValueCallback;
            set
            {
                CheckNotSealed();
                _coerceValueCallback = value;
            }
        }

        protected bool IsSealed => _isSealed;

        internal void Merge(PropertyMetadata baseMetadata, DependencyProperty dp, Type targetType)
        {
            Merge(baseMetadata, dp);
            OnApply(dp, targetType);
            _isSealed = true;
        }

        protected void CheckNotSealed()
        {
            if (IsSealed)
            {
                throw new InvalidOperationException("Cannot change metadata once it has been applied to a property");
            }
        }

        protected virtual void Merge(PropertyMetadata baseMetadata, DependencyProperty dp)
        {
            if (_defaultValue == null)
            {
                _defaultValue = baseMetadata._defaultValue;
            }

            if (_propertyChangedCallback == null)
            {
                _propertyChangedCallback = baseMetadata._propertyChangedCallback;
            }

            if (_coerceValueCallback == null)
            {
                _coerceValueCallback = baseMetadata._coerceValueCallback;
            }
        }

        protected virtual void OnApply(DependencyProperty dp, Type targetType)
        {
        }

        private void CheckNotUnset(object value)
        {
            if (value == DependencyProperty.UnsetValue)
            {
                throw new ArgumentException("Cannot set property metadata's default value to 'Unset'", nameof(value));
            }
        }
    }
}
