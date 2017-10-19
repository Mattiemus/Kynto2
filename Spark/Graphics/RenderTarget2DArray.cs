namespace Spark.Graphics
{
    using System;

    using Core;
    using Graphics.Implementation;

    /// <summary>
    /// Represents an array of Two-Dimensional textures, each with a width and a height, that can be used as a render target. A render target is an output that is bound to the rendering 
    /// pipeline to receive rendered data (e.g. geometry drawn to the back buffer, or to an off-screen target), which then can be used later as a regular texture.
    /// </summary>
    /// <remarks>
    /// Each render target has a depth-stencil buffer associated with it, if the format is specified, that is bound to the graphics pipeline when the render target
    /// is also bound. In a Multi-Render Target (MRT) scenario, the first render target's depth-stencil buffer (if it has one) is used for all targets. Depth-stencil buffers
    /// can also be shared between two different targets, if it is supported.
    /// </remarks>
    public class RenderTarget2DArray : Texture2DArray, IRenderTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        protected RenderTarget2DArray()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, false, SurfaceFormat.Color, MSAADescription.Default, DepthFormat.None, false, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, genMipMaps, format, MSAADescription.Default, depthFormat, false, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, RenderTargetUsage targetUsage)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, genMipMaps, format, MSAADescription.Default, depthFormat, false, targetUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, genMipMaps, format, preferredMSAA, depthFormat, false, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA, RenderTargetUsage targetUsage)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, genMipMaps, format, preferredMSAA, depthFormat, false, targetUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, false, format, preferredMSAA, depthFormat, false, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA, RenderTargetUsage targetUsage)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, false, format, preferredMSAA, depthFormat, false, targetUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="preferReadableDepth">True if it is preferred that the depth-stencil buffer is readable, that is if it can be bound as a shader resource, false otherwise. This may or may not be supported.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA, bool preferReadableDepth)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, genMipMaps, format, preferredMSAA, depthFormat, preferReadableDepth, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        /// <param name="preferReadableDepth">True if it is preferred that the depth-stencil buffer is readable, that is if it can be bound as a shader resource, false otherwise. This may or may not be supported.</param>
        /// <param name="targetUsage">Target usage, specifying how the render target should be handled when it is bound to the pipeline.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, int width, int height, int arrayCount, bool genMipMaps, SurfaceFormat format, DepthFormat depthFormat, MSAADescription preferredMSAA, bool preferReadableDepth, RenderTargetUsage targetUsage)
        {
            CreateImplementation(renderSystem, width, height, arrayCount, genMipMaps, format, preferredMSAA, depthFormat, preferReadableDepth, targetUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the render target, false otherwise (only the first mip level is created). If the target has MSAA it will only
        /// have one mip map level.</param>
        /// <param name="depthBuffer">Depth stencil buffer that is to be shared with this render target and dictate dimension and MSAA settings. It cannot be null.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, SurfaceFormat format, bool genMipMaps, IDepthStencilBuffer depthBuffer)
        {
            CreateImplementation(renderSystem, format, genMipMaps, depthBuffer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget2DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="depthBuffer">Depth stencil buffer that is to be shared with this render target and dictate dimension and MSAA settings. It cannot be null.</param>
        public RenderTarget2DArray(IRenderSystem renderSystem, SurfaceFormat format, IDepthStencilBuffer depthBuffer)
        {
            CreateImplementation(renderSystem, format, false, depthBuffer);
        }

        /// <summary>
        /// Gets the depth stencil buffer associated with the render target, if no buffer is associated then this will be null.
        /// </summary>
        public IDepthStencilBuffer DepthStencilBuffer => RenderTarget2DArrayImplementation.DepthStencilBuffer;

        /// <summary>
        /// Gets the depth stencil format of the associated depth buffer, if any. If this does not exist, then the format is None.
        /// </summary>
        public DepthFormat DepthStencilFormat
        {
            get
            {
                IDepthStencilBuffer depthBuffer = RenderTarget2DArrayImplementation.DepthStencilBuffer;
                return (depthBuffer == null) ? DepthFormat.None : depthBuffer.DepthStencilFormat;
            }
        }

        /// <summary>
        /// Gets the multisample settings for the resource. The MSAA count, quality, and if the resource should be resolved to
        /// a non-MSAA resource for shader input. MSAA targets that do not resolve to a non-MSAA resource will only ever have one mip map per array slice.
        /// </summary>
        public MSAADescription MultisampleDescription => RenderTarget2DArrayImplementation.MultisampleDescription;

        /// <summary>
        /// Gets the target usage, specifying how the target should be handled when it is bound to the pipeline. Generally this is
        /// set to discard by default.
        /// </summary>
        public RenderTargetUsage TargetUsage => RenderTarget2DArrayImplementation.TargetUsage;

        /// <summary>
        /// Gets the depth of the target resource, in pixels.
        /// </summary>
        public int Depth => 1;

        /// <summary>
        /// Gets if the resource is an array resource.
        /// </summary>
        public bool IsArrayResource => true;

        /// <summary>
        /// Gets if the resource is a cube resource, a special type of array resource. A cube resource has six faces. If the geometry shader stage is not supported by
        /// the render system, then all six cube faces cannot be bound as a single binding. Instead the main cube target graphics resource is treated as the very first
        /// cube face, subsequent faces must be bound individually.
        /// </summary>
        public bool IsCubeResource => false;

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
        /// Gets the render target 2D array implementation
        /// </summary>
        private IRenderTarget2DArrayImplementation RenderTarget2DArrayImplementation
        {
            get => Implementation as IRenderTarget2DArrayImplementation;
            set => BindImplementation(value);
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
                return RenderTarget2DArrayImplementation.GetSubRenderTarget(arrayIndex);
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
        /// <param name="width">Width of the target, in texels.</param>
        /// <param name="height">Height of the target, in texels.</param>
        /// <param name="arrayCount">Number of array slices in the texture array.</param>
        /// <param name="format">Surface format of the render target.</param>
        /// <param name="preferredMSAA">Preferred MSAA settings, if not supported the next best possible valid setting will be used.</param>
        /// <param name="depthFormat">Depth format of the depth-stencil buffer, if any. If this is set to none, no depth buffer is created.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, int width, int height, int arrayCount, SurfaceFormat format, ref MSAADescription preferredMSAA, DepthFormat depthFormat)
        {
            if (width <= 0 || width > adapter.MaximumTexture2DSize)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Texture dimension is out of range");
            }

            if (height <= 0 || height > adapter.MaximumTexture2DSize)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Texture dimension is out of range");
            }

            if (arrayCount <= 0 || arrayCount > adapter.MaximumTexture2DArrayCount)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayCount), "Texture dimension is out of range");
            }

            if (!adapter.CheckTextureFormat(format, TextureDimension.Two))
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

            if (!depthBuffer.IsArrayResource || depthBuffer.IsCubeResource)
            {
                throw new ArgumentException("Depth buffer not valid for resource", nameof(depthBuffer));
            }

            if (!depthBuffer.IsShareable)
            {
                throw new ArgumentException("Depth buffer is not shareable", nameof(depthBuffer));
            }

            if (!adapter.CheckTextureFormat(format, TextureDimension.Two))
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
        /// <param name="width">Texture width</param>
        /// <param name="height">Texture height</param>
        /// <param name="arrayCount">Number of arrays in the render target</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="format">Texture format</param>
        /// <param name="preferredMSAA">Preferred multisample description</param>
        /// <param name="depthFormat">Depth format</param>
        /// <param name="preferReadableDepth">True if a readable depth buffer is preferred, false otherwise</param>
        /// <param name="targetUsage">Target usage</param>
        private void CreateImplementation(IRenderSystem renderSystem, int width, int height, int arrayCount, bool genMipMaps, SurfaceFormat format, MSAADescription preferredMSAA, DepthFormat depthFormat, bool preferReadableDepth, RenderTargetUsage targetUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            ValidateCreationParameters(renderSystem.Adapter, width, height, arrayCount, format, ref preferredMSAA, depthFormat);

            bool canHaveMips = (preferredMSAA.IsMultisampled && preferredMSAA.ResolveShaderResource) || !preferredMSAA.IsMultisampled;
            int mipLevels = (genMipMaps && canHaveMips) ? CalculateMipMapCount(width, height) : 1;
            
            if (!renderSystem.TryGetImplementationFactory(out IRenderTarget2DArrayImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                RenderTarget2DArrayImplementation = factory.CreateImplementation(width, height, arrayCount, mipLevels, format, preferredMSAA, depthFormat, preferReadableDepth, targetUsage);
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
            
            if (!renderSystem.TryGetImplementationFactory(out IRenderTarget2DArrayImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                RenderTarget2DArrayImplementation = factory.CreateImplementation(format, mipLevels, depthBuffer);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        #endregion
    }
}
