namespace Spark.Graphics
{
    using System;

    /// <summary>
    /// An abstract renderer to serve as the root for other implementations.
    /// </summary>
    public abstract class BaseRenderer : IRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRenderer"/> class.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        protected BaseRenderer(IRenderContext renderContext)
        {
            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext));
            }

            RenderQueue = new RenderQueue();
            RenderStages = new RenderStageCollection();
            RenderContext = renderContext;
        }

        /// <summary>
        /// Gets the renderer's render context. This is the renderer's input.
        /// </summary>
        public IRenderContext RenderContext { get; }

        /// <summary>
        /// Gets the renderer's render stages. This is the renderer's draw logic.
        /// </summary>
        public RenderStageCollection RenderStages { get; }

        /// <summary>
        /// Gets the renderer's render queue. This is how the renderer draws its input.
        /// </summary>
        public RenderQueue RenderQueue { get; }

        /// <summary>
        /// Process a renderable to be rendered.
        /// </summary>
        /// <param name="renderable">Renderable to process.</param>
        /// <returns>True if the renderable was processed successfully, false otherwise.</returns>
        public virtual bool Process(IRenderable renderable)
        {
            if (renderable == null || !renderable.IsValidForDraw)
            {
                return false;
            }

            return RenderQueue.Enqueue(renderable);
        }

        /// <summary>
        /// Executes all render stages in the renderer. The render queue is sorted and cleared.
        /// </summary>
        public void Render()
        {
            Render(true, true);
        }

        /// <summary>
        /// Executes all render stages in the renderer.
        /// </summary>
        /// <param name="sortBuckets">True if the render queue should be sorted before executing render stages, false if not.</param>
        /// <param name="clearBuckets">True if the render queue should be cleared after executing render stages, false if not.</param>
        public virtual void Render(bool sortBuckets, bool clearBuckets)
        {
            if (sortBuckets)
            {
                RenderQueue.SortBuckets(RenderContext.Camera);
            }

            for (int i = 0; i < RenderStages.Count; i++)
            {
                RenderStages[i].Execute(RenderContext, RenderQueue);
            }

            if (clearBuckets)
            {
                RenderQueue.ClearBuckets();
            }
        }
    }
}
