namespace Spark.Scene
{
    /// <summary>
    /// Hint for how the engine will treat scene graph Spatials during the culling pass.
    /// </summary>
    public enum CullHint
    {
        /// <summary>
        /// Do what the parent does.
        /// </summary>
        Inherit = 0,

        /// <summary>
        /// Cull based on visibility, if the spatial is outside the frustum it is culled. Otherwise it is not.
        /// </summary>
        Dynamic = 1,

        /// <summary>
        /// Always cull the spatial, regardless of visibility.
        /// </summary>
        Always = 2,

        /// <summary>
        /// Never cull the spatial, regardless of visibility.
        /// </summary>
        Never = 3
    }
}
