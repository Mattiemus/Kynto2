namespace Spark.Math
{
    using System;

    /// <summary>
    /// Option flags for pick queries.
    /// </summary>
    [Flags]
    public enum PickingOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Do primitive picking if bounding check succeeds.
        /// </summary>
        PrimitivePick = 1,

        /// <summary>
        /// Back faces from primitive pick tests are not added to the result list.
        /// </summary>
        IgnoreBackfaces = 2
    }
}
