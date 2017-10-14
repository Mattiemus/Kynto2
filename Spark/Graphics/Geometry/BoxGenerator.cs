namespace Spark.Graphics.Geometry
{
    using Core;
    using Math;

    public sealed class BoxGenerator : GeometryGenerator
    {
        private const int MaxIndices = 36;
        private const int MaxVertices = 24;

        private Triad _axes;
        private Vector3 _center;
        private Vector3 _extents;

        public BoxGenerator() 
            : this(Triad.UnitAxes, Vector3.Zero, Vector3.One, false)
        {
        }

        public BoxGenerator(Vector3 extents) 
            : this(Triad.UnitAxes, Vector3.Zero, extents, false)
        {
        }

        public BoxGenerator(Triad axes, Vector3 extents) 
            : this(axes, Vector3.Zero, extents, false)
        {
        }

        public BoxGenerator(Vector3 center, Vector3 extents) 
            : this(Triad.UnitAxes, center, extents, false)
        {
        }

        public BoxGenerator(Triad axes, Vector3 center, Vector3 extents) 
            : this(axes, center, extents, false)
        {
        }

        public BoxGenerator(Triad axes, Vector3 center, Vector3 extents, bool insideOut)
        {
            _axes = axes;
            _center = center;
            _extents = extents;
            InsideOut = insideOut;
        }

        public Triad Axes
        {
            get
            {
                return _axes;
            }
            set
            {
                _axes = value;
            }
        }

        public Vector3 Center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
            }
        }

        public Vector3 Extents
        {
            get
            {
                return _extents;
            }
            set
            {
                _extents = value;
            }
        }

        /// <summary>
        /// Builds an interleaved vertex buffer of type <see cref="VertexPositionNormalTexture"/>. To reduce garbage, existing data buffers
        /// can be passed into this method. If they are either null or not the correct size, new instances are created and returned.
        /// </summary>
        /// <param name="vertices">Vertex buffer, if null, a new one is created.</param>
        /// <param name="indices">Index buffer, if null, a new one is created.</param>
        public override void Build(ref IDataBuffer<VertexPositionNormalTexture> vertices, ref IndexData indices)
        {
            if (vertices == null || vertices.Length != MaxVertices)
            {
                vertices = new DataBuffer<VertexPositionNormalTexture>(MaxVertices);
            }

            GenerateData(vertices, ref _axes, ref _center, ref _extents, InsideOut);

            if (!indices.IsValid)
            {
                indices = (Use32BitIndices) ? new IndexData(new DataBuffer<int>(MaxIndices)) : new IndexData(new DataBuffer<short>(MaxIndices));
            }

            GenerateIndexData(ref indices, InsideOut);
        }

        /// <summary>
        /// Builds individual vertex attribute buffers, based on the specified generation options. To reduce garbage, existing data buffers
        /// can be passed into this method. If they are either null or not the correct size, new instances are created and returned.
        /// </summary>
        /// <param name="positions">Positions buffer, if null and positions are to be generated, a new one is created.</param>
        /// <param name="normals">Normals buffer, if null and normals are to be generated, a new one is created.</param>
        /// <param name="textureCoordinates">Texture coordinates buffer, if null and texture coordinates are to be generated, a new one is created.</param>
        /// <param name="indices">Index buffer, if null, a new one is always created.</param>
        /// <param name="options">Generation options for vertex data.</param>
        public override void Build(ref IDataBuffer<Vector3> positions, ref IDataBuffer<Vector3> normals, ref IDataBuffer<Vector2> textureCoordinates, ref IndexData indices, GenerateOptions options)
        {
            if ((options & GenerateOptions.Positions) == GenerateOptions.Positions)
            {
                if (positions == null || positions.Length != MaxVertices)
                {
                    positions = new DataBuffer<Vector3>(MaxVertices);
                }

                GeneratePositionData(positions, ref _axes, ref _center, ref _extents);
            }

            if ((options & GenerateOptions.Normals) == GenerateOptions.Normals)
            {
                if (normals == null || normals.Length != MaxVertices)
                {
                    normals = new DataBuffer<Vector3>(MaxVertices);
                }

                GenerateNormalData(normals, InsideOut);
            }

            if ((options & GenerateOptions.TextureCoordinates) == GenerateOptions.TextureCoordinates)
            {
                if (textureCoordinates == null || textureCoordinates.Length != MaxVertices)
                {
                    textureCoordinates = new DataBuffer<Vector2>(MaxVertices);
                }

                GenerateTextureData(textureCoordinates, InsideOut);
            }

            if (!indices.IsValid)
            {
                indices = (Use32BitIndices) ? new IndexData(new DataBuffer<int>(MaxIndices)) : new IndexData(new DataBuffer<short>(MaxIndices));
            }

            GenerateIndexData(ref indices, InsideOut);
        }

        private static void ComputeVertices(ref Triad axes, ref Vector3 center, ref Vector3 extents, out Vector3 v0, out Vector3 v1, out Vector3 v2, out Vector3 v3, out Vector3 v4, out Vector3 v5, out Vector3 v6, out Vector3 v7)
        {
            Triad ExAxes;
            Vector3.Multiply(ref axes.XAxis, extents.X, out ExAxes.XAxis);
            Vector3.Multiply(ref axes.YAxis, extents.Y, out ExAxes.YAxis);
            Vector3.Multiply(ref axes.ZAxis, extents.Z, out ExAxes.ZAxis);

            // Use scaled axes to computer the corners
            Vector3.Subtract(ref center, ref ExAxes.XAxis, out Vector3 temp);
            Vector3.Add(ref temp, ref ExAxes.YAxis, out temp);
            Vector3.Add(ref temp, ref ExAxes.ZAxis, out v0);

            Vector3.Add(ref center, ref ExAxes.XAxis, out temp);
            Vector3.Add(ref temp, ref ExAxes.YAxis, out temp);
            Vector3.Add(ref temp, ref ExAxes.ZAxis, out v1);

            Vector3.Add(ref center, ref ExAxes.XAxis, out temp);
            Vector3.Subtract(ref temp, ref ExAxes.YAxis, out temp);
            Vector3.Add(ref temp, ref ExAxes.ZAxis, out v2);

            Vector3.Subtract(ref center, ref ExAxes.XAxis, out temp);
            Vector3.Subtract(ref temp, ref ExAxes.YAxis, out temp);
            Vector3.Add(ref temp, ref ExAxes.ZAxis, out v3);

            Vector3.Add(ref center, ref ExAxes.XAxis, out temp);
            Vector3.Add(ref temp, ref ExAxes.YAxis, out temp);
            Vector3.Subtract(ref temp, ref ExAxes.ZAxis, out v4);

            Vector3.Subtract(ref center, ref ExAxes.XAxis, out temp);
            Vector3.Add(ref temp, ref ExAxes.YAxis, out temp);
            Vector3.Subtract(ref temp, ref ExAxes.ZAxis, out v5);

            Vector3.Subtract(ref center, ref ExAxes.XAxis, out temp);
            Vector3.Subtract(ref temp, ref ExAxes.YAxis, out temp);
            Vector3.Subtract(ref temp, ref ExAxes.ZAxis, out v6);

            Vector3.Add(ref center, ref ExAxes.XAxis, out temp);
            Vector3.Subtract(ref temp, ref ExAxes.YAxis, out temp);
            Vector3.Subtract(ref temp, ref ExAxes.ZAxis, out v7);
        }

        private static void GenerateData(IDataBuffer<VertexPositionNormalTexture> data, ref Triad axes, ref Vector3 center, ref Vector3 extents, bool flip)
        {
            data.Position = 0;

            // Setup position data
            ComputeVertices(ref axes, ref center, ref extents, 
                out Vector3 v0, out Vector3 v1, 
                out Vector3 v2, out Vector3 v3,
                out Vector3 v4, out Vector3 v5, 
                out Vector3 v6, out Vector3 v7);

            // Setup normal data
            Vector3 xDir, yDir, zDir, negXDir, negYDir, negZDir;
            if (flip)
            {
                xDir = new Vector3(-1, 0, 0);
                yDir = new Vector3(0, -1, 0);
                zDir = new Vector3(0, 0, -1);

                negXDir = new Vector3(1, 0, 0);
                negYDir = new Vector3(0, 1, 0);
                negZDir = new Vector3(0, 0, 1);
            }
            else
            {
                xDir = new Vector3(1, 0, 0);
                yDir = new Vector3(0, 1, 0);
                zDir = new Vector3(0, 0, 1);

                negXDir = new Vector3(-1, 0, 0);
                negYDir = new Vector3(0, -1, 0);
                negZDir = new Vector3(0, 0, -1);
            }

            // Setup UV data
            Vector2 uv0, uv1, uv2, uv3;
            if (flip)
            {
                uv0 = new Vector2(1, 0);
                uv1 = new Vector2(0, 0);
                uv2 = new Vector2(0, 1);
                uv3 = new Vector2(1, 1);
            }
            else
            {
                uv0 = new Vector2(0, 0);
                uv1 = new Vector2(1, 0);
                uv2 = new Vector2(1, 1);
                uv3 = new Vector2(0, 1);
            }

            // Set vertex data
            // Front
            data.Set(new VertexPositionNormalTexture(v0, zDir, uv0));
            data.Set(new VertexPositionNormalTexture(v1, zDir, uv1));
            data.Set(new VertexPositionNormalTexture(v2, zDir, uv2));
            data.Set(new VertexPositionNormalTexture(v3, zDir, uv3));

            // Back
            data.Set(new VertexPositionNormalTexture(v4, negZDir, uv0));
            data.Set(new VertexPositionNormalTexture(v5, negZDir, uv1));
            data.Set(new VertexPositionNormalTexture(v6, negZDir, uv2));
            data.Set(new VertexPositionNormalTexture(v7, negZDir, uv3));

            // Left
            data.Set(new VertexPositionNormalTexture(v5, negXDir, uv0));
            data.Set(new VertexPositionNormalTexture(v0, negXDir, uv1));
            data.Set(new VertexPositionNormalTexture(v3, negXDir, uv2));
            data.Set(new VertexPositionNormalTexture(v6, negXDir, uv3));

            // Right
            data.Set(new VertexPositionNormalTexture(v1, xDir, uv0));
            data.Set(new VertexPositionNormalTexture(v4, xDir, uv1));
            data.Set(new VertexPositionNormalTexture(v7, xDir, uv2));
            data.Set(new VertexPositionNormalTexture(v2, xDir, uv3));

            // Top
            data.Set(new VertexPositionNormalTexture(v5, yDir, uv0));
            data.Set(new VertexPositionNormalTexture(v4, yDir, uv1));
            data.Set(new VertexPositionNormalTexture(v1, yDir, uv2));
            data.Set(new VertexPositionNormalTexture(v0, yDir, uv3));

            // Bottom
            data.Set(new VertexPositionNormalTexture(v7, negYDir, uv0));
            data.Set(new VertexPositionNormalTexture(v6, negYDir, uv1));
            data.Set(new VertexPositionNormalTexture(v3, negYDir, uv2));
            data.Set(new VertexPositionNormalTexture(v2, negYDir, uv3));
        }

        private static void GeneratePositionData(IDataBuffer<Vector3> positions, ref Triad axes, ref Vector3 center, ref Vector3 extents)
        {
            positions.Position = 0;
            
            ComputeVertices(ref axes, ref center, ref extents, 
                out Vector3 v0, out Vector3 v1, 
                out Vector3 v2, out Vector3 v3, 
                out Vector3 v4, out Vector3 v5, 
                out Vector3 v6, out Vector3 v7);

            // Front
            positions.Set(ref v0);
            positions.Set(ref v1);
            positions.Set(ref v2);
            positions.Set(ref v3);

            // Back
            positions.Set(ref v4);
            positions.Set(ref v5);
            positions.Set(ref v6);
            positions.Set(ref v7);

            // Left
            positions.Set(ref v5);
            positions.Set(ref v0);
            positions.Set(ref v3);
            positions.Set(ref v6);

            // Right
            positions.Set(ref v1);
            positions.Set(ref v4);
            positions.Set(ref v7);
            positions.Set(ref v2);

            // Top
            positions.Set(ref v5);
            positions.Set(ref v4);
            positions.Set(ref v1);
            positions.Set(ref v0);

            // Bottom
            positions.Set(ref v7);
            positions.Set(ref v6);
            positions.Set(ref v3);
            positions.Set(ref v2);
        }

        private static void GenerateNormalData(IDataBuffer<Vector3> normals, bool flip)
        {
            normals.Position = 0;

            Vector3 xDir, yDir, zDir, negXDir, negYDir, negZDir;
            if (flip)
            {
                xDir = new Vector3(-1, 0, 0);
                yDir = new Vector3(0, -1, 0);
                zDir = new Vector3(0, 0, -1);

                negXDir = new Vector3(1, 0, 0);
                negYDir = new Vector3(0, 1, 0);
                negZDir = new Vector3(0, 0, 1);
            }
            else
            {
                xDir = new Vector3(1, 0, 0);
                yDir = new Vector3(0, 1, 0);
                zDir = new Vector3(0, 0, 1);

                negXDir = new Vector3(-1, 0, 0);
                negYDir = new Vector3(0, -1, 0);
                negZDir = new Vector3(0, 0, -1);
            }

            // Front
            for (int i = 0; i < 4; i++)
            {
                normals.Set(ref zDir);
            }

            // Back
            for (int i = 4; i < 8; i++)
            {
                normals.Set(ref negZDir);
            }

            // Left
            for (int i = 8; i < 12; i++)
            {
                normals.Set(ref negXDir);
            }

            // Right
            for (int i = 12; i < 16; i++)
            {
                normals.Set(ref xDir);
            }

            // Top
            for (int i = 16; i < 20; i++)
            {
                normals.Set(ref yDir);
            }

            // Bottom
            for (int i = 20; i < 24; i++)
            {
                normals.Set(ref negYDir);
            }
        }

        private static void GenerateTextureData(IDataBuffer<Vector2> texCoords, bool flip)
        {
            texCoords.Position = 0;

            Vector2 uv0, uv1, uv2, uv3;
            if (flip)
            {
                uv0 = new Vector2(1, 0);
                uv1 = new Vector2(0, 0);
                uv2 = new Vector2(0, 1);
                uv3 = new Vector2(1, 1);
            }
            else
            {
                uv0 = new Vector2(0, 0);
                uv1 = new Vector2(1, 0);
                uv2 = new Vector2(1, 1);
                uv3 = new Vector2(0, 1);
            }

            for (int i = 0; i < 6; i++)
            {
                texCoords.Set(ref uv0);
                texCoords.Set(ref uv1);
                texCoords.Set(ref uv2);
                texCoords.Set(ref uv3);
            }
        }

        private static void GenerateIndexData(ref IndexData indices, bool flip)
        {
            indices.Position = 0;

            if (flip)
            {
                for (int i = 0; i < MaxVertices; i += 4)
                {
                    indices.Set(i + 2);
                    indices.Set(i + 1);
                    indices.Set(i);

                    indices.Set(i + 3);
                    indices.Set(i + 2);
                    indices.Set(i);
                }
            }
            else
            {
                for (int i = 0; i < MaxVertices; i += 4)
                {
                    indices.Set(i);
                    indices.Set(i + 1);
                    indices.Set(i + 2);

                    indices.Set(i);
                    indices.Set(i + 2);
                    indices.Set(i + 3);
                }
            }
        }
    }
}
