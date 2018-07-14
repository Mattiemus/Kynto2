namespace Spark.UI.Media
{
    using System.ComponentModel;

    using Animation;

    [TypeConverter(typeof(BrushConverter))]
    public abstract class Brush : Animatable
    {
    }
}
