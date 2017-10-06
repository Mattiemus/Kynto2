namespace Spark.Graphics
{
    /// <summary>
    /// Defines a resource that services as a render target.
    /// </summary>
    public interface IRenderTarget : IShaderResource
    {
        /// <summary>
        /// Gets the depth stencil buffer associated with the render target, if no buffer is associated then this will be null.
        /// </summary>
        IDepthStencilBuffer DepthStencilBuffer { get; }

        /// <summary>
        /// Gets the depth stencil format of the associated depth buffer, if any. If this does not exist, then the format is None.
        /// </summary>
        DepthFormat DepthStencilFormat { get; }

        /// <summary>
        /// Gets the surface format of the target.
        /// </summary>
        SurfaceFormat Format { get; }

        /// <summary>
        /// Gets the width of the target resource, in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the target resource, in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the depth of the target resource, in pixels.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Gets the number of mip maips for each array slice in the resource. MSAA targets that do not resolve to a non-MSAA resource will only ever have one mip map per array slice.
        /// </summary>
        int MipCount { get; }

        /// <summary>
        /// Gets the number of array slices in the resource. Slices may be indexed in the range [0, ArrayCount). Even if the resource is not an array resource, this may be greater than one (e.g. cube textures
        /// are a special 2D array resource with 6 slices).
        /// </summary>
        int ArrayCount { get; }

        /// <summary>
        /// Gets if the resource is an array resource.
        /// </summary>
        bool IsArrayResource { get; }

        /// <summary>
        /// Gets if the resource is a cube resource, a special type of array resource. A cube resource has six faces. If the geometry shader stage is not supported by 
        /// the render system, then all six cube faces cannot be bound as a single binding. Instead the main cube target graphics resource is treated as the very first
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

        /// <summary>
        /// Gets a sub render target at the specified array index. For non-array resources, this will not be valid, unless if the resource is cube.
        /// </summary>
        /// <param name="arrayIndex">Zero-based index of the sub render target.</param>
        /// <returns>The sub render target.</returns>
        IRenderTarget GetSubRenderTarget(int arrayIndex);
    }
}
