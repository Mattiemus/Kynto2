namespace Spark.Graphics.Renderer
{
    /// <summary>
    /// Container to allocate unique ids to each render property
    /// </summary>
    /// <typeparam name="T">Render property type</typeparam>
    internal static class RenderPropertyIdHolder<T>
    {
        /// <summary>
        /// Unique id for the render property type
        /// </summary>
        public static readonly RenderPropertyId Id = RenderPropertyId.GenerateNewUniqueId();
    }
}
