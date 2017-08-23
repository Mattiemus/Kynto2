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
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (OTK.GraphicsContext.CurrentContext != null && !OTK.GraphicsContext.CurrentContext.IsDisposed)
            {
                OGL.GL.DeleteVertexArray(ResourceId);
            }

            base.Dispose(isDisposing);
        }
    }
}
