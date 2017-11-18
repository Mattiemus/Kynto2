namespace Spark.Graphics
{
    /// <summary>
    /// Defines a coordinator for rendering objects. It defines a queue of objects that serves as input for the renderer and a collection
    /// of stages that serves as the logic that is executed to composite the final rendered image.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Gets the renderer's render context. This is the renderer's input.
        /// </summary>
        RenderQueue RenderQueue { get; }

        /// <summary>
        /// Gets the renderer's render stages. This is the renderer's draw logic.
        /// </summary>
        RenderStageCollection RenderStages { get; }

        /// <summary>
        /// Gets the renderer's render queue. This is how the renderer draws its input.
        /// </summary>
        IRenderContext RenderContext { get; }

        /// <summary>
        /// Process a renderable to be rendered.
        /// </summary>
        /// <param name="renderable">Renderable to process.</param>
        /// <returns>True if the renderable was processed successfully, false otherwise.</returns>
        bool Process(IRenderable renderable);

        /// <summary>
        /// Executes all render stages in the renderer.
        /// </summary>
        /// <param name="sortBuckets">True if the render queue should be sorted before executing render stages, false if not.</param>
        /// <param name="clearBuckets">True if the render queue should be cleared after executing render stages, false if not.</param>
        void Render(bool sortBuckets, bool clearBuckets);
    }
}
