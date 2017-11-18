namespace Spark.Graphics
{
    /// <summary>
    /// Enumeration for vertex element usage, these correspond
    /// directly to the semantics of vertex-shader inputs. There can be a certain
    /// level of disconnect of these semantics and actual intent, since texture coordinates can
    /// be used for custom data types.
    /// </summary>
    public enum VertexSemantic
    {
        /// <summary>
        /// Vertex position info in object space.
        /// </summary>
        Position = 0,

        /// <summary>
        /// Vertex color.
        /// </summary>
        Color = 1,

        /// <summary>
        /// Vertex texture coordinate. Often used for user-defined data also.
        /// </summary>
        TextureCoordinate = 2,

        /// <summary>
        /// Vertex normal.
        /// </summary>
        Normal = 3,

        /// <summary>
        /// Vertex tangent.
        /// </summary>
        Tangent = 4,

        /// <summary>
        /// Vertex bitangent.
        /// </summary>
        Bitangent = 5,

        /// <summary>
        /// Blending indices for skinning.
        /// </summary>
        BlendIndices = 6,

        /// <summary>
        /// Blending weights for skinning.
        /// </summary>
        BlendWeight = 7,

        /// <summary>
        /// Misc user defined semantic.
        /// </summary>
        UserDefined = 8
    }
}
