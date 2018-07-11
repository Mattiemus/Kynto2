namespace Spark.UI
{
    using Data;

    public class FrameworkPropertyMetadata : UIPropertyMetadata
    {
        public FrameworkPropertyMetadata()
        {
        }

        public FrameworkPropertyMetadata(object defaultValue)
            : base(defaultValue)
        {
        }

        public FrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
            : base(propertyChangedCallback)
        {
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags)
            : base(defaultValue)
        {
            LoadFlags(flags);
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback)
        {
        }

        public FrameworkPropertyMetadata(
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(propertyChangedCallback)
        {
            CoerceValueCallback = coerceValueCallback;
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback)
        {
            LoadFlags(flags);
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
            LoadFlags(flags);
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback,
            bool isAnimationProhibited)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback, isAnimationProhibited)
        {
            LoadFlags(flags);
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback,
            bool isAnimationProhibited,
            UpdateSourceTrigger defaultUpdateSourceTrigger)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback, isAnimationProhibited)
        {
            DefaultUpdateSourceTrigger = defaultUpdateSourceTrigger;
            LoadFlags(flags);
        }

        public bool AffectsArrange { get; set; }

        public bool AffectsMeasure { get; set; }

        public bool AffectsParentArrange { get; set; }

        public bool AffectsParentMeasure { get; set; }

        public bool AffectsRender { get; set; }

        public bool BindsTwoWayByDefault { get; set; }

        public UpdateSourceTrigger DefaultUpdateSourceTrigger { get; set; }

        public bool Inherits { get; set; }

        public bool IsNotDataBindable { get; set; }

        public bool Journal { get; set; }

        public bool OverridesInheritanceBehavior { get; set; }

        public bool SubPropertiesDoNotAffectRender { get; set; }

        private void LoadFlags(FrameworkPropertyMetadataOptions flags)
        {
            AffectsArrange = (flags & FrameworkPropertyMetadataOptions.AffectsArrange) != 0;
            AffectsMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsMeasure) != 0;
            AffectsParentArrange = (flags & FrameworkPropertyMetadataOptions.AffectsParentArrange) != 0;
            AffectsParentMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsParentMeasure) != 0;
            AffectsRender = (flags & FrameworkPropertyMetadataOptions.AffectsRender) != 0;
            BindsTwoWayByDefault = (flags & FrameworkPropertyMetadataOptions.BindsTwoWayByDefault) != 0;
            Inherits = (flags & FrameworkPropertyMetadataOptions.Inherits) != 0;
            IsNotDataBindable = (flags & FrameworkPropertyMetadataOptions.NotDataBindable) != 0;
            Journal = (flags & FrameworkPropertyMetadataOptions.Journal) != 0;
            OverridesInheritanceBehavior = (flags & FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior) != 0;
            SubPropertiesDoNotAffectRender = (flags & FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender) != 0;
        }
    }
}
