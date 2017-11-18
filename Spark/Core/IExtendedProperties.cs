namespace Spark.Core
{
    /// <summary>
    /// Interface for objects that contain a map of "extended properties".
    /// </summary>
    public interface IExtendedProperties
    {
        /// <summary>
        /// Gets the dictionary of extended properties.
        /// </summary>
        ExtendedPropertiesCollection ExtendedProperties { get; }
    }
}
