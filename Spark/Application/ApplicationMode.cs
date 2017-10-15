namespace Spark.Application
{
    /// <summary>
    /// Defines different modes a <see cref="SparkApplication"/> can be started in.
    /// </summary>
    public enum ApplicationMode
    {
        /// <summary>
        /// Default mode. The game window is a top-level window that has a message pump.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The game window is not a top-level window and can be a child of another form or embedded in an application.
        /// </summary>
        ChildWindow = 1,

        /// <summary>
        /// The game application does not manage any game window, only a render loop.
        /// </summary>
        Headless = 2
    }
}
