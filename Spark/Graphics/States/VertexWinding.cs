namespace Spark.Graphics
{
    /// <summary>
    /// Defines the how vertices are wound. This defines the criteria
    /// to define what are the back and front of a primitive.
    /// </summary>
    public enum VertexWinding
    {
        /// <summary>
        /// Clockwise vertices are defined as Front facing. 
        /// </summary>
        Clockwise = 0,

        /// <summary>
        /// CounterClockwise vertices are defined as Front facing. 
        /// </summary>
        CounterClockwise = 1
    }
}
