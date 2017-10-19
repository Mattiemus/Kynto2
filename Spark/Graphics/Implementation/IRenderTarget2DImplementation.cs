namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines an implementation for <see cref="RenderTarget2D"/>.
    /// </summary>
    public interface IRenderTarget2DImplementation : ITexture2DImplementation
    {
        /// <summary>
        /// Gets the depth stencil buffer associated with the render target, if no buffer is associated then this will be null.
        /// </summary>
        IDepthStencilBuffer DepthStencilBuffer { get; }

        /// <summary>
        /// Gets the multisample settings for the resource. The MSAA count, quality, and if the resource should be resolved to
        /// a non-MSAA resource for shader input. MSAA targets that do not resolve to a non-MSAA resource will only ever have one mip map per array slice.
        /// </summary>
        MSAADescription MultisampleDescription { get; }

        /// <summary>
        /// Gets the target usage, specifying how the target should be handled when it is bound to the pipeline. Generally this is
        /// set to discard by default.
        /// </summary>
        RenderTargetUsage TargetUsage { get; }
    }
}
