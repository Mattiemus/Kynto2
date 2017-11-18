namespace Spark
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// A read-only list that can wrap modifiable lists or simply contain a non-modifiable collection of elements.
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// The underlying dictionary.
        /// </summary>
        protected readonly IList<T> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class. This effectively creates a read only wrapper
        /// of the list, rather than copying each element.
        /// </summary>
        /// <param name="elements">Element list to hold</param>
        public ReadOnlyList(IList<T> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            _list = elements;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class. This copies the elements from the enumerable, rather than
        /// wrappering a collection.
        /// </summary>
        /// <param name="elements">Elements to copy</param>
        public ReadOnlyList(IEnumerable<T> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            _list = new List<T>(elements);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class. This copies the elements from the array, rather than
        /// wrappering a collection.
        /// </summary>
        /// <param name="elements">Elements to copy</param>
        public ReadOnlyList(params T[] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            _list = new List<T>(elements);
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Gets the element at the specified index.
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
        /// Determines the index of the specified item.
        /// </summary>
        /// <param name="item">Item to find in the list.</param>
        /// <returns>The index of the item, or -1 if it is not present in the list.</returns>
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
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
