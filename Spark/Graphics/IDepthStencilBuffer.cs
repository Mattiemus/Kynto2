namespace Spark.Graphics
{
    /// <summary>
    /// Defines a resource that serves as a depth stencil buffer. Depth buffers are generally created alongside a render target.
    /// </summary>
    public interface IDepthStencilBuffer : IShaderResource
    {
        /// <summary>
        /// Gets if the depth stencil buffer is readable, meaning it can be used as input for a shader.
        /// </summary>
        bool IsReadable { get; }

        /// <summary>
        /// Gets if the depth stencil buffer can be shared between multiple render targets. if this is true, then render targets can be initialized using this
        /// depth stencil buffer, otherwise an exception will be thrown.
        /// </summary>
        bool IsShareable { get; }

        /// <summary>
        /// Gets the depth stencil format of the buffer.
        /// </summary>
        DepthFormat DepthStencilFormat { get; }

        /// <summary>
        /// Gets the width of the buffer.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the buffer.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the number of array slices in the resource. Even if the resource is not an array resource, this may be greater than one (e.g. cube textures
        /// are a special 2D array resource with 6 slices).
        /// </summary>
        int ArrayCount { get; }

        /// <summary>
        /// Gets if the resource is an array resource.
        /// </summary>
        bool IsArrayResource { get; }

        /// <summary>
        /// Gets if the resource is a cube resource, a special type of array resource. A cube resource has six faces. If the geometry shader stage is not supported by 
        /// the render system, then all six cube faces cannot be bound as a single binding. Instead the main cube depth buffer resource is treated as the very first
        /// cube face, subsequent faces must be bound individually.
        /// </summary>
        bool IsCubeResource { get; }

        /// <summary>
        /// Gets if the resource is a sub-resource, representing an individual array slice if the main resource is an array resource or an
        /// individual face if its a cube resource. If this is a sub resource, then its sub resource index indicates the position in the array/cube.
        /// </summary>
        bool IsSubResource { get; }

        /// <summary>
        /// Gets the array index if the resource is a sub resource in an array. If not, then the index is -1.
        /// </summary>
        int SubResourceIndex { get; }

        /// <summary>
        /// Gets the multisample settings for the resource. The MSAA count, quality, and if the resource should be resolved to
        /// a non-MSAA resource for shader input. MSAA targets that do not resolve to a non-MSAA resource will only ever have one mip map per array slice.
        /// </summary>
        MSAADescription MultisampleDescription { get; }

        /// <summary>
        /// Gets the target usage, specifying how the target should be handled when it is bound to the pipeline. Generally this is
        /// set to discard by default.
        /// </summary>
        RenderTargetUsage TargetUsage { get; }
    }
}
