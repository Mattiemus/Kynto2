namespace Spark.Graphics
{
    /// <summary>
    /// Defines the topology of mesh data.
    /// </summary>
    public enum PrimitiveType
    {
        /// <summary>
        /// Vertex data is defined as a list of triangles, where every three new vertices make up a triangle.
        /// </summary>
        TriangleList = 0,

        /// <summary>
        /// Vertex data is defined as a list of triangles where every two new vertices, and a vertex from the 
        /// previous triangle make up a triangle.
        /// </summary>
        TriangleStrip = 1,

        /// <summary>
        /// Vertex data is defined as a list of lines where every two new vertices make up a line.
        /// </summary>
        LineList = 2,

        /// <summary>
        /// Vertex data is defined as a list of lines where every one new vertex and a vertex from the previous line
        /// make up a new line.
        /// </summary>
        LineStrip = 3,

        /// <summary>
        /// Vertex data is defined as a list of points.
        /// </summary>
        PointList = 4
    }
}
