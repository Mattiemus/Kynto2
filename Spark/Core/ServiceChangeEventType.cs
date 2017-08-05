namespace Spark.Core
{
    /// <summary>
    /// Enumerates possible engine service event types.
    /// </summary>
    public enum ServiceChangeEventType
    {
        /// <summary>
        /// Service has been added to the registry.
        /// </summary>
        Added,

        /// <summary>
        /// Service has been removed from the registry.
        /// </summary>
        Removed,

        /// <summary>
        /// Service has been replaced with another service.
        /// </summary>
        Replaced
    }
}
