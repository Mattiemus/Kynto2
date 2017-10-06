namespace Spark.Graphics
{
    /// <summary>
    /// Defines a vertex that has a layout.
    /// </summary>
    public interface IVertexType
    {
        /// <summary>
        /// Gets the layout of the vertex.
        /// </summary>
        /// <returns>Vertex layout</returns>
        VertexLayout GetVertexLayout();
    }
}
