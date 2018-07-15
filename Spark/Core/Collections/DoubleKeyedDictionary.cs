namespace Spark.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class DoubleKeyedDictionary<K1, K2> : IEnumerable<KeyValuePair<K1, K2>>
    {
        private readonly Dictionary<K1, K2> _forwards;
        private readonly Dictionary<K2, K1> _backwards;

        public DoubleKeyedDictionary()
        {
            _forwards = new Dictionary<K1, K2>();
            _backwards = new Dictionary<K2, K1>();
        }

        public K1 this[K2 key] => _backwards[key];

        public K2 this[K1 key] => _forwards[key];

        public void Add(K1 key1, K2 key2)
        {
            Add(key1, key2, false);
        }

        public void Add(K1 key1, K2 key2, bool ignoreExisting)
        {
            if (!ignoreExisting && (_forwards.ContainsKey(key1) || _backwards.ContainsKey(key2)))
            {
                throw new InvalidOperationException("Dictionary already contains this key pair");
            }

            _forwards[key1] = key2;
            _backwards[key2] = key1;
        }

        public void Clear()
        {
            _forwards.Clear();
            _backwards.Clear();
        }

        public void Remove(K1 key1, K2 key2)
        {
            if (!_forwards.ContainsKey(key1) || !_backwards.ContainsKey(key2))
            {
                throw new InvalidOperationException("Dictionary does not contain this key pair");
            }

            _forwards.Remove(key1);
            _backwards.Remove(key2);
        }

        public void Remove(K1 key1, K2 key2, bool ignoreExisting)
        {
            if (!ignoreExisting && (!_forwards.ContainsKey(key1) || !_backwards.ContainsKey(key2)))
            {
                throw new InvalidOperationException("Dictionary does not contain this key pair");
            }

            _forwards.Remove(key1);
            _backwards.Remove(key2);
        }

        public bool TryMap(K1 key, out K2 value)
        {
            return _forwards.TryGetValue(key, out value);
        }

        public bool TryMap(K2 key, out K1 value)
        {
            return _backwards.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<K1, K2>> GetEnumerator()
        {
            return _forwards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
