namespace Spark.Math
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an object that can participate in picking operations.
    /// </summary>
    public interface IPickable
    {
        /// <summary>
        /// Gets the absolute world bounding of the object.
        /// </summary>
        BoundingVolume WorldBounding { get; }

        /// <summary>
        /// Performs a ray-mesh intersection test.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="results">List of results to add to.</param>
        /// <param name="ignoreBackfaces">True if backfaces (relative to the pick ray) should be ignored, false if they should be considered a result.</param>
        /// <returns>True if an intersection occured and results were added to the output list, false if no intersection occured.</returns>
        bool IntersectsMesh(ref Ray ray, IList<Tuple<LineIntersectionResult, Triangle?>> results, bool ignoreBackfaces);
    }
}
