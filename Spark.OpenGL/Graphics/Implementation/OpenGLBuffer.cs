namespace Spark.Graphics.Implementation
{
    using Core;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;
        
    /// <summary>
    /// Base class for a buffer
    /// </summary>
    public class OpenGLBuffer : OpenGLGraphicsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLBuffer"/> class.
        /// </summary>
        /// <param name="target"></param>
        public OpenGLBuffer(OGL.BufferTarget target)
        {
            BufferType = target;
            ResourceId = OGL.GL.GenBuffer();
            OGL.GL.BindBuffer(BufferType, ResourceId);
        }

        /// <summary>
        /// Gets the buffer type
        /// </summary>
        public OGL.BufferTarget BufferType { get; }

        /// <summary>
        /// Sets the data contained within the buffer
        /// </summary>
        /// <typeparam name="T">Type of object to set</typeparam>
        /// <param name="data">Data buffer to copy to the buffer</param>
        public void SetData<T>(IDataBuffer<T> data) where T : struct
        {
            ThrowIfDisposed();

            using (MappedDataBuffer mappedData = data.Map())
            {
                OGL.GL.NamedBufferData(ResourceId, data.SizeInBytes, mappedData.Pointer, OGL.BufferUsageHint.StaticDraw);
            }
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

            OGL.GL.DeleteBuffer(ResourceId);
        }
    }
}
