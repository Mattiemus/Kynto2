namespace Spark.UI
{
    using System;

    public struct LocalValueEntry
    {
        private readonly DependencyProperty _property;
        private readonly object _value;

        internal LocalValueEntry(DependencyProperty property, object value)
        {
            _property = property;
            _value = value;
        }

        public DependencyProperty Property => _property;

        public object Value => _value;

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(LocalValueEntry obj1, LocalValueEntry obj2)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(LocalValueEntry obj1, LocalValueEntry obj2)
        {
            throw new NotImplementedException();
        }
    }
}
