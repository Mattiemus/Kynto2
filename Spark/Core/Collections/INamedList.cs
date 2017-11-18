namespace Spark
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an indexable list of named objects.
    /// </summary>
    /// <typeparam name="T">Type of named object</typeparam>
    public interface INamedList<out T> : IReadOnlyList<T> where T : INamed
    {
        /// <summary>
        /// Gets an element in the list that matches the specified name.
        /// </summary>
        /// <param name="name">Name of the element.</param>
        /// <returns>The element, or null if it could not be found.</returns>
        T this[string name] { get; }

        /// <summary>
        /// Determines the index of an element in the list that matches the specified name.
        /// </summary>
        /// <param name="name">Name of the element.</param>
        /// <returns>Zero-based index indicating the position of the element in the list. A value of -1 denotes it was not found.</returns>
        int IndexOf(string name);
    }
}
