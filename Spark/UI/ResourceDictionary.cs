namespace Spark.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Markup;

    [Ambient]
    public class ResourceDictionary : IDictionary, INameScope
    {
        private readonly NameScope _nameScope;
        private readonly Dictionary<object, object> _resources;

        public ResourceDictionary()
        {
            _nameScope = new NameScope();
            _resources = new Dictionary<object, object>();
        }

        bool ICollection.IsSynchronized
        {
            get { throw new System.NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new System.NotImplementedException(); }
        }

        bool IDictionary.IsFixedSize
        {
            get { throw new System.NotImplementedException(); }
        }

        bool IDictionary.IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        public int Count => _resources.Count;

        public ICollection Keys => _resources.Keys;

        public ICollection Values => _resources.Values;

        public object this[object key]
        {
            get
            {
                _resources.TryGetValue(key, out object result);
                return result;
            }
            set => _resources[key] = value;
        }

        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(object key, object value)
        {
            _resources.Add(key, value);
        }

        public void Clear()
        {
            _resources.Clear();
        }

        public bool Contains(object key)
        {
            return _resources.ContainsKey(key);
        }

        public object FindName(string name)
        {
            return _nameScope.FindName(name);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        public void RegisterName(string name, object scopedElement)
        {
            _nameScope.RegisterName(name, scopedElement);
        }

        public void Remove(object key)
        {
            _resources.Remove(key);
        }

        public void UnregisterName(string name)
        {
            _nameScope.UnregisterName(name);
        }
    }
}
