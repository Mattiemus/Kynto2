namespace Spark.Graphics
{
    /// <summary>
    /// A provider for creating and managing predefined sampler states.
    /// </summary>
    public interface IPredefinedSamplerStateProvider
    {
        /// <summary>
        /// Gets the predefined state object where point filtering is used and UVW coordinates wrap.
        /// </summary>
        SamplerState PointWrap { get; }

        /// <summary>
        /// Gets the predefined state object where point filtering is used and UVW coordinates are clamped in the range of [0, 1]. This
        /// is the default state.
        /// </summary>
        SamplerState PointClamp { get; }

        /// <summary>
        /// Gets the predefined state object where linear filtering is used and UVW coordinates wrap.
        /// </summary>
        SamplerState LinearWrap { get; }

        /// <summary>
        /// Gets the predefined state object where linear filtering is used and UVW coordinates are clamped in the range of [0, 1].
        /// </summary>
        SamplerState LinearClamp { get; }

        /// <summary>
        /// Gets the predefined state object where anisotropic filtering is used and UVW coordinates wrap.
        /// </summary>
        SamplerState AnisotropicWrap { get; }

        /// <summary>
        /// Gets the predefined state object where anisotropic filtering is used and UVW coordinates are
        /// clamped in the range of [0, 1].
        /// </summary>
        SamplerState AnisotropicClamp { get; }

        /// <summary>
        /// Queries a predefined sampler state by name.
        /// </summary>
        /// <param name="name">Name of the sampler state.</param>
        /// <returns>Sampler state, or null if it does not exist.</returns>
        SamplerState GetSamplerStateByName(string name);
    }
}
