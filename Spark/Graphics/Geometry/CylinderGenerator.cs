namespace Spark.Graphics
{
    using System;

    using Math;

    public sealed class CylinderGenerator : GeometryGenerator
    {
        private Segment _centerLine;
        private float _topRadius;
        private float _bottomRadius;
        private int _vertSegmentCount;
        private int _horizSegmentCount;

        public CylinderGenerator() 
            : this(new Segment(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f)), 0.5f, 0.5f, 8, false)
        {
        }

        public CylinderGenerator(Segment centerLine, float radius) 
            : this(centerLine, radius, radius, 8, false)
        {
        }

        public CylinderGenerator(Segment centerLine, float radius, int tessellation) 
            : this(centerLine, radius, radius, tessellation, false)
        {
        }

        public CylinderGenerator(Segment centerLine, float radius, int tessellation, bool insideOut) 
            : this(centerLine, radius, radius, tessellation, insideOut)
        {
        }
        
        public CylinderGenerator(Segment centerLine, float bottomRadius, float topRadius) 
            : this(centerLine, bottomRadius, topRadius, 8, false)
        {
        }

        public CylinderGenerator(Segment centerLine, float bottomRadius, float topRadius, int tessellation) 
            : this(centerLine, bottomRadius, topRadius, tessellation, false)
        {
        }

        public CylinderGenerator(Segment centerLine, float bottomRadius, float topRadius, int tessellation, bool insideOut)
        {
            _centerLine = centerLine;
            _bottomRadius = bottomRadius;
            _topRadius = topRadius;
            SetTessellation(tessellation);
            InsideOut = insideOut;
        }

        public Segment CenterLine
        {
            get => _centerLine;
            set => _centerLine = value;
        }

        public float TopRadius
        {
            get => _topRadius;
            set => _topRadius = value;
        }

        public float BottomRadius
        {
            get => _bottomRadius;
            set => _bottomRadius = value;
        }

        public int VerticalSegmentCount
        {
            get => _vertSegmentCount;
            set => _vertSegmentCount = (value < 1) ? 1 : value;
        }

        public int HorizontalSegmentCount
        {
            get => _horizSegmentCount;
            set => _horizSegmentCount = (value < 3) ? 3 : value;
        }

        public void SetTessellation(int tessellation)
        {
            VerticalSegmentCount = tessellation;
            HorizontalSegmentCount = tessellation * 2;
        }

        public override void Build(ref IDataBuffer<VertexPositionNormalTexture> vertices, ref IndexData indices)
        {
            int vertCount = (_vertSegmentCount + 1) * (_horizSegmentCount + 1);
            int indexCount = _vertSegmentCount * _horizSegmentCount * 6;

            if (!MathHelper.IsApproxZero(_topRadius))
            {
                vertCount += _horizSegmentCount + 1;
                indexCount += _horizSegmentCount * 3;
            }

            if (!MathHelper.IsApproxZero(_bottomRadius))
            {
                vertCount += _horizSegmentCount + 1;
                indexCount += _horizSegmentCount * 3;
            }

            if (vertices == null || vertices.Length != vertCount)
            {
                vertices = new DataBuffer<VertexPositionNormalTexture>(vertCount);
            }

            if (!indices.IsValid)
            {
                indices = (Use32BitIndices) ? new IndexData(new DataBuffer<int>(indexCount)) : new IndexData(new DataBuffer<short>(indexCount));
            }

            GenerateData(vertices, ref indices, ref _centerLine, _topRadius, _bottomRadius, _vertSegmentCount, _horizSegmentCount, InsideOut);
        }

        public override void Build(ref IDataBuffer<Vector3> positions, ref IDataBuffer<Vector3> normals, ref IDataBuffer<Vector2> textureCoordinates, ref IndexData indices, GenerateOptions options)
        {
            int vertCount = (_vertSegmentCount + 1) * (_horizSegmentCount + 1);
            int indexCount = _vertSegmentCount * _horizSegmentCount * 6;

            if (!MathHelper.IsApproxZero(_topRadius))
            {
                vertCount += _horizSegmentCount + 1;
                indexCount += _horizSegmentCount * 3;
            }

            if (!MathHelper.IsApproxZero(_bottomRadius))
            {
                vertCount += _horizSegmentCount + 1;
                indexCount += _horizSegmentCount * 3;
            }

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

            GenerateData(tempPos, tempNormals, tempTexCoordinates, ref indices, ref _centerLine, _topRadius, _bottomRadius, _vertSegmentCount, _horizSegmentCount, InsideOut);
        }

        private static void GenerateData(IDataBuffer<VertexPositionNormalTexture> vertices, ref IndexData indices, ref Segment centerLine, float topRadius, float bottomRadius, int vertSegmentCount, int horizSegmentCount, bool flip)
        {
            vertices.Position = 0;
            indices.Position = 0;

            Triad axes = Triad.FromZComplementBasis(-centerLine.Direction); // Cylinder slices exist on the XY plane with Z being the vertical axis

            float sliceAmt = MathHelper.Pi / horizSegmentCount;

            // Create rings of vertices at progressively higher elevations
            for (int i = 0; i <= vertSegmentCount; i++)
            {
                float elevationPercent = (float)i / vertSegmentCount;
                float v = 1.0f - elevationPercent;
                float radius = MathHelper.Lerp(bottomRadius, topRadius, elevationPercent);
                
                Vector3.Lerp(ref centerLine.StartPoint, ref centerLine.EndPoint, elevationPercent, out Vector3 center);

                // Create a single ring of vertices at this elevation
                for (int j = 0; j <= horizSegmentCount; j++)
                {
                    float u = (float)j / horizSegmentCount;
                    
                    GetCylinderNormal(sliceAmt, j, ref axes, out Vector3 normal);

                    VertexPositionNormalTexture vert;
                    Vector3.Multiply(ref normal, radius, out vert.Position);
                    Vector3.Add(ref vert.Position, ref center, out vert.Position);

                    if (flip)
                    {
                        normal.Negate();
                    }

                    vert.Normal = normal;
                    vert.TextureCoordinate = new Vector2(u, v);

                    vertices.Set(ref vert);
                }
            }

            // Generate indices
            int stride = horizSegmentCount + 1;

            if (flip)
            {
                for (int i = 0; i < vertSegmentCount; i++)
                {
                    for (int j = 0; j < horizSegmentCount; j++)
                    {
                        int index = (stride * i) + j;

                        indices.Set(index + 1);
                        indices.Set(index + stride + 1);
                        indices.Set(index + stride);

                        indices.Set(index + 1);
                        indices.Set(index + stride);
                        indices.Set(index);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertSegmentCount; i++)
                {
                    for (int j = 0; j < horizSegmentCount; j++)
                    {
                        int index = (stride * i) + j;

                        indices.Set(index + stride);
                        indices.Set(index + stride + 1);
                        indices.Set(index + 1);

                        indices.Set(index);
                        indices.Set(index + stride);
                        indices.Set(index + 1);
                    }
                }
            }

            if (!MathHelper.IsApproxZero(bottomRadius))
            {
                GenerateCylinderCap(vertices, ref indices, ref axes, ref centerLine.StartPoint, bottomRadius, true, horizSegmentCount, flip);
            }

            if (!MathHelper.IsApproxZero(topRadius))
            {
                GenerateCylinderCap(vertices, ref indices, ref axes, ref centerLine.EndPoint, topRadius, false, horizSegmentCount, flip);
            }
        }

        private static void GenerateData(IDataBuffer<Vector3> positions, IDataBuffer<Vector3> normals, IDataBuffer<Vector2> textureCoordinates, ref IndexData indices, ref Segment centerLine, float topRadius, float bottomRadius, int vertSegmentCount, int horizSegmentCount, bool flip)
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

            Triad axes = Triad.FromZComplementBasis(-centerLine.Direction); // Cylinder slices exist on the XY plane with Z being the vertical axis

            float sliceAmt = MathHelper.Pi / horizSegmentCount;

            // Create rings of vertices at progressively higher elevations
            for (int i = 0; i <= vertSegmentCount; i++)
            {
                float elevationPercent = (float)i / vertSegmentCount;
                float v = 1.0f - elevationPercent;
                float radius = MathHelper.Lerp(bottomRadius, topRadius, elevationPercent);
                
                Vector3.Lerp(ref centerLine.StartPoint, ref centerLine.EndPoint, elevationPercent, out Vector3 center);

                // Create a single ring of vertices at this elevation
                for (int j = 0; j <= horizSegmentCount; j++)
                {
                    float u = (float)j / horizSegmentCount;
                    
                    GetCylinderNormal(sliceAmt, j, ref axes, out Vector3 normal);

                    if (positions != null)
                    {
                        Vector3.Multiply(ref normal, radius, out Vector3 position);
                        Vector3.Add(ref position, ref center, out position);

                        positions.Set(ref position);
                    }

                    if (normals != null)
                    {
                        if (flip)
                        {
                            normal.Negate();
                        }

                        normals.Set(ref normal);
                    }

                    if (textureCoordinates != null)
                    {
                        var texCoords = new Vector2(u, v);
                        textureCoordinates.Set(ref texCoords);
                    }
                }
            }

            // Generate indices
            int stride = horizSegmentCount + 1;

            if (flip)
            {
                for (int i = 0; i < vertSegmentCount; i++)
                {
                    for (int j = 0; j < horizSegmentCount; j++)
                    {
                        int index = (stride * i) + j;

                        indices.Set(index + 1);
                        indices.Set(index + stride + 1);
                        indices.Set(index + stride);

                        indices.Set(index + 1);
                        indices.Set(index + stride);
                        indices.Set(index);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertSegmentCount; i++)
                {
                    for (int j = 0; j < horizSegmentCount; j++)
                    {
                        int index = (stride * i) + j;

                        indices.Set(index + stride);
                        indices.Set(index + stride + 1);
                        indices.Set(index + 1);

                        indices.Set(index);
                        indices.Set(index + stride);
                        indices.Set(index + 1);
                    }
                }
            }

            if (!MathHelper.IsApproxZero(bottomRadius))
            {
                GenerateCylinderCap(positions, normals, textureCoordinates, ref indices, ref axes, ref centerLine.StartPoint, bottomRadius, true, horizSegmentCount, flip);
            }

            if (!MathHelper.IsApproxZero(topRadius))
            {
                GenerateCylinderCap(positions, normals, textureCoordinates, ref indices, ref axes, ref centerLine.EndPoint, topRadius, false, horizSegmentCount, flip);
            }
        }

        private static void GetCylinderNormal(float sliceAmt, int slice, ref Triad axes, out Vector3 normal)
        {
            Angle angle = Angle.FromRadians(slice * 2.0f * sliceAmt);
            Vector3 dx = axes.XAxis;
            Vector3 dy = axes.YAxis;

            Vector3.Multiply(ref dx, angle.Sin, out dx);
            Vector3.Multiply(ref dy, angle.Cos, out dy);

            Vector3.Add(ref dx, ref dy, out normal);
            normal.Normalize();
        }

        private static void GenerateCylinderCap(IDataBuffer<VertexPositionNormalTexture> vertices, ref IndexData indices, ref Triad axes, ref Vector3 center, float radius, bool isBottom, int tessellation, bool flip)
        {
            var normal = (flip) ? axes.ZAxis : -axes.ZAxis;
            var texScale = new Vector2(0.5f);

            if (isBottom)
            {
                normal.Negate();
            }

            int vBaseIndex = vertices.Position;
            int stride = tessellation + 1;

            // Create cap indices
            for (int i = 0; i < tessellation; i++)
            {
                int index0 = i + 1;
                int index1 = Math.Max(1, (i + 2) % stride); // Wrap around, but to 1 not zero

                if (flip)
                {
                    if (!isBottom)
                    {
                        indices.Set(vBaseIndex + index0);
                        indices.Set(vBaseIndex + index1);
                        indices.Set(vBaseIndex);
                    }
                    else
                    {
                        indices.Set(vBaseIndex);
                        indices.Set(vBaseIndex + index1);
                        indices.Set(vBaseIndex + index0);
                    }
                }
                else
                {
                    if (isBottom)
                    {
                        indices.Set(vBaseIndex + index0);
                        indices.Set(vBaseIndex + index1);
                        indices.Set(vBaseIndex);
                    }
                    else
                    {
                        indices.Set(vBaseIndex);
                        indices.Set(vBaseIndex + index1);
                        indices.Set(vBaseIndex + index0);
                    }
                }
            }

            // Create center vertex
            var vert = new VertexPositionNormalTexture(center, normal, texScale);
            vertices.Set(ref vert);

            float sliceAmt = MathHelper.Pi / tessellation;

            // Create cap vertices
            for (int i = 0; i < tessellation; i++)
            {
                GetCylinderNormal(sliceAmt, i, ref axes, out Vector3 sideNormal);

                Vector3.Multiply(ref sideNormal, radius, out vert.Position);
                Vector3.Add(ref vert.Position, ref center, out vert.Position);

                vert.Normal = normal;
                vert.TextureCoordinate = new Vector2(sideNormal.X * texScale.X + 0.5f, sideNormal.Z * texScale.Y + 0.5f);

                vertices.Set(ref vert);
            }
        }

        private static void GenerateCylinderCap(IDataBuffer<Vector3> positions, IDataBuffer<Vector3> normals, IDataBuffer<Vector2> textureCoordinates, ref IndexData indices, ref Triad axes, ref Vector3 center, float radius, bool isBottom, int tessellation, bool flip)
        {
            var normal = (flip) ? axes.ZAxis : -axes.ZAxis;
            var texScale = new Vector2(0.5f);

            if (isBottom)
            {
                normal.Negate();
            }

            int vBaseIndex = GetBaseVertexIndex(positions, normals, textureCoordinates);
            int stride = tessellation + 1;

            // Create cap indices
            for (int i = 0; i < tessellation; i++)
            {
                int index0 = i + 1;
                int index1 = Math.Max(1, (i + 2) % stride); // Wrap around, but to 1 not zero

                if (flip)
                {
                    if (!isBottom)
                    {
                        indices.Set(vBaseIndex + index0);
                        indices.Set(vBaseIndex + index1);
                        indices.Set(vBaseIndex);
                    }
                    else
                    {
                        indices.Set(vBaseIndex);
                        indices.Set(vBaseIndex + index1);
                        indices.Set(vBaseIndex + index0);
                    }
                }
                else
                {
                    if (isBottom)
                    {
                        indices.Set(vBaseIndex + index0);
                        indices.Set(vBaseIndex + index1);
                        indices.Set(vBaseIndex);
                    }
                    else
                    {
                        indices.Set(vBaseIndex);
                        indices.Set(vBaseIndex + index1);
                        indices.Set(vBaseIndex + index0);
                    }
                }
            }

            // Create center vertex
            normals?.Set(ref normal);
            positions?.Set(ref center);
            textureCoordinates?.Set(ref texScale);

            float sliceAmt = MathHelper.Pi / tessellation;

            // Create cap vertices
            for (int i = 0; i < tessellation; i++)
            {
                GetCylinderNormal(sliceAmt, i, ref axes, out Vector3 sideNormal);

                normals?.Set(ref normal);

                if (positions != null)
                {
                    Vector3.Multiply(ref sideNormal, radius, out Vector3 position);
                    Vector3.Add(ref position, ref center, out position);

                    positions.Set(ref position);
                }

                if (textureCoordinates != null)
                {
                    var texCoords = new Vector2(sideNormal.X * texScale.X + 0.5f, sideNormal.Z * texScale.Y + 0.5f);
                    textureCoordinates.Set(ref texCoords);
                }
            }
        }
    }
}
