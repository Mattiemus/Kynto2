namespace Spark.UI.Media
{
    using System;
    using System.Collections.Generic;

    using Utilities;
    using Graphics;
    using Math;

    public class StreamGeometryContext : Disposable
    {
        private readonly StreamGeometry _geometry;
        private readonly List<Vector2> _points;

        internal StreamGeometryContext(StreamGeometry geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            _geometry = geometry;
            _points = new List<Vector2>();
        }

        public void BeginFigure(Vector2 startPoint, bool isFilled, bool isClosed)
        {
            _points.Add(startPoint);
        }

        public void BezierTo(Vector2 point1, Vector2 point2, Vector2 point3, bool isStroked, bool isSmoothJoin)
        {
            throw new NotImplementedException();
        }

        public void LineTo(Vector2 point, bool isStroked, bool isSmoothJoin)
        {
            _points.Add(point);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                BuildGeometry();
            }

            base.Dispose(isDisposing);
        }

        private void BuildGeometry()
        {
            Triangulator2D triangulator = new Triangulator2D();

            int[] sourceIndices;
            Vector2[] sourceVertices;

            triangulator.Triangulate(
                _points.ToArray(),
                VertexWinding.CounterClockwise,
                out sourceVertices,
                out sourceIndices);

            VertexPositionColor[] vertices = new VertexPositionColor[sourceVertices.Length];
            for (int i = 0; i < sourceVertices.Length; i++)
            {
                vertices[i] = new VertexPositionColor(new Vector3(sourceVertices[i], 0.0f), Color.AliceBlue);
            }
            
            _geometry.SetGeometry(
                new DataBuffer<VertexPositionColor>(vertices),
                new DataBuffer<int>(sourceIndices));
        }
    }
}
