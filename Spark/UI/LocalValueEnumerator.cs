namespace Spark.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public struct LocalValueEnumerator : IEnumerator
    {
        private readonly int _count;
        private readonly Dictionary<DependencyProperty, object> _properties;
        private readonly IDictionaryEnumerator _propertyEnumerator;

        internal LocalValueEnumerator(Dictionary<DependencyProperty, object> properties)
        {
            _count = properties.Count;
            _properties = properties;
            _propertyEnumerator = properties.GetEnumerator();
        }
        
        public int Count => _count;

        object IEnumerator.Current => Current;

        public LocalValueEntry Current 
            => new LocalValueEntry(
                (DependencyProperty)_propertyEnumerator.Key,
                _propertyEnumerator.Value);

        public bool MoveNext()
        {
            return _propertyEnumerator.MoveNext();
        }

        public void Reset()
        {
            _propertyEnumerator.Reset();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(LocalValueEnumerator obj1, LocalValueEnumerator obj2)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(LocalValueEnumerator obj1, LocalValueEnumerator obj2)
        {
            throw new NotImplementedException();
        }
    }
}
