namespace Spark.Graphics.Renderer
{
    /// <summary>
    /// A simple rendering stage that draws a single bucket with materials.
    /// </summary>
    public sealed class SimpleRenderStage : IRenderStage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleRenderStage"/> class.
        /// </summary>
        /// <param name="id">The bucket identifier.</param>
        public SimpleRenderStage(RenderBucketId id)
        {
            BucketToDraw = id;
        }

        /// <summary>
        /// Gets or sets the bucket to draw.
        /// </summary>
        public RenderBucketId BucketToDraw { get; set; }

        /// <summary>
        /// Executes the draw logic of the stage.
        /// </summary>
        /// <param name="queue">Render queue of objects that are to be drawn by the renderer.</param>
        /// <param name="renderContext">Render context of the renderer.</param>
        public void Execute(IRenderContext renderContext, RenderQueue queue)
        {
            RenderBucket bucket = queue[BucketToDraw];
            if (bucket != null)
            {
                bucket.DrawAll(renderContext, true);
            }
        }
    }
}
