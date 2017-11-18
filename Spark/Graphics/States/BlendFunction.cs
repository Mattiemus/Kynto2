namespace Spark.Graphics
{
    /// <summary>
    /// Defines how to combine source and destination colors for
    /// blending operations.
    /// </summary>
    public enum BlendFunction
    {
        /// <summary>
        /// Destination is added to the source. Result = (SourceColor * SourceBlend)
        /// + (DestinationColor * DestinationBlend).
        /// </summary>
        Add = 0,

        /// <summary>
        /// Destination is subtracted from the source. Result = (SourceColor * SourceBlend)
        /// - (DestinationColor * DestinationBlend).
        /// </summary>
        Subtract = 1,

        /// <summary>
        /// The source is subtracted from the destination. Result = (DestinationColor * DestinationBlend)
        /// - (SourceColor * SourceBlend).
        /// </summary>
        ReverseSubtract = 2,

        /// <summary>
        /// The result is the minimum of source and destination. Result = min(SourceColor * SourceBlend),
        /// (DestinationColor * DestinationBlend)).
        /// </summary>
        Minimum = 3,

        /// <summary>
        /// The result is the maximum of source and destination. Result = max(SourceColor * SourceBlend),
        /// (DestinationColor * DestinationBlend)).
        /// </summary>
        Maximum = 4
    }
}
