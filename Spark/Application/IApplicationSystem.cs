namespace Spark.Application
{
    using Core;
    using Implementation;

    /// <summary>
    /// Application system service
    /// </summary>
    public interface IApplicationSystem : IDisposableEngineService
    {
        /// <summary>
        /// Event fired when the system is disposing
        /// </summary>
        event TypedEventHandler<IApplicationSystem> Disposing;

        /// <summary>
        /// Gets the name of the application platform
        /// </summary>
        string Platform { get; }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="implementor">Application host implementor</param>
        /// <returns>The application host implementation</returns>
        IApplicationHostImplementation CreateApplicationHostImplementation(ApplicationHost implementor);
    }
}
