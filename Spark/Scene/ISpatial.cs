namespace Spark.Scene
{
    using Core;
    using Math;
    using Graphics.Renderer;

    /// <summary>
    /// Defines an object that exists in world space. A spatial may be a single mesh or a collection of meshes.
    /// </summary>
    public interface ISpatial : IExtendedProperties
    {
        /// <summary>
        /// Gets the bounding of the object in world space.
        /// </summary>
        BoundingVolume WorldBounding { get; }

        /// <summary>
        /// Find intersections between the spatial and a ray pick query.
        /// </summary>
        /// <param name="query">Pick query</param>
        /// <returns>True if a pick was found, false if not.</returns>
        bool FindPicks(PickQuery query);

        /// <summary>
        /// Processes visible rendereables in the spatial, by doing frustum culling and then adding them
        /// to the renderer.
        /// </summary>
        /// <param name="renderer">Renderer to process renderables.</param>
        /// <param name="skipCullCheck">True if frustum culling should be performed, false if they should be skipped.</param>
        void ProcessVisibleSet(IRenderer renderer, bool skipCullCheck);
    }
}
