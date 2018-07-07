namespace Spark.UI
{
    public class FrameworkPropertyMetadata : PropertyMetadata
    {
        public FrameworkPropertyMetadata()
        {
        }

        public FrameworkPropertyMetadata(object defaultValue) 
            : base(defaultValue)
        {
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions options) 
            : base(defaultValue)
        {
            SetOptions(options);
        }

        public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback callback, FrameworkPropertyMetadataOptions options) 
            : base(defaultValue, callback)
        {
            SetOptions(options);
        }

        public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback callback, FrameworkPropertyMetadataOptions options, UpdateSourceTrigger defaultUpdateSourceTrigger) 
            : this(defaultValue, callback, options)
        {
            DefaultUpdateSourceTrigger = defaultUpdateSourceTrigger;
        }

        public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback callback) 
            : base(defaultValue, callback)
        {
        }

        public FrameworkPropertyMetadata(PropertyChangedCallback callback) 
            : base(callback)
        {
        }

        /// <summary>
        /// The default UpdateSourceTrigger for two-way data bindings on this property.
        /// </summary>
        public UpdateSourceTrigger DefaultUpdateSourceTrigger { get; set; }

        /// <summary>
        /// Property is inheritable
        /// </summary>
		public bool Inherits { get; set; }

        /// <summary>
        /// Property affects measurement
        /// </summary>
        public bool AffectsMeasure { get; set; }

        /// <summary>
        /// Property affects arragement
        /// </summary>
        public bool AffectsArrange { get; set; }

        /// <summary>
        /// Property affects parent's measurement
        /// </summary>
        public bool AffectsParentMeasure { get; set; }

        /// <summary>
        ///     Property affects parent's arrangement
        /// </summary>
        public bool AffectsParentArrange { get; set; }

        /// <summary>
        /// Property affects rendering
        /// </summary>
        public bool AffectsRender { get; set; }

        /// <summary>
        /// Property evaluation must span separated trees
        /// </summary>
        public bool OverridesInheritanceBehavior { get; set; }
        
        /// <summary>
        /// Property cannot be data-bound
        /// </summary>
        public bool IsNotDataBindable { get; set; }

        /// <summary>
        /// Data bindings on this property default to two-way
        /// </summary>
        public bool BindsTwoWayByDefault { get; set; }

        /// <summary>
        /// The value of this property should be saved/restored when journaling by URI
        /// </summary>
        public bool Journal { get; set; }

        /// <summary>
        /// This property's subproperties do not affect rendering.
        /// For instance, a property X may have a subproperty Y.
        /// Changing X.Y does not require rendering to be updated.
        /// </summary>
        public bool SubPropertiesDoNotAffectRender { get; set; }
        
        private void SetOptions(FrameworkPropertyMetadataOptions options)
        {
            AffectsArrange = options.HasFlag(FrameworkPropertyMetadataOptions.AffectsArrange);
            AffectsMeasure = options.HasFlag(FrameworkPropertyMetadataOptions.AffectsMeasure);
            AffectsParentArrange = options.HasFlag(FrameworkPropertyMetadataOptions.AffectsParentArrange);
            AffectsParentMeasure = options.HasFlag(FrameworkPropertyMetadataOptions.AffectsParentMeasure);
            AffectsRender = options.HasFlag(FrameworkPropertyMetadataOptions.AffectsRender);
            BindsTwoWayByDefault = options.HasFlag(FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);
            Inherits = options.HasFlag(FrameworkPropertyMetadataOptions.Inherits);
            Journal = options.HasFlag(FrameworkPropertyMetadataOptions.Journal);
            IsNotDataBindable = options.HasFlag(FrameworkPropertyMetadataOptions.NotDataBindable);
            OverridesInheritanceBehavior = options.HasFlag(FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior);
            SubPropertiesDoNotAffectRender = options.HasFlag(FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender);
        }
    }
}
