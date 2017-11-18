namespace Spark.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A collection of extended properties for a <see cref="IExtendedProperties"/> object.
    /// </summary>
    public sealed class ExtendedPropertiesCollection : Dictionary<int, Object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedPropertiesCollection"/> class.
        /// </summary>
        public ExtendedPropertiesCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedPropertiesCollection"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public ExtendedPropertiesCollection(int initialCapacity) 
            : base(initialCapacity)
        {
        }

        /// <summary>
        /// Tries to get a value in the dictionary associated with the key. This safely attempts to cast the value to the specified type,
        /// and if that fails then the method returns false.
        /// </summary>
        /// <typeparam name="T">Type of value to cast to.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The return value.</param>
        /// <returns>True if the value was contained in the collection and was successfully casted to the type, false if it wasn't found or could not be casted.</returns>
        public bool TryGetValueAs<T>(int key, out T value)
        {
            if (TryGetValue(key, out object obj))
            {
                if (obj == null)
                {
                    value = default(T);
                    return true;
                }

                Type type = typeof(T);
                if (type.IsInstanceOfType(obj))
                {
                    value = (T)obj;
                    return true;
                }
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Counts the number of objects contained in the collection that are of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of value to count.</typeparam>
        /// <returns>The number of objects in the collection that are of the type.</returns>
        public int GetCountOf<T>()
        {
            int count = 0;
            foreach (KeyValuePair<int, Object> kv in this)
            {
                if (kv.Value is T)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
