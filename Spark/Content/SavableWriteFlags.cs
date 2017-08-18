namespace Spark.Content
{
    using System;

    /// <summary>
    /// Bit flags for controlling aspects of savable serialization.
    /// </summary>
    [Flags]
    public enum SavableWriteFlags
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// External writers should overwrite existing resources.
        /// </summary>
        OverwriteExistingResource = 1,

        /// <summary>
        /// Writers should favor compression when its available.
        /// </summary>
        UseCompression = 2,

        /// <summary>
        /// Shared resources should be treated as external resources.
        /// </summary>
        SharedAsExternal = 4
    }
}
