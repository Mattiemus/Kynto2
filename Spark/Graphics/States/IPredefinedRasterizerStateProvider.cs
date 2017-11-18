namespace Spark.Graphics
{
    /// <summary>
    /// A provider for creating and managing predefined rasterizer states.
    /// </summary>
    public interface IPredefinedRasterizerStateProvider
    {
        /// <summary>
        /// Gets a predefined state object where culling is disabled.
        /// </summary>
        RasterizerState CullNone { get; }

        /// <summary>
        /// Gets a predefined state object where back faces are culled and front faces have
        /// a clockwise vertex winding. This is the default state.
        /// </summary>
        RasterizerState CullBackClockwiseFront { get; }

        /// <summary>
        /// Gets a predefined state object where back faces are culled and front faces have a counterclockwise
        /// vertex winding.
        /// </summary>
        RasterizerState CullBackCounterClockwiseFront { get; }

        /// <summary>
        /// Gets a predefined state object where culling is disabled and fillmode is wireframe.
        /// </summary>
        RasterizerState CullNoneWireframe { get; }

        /// <summary>
        /// Queries a predefined rasterizer state by name.
        /// </summary>
        /// <param name="name">Name of the rasterizer state.</param>
        /// <returns>Rasterizer state, or null if it does not exist.</returns>
        RasterizerState GetRasterizerStateByName(string name);
    }
}
