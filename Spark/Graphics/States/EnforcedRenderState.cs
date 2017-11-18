namespace Spark.Graphics
{
    using System;

    /// <summary>
    /// Flags for for enforcing render states on the render context. When a state is enforced, state-setting for that type
    /// is ignored.
    /// </summary>
    [Flags]
    public enum EnforcedRenderState
    {
        /// <summary>
        /// No states enforced.
        /// </summary>
        None = 0,

        /// <summary>
        /// BlendState is enforced.
        /// </summary>
        BlendState = 1,

        /// <summary>
        /// RasterizerState is enforced.
        /// </summary>
        RasterizerState = 2,

        /// <summary>
        /// DepthStencilState is enforced.
        /// </summary>
        DepthStencilState = 4,

        /// <summary>
        /// All render states are enforced.
        /// </summary>
        All = BlendState | RasterizerState | DepthStencilState
    }
}
