namespace Spark.Content
{
    /// <summary>
    /// Enumerates how shared/external resources should be handled.
    /// </summary>
    public enum ExternalSharedMode
    {
        /// <summary>
        /// Default behavior.
        /// </summary>
        Default = 0,

        /// <summary>
        /// All savables to be written as external should be treated as shared.
        /// </summary>
        AllShared = 1,

        /// <summary>
        /// All savables to be written as shared should be treated as external.
        /// </summary>
        AllExternal = 2
    }
}
