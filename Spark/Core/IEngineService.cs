namespace Spark
{
    /// <summary>
    /// Interface for a core engine service, such as rendering, input, or windowing services.
    /// </summary>
    public interface IEngineService
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Initializes the service. This is called by the engine when a service is newly registered.
        /// </summary>
        /// <param name="engine">Engine instance</param>
        void Initialize(SparkEngine engine);
    }
}
