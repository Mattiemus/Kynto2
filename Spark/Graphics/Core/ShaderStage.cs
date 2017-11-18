namespace Spark.Graphics
{
    /// <summary>
    /// Enumerates different shader stage types in the programmable shader pipeline.
    /// </summary>
    public enum ShaderStage
    {
        /// <summary>
        /// The vertex shader stage, which processes individual vertices.
        /// </summary>
        VertexShader = 0,

        /// <summary>
        /// The hull shader stage, which produces a geometry patch for tessellation.
        /// </summary>
        HullShader = 1,

        /// <summary>
        /// The  domain shader stage, which calculates vertex positions of subdivided points in the output patch for tessellation.
        /// </summary>
        DomainShader = 2,

        /// <summary>
        /// The geometry shader stage, which processes entire primitives.
        /// </summary>
        GeometryShader = 3,

        /// <summary>
        /// The pixel shader stage, which processes fragments of rasterized primitives.
        /// </summary>
        PixelShader = 4,

        /// <summary>
        /// The compute shader stage, a specialized shader that can do general computations on the GPU.
        /// </summary>
        ComputeShader = 5
    }
}
