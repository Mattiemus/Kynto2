namespace Spark.Graphics
{
    /// <summary>
    /// Describes if presenting should enable or disable VSync.
    /// </summary>
    public enum PresentInterval
    {
        /// <summary>
        /// Present to the screen without waiting for vertical retrace.
        /// </summary>
        Immediate = 0,

        /// <summary>
        /// Wait for one vertical retrace period.
        /// </summary>
        One = 1,

        /// <summary>
        /// Wait for two vertical retrace periods.
        /// </summary>
        Two = 2
    }
}
