namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;

    using Math;

    public sealed class Triangulator2D
    {
        private readonly IndexableCyclicalLinkedList<Vertex> _polygonVertices = new IndexableCyclicalLinkedList<Vertex>();
        private readonly IndexableCyclicalLinkedList<Vertex> _earVertices = new IndexableCyclicalLinkedList<Vertex>();
        private readonly CyclicalList<Vertex> _convexVertices = new CyclicalList<Vertex>();
        private readonly CyclicalList<Vertex> _reflexVertices = new CyclicalList<Vertex>();

        /// <summary>
        /// Triangulates a 2D polygon produced the indexes required to render the points as a triangle list.
        /// </summary>
        /// <param name="inputVertices">The polygon vertices in counter-clockwise winding order.</param>
        /// <param name="desiredWindingOrder">The desired output winding order.</param>
        /// <param name="outputVertices">The resulting vertices that include any reversals of winding order and holes.</param>
        /// <param name="indices">The resulting indices for rendering the shape as a triangle list.</param>
        public void Triangulate(
            Vector2[] inputVertices,
            VertexWinding desiredWindingOrder,
            out Vector2[] outputVertices,
            out int[] indices)
        {
            List<Triangle> triangles = new List<Triangle>();

            if (DetermineWindingOrder(inputVertices) == VertexWinding.Clockwise)
            {
                outputVertices = ReverseWindingOrder(inputVertices);
            }
            else
            {
                outputVertices = (Vector2[])inputVertices.Clone();
            }

            _polygonVertices.Clear();
            _earVertices.Clear();
            _convexVertices.Clear();
            _reflexVertices.Clear();

            for (int i = 0; i < outputVertices.Length; i++)
            {
                _polygonVertices.AddLast(new Vertex(outputVertices[i], i));
            }

            FindConvexAndReflexVertices();
            FindEarVertices();

            while (_polygonVertices.Count > 3 && _earVertices.Count > 0)
            {
                ClipNextEar(triangles);
            }

            if (_polygonVertices.Count == 3)
            {
                triangles.Add(new Triangle(
                    _polygonVertices[0].Value,
                    _polygonVertices[1].Value,
                    _polygonVertices[2].Value));
            }

            indices = new int[triangles.Count * 3];

            if (desiredWindingOrder == VertexWinding.CounterClockwise)
            {
                for (int i = 0; i < triangles.Count; i++)
                {
                    indices[(i * 3)] = triangles[i].A.Index;
                    indices[(i * 3) + 1] = triangles[i].B.Index;
                    indices[(i * 3) + 2] = triangles[i].C.Index;
                }
            }
            else
            {
                for (int i = 0; i < triangles.Count; i++)
                {
                    indices[(i * 3)] = triangles[i].C.Index;
                    indices[(i * 3) + 1] = triangles[i].B.Index;
                    indices[(i * 3) + 2] = triangles[i].A.Index;
                }
            }
        }

        /// <summary>
        /// Cuts a hole into a shape.
        /// </summary>
        /// <param name="shapeVerts">An array of vertices for the primary shape.</param>
        /// <param name="holeVerts">An array of vertices for the hole to be cut. It is assumed that these vertices lie completely within the shape verts.</param>
        /// <returns>The new array of vertices that can be passed to Triangulate to properly triangulate the shape with the hole.</returns>
        public Vector2[] CutHoleInShape(Vector2[] shapeVerts, Vector2[] holeVerts)
        {
            shapeVerts = EnsureWindingOrder(shapeVerts, VertexWinding.CounterClockwise);
            holeVerts = EnsureWindingOrder(holeVerts, VertexWinding.Clockwise);

            _polygonVertices.Clear();
            _earVertices.Clear();
            _convexVertices.Clear();
            _reflexVertices.Clear();

            for (int i = 0; i < shapeVerts.Length; i++)
            {
                _polygonVertices.AddLast(new Vertex(shapeVerts[i], i));
            }

            CyclicalList<Vertex> holePolygon = new CyclicalList<Vertex>();
            for (int i = 0; i < holeVerts.Length; i++)
            {
                holePolygon.Add(new Vertex(holeVerts[i], i + _polygonVertices.Count));
            }

            FindConvexAndReflexVertices();
            FindEarVertices();

            Vertex rightMostHoleVertex = holePolygon[0];
            foreach (Vertex v in holePolygon)
            {
                if (v.Position.X > rightMostHoleVertex.Position.X)
                {
                    rightMostHoleVertex = v;
                }
            }

            List<LineSegment> segmentsToTest = new List<LineSegment>();
            for (int i = 0; i < _polygonVertices.Count; i++)
            {
                Vertex a = _polygonVertices[i].Value;
                Vertex b = _polygonVertices[i + 1].Value;

                if ((a.Position.X > rightMostHoleVertex.Position.X || b.Position.X > rightMostHoleVertex.Position.X) &&
                    ((a.Position.Y >= rightMostHoleVertex.Position.Y && b.Position.Y <= rightMostHoleVertex.Position.Y) ||
                    (a.Position.Y <= rightMostHoleVertex.Position.Y && b.Position.Y >= rightMostHoleVertex.Position.Y)))
                {
                    segmentsToTest.Add(new LineSegment(a, b));
                }
            }

            float? closestPoint = null;
            LineSegment closestSegment = new LineSegment();
            foreach (LineSegment segment in segmentsToTest)
            {
                float? intersection = segment.IntersectsWithRay(rightMostHoleVertex.Position, Vector2.UnitX);
                if (intersection != null && (closestPoint == null || closestPoint.Value > intersection.Value))
                {
                    closestPoint = intersection;
                    closestSegment = segment;
                }
            }

            if (closestPoint == null)
            {
                return shapeVerts;
            }

            Vector2 I = rightMostHoleVertex.Position + Vector2.UnitX * closestPoint.Value;
            Vertex P = (closestSegment.A.Position.X > closestSegment.B.Position.X)
                ? closestSegment.A
                : closestSegment.B;

            Triangle mip = new Triangle(rightMostHoleVertex, new Vertex(I, 1), P);

            List<Vertex> interiorReflexVertices = new List<Vertex>();
            foreach (Vertex v in _reflexVertices)
            {
                if (mip.ContainsPoint(v))
                {
                    interiorReflexVertices.Add(v);
                }
            }

            //if there are any interior reflex vertices, find the one that, when connected
            //to our rightMostHoleVertex, forms the line closest to Vector2.UnitX
            if (interiorReflexVertices.Count > 0)
            {
                float closestDot = -1.0f;
                foreach (Vertex v in interiorReflexVertices)
                {
                    Vector2 d = Vector2.Normalize(v.Position - rightMostHoleVertex.Position);
                    float dot = Vector2.Dot(Vector2.UnitX, d);

                    if (dot > closestDot)
                    {
                        closestDot = dot;
                        P = v;
                    }
                }
            }

            int mIndex = holePolygon.IndexOf(rightMostHoleVertex);
            int injectPoint = _polygonVertices.IndexOf(P);
            
            for (int i = mIndex; i <= mIndex + holePolygon.Count; i++)
            {
                _polygonVertices.AddAfter(_polygonVertices[injectPoint++], holePolygon[i]);
            }
            _polygonVertices.AddAfter(_polygonVertices[injectPoint], P);
            
            Vector2[] newShapeVerts = new Vector2[_polygonVertices.Count];
            for (int i = 0; i < _polygonVertices.Count; i++)
            {
                newShapeVerts[i] = _polygonVertices[i].Value.Position;
            }

            return newShapeVerts;
        }

        /// <summary>
        /// Ensures that a set of vertices are wound in a particular order, reversing them if necessary.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <param name="windingOrder">The desired winding order.</param>
        /// <returns>A new set of vertices if the winding order didn't match; otherwise the original set.</returns>
        private Vector2[] EnsureWindingOrder(Vector2[] vertices, VertexWinding windingOrder)
        {
            if (DetermineWindingOrder(vertices) != windingOrder)
            {
                return ReverseWindingOrder(vertices);
            }

            return vertices;
        }

        /// <summary>
        /// Reverses the winding order for a set of vertices.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The new vertices for the polygon with the opposite winding order.</returns>
        private Vector2[] ReverseWindingOrder(Vector2[] vertices)
        {
            Vector2[] newVerts = new Vector2[vertices.Length];

            newVerts[0] = vertices[0];
            for (int i = 1; i < newVerts.Length; i++)
            {
                newVerts[i] = vertices[vertices.Length - i];
            }

            return newVerts;
        }

        /// <summary>
        /// Determines the winding order of a polygon given a set of vertices.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The calculated winding order of the polygon.</returns>
        private VertexWinding DetermineWindingOrder(Vector2[] vertices)
        {
            int clockWiseCount = 0;
            int counterClockWiseCount = 0;
            Vector2 p1 = vertices[0];

            for (int i = 1; i < vertices.Length; i++)
            {
                Vector2 p2 = vertices[i];
                Vector2 p3 = vertices[(i + 1) % vertices.Length];

                Vector2 e1 = p1 - p2;
                Vector2 e2 = p3 - p2;

                if (e1.X * e2.Y - e1.Y * e2.X >= 0.0f)
                {
                    clockWiseCount++;
                }
                else
                {
                    counterClockWiseCount++;
                }

                p1 = p2;
            }

            return (clockWiseCount > counterClockWiseCount)
                ? VertexWinding.Clockwise
                : VertexWinding.CounterClockwise;
        }

        private void ClipNextEar(ICollection<Triangle> triangles)
        {
            Vertex ear = _earVertices[0].Value;
            Vertex prev = _polygonVertices[_polygonVertices.IndexOf(ear) - 1].Value;
            Vertex next = _polygonVertices[_polygonVertices.IndexOf(ear) + 1].Value;
            triangles.Add(new Triangle(ear, next, prev));

            _earVertices.RemoveAt(0);
            _polygonVertices.RemoveAt(_polygonVertices.IndexOf(ear));

            ValidateAdjacentVertex(prev);
            ValidateAdjacentVertex(next);
        }

        private void ValidateAdjacentVertex(Vertex vertex)
        {
            if (_reflexVertices.Contains(vertex))
            {
                if (IsConvex(vertex))
                {
                    _reflexVertices.Remove(vertex);
                    _convexVertices.Add(vertex);
                }
            }

            if (_convexVertices.Contains(vertex))
            {
                bool wasEar = _earVertices.Contains(vertex);
                bool isEar = IsEar(vertex);

                if (wasEar && !isEar)
                {
                    _earVertices.Remove(vertex);
                }
                else if (!wasEar && isEar)
                {
                    _earVertices.AddFirst(vertex);
                }
            }
        }

        private void FindConvexAndReflexVertices()
        {
            for (int i = 0; i < _polygonVertices.Count; i++)
            {
                Vertex v = _polygonVertices[i].Value;
                if (IsConvex(v))
                {
                    _convexVertices.Add(v);
                }
                else
                {
                    _reflexVertices.Add(v);
                }
            }
        }

        private void FindEarVertices()
        {
            for (int i = 0; i < _convexVertices.Count; i++)
            {
                Vertex c = _convexVertices[i];
                if (IsEar(c))
                {
                    _earVertices.AddLast(c);
                }
            }
        }

        private bool IsEar(Vertex c)
        {
            Vertex p = _polygonVertices[_polygonVertices.IndexOf(c) - 1].Value;
            Vertex n = _polygonVertices[_polygonVertices.IndexOf(c) + 1].Value;

            foreach (Vertex t in _reflexVertices)
            {
                if (t.Equals(p) || t.Equals(c) || t.Equals(n))
                {
                    continue;
                }

                if (Triangle.ContainsPoint(p, c, n, t))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsConvex(Vertex c)
        {
            Vertex p = _polygonVertices[_polygonVertices.IndexOf(c) - 1].Value;
            Vertex n = _polygonVertices[_polygonVertices.IndexOf(c) + 1].Value;

            Vector2 d1 = Vector2.Normalize(c.Position - p.Position);
            Vector2 d2 = Vector2.Normalize(n.Position - c.Position);
            Vector2 n2 = new Vector2(-d2.Y, d2.X);

            return (Vector2.Dot(d1, n2) <= 0.0f);
        }

        private struct Vertex
        {
            public readonly Vector2 Position;
            public readonly int Index;

            public Vertex(Vector2 position, int index)
            {
                Position = position;
                Index = index;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Vertex))
                {
                    return false;
                }

                return Equals((Vertex)obj);
            }

            public bool Equals(Vertex obj)
            {
                return obj.Position.Equals(Position) && obj.Index == Index;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Position.GetHashCode() * 397) ^ Index;
                }
            }

            public override string ToString()
            {
                return string.Format("{0} ({1})", Position, Index);
            }
        }

        private struct LineSegment
        {
            public Vertex A;
            public Vertex B;

            public LineSegment(Vertex a, Vertex b)
            {
                A = a;
                B = b;
            }

            public float? IntersectsWithRay(Vector2 origin, Vector2 direction)
            {
                float largestDistance = Math.Max(A.Position.X - origin.X, B.Position.X - origin.X) * 2.0f;
                LineSegment raySegment = new LineSegment(new Vertex(origin, 0), new Vertex(origin + (direction * largestDistance), 0));

                Vector2? intersection = FindIntersection(this, raySegment);
                float? value = null;

                if (intersection != null)
                {
                    value = Vector2.Distance(origin, intersection.Value);
                }

                return value;
            }

            public static Vector2? FindIntersection(LineSegment a, LineSegment b)
            {
                float x1 = a.A.Position.X;
                float y1 = a.A.Position.Y;
                float x2 = a.B.Position.X;
                float y2 = a.B.Position.Y;
                float x3 = b.A.Position.X;
                float y3 = b.A.Position.Y;
                float x4 = b.B.Position.X;
                float y4 = b.B.Position.Y;

                float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

                float uaNum = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
                float ubNum = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);

                float ua = uaNum / denom;
                float ub = ubNum / denom;

                if (!MathHelper.IsApproxEquals(MathHelper.Clamp(ua, 0.0f, 1.0f), ua) || 
                    !MathHelper.IsApproxEquals(MathHelper.Clamp(ub, 0.0f, 1.0f), ub))
                {
                    return null;
                }

                return a.A.Position + (a.B.Position - a.A.Position) * ua;
            }
        }

        /// <summary>
        /// A basic triangle structure that holds the three vertices that make up a given triangle.
        /// </summary>
        private struct Triangle
        {
            public readonly Vertex A;
            public readonly Vertex B;
            public readonly Vertex C;

            public Triangle(Vertex a, Vertex b, Vertex c)
            {
                A = a;
                B = b;
                C = c;
            }

            public bool ContainsPoint(Vertex point)
            {
                if (point.Equals(A) || point.Equals(B) || point.Equals(C))
                {
                    return true;
                }

                bool oddNodes = false;

                if (checkPointToSegment(C, A, point))
                {
                    oddNodes = !oddNodes;
                }

                if (checkPointToSegment(A, B, point))
                {
                    oddNodes = !oddNodes;
                }

                if (checkPointToSegment(B, C, point))
                {
                    oddNodes = !oddNodes;
                }

                return oddNodes;
            }

            public static bool ContainsPoint(Vertex a, Vertex b, Vertex c, Vertex point)
            {
                return new Triangle(a, b, c).ContainsPoint(point);
            }

            static bool checkPointToSegment(Vertex sA, Vertex sB, Vertex point)
            {
                if ((sA.Position.Y < point.Position.Y && sB.Position.Y >= point.Position.Y) ||
                    (sB.Position.Y < point.Position.Y && sA.Position.Y >= point.Position.Y))
                {
                    float x =
                        sA.Position.X +
                        (point.Position.Y - sA.Position.Y) /
                        (sB.Position.Y - sA.Position.Y) *
                        (sB.Position.X - sA.Position.X);

                    if (x < point.Position.X)
                    {
                        return true;
                    }
                }

                return false;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Triangle))
                {
                    return false;
                }

                return Equals((Triangle)obj);
            }

            public bool Equals(Triangle obj)
            {
                return obj.A.Equals(A) && obj.B.Equals(B) && obj.C.Equals(C);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = A.GetHashCode();
                    result = (result * 397) ^ B.GetHashCode();
                    result = (result * 397) ^ C.GetHashCode();
                    return result;
                }
            }
        }
    }
}
