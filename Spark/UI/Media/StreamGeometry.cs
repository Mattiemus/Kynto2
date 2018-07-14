namespace Spark.UI.Media
{
    public sealed class StreamGeometry : Geometry
    {        
        public StreamGeometryContext Open()
        {
            return new StreamGeometryContext(this);
        }
    }
}
