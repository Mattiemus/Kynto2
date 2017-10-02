namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    using Content;

    /// <summary>
    /// Defines a 3-point primitive. The vertex winding is considered to be clockwise ordering of {A, B, C}.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Triangle : IEquatable<Triangle>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// First vertex of the triangle.
        /// </summary>
        public Vector3 PointA;

        /// <summary>
        /// Second vertex of the triangle.
        /// </summary>
        public Vector3 PointB;

        /// <summary>
        /// Third vertex of the triangle.
        /// </summary>
        public Vector3 PointC;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> struct. The vertices are
        /// considered to be in clockwise order of {A, B, C}.
        /// </summary>
        /// <param name="pointA">First vertex</param>
        /// <param name="pointB">Second vertex</param>
        /// <param name="pointC">Third vertex</param>
        public Triangle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            PointA = pointA;
            PointB = pointB;
            PointC = pointC;
        }

        /// <summary>
        /// Gets the normal vector of the triangle. The triangle is considered to be in a clockwise ordering
        /// of {A, B, C}.
        /// </summary>
        public Vector3 Normal
        {
            get
            {
                Vector3.Subtract(ref PointC, ref PointA, out Vector3 edgeCA);
                Vector3.Subtract(ref PointB, ref PointA, out Vector3 edgeBA);

                Vector3.NormalizedCross(ref edgeCA, ref edgeBA, out Vector3 normal);

                return normal;
            }
        }

        /// <summary>
        /// Gets the plane that the triangle resides on.
        /// </summary>
        public Plane Plane
        {
            get
            {
                Vector3.Subtract(ref PointC, ref PointA, out Vector3 edgeCA);
                Vector3.Subtract(ref PointB, ref PointA, out Vector3 edgeBA);

                Vector3.NormalizedCross(ref edgeCA, ref edgeBA, out Vector3 normal);
                
                Vector3.Add(ref PointA, ref PointB, out Vector3 center);
                Vector3.Add(ref PointC, ref center, out center);
                Vector3.Multiply(ref center, MathHelper.OneThird, out center);

                return new Plane(normal, center);
            }
        }

        /// <summary>
        /// Gets the center of the triangle.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                Vector3.Add(ref PointA, ref PointB, out Vector3 center);
                Vector3.Add(ref PointC, ref center, out center);
                Vector3.Multiply(ref center, MathHelper.OneThird, out center);

                return center;
            }
        }

        /// <summary>
        /// Gets the edge representing B - A.
        /// </summary>
        public Vector3 EdgeBA
        {
            get
            {
                Vector3.Subtract(ref PointB, ref PointA, out Vector3 edgeBA);
                return edgeBA;
            }
        }

        /// <summary>
        /// Gets the edge representing C- A.
        /// </summary>
        public Vector3 EdgeCA
        {
            get
            {
                Vector3 edgeCA;
                Vector3.Subtract(ref PointC, ref PointA, out edgeCA);

                return edgeCA;
            }
        }

        /// <summary>
        /// Gets the edge representing B - C.
        /// </summary>
        public Vector3 EdgeBC
        {
            get
            {
                Vector3.Subtract(ref PointB, ref PointC, out Vector3 edgeBC);
                return edgeBC;
            }
        }

        /// <summary>
        /// Gets the area of the triangle.
        /// </summary>
        public float Area
        {
            get
            {
                float area = 0.5f * ((PointA.X * (PointB.Y - PointC.Y)) + (PointB.X * (PointC.Y - PointA.Y)) + (PointC.X * (PointA.Y - PointB.Y)));
                return Math.Abs(area);
            }
        }

        /// <summary>
        /// Gets if the triangle is degenerate (normal is zero).
        /// </summary>
        public bool IsDegenerate => Normal.Equals(Vector3.Zero);

        /// <summary>
        /// Gets whether any of the components of the triangle are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => PointA.IsNaN || PointB.IsNaN || PointC.IsNaN;

        /// <summary>
        /// Gets whether any of the components of the triangle are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => PointA.IsInfinity || PointB.IsInfinity || PointC.IsInfinity;

        /// <summary>
        /// Gets or sets triangle vertices by a zero-based index, in the order of {A, B, C} where index 0 is Point A.
        /// </summary>
        /// <param name="index">Zero based index.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
        public Vector3 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return PointA;
                    case 1:
                        return PointB;
                    case 2:
                        return PointC;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        PointA = value;
                        break;
                    case 1:
                        PointB = value;
                        break;
                    case 2:
                        PointC = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
                }
            }
        }

        #region Swap Vertex winding

        /// <summary>
        /// Swaps the vertex winding of the triangle from clockwise {A, B, C} to counter-clockwise {A, C, B}. Effectively,
        /// this swaps vertices B and C, and negates the triangle's normal.
        /// </summary>
        /// <param name="triangle">Triangle to swap vertex order.</param>
        /// <returns>Triangle with swapped vertex order.</returns>
        public static Triangle SwapVertexWinding(Triangle triangle)
        {
            triangle.SwapVertexWinding();
            return triangle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triangle">Triangle to swap vertex order.</param>
        /// <param name="result">Triangle with swapped vertex order.</param>
        public static void SwapVertexWinding(ref Triangle triangle, out Triangle result)
        {
            result = triangle;
            result.SwapVertexWinding();
        }

        #endregion

        #region Transform

        /// <summary>
        /// Transforms the triangle by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="rotation">Quaternion rotation.</param>
        /// <returns>Transformed triangle.</returns>
        public static Triangle Transform(Triangle triangle, Quaternion rotation)
        {
            Transform(ref triangle, ref rotation, out Triangle result);
            return result;
        }

        /// <summary>
        /// Transforms the triangle by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="rotation">Quaternion rotation.</param>
        /// <param name="result">Transformed triangle.</param>
        public static void Transform(ref Triangle triangle, ref Quaternion rotation, out Triangle result)
        {
            Vector3.Transform(ref triangle.PointA, ref rotation, out result.PointA);
            Vector3.Transform(ref triangle.PointB, ref rotation, out result.PointB);
            Vector3.Transform(ref triangle.PointC, ref rotation, out result.PointC);
        }

        /// <summary>
        /// Transforms the triangle by the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="transform">Transformation matrix.</param>
        /// <returns>Transformed triangle.</returns>
        public static Triangle Transform(Triangle triangle, Matrix4x4 transform)
        {
            Transform(ref triangle, ref transform, out Triangle result);
            return result;
        }

        /// <summary>
        /// Transforms the triangle by the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="transform">Transformation matrix.</param>
        /// <param name="result">Transformed triangle.</param>
        public static void Transform(ref Triangle triangle, ref Matrix4x4 transform, out Triangle result)
        {
            Vector3.Transform(ref triangle.PointA, ref transform, out result.PointA);
            Vector3.Transform(ref triangle.PointB, ref transform, out result.PointB);
            Vector3.Transform(ref triangle.PointC, ref transform, out result.PointC);
        }

        /// <summary>
        /// Translates the triangle by the given translation vector.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="translation">Translation vector.</param>
        /// <returns>Transformed triangle.</returns>
        public static Triangle Transform(Triangle triangle, Vector3 translation)
        {
            Transform(ref triangle, ref translation, out Triangle result);
            return result;
        }

        /// <summary>
        /// Translates the triangle by the given translation vector.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="translation">Translation vector.</param>
        /// <param name="result">Transformed triangle.</param>
        public static void Transform(ref Triangle triangle, ref Vector3 translation, out Triangle result)
        {
            Vector3.Add(ref triangle.PointA, ref translation, out result.PointA);
            Vector3.Add(ref triangle.PointB, ref translation, out result.PointB);
            Vector3.Add(ref triangle.PointC, ref translation, out result.PointC);
        }

        /// <summary>
        /// Scales the triangle by the given scaling factor, which is uniform along the X, Y, and Z axes.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <returns>Transformed triangle.</returns>
        public static Triangle Transform(Triangle triangle, float scale)
        {
            Transform(ref triangle, scale, out Triangle result);
            return result;
        }

        /// <summary>
        /// Scales the triangle by the given scaling factor, which is uniform along the X, Y, and Z axes.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <param name="result">Transformed triangle.</param>
        public static void Transform(ref Triangle triangle, float scale, out Triangle result)
        {
            Vector3.Multiply(ref triangle.PointA, scale, out result.PointA);
            Vector3.Multiply(ref triangle.PointB, scale, out result.PointB);
            Vector3.Multiply(ref triangle.PointC, scale, out result.PointC);
        }

        /// <summary>
        /// Scales the triangle by the given scaling factors along the X, Y, and Z axes.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="xScale">Scaling factor along the X axis.</param>
        /// <param name="yScale">Scaling factor along the Y axis.</param>
        /// <param name="zScale">Scaling factor along the Z axis.</param>
        /// <returns>Transformed triangle.</returns>
        public static Triangle Transform(Triangle triangle, float xScale, float yScale, float zScale)
        {
            Transform(ref triangle, xScale, yScale, zScale, out Triangle result);
            return result;
        }

        /// <summary>
        /// Scales the triangle by the given scaling factors along the X, Y, and Z axes.
        /// </summary>
        /// <param name="triangle">Triangle to transform.</param>
        /// <param name="xScale">Scaling factor along the X axis.</param>
        /// <param name="yScale">Scaling factor along the Y axis.</param>
        /// <param name="zScale">Scaling factor along the Z axis.</param>
        /// <param name="result">Transformed triangle.</param>
        public static void Transform(ref Triangle triangle, float xScale, float yScale, float zScale, out Triangle result)
        {
            Vector3 scale = new Vector3(xScale, yScale, zScale);

            Vector3.Multiply(ref triangle.PointA, ref scale, out result.PointA);
            Vector3.Multiply(ref triangle.PointB, ref scale, out result.PointB);
            Vector3.Multiply(ref triangle.PointC, ref scale, out result.PointC);
        }

        #endregion

        #region Equality Operators

        /// <summary>
        /// Tests equality between two triangles.
        /// </summary>
        /// <param name="a">First triangle</param>
        /// <param name="b">Second triangle</param>
        /// <returns>True if the points of the two triangles are equal, false otherwise.</returns>
        public static bool operator ==(Triangle a, Triangle b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two triangles.
        /// </summary>
        /// <param name="a">First triangle</param>
        /// <param name="b">Second triangle</param>
        /// <returns>True if the points of the two triangles are equal, false otherwise.</returns>
        public static bool operator !=(Triangle a, Triangle b)
        {
            return !a.Equals(ref b);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Swaps the vertex winding of the triangle from clockwise {A, B, C} to counter-clockwise {A, C, B}. Effectively,
        /// this swaps vertices B and C, and negates the triangle's normal.
        /// </summary>
        public void SwapVertexWinding()
        {
            Vector3 temp = PointB;
            PointB = PointC;
            PointC = temp;
        }

        /// <summary>
        /// Tests if the point in space is on the triangle.
        /// </summary>
        /// <param name="pt">Point on triangle</param>
        /// <returns>True if the point is in fact on the triangle, false otherwise.</returns>
        public bool IsPointOn(Vector3 pt)
        {
            IsPointOn(ref pt, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the point in space is on the triangle.
        /// </summary>
        /// <param name="pt">Point on triangle</param>
        /// <param name="result">True if the point is in fact on the triangle, false otherwise.</param>
        public void IsPointOn(ref Vector3 pt, out bool result)
        {
            if (IsDegenerate)
            {
                result = false;
                return;
            }
            
            Vector3.Subtract(ref PointC, ref PointA, out Vector3 v0);
            Vector3.Subtract(ref PointB, ref PointA, out Vector3 v1);
            Vector3.Subtract(ref pt, ref PointA, out Vector3 v2);
            
            Vector3.Dot(ref v0, ref v0, out float dot00);
            Vector3.Dot(ref v0, ref v1, out float dot01);
            Vector3.Dot(ref v0, ref v2, out float dot02);
            Vector3.Dot(ref v1, ref v1, out float dot11);
            Vector3.Dot(ref v1, ref v2, out float dot12);

            float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            float s = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float t = (dot00 * dot12 - dot01 * dot02) * invDenom;

            result = (s >= 0.0f) && (t >= 0.0f) && (s + t <= 1.0f);
        }

        #endregion

        #region Intersects

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Ray ray, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectRayTriangle(ref this, ref ray, ignoreBackface, out LineIntersectionResult result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Ray ray, out LineIntersectionResult result, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectRayTriangle(ref this, ref ray, ignoreBackface, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Ray ray, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectRayTriangle(ref this, ref ray, ignoreBackface, out LineIntersectionResult result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Ray ray, out LineIntersectionResult result, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectRayTriangle(ref this, ref ray, ignoreBackface, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Segment segment, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectSegmentTriangle(ref this, ref segment, ignoreBackface, out LineIntersectionResult result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Segment segment, out LineIntersectionResult result, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectSegmentTriangle(ref this, ref segment, ignoreBackface, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Segment segment, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectSegmentTriangle(ref this, ref segment, ignoreBackface, out LineIntersectionResult result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Segment segment, out LineIntersectionResult result, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectSegmentTriangle(ref this, ref segment, ignoreBackface, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Plane plane)
        {
            return GeometricToolsHelper.IntersectPlaneTriangle(ref this, ref plane);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Plane plane, out Segment result)
        {
            return GeometricToolsHelper.IntersectPlaneTriangle(ref this, ref plane, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Plane plane)
        {
            return GeometricToolsHelper.IntersectPlaneTriangle(ref this, ref plane);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Plane plane, out Segment result)
        {
            return GeometricToolsHelper.IntersectPlaneTriangle(ref this, ref plane, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Triangle triangle)
        {
            return GeometricToolsHelper.IntersectTriangleTriangle(ref this, ref triangle);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Triangle triangle, out Segment result)
        {
            return GeometricToolsHelper.IntersectTriangleTriangle(ref this, ref triangle, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Triangle triangle)
        {
            return GeometricToolsHelper.IntersectTriangleTriangle(ref this, ref triangle);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Triangle triangle, out Segment result)
        {
            return GeometricToolsHelper.IntersectTriangleTriangle(ref this, ref triangle, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Ellipse ellipse)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Ellipse ellipse, out Segment result)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Ellipse ellipse)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Ellipse ellipse, out Segment result)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="BoundingVolume"/>.
        /// </summary>
        /// <param name="volume">Bounding volume to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(BoundingVolume volume)
        {
            if (volume == null)
            {
                return false;
            }

            return volume.Intersects(ref this);
        }

        #endregion

        #region Distance To

        /// <summary>
        /// Determines the distance between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Vector3 point)
        {
            GeometricToolsHelper.DistancePointTriangle(ref this, ref point, out Vector3 ptOnTriangle, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointTriangle(ref this, ref point, out Vector3 ptOnTriangle, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Ray ray)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref this, ref ray, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Ray ray, out float result)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref this, ref ray, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Segment segment)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref this, ref segment, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Segment segment, out float result)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref this, ref segment, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Plane plane)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref this, ref plane, out Vector3 ptOnTriangle, out Vector3 ptOnPlane, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Plane plane, out float result)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref this, ref plane, out Vector3 ptOnTriangle, out Vector3 ptOnPlane, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Triangle triangle)
        {
            GeometricToolsHelper.DistanceTriangleTriangle(ref this, ref triangle, out Vector3 ptOnFirst, out Vector3 ptOnSecond, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Triangle triangle, out float result)
        {
            GeometricToolsHelper.DistanceTriangleTriangle(ref this, ref triangle, out Vector3 ptOnFirst, out Vector3 ptOnSecond, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Ellipse ellipse)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines the distance between this object and an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Ellipse ellipse, out float result)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion

        #region Distance Squared To

        /// <summary>
        /// Determines the distance squared between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Vector3 point)
        {
            GeometricToolsHelper.DistancePointTriangle(ref this, ref point, out Vector3 ptOnTriangle, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointTriangle(ref this, ref point, out Vector3 ptOnTriangle, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Ray ray)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref this, ref ray, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Ray ray, out float result)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref this, ref ray, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Segment segment)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref this, ref segment, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Segment segment, out float result)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref this, ref segment, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Plane plane)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref this, ref plane, out Vector3 ptOnTriangle, out Vector3 ptOnPlane, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Plane plane, out float result)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref this, ref plane, out Vector3 ptOnTriangle, out Vector3 ptOnPlane, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Triangle triangle)
        {
            GeometricToolsHelper.DistanceTriangleTriangle(ref this, ref triangle, out Vector3 ptOnFirst, out Vector3 ptOnSecond, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Triangle triangle, out float result)
        {
            GeometricToolsHelper.DistanceTriangleTriangle(ref this, ref triangle, out Vector3 ptOnFirst, out Vector3 ptOnSecond, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Ellipse ellipse)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines the distance squared between this object and an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Ellipse ellipse, out float result)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion

        #region Closest Point / Approach Segment

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>Closest point on this object.</returns>
        public Vector3 ClosestPointTo(Vector3 point)
        {
            GeometricToolsHelper.DistancePointTriangle(ref this, ref point, out Vector3 ptOnTriangle, out float sqrDist);
            return ptOnTriangle;
        }

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Closest point on this object.</param>
        public void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            GeometricToolsHelper.DistancePointTriangle(ref this, ref point, out result, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Closest point on this object.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestPointTo(ref Vector3 point, out Vector3 result, out float squaredDistance)
        {
            GeometricToolsHelper.DistancePointTriangle(ref this, ref point, out result, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Ray ray)
        {
            Segment result;
            GeometricToolsHelper.DistanceRayTriangle(ref this, ref ray, out result.StartPoint, out result.EndPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Ray ray, out Segment result)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref this, ref ray, out result.StartPoint, out result.EndPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Ray ray, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref this, ref ray, out result.StartPoint, out result.EndPoint, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Segment segment)
        {
            Segment result;
            GeometricToolsHelper.DistanceSegmentTriangle(ref this, ref segment, out result.StartPoint, out result.EndPoint, out float sqrDist);

            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Segment segment, out Segment result)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref this, ref segment, out result.StartPoint, out result.EndPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Segment segment, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref this, ref segment, out result.StartPoint, out result.EndPoint, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Plane plane)
        {
            Segment result;
            GeometricToolsHelper.DistancePlaneTriangle(ref this, ref plane, out result.StartPoint, out result.EndPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Plane plane, out Segment result)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref this, ref plane, out result.StartPoint, out result.EndPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Plane plane, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref this, ref plane, out result.StartPoint, out result.EndPoint, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Triangle triangle)
        {
            Segment result;
            GeometricToolsHelper.DistanceTriangleTriangle(ref this, ref triangle, out result.StartPoint, out result.EndPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Triangle triangle, out Segment result)
        {
            GeometricToolsHelper.DistanceTriangleTriangle(ref this, ref triangle, out result.StartPoint, out result.EndPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Triangle triangle, out Segment result, float squaredDistance)
        {
            GeometricToolsHelper.DistanceTriangleTriangle(ref this, ref triangle, out result.StartPoint, out result.EndPoint, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Ellipse ellipse)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Ellipse ellipse, out Segment result)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">Ellipse to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Ellipse ellipse, out Segment result, out float squaredDistance)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion

        #region Equality

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is Triangle)
            {
                return Equals((Triangle)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the triangle and another triangle.
        /// </summary>
        /// <param name="other">Other triangle</param>
        /// <returns>True if the points of the two triangles are equal, false otherwise.</returns>
        public bool Equals(Triangle other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the triangle and another triangle.
        /// </summary>
        /// <param name="other">Other triangle</param>
        /// <returns>True if the points of the two triangles are equal, false otherwise.</returns>
        public bool Equals(ref Triangle other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the triangle and another triangle within the specified tolerance.
        /// </summary>
        /// <param name="other">Other triangle</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the points of the two triangles are equal, false otherwise.</returns>
        public bool Equals(Triangle other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the triangle and another triangle within the specified tolerance.
        /// </summary>
        /// <param name="other">Other triangle</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the points of the two triangles are equal, false otherwise.</returns>
        public bool Equals(ref Triangle other, float tolerance)
        {
            return PointA.Equals(ref other.PointA, tolerance) && PointB.Equals(ref other.PointB, tolerance) && PointC.Equals(ref other.PointC, tolerance);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return PointA.GetHashCode() + PointB.GetHashCode() + PointC.GetHashCode();
            }
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString("G", formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return ToString();
            }

            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(formatProvider, "A: ({0}), B: ({1}), C: ({2}), Normal: ({3})", new object[] { PointA.ToString(format, formatProvider), PointB.ToString(format, formatProvider), PointC.ToString(format, formatProvider), Normal.ToString(format, formatProvider) });
        }

        #endregion

        #region IPrimitiveValue

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("PointA", PointA);
            output.Write("PointB", PointB);
            output.Write("PointC", PointC);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            PointA = input.Read<Vector3>();
            PointB = input.Read<Vector3>();
            PointC = input.Read<Vector3>();
        }

        #endregion
    }
}
