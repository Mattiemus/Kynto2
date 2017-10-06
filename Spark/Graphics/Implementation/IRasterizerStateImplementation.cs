namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines an implementation for <see cref="RasterizerStateImplementation"/>.
    /// </summary>
    public interface IRasterizerStateImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        bool IsBound { get; }

        /// <summary>
        /// Gets if the <see cref="AntialiasedLineEnable"/> property is supported. This can vary by implementation.
        /// </summary>
        bool IsAntialiasedLineOptionSupported { get; }

        /// <summary>
        /// Gets if the <see cref="DepthClipEnable"/> property is supported. This can vary by implementation.
        /// </summary>
        bool IsDepthClipOptionSupported { get; }

        /// <summary>
        /// Gets or sets how primitives are to be culled. By default, this value is <see cref="CullMode.Back"/>.
        /// </summary>
        CullMode Cull { get; set; }

        /// <summary>
        /// Gets or sets the vertex winding of a primitive, specifying the front face of the triangle. By default, this value is <see cref="Graphics.VertexWinding.CounterClockwise"/>.
        /// </summary>
        VertexWinding VertexWinding { get; set; }

        /// <summary>
        /// Gets or sets the fill mode of a primitive. By default, this value is <see cref="FillMode.Solid"/>.
        /// </summary>
        FillMode Fill { get; set; }

        /// <summary>
        /// Gets or sets the depth bias, which is a value added to the depth value at a given pixel. By default, this value is zero.
        /// </summary>
        int DepthBias { get; set; }

        /// <summary>
        /// Gets or sets the depth bias clamp (maximum value) of a pixel. By default, this value is zero.
        /// </summary>
        float DepthBiasClamp { get; set; }

        /// <summary>
        /// Gets or sets the slope scaled depth bias, a scalar on a given pixel's slope. By default, this value is zero.
        /// </summary>
        float SlopeScaledDepthBias { get; set; }

        /// <summary>
        /// Gets or sets if depth clipping is enabled. If false, the hardware skips z-clipping. By default, this value is true.
        /// </summary>
        bool DepthClipEnable { get; set; }

        /// <summary>
        /// Gets or sets whether to use the quadrilateral or alpha line anti-aliasing algorithm on MSAA render targets. If set to true, the quadrilaterla line anti-aliasing algorithm is used.
        /// Otherwise the alpha line-anti-aliasing algorithm is used, if <see cref="MultiSampleEnable"/> is set to false and is supported. By default, this value is true;
        /// </summary>
        bool MultiSampleEnable { get; set; }

        /// <summary>
        /// Gets or sets whether to enable line antialising. This only applies if doing line drawing and <see cref="MultiSampleEnable"/> is set to false.
        /// </summary>
        bool AntialiasedLineEnable { get; set; }

        /// <summary>
        /// Gets or sets if scissor rectangle culling should be enabled or not. All pixels outside an active scissor rectangle are culled. By default, this value is set to false.
        /// </summary>
        bool ScissorTestEnable { get; set; }

        /// <summary>
        /// Binds the implementation, creating the underlying state. Once bound the state is read-only. If unbound, this will happen
        /// automatically when the state is first used during rendering. It is best practice to do this ahead of time.
        /// </summary>
        void BindRasterizerState();
    }
}
