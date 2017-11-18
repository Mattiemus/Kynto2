namespace Spark.Graphics
{
    /// <summary>
    /// Defines the render state type.
    /// </summary>
    public enum RenderStateType
    {
        /// <summary>
        /// State is a BlendState.
        /// </summary>
        BlendState = 0,

        /// <summary>
        /// State is a DepthStencilState.
        /// </summary>
        DepthStencilState = 1,

        /// <summary>
        /// State is a RasterizerState.
        /// </summary>
        RasterizerState = 2,

        /// <summary>
        /// State is a SamplerState.
        /// </summary>
        SamplerState = 3
    }
}
