namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    
    using Math;
    using Content;
    using Utilities;

    /// <summary>
    /// A renderable that serves as the manager for many instances of single mesh. This uses hardware instancing, where data providers
    /// can be attached (e.g. world transforms), which will provide the per-instance data in a second vertex buffer.
    /// </summary>
    public class InstanceDefinition : IMarkedRenderable, ISavable, IMeshDataContainer
    {
        private IRenderSystem _renderSystem;
        private readonly MarkId _markId;
        private MaterialDefinition _matDef;
        private Transform _worldTransform;
        private LightCollection _worldLights;
        private RenderPropertyCollection _renderProperties;
        private MeshData _meshData;
        private SubMeshRange? _meshRange;
        private VertexBuffer _instanceVertexBuffer;
        private IDataBuffer<byte> _instanceDataBuffer;

        private readonly List<IInstancedRenderable> _instances;
        private readonly List<IInstancedRenderable> _instancesToDraw;
        private bool _renderedOnce;
        private bool _invalidateBuffers;
        private int _sequence;
        private readonly VisitLock _visitLock;
        private readonly SpinLock _lock;

        private readonly Dictionary<String, IInstanceDataProvider> _instanceDataProviderSet;
        private readonly List<IInstanceDataProvider> _instanceDataProviders;

        private readonly VertexBufferBinding[] _vbBindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDefinition"/> class.
        /// </summary>
        public InstanceDefinition() 
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDefinition"/> class.
        /// </summary>
        /// <param name="meshData">MeshData that contains the geometry of the instances.</param>
        public InstanceDefinition(MeshData meshData) 
            : this(null, meshData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDefinition"/> class.
        /// </summary>
        /// <param name="matDef">Materials used to render the instances, needs to support hardware instancing.</param>
        public InstanceDefinition(MaterialDefinition matDef) 
            : this(matDef, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDefinition"/> class.
        /// </summary>
        /// <param name="matDef">Materials used to render the instances, needs to support hardware instancing.</param>
        /// <param name="meshData">MeshData that contains the geometry of the instances.</param>
        public InstanceDefinition(MaterialDefinition matDef, MeshData meshData)
        {
            _matDef = matDef;
            _meshData = meshData;
            _worldTransform = new Transform();
            _worldLights = new LightCollection();
            _worldLights.MonitorLightChanges = true;

            _renderProperties = new RenderPropertyCollection();

            SetDefaultRenderProperties(false);

            _instanceDataProviders = new List<IInstanceDataProvider>();
            _instanceDataProviderSet = new Dictionary<String, IInstanceDataProvider>();

            _instances = new List<IInstancedRenderable>();
            _instancesToDraw = new List<IInstancedRenderable>();
            _instanceVertexBuffer = null;
            _instanceDataBuffer = null;
            _invalidateBuffers = false;
            _renderedOnce = false;
            _sequence = 1;
            _visitLock = new VisitLock();
            _lock = new SpinLock();
            _markId = MarkId.GenerateNewUniqueId();

            _renderSystem = SparkEngine.Instance.Services.GetService<IRenderSystem>();
            _vbBindings = new VertexBufferBinding[2];
        }

        /// <summary>
        /// Gets the material definition that contains the materials used to render the object.
        /// </summary>
        public MaterialDefinition MaterialDefinition
        {
            get => _matDef;
            set => _matDef = value;
        }

        /// <summary>
        /// Gets or sets the geometry that will be used for each instance.
        /// </summary>
        public MeshData MeshData
        {
            get => _meshData;
            set => _meshData = value;
        }

        /// <summary>
        /// Gets submesh information, if it exists. This is the range in the geometry buffers of <see cref="MeshData"/> that
        /// make up the mesh, as <see cref="MeshData"/> may contain more than one mesh. If null, then the entire range of <see cref="MeshData"/> 
        /// is a single mesh.
        /// </summary>
        public SubMeshRange? MeshRange
        {
            get => _meshRange;
            set => _meshRange = value;
        }

        /// <summary>
        /// Gets the ID used to reference this object in the render queue.
        /// </summary>
        public MarkId MarkId => _markId;

        /// <summary>
        /// Gets the world transform of the renderable. At the bare minimum, this render property should be present in the render properties collection.
        /// </summary>
        public Transform WorldTransform => _worldTransform;

        /// <summary>
        /// Gets the world lights of the renderable, if any.
        /// </summary>
        public LightCollection WorldLights => _worldLights;

        /// <summary>
        /// Gets the collection of render properties.
        /// </summary>
        public RenderPropertyCollection RenderProperties => _renderProperties;

        /// <summary>
        /// Gets the collection of instances that the root definition manages.
        /// </summary>
        public IReadOnlyList<IInstancedRenderable> Instances => _instances;

        /// <summary>
        /// Gets the collection of instances that have been marked to be rendered.
        /// </summary>
        public IReadOnlyList<IInstancedRenderable> InstancesToDraw => _instancesToDraw;

        /// <summary>
        /// Gets the collection of instance data providers that feed the per-instance vertex buffer.
        /// </summary>
        public IReadOnlyList<IInstanceDataProvider> InstanceDataProviders => _instanceDataProviders;

        /// <summary>
        /// Gets if the renderable is valid for drawing.
        /// </summary>
        public bool IsValidForDraw
        {
            get
            {
                bool invalid = _matDef == null || 
                               _matDef.Count == 0 || 
                               !_matDef.AreMaterialsValid() || 
                               _instances.Count == 0 || 
                               _meshData == null || 
                               _meshData.VertexBuffer == null || 
                               _meshData.VertexBuffer.IsDisposed || 
                               (_meshData.UseIndexedPrimitives && (_meshData.IndexBuffer == null || _meshData.IndexBuffer.IsDisposed));

                return !invalid;
            }
        }

        private bool HasInstanceData => _instanceDataProviders.Count > 0;

        /// <summary>
        /// Sets the default render properties.
        /// </summary>
        /// <param name="fromSavable">True if reading from a savable input, false otherwise.</param>
        protected void SetDefaultRenderProperties(bool fromSavable)
        {
            _renderProperties.Add(new WorldTransformProperty(_worldTransform));
            _renderProperties.Add(new LightCollectionProperty(_worldLights));

            //Will be serialized...
            if (!fromSavable)
            {
                _renderProperties.Add(new OrthoOrderProperty(0));
            }
        }

        /// <summary>
        /// Called when an instance is to be considered for rendering. This will ensure the instance definition to be added
        /// to the render queue if not present, and add the instance to be drawn. This method is thread safe.
        /// </summary>
        /// <param name="renderer">Renderer used to process objects to be rendered.</param>
        /// <param name="instance">Instance that should be drawn.</param>
        public void NotifyToDraw(IRenderer renderer, IInstancedRenderable instance)
        {
            if (renderer == null || instance == null || instance.InstanceDefinition != this)
                return;

            if (_visitLock.IsFirstVisit(_sequence))
            {
                if (renderer.RenderQueue.Mark(_markId, this))
                    renderer.Process(this);
            }

            bool lockTaken = false;
            _lock.Enter(ref lockTaken);

            _instancesToDraw.Add(instance);

            _lock.Exit();
        }

        /// <summary>
        /// Queues the instance definition to the render queue if not present and adds all the instances to be drawn.
        /// This method is not thread safe.
        /// </summary>
        /// <param name="renderer">Renderer used to process objects to be rendered.</param>
        public void NotifyToDrawAll(IRenderer renderer)
        {
            //If # of instances to draw equals the # of instances, then this is a redundant call
            if (renderer == null || _instancesToDraw.Count == _instances.Count)
                return;

            if (_visitLock.IsFirstVisit(_sequence))
            {
                if (renderer.RenderQueue.Mark(_markId, this))
                    renderer.Process(this);
            }

            //Make sure we don't add duplicates
            if (_instancesToDraw.Count > 0)
                _instancesToDraw.Clear();

            for (int i = 0; i < _instances.Count; i++)
                _instancesToDraw.Add(_instances[i]);
        }

        /// <summary>
        /// Binds an instance to the definition. The instance definition will have a reference to the instance and vice versa, when the
        /// instance is processed to be drawn it should call the method <see cref="NotifyToDraw(IRenderer, IInstancedRenderable)"/> to queue
        /// itself to be drawn.
        /// </summary>
        /// <param name="instance">Instance to be added.</param>
        /// <returns>True if the instance has been successfully added to the definition, false otherwise.</returns>
        public bool AddInstance(IInstancedRenderable instance)
        {
            if (instance == null || instance.InstanceDefinition != null)
                return false;

            if (instance.SetInstanceDefinition(this))
            {
                _instances.Add(instance);

                //Make sure the instances to draw is the same or greater capacity so we don't run into lots of allocations when notifying to draw
                _instancesToDraw.Capacity = Math.Max(_instances.Capacity, _instancesToDraw.Capacity);
                InvalidateInstanceBuffers();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes an instance binding from the definition.
        /// </summary>
        /// <param name="instance">Instance to be removed.</param>
        /// <returns>True if the instance has been successfully removed from the definition, false otherwise.</returns>
        public bool RemoveInstance(IInstancedRenderable instance)
        {
            if (instance == null || instance.InstanceDefinition != null)
                return false;

            _instances.Remove(instance);
            instance.RemoveInstanceDefinition();
            InvalidateInstanceBuffers();
            return true;
        }

        /// <summary>
        /// Adds an instance data provider to the definition. The order of which data providers are added to the instance definition matters and needs
        /// to match the vertex layout of the per-instance vertex data in the shader.
        /// </summary>
        /// <param name="dataProvider">Data provider to be added.</param>
        /// <returns>True if the data provider was successfully added, false if otherwise.</returns>
        public bool AddInstanceData(IInstanceDataProvider dataProvider)
        {
            if (dataProvider == null || _instanceDataProviderSet.ContainsKey(dataProvider.InstanceDataName))
                return false;

            _instanceDataProviderSet.Add(dataProvider.InstanceDataName, dataProvider);
            _instanceDataProviders.Add(dataProvider);

            InvalidateInstanceBuffers();

            return true;
        }

        /// <summary>
        /// Removes an instance data provider from the definition.
        /// </summary>
        /// <param name="dataProvider">Data provider to be removed.</param>
        /// <returns>True if the data provider was successfully removed, false if otherwise.</returns>
        public bool RemoveInstanceData(IInstanceDataProvider dataProvider)
        {
            if (dataProvider == null)
                return false;

            return RemoveInstanceData(dataProvider.InstanceDataName);
        }

        /// <summary>
        /// Removes an instance data provider from the definition.
        /// </summary>
        /// <param name="instanceDataName">Name of the instance provider.</param>
        /// <returns>True if the data provider was successfully removed, false if otherwise.</returns>
        public bool RemoveInstanceData(String instanceDataName)
        {
            if (String.IsNullOrEmpty(instanceDataName) || !_instanceDataProviderSet.ContainsKey(instanceDataName))
                return false;

            IInstanceDataProvider dataProvider;
            if (_instanceDataProviderSet.TryGetValue(instanceDataName, out dataProvider))
            {
                InvalidateInstanceBuffers();
                _instanceDataProviderSet.Remove(instanceDataName);
                return _instanceDataProviders.Remove(dataProvider);
            }

            return false;
        }

        /// <summary>
        /// Removes all instances from the definition and the associations of them to it.
        /// </summary>
        public void ClearInstances()
        {
            for (int i = 0; i < _instances.Count; i++)
            {
                IInstancedRenderable instance = _instances[i];
                if (instance != null)
                    instance.RemoveInstanceDefinition();
            }

            _instances.Clear();
            _instancesToDraw.Clear(); //make sure this is clear in case we queue up instances but don't actually draw
            InvalidateInstanceBuffers();
        }

        /// <summary>
        /// Remoes all instance data providers.
        /// </summary>
        public void ClearInstanceData()
        {
            _instanceDataProviders.Clear();
            _instanceDataProviderSet.Clear();
            InvalidateInstanceBuffers();
        }

        /// <summary>
        /// Performs the necessary draw calls to render the object.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="renderBucketId">The current bucket being drawn, may be invalid.</param>
        /// <param name="currentPass">The current pass that is drawing the renderable, may be null.</param>
        public void SetupDrawCall(IRenderContext renderContext, RenderBucketId renderBucketId, MaterialPass currentPass)
        {
            int instancesToDrawCount = _instancesToDraw.Count;

            if (instancesToDrawCount == 0)
                return;

            PrepRendering();

            BindBuffers(renderContext);

            PrimitiveType primType = _meshData.PrimitiveType;
            bool useIndices = _meshData.UseIndexedPrimitives;
            int offset;
            int count;
            int baseVertexOffset;

            GraphicsHelper.GetMeshDrawParameters(_meshData, ref _meshRange, out offset, out count, out baseVertexOffset);

            if (HasInstanceData)
            {
                //Always discard the instance buffer when filling, even if only drawing a small number of objects. Tests showed that
                //doing a circular buffer didn't give us any boost and caused multiple draw calls even for small batches. Maybe
                //one day come up with a better algorithm for that
                if (!_renderedOnce)
                {
                    FillBuffer();
                    _instanceVertexBuffer.SetData<byte>(renderContext, _instanceDataBuffer, DataWriteOptions.Discard);
                }

                //Draw the batch
                if (useIndices)
                {
                    renderContext.DrawIndexedInstanced(primType, count, instancesToDrawCount, offset, baseVertexOffset, 0);
                }
                else
                {
                    renderContext.DrawInstanced(primType, count, instancesToDrawCount, offset, 0);
                }
            }
            else
            {
                if (useIndices)
                {
                    renderContext.DrawIndexedInstanced(primType, count, instancesToDrawCount, offset, baseVertexOffset, 0);
                }
                else
                {
                    renderContext.DrawInstanced(primType, count, instancesToDrawCount, offset, 0);
                }
            }

            _renderedOnce = true;
        }

        /// <summary>
        /// Called when the renderable gets cleared from the queue.
        /// </summary>
        /// <param name="id">ID that corresponded to the renderale in the queue.</param>
        /// <param name="queue">Render queue that the renderable was marked in.</param>
        void IMarkedRenderable.OnMarkCleared(MarkId id, RenderQueue queue)
        {
            _sequence++;
            _instancesToDraw.Clear();
            _renderedOnce = false;
        }

        private void BindBuffers(IRenderContext renderContext)
        {
            if (HasInstanceData)
            {
                _vbBindings[0] = new VertexBufferBinding(_meshData.VertexBuffer);
                _vbBindings[1] = new VertexBufferBinding(_instanceVertexBuffer, 0, 1);
                renderContext.SetVertexBuffers(_vbBindings);
            }
            else
            {
                renderContext.SetVertexBuffer(_meshData.VertexBuffer);
            }

            if (_meshData.UseIndexedPrimitives)
                renderContext.SetIndexBuffer(_meshData.IndexBuffer);
        }

        private void FillBuffer()
        {
            _instanceDataBuffer.Position = 0;
            MappedDataBuffer dbPtr = _instanceDataBuffer.Map();

            try
            {
                for (int i = 0; i < _instancesToDraw.Count; i++)
                {
                    IInstancedRenderable renderable = _instancesToDraw[i];
                    for (int j = 0; j < _instanceDataProviders.Count; j++)
                        _instanceDataProviders[j].SetData(this, renderable.RenderProperties, ref dbPtr);
                }
            }
            finally
            {
                _instanceDataBuffer.Unmap();
            }
        }

        private void InvalidateInstanceBuffers()
        {
            _invalidateBuffers = true;
            _renderedOnce = false;
        }

        private void PrepRendering()
        {
            if (_invalidateBuffers)
            {
                if (_instanceDataBuffer != null)
                {
                    _instanceDataBuffer.Dispose();
                    _instanceDataBuffer = null;
                }

                if (_instanceVertexBuffer != null)
                {
                    _instanceVertexBuffer.Dispose();
                    _instanceVertexBuffer = null;
                }

                _invalidateBuffers = false;
            }

            if (_instanceVertexBuffer == null && HasInstanceData)
                CreateInstanceBuffers(_instances.Count, _instanceDataProviders);
        }

        private void CreateInstanceBuffers(int maxInstances, IList<IInstanceDataProvider> dataProviders)
        {
            List<VertexElement> vertexElements = new List<VertexElement>();
            Dictionary<VertexSemantic, int> semanticIndexCounter = new Dictionary<VertexSemantic, int>();
            ScanVertexLayout(_meshData.VertexBuffer.VertexLayout, semanticIndexCounter);

            int vertexOffset = 0;
            for (int i = 0; i < dataProviders.Count; i++)
            {
                IInstanceDataProvider provider = dataProviders[i];
                VertexElement[] elems = provider.DataLayout;
                for (int j = 0; j < elems.Length; j++)
                {
                    VertexElement incElem = elems[j];

                    int currIndex = 0;
                    if (semanticIndexCounter.TryGetValue(incElem.SemanticName, out currIndex))
                        currIndex++;

                    semanticIndexCounter[incElem.SemanticName] = currIndex;
                    vertexElements.Add(new VertexElement(incElem.SemanticName, currIndex, incElem.Format, vertexOffset));
                    vertexOffset += incElem.Format.SizeInBytes();
                }
            }

            VertexLayout layout = new VertexLayout(vertexElements.ToArray());
            _instanceVertexBuffer = new VertexBuffer(_renderSystem, layout, maxInstances, ResourceUsage.Dynamic);
            _instanceDataBuffer = new DataBuffer<byte>(maxInstances * layout.VertexStride);
        }

        private void ScanVertexLayout(VertexLayout layout, Dictionary<VertexSemantic, int> semanticIndexCounter)
        {
            for (int i = 0; i < layout.ElementCount; i++)
            {
                VertexElement elem = layout[i];

                int currIndex = elem.SemanticIndex;
                if (semanticIndexCounter.TryGetValue(elem.SemanticName, out currIndex))
                    currIndex = Math.Max(currIndex, elem.SemanticIndex);

                semanticIndexCounter[elem.SemanticName] = currIndex;
            }
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            _matDef = input.ReadExternalSavable<MaterialDefinition>();
            _worldTransform = input.ReadSavable<Transform>();
            _worldLights = input.ReadSavable<LightCollection>();
            _renderProperties = input.ReadSavable<RenderPropertyCollection>();
            _meshData = input.ReadSharedSavable<MeshData>();
            input.ReadNullable<SubMeshRange>(out _meshRange);

            int instanceDataCount = input.ReadInt32();
            for (int i = 0; i < instanceDataCount; i++)
            {
                IInstanceDataProvider instanceData = input.ReadSavable<IInstanceDataProvider>();
                _instanceDataProviders.Add(instanceData);
                _instanceDataProviderSet.Add(instanceData.InstanceDataName, instanceData);
            }

            SetDefaultRenderProperties(true);

            //Fixup render system
            _renderSystem = GraphicsHelper.GetRenderSystem(input.ServiceProvider);
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.WriteExternalSavable<MaterialDefinition>("MaterialDefinition", _matDef);
            output.WriteSavable<Transform>("WorldTransform", _worldTransform);
            output.WriteSavable<LightCollection>("WorldLights", _worldLights);
            output.WriteSavable<RenderPropertyCollection>("RenderProperties", _renderProperties);
            output.WriteSharedSavable<MeshData>("MeshData", _meshData);
            output.WriteNullable<SubMeshRange>("MeshRange", _meshRange);

            output.Write("InstanceDataProviderCount", _instanceDataProviders.Count);
            for (int i = 0; i < _instanceDataProviders.Count; i++)
            {
                output.WriteSavable<IInstanceDataProvider>("InstanceData", _instanceDataProviders[i]);
            }

            //Don't write out instances...they should write a reference to us as a SharedSavable and Add us
        }
    }
}
