namespace Spark.Input
{
    /// <summary>
    /// Enumerates the valid move directions.
    /// </summary>
    public enum ScrollDirection
    {
        /// <summary>
        /// Condition evaluates true if the wheel is scrolled forward (positive). 
        /// </summary>
        Forward = 0,

        /// <summary>
        /// Condition evaluates true if the wheel is scrolled backward (negative).
        /// </summary>
        Backward = 1,

        /// <summary>
        /// Condition evaluates true if the wheel is scrolled in any direction.
        /// </summary>
        Both = 2
    }
}
