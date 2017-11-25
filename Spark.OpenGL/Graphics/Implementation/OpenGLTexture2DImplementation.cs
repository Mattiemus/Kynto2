namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    
    /// <summary>
    /// OpenGL implementation for <see cref="Texture2D"/>
    /// </summary>
    public sealed class OpenGLTexture2DImplementation : OpenGLGraphicsResourceImplementation, ITexture2DImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLTexture2DImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        public OpenGLTexture2DImplementation(OpenGLRenderSystem renderSystem, int width, int height, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage)
            : base(renderSystem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLTexture2DImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="mipMapCount">Number of mip map levels, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of mip levels and each data buffer must not exceed the size of the corresponding mip level, and is
        /// permitted to be null.</param>
        public OpenGLTexture2DImplementation(OpenGLRenderSystem renderSystem, int width, int height, int mipMapCount, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
            : base(renderSystem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the format of the texture resource.
        /// </summary>
        public SurfaceFormat Format { get; }

        /// <summary>
        /// Gets the number of mip map levels in the texture resource. Mip levels may be indexed in the range of [0, MipCount).
        /// </summary>
        public int MipCount { get; }

        /// <summary>
        /// Gets the texture width, in texels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the texture height, in texels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the resource usage of the texture.
        /// </summary>
        public ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, int mipLevel, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int mipLevel, ResourceRegion2D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
        }
    }
}
