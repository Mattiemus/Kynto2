namespace Spark.Graphics
{
    /// <summary>
    /// Defines different types of resources that can be bound to a shader stage.
    /// </summary>
    public enum ShaderResourceType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A buffer resource, which is a 1D resource.
        /// </summary>
        Buffer = 1,

        /// <summary>
        /// A Texture1D resource.
        /// </summary>
        Texture1D = 2,

        /// <summary>
        /// A Texture1DArray resource.
        /// </summary>
        Texture1DArray = 3,

        /// <summary>
        /// A Texture2D resource.
        /// </summary>
        Texture2D = 4,

        /// <summary>
        /// A Texture2DArray resource.
        /// </summary>
        Texture2DArray = 5,

        /// <summary>
        /// A MultiSampled Texture2D resource.
        /// </summary>
        Texture2DMS = 6,

        /// <summary>
        /// A MultiSampled Texture2DArray resource.
        /// </summary>
        Texture2DMSArray = 7,

        /// <summary>
        /// A Texture3D resource.
        /// </summary>
        Texture3D = 8,

        /// <summary>
        /// A TextureCube resource.
        /// </summary>
        TextureCube = 9,

        /// <summary>
        /// A TextureCubeArray resource.
        /// </summary>
        TextureCubeArray = 10,

        /// <summary>
        /// A MultiSampled TextureCube resource.
        /// </summary>
        TextureCubeMS = 11,

        /// <summary>
        /// A MultiSampled TextureCubeArray resource.
        /// </summary>
        TextureCubeMSArray = 12,

        /// <summary>
        /// A SamplerState resource.
        /// </summary>
        SamplerState = 13
    }
}
