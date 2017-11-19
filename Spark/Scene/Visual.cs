namespace Spark.Scene
{
    using System;

    using Math;
    using Graphics;
    using Content;

    public abstract class Visual : Spatial, IRenderable
    {
        private MaterialDefinition _matDef;
        private BoundingVolume _modelBounding;
        private LightCollection _worldLights;
        private RenderPropertyCollection _renderProperties;
        private LightComparer _lightComparer;
        
        protected Visual()
        {
        }

        protected Visual(String name)
            : base(name)
        {
            // Monitor light changes, because while the visual may not need to be updated, we will still want to update the shader with new light info
            // when we render the visual
            _worldLights = new LightCollection();
            _worldLights.MonitorLightChanges = true;
            _renderProperties = new RenderPropertyCollection();
            SetDefaultRenderProperties(false);
        }

        public MaterialDefinition MaterialDefinition
        {
            get => _matDef;
            set
            {
                _matDef = value;

                if (value != null)
                {
                    _worldLights.UpdateShader = true;
                }
            }
        }

        public bool IsMaterialValid => _matDef != null && _matDef.AreMaterialsValid();

        public BoundingVolume ModelBounding
        {
            get => _modelBounding;
            set => SetModelBounding(value);
        }

        public LightCollection WorldLights => _worldLights;

        public RenderPropertyCollection RenderProperties => _renderProperties;

        public bool IsPickable
        {
            get => SceneHints.IsPickable;
            set
            {
                SceneHints hints = SceneHints;
                PickingHint hint = hints.PickingHint;

                if (value)
                {
                    switch (hint)
                    {
                        case PickingHint.Collidable:
                            hints.PickingHint = PickingHint.PickableAndCollidable;
                            break;
                        case PickingHint.None:
                        case PickingHint.Inherit:
                            hints.PickingHint = PickingHint.Pickable;
                            break;
                    }
                }
                else
                {
                    switch (hint)
                    {
                        case PickingHint.PickableAndCollidable:
                            hints.PickingHint = PickingHint.Collidable;
                            break;
                        case PickingHint.Pickable:
                        case PickingHint.Inherit:
                            hints.PickingHint = PickingHint.None;
                            break;
                    }
                }
            }
        }

        public abstract bool IsValidForDraw { get; }

        public abstract void UpdateModelBounding();
        public abstract void SetupDrawCall(IRenderContext renderContext, RenderBucketId currentBucketId, MaterialPass currentPass);

        public override void SetMaterialDefinition(MaterialDefinition newMatDef, bool copyMatDef)
        {
            _matDef = (copyMatDef) ? newMatDef.Clone() : newMatDef;
            _worldLights.UpdateShader = true;
        }

        public override void SetModelBounding(BoundingVolume volume, bool calculateBounds)
        {
            _modelBounding = volume;

            if (volume != null)
            {
                SetWorldBounding(volume.Clone());
            }
            else
            {
                RemoveWorldBounding();
            }

            // Decide if we use the bounding volume as is, or are using it as a template but
            // still want to calculate based on the mesh
            if (calculateBounds)
            {
                UpdateModelBounding();
            }
            else
            {
                // Even without calculation, the model bounding has changed
                PropagateDirtyUp(DirtyMark.Bounding);
            }
        }

        public override void UpdateWorldBound(bool recurse)
        {
            if (_modelBounding != null)
            {
                // Set and transform from local bound - guaranted to be the same type when set model bound.
                WorldBounding.Set(_modelBounding);
                WorldBounding.Transform(WorldTransform);
            }
            else
            {
                RemoveWorldBounding();
            }

            ClearDirty(DirtyMark.Bounding);
        }

        public override void UpdateWorldLights(bool recurse)
        {
            _worldLights.Clear();

            LightCollection localLights = Lights;
            _worldLights.GlobalAmbient = localLights.GlobalAmbient;
            _worldLights.IsEnabled = localLights.IsEnabled;

            if (!_worldLights.IsEnabled)
            {
                return;
            }

            switch (SceneHints.LightCombineHint)
            {
                case LightCombineHint.Local:
                    _worldLights.AddRange(localLights);
                    SortLights();
                    break;
                case LightCombineHint.CombineClosest:
                    CollectLights(_worldLights);
                    SortLights();
                    break;
            }

            ClearDirty(DirtyMark.Lighting);
        }

        public override void SortLights()
        {
            if (_lightComparer == null)
            {
                _lightComparer = new LightComparer(this);
            }

            _worldLights.Sort(_lightComparer);
        }

        public override void Read(ISavableReader input)
        {
            base.Read(input);

            _matDef = input.ReadExternalSavable<MaterialDefinition>();
            _modelBounding = input.ReadSavable<BoundingVolume>();

            // If have a model bounding, need to copy type as world bounding
            if (_modelBounding != null)
            {
                SetWorldBounding(_modelBounding.Clone());
            }

            _renderProperties = input.ReadSavable<RenderPropertyCollection>();

            // The default render properties are property accessors, and as such are not serialized
            _worldLights = new LightCollection();
            _worldLights.MonitorLightChanges = true;
            SetDefaultRenderProperties(true);
        }

        public override void Write(ISavableWriter output)
        {
            base.Write(output);

            output.WriteExternalSavable("MaterialDefinition", _matDef);
            output.WriteSavable("ModelBounding", _modelBounding);
            output.WriteSavable("RenderProperties", _renderProperties);
        }

        protected virtual void SetDefaultRenderProperties(bool fromSavable)
        {
            _renderProperties.Add(new WorldTransformProperty(WorldTransform));
            _renderProperties.Add(new LightCollectionProperty(WorldLights));

            _renderProperties.Add(new WorldBoundingVolumeProperty(delegate ()
            {
                return WorldBounding;
            }));

            // Ortho order will be serialized
            if (!fromSavable)
            {
                _renderProperties.Add(new OrthoOrderProperty(0));
            }
        }

        protected override void PopulateClone(Spatial clone, CopyContext copyContext)
        {
            base.PopulateClone(clone, copyContext);

            var vs = clone as Visual;
            if (vs == null)
            {
                return;
            }

            vs._worldLights = new LightCollection(_worldLights);
            vs._worldLights.MonitorLightChanges = true;
            vs._renderProperties = _renderProperties.Clone();

            if (_lightComparer != null)
            {
                vs._lightComparer = new LightComparer(vs);
            }

            if (copyContext[CopyContext.CopyMaterialKey])
            {
                if (_matDef != null)
                {
                    vs._matDef = _matDef.Clone();
                }
            }
            else
            {
                vs._matDef = _matDef;
            }

            if (_modelBounding != null)
            {
                vs._modelBounding = _modelBounding.Clone();
                vs.SetWorldBounding(WorldBounding.Clone());
            }

            vs.SetDefaultRenderProperties(true);
        }
    }
}
