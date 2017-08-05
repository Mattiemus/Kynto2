namespace Spark.Core
{
    /// <summary>
    /// Interface for objects that have a name.
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        string Name { get; }
    }
}
