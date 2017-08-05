namespace Spark.Graphics.Implementation
{
    using Utilities;

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
