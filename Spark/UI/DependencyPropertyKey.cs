namespace Spark.UI
{
    using System;

    public sealed class DependencyPropertyKey
    {
        private readonly DependencyProperty _dependencyProperty;

        internal DependencyPropertyKey(DependencyProperty dependencyProperty)
        {
            _dependencyProperty = dependencyProperty;
        }

        public DependencyProperty DependencyProperty => _dependencyProperty;

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
        {
            _dependencyProperty.OverrideMetadata(forType, typeMetadata, this);
        }
    }
}
