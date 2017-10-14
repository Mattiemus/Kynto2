namespace Spark.Graphics.Renderer
{
    using System.Collections.Generic;

    /// <summary>
    /// A simple rendering stage that just draws the specified buckets with their materials.
    /// </summary>
    public sealed class SimpleMultiRenderStage : IRenderStage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMultiRenderStage"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public SimpleMultiRenderStage(RenderBucketId id)
        {
            BucketsToDraw = new List<RenderBucketId>(1);
            BucketsToDraw.Add(id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMultiRenderStage"/> class.
        /// </summary>
        /// <param name="ids">The ids.</param>
        public SimpleMultiRenderStage(params RenderBucketId[] ids)
        {
            BucketsToDraw = new List<RenderBucketId>(ids);
        }

        /// <summary>
        /// Gets the list of render buckets that will be drawn, in the supplied order of the list, by the render stage.
        /// </summary>
        public List<RenderBucketId> BucketsToDraw { get; }

        /// <summary>
        /// Executes the draw logic of the stage.
        /// </summary>
        /// <param name="queue">Render queue of objects that are to be drawn by the renderer.</param>
        /// <param name="renderContext">Render context of the renderer.</param>
        public void Execute(IRenderContext renderContext, RenderQueue queue)
        {
            for (int i = 0; i < BucketsToDraw.Count; i++)
            {
                RenderBucket bucket = queue[BucketsToDraw[i]];
                if (bucket != null)
                {
                    bucket.DrawAll(renderContext, true);
                }
            }
        }
    }
}
