namespace Spark.Graphics
{
    /// <summary>
    /// Defines an extended functionality for a render context.
    /// </summary>
    public interface IRenderContextExtension
    {
        /// <summary>
        /// Gets the render context the extension is a part of.
        /// </summary>
        IRenderContext RenderContext { get; }
    }
}
