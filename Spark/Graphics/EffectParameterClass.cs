namespace Spark.Graphics
{
    /// <summary>
    /// Defines the class of an effect parameter.
    /// </summary>
    public enum EffectParameterClass
    {
        /// <summary>
        /// Parameter is a scalar value.
        /// </summary>
        Scalar = 0,

        /// <summary>
        /// Parameter is a vector value.
        /// </summary>
        Vector = 1,

        /// <summary>
        /// Parameter is a row-major matrix value.
        /// </summary>
        MatrixRows = 2,

        /// <summary>
        /// Parameter is a column-major matrix value.
        /// </summary>
        MatrixColumns = 3,

        /// <summary>
        /// Parameter is either a texture, string, or other resource type.
        /// </summary>
        Object = 4,

        /// <summary>
        /// Parameter is a struct.
        /// </summary>
        Struct = 5
    }
}
