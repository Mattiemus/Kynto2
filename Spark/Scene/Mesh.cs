namespace Spark.Scene
{
    using System;
    using System.Collections.Generic;

    using Math;
    using Graphics;
    using Content;

    public class Mesh : Visual, IInstancedRenderable, IMeshDataContainer, IPickable
    {
        private MeshData _meshData;
        private SubMeshRange? _meshRange;
        private InstanceDefinition _instanceDefinition;

        protected Mesh()
        {
        }

        public Mesh(string name) 
            : this(name, null)
        {
        }

        public Mesh(string name, MeshData meshData)
            : base(name)
        {
            _meshData = (meshData == null) ? new MeshData() : meshData;
        }

        public MeshData MeshData
        {
            get => _meshData;
            set
            {
                if (_instanceDefinition == null)
                {
                    _meshData = value;
                }
            }
        }

        public SubMeshRange? MeshRange
        {
            get => _meshRange;
            set
            {
                if (_instanceDefinition == null)
                {
                    _meshRange = value;
                }
            }
        }

        public InstanceDefinition InstanceDefinition => _instanceDefinition;

        public bool IsInstanced => _instanceDefinition != null;

        public override bool IsValidForDraw
        {
            get
            {
                if (_instanceDefinition?.IsValidForDraw == true)
                {
                    return true;
                }

                bool invalid = !IsMaterialValid || 
                    _meshData == null || 
                    _meshData.VertexBuffer == null || 
                    _meshData.VertexBuffer.IsDisposed || 
                    (_meshData.UseIndexedPrimitives && (_meshData.IndexBuffer == null || _meshData.IndexBuffer.IsDisposed));

                return !invalid;
            }
        }

        public void MakeInstanced(InstanceDefinition instanceDef)
        {
            if (instanceDef == null)
            {
                return;
            }

            _instanceDefinition?.RemoveInstance(this);

            // SetInstancedDefinition will be called
            instanceDef.AddInstance(this);
        }

        public override void MakeInstanced(MaterialDefinition instanceMatDef, bool copyMatDef, params IInstanceDataProvider[] instanceDataProviders)
        {
            if (copyMatDef && instanceMatDef != null)
            {
                instanceMatDef = instanceMatDef.Clone();
            }

            var instanceDef = new InstanceDefinition(instanceMatDef, _meshData);

            if (instanceDataProviders != null)
            {
                for (int i = 0; i < instanceDataProviders.Length; i++)
                {
                    instanceDef.AddInstanceData(instanceDataProviders[i]);
                }
            }

            // SetInstancedDefinition will be called
            instanceDef.AddInstance(this);
        }

        public override void MakeNonInstanced(MaterialDefinition newMatDef, bool copyMatDef)
        {
            _instanceDefinition?.RemoveInstance(this);

            SetMaterialDefinition(newMatDef, copyMatDef);
        }

        protected override void OnProcessVisibleSet(IRenderer renderer)
        {
            if (IsInstanced)
            {
                _instanceDefinition.NotifyToDraw(renderer, this);
            }
            else
            {
                // If no material, should not be drawn at all
                if (!IsMaterialValid)
                {
                    return;
                }

                renderer.Process(this);
            }
        }

        public override void SetupDrawCall(IRenderContext renderContext, RenderBucketId currentBucketId, MaterialPass currentPass)
        {
            renderContext.SetVertexBuffer(_meshData.VertexBuffer);
            
            GraphicsHelper.GetMeshDrawParameters(_meshData, ref _meshRange, out int offset, out int count, out int baseVertexOffset);

            if (_meshData.UseIndexedPrimitives)
            {
                renderContext.SetIndexBuffer(_meshData.IndexBuffer);
                renderContext.DrawIndexed(_meshData.PrimitiveType, count, offset, baseVertexOffset);
            }
            else
            {
                renderContext.Draw(_meshData.PrimitiveType, count, offset);
            }
        }

        public override void UpdateModelBounding()
        {
            BoundingVolume modelBounding = ModelBounding;

            if (_meshData == null || modelBounding == null)
            {
                return;
            }

            if (_meshData.UseIndexedPrimitives)
            {
                IDataBuffer<Vector3> pos = _meshData.Positions;
                IndexData? indices = _meshData.Indices;

                if (pos != null && indices.HasValue)
                {
                    modelBounding.ComputeFromIndexedPoints(pos, indices.Value, _meshRange);
                }
            }
            else
            {
                IDataBuffer<Vector3> pos = _meshData.Positions;
                if (pos != null)
                {
                    modelBounding.ComputeFromPoints(pos, _meshRange);
                }
            }

            PropagateDirtyUp(DirtyMark.Bounding);
        }

        public override void Read(ISavableReader input)
        {
            base.Read(input);

            _meshData = input.ReadSharedSavable<MeshData>();
            input.ReadNullable(out _meshRange);
            _instanceDefinition = input.ReadSharedSavable<InstanceDefinition>();
        }

        public override void Write(ISavableWriter output)
        {
            base.Write(output);

            output.WriteSharedSavable("MeshData", _meshData);
            output.WriteNullable("MeshRange", _meshRange);
            output.WriteSharedSavable("InstanceDefinition", _instanceDefinition);
        }

        protected override void PopulateClone(Spatial clone, CopyContext copyContext)
        {
            base.PopulateClone(clone, copyContext);

            var msh = clone as Mesh;
            if (msh == null)
            {
                return;
            }

            // If instanced...add the new instance
            if (_instanceDefinition != null)
            {
                _instanceDefinition.AddInstance(msh);
            }
            else
            {
                // Copy mesh data if want to, else just share it
                if (copyContext[CopyContext.CopyMeshDataKey])
                {
                    msh._meshData = _meshData.Clone();
                }
                else
                {
                    msh._meshData = _meshData;
                }

                msh._meshRange = _meshRange;
            }
        }

        #region IInstancedRenderable

        bool IInstancedRenderable.SetInstanceDefinition(InstanceDefinition instanceDef)
        {
            _instanceDefinition = instanceDef;
            _meshData = instanceDef.MeshData;
            _meshRange = instanceDef.MeshRange;
            MaterialDefinition = null; // If being instanced now, set the material to null

            return true;
        }

        void IInstancedRenderable.RemoveInstanceDefinition()
        {
            _instanceDefinition = null;
            _meshData = null;
            _meshRange = null;
        }

        public bool IntersectsMesh(ref Ray ray, IList<Tuple<LineIntersectionResult, Triangle?>> results, bool ignoreBackfaces)
        {
            if (_meshData == null)
            {
                return false;
            }
            
            WorldTransform.GetMatrix(out Matrix4x4 worldMatrix);

            return _meshData.Intersects(ref ray, ref worldMatrix, ref _meshRange, results, ignoreBackfaces);
        }

        #endregion
    }
}
