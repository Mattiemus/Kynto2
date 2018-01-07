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
        private IRenderSystem m_renderSystem;
        private MarkId m_markId = MarkId.Invalid;
        private MaterialDefinition m_matDef;
        private Transform m_worldTransform;
        private LightCollection m_worldLights;
        private RenderPropertyCollection m_renderProperties;
        private MeshData m_meshData;
        private SubMeshRange? m_meshRange;
        private VertexBuffer m_instanceVertexBuffer;
        private IDataBuffer<byte> m_instanceDataBuffer;

        private List<IInstancedRenderable> m_instances;
        private List<IInstancedRenderable> m_instancesToDraw;
        private bool m_renderedOnce;
        private bool m_invalidateBuffers;
        private int m_sequence;
        private VisitLock m_visitLock;
        private SpinLock m_lock;

        private Dictionary<String, IInstanceDataProvider> m_instanceDataProviderSet;
        private List<IInstanceDataProvider> m_instanceDataProviders;

        private VertexBufferBinding[] m_vbBindings;

        /// <summary>
        /// Gets the material definition that contains the materials used to render the object.
        /// </summary>
        public MaterialDefinition MaterialDefinition
        {
            get
            {
                return m_matDef;
            }
            set
            {
                m_matDef = value;
            }
        }

        /// <summary>
        /// Gets or sets the geometry that will be used for each instance.
        /// </summary>
        public MeshData MeshData
        {
            get
            {
                return m_meshData;
            }
            set
            {
                m_meshData = value;
            }
        }

        /// <summary>
        /// Gets submesh information, if it exists. This is the range in the geometry buffers of <see cref="MeshData"/> that
        /// make up the mesh, as <see cref="MeshData"/> may contain more than one mesh. If null, then the entire range of <see cref="MeshData"/> 
        /// is a single mesh.
        /// </summary>
        public SubMeshRange? MeshRange
        {
            get
            {
                return m_meshRange;
            }
            set
            {
                m_meshRange = value;
            }
        }

        /// <summary>
        /// Gets the ID used to reference this object in the render queue.
        /// </summary>
        public MarkId MarkId
        {
            get
            {
                return m_markId;
            }
        }

        /// <summary>
        /// Gets the world transform of the renderable. At the bare minimum, this render property should be present in the render properties collection.
        /// </summary>
        public Transform WorldTransform
        {
            get
            {
                return m_worldTransform;
            }
        }

        /// <summary>
        /// Gets the world lights of the renderable, if any.
        /// </summary>
        public LightCollection WorldLights
        {
            get
            {
                return m_worldLights;
            }
        }

        /// <summary>
        /// Gets the collection of render properties.
        /// </summary>
        public RenderPropertyCollection RenderProperties
        {
            get
            {
                return m_renderProperties;
            }
        }

        /// <summary>
        /// Gets the collection of instances that the root definition manages.
        /// </summary>
        public IReadOnlyList<IInstancedRenderable> Instances
        {
            get
            {
                return m_instances;
            }
        }

        /// <summary>
        /// Gets the collection of instances that have been marked to be rendered.
        /// </summary>
        public IReadOnlyList<IInstancedRenderable> InstancesToDraw
        {
            get
            {
                return m_instancesToDraw;
            }
        }

        /// <summary>
        /// Gets the collection of instance data providers that feed the per-instance vertex buffer.
        /// </summary>
        public IReadOnlyList<IInstanceDataProvider> InstanceDataProviders
        {
            get
            {
                return m_instanceDataProviders;
            }
        }

        /// <summary>
        /// Gets if the renderable is valid for drawing.
        /// </summary>
        public bool IsValidForDraw
        {
            get
            {
                bool invalid = m_matDef == null || m_matDef.Count == 0 || !m_matDef.AreMaterialsValid() || m_instances.Count == 0 || m_meshData == null || m_meshData.VertexBuffer == null || m_meshData.VertexBuffer.IsDisposed
                        || (m_meshData.UseIndexedPrimitives && (m_meshData.IndexBuffer == null || m_meshData.IndexBuffer.IsDisposed));

                return !invalid;
            }
        }

        private bool HasInstanceData
        {
            get
            {
                return m_instanceDataProviders.Count > 0;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="InstanceDefinition"/> class.
        /// </summary>
        public InstanceDefinition() : this(null, null) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="InstanceDefinition"/> class.
        /// </summary>
        /// <param name="meshData">MeshData that contains the geometry of the instances.</param>
        public InstanceDefinition(MeshData meshData) : this(null, meshData) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="InstanceDefinition"/> class.
        /// </summary>
        /// <param name="matDef">Materials used to render the instances, needs to support hardware instancing.</param>
        public InstanceDefinition(MaterialDefinition matDef) : this(matDef, null) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="InstanceDefinition"/> class.
        /// </summary>
        /// <param name="matDef">Materials used to render the instances, needs to support hardware instancing.</param>
        /// <param name="meshData">MeshData that contains the geometry of the instances.</param>
        public InstanceDefinition(MaterialDefinition matDef, MeshData meshData)
        {
            m_matDef = matDef;
            m_meshData = meshData;
            m_worldTransform = new Transform();
            m_worldLights = new LightCollection();
            m_worldLights.MonitorLightChanges = true;

            m_renderProperties = new RenderPropertyCollection();

            SetDefaultRenderProperties(false);

            m_instanceDataProviders = new List<IInstanceDataProvider>();
            m_instanceDataProviderSet = new Dictionary<String, IInstanceDataProvider>();

            m_instances = new List<IInstancedRenderable>();
            m_instancesToDraw = new List<IInstancedRenderable>();
            m_instanceVertexBuffer = null;
            m_instanceDataBuffer = null;
            m_invalidateBuffers = false;
            m_renderedOnce = false;
            m_sequence = 1;
            m_visitLock = new VisitLock();
            m_lock = new SpinLock();
            m_markId = MarkId.GenerateNewUniqueId();

            m_renderSystem = SparkEngine.Instance.Services.GetService<IRenderSystem>();
            m_vbBindings = new VertexBufferBinding[2];
        }

        /// <summary>
        /// Sets the default render properties.
        /// </summary>
        /// <param name="fromSavable">True if reading from a savable input, false otherwise.</param>
        protected void SetDefaultRenderProperties(bool fromSavable)
        {
            m_renderProperties.Add(new WorldTransformProperty(m_worldTransform));
            m_renderProperties.Add(new LightCollectionProperty(m_worldLights));

            //Will be serialized...
            if (!fromSavable)
            {
                m_renderProperties.Add(new OrthoOrderProperty(0));
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

            if (m_visitLock.IsFirstVisit(m_sequence))
            {
                if (renderer.RenderQueue.Mark(m_markId, this))
                    renderer.Process(this);
            }

            bool lockTaken = false;
            m_lock.Enter(ref lockTaken);

            m_instancesToDraw.Add(instance);

            m_lock.Exit();
        }

        /// <summary>
        /// Queues the instance definition to the render queue if not present and adds all the instances to be drawn.
        /// This method is not thread safe.
        /// </summary>
        /// <param name="renderer">Renderer used to process objects to be rendered.</param>
        public void NotifyToDrawAll(IRenderer renderer)
        {
            //If # of instances to draw equals the # of instances, then this is a redundant call
            if (renderer == null || m_instancesToDraw.Count == m_instances.Count)
                return;

            if (m_visitLock.IsFirstVisit(m_sequence))
            {
                if (renderer.RenderQueue.Mark(m_markId, this))
                    renderer.Process(this);
            }

            //Make sure we don't add duplicates
            if (m_instancesToDraw.Count > 0)
                m_instancesToDraw.Clear();

            for (int i = 0; i < m_instances.Count; i++)
                m_instancesToDraw.Add(m_instances[i]);
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
                m_instances.Add(instance);

                //Make sure the instances to draw is the same or greater capacity so we don't run into lots of allocations when notifying to draw
                m_instancesToDraw.Capacity = Math.Max(m_instances.Capacity, m_instancesToDraw.Capacity);
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

            m_instances.Remove(instance);
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
            if (dataProvider == null || m_instanceDataProviderSet.ContainsKey(dataProvider.InstanceDataName))
                return false;

            m_instanceDataProviderSet.Add(dataProvider.InstanceDataName, dataProvider);
            m_instanceDataProviders.Add(dataProvider);

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
            if (String.IsNullOrEmpty(instanceDataName) || !m_instanceDataProviderSet.ContainsKey(instanceDataName))
                return false;

            IInstanceDataProvider dataProvider;
            if (m_instanceDataProviderSet.TryGetValue(instanceDataName, out dataProvider))
            {
                InvalidateInstanceBuffers();
                m_instanceDataProviderSet.Remove(instanceDataName);
                return m_instanceDataProviders.Remove(dataProvider);
            }

            return false;
        }

        /// <summary>
        /// Removes all instances from the definition and the associations of them to it.
        /// </summary>
        public void ClearInstances()
        {
            for (int i = 0; i < m_instances.Count; i++)
            {
                IInstancedRenderable instance = m_instances[i];
                if (instance != null)
                    instance.RemoveInstanceDefinition();
            }

            m_instances.Clear();
            m_instancesToDraw.Clear(); //make sure this is clear in case we queue up instances but don't actually draw
            InvalidateInstanceBuffers();
        }

        /// <summary>
        /// Remoes all instance data providers.
        /// </summary>
        public void ClearInstanceData()
        {
            m_instanceDataProviders.Clear();
            m_instanceDataProviderSet.Clear();
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
            int instancesToDrawCount = m_instancesToDraw.Count;

            if (instancesToDrawCount == 0)
                return;

            PrepRendering();

            BindBuffers(renderContext);

            PrimitiveType primType = m_meshData.PrimitiveType;
            bool useIndices = m_meshData.UseIndexedPrimitives;
            int offset;
            int count;
            int baseVertexOffset;

            GraphicsHelper.GetMeshDrawParameters(m_meshData, ref m_meshRange, out offset, out count, out baseVertexOffset);

            if (HasInstanceData)
            {
                //Always discard the instance buffer when filling, even if only drawing a small number of objects. Tests showed that
                //doing a circular buffer didn't give us any boost and caused multiple draw calls even for small batches. Maybe
                //one day come up with a better algorithm for that
                if (!m_renderedOnce)
                {
                    FillBuffer();
                    m_instanceVertexBuffer.SetData<byte>(renderContext, m_instanceDataBuffer, DataWriteOptions.Discard);
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

            m_renderedOnce = true;
        }

        /// <summary>
        /// Called when the renderable gets cleared from the queue.
        /// </summary>
        /// <param name="id">ID that corresponded to the renderale in the queue.</param>
        /// <param name="queue">Render queue that the renderable was marked in.</param>
        void IMarkedRenderable.OnMarkCleared(MarkId id, RenderQueue queue)
        {
            m_sequence++;
            m_instancesToDraw.Clear();
            m_renderedOnce = false;
        }

        private void BindBuffers(IRenderContext renderContext)
        {
            if (HasInstanceData)
            {
                m_vbBindings[0] = new VertexBufferBinding(m_meshData.VertexBuffer);
                m_vbBindings[1] = new VertexBufferBinding(m_instanceVertexBuffer, 0, 1);
                renderContext.SetVertexBuffers(m_vbBindings);
            }
            else
            {
                renderContext.SetVertexBuffer(m_meshData.VertexBuffer);
            }

            if (m_meshData.UseIndexedPrimitives)
                renderContext.SetIndexBuffer(m_meshData.IndexBuffer);
        }

        private void FillBuffer()
        {
            m_instanceDataBuffer.Position = 0;
            MappedDataBuffer dbPtr = m_instanceDataBuffer.Map();

            try
            {
                for (int i = 0; i < m_instancesToDraw.Count; i++)
                {
                    IInstancedRenderable renderable = m_instancesToDraw[i];
                    for (int j = 0; j < m_instanceDataProviders.Count; j++)
                        m_instanceDataProviders[j].SetData(this, renderable.RenderProperties, ref dbPtr);
                }
            }
            finally
            {
                m_instanceDataBuffer.Unmap();
            }
        }

        private void InvalidateInstanceBuffers()
        {
            m_invalidateBuffers = true;
            m_renderedOnce = false;
        }

        private void PrepRendering()
        {
            if (m_invalidateBuffers)
            {
                if (m_instanceDataBuffer != null)
                {
                    m_instanceDataBuffer.Dispose();
                    m_instanceDataBuffer = null;
                }

                if (m_instanceVertexBuffer != null)
                {
                    m_instanceVertexBuffer.Dispose();
                    m_instanceVertexBuffer = null;
                }

                m_invalidateBuffers = false;
            }

            if (m_instanceVertexBuffer == null && HasInstanceData)
                CreateInstanceBuffers(m_instances.Count, m_instanceDataProviders);
        }

        private void CreateInstanceBuffers(int maxInstances, IList<IInstanceDataProvider> dataProviders)
        {
            List<VertexElement> vertexElements = new List<VertexElement>();
            Dictionary<VertexSemantic, int> semanticIndexCounter = new Dictionary<VertexSemantic, int>();
            ScanVertexLayout(m_meshData.VertexBuffer.VertexLayout, semanticIndexCounter);

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
            m_instanceVertexBuffer = new VertexBuffer(m_renderSystem, layout, maxInstances, ResourceUsage.Dynamic);
            m_instanceDataBuffer = new DataBuffer<byte>(maxInstances * layout.VertexStride);
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
            m_matDef = input.ReadExternalSavable<MaterialDefinition>();
            m_worldTransform = input.ReadSavable<Transform>();
            m_worldLights = input.ReadSavable<LightCollection>();
            m_renderProperties = input.ReadSavable<RenderPropertyCollection>();
            m_meshData = input.ReadSharedSavable<MeshData>();
            input.ReadNullable<SubMeshRange>(out m_meshRange);

            int instanceDataCount = input.ReadInt32();
            for (int i = 0; i < instanceDataCount; i++)
            {
                IInstanceDataProvider instanceData = input.ReadSavable<IInstanceDataProvider>();
                m_instanceDataProviders.Add(instanceData);
                m_instanceDataProviderSet.Add(instanceData.InstanceDataName, instanceData);
            }

            SetDefaultRenderProperties(true);

            //Fixup render system
            m_renderSystem = GraphicsHelper.GetRenderSystem(input.ServiceProvider);
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.WriteExternalSavable<MaterialDefinition>("MaterialDefinition", m_matDef);
            output.WriteSavable<Transform>("WorldTransform", m_worldTransform);
            output.WriteSavable<LightCollection>("WorldLights", m_worldLights);
            output.WriteSavable<RenderPropertyCollection>("RenderProperties", m_renderProperties);
            output.WriteSharedSavable<MeshData>("MeshData", m_meshData);
            output.WriteNullable<SubMeshRange>("MeshRange", m_meshRange);

            output.Write("InstanceDataProviderCount", m_instanceDataProviders.Count);
            for (int i = 0; i < m_instanceDataProviders.Count; i++)
            {
                output.WriteSavable<IInstanceDataProvider>("InstanceData", m_instanceDataProviders[i]);
            }

            //Don't write out instances...they should write a reference to us as a SharedSavable and Add us
        }
    }
}
