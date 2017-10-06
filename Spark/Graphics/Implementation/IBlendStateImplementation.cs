namespace Spark.Graphics.Implementation
{
    using Math;

    /// <summary>
    /// Defines an implementation for <see cref="BlendState"/>.
    /// </summary>
    public interface IBlendStateImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        bool IsBound { get; }

        /// <summary>
        /// Gets the number of render targets that allow for independent blending. This can vary by implementation, at least one is always guaranteed.
        /// </summary>
        int RenderTargetBlendCount { get; }

        /// <summary>
        /// Checks if alpha-to-coverage is supported. This can vary by implementation.
        /// </summary>
        bool IsAlphaToCoverageSupported { get; }

        /// <summary>
        /// Checks if independent blending of multiple render targets (MRT) is supported. This can vary by implementation. If not supported, then the blending options
        /// specified for the first render target index are used for all other bound render targets, if those targets blend are enabled.
        /// </summary>
        bool IsIndependentBlendSupported { get; }

        /// <summary>
        /// Gets or sets whether alpha-to-coverage should be used as a multisampling technique when writing a pixel to a render target. Support for this may vary by implementation. By
        /// default, this value is false.
        /// </summary>
        bool AlphaToCoverageEnable { get; set; }

        /// <summary>
        /// Gets or sets whether independent blending is enabled for multiple render targets (MRT). If this is false, the blending options specified for the first render target index
        /// is used for all render targets currently bound. Support for this may vary by implementation. By default, this value is false.
        /// </summary>
        bool IndependentBlendEnable { get; set; }

        /// <summary>
        /// Gets or sets the blend factor color. By default, this value is <see cref="Color.White"/>.
        /// </summary>
        Color BlendFactor { get; set; }

        /// <summary>
        /// Gets or sets the multisample mask. By default, this value is 0xffffffff.
        /// </summary>
        int MultiSampleMask { get; set; }

        /// <summary>
        /// Gets the complete blend description for a render target bound at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="blendDesc">The blend description that holds the blending options for the render target.</param>
        void GetRenderTargetBlendDescription(int renderTargetIndex, out RenderTargetBlendDescription blendDesc);

        /// <summary>
        /// Sets the complete blend description for a render target bound at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="blendDesc">The blend description that holds the blending options for the render target.</param>
        void SetRenderTargetBlendDescription(int renderTargetIndex, ref RenderTargetBlendDescription blendDesc);

        /// <summary>
        /// Binds the implementation, creating the underlying state. Once bound the state is read-only. If unbound, this will happen
        /// automatically when the state is first used during rendering. It is best practice to do this ahead of time.
        /// </summary>
        void BindBlendState();
    }
}
