namespace Spark.Graphics.Geometry
{
    using System;
    using System.Collections.Generic;

    using Core;
    using Math;
    using Core.Collections;
    using Content;
    
    public sealed class MeshData : ISavable, IDeepCloneable
    {
        private readonly SortedDictionary<TupleKey<VertexSemantic, int>, VertexStream> _bufferMap;
        private IndexData? _indices;
        private PrimitiveType _primitiveType;

        private bool _useIndexedPrimitives;
        private bool _useDynamicVertexBuffer;
        private bool _useDynamicIndexBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshData"/> class.
        /// </summary>
        public MeshData()
        {
            _bufferMap = new SortedDictionary<TupleKey<VertexSemantic, int>, VertexStream>();
            _primitiveType = PrimitiveType.TriangleList;
            _indices = null;
            VertexBuffer = null;
            IndexBuffer = null;
            _useIndexedPrimitives = false;
            _useDynamicIndexBuffer = false;
            _useDynamicVertexBuffer = false;
            IsVertexBufferDirty = true;
            IsIndexBufferDirty = true;
            PrimitiveCount = 0;
            VertexCount = 0;
        }

        /// <summary>
        /// Gets the mesh's GPU vertex buffer.
        /// </summary>
        public VertexBuffer VertexBuffer { get; private set; }

        /// <summary>
        /// Gets the mesh's GPU index buffer.
        /// </summary>
        public IndexBuffer IndexBuffer { get; private set; }

