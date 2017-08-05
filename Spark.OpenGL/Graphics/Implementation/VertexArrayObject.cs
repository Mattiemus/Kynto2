namespace Spark.Graphics.Implementation
{
    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Vertex array object
    /// </summary>
    public sealed class VertexArrayObject : OpenGLGraphicsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexArrayObject"/> class.
        /// </summary>
        public VertexArrayObject()
        {
            ResourceId = OGL.GL.GenVertexArray();
        }

        /// <summary>
        /// Performs the dispose action
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void DisposeInternal(bool isDisposing)
        {
            if (OTK.GraphicsContext.CurrentContext == null || OTK.GraphicsContext.CurrentContext.IsDisposed)
            {
                return;
            }

            OGL.GL.DeleteVertexArray(ResourceId);
        }
    }
}
