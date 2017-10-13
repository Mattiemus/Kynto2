namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics.Implementation;

    /// <summary>
    /// OpenGL implementation for <see cref="Spark.Graphics.RasterizerState"/>
    /// </summary>
    public sealed class OpenGLRasterizerStateImplementation : RasterizerStateImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLRasterizerStateImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceId">ID of the resource, supplied by the render system</param>
        public OpenGLRasterizerStateImplementation(OpenGLRenderSystem renderSystem, int resourceId)
            : base(renderSystem, resourceId)
        {
        }

        /// <summary>
        /// Gets if the <see cref="AntialiasedLineEnable" /> property is supported. This can vary by implementation.
        /// </summary>
        public override bool IsAntialiasedLineOptionSupported { get; }

        /// <summary>
        /// Gets if the <see cref="DepthClipEnable" /> property is supported. This can vary by implementation.
        /// </summary>
        public override bool IsDepthClipOptionSupported { get; }

        /// <summary>
        /// Called when the state is bound, signaling the implementation to create and bind the native state.
        /// </summary>
        protected override void CreateNativeState()
        {
            // Binding the state is a no-op in OpenGL
        }

        /// <summary>
        /// Applies the state to the given render context
        /// </summary>
        /// <param name="context">Context to apply the state to</param>
        public void ApplyState(OpenGLRenderContext context)
        {
            // TODO: Cull
            // TODO: VertexWinding
            // TODO: Fill
            // TODO: DepthBias
            // TODO: DepthBiasClamp
            // TODO: SlopeScaledDepthBias
            // TODO: DepthClipEnable
            // TODO: MultiSampleEnable
            // TODO: AntialiasedLineEnable
            // TODO: ScissorTestEnable
        }
    }
}
