namespace Spark.Core
{
    /// <summary>
    /// Interface that will register all the necessary services to get fully setup and operational on a given platform. Its a logical container
    /// to easily package together different services together based on platform, which can have several configurations in itself.
    /// </summary>
    public interface IPlatformInitializer
    {
        /// <summary>
        /// Initializes the platform's services.
        /// </summary>
        /// <param name="engine">Engine instance</param>
        void Initialize(Engine engine);
    }
}
