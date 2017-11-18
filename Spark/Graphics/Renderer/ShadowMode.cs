namespace Spark.Graphics
{
    /// <summary>
    /// Defines the different options for shadow mapping.
    /// </summary>
    public enum ShadowMode
    {
        /// <summary>
        /// The object will not be included in casting shadows nor receive shadow maps.
        /// </summary>
        None = 0,

        /// <summary>
        /// The object will be included in casting shadows.
        /// </summary>
        Cast = 1,

        /// <summary>
        /// The object will only receive shadow maps.
        /// </summary>
        Receive = 2,

        /// <summary>
        /// The object will be included in casting shadows and will also receive shadow maps.
        /// </summary>
        CastAndReceive = 3
    }
}
