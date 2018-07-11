namespace Spark.UI.Data
{
    public class RelativeSource
    {
        public RelativeSource()
        {
        }

        public RelativeSource(RelativeSourceMode mode)
        {
            Mode = mode;
        }

        public RelativeSourceMode Mode { get; set; }
    }
}
