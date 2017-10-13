namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics.Implementation;

    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// OpenGL implementation for <see cref="Spark.Graphics.DepthStencilState"/>
    /// </summary>
    public sealed class OpenGLDepthStencilStateImplementation : DepthStencilStateImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLDepthStencilStateImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceId">ID of the resource, supplied by the render system</param>
        public OpenGLDepthStencilStateImplementation(OpenGLRenderSystem renderSystem, int resourceId)
            : base(renderSystem, resourceId)
        {
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
            context.OpenGLState.SetEnabled(OGL.EnableCap.DepthTest, DepthEnable);

            // TODO: DepthWriteEnable
            // TODO: DepthFunction
            
            context.OpenGLState.SetEnabled(OGL.EnableCap.StencilTest, StencilEnable);
            
            // TODO: ReferenceStencil
            // TODO: StencilReadMask
            // TODO: StencilWriteMask
            // TODO: TwoSidedStencilEnable
            // TODO: StencilFunction
            // TODO: StencilDepthFail
            // TODO: StencilFail
            // TODO: StencilPass
            // TODO: BackFaceStencilFunction
            // TODO: BackFaceStencilDepthFail
            // TODO: BackFaceStencilFail
            // TODO: BackFaceStencilPass
        }
    }
}