        /// <summary>
        /// Gets if the mesh is using indices (if at all) that are 16-bit in size.
        /// </summary>
        public bool IsUsingShortIndices
        {
            get
            {
                if (_indices.HasValue && _indices.Value.IndexFormat == IndexFormat.SixteenBits)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets if the vertex buffer needs to be recompiled due to data change (such as the number of vertices change or vertex elements are added/removed).
        /// </summary>
        public bool IsVertexBufferDirty { get; private set; }

        /// <summary>
        /// Gets if the index buffer needs to be recompiled due to data change (such as the number of indices change).
        /// </summary>
        public bool IsIndexBufferDirty { get; private set; }

        /// <summary>
        /// Gets the total number of vertices in the mesh.
        /// </summary>
        public int VertexCount { get; private set; }

        /// <summary>
        /// Gets the total number of indices in the mesh.
        /// </summary>
        public int IndexCount
        {
            get
            {
                if (_indices.HasValue)
                {
                    return _indices.Value.Length;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the total number of primitives in the mesh.
        /// </summary>
        public int PrimitiveCount { get; private set; }

        /// <summary>
        /// Gets or sets if the geometry should be indexed data.
        /// </summary>
        public bool UseIndexedPrimitives
        {
            get => _useIndexedPrimitives;
            set
            {
                if (_useIndexedPrimitives != value)
                {
                    _useIndexedPrimitives = value;
                    IsIndexBufferDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets if the mesh should use a dynamic vertex buffer.
        /// </summary>
        public bool UseDynamicVertexBuffer
        {
            get => _useDynamicVertexBuffer;
            set
            {
                if (_useDynamicVertexBuffer != value)
                {
                    _useDynamicVertexBuffer = value;
                    IsVertexBufferDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets if the mesh should use a dynamic index buffer.
        /// </summary>
        public bool UseDynamicIndexBuffer
        {
            get => _useDynamicIndexBuffer;
            set
            {
                if (_useDynamicIndexBuffer != value)
                {
                    _useDynamicIndexBuffer = value;
                    IsIndexBufferDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the geometric primitive type of the mesh.
        /// </summary>
        public PrimitiveType PrimitiveType
        {
            get => _primitiveType;
            set
            {
                if (_primitiveType != value)
                {
                    _primitiveType = value;
                    IsVertexBufferDirty = true;
                    IsIndexBufferDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets the number of vertex element buffers contained in the mesh.
        /// </summary>
        public int BufferCount => _bufferMap.Count;
        
        /// <summary>
        /// Gets or sets the indices of the mesh. This also sets the property <seealso cref="UseIndexedPrimitives"/> based on the validity of the index buffer.
        /// </summary>
        public IndexData? Indices
        {
            get => _indices;
            set
            {
                _indices = value;
                _useIndexedPrimitives = _indices.HasValue;
                IsIndexBufferDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertex positions of the mesh (Semantic: Position, SemanticIndex: 0).
        /// </summary>
        public IDataBuffer<Vector3> Positions
        {
            get
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Position, 0);
                VertexStream vs;
                if (_bufferMap.TryGetValue(key, out vs))
                {
                    return vs.Data as IDataBuffer<Vector3>;
                }

                return null;
            }
            set
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Position, 0);
                if (value == null)
                {
                    _bufferMap.Remove(key);
                }
                else
                {
                    _bufferMap[key] = new VertexStream(VertexFormat.Float3, value);
                }

                IsVertexBufferDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertex normals of the mesh (Semantic: Normal, SemanticIndex: 0).
        /// </summary>
        public IDataBuffer<Vector3> Normals
        {
            get
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Normal, 0);
                if (_bufferMap.TryGetValue(key, out VertexStream vs))
                {
                    return vs.Data as IDataBuffer<Vector3>;
                }

                return null;
            }
            set
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Normal, 0);
                if (value == null)
                {
                    _bufferMap.Remove(key);
                }
                else
                {
                    _bufferMap[key] = new VertexStream(VertexFormat.Float3, value);
                }

                IsVertexBufferDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertex colors of the mesh (Semantic: Color, SemanticIndex: 0).
        /// </summary>
        public IDataBuffer<Color> Colors
        {
            get
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Color, 0);
                if (_bufferMap.TryGetValue(key, out VertexStream vs))
                {
                    return vs.Data as IDataBuffer<Color>;
                }

                return null;
            }
            set
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Color, 0);
                if (value == null)
                {
                    _bufferMap.Remove(key);
                }
                else
                {
                    _bufferMap[key] = new VertexStream(VertexFormat.Color, value);
                }

                IsVertexBufferDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertex texture coordinates of the mesh (Semantic: TextureCoordinate, SemanticIndex: 0).
        /// </summary>
        public IDataBuffer<Vector2> TextureCoordinates
        {
            get
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.TextureCoordinate, 0);
                if (_bufferMap.TryGetValue(key, out VertexStream vs))
                {
                    return vs.Data as IDataBuffer<Vector2>;
                }

                return null;
            }
            set
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.TextureCoordinate, 0);
                if (value == null)
                {
                    _bufferMap.Remove(key);
                }
                else
                {
                    _bufferMap[key] = new VertexStream(VertexFormat.Float2, value);
                }

                IsVertexBufferDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertex bitangents of the mesh (Semantic: Bitangent, SemanticIndex: 0).
        /// </summary>
        public IDataBuffer<Vector3> Bitangents
        {
            get
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Bitangent, 0);
                if (_bufferMap.TryGetValue(key, out VertexStream vs))
                {
                    return vs.Data as IDataBuffer<Vector3>;
                }

                return null;
            }
            set
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Bitangent, 0);
                if (value == null)
                {
                    _bufferMap.Remove(key);
                }
                else
                {
                    _bufferMap[key] = new VertexStream(VertexFormat.Float3, value);
                }

                IsVertexBufferDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertex tangents of the mesh (Semantic: Tangent, SemanticIndex: 0).
        /// </summary>
        public IDataBuffer<Vector3> Tangents
        {
            get
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Tangent, 0);
                if (_bufferMap.TryGetValue(key, out VertexStream vs))
                {
                    return vs.Data as IDataBuffer<Vector3>;
                }

                return null;
            }
            set
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.Tangent, 0);
                if (value == null)
                {
                    _bufferMap.Remove(key);
                }
                else
                {
                    _bufferMap[key] = new VertexStream(VertexFormat.Float3, value);
                }

                IsVertexBufferDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertex blend indices of the mesh (Semantic: BlendIndices, SemanticIndex: 0).
        /// </summary>
        public IDataBuffer<Vector4> BlendIndices
        {
            get
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.BlendIndices, 0);
                if (_bufferMap.TryGetValue(key, out VertexStream vs))
                {
                    return vs.Data as IDataBuffer<Vector4>;
                }

                return null;
            }
            set
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.BlendIndices, 0);
                if (value == null)
                {
                    _bufferMap.Remove(key);
                }
                else
                {
                    _bufferMap[key] = new VertexStream(VertexFormat.Float4, value);
                }

                IsVertexBufferDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertex blend weights of the mesh (Semantic: BlendWeight, SemanticIndex: 0).
        /// </summary>
        public IDataBuffer<Vector4> BlendWeights
        {
            get
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.BlendWeight, 0);
                if (_bufferMap.TryGetValue(key, out VertexStream vs))
                {
                    return vs.Data as IDataBuffer<Vector4>;
                }

                return null;
            }
            set
            {
                TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(VertexSemantic.BlendWeight, 0);
                if (value == null)
                {
                    _bufferMap.Remove(key);
                }
                else
                {
                    _bufferMap[key] = new VertexStream(VertexFormat.Float4, value);
                }

                IsVertexBufferDirty = true;
            }
        }
        
        public void AddBuffer(VertexSemantic semantic, IDataBuffer buffer)
        {
            VertexFormat format = (buffer != null) ? VertexStream.InferVertexFormat(buffer) : VertexFormat.Float;
            AddBuffer(semantic, 0, format, buffer);
        }

        public void AddBuffer(VertexSemantic semantic, int index, IDataBuffer buffer)
        {
            VertexFormat format = (buffer != null) ? VertexStream.InferVertexFormat(buffer) : VertexFormat.Float;
            AddBuffer(semantic, index, format, buffer);
        }

        public void AddBuffer(VertexSemantic semantic, int index, VertexFormat format, IDataBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "Data buffer is null or empty");
            }

            TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(semantic, index);
            if (_bufferMap.ContainsKey(key))
            {
                throw new ArgumentException("Key already present in collection", nameof(semantic));
            }

            VertexStream vs = new VertexStream(format, buffer);
            _bufferMap.Add(key, vs);

            IsVertexBufferDirty = true;
        }

        public bool RemoveBuffer(VertexSemantic semantic)
        {
            TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(semantic, 0);
            return _bufferMap.Remove(key);
        }

        public bool RemoveBuffer(VertexSemantic semantic, int index)
        {
            TupleKey<VertexSemantic, int> key = new TupleKey<VertexSemantic, int>(semantic, index);
            return _bufferMap.Remove(key);
        }

        public IDataBuffer<T> GetBuffer<T>(VertexSemantic semantic) where T : struct
        {
            if (_bufferMap.TryGetValue(new TupleKey<VertexSemantic, int>(semantic, 0), out VertexStream vs))
            {
                return vs.Data as IDataBuffer<T>;
            }

            return null;
        }

        public IDataBuffer<T> GetBuffer<T>(VertexSemantic semantic, int index) where T : struct
        {
            if (_bufferMap.TryGetValue(new TupleKey<VertexSemantic, int>(semantic, index), out VertexStream vs))
            {
                return vs.Data as IDataBuffer<T>;
            }

            return null;
        }

        public IDataBuffer GetBuffer(VertexSemantic semantic)
        {
            if (_bufferMap.TryGetValue(new TupleKey<VertexSemantic, int>(semantic, 0), out VertexStream vs))
            {
                return vs.Data;
            }

            return null;
        }

        public IDataBuffer GetBuffer(VertexSemantic semantic, int index)
        {
            if (_bufferMap.TryGetValue(new TupleKey<VertexSemantic, int>(semantic, index), out VertexStream vs))
            {
                return vs.Data;
            }

            return null;
        }

        public bool ContainsBuffer(VertexSemantic semantic)
        {
            return _bufferMap.ContainsKey(new TupleKey<VertexSemantic, int>(semantic, 0));
        }

        public bool ContainsBuffer(VertexSemantic semantic, int index)
        {
            return _bufferMap.ContainsKey(new TupleKey<VertexSemantic, int>(semantic, index));
        }

        public void ClearData()
        {
            _bufferMap.Clear();
            _indices = null;
            IsVertexBufferDirty = true;
            IsIndexBufferDirty = true;
            VertexCount = 0;
            PrimitiveCount = 0;

            if (VertexBuffer != null)
            {
                VertexBuffer.Dispose();
                VertexBuffer = null;
            }

            if (IndexBuffer != null)
            {
                IndexBuffer.Dispose();
                IndexBuffer = null;
            }
        }

        public MeshData Clone()
        {
            MeshData mshd = new MeshData();

            foreach (KeyValuePair<TupleKey<VertexSemantic, int>, VertexStream> kv in _bufferMap)
            {
                VertexStream strm = kv.Value;

                VertexStream newStrm = new VertexStream();
                newStrm.Data = strm.Data.Clone();
                newStrm.Format = strm.Format;

                mshd._bufferMap.Add(kv.Key, strm);
            }

            if (_indices.HasValue)
            {
                mshd._indices = _indices.Value.Clone();
            }

            mshd._primitiveType = _primitiveType;
            mshd.VertexCount = VertexCount;
            mshd.PrimitiveCount = PrimitiveCount;

            mshd._useIndexedPrimitives = _useIndexedPrimitives;
            mshd._useDynamicVertexBuffer = _useDynamicVertexBuffer;
            mshd._useDynamicIndexBuffer = _useDynamicIndexBuffer;
            mshd.IsVertexBufferDirty = true;
            mshd.IsIndexBufferDirty = true;

            IRenderSystem renderSystem = (VertexBuffer == null) ? null : VertexBuffer.RenderSystem;
            if (renderSystem != null)
            {
                mshd.Compile(renderSystem);
            }
            else
            {
                mshd.Compile();
            }

            return mshd;
        }

        /// <summary>
        /// Get a copy of the object.
        /// </summary>
        /// <returns>Cloned copy.</returns>
        IDeepCloneable IDeepCloneable.Clone()
        {
            return Clone();
        }

        public void Compile()
        {
            Compile(Engine.Instance.Services.GetService<IRenderSystem>());
        }

        public void Compile(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            UpdateVertexAndPrimitiveCount();
            ConstructIndexBuffer(renderSystem);
            ConstructVertexBuffer(renderSystem);
        }

        public void Read(ISavableReader input)
        {
            _useIndexedPrimitives = input.ReadBoolean();
            _useDynamicVertexBuffer = input.ReadBoolean();
            _useDynamicIndexBuffer = input.ReadBoolean();
            _primitiveType = input.ReadEnum<PrimitiveType>();

            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                VertexSemantic semantic = input.ReadEnum<VertexSemantic>();
                int index = input.ReadInt32();
                VertexFormat format = input.ReadEnum<VertexFormat>();
                IDataBuffer data = input.ReadSavable<IDataBuffer>();
                AddBuffer(semantic, index, format, data);
            }

            if (_useIndexedPrimitives)
            {
                _indices = null;

                IDataBuffer indices = input.ReadSavable<IDataBuffer>();
                if (indices != null)
                {
                    if (indices is IDataBuffer<int>)
                    {
                        _indices = new IndexData(indices as IDataBuffer<int>);
                    }
                    else if (indices is IDataBuffer<short>)
                    {
                        _indices = new IndexData(indices as IDataBuffer<short>);
                    }
                }
            }

            IRenderSystem renderSystem = GraphicsHelper.GetRenderSystem(input.ServiceProvider);
            Compile(renderSystem);
        }

