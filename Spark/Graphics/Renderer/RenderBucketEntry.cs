namespace Spark.Graphics
{
    /// <summary>
    /// An entry inside a render bucket. Each entry is a renderable with an (optional) material that will be used
    /// to render the object.
    /// </summary>
    public struct RenderBucketEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderBucketEntry"/> struct.
        /// </summary>
        /// <param name="material">The (optional) material used for rendering.</param>
        /// <param name="renderable">The object to be rendered.</param>
        public RenderBucketEntry(Material material, IRenderable renderable)
        {
            Material = material;
            Renderable = renderable;
        }

        /// <summary>
        /// Material used for rendering the object.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Object to be rendered.
        /// </summary>
        public IRenderable Renderable { get; set; }
    }
}
