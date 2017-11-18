namespace Spark.Scene
{
    /// <summary>
    /// Picking hints for how the engine will treat the scene graph Spatials during picking/collision queries.
    /// </summary>
    public enum PickingHint
    {
        /// <summary>
        /// Spatial will not be included in either picking or collision tests.
        /// </summary>
        None = 0,

        /// <summary>
        /// Do what the parent does.
        /// </summary>
        Inherit = 1,

        /// <summary>
        /// Spatial can be included in picking tests.
        /// </summary>
        Pickable = 2,

        /// <summary>
        /// Spatial can be included in collision tests.
        /// </summary>
        Collidable = 3,

        /// <summary>
        /// Spatial can be included in both picking and collision tests.
        /// </summary>
        PickableAndCollidable = 4
    }
}
