namespace Spark.Math
{
    /// <summary>
    /// Enumerates containment types between bounding volumes.
    /// </summary>
    public enum ContainmentType
    {
        /// <summary>
        /// The two bounding volumes do not overlap one another at all.
        /// </summary>
        Outside = 0,

        /// <summary>
        /// One bounding volume is completely inside the other.
        /// </summary>
        Inside = 1,

        /// <summary>
        /// The two bounding volumes partially overlap or touch.
        /// </summary>
        Intersects = 2
    }
}
