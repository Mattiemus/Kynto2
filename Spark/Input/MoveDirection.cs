namespace Spark.Input
{
    /// <summary>
    /// Enumerates the valid move directions.
    /// </summary>
    public enum MoveDirection
    {
        /// <summary>
        /// Condition evaluates true if mouse only moves along X axis.
        /// </summary>
        XOnly = 0,

        /// <summary>
        /// Condition evaluates true if mouse only moves along Y axis.
        /// </summary>
        YOnly = 1,

        /// <summary>
        /// Condition evaluates true if mouse moves along any axis.
        /// </summary>
        XAndY = 2
    }
}
