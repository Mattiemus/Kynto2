namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="IRenderTargetCubeImplementation"/>.
    /// </summary>
    public interface IRenderTargetCubeImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Gets if a render target can be created with a shared depth stencil buffer.
        /// </summary>
        bool SupportsDepthBufferSharing { get; }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="size">Size of the target (width/height), in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero. If the target has MSAA it only will have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="preferReadableDepth">True if it is preferred that the depth-stencil buffer is readable, that is if it can be bound as a shader resource, false otherwise. This may or may not be supported.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        /// <returns>The render target implementation</returns>
        IRenderTargetCubeImplementation CreateImplementation(int size, int mipMapCount, SurfaceFormat format, MSAADescription preferredMSAA, DepthFormat depthFormat, bool preferReadableDepth, RenderTargetUsage targetUsage);

        /// <summary>
        /// Creates a new implementation object instance. If the depth stencil buffer is not shareable, this will fail.
        /// </summary>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero. If the target has MSAA it only will have one mip map level.</param>
        /// <param name="depthBuffer">Depth stencil buffer that is to be shared with this render target and dictate dimension and MSAA settings. It cannot be null.</param>
        /// <returns>The render target implementation</returns>
        IRenderTargetCubeImplementation CreateImplementation(SurfaceFormat format, int mipMapCount, IDepthStencilBuffer depthBuffer);
    }
}
