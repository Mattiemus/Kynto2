namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using Math;

    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// OpenGL implementation for <see cref="BlendState"/>
    /// </summary>
    public sealed class OpenGLBlendStateImplementation : BlendStateImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLBlendStateImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceId">ID of the resource, supplied by the render system</param>
        public OpenGLBlendStateImplementation(OpenGLRenderSystem renderSystem, int resourceId)
            : base(renderSystem, resourceId)
        {
        }

        /// <summary>
        /// Gets the number of render targets that allow for independent blending. This can vary by implementation, at least one is always guaranteed.
        /// </summary>
        public override int RenderTargetBlendCount => OGL.GL.GetInteger(OGL.GetPName.MaxDrawBuffers);

        /// <summary>
        /// Checks if alpha-to-coverage is supported. This can vary by implementation.
        /// </summary>
        public override bool IsAlphaToCoverageSupported => OGL.GL.GetBoolean(OGL.GetPName.SampleAlphaToCoverage);

        /// <summary>
        /// Checks if independent blending of multiple render targets (MRT) is supported. This can vary by implementation. If not supported, then the blending options
        /// specified for the first render target index are used for all other bound render targets, if those targets blend are enabled.
        /// </summary>
        public override bool IsIndependentBlendSupported => true;

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
            context.OpenGLState.SetEnabled(OGL.EnableCap.SampleAlphaToCoverage, AlphaToCoverageEnable);

            Vector4 blendColor = BlendFactor.ToVector4();
            OGL.GL.BlendColor(blendColor.X, blendColor.Y, blendColor.Z, blendColor.W);

            // TODO: MultiSampleMask
                                    
            for (int i = 0; i < RenderTargetBlendCount; i++)
            {
                RenderTargetBlendDescription desc;
                if (IndependentBlendEnable)
                {
                    GetRenderTargetBlendDescription(i, out desc);
                }
                else
                {
                    GetRenderTargetBlendDescription(0, out desc);
                }

                if (desc.BlendEnable)
                {
                    OGL.GL.Enable(OGL.IndexedEnableCap.Blend, i);
                }
                else
                {
                    OGL.GL.Disable(OGL.IndexedEnableCap.Blend, i);
                }

                if (desc.BlendEnable)
                {
                    OGL.BlendEquationMode alphaBlendFunc = OpenGLHelper.ToNative(desc.AlphaBlendFunction);
                    OGL.BlendEquationMode colorBlendFunc = OpenGLHelper.ToNative(desc.ColorBlendFunction);
                    OGL.GL.BlendEquationSeparate(i, colorBlendFunc, alphaBlendFunc);

                    OGL.BlendingFactorSrc alphaSrc = OpenGLHelper.ToNativeSource(desc.AlphaSourceBlend);
                    OGL.BlendingFactorDest alphaDest = OpenGLHelper.ToNativeDest(desc.AlphaDestinationBlend);
                    OGL.BlendingFactorSrc colorSrc = OpenGLHelper.ToNativeSource(desc.ColorDestinationBlend);
                    OGL.BlendingFactorDest colorDest = OpenGLHelper.ToNativeDest(desc.ColorDestinationBlend);
                    OGL.GL.BlendFuncSeparate(i, colorSrc, colorDest, alphaSrc, alphaDest);
                    
                    OGL.GL.ColorMask(desc.WriteChannels.HasFlag(ColorWriteChannels.Red),
                                     desc.WriteChannels.HasFlag(ColorWriteChannels.Green),
                                     desc.WriteChannels.HasFlag(ColorWriteChannels.Blue),
                                     desc.WriteChannels.HasFlag(ColorWriteChannels.Alpha));
                }
            }
        }
    }
}

