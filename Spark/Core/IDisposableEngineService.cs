namespace Spark
{
    using System;

    /// <summary>
    /// Interface for a core engine service that has resources that need to be cleaned up when the engine is destroyed.
    /// </summary>
    public interface IDisposableEngineService : IEngineService, IDisposable
    {
        /// <summary>
        /// Gets whether the service has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }
    }
}
