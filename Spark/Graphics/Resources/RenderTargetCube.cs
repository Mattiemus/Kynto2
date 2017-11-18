namespace Spark.Graphics
{
    using System;

    using Graphics.Implementation;

    /// <summary>
    /// Represents a Cube-Dimensional texture (6 faces) where each face is a square 2D texture with width/height, and which can be used as a render target. A render target is an output 
    /// that is bound to the rendering pipeline to receive rendered data (e.g. geometry drawn to the back buffer, or to an off-screen target), which then can be 
    /// used later as a regular texture.
    /// </summary>
    /// <remarks>
    /// Each render target has a depth-stencil buffer associated with it, if the format is specified, that is bound to the graphics pipeline when the render target
    /// is also bound. In a Multi-Render Target (MRT) scenario, the first render target's depth-stencil buffer (if it has one) is used for all targets. Depth-stencil buffers
    /// can also be shared between two different targets, if it is supported.
    /// </remarks>
    public class RenderTargetCube : TextureCube, IRenderTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        protected RenderTargetCube()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size)
        {
            CreateImplementation(renderSystem, size, false, SurfaceFormat.Color, MSAADescription.Default, DepthFormat.None, false, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, MSAADescription.Default, depthFormat, false, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, RenderTargetUsage targetUsage)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, MSAADescription.Default, depthFormat, false, targetUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, preferredMSAA, depthFormat, false, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA, RenderTargetUsage targetUsage)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, preferredMSAA, depthFormat, false, targetUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA)
        {
            CreateImplementation(renderSystem, size, false, format, preferredMSAA, depthFormat, false, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA, RenderTargetUsage targetUsage)
        {
            CreateImplementation(renderSystem, size, false, format, preferredMSAA, depthFormat, false, targetUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="preferReadableDepth">True if it is preferred that the depth-stencil buffer is readable, that is if it can be bound as a shader resource, false otherwise. This may or may not be supported.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA, bool preferReadableDepth)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, preferredMSAA, depthFormat, preferReadableDepth, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="preferReadableDepth">True if it is preferred that the depth-stencil buffer is readable, that is if it can be bound as a shader resource, false otherwise. This may or may not be supported.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        public RenderTargetCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA, bool preferReadableDepth, RenderTargetUsage targetUsage)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, preferredMSAA, depthFormat, preferReadableDepth, targetUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="depthBuffer">Depth stencil buffer that is to be shared with this render target and dictate dimension and MSAA settings. It cannot be null.</param>
        public RenderTargetCube(IRenderSystem renderSystem, SurfaceFormat format, bool genMipMaps, IDepthStencilBuffer depthBuffer)
        {
            CreateImplementation(renderSystem, format, genMipMaps, depthBuffer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="depthBuffer">Depth stencil buffer that is to be shared with this render target and dictate dimension and MSAA settings. It cannot be null.</param>
        public RenderTargetCube(IRenderSystem renderSystem, SurfaceFormat format, IDepthStencilBuffer depthBuffer)
        {
            CreateImplementation(renderSystem, format, false, depthBuffer);
        }

        /// <summary>
        /// Gets the depth stencil buffer associated with the render target, if no buffer is associated then this will be null.
        /// </summary>
        public IDepthStencilBuffer DepthStencilBuffer => RenderTargetCubeImplementation.DepthStencilBuffer;

        /// <summary>
        /// Gets the depth stencil format of the associated depth buffer, if any. If this does not exist, then the format is None.
        /// </summary>
        public DepthFormat DepthStencilFormat
        {
            get
            {
                IDepthStencilBuffer depthBuffer = RenderTargetCubeImplementation.DepthStencilBuffer;
                return (depthBuffer == null) ? DepthFormat.None : depthBuffer.DepthStencilFormat;
            }
        }

        /// <summary>
        /// Gets the multisample settings for the resource. The MSAA count, quality, and if the resource should be resolved to
        /// a non-MSAA resource for shader input. MSAA targets that do not resolve to a non-MSAA resource will only ever have one mip map per array slice.
        /// </summary>
        public MSAADescription MultisampleDescription => RenderTargetCubeImplementation.MultisampleDescription;

        /// <summary>
        /// Gets the target usage, specifying how the target should be handled when it is bound to the pipeline. Generally this is
        /// set to discard by default.
        /// </summary>
        public RenderTargetUsage TargetUsage => RenderTargetCubeImplementation.TargetUsage;

        /// <summary>
        /// Gets the width of the target resource, in pixels.
        /// </summary>
        public int Width => RenderTargetCubeImplementation.Size;

        /// <summary>
        /// Gets the height of the target resource, in pixels.
        /// </summary>
        public int Height => RenderTargetCubeImplementation.Size;

        /// <summary>
        /// Gets the depth of the target resource, in pixels.
        /// </summary>
        public int Depth => 1;

        /// <summary>
        /// Gets the number of array slices in the resource. Slices may be indexed in the range [0, ArrayCount). Even if the resource is not an array resource, this may be greater than one (e.g. cube textures
        /// are a special 2D array resource with 6 slices).
        /// </summary>
        public int ArrayCount => 6;

        /// <summary>
        /// Gets if the resource is an array resource.
        /// </summary>
        public bool IsArrayResource => false;

        /// <summary>
        /// Gets if the resource is a cube resource, a special type of array resource. A cube resource has six faces. If the geometry shader stage is not supported by
        /// the render system, then all six cube faces cannot be bound as a single binding. Instead the main cube target graphics resource is treated as the very first
        /// cube face, subsequent faces must be bound individually.
        /// </summary>
        public bool IsCubeResource => true;

        /// <summary>
        /// Gets if the resource is a sub-resource, representing an individual array slice if the main resource is an array resource or an
        /// individual face if its a cube resource. If this is a sub resource, then its sub resource index indicates the position in the array/cube.
        /// </summary>
        public bool IsSubResource => false;

        /// <summary>
        /// Gets the array index if the resource is a sub resource in an array. If not, then the index is -1.
        /// </summary>
        public int SubResourceIndex => -1;

        /// <summary>
        /// Gets the render target cube implementation
        /// </summary>
        private IRenderTargetCubeImplementation RenderTargetCubeImplementation
        {
            get => Implementation as IRenderTargetCubeImplementation;
            set => BindImplementation(value);
        }

        /// <summary>
        /// Gets a sub render target of the specified face.
        /// </summary>
        /// <param name="face">Face index of the sub render target.</param>
        /// <returns>The sub render target.</returns>
        public IRenderTarget GetSubRenderTarget(CubeMapFace face)
        {
            try
            {
                return RenderTargetCubeImplementation.GetSubRenderTarget(face);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error retrieving sub render target", e);
            }
        }

        /// <summary>
        /// Gets a sub render target at the specified array index. For non-array resources, this will not be valid, unless if the resource is cube.
        /// </summary>
        /// <param name="arrayIndex">Zero-based index of the sub render target.</param>
        /// <returns>The sub render target.</returns>
        public IRenderTarget GetSubRenderTarget(int arrayIndex)
        {
            try
            {
                return RenderTargetCubeImplementation.GetSubRenderTarget((CubeMapFace)arrayIndex);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error retrieving sub render target", e);
            }
        }

        #region Creation Parameter Validation

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="size">Size of the target(width/height), in texels.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, int size, SurfaceFormat format, ref MSAADescription preferredMSAA, DepthFormat depthFormat)
        {
            if (size <= 0 || size > adapter.MaximumTextureCubeSize)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Texture dimension is out of range");
            }

            if (!adapter.CheckTextureFormat(format, TextureDimension.Cube))
            {
                throw new SparkGraphicsException("Bad texture format");
            }

            MSAADescription.CheckMSAAConfiguration(adapter, format, ref preferredMSAA);

            if (!adapter.CheckRenderTargetFormat(format, depthFormat, preferredMSAA.Count))
            {
                throw new SparkGraphicsException("Bad render target format");
            }
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="depthBuffer">Depth stencil buffer that is to be shared with this render target and dictate dimension and MSAA settings. It cannot be null.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, SurfaceFormat format, IDepthStencilBuffer depthBuffer)
        {
            if (depthBuffer == null)
            {
                throw new ArgumentNullException(nameof(depthBuffer), "Depth buffer cannot be null");
            }

            if (depthBuffer.IsArrayResource || !depthBuffer.IsCubeResource)
            {
                throw new ArgumentException("Depth buffer not valid for resource", nameof(depthBuffer));
            }

            if (!depthBuffer.IsShareable)
            {
                throw new ArgumentException("Depth buffer is not shareable", nameof(depthBuffer));
            }

            if (!adapter.CheckTextureFormat(format, TextureDimension.Cube))
            {
                throw new SparkGraphicsException("Bad texture format");
            }

            if (!adapter.CheckRenderTargetFormat(format, depthBuffer.DepthStencilFormat, depthBuffer.MultisampleDescription.Count))
            {
                throw new SparkGraphicsException("Bad render target format");
            }
        }

        #endregion

        #region Implementation Creation

        /// <summary>
        /// Creates the underlying implementation
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="format">Texture format</param>
        /// <param name="preferredMSAA">Preferred multisample description</param>
        /// <param name="depthFormat">Depth format</param>
        /// <param name="preferReadableDepth">True if a readable depth buffer is preferred, false otherwise</param>
        /// <param name="targetUsage">Target usage</param>
        private void CreateImplementation(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, MSAADescription preferredMSAA, DepthFormat depthFormat, bool preferReadableDepth, RenderTargetUsage targetUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            ValidateCreationParameters(renderSystem.Adapter, size, format, ref preferredMSAA, depthFormat);

            bool canHaveMips = (preferredMSAA.IsMultisampled && preferredMSAA.ResolveShaderResource) || !preferredMSAA.IsMultisampled;
            int mipLevels = (genMipMaps && canHaveMips) ? CalculateMipMapCount(size, size) : 1;
            
            if (!renderSystem.TryGetImplementationFactory(out IRenderTargetCubeImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                RenderTargetCubeImplementation = factory.CreateImplementation(size, mipLevels, format, preferredMSAA, depthFormat, preferReadableDepth, targetUsage);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        /// <summary>
        /// Creates the underlying implementation
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="format">Texture format</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="depthBuffer">Depth buffer</param>
        private void CreateImplementation(IRenderSystem renderSystem, SurfaceFormat format, bool genMipMaps, IDepthStencilBuffer depthBuffer)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            ValidateCreationParameters(renderSystem.Adapter, format, depthBuffer);

            MSAADescription msaa = depthBuffer.MultisampleDescription;
            bool canHaveMips = (msaa.IsMultisampled && msaa.ResolveShaderResource) || !msaa.IsMultisampled;
            int mipLevels = (genMipMaps && canHaveMips) ? CalculateMipMapCount(depthBuffer.Width, depthBuffer.Height) : 1;
            
            if (!renderSystem.TryGetImplementationFactory(out IRenderTargetCubeImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                RenderTargetCubeImplementation = factory.CreateImplementation(format, mipLevels, depthBuffer);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        #endregion
    }
}
