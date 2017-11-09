namespace Spark.UI
{
    using System;

    [Flags]
    public enum FrameworkPropertyMetadataOptions
    {
        None = 0x000,
        AffectsMeasure = 0x001,
        AffectsArrange = 0x002,
        AffectsParentMeasure = 0x004,
        AffectsParentArrange = 0x008,
        AffectsRender = 0x010,
        Inherits = 0x020,
        OverridesInheritanceBehavior = 0x040,
        NotDataBindable = 0x080,
        BindsTwoWayByDefault = 0x100,
        Journal = 0x400,
        SubPropertiesDoNotAffectRender = 0x800,
    }
}
