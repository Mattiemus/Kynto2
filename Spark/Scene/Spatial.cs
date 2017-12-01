namespace Spark.Scene
{
    using System;
    using System.Collections.Generic;
    
    using Math;
    using Content;
    using Graphics;
    using Utilities;

    public abstract class Spatial : ISpatial, ITransformed, ISavable, IHintable
    {
        public static readonly ushort VersionNumber = 1;

        private Node _parent;
        private DirtyMark _dirtyMark;
        private Transform _transform;
        private Transform _worldTransform;
        private BoundingVolume _worldBounding;
        private SceneHints _sceneHints;
        private LightCollection _lights;
        private ExtendedPropertiesCollection _extendedProperties;
        private ushort _currentVersion;

        protected Spatial()
        {
        }

        protected Spatial(string name)
        {
            Name = name;
            _parent = null;
            _transform = new Transform();
            _worldTransform = new Transform();
            _dirtyMark = DirtyMark.All;
            _sceneHints = new SceneHints(this);
            _lights = new LightCollection();
            _lights.CollectionModified += LocalLights_CollectionModified;
            _extendedProperties = new ExtendedPropertiesCollection();
        }

        public string Name { get; set; }

        public Node Parent => _parent;

        /// <summary>
        /// Gets or sets the local transform
        /// </summary>
        public Transform Transform
        {
            get => _transform;
            set
            {
                if (value == null)
                {
                    _transform.SetIdentity();
                }

                _transform = value;
                PropagateDirtyDown(DirtyMark.All);
                PropagateDirtyUp(DirtyMark.Bounding);
            }
        }

        /// <summary>
        /// Gets or sets the local scale
        /// </summary>
        public Vector3 Scale
        {
            get => _transform.Scale;
            set
            {
                _transform.SetScale(ref value);
                PropagateDirtyDown(DirtyMark.All);
                PropagateDirtyUp(DirtyMark.Bounding);
            }
        }

        /// <summary>
        /// Gets or sets the local rotation
        /// </summary>
        public Quaternion Rotation
        {
            get => _transform.Rotation;
            set
            {
                _transform.SetRotation(ref value);
                PropagateDirtyDown(DirtyMark.All);
                PropagateDirtyUp(DirtyMark.Bounding);
            }
        }

        /// <summary>
        /// Gets or sets the local translation
        /// </summary>
        public Vector3 Translation
        {
            get => _transform.Translation;
            set
            {
                _transform.SetTranslation(ref value);
                PropagateDirtyDown(DirtyMark.All);
                PropagateDirtyUp(DirtyMark.Bounding);
            }
        }

        /// <summary>
        /// Gets the world transform
        /// </summary>
        public Transform WorldTransform => _worldTransform;

        /// <summary>
        /// Gets the world scale
        /// </summary>
        public Vector3 WorldScale => _worldTransform.Scale;

        /// <summary>
        /// Gets the world rotation
        /// </summary>
        public Quaternion WorldRotation => _worldTransform.Rotation;

        /// <summary>
        /// Gets the world translation
        /// </summary>
        public Vector3 WorldTranslation => _worldTransform.Translation;

        /// <summary>
        /// Gets the world transformation matrix
        /// </summary>
        public Matrix4x4 WorldMatrix => _worldTransform.Matrix;

        public BoundingVolume WorldBounding => _worldBounding;

        public IHintable ParentHintable => _parent;

        public SceneHints SceneHints => _sceneHints;

        public LightCollection Lights => _lights;

        public ExtendedPropertiesCollection ExtendedProperties => _extendedProperties;

        protected ushort CurrentVersion => _currentVersion;

        #region Visitor

        public virtual bool AcceptVisitor(ISpatialVisitor visitor, bool preexecute = true)
        {
            if (visitor != null)
            {
                return visitor.Visit(this);
            }

            return true;
        }

        public virtual bool AcceptVisitor(Func<Spatial, bool> visitor, bool preexecute = true)
        {
            if (visitor != null)
            {
                return visitor(this);
            }

            return true;
        }

        #endregion

        #region Transform

        public void SetScale(float scale)
        {
            _transform.SetScale(scale);
            PropagateDirtyDown(DirtyMark.All);
            PropagateDirtyUp(DirtyMark.Bounding);
        }

        public void SetScale(ref Vector3 scale)
        {
            _transform.SetScale(ref scale);
            PropagateDirtyDown(DirtyMark.All);
            PropagateDirtyUp(DirtyMark.Bounding);
        }

        public void SetTranslation(float x, float y, float z)
        {
            _transform.SetTranslation(x, y, z);
            PropagateDirtyDown(DirtyMark.All);
            PropagateDirtyUp(DirtyMark.Bounding);
        }

        public void SetTranslation(ref Vector3 translation)
        {
            _transform.SetTranslation(ref translation);
            PropagateDirtyDown(DirtyMark.All);
            PropagateDirtyUp(DirtyMark.Bounding);
        }

        public void SetRotation(Matrix4x4 matrix)
        {
            _transform.SetRotation(ref matrix);
            PropagateDirtyDown(DirtyMark.All);
            PropagateDirtyUp(DirtyMark.Bounding);
        }

        public void SetRotation(ref Matrix4x4 matrix)
        {
            _transform.SetRotation(ref matrix);
            PropagateDirtyDown(DirtyMark.All);
            PropagateDirtyUp(DirtyMark.Bounding);
        }

        public void SetRotation(ref Quaternion rotation)
        {
            _transform.SetRotation(ref rotation);
            PropagateDirtyDown(DirtyMark.All);
            PropagateDirtyUp(DirtyMark.Bounding);
        }

        #endregion

        #region Dirty Notification

        public void MarkDirty(DirtyMark flag)
        {
            _dirtyMark |= flag;
        }

        public void ClearDirty(DirtyMark flag)
        {
            _dirtyMark &= ~flag;
        }

        public bool IsDirty(DirtyMark flag)
        {
            return (_dirtyMark & flag) == flag;
        }

        public virtual void PropagateDirtyUp(DirtyMark flag)
        {
            _dirtyMark |= flag;
            if (_parent != null)
            {
                _parent.PropagateDirtyUp(flag);
            }
        }

        public virtual void PropagateDirtyDown(DirtyMark flag)
        {
            _dirtyMark |= flag;
        }

        #endregion

        #region Update

        public void Update(IGameTime time)
        {
            Update(time, true);
        }

        public virtual void Update(IGameTime time, bool initiator)
        {
            if (IsDirty(DirtyMark.Transform))
            {
                UpdateWorldTransform(false);
            }

            if (IsDirty(DirtyMark.Lighting))
            {
                UpdateWorldLights(false);
            }

            if (IsDirty(DirtyMark.Bounding))
            {
                UpdateWorldBound(false);
                if (initiator)
                {
                    PropagateBoundToRoot();
                }
            }
        }

        public virtual void UpdateWorldTransform(bool recurse)
        {
            _worldTransform.Set(_transform);

            if (_parent != null)
            {
                _worldTransform.CombineWithParent(_parent.WorldTransform);
            }

            ClearDirty(DirtyMark.Transform);
        }

        public abstract void UpdateWorldBound(bool recurse);
        public abstract void UpdateWorldLights(bool recurse);

        public void PropagateBoundToRoot()
        {
            if (_parent != null)
            {
                _parent.UpdateWorldBound(false);
                _parent.PropagateBoundToRoot();
            }
        }

        #endregion

        #region Process visible set

        public void ProcessVisibleSet(IRenderer renderer)
        {
            ProcessVisibleSet(renderer, false);
        }

        public virtual void ProcessVisibleSet(IRenderer renderer, bool skipCullCheck)
        {
            // Will be either Always, Never, or Dynamic
            CullHint hint = _sceneHints.CullHint;

            // If always call, immediately return
            if (hint == CullHint.Always)
            {
                return;
            }

            var frustumIntersect = ContainmentType.Inside;

            // Do frustum check - unless if no world bounding, or if we're skipping the check for any reason
            if (!skipCullCheck && hint != CullHint.Never && _worldBounding != null)
            {
                Camera cam = renderer.RenderContext.Camera;
                frustumIntersect = cam.Frustum.Contains(_worldBounding);
            }

            // If we're not outside, then continue drawing
            if (frustumIntersect != ContainmentType.Outside)
            {
                OnProcessVisibleSet(renderer);
            }
        }

        protected virtual void OnProcessVisibleSet(IRenderer renderer)
        {
        }

        #endregion

        #region Misc

        public virtual bool FindPicks(PickQuery query)
        {
            if (query == null)
            {
                return false;
            }

            // If pickable, the query will do a bounding intersection test first so no need to do it here
            var pickable = this as IPickable;
            if (pickable != null)
            {
                return query.AddPick(pickable);
            }

            return false;
        }

        public Spatial GetAncestorNamed(string name)
        {
            return GetAncestorNamed<Spatial>(name);
        }

        public T GetAncestorNamed<T>(string name) where T : Spatial
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (_parent != null)
            {
                if (_parent.Name.Equals(name) && _parent is T)
                {
                    return _parent as T;
                }

                return _parent.GetAncestorNamed<T>(name);
            }

            return null;
        }

        public Spatial GetDescendentNamed(string name)
        {
            return GetDescendentNamed<Spatial>(name);
        }

        public virtual T GetDescendentNamed<T>(string name) where T : Spatial
        {
            return null; // No children
        }

        public bool RemoveFromParent()
        {
            if (_parent != null)
            {
                _parent.Children.Remove(this);
                _parent = null;
                return true;
            }

            return false;
        }

        internal void AttachToParent(Node newParent)
        {
            RemoveFromParent();
            _parent = newParent;
        }

        internal void SetParent(Node newParent)
        {
            _parent = newParent;
        }

        public Spatial Clone(CopyContext copyContext = null)
        {
            var clone = SmartActivator.CreateInstance(GetType()) as Spatial;
            if (clone == null)
            {
                return null;
            }

            PopulateClone(clone, (copyContext == null) ? new CopyContext() : copyContext);

            return clone;
        }

        public void MakeInstanced(MaterialDefinition instanceMatDef, params IInstanceDataProvider[] instanceDataProviders)
        {
            MakeInstanced(instanceMatDef, true, instanceDataProviders);
        }

        public void MakeNonInstanced(MaterialDefinition newMatDef)
        {
            MakeNonInstanced(newMatDef, true);
        }

        public void SetMaterialDefinition(MaterialDefinition newMatDef)
        {
            SetMaterialDefinition(newMatDef, true);
        }

        public void SetModelBounding(BoundingVolume volume)
        {
            SetModelBounding(volume, true);
        }

        public abstract void MakeInstanced(MaterialDefinition instanceMatDef, bool copyMatDef, params IInstanceDataProvider[] instanceDataProviders);
        public abstract void MakeNonInstanced(MaterialDefinition newMatDef, bool copyMatDef);
        public abstract void SetMaterialDefinition(MaterialDefinition newMatDef, bool copyMatDef);
        public abstract void SetModelBounding(BoundingVolume volume, bool calculateBounds);
        public abstract void SortLights();

        protected void CollectLights(LightCollection lights)
        {
            lights.AddRange(_lights);

            if (_parent != null)
            {
                _parent.CollectLights(lights);
            }
        }

        protected void RemoveWorldBounding()
        {
            _worldBounding = null;
        }

        protected void SetWorldBounding(BoundingVolume volume)
        {
            _worldBounding = volume;
        }

        protected virtual void PopulateClone(Spatial clone, CopyContext copyContext)
        {
            clone.Name = Name;
            clone._parent = null;
            clone._transform = new Transform(_transform);
            clone._worldTransform = new Transform(_worldTransform);
            clone._dirtyMark = DirtyMark.None; // Copying EVERYTHING, so there shouldn't be a need to update
            clone._sceneHints = new SceneHints(clone, _sceneHints);
            clone._lights = new LightCollection(_lights);
            clone._lights.CollectionModified += clone.LocalLights_CollectionModified;
        }

        private void LocalLights_CollectionModified(LightCollection sender, EventArgs args)
        {
            PropagateDirtyDown(DirtyMark.Lighting);
        }

        #endregion

        public virtual void Read(ISavableReader input)
        {
            _currentVersion = input.ReadUInt16();

            Name = input.ReadString();
            _transform = input.ReadSavable<Transform>();
            _sceneHints = input.ReadSavable<SceneHints>();
            _sceneHints.SetSource(this);
            _lights = input.ReadSavable<LightCollection>();
            _lights.CollectionModified += LocalLights_CollectionModified;

            _worldTransform = new Transform(_transform);
            _parent = null;
            _dirtyMark = DirtyMark.All;

            ReadExtendedProperties(input);
        }

        public virtual void Write(ISavableWriter output)
        {
            output.Write("Version", _currentVersion);
            output.Write("Name", Name);
            output.WriteSavable("LocalTransform", _transform);
            output.WriteSavable("SceneHints", _sceneHints);
            output.WriteSavable("LocalLights", _lights);

            WriteExtendedProperties(output);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return $"{GetType().Name}: {Name}";
            }

            return base.ToString();
        }

        private void ReadExtendedProperties(ISavableReader input)
        {
            int extendedPropCount = input.BeginReadGroup();
            _extendedProperties = new ExtendedPropertiesCollection(extendedPropCount);

            for (int i = 0; i < extendedPropCount; i++)
            {
                input.BeginReadGroup();

                int key = input.ReadInt32();
                ISavable prop = input.ReadSavable<ISavable>();
                _extendedProperties.Add(key, prop);

                input.EndReadGroup();
            }

            input.EndReadGroup();
        }

        private void WriteExtendedProperties(ISavableWriter output)
        {
            int extendedSavablePropCount = _extendedProperties.GetCountOf<ISavable>();

            output.BeginWriteGroup("ExtendedProperties", extendedSavablePropCount);

            foreach (KeyValuePair<int, Object> kv in _extendedProperties)
            {
                if (kv.Value is ISavable)
                {
                    output.BeginWriteGroup("Property");

                    output.Write("Key", kv.Key);
                    output.WriteSavable("Value", kv.Value as ISavable);

                    output.EndWriteGroup();
                }
            }

            output.EndWriteGroup();
        }
    }
}
