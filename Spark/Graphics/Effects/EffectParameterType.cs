namespace Spark.Graphics
{
    /// <summary>
    /// Defines the type of an effect parameter.
    /// </summary>
    public enum EffectParameterType
    {
        /// <summary>
        /// Parameter is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Parameter is a void pointer.
        /// </summary>
        Void = 1,

        /// <summary>
        /// Parameter is a boolean.
        /// </summary>
        Bool = 2,

        /// <summary>
        /// Parameter is a 32-bit integer.
        /// </summary>
        Int32 = 3,

        /// <summary>
        /// Parameter is a 32-bit floating point.
        /// </summary>
        Single = 4,

        /// <summary>
        /// Parameter is a string.
        /// </summary>
        String = 5,

        /// <summary>
        /// Parameter is a texture.
        /// </summary>
        Texture = 6,

        /// <summary>
        /// Parameter is a 1D texture.
        /// </summary>
        Texture1D = 7,

        /// <summary>
        /// Parameter is a 1D array texture.
        /// </summary>
        Texture1DArray = 8,

        /// <summary>
        /// Parameter is a 2D texture.
        /// </summary>
        Texture2D = 9,

        /// <summary>
        /// Parameter is a 2D array texture.
        /// </summary>
        Texture2DArray = 10,

        /// <summary>
        /// Parameter is a multisampled 2D texture.
        /// </summary>
        Texture2DMS = 11,

        /// <summary>
        /// Parameter is a multisampled 2D texture.
        /// </summary>
        Texture2DMSArray = 12,

        /// <summary>
        /// Parameter is a 3D texture.
        /// </summary>
        Texture3D = 13,

        /// <summary>
        /// Parameter is a cube texture.
        /// </summary>
        TextureCube = 14,

        /// <summary>
        /// Parameter is a sampler state.
        /// </summary>
        SamplerState = 15
    }
}
