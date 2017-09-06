namespace Spark.Core.Collections
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a collection of two-dimensional keys and values.
    /// </summary>
    /// <remarks>
    /// This is similar to <see cref="Dictionary{TKey, TValue}"/> and thus inherits its properties (e.g. O(1) for lookup) and can be thought
    /// of as a "dictionary of dictionaries". It is compatible with the .NET interfaces by using <see cref="MultiKey{TMajorKey, TMinorKey}"/> as the
    /// singular key to map data. An example of usage would be for a 2D tile that has the XY integer coordinates as the key.
    /// </remarks>
    /// <typeparam name="TMajorKey">Major key type.</typeparam>
    /// <typeparam name="TMinorKey">Minor key type.</typeparam>
    /// <typeparam name="TValue">Data value type.</typeparam>
    public sealed class MultiKeyDictionary<TMajorKey, TMinorKey, TValue> : IDictionary<MultiKey<TMajorKey, TMinorKey>, TValue>, IReadOnlyDictionary<MultiKey<TMajorKey, TMinorKey>, TValue>
    {
        private int _version;
        private int _totalValueCount;
        private KeyCollection _keys;
        private ValueCollection _values;
        private List<TMajorKey> _tempMajorKeyList;
        private readonly IEqualityComparer<TMinorKey> _minorComparer;
        private readonly Dictionary<TMajorKey, Dictionary<TMinorKey, TValue>> _table;
        private readonly Queue<Dictionary<TMinorKey, TValue>> _freeSubDictionaries;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/> class.
        /// </summary>
        public MultiKeyDictionary() 
            : this(0, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/> class.
        /// </summary>
        /// <param name="capacity">Initial capacity of the dictionary.</param>
        public MultiKeyDictionary(int capacity) 
            : this(capacity, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/> class.
        /// </summary>
        /// <param name="majorComparer">Equality comparer for major keys. If null, <see cref="EqualityComparer{TMajorKey}.Default"/> is used.</param>
        /// <param name="minorComparer">Equality comparer for minor keys. If null, <see cref="EqualityComparer{TMinorKey}.Default"/> is used. </param>
        public MultiKeyDictionary(IEqualityComparer<TMajorKey> majorComparer, IEqualityComparer<TMinorKey> minorComparer) 
            : this(0, majorComparer, minorComparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/> class.
        /// </summary>
        /// <param name="capacity">Initial capacity of the dictionary.</param>
        /// <param name="majorComparer">Equality comparer for major keys. If null, <see cref="EqualityComparer{TMajorKey}.Default"/> is used.</param>
        /// <param name="minorComparer">Equality comparer for minor keys. If null, <see cref="EqualityComparer{TMinorKey}.Default"/> is used. </param>
        public MultiKeyDictionary(int capacity, IEqualityComparer<TMajorKey> majorComparer, IEqualityComparer<TMinorKey> minorComparer)
        {
            _table = new Dictionary<TMajorKey, Dictionary<TMinorKey, TValue>>(0, majorComparer);
            _freeSubDictionaries = new Queue<Dictionary<TMinorKey, TValue>>(0);
            _minorComparer = minorComparer ?? EqualityComparer<TMinorKey>.Default;
            _version = 0;
            _totalValueCount = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">Existing dictionary of key-value pairs to populate this collection with.</param>
        public MultiKeyDictionary(IDictionary<MultiKey<TMajorKey, TMinorKey>, TValue> dictionary) 
            : this(dictionary, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">Existing dictionary of key-value pairs to populate this collection with.</param>
        /// <param name="majorComparer">Equality comparer for major keys. If null, <see cref="EqualityComparer{TMajorKey}.Default"/> is used.</param>
        /// <param name="minorComparer">Equality comparer for minor keys. If null, <see cref="EqualityComparer{TMinorKey}.Default"/> is used. </param>
        public MultiKeyDictionary(IDictionary<MultiKey<TMajorKey, TMinorKey>, TValue> dictionary, IEqualityComparer<TMajorKey> majorComparer, IEqualityComparer<TMinorKey> minorComparer)
        {
            _table = new Dictionary<TMajorKey, Dictionary<TMinorKey, TValue>>(GetCountFromSeedDictionary(dictionary), majorComparer);
            _freeSubDictionaries = new Queue<Dictionary<TMinorKey, TValue>>(0);
            _minorComparer = minorComparer ?? EqualityComparer<TMinorKey>.Default;

            foreach (KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue> kv in dictionary)
            {
                TValue val = kv.Value;
                Insert(kv.Key, ref val, true);
            }

            _version = 0;
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        public TValue this[MultiKey<TMajorKey, TMinorKey> key]
        {
            get
            {
                if (_table.TryGetValue(key.Major, out Dictionary<TMinorKey, TValue> subDictionary))
                {
                    return subDictionary[key.Minor];
                }

                throw new KeyNotFoundException();
            }
            set => Insert(key, ref value, false);
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="majorKey">Major key component.</param>
        /// <param name="minorKey">Minor key component.</param>
        public TValue this[TMajorKey majorKey, TMinorKey minorKey]
        {
            get
            {
                if (_table.TryGetValue(majorKey, out Dictionary<TMinorKey, TValue> subDictionary))
                {
                    return subDictionary[minorKey];
                }

                throw new KeyNotFoundException();
            }
            set => Insert(new MultiKey<TMajorKey, TMinorKey>(majorKey, minorKey), ref value, false);
        }

        /// <summary>
        /// Gets the number of elements contained in the dictionary.
        /// </summary>
        public int Count => _totalValueCount;

        /// <summary>
        /// Gets the number of major keys present in the dictionary.
        /// </summary>
        public int TotalMajorKeyCount => _table.Count;

        /// <summary>
        /// Gets a value indicating whether the dictionary is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the comperer used for major key equality.
        /// </summary>
        public IEqualityComparer<TMajorKey> MajorKeyComparer => _table.Comparer;

        /// <summary>
        /// Gets the comparer used for minor key equality.
        /// </summary>
        public IEqualityComparer<TMinorKey> MinorKeyComparer => _minorComparer;

        /// <summary>
        /// Gets a collection containing the keys of the dictionary.
        /// </summary>
        public KeyCollection Keys => SafeGetKeys();

        /// <summary>
        /// Gets a collection containing the values of the dictionary.
        /// </summary>
        public ValueCollection Values => SafeGetValues();

        /// <summary>
        /// Gets a collection containing the keys of the dictionary.
        /// </summary>
        ICollection<MultiKey<TMajorKey, TMinorKey>> IDictionary<MultiKey<TMajorKey, TMinorKey>, TValue>.Keys => SafeGetKeys();

        /// <summary>
        /// Gets a collection containing the values of the dictionary.
        /// </summary>
        ICollection<TValue> IDictionary<MultiKey<TMajorKey, TMinorKey>, TValue>.Values => SafeGetValues();

        /// <summary>
        /// Gets a collection containing the keys of the dictionary.
        /// </summary>
        IEnumerable<MultiKey<TMajorKey, TMinorKey>> IReadOnlyDictionary<MultiKey<TMajorKey, TMinorKey>, TValue>.Keys => SafeGetKeys();

        /// <summary>
        /// Gets a collection containing the values of the dictionary.
        /// </summary>
        IEnumerable<TValue> IReadOnlyDictionary<MultiKey<TMajorKey, TMinorKey>, TValue>.Values => SafeGetValues();

        /// <summary>
        /// Adds the key-value pair to the dictionary.
        /// </summary>
        /// <param name="majorKey">Major key component.</param>
        /// <param name="minorKey">Minor key component.</param>
        /// <param name="value">Value to add.</param>
        public void Add(TMajorKey majorKey, TMinorKey minorKey, TValue value)
        {
            Insert(new MultiKey<TMajorKey, TMinorKey>(majorKey, minorKey), ref value, true);
        }

        /// <summary>
        /// Adds the key-value pair to the dictionary.
        /// </summary>
        /// <param name="key">Key representing the value.</param>
        /// <param name="value">Value to add.</param>
        public void Add(MultiKey<TMajorKey, TMinorKey> key, TValue value)
        {
            Insert(key, ref value, true);
        }

        /// <summary>
        /// Clears the dictionary of values.
        /// </summary>
        public void Clear()
        {
            foreach (KeyValuePair<TMajorKey, Dictionary<TMinorKey, TValue>> kv in _table)
            {
                kv.Value.Clear();
                FreeSubDictionary(kv.Value);
            }

            _totalValueCount = 0;
            _table.Clear();

            _version++;
        }

        /// <summary>
        /// Clears the dictionary of values associated with the major key.
        /// </summary>
        /// <param name="majorKey">Major key.</param>
        public void ClearMajorKeys(TMajorKey majorKey)
        {
            if (majorKey == null)
            {
                return;
            }
            
            if (_table.TryGetValue(majorKey, out Dictionary<TMinorKey, TValue> subDict))
            {
                subDict.Clear();
                FreeSubDictionary(subDict);

                _table.Remove(majorKey);
            }
        }

        /// <summary>
        /// Clears the dictionary of values associated with the minor key.
        /// </summary>
        /// <param name="minorKey">Minor key.</param>
        public void ClearMinorKeys(TMinorKey minorKey)
        {
            if (minorKey == null)
            {
                return;
            }

            // If we remove minor keys and the subdictionary count goes to zero, we need to remove the subdictionary
            foreach (KeyValuePair<TMajorKey, Dictionary<TMinorKey, TValue>> kv in _table)
            {
                if (kv.Value.Remove(minorKey) && kv.Value.Count == 0)
                {
                    if (_tempMajorKeyList == null)
                    {
                        _tempMajorKeyList = new List<TMajorKey>(1);
                    }

                    _tempMajorKeyList.Add(kv.Key);
                    FreeSubDictionary(kv.Value);
                }
            }

            if (_tempMajorKeyList != null)
            {
                for (int i = 0; i < _tempMajorKeyList.Count; i++)
                {
                    _table.Remove(_tempMajorKeyList[i]);
                }

                _tempMajorKeyList.Clear();
            }
        }

        /// <summary>
        /// Determines if the major/minor keys are contained in the dictionary.
        /// </summary>
        /// <param name="majorKey">Major key component.</param>
        /// <param name="minorKey">Minor key component.</param>
        /// <returns>True if the key was contained, false if not.</returns>
        public bool ContainsKey(TMajorKey majorKey, TMinorKey minorKey)
        {
            return ContainsKey(new MultiKey<TMajorKey, TMinorKey>(majorKey, minorKey));
        }

        /// <summary>
        /// Determines if the key is contained in the dictionary.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if the key was contained, false if not.</returns>
        public bool ContainsKey(MultiKey<TMajorKey, TMinorKey> key)
        {
            if (_table.TryGetValue(key.Major, out Dictionary<TMinorKey, TValue> subDictionary))
            {
                return subDictionary.ContainsKey(key.Minor);
            }

            return false;
        }

        /// <summary>
        /// Determines if the value is contained in the dictionary.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if the value was contained, false if not.</returns>
        public bool ContainsValue(TValue value)
        {
            foreach (KeyValuePair<TMajorKey, Dictionary<TMinorKey, TValue>> kv in _table)
            {
                if (kv.Value.ContainsValue(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the value with the specified major/minor keys from the dictionary.
        /// </summary>
        /// <param name="majorKey">Major key component.</param>
        /// <param name="minorKey">Minor key component.</param>
        /// <returns>True if the key was found and the value removed, false if it was not.</returns>
        public bool Remove(TMajorKey majorKey, TMinorKey minorKey)
        {
            return Remove(new MultiKey<TMajorKey, TMinorKey>(majorKey, minorKey));
        }

        /// <summary>
        /// Removes the value with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">Key of the value to remove.</param>
        /// <returns>True if the key was found and the value removed, false if it was not.</returns>
        public bool Remove(MultiKey<TMajorKey, TMinorKey> key)
        {
            if (_table.TryGetValue(key.Major, out Dictionary<TMinorKey, TValue> subDictionary) && subDictionary.Remove(key.Minor))
            {
                _totalValueCount--;

                // If no more values, remove from the major key table, but put it back into our pool for future use
                if (subDictionary.Count == 0)
                {
                    _table.Remove(key.Major);
                    FreeSubDictionary(subDictionary);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified major/minor keys.
        /// </summary>
        /// <param name="majorKey">Major key component.</param>
        /// <param name="minorKey">Minor key component.</param>
        /// <param name="value">Value contained in the dictionary, if not found then default.</param>
        /// <returns>True if the key was found in the dictionary, false if it was not.</returns>
        public bool TryGetValue(TMajorKey majorKey, TMinorKey minorKey, out TValue value)
        {
            return TryGetValue(new MultiKey<TMajorKey, TMinorKey>(majorKey, minorKey), out value);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">Key of the value.</param>
        /// <param name="value">Value contained in the dictionary, if not found then default.</param>
        /// <returns>True if the key was found in the dictionary, false if it was not.</returns>
        public bool TryGetValue(MultiKey<TMajorKey, TMinorKey> key, out TValue value)
        {
            if (_table.TryGetValue(key.Major, out Dictionary<TMinorKey, TValue> subDictionary))
            {
                return subDictionary.TryGetValue(key.Minor, out value);
            }

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Queries for the total count of values associated with the minor key.
        /// </summary>
        /// <param name="minorKey">Minor key to query with.</param>
        /// <returns>The number of values associated with the minor key.</returns>
        public int QueryMajorKeyCount(TMinorKey minorKey)
        {
            if (_table.Count == 0 || minorKey == null)
            {
                return 0;
            }
            
            return _table.Count(kv => kv.Value.ContainsKey(minorKey));
        }

        /// <summary>
        /// Queries for the total count of values associated with the major key.
        /// </summary>
        /// <param name="majorKey">Major key to query with.</param>
        /// <returns>The number of values associated with the major key.</returns>
        public int QueryMinorKeyCount(TMajorKey majorKey)
        {
            if (_table.Count == 0 || majorKey == null)
            {
                return 0;
            }

            if (_table.TryGetValue(majorKey, out Dictionary<TMinorKey, TValue> subdict))
            {
                return subdict.Count;
            }

            return 0;
        }

        /// <summary>
        /// Queries all values in the dictionary that match the specified major key.
        /// </summary>
        /// <param name="majorKey">Major key to query with.</param>
        /// <param name="keyValuePairs">List of key-value pairs that match the major key. If null is passed, a list will be created.</param>
        /// <returns>True if at least one value is found with the matching major key, false if no values are returned.</returns>
        public bool QueryValuesWithMajorKey(TMajorKey majorKey, ref List<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>> keyValuePairs)
        {
            if (_table.Count == 0 || majorKey == null)
            {
                return false;
            }

            if (keyValuePairs == null)
            {
                keyValuePairs = new List<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>>();
            }

            MinorKeyValueEnumerator enumerator = GetMinorKeyValueEnumerator(majorKey);
            bool foundOne = false;

            while (enumerator.MoveNext())
            {
                foundOne = true;
                KeyValuePair<TMinorKey, TValue> kv = enumerator.Current;
                keyValuePairs.Add(new KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>(new MultiKey<TMajorKey, TMinorKey>(majorKey, kv.Key), kv.Value));
            }

            return foundOne;
        }

        /// <summary>
        /// Queries all values in the dictionary that match the specified minor key.
        /// </summary>
        /// <param name="minorKey">Minor key to query with.</param>
        /// <param name="keyValuePairs">List of key-value pairs that match the minor key. If null is passed, a list will be created.</param>
        /// <returns>True if at least one value is found with the matching minor key, false if no values are returned.</returns>
        public bool QueryValuesWithMinorKey(TMinorKey minorKey, ref List<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>> keyValuePairs)
        {
            if (_table.Count == 0 || minorKey == null)
            {
                return false;
            }

            if (keyValuePairs == null)
            {
                keyValuePairs = new List<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>>();
            }

            MajorKeyValueEnumerator enumerator = GetMajorKeyValueEnumerator(minorKey);
            bool foundOne = false;

            while (enumerator.MoveNext())
            {
                foundOne = true;
                KeyValuePair<TMajorKey, TValue> kv = enumerator.Current;
                keyValuePairs.Add(new KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>(new MultiKey<TMajorKey, TMinorKey>(kv.Key, minorKey), kv.Value));
            }

            return foundOne;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection, filtering on the specified minor key.
        /// </summary>
        /// <param name="minorKey">Minor key to query with.</param>
        /// <returns>An enumerator that iterates over values that have the associated minor key.</returns>
        public MajorKeyValueEnumerator GetMajorKeyValueEnumerator(TMinorKey minorKey)
        {
            return new MajorKeyValueEnumerator(minorKey, this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection, filtering on the specified major key.
        /// </summary>
        /// <param name="majorKey">Major key to query with.</param>
        /// <returns>An enumerator that iterates over values that have the associated major key.</returns>
        public MinorKeyValueEnumerator GetMinorKeyValueEnumerator(TMajorKey majorKey)
        {
            return new MinorKeyValueEnumerator(majorKey, this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>> IEnumerable<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Adds the specified item to the collection.
        /// </summary>
        /// <param name="item">Dictionary item to add.</param>
        void ICollection<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>>.Add(KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue> item)
        {
            TValue val = item.Value;
            Insert(item.Key, ref val, true);
        }

        /// <summary>
        /// Copies the contents of the collection to the array.
        /// </summary>
        /// <param name="array">Array of dictionary items.</param>
        /// <param name="arrayIndex">Starting position to write to the array.</param>
        public void CopyTo(KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index is out of range");
            }

            if ((array.Length - arrayIndex) < _totalValueCount)
            {
                throw new ArgumentException("Buffer write overflow");
            }

            int currentIndex = arrayIndex;
            foreach (KeyValuePair<TMajorKey, Dictionary<TMinorKey, TValue>> kv in _table)
            {
                Dictionary<TMinorKey, TValue> subDictionary = kv.Value;
                foreach (KeyValuePair<TMinorKey, TValue> subKv in subDictionary)
                {
                    array[currentIndex++] = new KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>(new MultiKey<TMajorKey, TMinorKey>(kv.Key, subKv.Key), subKv.Value);
                }
            }
        }

        /// <summary>
        /// Determines if the item is contained in the collection.
        /// </summary>
        /// <param name="item">Dictionary item to check.</param>
        /// <returns>True if the item is contained, false otherwise.</returns>
        public bool Contains(KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue> item)
        {
            if (_table.TryGetValue(item.Key.Major, out Dictionary<TMinorKey, TValue> subDictionary) && 
                subDictionary.TryGetValue(item.Key.Minor, out TValue value))
            {
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            }

            return false;
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="item">Dictionary item to remove.</param>
        /// <returns>True if the item was successfully removed, false otherwise.</returns>
        bool ICollection<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>>.Remove(KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Inserts a new element or replaces the value of an old one
        /// </summary>
        /// <param name="key">Element key</param>
        /// <param name="value">Element value</param>
        /// <param name="add">True if the value should be added, false if it should be replaced</param>
        private void Insert(MultiKey<TMajorKey, TMinorKey> key, ref TValue value, bool add)
        {
            _table.TryGetValue(key.Major, out Dictionary<TMinorKey, TValue> subDictionary);

            // Always add the dictionary for any inserts if it hasn't been created yet
            if (subDictionary == null)
            {
                subDictionary = GetNextFreeSubDictionary();
                _table.Add(key.Major, subDictionary);
            }

            int prevCount = subDictionary.Count;

            // Follow Dictionary conventions, if a key is present on an add operation an exception occurs,
            // but if not then we want to either add or replace
            if (add)
            {
                subDictionary.Add(key.Minor, value);
            }
            else
            {
                subDictionary[key.Minor] = value;
            }

            // Increment if we added the value
            if (prevCount != subDictionary.Count)
            {
                _totalValueCount++;
            }

            _version++;
        }

        /// <summary>
        /// Gets the total number of major keys within a dictionary
        /// </summary>
        /// <param name="dict">Dictionary instance</param>
        /// <returns>Number of major keys within the dictionary</returns>
        private int GetCountFromSeedDictionary(IDictionary<MultiKey<TMajorKey, TMinorKey>, TValue> dict)
        {
            if (dict is MultiKeyDictionary<TMajorKey, TMinorKey, TValue> mkDict)
            {
                return mkDict.TotalMajorKeyCount;
            }

            return (dict != null) ? dict.Count : 0;
        }

        /// <summary>
        /// Gets the next freed sub dictionary instance
        /// </summary>
        /// <returns>Sub dictionary instance</returns>
        private Dictionary<TMinorKey, TValue> GetNextFreeSubDictionary()
        {
            if (_freeSubDictionaries.Count > 0)
            {
                return _freeSubDictionaries.Dequeue();
            }

            return new Dictionary<TMinorKey, TValue>(1, _minorComparer);
        }

        /// <summary>
        /// Releases a sub dictionary instance
        /// </summary>
        /// <param name="subDictionary">Sub dictionary</param>
        private void FreeSubDictionary(Dictionary<TMinorKey, TValue> subDictionary)
        {
            _freeSubDictionaries.Enqueue(subDictionary);
        }

        /// <summary>
        /// Gets the current key collection instance
        /// </summary>
        /// <returns>Key collection instance</returns>
        private KeyCollection SafeGetKeys()
        {
            if (_keys == null)
            {
                _keys = new KeyCollection(this);
            }

            return _keys;
        }

        /// <summary>
        /// Gets the current values collection instance
        /// </summary>
        /// <returns>Value collection instance</returns>
        private ValueCollection SafeGetValues()
        {
            if (_values == null)
            {
                _values = new ValueCollection(this);
            }

            return _values;
        }
        
        /// <summary>
        /// Enumerator for <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/> that enumerates all
        /// values in the dictionary associated with a certain minor key.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MajorKeyValueEnumerator : IEnumerator<KeyValuePair<TMajorKey, TValue>>
        {
            private KeyValuePair<TMajorKey, TValue> _current;
            private readonly MultiKeyDictionary<TMajorKey, TMinorKey, TValue> _dictionary;
            private readonly Enumerator _enumerator;
            private readonly TMinorKey _minorKey;

            /// <summary>
            /// Initializes a new instance of the <see cref="MajorKeyValueEnumerator"/> struct
            /// </summary>
            /// <param name="minorKey">Minor key</param>
            /// <param name="mkDictionary">Parent dictionary</param>
            internal MajorKeyValueEnumerator(TMinorKey minorKey, MultiKeyDictionary<TMajorKey, TMinorKey, TValue> mkDictionary)
            {
                _dictionary = mkDictionary;
                _enumerator = mkDictionary.GetEnumerator();
                _current = new KeyValuePair<TMajorKey, TValue>();
                _minorKey = minorKey;
            }

            /// <summary>
            /// Gets the current enumerated key-value pair.
            /// </summary>
            public KeyValuePair<TMajorKey, TValue> Current => _current;

            /// <summary>
            /// Gets the current enumerated key-value pair.
            /// </summary>
            object IEnumerator.Current => _current;

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue> kv = _enumerator.Current;
                    if (_dictionary._minorComparer.Equals(kv.Key.Minor, _minorKey))
                    {
                        _current = new KeyValuePair<TMajorKey, TValue>(kv.Key.Major, kv.Value);
                        return true;
                    }
                }
                
                _current = new KeyValuePair<TMajorKey, TValue>();
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                _enumerator.Reset();
                _current = new KeyValuePair<TMajorKey, TValue>();
            }

            /// <summary>
            /// Not used.
            /// </summary>
            public void Dispose()
            {
                // No-op
            }
        }

        /// <summary>
        /// Enumerator for <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/> that enumerates all
        /// values in the dictionary associated with a certain major key.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MinorKeyValueEnumerator : IEnumerator<KeyValuePair<TMinorKey, TValue>>
        {
            private bool _isValid;
            private Dictionary<TMinorKey, TValue>.Enumerator _subDictEnumerator;
            private readonly MultiKeyDictionary<TMajorKey, TMinorKey, TValue> _dictionary;
            private readonly TMajorKey _majorKey;

            /// <summary>
            /// Initializes a new instance of the <see cref="MinorKeyValueEnumerator"/> struct
            /// </summary>
            /// <param name="majorKey">Major key</param>
            /// <param name="mkDictionary">Parent dictionary</param>
            internal MinorKeyValueEnumerator(TMajorKey majorKey, MultiKeyDictionary<TMajorKey, TMinorKey, TValue> mkDictionary)
            {
                _majorKey = majorKey;
                _dictionary = mkDictionary;                
                _isValid = mkDictionary._table.TryGetValue(majorKey, out Dictionary<TMinorKey, TValue> subDict);

                if (_isValid)
                {
                    _subDictEnumerator = subDict.GetEnumerator();
                }
                else
                {
                    _subDictEnumerator = new Dictionary<TMinorKey, TValue>.Enumerator();
                }
            }

            /// <summary>
            /// Gets the current enumerated key-value pair.
            /// </summary>
            public KeyValuePair<TMinorKey, TValue> Current
            {
                get
                {
                    if (!_isValid)
                    {
                        return new KeyValuePair<TMinorKey, TValue>();
                    }

                    return _subDictEnumerator.Current;
                }
            }

            /// <summary>
            /// Gets the current enumerated key-value pair.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (!_isValid)
                    {
                        return new KeyValuePair<TMinorKey, TValue>();
                    }

                    return _subDictEnumerator.Current;
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                if (!_isValid)
                {
                    return false;
                }

                return _subDictEnumerator.MoveNext();
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                if (!_isValid)
                {
                    return;
                }
                
                _isValid = _dictionary._table.TryGetValue(_majorKey, out Dictionary<TMinorKey, TValue> subDict);

                if (_isValid)
                {
                    _subDictEnumerator = subDict.GetEnumerator();
                }
                else
                {
                    _subDictEnumerator = new Dictionary<TMinorKey, TValue>.Enumerator();
                }
            }

            /// <summary>
            /// Not used.
            /// </summary>
            public void Dispose()
            {
                // No-op
            }
        }

        /// <summary>
        /// Enumerator for <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>>
        {
            private readonly int _version;
            private readonly MultiKeyDictionary<TMajorKey, TMinorKey, TValue> _dictionary;
            private Dictionary<TMajorKey, Dictionary<TMinorKey, TValue>>.Enumerator _tableEnumerator;
            private Dictionary<TMinorKey, TValue>.Enumerator _currentSubDictEnumerator;
            private bool _hasTableEnum;
            private bool _hasCurrentSubDictEnum;
            private KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue> _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct
            /// </summary>
            /// <param name="mkDictionary">Parent dictionary</param>
            internal Enumerator(MultiKeyDictionary<TMajorKey, TMinorKey, TValue> mkDictionary)
            {
                _dictionary = mkDictionary;
                _tableEnumerator = mkDictionary._table.GetEnumerator();
                _currentSubDictEnumerator = new Dictionary<TMinorKey, TValue>.Enumerator();
                _hasTableEnum = true;
                _hasCurrentSubDictEnum = false;
                _version = mkDictionary._version;

                _current = new KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>();
            }

            /// <summary>
            /// Gets the current enumerated key-value pair.
            /// </summary>
            public KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue> Current => _current;

            /// <summary>
            /// Gets the current enumerated key-value pair.
            /// </summary>
            object IEnumerator.Current => _current;

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                ThrowIfChanged();

                // If table enumerator null, we hit the end of the enumeration and provide an early out. Reset needs to be called
                if (!_hasTableEnum)
                {
                    return false;
                }

                // If subdict enumerator null already, we are at the start of the enumeration
                if (!_hasCurrentSubDictEnum)
                {
                    return AdvanceToNextSubDictionaryEnumerator();
                }

                // Else we have a valid subdict enumerator
                if (_currentSubDictEnumerator.MoveNext())
                {
                    SetCurrent();
                    return true;
                }
                else
                {
                    // End of the sub dict collection, take a look at the next one, if applicable.
                    _currentSubDictEnumerator = new Dictionary<TMinorKey, TValue>.Enumerator();
                    _hasCurrentSubDictEnum = false;
                    return AdvanceToNextSubDictionaryEnumerator();
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                ThrowIfChanged();

                _tableEnumerator = _dictionary._table.GetEnumerator();
                _currentSubDictEnumerator = new Dictionary<TMinorKey, TValue>.Enumerator();
                _hasTableEnum = true;
                _hasCurrentSubDictEnum = false;

                _current = new KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>();
            }

            /// <summary>
            /// Not used.
            /// </summary>
            public void Dispose()
            {
                // No-op
            }

            /// <summary>
            /// Advances to the next sub dictionary enumeration
            /// </summary>
            /// <returns>True if the advance was successful, false if there is no subsequent element</returns>
            private bool AdvanceToNextSubDictionaryEnumerator()
            {
                while (_hasTableEnum && !_hasCurrentSubDictEnum)
                {
                    if (_tableEnumerator.MoveNext())
                    {
                        //If we found a sub dictionary, take a look
                        Dictionary<TMinorKey, TValue>.Enumerator subDictEnum = _tableEnumerator.Current.Value.GetEnumerator();

                        //If it has items, then move to the next and set it as the current enumerator
                        if (subDictEnum.MoveNext())
                        {
                            //Found a sub dict with a collection not empty
                            _currentSubDictEnumerator = subDictEnum;
                            _hasCurrentSubDictEnum = true;
                            SetCurrent();

                            return true;
                        }
                        else
                        {
                            //Else empty, keep looking at table enumerator
                            _currentSubDictEnumerator = new Dictionary<TMinorKey, TValue>.Enumerator();
                            _hasCurrentSubDictEnum = false;
                        }
                    }
                    else
                    {
                        //If no more items in table, then we hit the end of the our enumeration
                        SetCurrent(true);
                        _currentSubDictEnumerator = new Dictionary<TMinorKey, TValue>.Enumerator();
                        _tableEnumerator = new Dictionary<TMajorKey, Dictionary<TMinorKey, TValue>>.Enumerator();
                        _hasCurrentSubDictEnum = false;
                        _hasTableEnum = false;
                        return false;
                    }
                }

                return false;
            }

            /// <summary>
            /// Sets the current element
            /// </summary>
            /// <param name="nullOut">True if the value should be set to null, false if we should continue</param>
            private void SetCurrent(bool nullOut = false)
            {
                if (nullOut)
                {
                    _current = new KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>();
                    return;
                }

                KeyValuePair<TMinorKey, TValue> minorKv = _currentSubDictEnumerator.Current;
                _current = new KeyValuePair<MultiKey<TMajorKey, TMinorKey>, TValue>(new MultiKey<TMajorKey, TMinorKey>(_tableEnumerator.Current.Key, minorKv.Key), minorKv.Value);
            }

            /// <summary>
            /// Throws an exception if the parent dictionary has been changed
            /// </summary>
            private void ThrowIfChanged()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException("Collection modified during enumeration");
                }
            }
        }
                
        /// <summary>
        /// Represents the collection of keys in a <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/>.
        /// </summary>
        public sealed class KeyCollection : ICollection<MultiKey<TMajorKey, TMinorKey>>, IEnumerable<MultiKey<TMajorKey, TMinorKey>>, IReadOnlyCollection<MultiKey<TMajorKey, TMinorKey>>
        {
            private readonly MultiKeyDictionary<TMajorKey, TMinorKey, TValue> _dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection"/> class
            /// </summary>
            /// <param name="dictionary">Parent dictionary</param>
            internal KeyCollection(MultiKeyDictionary<TMajorKey, TMinorKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            /// <summary>
            /// Gets the number of keys in the dictionary.
            /// </summary>
            public int Count => _dictionary.Count;

            /// <summary>
            /// Gets if this collection is read only. This will always return true.
            /// </summary>
            public bool IsReadOnly => true;

            /// <summary>
            /// Copies the contents of the collection to the array.
            /// </summary>
            /// <param name="array">Array to copy data to.</param>
            /// <param name="arrayIndex">Zero-based starting index in the array at which to start copying to.</param>
            public void CopyTo(MultiKey<TMajorKey, TMinorKey>[] array, int arrayIndex)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (arrayIndex < 0 || arrayIndex > array.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index is out of range");
                }

                if ((array.Length - arrayIndex) < _dictionary.Count)
                {
                    throw new ArgumentException("Write buffer overflow");
                }

                int currentIndex = arrayIndex;
                foreach (KeyValuePair<TMajorKey, Dictionary<TMinorKey, TValue>> kv in _dictionary._table)
                {
                    Dictionary<TMinorKey, TValue> subDictionary = kv.Value;
                    foreach (KeyValuePair<TMinorKey, TValue> subKv in subDictionary)
                    {
                        array[currentIndex++] = new MultiKey<TMajorKey, TMinorKey>(kv.Key, subKv.Key);
                    }
                }
            }
            
            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            IEnumerator<MultiKey<TMajorKey, TMinorKey>> IEnumerable<MultiKey<TMajorKey, TMinorKey>>.GetEnumerator()
            {
                foreach (KeyValuePair<TMajorKey, Dictionary<TMinorKey, TValue>> majorKeys in _dictionary._table)
                {
                    foreach (TMinorKey minorKeys in majorKeys.Value.Keys)
                    {
                        yield return new MultiKey<TMajorKey, TMinorKey>(majorKeys.Key, minorKeys);
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (KeyValuePair<TMajorKey, Dictionary<TMinorKey, TValue>> majorKeys in _dictionary._table)
                {
                    foreach (TMinorKey minorKeys in majorKeys.Value.Keys)
                    {
                        yield return new MultiKey<TMajorKey, TMinorKey>(majorKeys.Key, minorKeys);
                    }
                }
            }

            /// <summary>
            /// Not Supported.
            /// </summary>
            void ICollection<MultiKey<TMajorKey, TMinorKey>>.Add(MultiKey<TMajorKey, TMinorKey> item)
            {
                throw new NotSupportedException("Collection is read only");
            }

            /// <summary>
            /// Not Supported.
            /// </summary>
            void ICollection<MultiKey<TMajorKey, TMinorKey>>.Clear()
            {
                throw new NotSupportedException("Collection is read only");
            }

            /// <summary>
            /// Not Supported.
            /// </summary>
            bool ICollection<MultiKey<TMajorKey, TMinorKey>>.Remove(MultiKey<TMajorKey, TMinorKey> item)
            {
                throw new NotSupportedException("Collection is read only");
            }

            /// <summary>
            /// Determines if the value is contained in the collection.
            /// </summary>
            /// <param name="item">Item to check.</param>
            /// <returns>True if the value is in the collection, false if not.</returns>
            bool ICollection<MultiKey<TMajorKey, TMinorKey>>.Contains(MultiKey<TMajorKey, TMinorKey> item)
            {
                return _dictionary.ContainsKey(item);
            }
        }
                
        /// <summary>
        /// Represents the collection of values in the <see cref="MultiKeyDictionary{TMajorKey, TMinorKey, TValue}"/>.
        /// </summary>
        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IReadOnlyCollection<TValue>
        {
            private readonly MultiKeyDictionary<TMajorKey, TMinorKey, TValue> _dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueCollection"/> class
            /// </summary>
            /// <param name="dictionary">Parent dictionary</param>
            internal ValueCollection(MultiKeyDictionary<TMajorKey, TMinorKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            /// <summary>
            /// Gets the number of values in the dictionary.
            /// </summary>
            public int Count => _dictionary.Count;

            /// <summary>
            /// Gets if this collection is read only. This will always return true.
            /// </summary>
            public bool IsReadOnly => true;

            /// <summary>
            /// Copies the contents of the collection to the array.
            /// </summary>
            /// <param name="array">Array to copy data to.</param>
            /// <param name="arrayIndex">Zero-based starting index in the array at which to start copying to.</param>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (arrayIndex < 0 || arrayIndex > array.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index is out of range");
                }

                if ((array.Length - arrayIndex) < _dictionary.Count)
                {
                    throw new ArgumentException("Write buffer overflow");
                }

                int currentIndex = arrayIndex;
                foreach (KeyValuePair<TMajorKey, Dictionary<TMinorKey, TValue>> kv in _dictionary._table)
                {
                    Dictionary<TMinorKey, TValue> subDictionary = kv.Value;
                    foreach (KeyValuePair<TMinorKey, TValue> subKv in subDictionary)
                    {
                        array[currentIndex++] = subKv.Value;
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                foreach (Dictionary<TMinorKey, TValue> minorKeyDictionary in _dictionary._table.Values)
                {
                    foreach (KeyValuePair<TMinorKey, TValue> value in minorKeyDictionary)
                    {
                        yield return value.Value;
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (Dictionary<TMinorKey, TValue> minorKeyDictionary in _dictionary._table.Values)
                {
                    foreach (KeyValuePair<TMinorKey, TValue> value in minorKeyDictionary)
                    {
                        yield return value.Value;
                    }
                }
            }

            /// <summary>
            /// Not Supported.
            /// </summary>
            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException("Collection is read only");
            }

            /// <summary>
            /// Not Supported.
            /// </summary>
            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("Collection is read only");
            }

            /// <summary>
            /// Not Supported.
            /// </summary>
            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException("Collection is read only");
            }

            /// <summary>
            /// Determines if the value is contained in the collection.
            /// </summary>
            /// <param name="item">Item to check.</param>
            /// <returns>True if the value is in the collection, false if not.</returns>
            bool ICollection<TValue>.Contains(TValue item)
            {
                return _dictionary.ContainsValue(item);
            }
        }
    }
}