        public void Write(ISavableWriter output)
        {
            output.Write("UseIndexedPrimitives", _useIndexedPrimitives);
            output.Write("UseDynamicVertexBuffer", _useDynamicVertexBuffer);
            output.Write("UseDynamicIndexBuffer", _useDynamicIndexBuffer);
            output.WriteEnum("PrimitiveType", _primitiveType);

            output.Write("NumBuffers", _bufferMap.Count);

            foreach (KeyValuePair<TupleKey<VertexSemantic, int>, VertexStream> kv in _bufferMap)
            {
                output.WriteEnum("VertexSemantic", kv.Key.First);
                output.Write("SemanticIndex", kv.Key.Second);
                output.WriteEnum("VertexFormat", kv.Value.Format);
                output.WriteSavable("Buffer", kv.Value.Data);
            }

            if (_useIndexedPrimitives)
            {
                IDataBuffer indices = (_indices.HasValue) ? _indices.Value.UnderlyingDataBuffer : null;
                output.WriteSavable("Indices", indices);
            }
        }

        // TEMP - Even more now with sub mesh support.
        public void ComputeTangentBasis()
        {
            if (_primitiveType == PrimitiveType.LineList || _primitiveType == PrimitiveType.LineStrip || _primitiveType == PrimitiveType.PointList)
            {
                return;
            }

            IDataBuffer<Vector2> texCoords = TextureCoordinates;
            IDataBuffer<Vector3> normals = Normals;
            IDataBuffer<Vector3> pos = Positions;

            if (texCoords == null || normals == null || pos == null)
            {
                return;
            }

            IDataBuffer<Vector3> bitangents = Bitangents;
            IDataBuffer<Vector3> tangents = Tangents;

            if (bitangents == null)
            {
                bitangents = new DataBuffer<Vector3>(pos.Length);
                Bitangents = bitangents;
            }

            if (tangents == null)
            {
                tangents = new DataBuffer<Vector3>(pos.Length);
                Tangents = tangents;
            }

            bitangents.Position = 0;
            tangents.Position = 0;

            UpdateVertexAndPrimitiveCount();
            int[] triVerts = new int[3];

            for (int i = 0; i < PrimitiveCount; i++)
            {
                GetPrimitiveIndices(i, triVerts);

                int index0 = triVerts[0];
                int index1 = triVerts[1];
                int index2 = triVerts[2];

                // Get the triangle verts
                Vector3 p0 = pos.Get(index0);
                Vector3 p1 = pos.Get(index1);
                Vector3 p2 = pos.Get(index2);

                // Get the triangle uv coords
                Vector2 uv0 = texCoords.Get(index0);
                Vector2 uv1 = texCoords.Get(index1);
                Vector2 uv2 = texCoords.Get(index2);

                float x1 = p1.X - p0.X;
                float x2 = p2.X - p0.X;
                float y1 = p1.Y - p0.Y;
                float y2 = p2.Y - p0.Y;
                float z1 = p1.Z - p0.Z;
                float z2 = p2.Z - p0.Z;

                float s1 = uv1.X - uv0.X;
                float s2 = uv2.X - uv0.X;
                float t1 = uv1.Y - uv0.Y;
                float t2 = uv2.Y - uv0.Y;

                // calculate determinate
                float r = 1.0f / (s1 * t2 - s2 * t1);

                // Calculate the s, t directions
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                // Add them to the existing tangent, binormal arrays
                tangents.Set(index0, tangents.Get(index0) + sdir);
                tangents.Set(index1, tangents.Get(index1) + sdir);
                tangents.Set(index2, tangents.Get(index2) + sdir);

                bitangents.Set(index0, bitangents.Get(index0) + tdir);
                bitangents.Set(index1, bitangents.Get(index1) + tdir);
                bitangents.Set(index2, bitangents.Get(index2) + tdir);
            }

            // Gram-Schmidt orthogonalization
            for (int i = 0; i < normals.Length; i++)
            {
                Vector3 n = normals.Get(i);
                Vector3 t = tangents.Get(i);
                Vector3 b = bitangents.Get(i);

                Vector3 tangent = Vector3.Normalize(t - n * Vector3.Dot(n, t));

                float handy = (Vector3.Dot(Vector3.Cross(n, t), b) < 0) ? -1.0f : 1.0f;
                b = Vector3.Cross(n, t);
                b *= handy;
                tangents.Set(i, tangent);
                bitangents.Set(i, Vector3.Normalize(b));
            }

            Compile();
        }

