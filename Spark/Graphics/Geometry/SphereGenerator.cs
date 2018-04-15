namespace Spark.Graphics
{
    using Math;

    public sealed class SphereGenerator : GeometryGenerator
    {
        private Vector3 _center;
        private float _radius;
        private int _tessellation;

        public SphereGenerator() 
            : this(Vector3.Zero, 1.0f, 16, false)
        {
        }

        public SphereGenerator(float radius) 
            : this(Vector3.Zero, radius, 16, false)
        {
        }

        public SphereGenerator(float radius, int tessellation) 
            : this(Vector3.Zero, radius, tessellation, false)
        {
        }

        public SphereGenerator(Vector3 center, float radius)
            : this(center, radius, 16, false)
        {
        }

        public SphereGenerator(Vector3 center, float radius, int tessellation) 
            : this(center, radius, tessellation, false)
        {
        }

        public SphereGenerator(Vector3 center, float radius, int tessellation, bool insideOut)
        {
            _center = center;
            _radius = radius;
            Tessellation = (tessellation < 3) ? 3 : tessellation;
            InsideOut = insideOut;
        }

        public Vector3 Center
        {
            get => _center;
            set => _center = value;
        }

        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }

        public int Tessellation
        {
            get => _tessellation;
            set => _tessellation = (value < 3) ? 3 : value;
        }

        public override void Build(ref IDataBuffer<VertexPositionNormalTexture> vertices, ref IndexData indices)
        {
            int vertSegmentCount = _tessellation;
            int horizSegmentCount = _tessellation * 2;
            int vertCount = (vertSegmentCount + 1) * (horizSegmentCount + 1);
            int indexCount = vertSegmentCount * (horizSegmentCount + 1) * 6;

            if (vertices == null || vertices.Length != vertCount)
            {
                vertices = new DataBuffer<VertexPositionNormalTexture>(vertCount);
            }

            GenerateData(vertices, ref _center, _radius, vertSegmentCount, horizSegmentCount, InsideOut);

            if (!indices.IsValid)
            {
                indices = (Use32BitIndices) ? new IndexData(new DataBuffer<int>(indexCount)) : new IndexData(new DataBuffer<short>(indexCount));
            }

            GenerateIndexData(ref indices, vertSegmentCount, horizSegmentCount, InsideOut);
        }

        public override void Build(ref IDataBuffer<Vector3> positions, ref IDataBuffer<Vector3> normals, ref IDataBuffer<Vector2> textureCoordinates, ref IndexData indices, GenerateOptions options)
        {
            int vertSegmentCount = _tessellation;
            int horizSegmentCount = _tessellation * 2;
            int vertCount = (vertSegmentCount + 1) * (horizSegmentCount + 1);
            int indexCount = vertSegmentCount * (horizSegmentCount + 1) * 6;

            IDataBuffer<Vector3> tempPos = null;
            IDataBuffer<Vector3> tempNormals = null;
            IDataBuffer<Vector2> tempTexCoordinates = null;

            if ((options & GenerateOptions.Positions) == GenerateOptions.Positions)
            {
                if (positions == null || positions.Length != vertCount)
                {
                    positions = new DataBuffer<Vector3>(vertCount);
                }

                tempPos = positions;
            }

            if ((options & GenerateOptions.Normals) == GenerateOptions.Normals)
            {
                if (normals == null || normals.Length != vertCount)
                {
                    normals = new DataBuffer<Vector3>(vertCount);
                }

                tempNormals = normals;
            }

            if ((options & GenerateOptions.TextureCoordinates) == GenerateOptions.TextureCoordinates)
            {
                if (textureCoordinates == null || textureCoordinates.Length != vertCount)
                {
                    textureCoordinates = new DataBuffer<Vector2>(vertCount);
                }

                tempTexCoordinates = textureCoordinates;
            }

            GenerateData(tempPos, tempNormals, tempTexCoordinates, ref _center, _radius, vertSegmentCount, horizSegmentCount, InsideOut);

            if (!indices.IsValid)
            {
                indices = (Use32BitIndices) ? new IndexData(new DataBuffer<int>(indexCount)) : new IndexData(new DataBuffer<short>(indexCount));
            }

            GenerateIndexData(ref indices, vertSegmentCount, horizSegmentCount, InsideOut);
        }

        private static void GenerateData(IDataBuffer<Vector3> positions, IDataBuffer<Vector3> normals, IDataBuffer<Vector2> textureCoordinates, ref Vector3 center, float radius, int vertSegmentCount, int horizSegmentCount, bool flip)
        {
            if (positions != null)
            {
                positions.Position = 0;
            }

            if (normals != null)
            {
                normals.Position = 0;
            }

            if (textureCoordinates != null)
            {
                textureCoordinates.Position = 0;
            }

            // Create rings of vertices at progressively higher latitudes
            for (int i = 0; i <= vertSegmentCount; i++)
            {
                float v = 1.0f - ((float)i / vertSegmentCount);

                Angle latitude = Angle.FromRadians(((i * MathHelper.Pi) / vertSegmentCount) - MathHelper.PiOverTwo);
                float dy = latitude.Sin;
                float dxz = latitude.Cos;

                // Create a single ring of vertices at this latitude
                for (int j = 0; j <= horizSegmentCount; j++)
                {
                    float u = (float)j / horizSegmentCount;

                    Angle longitude = Angle.FromRadians((j * 2.0f * MathHelper.Pi) / horizSegmentCount);
                    float dx = longitude.Sin;
                    float dz = longitude.Cos;

                    dx *= dxz;
                    dz *= dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);
                    Vector2 texCoords = new Vector2(u, v);
                    
                    Vector3.Multiply(ref normal, radius, out Vector3 pos);
                    Vector3.Add(ref pos, ref center, out pos);

                    if (flip)
                    {
                        normal.Negate();
                    }

                    positions?.Set(ref pos);
                    normals?.Set(ref normal);
                    textureCoordinates?.Set(ref texCoords);
                }
            }
        }

        private static void GenerateData(IDataBuffer<VertexPositionNormalTexture> data, ref Vector3 center, float radius, int vertSegmentCount, int horizSegmentCount, bool flip)
        {
            data.Position = 0;

            // Create rings of vertices at progressively higher latitudes
            for (int i = 0; i <= vertSegmentCount; i++)
            {
                float v = 1.0f - ((float)i / vertSegmentCount);

                Angle latitude = Angle.FromRadians(((i * MathHelper.Pi) / vertSegmentCount) - MathHelper.PiOverTwo);
                float dy = latitude.Sin;
                float dxz = latitude.Cos;

                // Create a single ring of vertices at this latitude
                for (int j = 0; j <= horizSegmentCount; j++)
                {
                    float u = (float)j / horizSegmentCount;

                    Angle longitude = Angle.FromRadians((j * 2.0f * MathHelper.Pi) / horizSegmentCount);
                    float dx = longitude.Sin;
                    float dz = longitude.Cos;

                    dx *= dxz;
                    dz *= dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);
                    Vector2 texCoords = new Vector2(u, v);
                    
                    Vector3.Multiply(ref normal, radius, out Vector3 pos);
                    Vector3.Add(ref pos, ref center, out pos);

                    if (flip)
                    {
                        normal.Negate();
                    }

                    var vert = new VertexPositionNormalTexture(pos, normal, texCoords);
                    data.Set(ref vert);
                }
            }
        }

        private static void GenerateIndexData(ref IndexData indices, int vertSegmentCount, int horizSegmentCount, bool flip)
        {
            indices.Position = 0;

            // Create triangles joining each pair of latitude rings
            int stride = horizSegmentCount + 1;

            if (flip)
            {
                for (int i = 0; i < vertSegmentCount; i++)
                {
                    for (int j = 0; j <= horizSegmentCount; j++)
                    {
                        int nextI = i + 1;
                        int nextJ = (j + 1) % stride;

                        indices.Set(i * stride + nextJ);
                        indices.Set(nextI * stride + j);
                        indices.Set(i * stride + j);

                        indices.Set(nextI * stride + nextJ);
                        indices.Set(nextI * stride + j);
                        indices.Set(i * stride + nextJ);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertSegmentCount; i++)
                {
                    for (int j = 0; j <= horizSegmentCount; j++)
                    {
                        int nextI = i + 1;
                        int nextJ = (j + 1) % stride;

                        indices.Set(i * stride + j);
                        indices.Set(nextI * stride + j);
                        indices.Set(i * stride + nextJ);

                        indices.Set(i * stride + nextJ);
                        indices.Set(nextI * stride + j);
                        indices.Set(nextI * stride + nextJ);
                    }
                }
            }
        }
    }
}
