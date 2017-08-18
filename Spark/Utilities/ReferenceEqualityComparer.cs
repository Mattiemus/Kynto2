namespace Spark.Utilities
{
    using System.Collections.Generic;

    /// <summary>
    /// Equality comparer that uses <see cref="object.ReferenceEquals"/> to compare two objects.
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Equalses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(T obj)
        {
            unchecked
            {
                return obj.GetHashCode();
            }
        }
    }
}
