namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines an implementation for <see cref="Texture3D"/>.
    /// </summary>
    public interface ITexture3DImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets the format of the texture resource.
        /// </summary>
        SurfaceFormat Format { get; }

        /// <summary>
        /// Gets the number of mip map levels in the texture resource. Mip levels may be indexed in the range of [0, MipCount).
        /// </summary>
        int MipCount { get; }

        /// <summary>
        /// Gets the texture width, in texels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the texture height, in texels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the texture depth, in texels.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Gets the resource usage of the texture.
        /// </summary>
        ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 3D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        void GetData<T>(IDataBuffer<T> data, int mipLevel, ResourceRegion3D? subimage, int startIndex) where T : struct;

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 3D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int mipLevel, ResourceRegion3D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct;
    }
}
