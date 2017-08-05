
namespace Spark.Core
{
    /// <summary>
    /// Interface for a core engine servie that can have its state updated. For example, an input service would poll the device to synchronize its
    /// internal state.
    /// </summary>
    public interface IUpdatableEngineService : IEngineService
    {
        /// <summary>
        /// Notifies the service to update its internal state.
        /// </summary>
        void Update();
    }
}
