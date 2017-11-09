namespace Spark.UI
{
    using Data;

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

        public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback callback)
            : base(defaultValue, callback)
        {
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

        public FrameworkPropertyMetadata(PropertyChangedCallback callback)
            : base(callback)
        {
        }

        public UpdateSourceTrigger DefaultUpdateSourceTrigger { get; set; }
        
		public bool Inherits { get; set; }
        
        public bool AffectsMeasure { get; set; }
        
        public bool AffectsArrange { get; set; }
        
        public bool AffectsParentMeasure { get; set; }
        
        public bool AffectsParentArrange { get; set; }
        
        public bool AffectsRender { get; set; }
        
        public bool OverridesInheritanceBehavior { get; set; }
        
        public bool IsNotDataBindable { get; set; }
        
        public bool BindsTwoWayByDefault { get; set; }
        
        public bool Journal { get; set; }
        
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
