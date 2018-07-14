namespace Spark.Graphics
{
    using System;

    using Utilities;

    /// <summary>
    /// Batcher for interleaved mesh data to draw many pieces of data in as few draw calls as possible. Supports all primitive 
    /// topologies as indexed and non-indexed, including drawing quads (as triangle lists). It is best to order batch submission based on topology and indexing,
    /// in order to reduce the number of draw calls (e.g. switching between lines and triangles or indexed geometry and non-indexed results in the batcher flushing
    /// queued data). The batcher also does not split geometry, so if a batch is submitted that is too large to fit then an exception will be thrown.
    /// Unlike other batchers, this is a low-level batcher, which means no material/shader/state setting is done. Setting up the render pipeline must be done before the batcher 
    /// does any drawing.
    /// </summary>
    /// <typeparam name="T">Type of vertex.</typeparam>
    public sealed class PrimitiveBatch<T> : Disposable where T : struct, IVertexType
    {
        private const int MaxBatchVertexCount = 65535;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private DataBuffer<T> _vertices;
        private IndexData _indices;

        private DataBuffer<T> _tempVertices;
        private DataBuffer<short> _quadIndices;

        private readonly int _maxIndexCount;
        private readonly int _maxVertexCount;
        private readonly int _vertexStride;

        private IRenderContext _renderContext;
        private bool _inBeginEnd;
        private bool _isCurrentlyIndexed;
        private PrimitiveBatchTopology _currentTopology;
        private int _currentIndex;
        private int _numIndicesInBatch;
        private int _currentVertex;
        private int _numVerticesInBatch;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveBatch{T}"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create resources.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the max batch size is less than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the render system is null.</exception>
        public PrimitiveBatch(IRenderSystem renderSystem) 
            : this(renderSystem, MaxBatchVertexCount, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveBatch{T}"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create resources.</param>
        /// <param name="maxBatchVertexCount">Maximum number of vertices that can be buffered.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the max batch size is less than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the render system is null.</exception>
        public PrimitiveBatch(IRenderSystem renderSystem, int maxBatchVertexCount) 
            : this(renderSystem, maxBatchVertexCount, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveBatch{T}"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create resources.</param>
        /// <param name="maxBatchVertexCount">Maximum number of vertices that can be buffered.</param>
        /// <param name="noIndexBuffer">True if no index buffer should be created, false if an index buffer should be. Use this if you
        /// never will be submitted indexed batches. 16-bit or 32-bit indices automatically will be chose based on the max batch size.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the max batch size is less than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the render system is null.</exception>
        public PrimitiveBatch(IRenderSystem renderSystem, int maxBatchVertexCount, bool noIndexBuffer)
        {
            // At least be able to fit the largest non-indexed primitive we can draw (a quad, split into two triangles)
            if (maxBatchVertexCount <= 6)
            {
                throw new ArgumentOutOfRangeException(nameof(maxBatchVertexCount), "Batch size must be positive");
            }

            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem));
            }

            _maxVertexCount = maxBatchVertexCount;
            _maxIndexCount = (noIndexBuffer) ? 0 : maxBatchVertexCount;

            VertexLayout layout = new T().GetVertexLayout();
            _vertexBuffer = new VertexBuffer(renderSystem, layout, _maxVertexCount, ResourceUsage.Dynamic);
            _vertices = new DataBuffer<T>(_maxVertexCount);
            _tempVertices = new DataBuffer<T>(4);
            _vertexStride = layout.VertexStride;

            if (!noIndexBuffer)
            {
                // Automatically determine if short or int indices will be used
                IndexFormat indexFormat = IndexFormat.ThirtyTwoBits;
                if (_maxIndexCount < short.MaxValue)
                {
                    indexFormat = IndexFormat.SixteenBits;
                }

                _indexBuffer = new IndexBuffer(renderSystem, indexFormat, _maxIndexCount, ResourceUsage.Dynamic);

                if (indexFormat == IndexFormat.ThirtyTwoBits)
                {
                    _indices = new DataBuffer<int>(_maxIndexCount);
                }
                else
                {
                    _indices = new DataBuffer<short>(_maxIndexCount);
                }

                _quadIndices = new DataBuffer<short>(new short[] { 0, 1, 2, 3 });
            }

            _inBeginEnd = false;
            _isCurrentlyIndexed = false;
            _currentTopology = PrimitiveBatchTopology.TriangleList;
            _currentVertex = 0;
            _numVerticesInBatch = 0;
            _currentIndex = 0;
            _numIndicesInBatch = 0;
        }

        /// <summary>
        /// Queries the maximum number of primitives that can be in a single batch. This will vary based on topology.
        /// </summary>
        /// <param name="topology">Topology of the geometry.</param>
        /// <returns>The maximum number of primitives that can be submitted in a single batch.</returns>
        public int QueryMaximumPrimitiveCount(PrimitiveBatchTopology topology)
        {
            switch (topology)
            {
                // Quadlist is a bit tricky since we create triangles, so a straight up # of quads based on vertex count won't be accurate, so take the number
                // of triangles and then divide that by 2 since worst case, if we have non-indexed geometry of N quads, we'll have 2N triangles to deal with.
                case PrimitiveBatchTopology.QuadList:
                    return (_maxVertexCount / 3) / 2;
                case PrimitiveBatchTopology.TriangleList:
                    return _maxVertexCount / 3;
                case PrimitiveBatchTopology.LineList:
                    return _maxVertexCount / 2;
                case PrimitiveBatchTopology.PointList:
                    return _maxVertexCount;
                case PrimitiveBatchTopology.TriangleStrip:
                    return _maxVertexCount - 2;
                case PrimitiveBatchTopology.LineStrip:
                    return _maxVertexCount - 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Begins a primitive batch operation. No state is changed on the render context, those changes should be set before this
        /// method is called.
        /// </summary>
        /// <param name="renderContext">Render context to be used to issue draw calls.</param>
        public void Begin(IRenderContext renderContext)
        {
            ThrowIfDisposed();

            if (_inBeginEnd)
            {
                throw new InvalidOperationException("Cannot nest begin calls");
            }

            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext), "Render context cannot be null");
            }

            _renderContext = renderContext;
            _inBeginEnd = true;

            // If deferred, ensure we always do a write discard for the first call
            if (!_renderContext.IsImmediateContext)
            {
                _currentVertex = 0;
                _currentIndex = 0;
            }

            SetBuffers();
        }

        /// <summary>
        /// Flushes the primitive batch, causing any queued batches to be drawn. This does not change any state on the render context.
        /// </summary>
        public void End()
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("End called before begin");
            }

            ProcessRenderQueue();
            _inBeginEnd = false;
        }

        /// <summary>
        /// Convenience method for adding a line to the batch. This will always draw in non-indexed mode.
        /// </summary>
        /// <param name="v0">First vertex.</param>
        /// <param name="v1">Second vertex.</param>
        public void DrawLine(T v0, T v1)
        {
            _tempVertices[0] = v0;
            _tempVertices[1] = v1;

            Draw(PrimitiveBatchTopology.LineList, _tempVertices, 0, 2);
        }

        /// <summary>
        /// Convenience method for adding a triangle to the batch. Assumed clockwise winding with the first vertex
        /// being the top left corner. This will always draw in non-indexed mode.
        /// </summary>
        /// <param name="v0">First vertex.</param>
        /// <param name="v1">Second vertex.</param>
        /// <param name="v2">Third vertex.</param>
        public void DrawTriangle(T v0, T v1, T v2)
        {
            _tempVertices[0] = v0;
            _tempVertices[1] = v1;
            _tempVertices[2] = v2;

            Draw(PrimitiveBatchTopology.TriangleList, _tempVertices, 0, 3);
        }

        /// <summary>
        /// Convenience method for adding a quad to the batch. Assumed clockwise winding with the first vertex
        /// being the top left corner. This will follow the current index mode.
        /// </summary>
        /// <param name="v0">First vertex.</param>
        /// <param name="v1">Second vertex.</param>
        /// <param name="v2">Third vertex.</param>
        /// <param name="v3">Fourth vertex.</param>
        public void DrawQuad(T v0, T v1, T v2, T v3)
        {
            _tempVertices[0] = v0;
            _tempVertices[1] = v1;
            _tempVertices[2] = v2;
            _tempVertices[3] = v3;

            if (_maxIndexCount > 0)
            {
                DrawIndexed(PrimitiveBatchTopology.QuadList, _tempVertices, 0, 4, _quadIndices, 0, 4);
            }
            else
            {
                Draw(PrimitiveBatchTopology.QuadList, _tempVertices, 0, 4);
            }
        }

        /// <summary>
        /// Submits a mesh for batching. This may result in a draw call of queued batches if the capacity of the internal buffers 
        /// cannot fit the incoming batch, or if the topology/index mode changes, or if the batch is a triangle strip. 
        /// </summary>
        /// <param name="primTopology">Topology of the vertex data.</param>
        /// <param name="vertices">Vertex data.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the batcher has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if called before begin has been called.</exception>
        /// <exception cref="ArgumentNullException">Thrown if input data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if offsets or counts to read from data will exceed capacity.</exception>
        public void Draw(PrimitiveBatchTopology primTopology, IReadOnlyDataBuffer<T> vertices)
        {
            Draw(primTopology, vertices, 0, (vertices != null) ? vertices.Length : 0);
        }

        /// <summary>
        /// Submits a mesh for batching. This may result in a draw call of queued batches if the capacity of the internal buffers 
        /// cannot fit the incoming batch, or if the topology/index mode changes, or if the batch is a triangle strip. 
        /// </summary>
        /// <param name="primTopology">Topology of the vertex data.</param>
        /// <param name="vertices">Vertex data.</param>
        /// <param name="startVertex">Offset in the vertex buffer to start reading from.</param>
        /// <param name="vertexCount">Number of vertices to read.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the batcher has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if called before begin has been called.</exception>
        /// <exception cref="ArgumentNullException">Thrown if input data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if offsets or counts to read from data will exceed capacity.</exception>
        public void Draw(PrimitiveBatchTopology primTopology, IReadOnlyDataBuffer<T> vertices, int startVertex, int vertexCount)
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("Draw called before begin");
            }

            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            CheckArrayIndex(vertices.Length, startVertex, vertexCount, nameof(startVertex), nameof(vertexCount));

            int actualVertexCount = vertexCount;

            if (primTopology == PrimitiveBatchTopology.QuadList)
            {
                if ((actualVertexCount % 4) != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(vertexCount), "Bad quad index count");
                }

                actualVertexCount = vertexCount + (vertexCount / 4) * 2;
            }

            if (actualVertexCount > _maxVertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCount), "Too many indices");
            }

            // Check if we can merge with previous batch
            bool wrapVertexBuffer = (_currentVertex + actualVertexCount) > _maxVertexCount;
            bool sameTopology = (primTopology == _currentTopology) ||
                                (primTopology == PrimitiveBatchTopology.QuadList && _currentTopology == PrimitiveBatchTopology.TriangleList) ||
                                (primTopology == PrimitiveBatchTopology.TriangleList && _currentTopology == PrimitiveBatchTopology.QuadList);

            if (!sameTopology || _isCurrentlyIndexed || !CanBatch(primTopology) || wrapVertexBuffer)
            {
                ProcessRenderQueue();
            }

            if (wrapVertexBuffer)
            {
                _currentVertex = 0;
            }

            _currentTopology = primTopology;
            _isCurrentlyIndexed = false;

            // Each quad becomes two triangles (so we're adding new points), assumed to be clockwise from the top left corner:
            //
            //  P0   P1   P2   P3
            //     \         \
            //  P7   P6   P5   P4
            //
            // Has two quads {0, 1, 6, 7} and {2, 3, 4, 5} which becomes four triangles {0,1,6},{0,6,7}
            // and {2,3,4}, {2,4,5}.
            //
            if (_currentTopology == PrimitiveBatchTopology.QuadList)
            {
                for (int i = startVertex; i < (startVertex + vertexCount); i += 4)
                {
                    T p0 = vertices[i];
                    T p1 = vertices[i + 1];
                    T p2 = vertices[i + 2];
                    T p3 = vertices[i + 3];

                    _vertices[_currentVertex++] = p0;
                    _vertices[_currentVertex++] = p1;
                    _vertices[_currentVertex++] = p2;
                    _vertices[_currentVertex++] = p0;
                    _vertices[_currentVertex++] = p2;
                    _vertices[_currentVertex++] = p3;
                }

                _numVerticesInBatch += actualVertexCount;
            }
            else
            {
                // Copy over vertex data, just a direct memory copy
                using (MappedDataBuffer dstVerts = _vertices.Map())
                {
                    IntPtr dstPtr = dstVerts.Pointer + (_vertices.ElementSizeInBytes * _currentVertex);
                    using (MappedDataBuffer srcVerts = vertices.Map())
                    {
                        IntPtr srcPtr = srcVerts.Pointer + (vertices.ElementSizeInBytes * startVertex);

                        MemoryHelper.CopyMemory(dstPtr, srcPtr, vertices.ElementSizeInBytes * actualVertexCount);
                        _currentVertex += actualVertexCount;
                        _numVerticesInBatch += actualVertexCount;
                    }
                }
            }
        }

        /// <summary>
        /// Submits an indexed mesh for batching. This may result in a draw call of queued batches if the capacity of the internal buffers 
        /// cannot fit the incoming batch, or if the topology/index mode changes, or if the batch is a triangle strip. 
        /// </summary>
        /// <param name="primTopology">Topology of the vertex data.</param>
        /// <param name="vertices">Vertex data.</param>
        /// <param name="indices">Index data.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the batcher has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if called before begin has been called.</exception>
        /// <exception cref="ArgumentNullException">Thrown if input data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if offsets or counts to read from data will exceed capacity.</exception>
        public void DrawIndexed(PrimitiveBatchTopology primTopology, IReadOnlyDataBuffer<T> vertices, IndexData indices)
        {
            DrawIndexed(primTopology, vertices, 0, (vertices != null) ? vertices.Length : 0, indices, 0, (indices.IsValid) ? indices.Length : 0);
        }

        /// <summary>
        /// Submits an indexed mesh for batching. This may result in a draw call of queued batches if the capacity of the internal buffers 
        /// cannot fit the incoming batch, or if the topology/index mode changes, or if the batch is a triangle strip. 
        /// </summary>
        /// <param name="primTopology">Topology of the vertex data.</param>
        /// <param name="vertices">Vertex data.</param>
        /// <param name="startVertex">Offset in the vertex buffer to start reading from.</param>
        /// <param name="vertexCount">Number of vertices to read.</param>
        /// <param name="indices">Index data.</param>
        /// <param name="startIndex">Offset in the index buffer to start reading from.</param>
        /// <param name="indexCount">Number of indices to read.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the batcher has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if called before begin has been called.</exception>
        /// <exception cref="ArgumentNullException">Thrown if input data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if offsets or counts to read from data will exceed capacity.</exception>
        public void DrawIndexed(PrimitiveBatchTopology primTopology, IReadOnlyDataBuffer<T> vertices, int startVertex, int vertexCount, IndexData indices, int startIndex, int indexCount)
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("Draw called before begin");
            }

            if (!indices.IsValid)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            CheckArrayIndex(vertices.Length, startVertex, vertexCount, nameof(startVertex), nameof(vertexCount));
            CheckArrayIndex(indices.Length, startIndex, indexCount, nameof(startIndex), nameof(indexCount));

            int actualIndexCount = indexCount;

            if (primTopology == PrimitiveBatchTopology.QuadList)
            {
                if ((actualIndexCount % 4) != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(indexCount), "Bad quad index count");
                }

                actualIndexCount = indexCount + ((indexCount / 4) * 2);
            }

            if (vertexCount > _maxVertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCount), "Too many indices");
            }

            if (actualIndexCount > _maxIndexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCount), "Too many indices");
            }

            // Check if we can merge with previous batch
            bool wrapVertexBuffer = (_currentVertex + vertexCount) > _maxVertexCount;
            bool wrapIndexBuffer = (_currentIndex + actualIndexCount) > _maxIndexCount;
            bool sameTopology = (primTopology == _currentTopology) ||
                                (primTopology == PrimitiveBatchTopology.QuadList && _currentTopology == PrimitiveBatchTopology.TriangleList) ||
                                (primTopology == PrimitiveBatchTopology.TriangleList && _currentTopology == PrimitiveBatchTopology.QuadList);

            if (!sameTopology || !_isCurrentlyIndexed || !CanBatch(primTopology) || wrapIndexBuffer || wrapVertexBuffer)
            {
                ProcessRenderQueue();
            }

            if (wrapIndexBuffer)
            {
                _currentIndex = 0;
            }

            if (wrapVertexBuffer)
            {
                _currentVertex = 0;
            }

            _currentTopology = primTopology;
            _isCurrentlyIndexed = true;
            _numIndicesInBatch += actualIndexCount;

            if (_currentTopology == PrimitiveBatchTopology.QuadList)
            {
                // Copy over index data and triangulate the indices (this will add indices)
                // Quad indices are assumed to be in clockwise fashion starting from the top left corner:
                //
                //  P0   P1   P2
                //     \    \
                //  P5   P4   P3
                //
                // Has indices {0,1,4,5} and {1,2,3,4} becomes six triangles with indices {0,1,4},{0,4,5}
                // and {1,2,3},{1,3,4}.
                //

                for (int i = startIndex; i < (startIndex + indexCount); i += 4)
                {
                    int i0 = indices[i] + _currentVertex;
                    int i1 = indices[i + 1] + _currentVertex;
                    int i2 = indices[i + 2] + _currentVertex;
                    int i3 = indices[i + 3] + _currentVertex;

                    _indices[_currentIndex++] = i0;
                    _indices[_currentIndex++] = i1;
                    _indices[_currentIndex++] = i2;
                    _indices[_currentIndex++] = i0;
                    _indices[_currentIndex++] = i2;
                    _indices[_currentIndex++] = i3;
                }
            }
            else
            {
                // Copy over index data
                for (int i = startIndex; i < (startIndex + actualIndexCount); i++)
                {
                    _indices[_currentIndex++] = indices[i] + _currentVertex;
                }
            }

            // Copy over vertex data
            using (MappedDataBuffer dstVerts = _vertices.Map())
            {
                IntPtr dstPtr = dstVerts.Pointer + (_vertices.ElementSizeInBytes * _currentVertex);
                using (MappedDataBuffer srcVerts = vertices.Map())
                {
                    IntPtr srcPtr = srcVerts.Pointer + (vertices.ElementSizeInBytes * startVertex);

                    MemoryHelper.CopyMemory(dstPtr, srcPtr, vertices.ElementSizeInBytes * vertexCount);
                    _currentVertex += vertexCount;
                    _numVerticesInBatch += vertexCount;
                }
            }
        }
        
        /// <summary>
        /// Disposes of unmanaged resources.
        /// </summary>
        /// <param name="isDisposing">True if dispose has been called, false if from destructor.</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (_vertexBuffer != null)
                {
                    _vertexBuffer.Dispose();
                    _vertexBuffer = null;
                }

                if (_indexBuffer != null)
                {
                    _indexBuffer.Dispose();
                    _indexBuffer = null;
                }

                if (_vertices != null)
                {
                    _vertices.Dispose();
                    _vertices = null;
                }

                if (_indices.IsValid)
                {
                    _indices.Dispose();
                }

                if (_tempVertices != null)
                {
                    _tempVertices.Dispose();
                    _tempVertices = null;
                }

                if (_quadIndices != null)
                {
                    _quadIndices.Dispose();
                    _quadIndices = null;
                }
            }

            base.Dispose(isDisposing);
        }
        
        private void SetBuffers()
        {
            _renderContext.SetVertexBuffer(_vertexBuffer);

            if (_maxIndexCount > 0)
            {
                _renderContext.SetIndexBuffer(_indexBuffer);
            }
        }

        // Flush the currently queued primitives
        private void ProcessRenderQueue()
        {
            if (_numVerticesInBatch == 0 || (_isCurrentlyIndexed && _numIndicesInBatch == 0))
            {
                return;
            }

            WriteVertexData();

            if (_isCurrentlyIndexed)
            {
                WriteIndexData();
            }

            if (_isCurrentlyIndexed)
            {
                _renderContext.DrawIndexed(ToPrimitiveType(_currentTopology), _numIndicesInBatch, _currentIndex - _numIndicesInBatch, 0);
            }
            else
            {
                _renderContext.Draw(ToPrimitiveType(_currentTopology), _numVerticesInBatch, _currentVertex - _numVerticesInBatch);
            }

            _numIndicesInBatch = 0;
            _numVerticesInBatch = 0;
        }

        private void WriteIndexData()
        {
            int startIndex = _currentIndex - _numIndicesInBatch;
            int offsetInBytes = ((_indexBuffer.IndexFormat == IndexFormat.ThirtyTwoBits) ? sizeof(int) : sizeof(short)) * startIndex;

            DataWriteOptions writeOptions = (startIndex == 0) ? DataWriteOptions.Discard : DataWriteOptions.NoOverwrite;
            _indexBuffer.SetData(_renderContext, new IndexData(_indices), startIndex, _numIndicesInBatch, offsetInBytes, writeOptions);
        }

        private void WriteVertexData()
        {
            int startIndex = _currentVertex - _numVerticesInBatch;
            int offsetInBytes = _vertexBuffer.VertexLayout.VertexStride * startIndex;

            DataWriteOptions writeOptions = (startIndex == 0) ? DataWriteOptions.Discard : DataWriteOptions.NoOverwrite;
            _vertexBuffer.SetData<T>(_renderContext, _vertices, startIndex, _numVerticesInBatch, offsetInBytes, 0, writeOptions);
        }

        private static bool CanBatch(PrimitiveBatchTopology primTopology)
        {
            switch (primTopology)
            {
                case PrimitiveBatchTopology.TriangleList:
                case PrimitiveBatchTopology.LineList:
                case PrimitiveBatchTopology.PointList:
                case PrimitiveBatchTopology.QuadList:
                    return true;
                default:
                    return false;
            }
        }

        private static PrimitiveType ToPrimitiveType(PrimitiveBatchTopology primTopology)
        {
            switch (primTopology)
            {
                case PrimitiveBatchTopology.TriangleList:
                case PrimitiveBatchTopology.QuadList:
                    return PrimitiveType.TriangleList;
                case PrimitiveBatchTopology.TriangleStrip:
                    return PrimitiveType.TriangleStrip;
                case PrimitiveBatchTopology.LineList:
                    return PrimitiveType.LineList;
                case PrimitiveBatchTopology.LineStrip:
                    return PrimitiveType.LineStrip;
                case PrimitiveBatchTopology.PointList:
                    return PrimitiveType.PointList;
                default:
                    throw new SparkGraphicsException();
            }
        }

        private static void CheckArrayIndex(int length, int index, int count, string paramIndexName, string paramCountName)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(paramCountName, "Count must be greater than or equal to zero");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(paramIndexName, "Index out of range");
            }

            if (index + count > length)
            {
                throw new ArgumentOutOfRangeException(paramIndexName + paramCountName, "Index and count out of range");
            }
        }
    }
}
