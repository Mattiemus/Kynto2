namespace Spark.Graphics
{
    /// <summary>
    /// A provider for creating and managing predefined blend states.
    /// </summary>
    public interface IPredefinedBlendStateProvider
    {
        /// <summary>
        /// Gets a predefined state object for opaque blending, where no blending occurs and destination color overwrites source color. This is 
        /// the default state.
        /// </summary>
        BlendState Opaque { get; }

        /// <summary>
        /// Gets a predefined state object for premultiplied alpha blending, where source and destination colors are blended by using
        /// alpha, and where the color contains alpha pre-multiplied.
        /// </summary>
        BlendState AlphaBlendPremultiplied { get; }

        /// <summary>
        /// Gets a predefined state object for additive blending, where source and destination color are blended without using alpha.
        /// </summary>
        BlendState AdditiveBlend { get; }

        /// <summary>
        ///Gets a predefined state object for non-premultiplied alpha blending, where the source and destination color are blended by using alpha,
        ///and where the color does not contain the alpha pre-multiplied.
        /// </summary>
        BlendState AlphaBlendNonPremultiplied { get; }

        /// <summary>
        /// Queries a predefined blend state by name.
        /// </summary>
        /// <param name="name">Name of the blend state.</param>
        /// <returns>Blend state, or null if it does not exist.</returns>
        BlendState GetBlendStateByName(string name);
    }
}
