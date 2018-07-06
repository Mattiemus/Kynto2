namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="IRenderTarget2DArrayImplementation"/>.
    /// </summary>
    public sealed class D3D11RenderTarget2DArrayImplementationFactory : D3D11GraphicsResourceImplementationFactory, IRenderTarget2DArrayImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11RenderTarget2DArrayImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11RenderTarget2DArrayImplementationFactory(D3D11RenderSystem renderSystem)
            : base(renderSystem, typeof(RenderTarget2DArray))
        {
        }

        /// <summary>
        /// Gets if a render target can be created with a shared depth stencil buffer.
        /// </summary>
        public bool SupportsDepthBufferSharing => true;

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero. If the target has MSAA it only will have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth fo++rmat of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="preferReadableDepth">True if it is preferred that the depth-stencil buffer is readable, that is if it can be bound as a shader resource, false otherwise. This may or may not be supported.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        /// <returns>The render target implementation</returns>
        public IRenderTarget2DArrayImplementation CreateImplementation(int width, int height, int arrayCount, int mipMapCount, SurfaceFormat format, MSAADescription preferredMSAA, DepthFormat depthFormat, bool preferReadableDepth, RenderTargetUsage targetUsage)
        {
            return new D3D11RenderTarget2DImplementation(D3DRenderSystem, GetNextUniqueResourceId(), width, height, arrayCount, mipMapCount, format, preferredMSAA, depthFormat, preferReadableDepth, targetUsage);
        }

        /// <summary>
        /// Creates a new implementation object instance. If the depth stencil buffer is not shareable, this will fail.
        /// </summary>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero. If the target has MSAA it only will have one mip map level.</param>
        /// <param name="depthBuffer">Depth stencil buffer that is to be shared with this render target and dictate dimension and MSAA settings. It cannot be null.</param>
        /// <returns>The render target implementation</returns>
        public IRenderTarget2DArrayImplementation CreateImplementation(SurfaceFormat format, int mipMapCount, IDepthStencilBuffer depthBuffer)
        {
            D3D11DepthStencilBufferWrapper d3d11DepthBuffer = depthBuffer as D3D11DepthStencilBufferWrapper;

            if (d3d11DepthBuffer == null || d3d11DepthBuffer.D3DDevice != D3DRenderSystem.D3DDevice)
            {
                throw new SparkGraphicsException("Depth buffer is invalid");
            }

            return new D3D11RenderTarget2DImplementation(D3DRenderSystem, GetNextUniqueResourceId(), format, mipMapCount, d3d11DepthBuffer);
        }
    }
}