        public bool Intersects(ref Ray rayInWorldSpace, bool ignoreBackfaces = false)
        {
            Matrix4x4 identity = Matrix4x4.Identity;
            SubMeshRange? range = null;
            return Intersects(ref rayInWorldSpace, ref identity, ref range, null, ignoreBackfaces);
        }

        public bool Intersects(ref Ray rayInWorldSpace, ref SubMeshRange? subMeshRange, bool ignoreBackfaces = false)
        {
            Matrix4x4 identity = Matrix4x4.Identity;
            return Intersects(ref rayInWorldSpace, ref identity, ref subMeshRange, null, ignoreBackfaces);
        }

        public bool Intersects(ref Ray rayInWorldSpace, ref Matrix4x4 worldMatrix, IList<Tuple<LineIntersectionResult, Triangle?>> results, bool ignoreBackfaces = false)
        {
            SubMeshRange? range = null;
            return Intersects(ref rayInWorldSpace, ref worldMatrix, ref range, results, ignoreBackfaces);
        }

        public bool Intersects(ref Ray rayInWorldSpace, ref Matrix4x4 worldMatrix, ref SubMeshRange? subMeshRange, IList<Tuple<LineIntersectionResult, Triangle?>> results, bool ignoreBackfaces = false)
        {
            if (!HasTriangles() || !HasValidPositions())
            {
                return false;
            }

            Matrix4x4 worldToObjMatrix;
            Ray rayInObjSpace;

            bool noTransformNeeded = worldMatrix.IsIdentity;

            if (!noTransformNeeded)
            {
                Matrix4x4.Invert(ref worldMatrix, out worldToObjMatrix);
                Ray.Transform(ref rayInWorldSpace, ref worldToObjMatrix, out rayInObjSpace);
            }
            else
            {
                worldToObjMatrix = Matrix4x4.Identity;
                rayInObjSpace = rayInWorldSpace;
            }

            bool haveOne = false;
            IDataBuffer<Vector3> pos = Positions;
            
            GetPrimitiveRange(ref subMeshRange, out int startPrimitive, out int primitiveCount);

            int baseVertexOffset = 0;
            if (_useIndexedPrimitives && subMeshRange.HasValue)
            {
                baseVertexOffset = subMeshRange.Value.BaseVertexOffset;
            }

            for (int i = startPrimitive; i < primitiveCount; i++)
            {
                // Get the triangle
                Triangle tri;
                if (_useIndexedPrimitives)
                {
                    GetPrimitive(i, baseVertexOffset, out tri);
                }
                else
                {
                    GetPrimitive(i, 0, out tri);
                }

                LineIntersectionResult intersection;
                if (rayInObjSpace.Intersects(ref tri, out intersection, ignoreBackfaces))
                {
                    haveOne = true;

                    if (results == null)
                    {
                        break;
                    }

                    // Transform triangle to world space, as well as the intersection result
                    if (!noTransformNeeded)
                    {
                        Vector3 normal = intersection.Normal.Value;
                        Vector3 pt = intersection.Point;

                        Vector3.TransformNormal(ref normal, ref worldMatrix, out normal);
                        Vector3.Transform(ref pt, ref worldMatrix, out pt);
                        Vector3.Distance(ref pt, ref rayInWorldSpace.Origin, out float distance);

                        intersection = new LineIntersectionResult(pt, distance, normal);

                        Triangle.Transform(ref tri, ref worldMatrix, out tri);
                    }

                    results.Add(new Tuple<LineIntersectionResult, Triangle?>(intersection, tri));
                }
            }

            return haveOne;
        }

