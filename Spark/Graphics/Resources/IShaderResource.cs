namespace Spark.Graphics
{
    using System;
    
    /// <summary>
    /// Defines a resource that can be bound to a shader.
    /// </summary>
    public interface IShaderResource : INamable, IDisposable
    {
        /// <summary>
        /// Gets if the resource has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets the shader resource type.
        /// </summary>
        ShaderResourceType ResourceType { get; }
    }
}
