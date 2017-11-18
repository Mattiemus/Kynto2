namespace Spark.Graphics
{
    /// <summary>
    /// Stencil buffer operation enumeration.
    /// </summary>
    public enum StencilOperation
    {
        /// <summary>
        /// Do not update the stencil-buffer entry. This is the default value.
        /// </summary>
        Keep = 0,

        /// <summary>
        /// Sets the stencil-buffer entry to zero.
        /// </summary>
        Zero = 1,

        /// <summary>
        /// Replaces the stencil-buffer entry with the reference value.
        /// </summary>
        Replace = 2,

        /// <summary>
        /// Increments the stencil-buffer entry, wrapping to zero if the new value
        /// exceeds the maximum value.
        /// </summary>
        Increment = 3,

        /// <summary>
        /// Decrements the stencil-buffer entry, wrapping to the maximum value if the new
        /// value is less than zero.
        /// </summary>
        Decrement = 4,

        /// <summary>
        /// Increments the stencil-buffer entry, clamping to the maximum value.
        /// </summary>
        IncrementAndClamp = 5,

        /// <summary>
        /// Decrements the stencil-buffer entry, clamping to zero.
        /// </summary>
        DecrementAndClamp = 6,

        /// <summary>
        /// Inverts the bits in the stencil-buffer entry.
        /// </summary>
        Invert = 7
    }
}
