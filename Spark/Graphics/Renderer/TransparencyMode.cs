namespace Spark.Graphics
{
    /// <summary>
    /// Defines the different transparency options when rendering translucent geometry.
    /// </summary>
    public enum TransparencyMode
    {
        /// <summary>
        /// One sided - where the renderable is drawn normally.
        /// </summary>
        OneSided = 0,

        /// <summary>
        /// Two sided - where the renderable is drawn using a two-pass scheme that will enforce certain render states in order to properly
        /// draw front and back parts of a transparent geometry.
        /// </summary>
        TwoSided = 1
    }
}
