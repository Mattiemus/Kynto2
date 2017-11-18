namespace Spark.Scene
{
    using System;

    using Core;
    using Math;
    using Content;
    using Graphics;
    using Graphics.Renderer;
    using Graphics.Materials;

    public class Node : Spatial
    {
        private NodeChildrenCollection _children;
        
        protected Node()
        {
        }

        public Node(string name)
            : base(name)
        {
            _children = new NodeChildrenCollection(this);
        }

        public Node(string name, int initialChildCapacity)
            : base(name)
        {
            _children = new NodeChildrenCollection(this, initialChildCapacity);
        }
        
        public NodeChildrenCollection Children => _children;

        public override bool FindPicks(PickQuery query)
        {
            if (query == null)
            {
                return false;
            }

            BoundingVolume bv = WorldBounding;
            query.GetPickRay(out Ray pickRay);
            if (bv != null && !bv.Intersects(ref pickRay))
            {
                return false;
            }

            bool haveOne = false;
            for (int i = 0; i < _children.Count; i++)
            {
                Spatial child = _children[i];
                haveOne |= child.FindPicks(query);
            }

            return haveOne;
        }

        public override bool AcceptVisitor(ISpatialVisitor visitor, bool preExecute)
        {
            // Determine if we want to keep visiting when we visit *THIS* node. If we do, then
            // visit all children
            bool keepVisiting = true;

            if (preExecute)
            {
                keepVisiting = visitor.Visit(this);
            }

            if (keepVisiting)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].AcceptVisitor(visitor, preExecute);
                }
            }

            if (!preExecute)
            {
                keepVisiting = visitor.Visit(this);
            }

            return keepVisiting;
        }

        public override bool AcceptVisitor(Func<Spatial, bool> visitor, bool preexecute = true)
        {
            // Determine if we want to keep visiting when we visit *THIS* node. If we do, then
            // visit all children
            bool keepVisiting = true;

            if (preexecute)
            {
                keepVisiting = visitor(this);
            }

            if (keepVisiting)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].AcceptVisitor(visitor, preexecute);
                }
            }

            if (!preexecute)
            {
                keepVisiting = visitor(this);
            }

            return keepVisiting;
        }

        public override T GetDescendentNamed<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            T child = _children[name] as T;

            if (child == null)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    child = _children[i].GetDescendentNamed<T>(name);

                    if (child != null)
                    {
                        break;
                    }
                }
            }

            return child;
        }

        public override void MakeInstanced(MaterialDefinition instanceMatDef, bool copyMatDef, params IInstanceDataProvider[] instanceDataProviders)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                Spatial child = _children[i];
                child.MakeInstanced(instanceMatDef, copyMatDef, instanceDataProviders);
            }
        }

        public override void MakeNonInstanced(MaterialDefinition newMatDef, bool copyMatDef)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                Spatial child = _children[i];
                child.MakeNonInstanced(newMatDef, copyMatDef);
            }
        }

        public override void SetMaterialDefinition(MaterialDefinition newMatDef, bool copyMatDef)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                Spatial child = _children[i];
                child.SetMaterialDefinition(newMatDef, copyMatDef);
            }
        }

        public override void PropagateDirtyDown(DirtyMark flag)
        {
            MarkDirty(flag);

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].PropagateDirtyDown(flag);
            }
        }

        public override void ProcessVisibleSet(IRenderer renderer, bool skipCullCheck)
        {
            // Will be either Always, Never, or Dynamic
            CullHint hint = SceneHints.CullHint;

            // If always call, immediately return
            if (hint == CullHint.Always)
            {
                return;
            }

            ContainmentType frustumIntersect = ContainmentType.Inside;
            BoundingVolume worldBounding = WorldBounding;

            // Do frustum check - unless if no world bounding, or if we're skipping the check for any reason
            if (!skipCullCheck && hint != CullHint.Never && worldBounding != null)
            {
                Camera cam = renderer.RenderContext.Camera;
                frustumIntersect = cam.Frustum.Contains(worldBounding);
            }

            // If we're not outside, then continue drawing
            if (frustumIntersect != ContainmentType.Outside)
            {
                bool skipCullForChildren = frustumIntersect == ContainmentType.Inside;

                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].ProcessVisibleSet(renderer, skipCullForChildren);
                }
            }
        }

        public override void Update(IGameTime time, bool initiator)
        {
            if (IsDirty(DirtyMark.Transform))
            {
                UpdateWorldTransform(false);
            }

            if (IsDirty(DirtyMark.Lighting))
            {
                UpdateWorldLights(false);
            }

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Update(time, false);
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

        public override void UpdateWorldBound(bool recurse)
        {
            // If no children, make sure the world bounding gets set to null
            if (_children.Count == 0)
            {
                RemoveWorldBounding();
                return;
            }

            BoundingVolume parentBV = WorldBounding;
            bool firstChild = true;

            // Loop over each child, and merge their world bounds with our nodes
            for (int i = 0; i < _children.Count; i++)
            {
                Spatial child = _children[i];

                if (recurse)
                {
                    child.UpdateWorldBound(true);
                }

                BoundingVolume childBV = child.WorldBounding;

                if (childBV != null)
                {
                    // If at first child, either clone or set from their BoundingVolume
                    if (firstChild)
                    {
                        BoundingCombineHint combineHint = SceneHints.BoundingCombineHint;
                        if (!IsBoundingVolumeCompatible(parentBV, childBV, combineHint))
                        {
                            parentBV = null;
                        }

                        if (parentBV == null)
                        {
                            switch (combineHint)
                            {
                                case BoundingCombineHint.Sphere:
                                    parentBV = new BoundingSphere();
                                    parentBV.Set(childBV);
                                    break;
                                case BoundingCombineHint.AxisAlignedBoundingBox:
                                    parentBV = new BoundingBox();
                                    parentBV.Set(childBV);
                                    break;
                                case BoundingCombineHint.Capsule:
                                    parentBV = new BoundingCapsule();
                                    parentBV.Set(childBV);
                                    break;
                                case BoundingCombineHint.OrientedBoundingBox:
                                    parentBV = new OrientedBoundingBox();
                                    parentBV.Set(childBV);
                                    break;
                                default:
                                    parentBV = childBV.Clone();
                                    break;
                            }

                            SetWorldBounding(parentBV);
                        }
                        else
                        {
                            parentBV.Set(childBV);
                        }

                        firstChild = false;
                    }
                    // Else merge the parent with the child's BoundingVolume
                    else
                    {
                        parentBV.Merge(childBV);
                    }
                }
            }

            ClearDirty(DirtyMark.Bounding);
        }

        public override void UpdateWorldTransform(bool recurse)
        {
            base.UpdateWorldTransform(recurse);

            if (recurse)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].UpdateWorldTransform(true);
                }
            }
        }

        public override void UpdateWorldLights(bool recurse)
        {
            if (recurse)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].UpdateWorldLights(true);
                }
            }

            ClearDirty(DirtyMark.Lighting);
        }

        public override void SetModelBounding(BoundingVolume volume, bool calculateBounds)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                Spatial child = _children[i];

                // Each leaf node (mesh) gets its own unique instance
                if (child is IRenderable)
                {
                    child.SetModelBounding(volume.Clone(), calculateBounds);
                }
                else
                {
                    child.SetModelBounding(volume, calculateBounds);
                }
            }
        }

        public override void SortLights()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].SortLights();
            }
        }

        public override void Read(ISavableReader input)
        {
            base.Read(input);

            Spatial[] spatials = input.ReadSavableArray<Spatial>();
            _children = new NodeChildrenCollection(this, spatials.Length);
            _children.AddRange(spatials);
        }

        public override void Write(ISavableWriter output)
        {
            base.Write(output);

            output.WriteSavable("Children", _children.ToArray());
        }

        protected override void PopulateClone(Spatial clone, CopyContext copyContext)
        {
            base.PopulateClone(clone, copyContext);

            Node node = clone as Node;
            if (node == null)
            {
                return;
            }

            node._children = new NodeChildrenCollection(node, _children.Count);
            node._children.SuspendPropogateDirty = true;

            try
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    Spatial childClone = _children[i].Clone(copyContext);
                    node._children.Add(childClone);
                }
            }
            finally
            {
                node._children.SuspendPropogateDirty = false;
            }
        }

        private bool IsBoundingVolumeCompatible(BoundingVolume volume, BoundingVolume firstChildBoundingVolume, BoundingCombineHint hint)
        {
            if (volume == null)
            {
                return false;
            }

            switch (hint)
            {
                case BoundingCombineHint.CloneFromChildren:
                    return volume.BoundingType == firstChildBoundingVolume.BoundingType;
                case BoundingCombineHint.Sphere:
                    return volume is BoundingSphere;
                case BoundingCombineHint.AxisAlignedBoundingBox:
                    return volume is BoundingBox;
                case BoundingCombineHint.Capsule:
                    return volume is BoundingCapsule;
                case BoundingCombineHint.OrientedBoundingBox:
                    return volume is OrientedBoundingBox;
            }

            return false;
        }
    }
}
