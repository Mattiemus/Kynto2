namespace Spark.Scene
{
    /// <summary>
    /// Bounding hints for how nodes in the scenegraph will construc their world boundings based on their children.
    /// </summary>
    public enum BoundingCombineHint
    {
        /// <summary>
        /// Do what the parent does.
        /// </summary>
        Inherit = 0,

        /// <summary>
        /// Clone the first valid child's world bounding and use that type as the world bounding of the node.
        /// </summary>
        CloneFromChildren = 1,

        /// <summary>
        /// Always use a bounding sphere.
        /// </summary>
        Sphere = 2,

        /// <summary>
        /// Always use an axis aligned bounding box.
        /// </summary>
        AxisAlignedBoundingBox = 3,

        /// <summary>
        /// Always use a bounding capsule.
        /// </summary>
        Capsule = 4,

        /// <summary>
        /// Always use an oriented bounding box.
        /// </summary>
        OrientedBoundingBox = 5
    }
}
