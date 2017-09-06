namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Utilities;

    /// <summary>
    /// Base implementation of a graphics resource
    /// </summary>
    public abstract class OpenGLGraphicsResource : BaseDisposable
    {
        /// <summary>
        /// Gets the resource id
        /// </summary>
        public int ResourceId { get; protected set; }
    }
}
