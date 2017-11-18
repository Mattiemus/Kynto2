namespace Spark.Graphics
{
    /// <summary>
    /// Sprite sort mode enumeration.
    /// </summary>
    public enum SpriteSortMode
    {
        /// <summary>
        /// Default, where sprites are queued and rendered all together in the order that they are received. No sorting
        /// is done.
        /// </summary>
        Deferred = 0,

        /// <summary>
        /// Same as deferred, except sort back to front using the sprite's depth order. Useful for transparent sprites.
        /// </summary>
        BackToFront = 1,

        /// <summary>
        /// Same as deferred, except sort front to back using the sprite's depth order. Useful for opaque sprites.
        /// </summary>
        FrontToBack = 2,

        /// <summary>
        /// Same as deferred, except sort by texture, where all sprites that use the same texture will be drawn as a single batch.
        /// </summary>
        Texture = 3,

        /// <summary>
        /// Incoming sprites are not sorted or queued, instead they are drawn immediately.
        /// </summary>
        Immediate = 4
    }
}
