namespace Spark.OpenGL.Graphics
{
    using Spark.Graphics;
    using Spark.Utilities;

    using Math;
    using Implementation;
    
    using OGL = OpenTK.Graphics.OpenGL;

    public sealed class OpenGLRenderContext : BaseDisposable, IRenderContext
    {
        private readonly VertexArrayObject _vao;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLRenderContext"/> class.
        /// </summary>
        /// <param name="renderSystem">Parent render context</param>
        public OpenGLRenderContext(OpenGLRenderSystem renderSystem)
        {
            OpenGLState = new OpenGLState();

            _vao = new VertexArrayObject();
            OGL.GL.BindVertexArray(_vao.ResourceId);
        }

        /// <summary>
        /// Gets the parent render system
        /// </summary>
        internal OpenGLRenderSystem OpenGLRenderSystem { get; }

        /// <summary>
        /// Gets the state manager instance
        /// </summary>
        internal OpenGLState OpenGLState { get; }

        /// <summary>
        /// Binds the specified vertex buffer to the first slot and the remaining slots are set to null. A value of null will unbind all currently bound buffers.
        /// </summary>
        /// <param name="vertexBuffer">Vertex buffer to bind.</param>
        public void SetVertexBuffer(VertexBufferBinding vertexBuffer)
        {
            OGL.GL.BindBuffer(OGL.BufferTarget.ArrayBuffer, vertexBuffer.VertexBuffer.ResourceId);

            OGL.GL.EnableVertexAttribArray(0);
            OGL.GL.VertexAttribPointer(0, 3, OGL.VertexAttribPointerType.Float, false, 0, 0);
        }

        /// <summary>
        /// Clears all bounded render targets to the specified color
        /// </summary>
        /// <param name="color">Color to clear to</param>
        public void Clear(LinearColor color)
        {
            Clear(ClearOptions.All, color, 1.0f, 0);
        }

        /// <summary>
        /// Clears all bounded render targets and depth buffer.
        /// </summary>
        /// <param name="options">Clear options specifying which buffer to clear.</param>
        /// <param name="color">Color to clear to</param>
        /// <param name="depth">Depth value to clear to</param>
        /// <param name="stencil">Stencil value to clear to</param>
        public void Clear(ClearOptions options, LinearColor color, float depth, int stencil)
        {
            OpenGLState.ColorBuffer.ClearValue = color;
            OpenGLState.DepthBuffer.ClearValue = depth;
            OpenGLState.StencilBuffer.ClearValue = stencil;
            OGL.GL.Clear(Utilities.GraphicsHelpers.ToNative(options));
        }

        /// <summary>
        /// Draws non-indexed, non-instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="vertexCount">Number of vertices to draw.</param>
        /// <param name="startVertexIndex">Starting index in a vertex buffer at which to read vertices from.</param>
        public void Draw(PrimitiveType primitiveType, int vertexCount, int startVertexIndex)
        {
            OGL.GL.DrawArrays(Utilities.GraphicsHelpers.ToNative(primitiveType), vertexCount, startVertexIndex);
        }

        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            _vao.Dispose();

            base.Dispose(isDisposing);
        }
    }
}
