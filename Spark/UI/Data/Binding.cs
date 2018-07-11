namespace Spark.UI.Data
{
    using System;

    public class Binding : BindingBase
    {
        public Binding()
        {
        }

        public Binding(string path)
        {
            Path = new PropertyPath(path);
        }

        public PropertyPath Path { get; set; }

        public RelativeSource RelativeSource { get; set; }

        public object Source { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
