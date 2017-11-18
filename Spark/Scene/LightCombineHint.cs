namespace Spark.Scene
{
    /// <summary>
    /// Hint for how the engine combines lights when updating the world light list for each mesh.
    /// </summary>
    public enum LightCombineHint
    {
        /// <summary>
        /// Do what the parent does.
        /// </summary>
        Inherit = 0,

        /// <summary>
        /// Combine lights starting from the spatial working towards the root node. The resulting lights are sorted and only the closest are used up to the max number
        /// of lights.
        /// </summary>
        CombineClosest = 1,

        /// <summary>
        /// Use only the spatial's local lights and do not combine.
        /// </summary>
        Local = 2,

        /// <summary>
        /// Do not update lights.
        /// </summary>
        Off = 3
    }
}
