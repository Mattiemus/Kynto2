namespace Spark.Content
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a collection of missing content handlers. Each handler is registered to a type and can serve place holder
    /// content for a resource that could not be found or loaded.
    /// </summary>
    public sealed class MissingContentHandlerCollection : ICollection<IMissingContentHandler>
    {
        private readonly Dictionary<Type, IMissingContentHandler> _handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingContentHandlerCollection"/> class.
        /// </summary>
        public MissingContentHandlerCollection()
        {
            _handlers = new Dictionary<Type, IMissingContentHandler>();
        }

        /// <summary>
        /// Gets the number of handlers.
        /// </summary>
        public int Count => _handlers.Count;

        /// <summary>
        /// Gets a value indicating whether the collection is read only
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the <see cref="IMissingContentHandler"/> with the specified target type.
        /// </summary>
        public IMissingContentHandler this[Type targetType]
        {
            get
            {
                if (targetType == null)
                {
                    return null;
                }

                if (!_handlers.TryGetValue(targetType, out IMissingContentHandler handler))
                {
                    handler = null;
                }

                return handler;
            }
        }

        /// <summary>
        /// Adds the content handler to the collection, which will be registered to its target type.
        /// </summary>
        /// <param name="item">Missing content handler</param>
        public void Add(IMissingContentHandler item)
        {
            if (item == null)
            {
                return;
            }

            _handlers[item.TargetType] = item;
        }

        /// <summary>
        /// Remove the the content handler from the collection.
        /// </summary>
        /// <param name="item">Handler to remove</param>
        /// <returns>True if the handler was removed, false otherwise</returns>
        public bool Remove(IMissingContentHandler item)
        {
            return Remove(item.TargetType);
        }

        /// <summary>
        /// Remove the the content handler registered to the specified type from the collection.
        /// </summary>
        /// <param name="targetType">Target type that the handler is registered to</param>
        /// <returns>True if the handler was removed, false otherwise</returns>
        public bool Remove(Type targetType)
        {
            if (targetType == null)
            {
                return false;
            }

            return _handlers.Remove(targetType);
        }

        /// <summary>
        /// Clear all content handlers from this collection.
        /// </summary>
        public void Clear()
        {
            _handlers.Clear();
        }

        /// <summary>
        /// Determines whether the container contains the given handler
        /// </summary>
        /// <param name="item">Handler to search for</param>
        /// <returns>True if the collection contains the handler, false otherwise</returns>
        public bool Contains(IMissingContentHandler item)
        {
            return _handlers.ContainsValue(item);
        }

        /// <summary>
        /// Determines whether the container contains a handler which targets the given type
        /// </summary>
        /// <param name="item">Target type to search for</param>
        /// <returns>True if the collection contains a handler for the given type, false otherwise</returns>
        public bool Contains(Type item)
        {
            return _handlers.ContainsKey(item);
        }

        /// <summary>
        /// Copies the elements within the collection to a given array, starting at a specific index
        /// </summary>
        /// <param name="array">Array to copy elements to</param>
        /// <param name="arrayIndex">Starting index</param>
        public void CopyTo(IMissingContentHandler[] array, int arrayIndex)
        {
            _handlers.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns> A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public Dictionary<Type, IMissingContentHandler>.ValueCollection.Enumerator GetEnumerator()
        {
            return _handlers.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns> A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        IEnumerator<IMissingContentHandler> IEnumerable<IMissingContentHandler>.GetEnumerator()
        {
            return _handlers.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _handlers.Values.GetEnumerator();
        }
    }
}
