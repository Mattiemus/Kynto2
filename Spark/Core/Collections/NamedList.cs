namespace Spark.Core.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of named elements.
    /// </summary>
    /// <typeparam name="T">Named element type</typeparam>
    public class NamedList<T> : List<T>, INamedList<T> where T : INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedList{T}"/> class.
        /// </summary>
        public NamedList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedList{T}"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list.</param>
        public NamedList(int initialCapacity) 
            : base(initialCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedList{T}"/> class. This copies the elements from the enumerable, rather than
        /// wrappering a collection.
        /// </summary>
        /// <param name="elements">Elements to copy</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the enumerable is null.</exception>
        public NamedList(IEnumerable<T> elements) 
            : base(elements)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedList{T}"/> class. This copies the elements from the array, rather than
        /// wrappering a collection.
        /// </summary>
        /// <param name="elements">Elements to copy</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the array is null.</exception>
        public NamedList(params T[] elements) 
            : base(elements)
        {
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

                for (int i = 0; i < Count; i++)
                {
                    T obj = this[i];
                    if (obj != null && obj.Name.Equals(name))
                    {
                        return obj;
                    }
                }

                return default(T);
            }
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

            for (int i = 0; i < Count; i++)
            {
                T obj = this[i];

                if (obj != null && obj.Name.Equals(name))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
