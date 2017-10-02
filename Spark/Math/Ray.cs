namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a 3D unbounded line in space that has an origin, and a direction.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Ray : IEquatable<Ray>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// Origin of the ray.
        /// </summary>
        public Vector3 Origin;

        /// <summary>
        /// Direction of the ray.
        /// </summary>
        public Vector3 Direction;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Ray"/> struct.
        /// </summary>
        /// <param name="origin">Origin point of the ray.</param>
        /// <param name="direction">Direction of the ray.</param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
            Direction.Normalize();
        }

        /// <summary>
        /// Gets the size of the <see cref="Ray"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Ray>();

        /// <summary>
        /// Gets if the ray is degenerate (normal is zero).
        /// </summary>
        public bool IsDegenerate => Direction.Equals(Vector3.Zero);

        /// <summary>
        /// Gets whether any of the components of the ray are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => Origin.IsNaN || Direction.IsNaN;

        /// <summary>
        /// Gets whether any of the components of the ray are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => Origin.IsInfinity || Direction.IsInfinity;

        #region Normalize

        /// <summary>
        /// Normalizes the ray.
        /// </summary>
        /// <param name="ray">Ray to normalize.</param>
        /// <returns>Normalized ray.</returns>
        public static Ray Normalize(Ray ray)
        {
            Normalize(ref ray, out Ray result);
            return result;
        }

        /// <summary>
        /// Normalizes the ray.
        /// </summary>
        /// <param name="ray">Ray to normalize.</param>
        /// <param name="result">Normalized ray.</param>
        public static void Normalize(ref Ray ray, out Ray result)
        {
            result.Origin = ray.Origin;
            Vector3.Normalize(ref ray.Direction, out result.Direction);
        }

        #endregion

        #region Negate

        /// <summary>
        /// Negates the ray.
        /// </summary>
        /// <param name="ray">Ray to negate.</param>
        /// <returns>Reversed ray.</returns>
        public static Ray Negate(Ray ray)
        {
            Negate(ref ray, out Ray result);
            return result;
        }

        /// <summary>
        /// Negates the ray.
        /// </summary>
        /// <param name="ray">Ray to negate.</param>
        /// <param name="result">Reversed ray.</param>
        public static void Negate(ref Ray ray, out Ray result)
        {
            result.Origin = ray.Origin;
            Vector3.Negate(ref ray.Direction, out result.Direction);
        }

        #endregion

        #region From Methods

        /// <summary>
        /// Creates a ray from an origin point and a target point.
        /// </summary>
        /// <param name="origin">Origin of the ray</param>
        /// <param name="target">Target of the ray (target - origin defines direction of the ray)</param>
        /// <returns>The resulting ray</returns>
        public static Ray FromOriginTarget(Vector3 origin, Vector3 target)
        {
            FromOriginTarget(ref origin, ref target, out Ray result);
            return result;
        }

        /// <summary>
        /// Creates a ray from an origin point and a target point.
        /// </summary>
        /// <param name="origin">Origin of the ray</param>
        /// <param name="target">Target of the ray (target - origin defines direction of the ray)</param>
        /// <param name="result">The resulting ray</param>
        public static void FromOriginTarget(ref Vector3 origin, ref Vector3 target, out Ray result)
        {
            result.Origin = origin;
            Vector3.Subtract(ref target, ref origin, out result.Direction);
            result.Direction.Normalize();
        }

        /// <summary>
        /// Creates a ray from a line segment.
        /// </summary>
        /// <param name="line">Line segment</param>
        /// <returns>The resulting ray</returns>
        public static Ray FromSegment(Segment line)
        {
            FromSegment(ref line, out Ray result);
            return result;
        }

        /// <summary>
        /// Creates a ray from a line segment.
        /// </summary>
        /// <param name="line">Line segment</param>
        /// <param name="result">The resulting ray</param>
        public static void FromSegment(ref Segment line, out Ray result)
        {
            FromOriginTarget(ref line.StartPoint, ref line.EndPoint, out result);
        }

        #endregion

        #region Transform

        /// <summary>
        /// Transforms a ray by a SRT transformation matrix.
        /// </summary>
        /// <param name="ray">Ray to transform</param>
        /// <param name="transform">Transformation matrix</param>
        /// <returns>Transformed ray</returns>
        public static Ray Transform(Ray ray, Matrix4x4 transform)
        {
            Transform(ref ray, ref transform, out Ray result);
            return result;
        }

        /// <summary>
        /// Transforms a ray by a SRT transformation matrix.
        /// </summary>
        /// <param name="ray">Ray to transform</param>
        /// <param name="transform">Transformation matrix</param>
        /// <param name="result">Transformed ray</param>
        public static void Transform(ref Ray ray, ref Matrix4x4 transform, out Ray result)
        {
            Vector3.Transform(ref ray.Origin, ref transform, out Vector3 origin);
            Vector3.TransformNormal(ref ray.Direction, ref transform, out Vector3 direction);

            direction.Normalize();
            result.Origin = origin;
            result.Direction = direction;
        }

        /// <summary>
        /// Transforms a ray by a rotation.
        /// </summary>
        /// <param name="ray">Ray to be transformed.</param>
        /// <param name="rotation">Quaternion rotation.</param>
        /// <returns>Transformed ray.</returns>
        public static Ray Transform(Ray ray, Quaternion rotation)
        {
            Transform(ref ray, ref rotation, out Ray result);
            return result;
        }

        /// <summary>
        /// Transforms a ray by a rotation.
        /// </summary>
        /// <param name="ray">Ray to be transformed.</param>
        /// <param name="rotation">Quaternion rotation.</param>
        /// <param name="result">Transformed ray.</param>
        public static void Transform(ref Ray ray, ref Quaternion rotation, out Ray result)
        {
            Vector3.Transform(ref ray.Direction, ref rotation, out Vector3 direction);

            direction.Normalize();

            result.Origin = ray.Origin;
            result.Direction = direction;
        }

        /// <summary>
        /// Transforms a ray by a translation vector.
        /// </summary>
        /// <param name="ray">Ray to be transformed.</param>
        /// <param name="translation">Translation vector.</param>
        /// <returns>Transformed ray.</returns>
        public static Ray Transform(Ray ray, Vector3 translation)
        {
            Transform(ref ray, ref translation, out Ray result);
            return result;
        }

        /// <summary>
        /// Transforms a ray by a translation vector.
        /// </summary>
        /// <param name="ray">Ray to be transformed.</param>
        /// <param name="translation">Translation vector.</param>
        /// <param name="result">Transformed ray.</param>
        public static void Transform(ref Ray ray, ref Vector3 translation, out Ray result)
        {
            Vector3.Add(ref ray.Origin, ref translation, out Vector3 origin);

            result.Origin = origin;
            result.Direction = ray.Direction;
        }

        #endregion

        #region Equality Operators

        /// <summary>
        /// Tests equality between two rays.
        /// </summary>
        /// <param name="a">First ray</param>
        /// <param name="b">Second ray</param>
        /// <returns>True if the rays are equal, false otherwise.</returns>
        public static bool operator ==(Ray a, Ray b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two rays.
        /// </summary>
        /// <param name="a">First ray</param>
        /// <param name="b">Second ray</param>
        /// <returns>True if the rays are not equal, false otherwise.</returns>
        public static bool operator !=(Ray a, Ray b)
        {
            return !a.Equals(ref b);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Normalizes the ray's direction.
        /// </summary>
        public void Normalize()
        {
            Direction.Normalize();
        }

        /// <summary>
        /// Reverses the ray's direction.
        /// </summary>
        public void Negate()
        {
            Direction.Negate();
        }

        /// <summary>
        /// Tests if the ray is perpendicular to the specified ray. This assumes both rays are normalized.
        /// </summary>
        /// <param name="ray">Ray to test against</param>
        /// <returns>True if the rays are perpendicular to each other, false otherwise.</returns>
        public bool IsPerpendicularTo(Ray ray)
        {
            IsPerpendicularTo(ref ray, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the ray is perpendicular to the specified ray. This assumes both rays are normalized.
        /// </summary>
        /// <param name="ray">Ray to test against</param>
        /// <param name="result">True if the rays are perpendicular to each other, false otherwise.</param>
        public void IsPerpendicularTo(ref Ray ray, out bool result)
        {
            Vector3.Dot(ref Direction, ref ray.Direction, out float dot);
            result = MathHelper.IsApproxZero(dot);
        }

        /// <summary>
        /// Tests if the ray is parallel to the specified ray.
        /// </summary>
        /// <param name="ray">Ray to test against</param>
        /// <returns>True if the rays are parallel to each other, false otherwise.</returns>
        public bool IsParallelTo(Ray ray)
        {
            IsParallelTo(ref ray, out bool result);
            return result;
        }

        /// <summary>
        /// Tests if the ray is parallel to the specified ray.
        /// </summary>
        /// <param name="ray">Ray to test against</param>
        /// <param name="result">True if the rays are parallel to each other, false otherwise.</param>
        public void IsParallelTo(ref Ray ray, out bool result)
        {
            Vector3.NormalizedCross(ref Direction, ref ray.Direction, out Vector3 cross);
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
            return GeometricToolsHelper.IntersectRayRayXY(ref this, ref ray);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(Ray ray, out Vector3 result)
        {
            return GeometricToolsHelper.IntersectRayRayXY(ref this, ref ray, out result, out float ray0Param, out float ray1Param);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(ref Ray ray)
        {
            return GeometricToolsHelper.IntersectRayRayXY(ref this, ref ray);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(ref Ray ray, out Vector3 result)
        {
            return GeometricToolsHelper.IntersectRayRayXY(ref this, ref ray, out result, out float ray0Param, out float ray1Param);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(Segment segment)
        {
            return GeometricToolsHelper.IntersectRaySegmentXY(ref this, ref segment);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(Segment segment, out Vector3 result)
        {
            return GeometricToolsHelper.IntersectRaySegmentXY(ref this, ref segment, out result, out float rayParam, out float segParam);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(ref Segment segment)
        {
            return GeometricToolsHelper.IntersectRaySegmentXY(ref this, ref segment);
        }

        /// <summary>
        /// Determines whether there is an apparent 2D (XY only) intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(ref Segment segment, out Vector3 result)
        {
            return GeometricToolsHelper.IntersectRaySegmentXY(ref this, ref segment, out result, out float rayParam, out float segParam);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Plane plane)
        {
            return GeometricToolsHelper.IntersectRayPlane(ref this, ref plane);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Plane plane, out LineIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectRayPlane(ref this, ref plane, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Plane plane)
        {
            return GeometricToolsHelper.IntersectRayPlane(ref this, ref plane);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Plane plane, out LineIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectRayPlane(ref this, ref plane, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Triangle triangle, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectRayTriangle(ref triangle, ref this, ignoreBackface, out LineIntersectionResult result);
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
            return GeometricToolsHelper.IntersectRayTriangle(ref triangle, ref this, ignoreBackface, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="ignoreBackface">True if the test should ignore an intersection if the triangle is a back face, false if otherwise.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Triangle triangle, bool ignoreBackface = false)
        {
            return GeometricToolsHelper.IntersectRayTriangle(ref triangle, ref this, ignoreBackface, out LineIntersectionResult result);
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
            return GeometricToolsHelper.IntersectRayTriangle(ref triangle, ref this, ignoreBackface, out result);
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
        /// Gets the point along the ray that is the distance from the origin.
        /// </summary>
        /// <param name="distance">Distance of the point from the origin.</param>
        /// <returns>The point along the ray.</returns>
        public Vector3 PointAtDistance(float distance)
        {
            Vector3.Multiply(ref Direction, distance, out Vector3 result);
            Vector3.Add(ref Origin, ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the point along the ray that is the distance from the origin.
        /// </summary>
        /// <param name="distance">Distance of the point from the origin.</param>
        /// <param name="result">The point along the ray.</param>
        public void PointAtDistance(float distance, out Vector3 result)
        {
            Vector3.Multiply(ref Direction, distance, out result);
            Vector3.Add(ref Origin, ref result, out result);
        }

        /// <summary>
        /// Gets the distance of the point (or closest on the ray) from the origin.
        /// </summary>
        /// <param name="point">Point along or near the ray.</param>
        /// <returns>The distance from the point (or point closest on the ray) to the origin.</returns>
        public float DistanceAtPoint(Vector3 point)
        {
            DistanceAtPoint(ref point, out float result);
            return result;
        }

        /// <summary>
        /// Gets the distance of the point (or closest on the ray) from the origin.
        /// </summary>
        /// <param name="point">Point along or near the ray.</param>
        /// <param name="distance">The distance from the point (or point closest on the ray) to the origin.</param>
        public void DistanceAtPoint(ref Vector3 point, out float distance)
        {
            Vector3.Subtract(ref point, ref Origin, out Vector3 v);
            Vector3.Dot(ref Direction, ref v, out float dot);
            Vector3.Dot(ref Direction, ref Direction, out float dotDir);

            if (MathHelper.IsApproxZero(dotDir))
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
            GeometricToolsHelper.DistancePointRay(ref point, ref this, out Vector3 ptOnRay, out float rayParam, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Ray ray)
        {
            GeometricToolsHelper.DistanceRayRay(ref this, ref ray, out float ray0Param, out float ray1Param, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Ray ray, out float result)
        {
            GeometricToolsHelper.DistanceRayRay(ref this, ref ray, out float ray0Param, out float ray1Param, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Segment segment)
        {
            GeometricToolsHelper.DistanceRaySegment(ref this, ref segment, out float rayParam, out float segParam, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Segment segment, out float result)
        {
            GeometricToolsHelper.DistanceRaySegment(ref this, ref segment, out float rayParam, out float segParam, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Plane plane)
        {
            GeometricToolsHelper.DistanceRayPlane(ref plane, ref this, out Vector3 ptOnPlane, out Vector3 ptOnRay, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Plane plane, out float result)
        {
            GeometricToolsHelper.DistanceRayPlane(ref plane, ref this, out Vector3 ptOnPlane, out Vector3 ptOnRay, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Triangle triangle)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Triangle triangle, out float result)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out float sqrDist);
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
            GeometricToolsHelper.DistancePointRay(ref point, ref this, out Vector3 ptOnRay, out float rayParam, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointRay(ref point, ref this, out Vector3 ptOnRay, out float rayParam, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Ray ray)
        {
            GeometricToolsHelper.DistanceRayRay(ref this, ref ray, out float ray0Param, out float ray1Param, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Ray ray, out float result)
        {
            GeometricToolsHelper.DistanceRayRay(ref this, ref ray, out float ray0Param, out float ray1Param, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Segment segment)
        {
            GeometricToolsHelper.DistanceRaySegment(ref this, ref segment, out float rayParam, out float segParam, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Segment segment, out float result)
        {
            GeometricToolsHelper.DistanceRaySegment(ref this, ref segment, out float rayParam, out float segParam, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Plane plane)
        {
            GeometricToolsHelper.DistanceRayPlane(ref plane, ref this, out Vector3 ptOnPlane, out Vector3 ptOnRay, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Plane plane, out float result)
        {
            GeometricToolsHelper.DistanceRayPlane(ref plane, ref this, out Vector3 ptOnPlane, out Vector3 ptOnRay, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Triangle triangle)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Triangle triangle, out float result)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out result);
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
            GeometricToolsHelper.DistancePointRay(ref point, ref this, out Vector3 ptOnRay, out float rayParam, out float sqrDist);
            return ptOnRay;
        }

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Closest point on this object.</param>
        public void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            GeometricToolsHelper.DistancePointRay(ref point, ref this, out result, out float rayParam, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Closest point on this object.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestPointTo(ref Vector3 point, out Vector3 result, out float squaredDistance)
        {
            GeometricToolsHelper.DistancePointRay(ref point, ref this, out result, out float rayParam, out squaredDistance);
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
            GeometricToolsHelper.DistanceRayRay(ref this, ref ray, out float ray0Param, out float ray1Param, out squaredDistance);

            Vector3.Multiply(ref Direction, ray0Param, out result.StartPoint);
            Vector3.Add(ref result.StartPoint, ref Origin, out result.StartPoint);

            Vector3.Multiply(ref ray.Direction, ray1Param, out result.EndPoint);
            Vector3.Multiply(ref result.EndPoint, ref ray.Origin, out result.EndPoint);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Segment segment)
        {
            ClosestApproachSegment(ref segment, out Segment result);
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
            GeometricToolsHelper.DistanceRaySegment(ref this, ref segment, out float rayParam, out float segParam, out squaredDistance);

            Vector3.Multiply(ref Direction, rayParam, out result.StartPoint);
            Vector3.Add(ref result.StartPoint, ref Origin, out result.StartPoint);

            // Calculate point along segment
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out Vector3 dir);
            dir.Normalize();

            Vector3.Multiply(ref dir, segParam, out dir);
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
            GeometricToolsHelper.DistanceRayPlane(ref plane, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Plane plane, out Segment result)
        {
            GeometricToolsHelper.DistanceRayPlane(ref plane, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Plane plane, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistanceRayPlane(ref plane, ref this, out result.EndPoint, out result.StartPoint, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Triangle triangle)
        {
            Segment result;
            GeometricToolsHelper.DistanceRayTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Triangle triangle, out Segment result)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Triangle triangle, out Segment result, float squaredDistance)
        {
            GeometricToolsHelper.DistanceRayTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out squaredDistance);
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
        /// <returns>
        /// true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (obj is Ray)
            {
                return Equals((Ray)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the ray and another ray.
        /// </summary>
        /// <param name="other">Other ray to test</param>
        /// <returns>True if the rays are equal, false otherwise</returns>
        public bool Equals(Ray other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the ray and another ray.
        /// </summary>
        /// <param name="other">Other ray to test</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the rays are equal, false otherwise</returns>
        public bool Equals(Ray other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the ray and another ray.
        /// </summary>
        /// <param name="other">Other ray to test</param>
        /// <returns>True if the rays are equal, false otherwise</returns>
        public bool Equals(ref Ray other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the ray and another ray.
        /// </summary>
        /// <param name="other">Other ray to test</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the rays are equal, false otherwise</returns>
        public bool Equals(ref Ray other, float tolerance)
        {
            return Origin.Equals(ref other.Origin, tolerance) && Direction.Equals(ref other.Direction, tolerance);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Origin.GetHashCode() + Direction.GetHashCode();
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

            return string.Format(formatProvider, "Origin: ({0}), Direction: ({1})", new object[] { Origin.ToString(format, formatProvider), Direction.ToString(format, formatProvider) });
        }

        #endregion

        #region IPrimitiveValue

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("Origin", Origin);
            output.Write("Direction", Direction);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            Origin = input.Read<Vector3>();
            Direction = input.Read<Vector3>();
        }

        #endregion
    }
}
