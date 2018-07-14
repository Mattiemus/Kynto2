namespace Spark.UI.Media
{
    using System.Linq;
    using System.ComponentModel;

    using Graphics;
    using Animation;
    using Math;

    [TypeConverter(typeof(GeometryConverter))]
    public abstract class Geometry : Animatable
    {
        public static float StandardFlatteningTolerance => 0.25f;

        public RectangleF Bounds { get; private set; }

        internal IReadOnlyDataBuffer<VertexPositionColor> VertexData { get; private set; }

        internal IndexData IndexData { get; private set; }

        public RectangleF GetRenderBounds(Pen pen)
        {
            return GetRenderBounds(pen, StandardFlatteningTolerance, ToleranceType.Absolute);
        }

        public RectangleF GetRenderBounds(Pen pen, double tolerance, ToleranceType type)
        {
            return Bounds;
        }

        public static Geometry Parse(string source)
        {
            StreamGeometry result = new StreamGeometry();

            using (StreamGeometryContext ctx = result.Open())
            {
                PathMarkupParser parser = new PathMarkupParser(result, ctx);
                parser.Parse(source);
                return result;
            }
        }

        internal void SetGeometry(IReadOnlyDataBuffer<VertexPositionColor> vertexData, IndexData indices)
        {
            VertexData = vertexData;

            IndexData = indices;
            
            Bounds = new RectangleF(
                vertexData.Min(v => v.Position.X),
                vertexData.Min(v => v.Position.Y),
                vertexData.Min(v => v.Position.X) + vertexData.Max(v => v.Position.Y),
                vertexData.Min(v => v.Position.Y) + vertexData.Max(v => v.Position.Y));
        }
    }
}
