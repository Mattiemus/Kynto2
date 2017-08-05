namespace Spark.Core
{
    /// <summary>
    /// Interface for objects that can have their data cloned deeply.
    /// </summary>
    public interface IDeepCloneable
    {
        /// <summary>
        /// Get a copy of the object.
        /// </summary>
        /// <returns>Cloned copy.</returns>
        IDeepCloneable Clone();
    }
}
