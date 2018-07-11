namespace Spark.UI
{
    using System.Windows.Markup;

    [DictionaryKeyProperty(nameof(DataTemplateKey))]
    public class DataTemplate : FrameworkTemplate
    {
        public DataTemplate()
        {
            Triggers = new TriggerCollection();
        }

        public DataTemplate(object dataType)
            : this()
        {
            DataType = dataType;
        }

        public object DataTemplateKey
        {
            get { return (DataType != null) ? new DataTemplateKey(DataType) : null; }
        }

        [Ambient]
        public object DataType { get; set; }

        public TriggerCollection Triggers { get; private set; }
    }
}
