namespace Spark.UI
{
    using System;
    using System.Windows.Markup;

    using Data;

    public class TemplateBindingExtension : MarkupExtension
    {
        private string _path;

        public TemplateBindingExtension(string path)
        {
            _path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding(_path)
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent)
            };
        }
    }
}