        public void GetPrimitiveRange(ref SubMeshRange? range, out int startPrimitive, out int primitiveCount)
        {
            if (!range.HasValue)
            {
                startPrimitive = 0;
                primitiveCount = PrimitiveCount;
                return;
            }

            SubMeshRange meshRange = range.Value;

            switch (_primitiveType)
            {
                case PrimitiveType.TriangleList:
                    startPrimitive = meshRange.Offset / 3;
                    primitiveCount = meshRange.Count / 3;
                    break;
                case PrimitiveType.LineList:
                    startPrimitive = meshRange.Offset / 2;
                    primitiveCount = meshRange.Count / 2;
                    break;
                default:
                    startPrimitive = meshRange.Offset;
                    primitiveCount = meshRange.Count;
                    break;
            }
        }

        public bool GetPrimitive(int primitiveIndex, out Segment line)
        {
            return GetPrimitive(primitiveIndex, 0, out line);
        }

        public bool GetPrimitive(int primitiveIndex, int baseVertexOffset, out Segment line)
        {
            IDataBuffer<Vector3> pos = Positions;

            if (_useIndexedPrimitives)
            {
                IndexData indices = _indices.Value;

                pos.Get(indices.Get(GetVertexIndex(primitiveIndex, 0) + baseVertexOffset), out line.StartPoint);
                pos.Get(indices.Get(GetVertexIndex(primitiveIndex, 1) + baseVertexOffset), out line.EndPoint);
            }
            else
            {
                pos.Get(GetVertexIndex(primitiveIndex, 0), out line.StartPoint);
                pos.Get(GetVertexIndex(primitiveIndex, 1), out line.EndPoint);
            }

            return true;
        }

        public bool GetPrimitive(int primitiveIndex, out Triangle triangle)
        {
            return GetPrimitive(primitiveIndex, 0, out triangle);
        }

        public bool GetPrimitive(int primitiveIndex, int baseVertexOffset, out Triangle triangle)
        {
            IDataBuffer<Vector3> pos = Positions;

            if (_useIndexedPrimitives)
            {
                IndexData indices = _indices.Value;

                pos.Get(indices.Get(GetVertexIndex(primitiveIndex, 0) + baseVertexOffset), out triangle.PointA);
                pos.Get(indices.Get(GetVertexIndex(primitiveIndex, 1) + baseVertexOffset), out triangle.PointB);
                pos.Get(indices.Get(GetVertexIndex(primitiveIndex, 2) + baseVertexOffset), out triangle.PointC);
            }
            else
            {
                pos.Get(GetVertexIndex(primitiveIndex, 0), out triangle.PointA);
                pos.Get(GetVertexIndex(primitiveIndex, 1), out triangle.PointB);
                pos.Get(GetVertexIndex(primitiveIndex, 2), out triangle.PointC);
            }

            return true;
        }

        public void Transform(Transform transform)
        {
            if (transform == null)
            {
                return;
            }
            
            transform.GetMatrix(out Matrix4x4 mat);
            Transform(ref mat);
        }

        public void Transform(ref Vector3 translation)
        {
            Matrix4x4.FromTranslation(ref translation, out Matrix4x4 transform);
            Transform(ref transform);
        }

        public void Transform(ref Quaternion rotation)
        {
            Matrix4x4.FromQuaternion(ref rotation, out Matrix4x4 transform);
            Transform(ref transform);
        }

        public void Transform(ref Quaternion rotation, ref Vector3 translation)
        {
            Matrix4x4.CreateTransformationMatrix(ref rotation, ref translation, out Matrix4x4 transform);
            Transform(ref transform);
        }

