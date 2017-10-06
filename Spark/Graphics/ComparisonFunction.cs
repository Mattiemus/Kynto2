namespace Spark.Graphics
{
    /// <summary>
    /// Comparison function enumeration for alpha, stencil, sampling or depth-buffer testing.
    /// </summary>
    public enum ComparisonFunction
    {
        /// <summary>
        /// Always pass the test.
        /// </summary>
        Always = 0,

        /// <summary>
        /// Always fail the test.
        /// </summary>
        Never = 1,

        /// <summary>
        /// Accept new pixel if its value is less than the value of the current pixel.
        /// </summary>
        Less = 2,

        /// <summary>
        /// Accept new pixel if its value is less than or equal to the value of the current pixel.
        /// </summary>
        LessEqual = 3,

        /// <summary>
        /// Accept the new pixel if its value is equal to the value of the current pixel.
        /// </summary>
        Equal = 4,

        /// <summary>
        /// Accept the new pixel if its value is greater than or equal to the value of the current pixel.
        /// </summary>
        GreaterEqual = 5,

        /// <summary>
        /// Accept the new pixel if its value is greater than the value of the current pixel.
        /// </summary>
        Greater = 6,

        /// <summary>
        /// Accept the new pixel if its value is not equal to the current pixel.
        /// </summary>
        NotEqual = 7
    }
}
