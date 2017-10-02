namespace Spark.Math
{
    /// <summary>
    /// Enumerates the six planes that make up a bounding frustum.
    /// </summary>
    public enum FrustumPlane
    {
        /// <summary>
        /// Left frustum plane.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Right frustum plane.
        /// </summary>
        Right = 1,

        /// <summary>
        /// Top frustum plane.
        /// </summary>
        Top = 2,

        /// <summary>
        /// Bottom frustum plane.
        /// </summary>
        Bottom = 3,

        /// <summary>
        /// Near frustum plane.
        /// </summary>
        Near = 4,

        /// <summary>
        /// Far frustum plane.
        /// </summary>
        Far = 5
    }
}
