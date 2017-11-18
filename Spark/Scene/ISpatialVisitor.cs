namespace Spark.Scene
{
    /// <summary>
    /// Defines an object that visists each spatial in the scene graph.
    /// </summary>
    public interface ISpatialVisitor
    {
        /// <summary>
        /// Visists the spatial.
        /// </summary>
        /// <param name="spatial">Spatial to visit.</param>
        /// <returns>True if to keep visiting the subsequent spatials, false to stop processing.</returns>
        bool Visit(Spatial spatial);
    }
}
