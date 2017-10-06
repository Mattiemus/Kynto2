namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines an implementation for <see cref="DepthStencilState"/>.
    /// </summary>
    public interface IDepthStencilStateImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        bool IsBound { get; }

        /// <summary>
        /// Gets or sets if the depth buffer should be enabled. By default, this value is true.
        /// </summary>
        bool DepthEnable { get; set; }

        /// <summary>
        /// Gets or sets if the depth buffer should be writable. By default, this value is true.
        /// </summary>
        bool DepthWriteEnable { get; set; }

        /// <summary>
        /// Gets or sets the depth comparison function for the depth test. By default, this value is <see cref="ComparisonFunction.LessEqual"/>.
        /// </summary>
        ComparisonFunction DepthFunction { get; set; }

        /// <summary>
        /// Gets or sets if the stencil buffer should be enabled. By default, this value is false.
        /// </summary>
        bool StencilEnable { get; set; }

        /// <summary>
        /// Gets or sets the reference stencil value used for stencil testing. By default, this value is zero.
        /// </summary>
        int ReferenceStencil { get; set; }

        /// <summary>
        /// Gets or sets the value that identifies a portion of the depth-stencil buffer for reading stencil data. By default, this value is <see cref="int.MaxValue"/>.
        /// </summary>
        int StencilReadMask { get; set; }

        /// <summary>
        /// Gets or sets the value that identifies a portion of the depth-stencil buffer for writing stencil data. By default, this value is <see cref="int.MaxValue"/>.
        /// </summary>
        int StencilWriteMask { get; set; }

        /// <summary>
        /// Gets or sets if two sided stenciling is enabled, where if back face stencil testing/operations should be conducted in addition to the front face (as dictated by the winding order
        /// of the primitive). By default, this value is false.
        /// </summary>
        bool TwoSidedStencilEnable { get; set; }

        /// <summary>
        /// Gets or sets the comparison function used for testing a front facing triangle. By default, this value is <see cref="ComparisonFunction.Always"/>.
        /// </summary>
        ComparisonFunction StencilFunction { get; set; }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes, but the depth test fails for a front facing triangle. By default, this value is
        /// <see cref="StencilOperation.Keep"/>.
        /// </summary>
        StencilOperation StencilDepthFail { get; set; }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test fails for a front facing triangle. By default, this value is <see cref="StencilOperation.Keep"/>.
        /// </summary>
        StencilOperation StencilFail { get; set; }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes for a front facing triangle. By default, this value is <see cref="StencilOperation.Keep"/>.
        /// </summary>
        StencilOperation StencilPass { get; set; }

        /// <summary>
        /// Gets or sets the comparison function used for testing a back facing triangle. By default, this value is <see cref="ComparisonFunction.Always"/>.
        /// </summary>
        ComparisonFunction BackFaceStencilFunction { get; set; }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes, but the depth test fails for a back facing triangle. By default, this value is 
        /// <see cref="StencilOperation.Keep"/>.
        /// </summary>
        StencilOperation BackFaceStencilDepthFail { get; set; }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test fails for a back facing triangle. By default, this value is <see cref="StencilOperation.Keep"/>.
        /// </summary>
        StencilOperation BackFaceStencilFail { get; set; }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes for a back facing triangle. By default, this value is <see cref="StencilOperation.Keep"/>.
        /// </summary>
        StencilOperation BackFaceStencilPass { get; set; }

        /// <summary>
        /// Binds the implementation, creating the underlying state. Once bound the state is read-only. If unbound, this will happen
        /// automatically when the state is first used during rendering. It is best practice to do this ahead of time.
        /// </summary>
        void BindDepthStencilState();
    }
}
