namespace Spark.Graphics.Renderer.Forward
{
    /// <summary>
    /// Simple forward renderer.
    /// </summary>
    public sealed class ForwardRenderer : BaseRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardRenderer"/> class.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        public ForwardRenderer(IRenderContext renderContext) 
            : base(renderContext)
        {
            SetDefaultStages();
        }

        /// <summary>
        /// Initializes the render stages for the <see cref="ForwardRenderer"/>
        /// </summary>
        private void SetDefaultStages()
        {
            int initialSize = 32;

            OpaqueRenderBucketComparer opaqueComparer = new OpaqueRenderBucketComparer();
            TransparentRenderBucketComparer transparentComparer = new TransparentRenderBucketComparer();
            OrthoRenderBucketComparer orthoComparer = new OrthoRenderBucketComparer();

            RenderQueue.AddBucket(new RenderBucket(RenderBucketId.PreOpaqueBucket, opaqueComparer, initialSize));
            RenderQueue.AddBucket(new RenderBucket(RenderBucketId.Opaque, opaqueComparer, initialSize));
            RenderQueue.AddBucket(new RenderBucket(RenderBucketId.Transparent, transparentComparer, initialSize));
            RenderQueue.AddBucket(new RenderBucket(RenderBucketId.Ortho, orthoComparer, initialSize));
            RenderQueue.AddBucket(new RenderBucket(RenderBucketId.PostOpaqueBucket, opaqueComparer, initialSize));

            RenderStages.AddStage(new SimpleRenderStage(RenderBucketId.PreOpaqueBucket));
            RenderStages.AddStage(new SimpleRenderStage(RenderBucketId.Opaque));
            RenderStages.AddStage(new TransparentRenderStage(RenderContext.RenderSystem, RenderBucketId.Transparent));
            RenderStages.AddStage(new SimpleRenderStage(RenderBucketId.Ortho));
            RenderStages.AddStage(new SimpleRenderStage(RenderBucketId.PostOpaqueBucket));
        }
    }
}
