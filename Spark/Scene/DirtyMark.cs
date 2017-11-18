namespace Spark.Scene
{
    using System;

    /// <summary>
    /// Dirty flags for use in updating the scene graph.
    /// </summary>
    [Flags]
    public enum DirtyMark
    {
        /// <summary>
        /// No update necessary.
        /// </summary>
        None = 0,

        /// <summary>
        /// Transform values require updating.
        /// </summary>
        Transform = 1,

        /// <summary>
        /// Bounding values require updating.
        /// </summary>
        Bounding = 2,

        /// <summary>
        /// Lighting state requires updating.
        /// </summary>
        Lighting = 4,

        /// <summary>
        /// All states require updating.
        /// </summary>
        All = Transform | Bounding | Lighting
    }
}
