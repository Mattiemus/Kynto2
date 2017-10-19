namespace Spark.Graphics.Implementation
{
    using Core;

    /// <summary>
    /// Defines an implementation for <see cref="Texture1DArray"/>.
    /// </summary>
    public interface ITexture1DArrayImplementation : ITexture1DImplementation
    {
        /// <summary>
        /// Gets the number of array slices in the texture. Slices may be indexed in the range [0, ArrayCount).
        /// </summary>
        int ArrayCount { get; }

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        void GetData<T>(IDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion1D? subimage, int startIndex) where T : struct;

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion1D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct;

        /// <summary>
        /// Gets a sub texture at the specified array index.
        /// </summary>
        /// <param name="arrayIndex">Zero-based index of the sub texture.</param>
        /// <returns>The sub texture.</returns>
        IShaderResource GetSubTexture(int arrayIndex);
    }
}
