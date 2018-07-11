namespace Spark.UI
{
    using System;
    using System.Reflection;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(ResourceKey))]
    public abstract class ResourceKey : MarkupExtension
    {
        protected ResourceKey()
        {
        }

        public abstract Assembly Assembly { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
