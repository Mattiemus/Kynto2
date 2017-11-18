namespace Spark.Graphics
{
    /// <summary>
    /// Enumerates supported types of lights.
    /// </summary>
    public enum LightType
    {
        /// <summary>
        /// A light that represents a source that has a position in space and is omni-directional.
        /// </summary>
        Point = 0,

        /// <summary>
        /// A light that represents a source that has a position in space that emits light in a cone.
        /// </summary>
        Spot = 1,

        /// <summary>
        /// A light that represents a source infinitely away and is purely directional based.
        /// </summary>
        Directional = 2,

        /// <summary>
        /// A custom light defined by user.
        /// </summary>
        Custom = 3
    }
}
