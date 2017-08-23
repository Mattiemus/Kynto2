namespace Spark.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a readonly collection of named objects that allows for fast look up by name, but acts like a list otherwise.
    /// </summary>
    /// <typeparam name="T">Named element type</typeparam>
    public class ReadOnlyNamedListFast<T> : INamedList<T> where T : INamed
    {
        protected IList<T> _list;
        private readonly Dictionary<string, int> _fastLookUpIndices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyNamedListFast{T}"/> class.
        /// </summary>
        /// <param name="elements">Elements to copy</param>
        public ReadOnlyNamedListFast(IEnumerable<T> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            _list = new List<T>(elements);
            _fastLookUpIndices = new Dictionary<String, int>();

            PopulateFastLookupTable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyNamedListFast{T}"/> class.
        /// </summary>
        /// <param name="elements">Elements to copy</param>
        public ReadOnlyNamedListFast(params T[] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            _list = new List<T>(elements);
            _fastLookUpIndices = new Dictionary<String, int>();

            PopulateFastLookupTable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyNamedListFast{T}"/> class.
        /// </summary>
        protected ReadOnlyNamedListFast() 
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyNamedListFast{T}"/> class.
        /// </summary>
        protected ReadOnlyNamedListFast(int initialCapacity)
        {
            _list = new List<T>(initialCapacity);
            _fastLookUpIndices = new Dictionary<String, int>();
        }

        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">Index of element</param>
        /// <returns>The element</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _list.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }

                return _list[index];
            }
        }

        /// <summary>
        /// Gets an element in the list that matches the specified name.
        /// </summary>
        /// <param name="name">Name of the element to find.</param>
        /// <returns>The element that corresponds to the name, or null if not found.</returns>
        public T this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return default(T);
                }

                if (_fastLookUpIndices.TryGetValue(name, out int index))
                {
                    return _list[index];
                }

                return default(T);
            }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Queries the collection to see if the name of the effect part is present in the collection.
        /// </summary>
        /// <param name="name">Name of effect part to query.</param>
        /// <returns>True if the object is present in the collection, false otherwise.</returns>
        public bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return _fastLookUpIndices.ContainsKey(name);
        }

        /// <summary>
        /// Tries to get the associated effect part with the name.
        /// </summary>
        /// <param name="name">Name of effect part to get.</param>
        /// <param name="value">Effect part, if it exists.</param>
        /// <returns>True if the value was found, false otherwise.</returns>
        public bool TryGetValue(string name, out T value)
        {
            value = default(T);

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (_fastLookUpIndices.TryGetValue(name, out int index))
            {
                value = _list[index];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines the index of an element in the list that matches the specified name.
        /// </summary>
        /// <param name="name">Name of the element.</param>
        /// <returns>Zero-based index indicating the position of the element in the list. A value of -1 denotes it was not found.</returns>
        public int IndexOf(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            if (_fastLookUpIndices.TryGetValue(name, out int index))
            {
                return index;
            }

            return -1;
        }

        /// <summary>
        /// Causes the fast look up cache to be updated.
        /// </summary>
        protected void PopulateFastLookupTable()
        {
            _fastLookUpIndices.Clear();

            for (int i = 0; i < _list.Count; i++)
            {
                T obj = _list[i];
                if (obj != null && !_fastLookUpIndices.ContainsKey(obj.Name))
                {
                    _fastLookUpIndices.Add(obj.Name, i);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
