namespace Spark.Scene
{
    using System;

    using Content;

    /// <summary>
    /// Represents a collection of hints that are used during rendering of an object in the scenegraph.
    /// </summary>
    public sealed class SceneHints : ISavable
    {
        private IHintable _source;
        private CullHint _cullHint;
        private PickingHint _pickHint;
        private LightCombineHint _lightHint;
        private BoundingCombineHint _boundingHint;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneHints"/> class.
        /// </summary>
        private SceneHints()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneHints"/> class.
        /// </summary>
        /// <param name="source">Source hintable</param>
        public SceneHints(IHintable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _source = source;
            _cullHint = CullHint.Inherit;
            _lightHint = LightCombineHint.Inherit;
            _boundingHint = BoundingCombineHint.Inherit;
            _pickHint = PickingHint.Inherit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneHints"/> class.
        /// </summary>
        /// <param name="source">Source hintable</param>
        /// <param name="toCopyFrom">Scene hints to copy data from.</param>
        public SceneHints(IHintable source, SceneHints toCopyFrom)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _source = source;
            Set(toCopyFrom);
        }

        /// <summary>
        /// Gets or sets the cull hint. If the local hint is set to inherit, then this is influenced by the parent hintable.
        /// </summary>
        public CullHint CullHint
        {
            get
            {
                // If not inherit, return local
                if (_cullHint != CullHint.Inherit)
                {
                    return _cullHint;
                }

                // If inherit, return parent's
                IHintable parent = _source.ParentHintable;
                if (parent != null)
                {
                    return parent.SceneHints.CullHint;
                }

                // If inherit with no parent, return default
                return CullHint.Dynamic;
            }
            set
            {
                _cullHint = value;
            }
        }

        /// <summary>
        /// Gets or sets the light hint. If the local hint is set to inherit, then this is influenced by the parent hintable.
        /// </summary>
        public LightCombineHint LightCombineHint
        {
            get
            {
                // If not inherit, return local
                if (_lightHint != LightCombineHint.Inherit)
                {
                    return _lightHint;
                }

                // If inherit, return parent's
                IHintable parent = _source.ParentHintable;
                if (parent != null)
                {
                    return parent.SceneHints.LightCombineHint;
                }

                // If inherit with no parent, return default
                return LightCombineHint.CombineClosest;
            }
            set
            {
                _lightHint = value;
            }
        }

        /// <summary>
        /// Gets or sets the bounding hint. If the local hint is set to inherit, then this is influenced by the parent hintable.
        /// </summary>
        public BoundingCombineHint BoundingCombineHint
        {
            get
            {
                if (_boundingHint != BoundingCombineHint.Inherit)
                {
                    return _boundingHint;
                }

                // If inherit, return parent's
                IHintable parent = _source.ParentHintable;
                if (parent != null)
                {
                    return parent.SceneHints.BoundingCombineHint;
                }

                // If inherit with no parent, return default
                return BoundingCombineHint.AxisAlignedBoundingBox;
            }
            set
            {
                _boundingHint = value;
            }
        }

        /// <summary>
        /// Gets or sets the picking hint. If the local is set to inherit, then this is influenced by the parent hintable.
        /// </summary>
        public PickingHint PickingHint
        {
            get
            {
                // If not inherit, return local
                if (_pickHint != PickingHint.Inherit)
                {
                    return _pickHint;
                }

                // If inherit, return parent's
                IHintable parent = _source.ParentHintable;
                if (parent != null)
                {
                    return parent.SceneHints.PickingHint;
                }

                // If inherit with no parent, return default
                return PickingHint.PickableAndCollidable;
            }
            set
            {
                _pickHint = value;
            }
        }

        /// <summary>
        /// Gets if the object can be used in picking queries.
        /// </summary>
        public bool IsPickable => PickingHint == PickingHint.Pickable || PickingHint == PickingHint.PickableAndCollidable;

        /// <summary>
        /// Gets if the object can be used in collision queries.
        /// </summary>
        public bool IsCollidable => PickingHint == PickingHint.Collidable || PickingHint == PickingHint.PickableAndCollidable;

        /// <summary>
        /// Sets the hintable source.
        /// </summary>
        /// <param name="src">Source hintable.</param>
        public void SetSource(IHintable src)
        {
            _source = src;
        }

        /// <summary>
        /// Copies the scene hints from another instance.
        /// </summary>
        /// <param name="toCopyFrom">Scene hints to copy data from.</param>
        public void Set(SceneHints toCopyFrom)
        {
            if (toCopyFrom == null)
            {
                throw new ArgumentNullException(nameof(toCopyFrom));
            }

            _cullHint = toCopyFrom._cullHint;
            _lightHint = toCopyFrom._lightHint;
            _boundingHint = toCopyFrom._boundingHint;
            _pickHint = toCopyFrom._pickHint;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            _cullHint = input.ReadEnum<CullHint>();
            _pickHint = input.ReadEnum<PickingHint>();
            _lightHint = input.ReadEnum<LightCombineHint>();
            _boundingHint = input.ReadEnum<BoundingCombineHint>();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.WriteEnum("CullHint", _cullHint);
            output.WriteEnum("PickingHint", _pickHint);
            output.WriteEnum("LightCombineHint", _lightHint);
            output.WriteEnum("BoundingCombineHint", _boundingHint);
        }
    }
}