        public void Transform(ref Matrix4x4 transform)
        {
            if (transform.IsIdentity)
            {
                return;
            }

            IDataBuffer<Vector3> positions = Positions;
            IDataBuffer<Vector3> normals = Normals;
            IDataBuffer<Vector3> tangents = Tangents;
            IDataBuffer<Vector3> bitangents = Bitangents;

            if (positions != null)
            {
                Transform(positions, ref transform, false, false);
            }

            if (normals != null)
            {
                Transform(normals, ref transform, true, true);
            }

            if (tangents != null)
            {
                Transform(tangents, ref transform, true, true);
            }

            if (bitangents != null)
            {
                Transform(bitangents, ref transform, true, true);
            }
        }

        private void Transform(IDataBuffer<Vector3> buffer, ref Matrix4x4 transform, bool rotateOnly = false, bool normalize = false)
        {
            if (rotateOnly)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer.Get(i, out Vector3 v);

                    Vector3.TransformNormal(ref v, ref transform, out v);
                    if (normalize)
                    {
                        v.Normalize();
                    }

                    buffer.Set(i, ref v);
                }
            }
            else
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer.Get(i, out Vector3 v);

                    Vector3.Transform(ref v, ref transform, out v);

                    // Normalizing doesn't make sense in this context, ignore the option
                    buffer.Set(i, ref v);
                }
            }
        }

        public void Merge(MeshData other)
        {
            if (other == null || HasValidPositions() || other.HasValidPositions())
            {
                return;
            }

            IndexData? thisIndexData = _indices;
            IndexData? otherIndexData = other._indices;
            int thisNumVerts = Positions.Length;
            int otherNumVerts = other.Positions.Length;

            // Cases:
            // Both have indexed - enlarge current, copy over + offset from other
            // Both have non-indexed - Do nothing
            // This has indexed, other has non-indexed - enlarge current, offset and create dumb indices for other
            // This has non-indexed, other has indexed - create a new databuffer that can contain both, create dumb indices for current, copy + offset other

            if (thisIndexData.HasValue && otherIndexData.HasValue)
            {
                IndexData thisIndices = thisIndexData.Value;
                IndexData otherIndices = otherIndexData.Value;

                int startIndex = thisIndices.Length;
                int indexOffset = thisNumVerts;

                // Safely expand the index buffer (if short and we're about to go out of range, make int)
                SafeRecreateIndices(thisIndices.Length + otherIndices.Length);
                thisIndices = _indices.Value; //Get the databuffer reference again, since it may have changed

                // Copy and offset other indices
                for (int i = startIndex, j = 0; j < otherIndices.Length; i++, j++)
                {
                    int indx = otherIndices[j];
                    indx += indexOffset;
                    thisIndices[i] = indx;
                }

                IsIndexBufferDirty = true;
            }
            else if (!thisIndexData.HasValue && otherIndexData.HasValue)
            {
                IndexData thisIndices;
                IndexData otherIndices = otherIndexData.Value;
                int numIndices = thisNumVerts;
                int newSize = numIndices + otherIndices.Length;

                if (otherIndices.IndexFormat == IndexFormat.SixteenBits && ((uint)(newSize) > ushort.MaxValue))
                {
                    thisIndices = new IndexData(new DataBuffer<int>(newSize));
                }
                else if (otherIndices.IndexFormat == IndexFormat.SixteenBits)
                {
                    thisIndices = new IndexData(new DataBuffer<short>(newSize));
                }
                else
                {
                    thisIndices = new IndexData(new DataBuffer<int>(newSize));
                }

                // Fill dumb indices
                for (int i = 0; i < numIndices; i++)
                {
                    thisIndices[i] = i;
                }

                // Copy over and offset other indices
                for (int i = numIndices, j = 0; j < otherIndices.Length; i++, j++)
                {
                    int indx = otherIndices[j];
                    indx += numIndices;
                    thisIndices[i] = indx;
                }

                Indices = thisIndices;
            }
            else if (thisIndexData.HasValue && !otherIndexData.HasValue)
            {
                IndexData thisIndices = thisIndexData.Value;
                int startIndex = thisIndices.Length;
                int indexOffset = thisNumVerts;
                int newSize = thisIndices.Length + otherNumVerts;

                // Safely expand the index buffer (if short and we're about to go out of range, make int)
                SafeRecreateIndices(newSize);
                thisIndices = _indices.Value; //Get the databuffer reference again, since it may have changed

                // Create dumb indices for other mesh
                for (int i = startIndex, j = 0; i < thisIndices.Length; i++, j++)
                {
                    int indx = j + indexOffset;
                    thisIndices[i] = indx;
                }

                IsIndexBufferDirty = true;
            }

            int newTotalVerts = thisNumVerts + otherNumVerts;

            foreach (KeyValuePair<TupleKey<VertexSemantic, int>, VertexStream> kv in other._bufferMap)
            {
                IDataBuffer db = GetBuffer(kv.Key.First, kv.Key.Second);
                db.Position = 0;

                // If we have the buffer, need to resize and append new data, otherwise just insert a clone and set a portion to zero
                if (db != null && db.ElementSizeInBytes == kv.Value.Data.ElementSizeInBytes)
                {
                    db.Resize(newTotalVerts);
                    kv.Value.Data.CopyTo(db, 0, thisNumVerts * db.ElementSizeInBytes, kv.Value.Data.SizeInBytes);
                }
                else if (db == null)
                {
                    // This is a bit hacky, to make sure we create the same typed buffer and things match with the index buffer, we clone, clear,
                    // then copy to the upper range of the buffer
                    db = kv.Value.Data.Clone();
                    db.Clear();
                    db.Resize(newTotalVerts);
                    kv.Value.Data.CopyTo(db, 0, thisNumVerts * db.ElementSizeInBytes, kv.Value.Data.SizeInBytes);

                    AddBuffer(kv.Key.First, kv.Key.Second, db);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(true, "Element size mismatch?");
                }
            }

            IsVertexBufferDirty = true;
        }

        private void SafeRecreateIndices(int newSize)
        {
            IndexData db = _indices.Value;
            if (!db.IsValid || newSize <= db.Length)
            {
                return;
            }

            if (_indices.Value.IndexFormat == IndexFormat.SixteenBits)
            {
                if ((uint)newSize > ushort.MaxValue)
                {
                    IndexData newIndices = new IndexData(new DataBuffer<int>(newSize));
                    for (int i = 0; i < newSize; i++)
                    {
                        newIndices[i] = db[i];
                    }

                    _indices = newIndices;
                }
                else
                {
                    db.Resize(newSize);
                }
            }
            else
            {
                // Internally resizes
                db.Resize(newSize);
            }
        }
        
        private bool HasValidPositions()
        {
            if (Positions == null || (_useIndexedPrimitives && _indices == null))
            {
                return false;
            }

            return true;
        }
        
        private bool HasTriangles()
        {
            switch (_primitiveType)
            {
                case PrimitiveType.TriangleList:
                case PrimitiveType.TriangleStrip:
                    return true;
                default:
                    return false;
            }
        }
        
        private void GetPrimitiveIndices(int startIndex, int[] indices)
        {
            int rSize = 0;
            switch (_primitiveType)
            {
                case PrimitiveType.TriangleList:
                case PrimitiveType.TriangleStrip:
                    rSize = 3;
                    break;
                case PrimitiveType.LineList:
                case PrimitiveType.LineStrip:
                    rSize = 2;
                    break;
                case PrimitiveType.PointList:
                    rSize = 1;
                    break;
            }

            for (int i = 0; i < rSize; i++)
            {
                if (_useIndexedPrimitives)
                {
                    indices[i] = _indices.Value[GetVertexIndex(startIndex, i)];
                }
                else
                {
                    indices[i] = GetVertexIndex(startIndex, i);
                }
            }
        }

        private int GetVertexIndex(int primIndex, int pointIndex)
        {
            switch (_primitiveType)
            {
                case PrimitiveType.TriangleList:
                    return (primIndex * 3) + pointIndex;
                case PrimitiveType.TriangleStrip:
                    return primIndex + pointIndex;
                case PrimitiveType.LineList:
                    return (primIndex * 2) + pointIndex;
                case PrimitiveType.LineStrip:
                    return primIndex + pointIndex;
                default:
                    return primIndex;
            }
        }

        private void UpdateVertexAndPrimitiveCount()
        {
            IDataBuffer positions = GetBuffer(VertexSemantic.Position, 0);

            if (positions == null || (_useIndexedPrimitives && _indices == null))
            {
                VertexCount = 0;
                PrimitiveCount = 0;
                return;
            }

            VertexCount = (_useIndexedPrimitives) ? _indices.Value.Length : positions.Length;

            switch (_primitiveType)
            {
                case PrimitiveType.TriangleStrip:
                    PrimitiveCount = VertexCount - 2;
                    break;
                case PrimitiveType.TriangleList:
                    PrimitiveCount = VertexCount / 3;
                    break;
                case PrimitiveType.LineStrip:
                    PrimitiveCount = VertexCount - 1;
                    break;
                case PrimitiveType.LineList:
                    PrimitiveCount = VertexCount / 2;
                    break;
                case PrimitiveType.PointList:
                    PrimitiveCount = VertexCount;
                    break;
                default:
                    PrimitiveCount = 0;
                    break;
            }
        }

        private void ConstructIndexBuffer(IRenderSystem renderSystem)
        {
            IsIndexBufferDirty = false;
            ResourceUsage usage = (_useDynamicIndexBuffer) ? ResourceUsage.Dynamic : ResourceUsage.Static;

            if (!_useIndexedPrimitives)
            {
                return;
            }

            if (IndexBuffer != null)
            {
                // Not valid indices, dispose and set to null
                if (!_indices.HasValue || !_indices.Value.IsValid)
                {
                    IndexBuffer.Dispose();
                    IndexBuffer = null;
                    return;
                }

                IndexData data = _indices.Value;

                // If buffer is same size format and usage, can just set data, don't need to recreate
                if (IndexBuffer.IndexCount == data.Length &&
                    IndexBuffer.IndexFormat == data.IndexFormat &&
                    IndexBuffer.ResourceUsage == usage)
                {
                    DataWriteOptions writeOptions = (usage == ResourceUsage.Dynamic) ? DataWriteOptions.Discard : DataWriteOptions.None;
                    IndexBuffer.SetData(renderSystem.ImmediateContext, _indices.Value, writeOptions);
                    return;
                }
                else
                {
                    IndexBuffer.Dispose();
                    IndexBuffer = null;
                }
            }

            if (!_indices.HasValue)
            {
                return;
            }

            // Else create a new index buffer
            IndexBuffer = new IndexBuffer(renderSystem, _indices.Value);
        }

        private void ConstructVertexBuffer(IRenderSystem renderSystem)
        {
            IsVertexBufferDirty = false;
            ResourceUsage usage = (_useDynamicVertexBuffer) ? ResourceUsage.Dynamic : ResourceUsage.Static;

            if (VertexBuffer != null)
            {
                // If no vertex data, dispose and set to null
                if (_bufferMap.Count == 0)
                {
                    VertexBuffer.Dispose();
                    VertexBuffer = null;
                    return;
                }

                IDataBuffer[] buffers = GetBuffers();
                IDataBuffer db = buffers[0];

                // If buffer is same size format and usage, can just set data, don't need to recreate
                if (VertexBuffer.VertexCount == db.Length && VertexBuffer.ResourceUsage == usage && buffers.Length == VertexBuffer.VertexLayout.ElementCount)
                {
                    VertexBuffer.SetInterleavedData(renderSystem.ImmediateContext, buffers);
                    return;
                }
                else
                {
                    VertexBuffer.Dispose();
                    VertexBuffer = null;
                }
            }

            if (_bufferMap.Count == 0)
            {
                return;
            }

            // Else create a new vertex buffer
            VertexBuffer = new VertexBuffer(renderSystem, CreateVertexLayout(), usage, GetBuffers());

        }

        private IDataBuffer[] GetBuffers()
        {
            IDataBuffer[] buffers = new IDataBuffer[_bufferMap.Count];
            int index = 0;
            foreach (KeyValuePair<TupleKey<VertexSemantic, int>, VertexStream> kv in _bufferMap)
            {
                buffers[index++] = kv.Value.Data;
            }

            return buffers;
        }

        private VertexLayout CreateVertexLayout()
        {
            VertexElement[] vs = new VertexElement[_bufferMap.Count];
            int index = 0;
            int offset = 0;
            foreach (KeyValuePair<TupleKey<VertexSemantic, int>, VertexStream> kv in _bufferMap)
            {
                vs[index++] = new VertexElement(kv.Key.First, kv.Key.Second, kv.Value.Format, offset);
                offset += kv.Value.Format.SizeInBytes();
            }

            return new VertexLayout(vs);
        }
        
        private struct VertexStream
        {
            private static Dictionary<Type, VertexFormat> s_typesToFormats;

            public VertexFormat Format;
            public IDataBuffer Data;

            static VertexStream()
            {
                s_typesToFormats = new Dictionary<Type, VertexFormat>();

                s_typesToFormats.Add(typeof(Color), VertexFormat.Color);
                s_typesToFormats.Add(typeof(float), VertexFormat.Float);
                s_typesToFormats.Add(typeof(Vector2), VertexFormat.Float2);
                s_typesToFormats.Add(typeof(Vector3), VertexFormat.Float3);
                s_typesToFormats.Add(typeof(Vector4), VertexFormat.Float4);
                //s_typesToFormats.Add(typeof(Half), VertexFormat.Half); //TODO
                //s_typesToFormats.Add(typeof(Half2), VertexFormat.Half2); //TODO
                //s_typesToFormats.Add(typeof(Half4), VertexFormat.Half4); //TODO
                s_typesToFormats.Add(typeof(int), VertexFormat.Int);
                s_typesToFormats.Add(typeof(Int2), VertexFormat.Int2);
                s_typesToFormats.Add(typeof(Int3), VertexFormat.Int3);
                s_typesToFormats.Add(typeof(Int4), VertexFormat.Int4);
                s_typesToFormats.Add(typeof(uint), VertexFormat.UInt);
                //s_typesToFormats.Add(typeof(Uint2), VertexFormat.UInt2); //Unlikely
                //s_typesToFormats.Add(typeof(Uint3), VertexFormat.UInt3); //Unlikely
                //s_typesToFormats.Add(typeof(Uint4), VertexFormat.UInt4); //Unlikely
                s_typesToFormats.Add(typeof(short), VertexFormat.Short);
                //s_typesToFormats.Add(typeof(Short2), VertexFormat.Short2); //Maybe
                //s_typesToFormats.Add(typeof(Short4), VertexFormat.Short4); //Maybe
                s_typesToFormats.Add(typeof(ushort), VertexFormat.UShort);
                //s_typesToFormats.Add(typeof(UShort2), VertexFormat.UShort2); //Unlikely
                //s_typesToFormats.Add(typeof(UShort4), VertexFormat.UShort4); //Unlikely
                //s_typesToFormats.Add(typeof(NormalizedShort), VertexFormat.NormalizedShort); //TODO
                //s_typesToFormats.Add(typeof(NormalizedShort2), VertexFormat.NormalizedShort2); //TODO
                //s_typesToFormats.Add(typeof(NormalizedShort4), VertexFormat.NormalizedShort4); //TODO
                //s_typesToFormats.Add(typeof(NormalizedUShort), VertexFormat.NormalizedUShort); //Unlikely
                //s_typesToFormats.Add(typeof(NormalizedUShort2), VertexFormat.NormalizedUShort2); //Unlikely
                //s_typesToFormats.Add(typeof(NormalizedUShort4), VertexFormat.NormalizedUShort4); //Unlikely
            }
            
            public VertexStream(VertexFormat format, IDataBuffer data)
            {
                Format = format;
                Data = data;
            }

            public static VertexFormat InferVertexFormat(IReadOnlyDataBuffer data)
            {
                if (s_typesToFormats.TryGetValue(data.ElementType, out VertexFormat format))
                {
                    return format;
                }

                throw new SparkGraphicsException("Unsupported vertex format");
            }
        }
    }
}
