namespace Spark.Graphics
{
    /// <summary>
    /// Defines a renderable that will be notified by a render queue when marks are cleared.
    /// </summary>
    public interface IMarkedRenderable : IRenderable
    {
        /// <summary>
        /// Called when the renderable gets cleared from the queue.
        /// </summary>
        /// <param name="id">ID that corresponded to the renderale in the queue.</param>
        /// <param name="queue">Render queue that the renderable was marked in.</param>
        void OnMarkCleared(MarkId id, RenderQueue queue);
    }
}
