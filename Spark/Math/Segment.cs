namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a finite line segment that has a start point and an end point.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Segment : IEquatable<Segment>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// Start point of the line segment.
        /// </summary>
        public Vector3 StartPoint;

        /// <summary>
        /// End point of the line segment.
        /// </summary>
        public Vector3 EndPoint;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> struct.
        /// </summary>
        /// <param name="start">Start point of the line.</param>
        /// <param name="end">End point of the line.</param>
        public Segment(Vector3 start, Vector3 end)
        {
            StartPoint = start;
            EndPoint = end;
        }

        /// <summary>
        /// Gets the size of the <see cref="Segment"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Segment>();

        /// <summary>
        /// Gets the degenerate segment where both endpoints are at the origin.
        /// </summary>
        public static Segment Zero => new Segment(Vector3.Zero, Vector3.Zero);

        /// <summary>
        /// Gets the length of the line segment.
        /// </summary>
        public float Length
        {
            get
            {
                Vector3.Distance(ref StartPoint, ref EndPoint, out float length);
                return length;
            }
        }

        /// <summary>
        /// Gets the half-length (length from the center to each end point) of the line segment.
        /// </summary>
        public float Extent
        {
            get
            {
                Vector3.Distance(ref StartPoint, ref EndPoint, out float length);
                return length * 0.5f;
            }
        }

        /// <summary>
        /// Gets or sets the center point of the line segment.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 dir);

                float halfLength = dir.Length() * 0.5f;
                dir.Normalize();

                Vector3.Multiply(ref dir, halfLength, out dir);
                Vector3.Add(ref dir, ref StartPoint, out Vector3 center);

                return center;
            }
            set
            {
                Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 dir);

                float halfLength = dir.Length() * 0.5f;
                dir.Normalize();

                Vector3.Multiply(ref dir, -halfLength, out Vector3 temp);
                Vector3.Add(ref temp, ref value, out StartPoint);

                Vector3.Multiply(ref dir, halfLength, out temp);
                Vector3.Add(ref temp, ref value, out EndPoint);
            }
        }

        /// <summary>
        /// Gets the direction of the line segment.
        /// </summary>
        public Vector3 Direction
        {
            get
            {
                Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 dir);
                dir.Normalize();

                return dir;
            }
        }

        /// <summary>
        /// Gets if the line segment is degenerate (a point).
        /// </summary>
        public bool IsDegenerate => MathHelper.IsApproxEquals(Length, 0.0f, MathHelper.ZeroTolerance);

        /// <summary>
        /// Gets whether any of the components of the segment are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => StartPoint.IsNaN || EndPoint.IsNaN;

        /// <summary>
        /// Gets whether any of the components of the segment are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => StartPoint.IsInfinity || EndPoint.IsInfinity;

        #region From methods

        /// <summary>
        /// Creates a segment from a center-extent representation. The extent is the half-length positive and negative distance along the direction
        /// vector from the center to each end point. 
        /// </summary>
        /// <param name="center">Center of the segment.</param>
        /// <param name="direction">Direction of the segment.</param>
        /// <param name="extent">Half-length extent of the segment.</param>
        /// <returns>Segment</returns>
        public static Segment FromCenterExtent(Vector3 center, Vector3 direction, float extent)
        {
            FromCenterExtent(ref center, ref direction, extent, out Segment result);
            return result;
        }

        /// <summary>
        /// Creates a segment from a center-extent representation. The extent is the half-length positive and negative distance along the direction
        /// vector from the center to each end point. 
        /// </summary>
        /// <param name="center">Center of the segment.</param>
        /// <param name="direction">Direction of the segment.</param>
        /// <param name="extent">Half-length extent of the segment.</param>
        /// <param name="result">Segment</param>
        public static void FromCenterExtent(ref Vector3 center, ref Vector3 direction, float extent, out Segment result)
        {
            Vector3.Multiply(ref direction, -extent, out Vector3 temp);
            Vector3.Add(ref temp, ref center, out result.StartPoint);

            Vector3.Multiply(ref direction, extent, out temp);
            Vector3.Add(ref temp, ref center, out result.EndPoint);
        }

        #endregion

        #region Transform

        /// <summary>
        /// Transforms the segment's end points by the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="segment">Segment to transform.</param>
        /// <param name="m">Transformation matrix.</param>
        /// <returns>Transformed segment.</returns>
        public static Segment Transform(Segment segment, Matrix4x4 m)
        {
            Transform(ref segment, ref m, out Segment result);
            return result;
        }

        /// <summary>
        /// Transforms the segment's end points by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="segment">Segment to transform.</param>
        /// <param name="m">Transformation matrix.</param>
        /// <param name="result">Transformed segment.</param>
        public static void Transform(ref Segment segment, ref Matrix4x4 m, out Segment result)
        {
            Vector3.Transform(ref segment.StartPoint, ref m, out result.StartPoint);
            Vector3.Transform(ref segment.EndPoint, ref m, out result.EndPoint);
        }

        /// <summary>
        /// Transforms the segment's end points by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="segment">Segment to transform.</param>
        /// <param name="rotation">Quaternion rotation.</param>
        /// <returns>Transformed segment.</returns>
        public static Segment Transform(Segment segment, Quaternion rotation)
        {
            Transform(ref segment, ref rotation, out Segment result);
            return result;
        }

        /// <summary>
        /// Transforms the segment's end points by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="segment">Segment to transform.</param>
        /// <param name="rotation">Quaternion rotation.</param>
        /// <param name="result">Transformed segment.</param>
        public static void Transform(ref Segment segment, ref Quaternion rotation, out Segment result)
        {
            Vector3.Transform(ref segment.StartPoint, ref rotation, out result.StartPoint);
            Vector3.Transform(ref segment.EndPoint, ref rotation, out result.EndPoint);
        }

        /// <summary>
        /// Scales the segment by the given scaling factor. This will scale the extents (half-lengths from the center
        /// to the end points) and then recompute the end points using the scaled extent and the direction of the
        /// original segment.
        /// </summary>
        /// <param name="segment">Segment to transform.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <returns>Transformed segment.</returns>
        public static Segment Transform(Segment segment, float scale)
        {
            Transform(ref segment, scale, out Segment result);
            return result;
        }

        /// <summary>
        /// Scales the segment by the given scaling factor. This will scale the extents (half-lengths from the center
        /// to the end points) and then recompute the end points using the scaled extent and the direction of the
        /// original segment.
        /// </summary>
        /// <param name="segment">Segment to transform.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <param name="result">Transformed segment.</param>
        public static void Transform(ref Segment segment, float scale, out Segment result)
        {
            Vector3 dir, center;
            float extent;
            GeometricToolsHelper.CalculateSegmentProperties(ref segment, out center, out dir, out extent);

            extent *= scale;

            Vector3.Multiply(ref dir, -extent, out result.StartPoint);
            Vector3.Add(ref result.StartPoint, ref center, out result.StartPoint);

            Vector3.Multiply(ref dir, extent, out result.EndPoint);
            Vector3.Add(ref result.EndPoint, ref center, out result.EndPoint);
        }

        /// <summary>
        /// Translates the segment by the given translation vector.
        /// </summary>
        /// <param name="segment">Segment to transform.</param>
        /// <param name="translation">Translation vector.</param>
        /// <returns>Transformed segment.</returns>
        public static Segment Transform(Segment segment, Vector3 translation)
        {
            Transform(ref segment, ref translation, out Segment result);
            return result;
        }

        /// <summary>
        /// Translates the segment by the given translation vector.
        /// </summary>
        /// <param name="segment">Segment to transform.</param>
        /// <param name="translation">Translation vector.</param>
        /// <param name="result">Transformed segment.</param>
        public static void Transform(ref Segment segment, ref Vector3 translation, out Segment result)
        {
            Vector3.Add(ref segment.StartPoint, ref translation, out result.StartPoint);
            Vector3.Add(ref segment.EndPoint, ref translation, out result.EndPoint);
        }

        #endregion

        #region Equality Operators

        /// <summary>
        /// Tests equality between two lines.
        /// </summary>
        /// <param name="a">First line</param>
        /// <param name="b">Second line</param>
        /// <returns>True if both lines are equal, false otherwise.</returns>
        public static bool operator ==(Segment a, Segment b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two lines.
        /// </summary>
        /// <param name="a">First line</param>
        /// <param name="b">Second line</param>
        /// <returns>True if both lines are not equal, false otherwise.</returns>
        public static bool operator !=(Segment a, Segment b)
        {
            return !a.Equals(ref b);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Tests if the line is perpendicular to the specified line.
        /// </summary>
        /// <param name="line">Line to test against.</param>
        /// <returns>True if the lines are perpendicular, false otherwise.</returns>
        public bool IsPerpendicularTo(Segment line)
        {
            IsPerpendicularTo(ref line, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the line is perpendicular to the specified line.
        /// </summary>
        /// <param name="line">Line to test against.</param>
        /// <param name="result">True if the lines are perpendicular, false otherwise.</param>
        public void IsPerpendicularTo(ref Segment line, out bool result)
        {
            Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 v0);
            Vector3.Subtract(ref line.EndPoint, ref line.StartPoint, out Vector3 v1);

            v0.Normalize();
            v1.Normalize();
            
            Vector3.Dot(ref v0, ref v1, out float dot);

            result = MathHelper.IsApproxEquals(dot, 0.0f);
        }

        /// <summary>
        /// Tests if the line is parallel to the specified line.
        /// </summary>
        /// <param name="line">Line to test against.</param>
        /// <returns>True if the lines are parallel, false otherwise.</returns>
        public bool IsParallelTo(Segment line)
        {
            IsParallelTo(ref line, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the line is parallel to the specified line.
        /// </summary>
        /// <param name="line">Line to test against.</param>
        /// <param name="result">True if the lines are parallel, false otherwise.</param>
        public void IsParallelTo(ref Segment line, out bool result)
        {
            Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 v0);
            Vector3.Subtract(ref line.EndPoint, ref line.StartPoint, out Vector3 v1);
            
            Vector3.NormalizedCross(ref v0, ref v1, out Vector3 cross);

            result = cross.Equals(Vector3.Zero);
        }

        #endregion

        #region Intersects

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(Ray ray)
        {
            return GeometricToolsHelper.IntersectRaySegmentXY(ref ray, ref this);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(Ray ray, out Vector3 result)
        {
            return GeometricToolsHelper.IntersectRaySegmentXY(ref ray, ref this, out result, out float rayParam, out float segParam);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(ref Ray ray)
        {
            return GeometricToolsHelper.IntersectRaySegmentXY(ref ray, ref this);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(ref Ray ray, out Vector3 result)
        {
            return GeometricToolsHelper.IntersectRaySegmentXY(ref ray, ref this, out result, out float rayParam, out float segParam);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(Segment segment)
        {
            return GeometricToolsHelper.IntersectSegmentSegmentXY(ref this, ref segment);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(Segment segment, out Vector3 result)
        {
            return GeometricToolsHelper.IntersectSegmentSegmentXY(ref this, ref segment, out result, out float seg0Param, out float seg1Param);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(ref Segment segment)
        {
            return GeometricToolsHelper.IntersectSegmentSegmentXY(ref this, ref segment);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(ref Segment segment, out Vector3 result)
        {
            return GeometricToolsHelper.IntersectSegmentSegmentXY(ref this, ref segment, out result, out float seg0Param, out float seg1Param);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Plane plane)
        {
            return GeometricToolsHelper.IntersectSegmentPlane(ref this, ref plane);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Plane plane, out LineIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectSegmentPlane(ref this, ref plane, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Plane plane)
        {
            return GeometricToolsHelper.IntersectSegmentPlane(ref this, ref plane);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Plane plane, out LineIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectSegmentPlane(ref this, ref plane, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Triangle triangle, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectSegmentTriangle(ref triangle, ref this, ignoreBackface, out LineIntersectionResult result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Triangle triangle, out LineIntersectionResult result, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectSegmentTriangle(ref triangle, ref this, ignoreBackface, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Triangle triangle, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectSegmentTriangle(ref triangle, ref this, ignoreBackface, out LineIntersectionResult result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Triangle triangle, out LineIntersectionResult result, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectSegmentTriangle(ref triangle, ref this, ignoreBackface, out result);
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
        public bool Intersects(Ellipse ellipse, out LineIntersectionResult result)
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
        public bool Intersects(ref Ellipse ellipse, out LineIntersectionResult result)
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

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="BoundingVolume"/>.
        /// </summary>
        /// <param name="volume">Bounding volume to test.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(BoundingVolume volume, out BoundingIntersectionResult result)
        {
            if (volume == null)
            {
                result = new BoundingIntersectionResult();
                return false;
            }

            return volume.Intersects(ref this, out result);
        }

        #endregion

        #region Distance/Point To/From

        /// <summary>
        /// Gets the point along the segment that is the distance from the start point.
        /// </summary>
        /// <param name="distance">Distance of the point from the start point.</param>
        /// <returns>The point along the segment.</returns>
        public Vector3 PointAtDistance(float distance)
        {
            PointAtDistance(distance, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Gets the point along the segment that is the distance from the start point.
        /// </summary>
        /// <param name="distance">Distance of the point from the start point.</param>
        /// <param name="result">The point along the segment.</param>
        public void PointAtDistance(float distance, out Vector3 result)
        {
            Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 dir);
            dir.Normalize();

            Vector3.Multiply(ref dir, distance, out result);
            Vector3.Add(ref StartPoint, ref result, out result);
        }

        /// <summary>
        /// Gets the distance of the point (or closest on the segment) from the start point.
        /// </summary>
        /// <param name="point">Point along or near the segment.</param>
        /// <returns>The distance from the point (or point closest on the segment) to the start point.</returns>
        public float DistanceAtPoint(Vector3 point)
        {
            DistanceAtPoint(ref point, out float distance);
            return distance;
        }

        /// <summary>
        /// Gets the distance of the point (or closest on the segment) from the start point.
        /// </summary>
        /// <param name="point">Point along or near the segment.</param>
        /// <param name="distance">The distance from the point (or point closest on the segment) to the start point.</param>
        public void DistanceAtPoint(ref Vector3 point, out float distance)
        {
            Vector3.Subtract(ref point, ref StartPoint, out Vector3 v);
            
            Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 dir);
            dir.Normalize();
            
            Vector3.Dot(ref dir, ref v, out float dot);
            
            Vector3.Dot(ref dir, ref dir, out float dotDir);

            if (MathHelper.IsApproxEquals(dotDir, MathHelper.ZeroTolerance))
            {
                distance = 0.0f;
                return;
            }

            distance = dot / dotDir;
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
            DistanceTo(ref point, out float result);
            return result;
        }

        /// <summary>
        /// Determines the distance between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointSegment(ref point, ref this, out Vector3 ptOnSegment, out float segParam, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Ray ray)
        {
            GeometricToolsHelper.DistanceRaySegment(ref ray, ref this, out float rayParam, out float segParam, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Ray ray, out float result)
        {
            GeometricToolsHelper.DistanceRaySegment(ref ray, ref this, out float rayParam, out float segParam, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Segment segment)
        {
            GeometricToolsHelper.DistanceSegmentSegment(ref this, ref segment, out float seg0Param, out float seg1Param, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Segment segment, out float result)
        {
            GeometricToolsHelper.DistanceSegmentSegment(ref this, ref segment, out float seg0Param, out float seg1Param, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Plane plane)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref plane, ref this, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Plane plane, out float result)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref plane, ref this, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Triangle triangle)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Triangle triangle, out float result)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out float sqrDist);
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
            GeometricToolsHelper.DistancePointSegment(ref point, ref this, out Vector3 ptOnSegment, out float segParam, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointSegment(ref point, ref this, out Vector3 ptOnSegment, out float segParam, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Ray ray)
        {
            GeometricToolsHelper.DistanceRaySegment(ref ray, ref this, out float rayParam, out float segParam, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Ray ray, out float result)
        {
            GeometricToolsHelper.DistanceRaySegment(ref ray, ref this, out float rayParam, out float segParam, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Segment segment)
        {
            GeometricToolsHelper.DistanceSegmentSegment(ref this, ref segment, out float seg0Param, out float seg1Param, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Segment segment, out float result)
        {
            GeometricToolsHelper.DistanceSegmentSegment(ref this, ref segment, out float seg0Param, out float seg1Param, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Plane plane)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref plane, ref this, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Plane plane, out float result)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref plane, ref this, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Triangle triangle)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Triangle triangle, out float result)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out result);
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
            GeometricToolsHelper.DistancePointSegment(ref point, ref this, out Vector3 ptOnSegment, out float segParam, out float sqrDist);
            return ptOnSegment;
        }

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Closest point on this object.</param>
        public void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            GeometricToolsHelper.DistancePointSegment(ref point, ref this, out result, out float segParam, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Closest point on this object.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestPointTo(ref Vector3 point, out Vector3 result, out float squaredDistance)
        {
            GeometricToolsHelper.DistancePointSegment(ref point, ref this, out result, out float segParam, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Ray ray)
        {
            ClosestApproachSegment(ref ray, out Segment result);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Ray ray, out Segment result)
        {
            ClosestApproachSegment(ref ray, out result, out float squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Ray ray, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistanceRaySegment(ref ray, ref this, out float rayParam, out float segParam, out squaredDistance);

            // Calculate point along segment;
            Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 dir);
            dir.Normalize();

            Vector3.Multiply(ref dir, segParam, out result.StartPoint);
            Vector3.Add(ref result.StartPoint, ref StartPoint, out result.StartPoint);

            // Calculate point along ray
            Vector3.Multiply(ref ray.Direction, rayParam, out result.EndPoint);
            Vector3.Add(ref result.EndPoint, ref ray.Origin, out result.EndPoint);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Segment segment)
        {
            ClosestApproachSegment(ref segment, out Segment result, out float squaredDistance);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Segment segment, out Segment result)
        {
            ClosestApproachSegment(ref segment, out result, out float squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Segment segment, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistanceSegmentSegment(ref this, ref segment, out float seg0Param, out float seg1Param, out squaredDistance);

            // Calculate point along first segment
            Vector3.Subtract(ref EndPoint, ref StartPoint, out Vector3 dir);
            dir.Normalize();

            Vector3.Multiply(ref dir, seg0Param, out result.StartPoint);
            Vector3.Add(ref result.StartPoint, ref StartPoint, out result.StartPoint);

            // Calculate point along second segment
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out dir);
            dir.Normalize();

            Vector3.Multiply(ref dir, seg1Param, out dir);
            Vector3.Add(ref segment.StartPoint, ref dir, out result.EndPoint);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Plane plane)
        {
            Segment result;
            GeometricToolsHelper.DistanceSegmentPlane(ref plane, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Plane plane, out Segment result)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref plane, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Plane plane, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref plane, ref this, out result.EndPoint, out result.StartPoint, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Triangle triangle)
        {
            Segment result;
            GeometricToolsHelper.DistanceSegmentTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Triangle triangle, out Segment result)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Triangle triangle, out Segment result, float squaredDistance)
        {
            GeometricToolsHelper.DistanceSegmentTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out squaredDistance);
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
            if (obj is Segment)
            {
                return Equals((Segment)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the line and another line.
        /// </summary>
        /// <param name="other">Other line to test against</param>
        /// <returns>True if the lines are equal, false otherwise.</returns>
        public bool Equals(Segment other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the line and another line.
        /// </summary>
        /// <param name="other">Other line to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the lines are equal, false otherwise.</returns>
        public bool Equals(Segment other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the line and another line.
        /// </summary>
        /// <param name="other">Other line to test against</param>
        /// <returns>True if the lines are equal, false otherwise.</returns>
        public bool Equals(ref Segment other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the line and another line.
        /// </summary>
        /// <param name="other">Other line to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the lines are equal, false otherwise.</returns>
        public bool Equals(ref Segment other, float tolerance)
        {
            return StartPoint.Equals(ref other.StartPoint, tolerance) && EndPoint.Equals(ref other.EndPoint, tolerance);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return StartPoint.GetHashCode() + EndPoint.GetHashCode();
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
            return ToString("G");
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format)
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return ToString();
            }

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

            return string.Format(formatProvider, "Start: ({0}), End: ({1}), Length: {2}", new object[] { StartPoint.ToString(format, formatProvider), EndPoint.ToString(format, formatProvider), Length.ToString(format, formatProvider) });
        }

        #endregion

        #region IPrimitiveValue

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("Start", StartPoint);
            output.Write("End", EndPoint);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            StartPoint = input.Read<Vector3>();
            EndPoint = input.Read<Vector3>();
        }

        #endregion
    }
}
