namespace Spark.Graphics
{
    /// <summary>
    /// Defines a stage in a renderer. Each render stage executes logic to contribute to the final output (e.g. shadow map generation, object rendering,
    /// composition, etc).
    /// </summary>
    public interface IRenderStage
    {
        /// <summary>
        /// Executes the draw logic of the stage.
        /// </summary>
        /// <param name="renderContext">Render context of the renderer.</param>
        /// <param name="queue">Render queue of objects that are to be drawn by the renderer.</param>
        void Execute(IRenderContext renderContext, RenderQueue queue);
    }
}
