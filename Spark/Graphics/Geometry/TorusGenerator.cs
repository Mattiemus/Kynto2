namespace Spark.Graphics
{
    using Math;

    public sealed class TorusGenerator : GeometryGenerator
    {
        private Vector3 _center;
        private float _radius;
        private float _thickness;
        private int _tessellation;

        public TorusGenerator() 
            : this(Vector3.Zero, 1.0f, MathHelper.TwoThird, 32, false)
        {
        }

        public TorusGenerator(float radius, float thickness) 
            : this(Vector3.Zero, radius, thickness, 32, false)
        {
        }

        public TorusGenerator(float radius, float thickness, int tessellation) 
            : this(Vector3.Zero, radius, thickness, tessellation, false)
        {
        }

        public TorusGenerator(Vector3 center, float radius, float thickness) 
            : this(center, radius, thickness, 32, false)
        {
        }

        public TorusGenerator(Vector3 center, float radius, float thickness, int tessellation) 
            : this(center, radius, thickness, tessellation, false)
        {
        }

        public TorusGenerator(Vector3 center, float radius, float thickness, int tessellation, bool insideOut)
        {
            _center = center;
            _radius = radius;
            _thickness = thickness;
            Tessellation = tessellation;
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

        public float Thickness
        {
            get => _thickness;
            set => _thickness = value;
        }

        public int Tessellation
        {
            get => _tessellation;
            set => _tessellation = (value < 3) ? 3 : value;
        }

        public override void Build(ref IDataBuffer<VertexPositionNormalTexture> vertices, ref IndexData indices)
        {
            int vertCount = (_tessellation + 1) * (_tessellation + 1);
            int indexCount = _tessellation * (_tessellation + 1) * 6;

            if (vertices == null || vertices.Length != vertCount)
            {
                vertices = new DataBuffer<VertexPositionNormalTexture>(vertCount);
            }

            if (!indices.IsValid)
            {
                indices = (Use32BitIndices) ? new IndexData(new DataBuffer<int>(indexCount)) : new IndexData(new DataBuffer<short>(indexCount));
            }

            GenerateData(vertices, ref indices, ref _center, _radius, _thickness, _tessellation, InsideOut);
        }

        public override void Build(ref IDataBuffer<Vector3> positions, ref IDataBuffer<Vector3> normals, ref IDataBuffer<Vector2> textureCoordinates, ref IndexData indices, GenerateOptions options)
        {
            int vertCount = (_tessellation + 1) * (_tessellation + 1);
            int indexCount = vertCount * 6;

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

            if (!indices.IsValid)
            {
                indices = (Use32BitIndices) ? new IndexData(new DataBuffer<int>(indexCount)) : new IndexData(new DataBuffer<short>(indexCount));
            }

            GenerateData(tempPos, tempNormals, tempTexCoordinates, ref indices, ref _center, _radius, _thickness, _tessellation, InsideOut);
        }

        private static void GenerateData(IDataBuffer<VertexPositionNormalTexture> data, ref IndexData indices, ref Vector3 center, float radius, float thickness, int tessellation, bool flip)
        {
            data.Position = 0;
            indices.Position = 0;

            int stride = tessellation + 1;

            // Loop around the main ring
            for (int i = 0; i <= tessellation; i++)
            {
                float u = (float)i / tessellation;

                Angle outerAngle = Angle.FromRadians(((i * MathHelper.TwoPi) / (float)tessellation) - MathHelper.PiOverTwo);

                Matrix4x4.FromTranslation(radius + center.X, center.Y, center.Z, out Matrix4x4 transMat);
                Matrix4x4.FromRotationY(outerAngle, out Matrix4x4 rotMat);
                Matrix4x4.Multiply(ref transMat, ref rotMat, out Matrix4x4 transform);

                // Loop along other axis, around side of the tube
                for (int j = 0; j <= tessellation; j++)
                {
                    float v = 1.0f - ((float)j / tessellation);

                    Angle innerAngle = Angle.FromRadians(((j * MathHelper.TwoPi) / tessellation) + MathHelper.Pi);
                    float dx = innerAngle.Cos;
                    float dy = innerAngle.Sin;

                    Vector3 normal = new Vector3(dx, dy, 0);
                    Vector2 texCoord = new Vector2(u, v);
                    Vector3.Multiply(ref normal, thickness * 0.5f, out Vector3 position);

                    Vector3.Transform(ref position, ref transform, out position);
                    Vector3.TransformNormal(ref normal, ref transform, out normal);

                    if (flip)
                    {
                        normal.Negate();
                    }

                    var vertex = new VertexPositionNormalTexture(position, normal, texCoord);
                    data.Set(vertex);

                    int nextI = (i + 1) % stride;
                    int nextJ = (j + 1) % stride;

                    if (flip)
                    {
                        indices.Set((nextI * stride) + j);
                        indices.Set((i * stride) + nextJ);
                        indices.Set((i * stride) + j);

                        indices.Set((nextI * stride) + j);
                        indices.Set((nextI * stride) + nextJ);
                        indices.Set((i * stride) + nextJ);
                    }
                    else
                    {
                        indices.Set((i * stride) + j);
                        indices.Set((i * stride) + nextJ);
                        indices.Set((nextI * stride) + j);

                        indices.Set((i * stride) + nextJ);
                        indices.Set((nextI * stride) + nextJ);
                        indices.Set((nextI * stride) + j);
                    }
                }
            }
        }

        private static void GenerateData(IDataBuffer<Vector3> positions, IDataBuffer<Vector3> normals, IDataBuffer<Vector2> textureCoordinates, ref IndexData indices, ref Vector3 center, float radius, float thickness, int tessellation, bool flip)
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

            indices.Position = 0;

            int stride = tessellation + 1;

            // Loop around the main ring
            for (int i = 0; i <= tessellation; i++)
            {
                float u = (float)i / tessellation;

                Angle outerAngle = Angle.FromRadians(((i * MathHelper.TwoPi) / (float)tessellation) - MathHelper.PiOverTwo);
                
                Matrix4x4.FromTranslation(radius + center.X, center.Y, center.Z, out Matrix4x4 transMat);
                Matrix4x4.FromRotationY(outerAngle, out Matrix4x4 rotMat);
                Matrix4x4.Multiply(ref transMat, ref rotMat, out Matrix4x4 transform);

                // Loop along other axis, around side of the tube
                for (int j = 0; j <= tessellation; j++)
                {
                    float v = 1.0f - ((float)j / tessellation);

                    Angle innerAngle = Angle.FromRadians(((j * MathHelper.TwoPi) / tessellation) + MathHelper.Pi);
                    float dx = innerAngle.Cos;
                    float dy = innerAngle.Sin;

                    Vector3 normal = new Vector3(dx, dy, 0);
                    Vector3.Multiply(ref normal, thickness, out Vector3 position);
                    Vector3.Multiply(ref position, 0.5f, out position);

                    if (flip)
                    {
                        normal.Negate();
                    }

                    if (positions != null)
                    {
                        Vector3.Transform(ref position, ref transform, out position);
                        positions.Set(ref position);
                    }

                    if (normals != null)
                    {
                        Vector3.TransformNormal(ref normal, ref transform, out normal);
                        normals.Set(ref normal);
                    }

                    if (textureCoordinates != null)
                    {
                        Vector2 texCoord = new Vector2(u, v);
                        textureCoordinates.Set(ref texCoord);
                    }

                    int nextI = (i + 1) % stride;
                    int nextJ = (j + 1) % stride;

                    if (flip)
                    {
                        indices.Set(nextI * stride + j);
                        indices.Set(i * stride + nextJ);
                        indices.Set(i * stride + j);

                        indices.Set(nextI * stride + j);
                        indices.Set(nextI * stride + nextJ);
                        indices.Set(i * stride + nextJ);
                    }
                    else
                    {
                        indices.Set(i * stride + j);
                        indices.Set(i * stride + nextJ);
                        indices.Set(nextI * stride + j);

                        indices.Set(i * stride + nextJ);
                        indices.Set(nextI * stride + nextJ);
                        indices.Set(nextI * stride + j);
                    }
                }
            }
        }
    }
}
