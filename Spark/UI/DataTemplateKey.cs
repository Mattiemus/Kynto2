namespace Spark.UI
{
    public class DataTemplateKey : TemplateKey
    {
        public DataTemplateKey()
            : base(TemplateType.DataTemplate)
        {
        }

        public DataTemplateKey(object dataType)
            : base(TemplateType.DataTemplate, dataType)
        {
        }
    }
}
