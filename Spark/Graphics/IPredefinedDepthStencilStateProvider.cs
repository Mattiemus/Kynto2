namespace Spark.Graphics
{
    /// <summary>
    /// A provider for creaing and managing predefined depthstencil states.
    /// </summary>
    public interface IPredefinedDepthStencilStateProvider
    {
        /// <summary>
        /// Gets a predefined state object where the depth buffer is disabled and writing to it is disabled.
        /// </summary>
        DepthStencilState None { get; }

        /// <summary>
        /// Gets a predefined state object where the depth buffer is enabled and writing to it is disabled.
        /// </summary>
        DepthStencilState DepthWriteOff { get; }

        /// <summary>
        /// Gets a predefined state object where the depth buffer is enabld and writing to it is enabled.
        /// This is the default state.
        /// </summary>
        DepthStencilState Default { get; }

        /// <summary>
        /// Queries a predefined depth stencil state by name.
        /// </summary>
        /// <param name="name">Name of the depth stencil state.</param>
        /// <returns>DepthStencil state, or null if it does not exist.</returns>
        DepthStencilState GetDepthStencilStateByName(string name);
    }
}
