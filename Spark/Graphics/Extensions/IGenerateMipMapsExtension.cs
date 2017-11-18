namespace Spark.Graphics
{
    /// <summary>
    /// Defines an extension functionality for a render context to generate texture mipmaps.
    /// </summary>
    public interface IGenerateMipMapsExtension : IRenderContextExtension
    {
        /// <summary>
        /// Generate mipmaps for the texture.
        /// </summary>
        /// <param name="texture">Texture to generate mipmaps for.</param>
        /// <returns>True if mipmaps were generated, false otherwise.</returns>
        bool GenerateMipMaps(Texture texture);
    }
}
