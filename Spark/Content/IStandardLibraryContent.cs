namespace Spark.Content
{
    /// <summary>
    /// An object that represents a standard content item from a library.
    /// </summary>
    public interface IStandardLibraryContent
    {
        /// <summary>
        /// Gets the name of the content this instance represents. If <see cref="IsStandardContent"/> is false, then this returns an empty string.
        /// </summary>
        string StandardContentName { get; }

        /// <summary>
        /// Gets if the instance represents standard library content or not.
        /// </summary>
        bool IsStandardContent { get; }

        /// <summary>
        /// Gets a consistent hash code that identifies the content item. If it is not standard content, each instance should have a unique hash, if the instance is
        /// standard content, each instance should have the same hash code. This might differ from .NET's hash code and is only used to identify two instances that
        /// represent the same data (there may be situations where we want to differentiate the two instances, so we don't rely on the .NET's get hash code function).
        /// </summary>
        /// <returns>32-bit hash code.</returns>
        int GetContentHashCode();
    }
}
