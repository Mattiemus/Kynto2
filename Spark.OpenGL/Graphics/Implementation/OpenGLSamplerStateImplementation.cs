namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// OpenGL implementation for <see cref="SamplerState"/>
    /// </summary>
    public sealed class OpenGLSamplerStateImplementation : SamplerStateImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLSamplerStateImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceId">ID of the resource, supplied by the render system</param>
        public OpenGLSamplerStateImplementation(OpenGLRenderSystem renderSystem, int resourceId)
            : base(renderSystem, resourceId)
        {
        }

        /// <summary>
        /// Gets the number of anisotropy levels supported. This can vary by implementation.
        /// </summary>
        public override int SupportedAnisotropyLevels { get; }

        /// <summary>
        /// Checks if the specified texture addressing mode is supported by the graphics platform.
        /// </summary>
        /// <param name="mode">Texture addressing mode</param>
        /// <returns>True if supported, false otherwise.</returns>
        public override bool IsAddressingModeSupported(TextureAddressMode mode)
        {
            // TODO
            return false;
        }

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
            // TODO: AddressU
            // TODO: AddressV
            // TODO: AddressW
            // TODO: Filter
            // TODO: MaxAnisotropy
            // TODO: MipMapLevelOfDetailBias
            // TODO: MinMipMapLevel
            // TODO: MaxMipMapLevel
            // TODO: BorderColor
        }
    }
}
