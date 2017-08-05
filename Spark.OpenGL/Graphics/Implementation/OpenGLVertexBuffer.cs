namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Vertex buffer object
    /// </summary>
    public sealed class OpenGLVertexBuffer : OpenGLBuffer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLVertexBuffer"/> class.
        /// </summary>
        /// <param name="vertexLayout">Vertex layout</param>
        public OpenGLVertexBuffer(VertexLayout vertexLayout)
            : base(OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer)
        {
            VertexLayout = vertexLayout;
        }

        /// <summary>
        /// Gets the vertex layout used by the buffer
        /// </summary>
        public VertexLayout VertexLayout { get; }
    }
}
