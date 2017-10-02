namespace Spark.Math
{
    /// <summary>
    /// Enumerates supported bounding volume types.
    /// </summary>
    public enum BoundingType
    {
        /// <summary>
        /// Sphere bounding volume, defined by a point and radius.
        /// </summary>
        Sphere = 0,

        /// <summary>
        /// Axis aligned bounding box with extents along the X, Y, Z axes.
        /// </summary>
        AxisAlignedBoundingBox = 1,

        /// <summary>
        /// Oriented bounding box, with extents along a rotated frame.
        /// </summary>
        OrientedBoundingBox = 2,

        /// <summary>
        /// Capsule bounding volume that is defined by the sweep of a sphere of a certain radius from one point to another point.
        /// </summary>
        Capsule = 3,

        /// <summary>
        /// A frustum defined by six intersecting planes.
        /// </summary>
        Frustum = 4,

        /// <summary>
        /// Simple convex mesh shape.
        /// </summary>
        Mesh = 5
    }
}
