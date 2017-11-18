namespace Spark.Graphics
{
    using Math;

    /// <summary>
    /// Base class for geometry generators.
    /// </summary>
    public abstract class GeometryGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryGenerator"/> class.
        /// </summary>
        protected GeometryGenerator()
        {
            Use32BitIndices = false;
            InsideOut = false;
        }

        /// <summary>
        /// Gets or sets if the generator should compute 32-bit or 16-bit indices.
        /// </summary>
        public bool Use32BitIndices { get; set; }

        /// <summary>
        /// Gets or sets if the geometry should be generated "inside out", meaning normals are reversed
        /// and so are vertex windings.
        /// </summary>
        public bool InsideOut { get; set; }

        /// <summary>
        /// Builds an interleaved vertex buffer of type <see cref="VertexPositionNormalTexture"/>. To reduce garbage, existing data buffers
        /// can be passed into this method. If they are either null or not the correct size, new instances are created and returned.
        /// </summary>
        /// <param name="vertices">Vertex buffer, if null, a new one is created.</param>
        /// <param name="indices">Index buffer, if null, a new one is created.</param>
        public abstract void Build(ref IDataBuffer<VertexPositionNormalTexture> vertices, ref IndexData indices);

        /// <summary>
        /// Builds individual vertex attribute buffers, based on the specified generation options. To reduce garbage, existing data buffers
        /// can be passed into this method. If they are either null or not the correct size, new instances are created and returned.
        /// </summary>
        /// <param name="positions">Positions buffer, if null and positions are to be generated, a new one is created.</param>
        /// <param name="normals">Normals buffer, if null and normals are to be generated, a new one is created.</param>
        /// <param name="textureCoordinates">Texture coordinates buffer, if null and texture coordinates are to be generated, a new one is created.</param>
        /// <param name="indices">Index buffer, if null, a new one is always created.</param>
        /// <param name="options">Generation options for vertex data.</param>
        public abstract void Build(ref IDataBuffer<Vector3> positions, ref IDataBuffer<Vector3> normals, ref IDataBuffer<Vector2> textureCoordinates, ref IndexData indices, GenerateOptions options);

        /// <summary>
        /// Builds position and index buffers. To reduce garbage, existing data buffers
        /// can be passed into this method. If they are either null or not the correct size, new instances are created and returned.
        /// </summary>
        /// <param name="positions">Positions buffer, if null, a new one is created.</param>
        /// <param name="indices">Index buffer, if null, a new one is created.</param>
        public void Build(ref IDataBuffer<Vector3> positions, ref IndexData indices)
        {
            IDataBuffer<Vector3> normals = null;
            IDataBuffer<Vector2> texCoords = null;

            Build(ref positions, ref normals, ref texCoords, ref indices, GenerateOptions.Positions);
        }

        /// <summary>
        /// Builds position, normals, and index buffers. To reduce garbage, existing data buffers
        /// can be passed into this method. If they are either null or not the correct size, new instances are created and returned.
        /// </summary>
        /// <param name="positions">Positions buffer, if null, a new one is created.</param>
        /// <param name="normals">Normals buffer, if null, a new one is created.</param>
        /// <param name="indices">Index buffer, if null, a new one is created.</param>
        public void Build(ref IDataBuffer<Vector3> positions, ref IDataBuffer<Vector3> normals, ref IndexData indices)
        {
            IDataBuffer<Vector2> texCoords = null;

            Build(ref positions, ref normals, ref texCoords, ref indices, GenerateOptions.Positions | GenerateOptions.Normals);
        }

        /// <summary>
        /// Builds position, texture coordinate, and index buffers.To reduce garbage, existing data buffers
        /// can be passed into this method. If they are either null or not the correct size, new instances are created and returned.
        /// </summary>
        /// <param name="positions">Positions buffer, if null, a new one is created.</param>
        /// <param name="textureCoordinates">Texture coordinates buffer, if null, a new one is created.</param>
        /// <param name="indices">Index buffer, if null, a new one is created.</param>
        public void Build(ref IDataBuffer<Vector3> positions, ref IDataBuffer<Vector2> textureCoordinates, ref IndexData indices)
        {
            IDataBuffer<Vector3> normals = null;

            Build(ref positions, ref normals, ref textureCoordinates, ref indices, GenerateOptions.Positions | GenerateOptions.TextureCoordinates);
        }

        /// <summary>
        /// Builds indexed position, normal, and texture coordinate vertex data and populates the <see cref="MeshData"/> object with the data. 
        /// <remarks>
        /// If the <see cref="MeshData"/> has existing buffers, then an attempt will be made to use them, otherwise new data buffers will be created and set. 
        /// The vertex data buffers are always set  to the appropiate buffer semantics of index zero. This method does not call <see cref="MeshData.ClearData"/> 
        /// and <see cref="MeshData.Compile()"/>. Those are left to the caller in case of situations where additional data will be computed or data buffers first need to be
        /// cleared.
        /// </remarks>
        /// </summary>
        /// <param name="meshData">Meshdata to populate.</param>
        public void BuildMeshData(MeshData meshData)
        {
            BuildMeshData(meshData, GenerateOptions.All);
        }

        /// <summary>
        /// Builds indexed position, normal, and/or texture coordinate vertex data and populates the <see cref="MeshData"/> object with the data. 
        /// <remarks>
        /// If the <see cref="MeshData"/> has existing buffers, then an attempt will be made to use them, otherwise new data buffers will be created and set. 
        /// The vertex data buffers are always set  to the appropiate buffer semantics of index zero. This method does not call <see cref="MeshData.ClearData"/> 
        /// and <see cref="MeshData.Compile()"/>. Those are left to the caller in case of situations where additional data will be computed or data buffers first need to be
        /// cleared.
        /// </remarks>
        /// </summary>
        /// <param name="meshData">Meshdata to populate.</param>
        /// <param name="options">Generation options for vertex data.</param>
        public void BuildMeshData(MeshData meshData, GenerateOptions options)
        {
            if (meshData == null || options == GenerateOptions.None)
            {
                return;
            }

            IDataBuffer<Vector3> positions = ((options & GenerateOptions.Positions) == GenerateOptions.Positions) ? meshData.Positions : null;
            IDataBuffer<Vector3> normals = ((options & GenerateOptions.Normals) == GenerateOptions.Normals) ? meshData.Normals : null;
            IDataBuffer<Vector2> texCoords = ((options & GenerateOptions.TextureCoordinates) == GenerateOptions.TextureCoordinates) ? meshData.TextureCoordinates : null;
            IndexData indices = new IndexData();

            if (meshData.Indices.HasValue)
            {
                indices = meshData.Indices.Value;
                if ((Use32BitIndices && indices.IndexFormat == IndexFormat.SixteenBits) || (!Use32BitIndices && indices.IndexFormat == IndexFormat.ThirtyTwoBits))
                {
                    indices = new IndexData();
                }
            }

            Build(ref positions, ref normals, ref texCoords, ref indices, options);

            if (positions != null)
            {
                positions.Position = 0;
                meshData.Positions = positions;
            }

            if (normals != null)
            {
                normals.Position = 0;
                meshData.Normals = normals;
            }

            if (texCoords != null)
            {
                texCoords.Position = 0;
                meshData.TextureCoordinates = texCoords;
            }

            if (indices.IsValid)
            {
                indices.Position = 0;
                meshData.Indices = indices;
            }
        }

        /// <summary>
        /// Convienence method for returning the current position index from any of the input buffers, whichever is not null first.
        /// </summary>
        /// <param name="positions">Positions buffer.</param>
        /// <param name="normals">Normals buffer.</param>
        /// <param name="textureCoordinates">Texture coordinate buffer.</param>
        /// <returns>Current position index.</returns>
        protected static int GetBaseVertexIndex(IDataBuffer<Vector3> positions, IDataBuffer<Vector3> normals, IDataBuffer<Vector2> textureCoordinates)
        {
            if (positions != null)
            {
                return positions.Position;
            }

            if (normals != null)
            {
                return normals.Position;
            }

            if (textureCoordinates != null)
            {
                return textureCoordinates.Position;
            }

            return 0;
        }
    }
}
