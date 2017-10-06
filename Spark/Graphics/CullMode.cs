namespace Spark.Graphics
{
    /// <summary>
    /// Defines how a primitive should be culled.
    /// </summary>
    public enum CullMode
    {
        /// <summary>
        /// No primitive is culled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Front facing primitives are culled.
        /// </summary>
        Front = 1,

        /// <summary>
        /// Back facing primitives are culled.
        /// </summary>
        Back = 2
    }
}
