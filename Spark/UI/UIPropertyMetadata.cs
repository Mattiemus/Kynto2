namespace Spark.UI
{
    public delegate object CoerceValueCallback(DependencyObject d, object baseValue);

    public class UIPropertyMetadata : PropertyMetadata
    {
        private bool _isAnimationProhibited;

        public UIPropertyMetadata()
        {
        }

        public UIPropertyMetadata(object defaultValue)
            : base(defaultValue)
        {
        }

        public UIPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
            : base(propertyChangedCallback)
        {
        }

        public UIPropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback)
        {
        }

        public UIPropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
        }

        public UIPropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback,
            bool isAnimationProhibited)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
            _isAnimationProhibited = isAnimationProhibited;
        }

        public bool IsAnimationProhibited
        {
            get => _isAnimationProhibited;
            set => _isAnimationProhibited = value;
        }
    }
}
