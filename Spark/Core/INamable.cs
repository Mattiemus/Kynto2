namespace Spark.Core
{
    /// <summary>
    /// Interface for objects that has a name and can be renamed.
    /// </summary>
    public interface INamable : INamed
    {
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        new string Name { get; set; }
    }
}
