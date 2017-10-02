namespace Spark.Math
{
    /// <summary>
    /// Enumerates the intersection types between a plane and bounding volume.
    /// </summary>
    public enum PlaneIntersectionType
    {
        /// <summary>
        /// No intersection and object is in the positive half space of the plane. This is on the same side as the direction which the plane's normal vector is pointing in.
        /// </summary>
        Front = 0,

        /// <summary>
        /// No intersection and object is in the negative half space of the plane. This is opposite of the plane's Normal vector.
        /// </summary>
        Back = 1,

        /// <summary>
        /// The plane and the object intersect.
        /// </summary>
        Intersects = 2
    }
}
