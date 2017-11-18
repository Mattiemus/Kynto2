namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    using Content;

    /// <summary>
    /// Defines an infinite plane at an origin with a normal. The origin of the plane is represented by a distance value from zero, along the normal vector.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Plane : IEquatable<Plane>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// The normal vector of the plane.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The plane constant, which is the (negative) distance from the origin (0, 0, 0) to the origin of the plane along its normal.
        /// </summary>
        public float D;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="x">X component of the plane normal</param>
        /// <param name="y">Y component of the plane normal</param>
        /// <param name="z">Z component of the plane normal</param>
        /// <param name="d">Plane constant, (negative) distance from origin (0, 0, 0) to plane origin.</param>
        public Plane(float x, float y, float z, float d)
        {
            Normal = new Vector3(x, y, z);
            D = d;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="normal">Plane normal</param>
        /// <param name="d">Plane constant, (negative) distance from origin (0, 0, 0) to plane origin.</param>
        public Plane(Vector3 normal, float d)
        {
            Normal = normal;
            D = d;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="plane">XYZ contains the plane normal vector, and W contains the plane constant.</param>
        public Plane(Vector4 plane)
        {
            Normal = new Vector3(plane.X, plane.Y, plane.Z);
            D = plane.W;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="normal">Plane normal.</param>
        /// <param name="origin">Plane origin.</param>
        public Plane(Vector3 normal, Vector3 origin)
        {
            Vector3.Dot(ref normal, ref origin, out float dot);

            Normal = normal;
            D = -dot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct from three points.
        /// </summary>
        /// <param name="p1">First position</param>
        /// <param name="p2">Second position</param>
        /// <param name="p3">Third position</param>
        public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Compute first vector
            Vector3 v1;
            v1.X = p2.X - p1.X;
            v1.Y = p2.Y - p1.Y;
            v1.Z = p2.Z - p1.Z;

            // Compute second vector
            Vector3 v2;
            v2.X = p3.X - p1.X;
            v2.Y = p3.Y - p1.Y;
            v2.Z = p3.Z - p1.Z;

            // Take cross product
            Vector3.NormalizedCross(ref v1, ref v2, out Normal);

            D = -((p1.X * Normal.X) + (p1.Y * Normal.Y) + (p1.Z * Normal.Z));
        }

        /// <summary>
        /// Gets a unit <see cref="Plane"/> that has its origin at zero and normal set to (1, 0, 0).
        /// </summary>
        public static Plane UnitX => new Plane(Vector3.UnitX, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Plane"/> that has its origin at zero and normal set to (0, 1, 0).
        /// </summary>
        public static Plane UnitY => new Plane(Vector3.UnitY, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Plane"/> that has its origin at zero and normal set to (0, 0, 1).
        /// </summary>
        public static Plane UnitZ => new Plane(Vector3.UnitZ, 0.0f);

        /// <summary>
        /// Gets or sets the plane's origin.
        /// </summary>
        public Vector3 Origin
        {
            get
            {
                Vector3.Multiply(ref Normal, -D, out Vector3 origin);
                return origin;
            }
            set
            {
                Vector3.Dot(ref Normal, ref value, out float dot);
                D = -dot;
            }
        }

        /// <summary>
        /// Gets the size of Plane structure in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Plane>();

        /// <summary>
        /// Gets if the plane is degenerate (normal is zero).
        /// </summary>
        public bool IsDegenerate => Normal.Equals(Vector3.Zero);

        /// <summary>
        /// Gets whether any of the components of the plane are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(D) || Normal.IsNaN;

        /// <summary>
        /// Gets whether any of the components of the plane are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsNegativeInfinity(D) || float.IsPositiveInfinity(D) || Normal.IsInfinity;

        #region Public static methods

        /// <summary>
        /// Compute the dot product between the two specified planes (4D dot product where normals are XYZ and D is W).
        /// </summary>
        /// <param name="a">First plane</param>
        /// <param name="b">Second plane</param>
        /// <returns>Dot product</returns>
        public static float Dot(Plane a, Plane b)
        {
            Dot(ref a, ref b, out float result);
            return result;
        }

        /// <summary>
        /// Compute the dot product between the two specified planes (4D dot product where normals are XYZ and D is W).
        /// </summary>
        /// <param name="a">First plane</param>
        /// <param name="b">Second plane</param>
        /// <param name="result">Dot product</param>
        public static void Dot(ref Plane a, ref Plane b, out float result)
        {
            result = (a.Normal.X * b.Normal.X) + (a.Normal.Y * b.Normal.Y) + (a.Normal.Z * b.Normal.Z) + (a.D * b.D);
        }

        /// <summary>
        /// Compute the dot product between the specified plane's normal and vector plus the plane's constant.
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="value">Vector3</param>
        /// <returns>Dot product</returns>
        public static float DotCoordinate(Plane plane, Vector3 value)
        {
            DotCoordinate(ref plane, ref value, out float result);
            return result;
        }

        /// <summary>
        /// Compute the dot product between the specified plane's normal and vector plus the plane's constant.
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="value">Vector3</param>
        /// <param name="result">Dot product</param>
        public static void DotCoordinate(ref Plane plane, ref Vector3 value, out float result)
        {
            result = (plane.Normal.X * value.X) + (plane.Normal.Y * value.Y) + (plane.Normal.Z * value.Z) + plane.D;
        }

        /// <summary>
        /// Compute the dot product between the specified plane's normal and vector.
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="value">Vector</param>
        /// <returns>Dot product</returns>
        public static float DotNormal(Plane plane, Vector3 value)
        {
            DotNormal(ref plane, ref value, out float result);
            return result;
        }

        /// <summary>
        /// Compute the dot product between the specified plane's normal and vector.
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="value">Vector3</param>
        /// <param name="result">Dot product</param>
        public static void DotNormal(ref Plane plane, ref Vector3 value, out float result)
        {
            result = (plane.Normal.X * value.X) + (plane.Normal.Y * value.Y) + (plane.Normal.Z * value.Z);
        }

        /// <summary>
        /// Compute the dot product between the two specified plane's normals.
        /// </summary>
        /// <param name="a">First plane</param>
        /// <param name="b">Second plane</param>
        /// <returns>Dot product</returns>
        public static float DotNormal(Plane a, Plane b)
        {
            DotNormal(ref a, ref b, out float result);
            return result;
        }

        /// <summary>
        /// Compute the dot product between the two specified plane's normals.
        /// </summary>
        /// <param name="a">First plane</param>
        /// <param name="b">Second plane</param>
        /// <param name="result">Dot product</param>
        public static void DotNormal(ref Plane a, ref Plane b, out float result)
        {
            result = (a.Normal.X * b.Normal.X) + (a.Normal.Y * b.Normal.Y) + (a.Normal.Z * b.Normal.Z);
        }

        /// <summary>
        /// Normalizes the plane's normal to be unit length.
        /// </summary>
        /// <param name="plane">Plane to normalize.</param>
        /// <returns>Normalized plane.</returns>
        public static Plane Normalize(Plane plane)
        {
            Normalize(ref plane, out Plane result);
            return plane;
        }

        /// <summary>
        /// Normalizes the plane's normal to be unit length.
        /// </summary>
        /// <param name="plane">Plane to normalize.</param>
        /// <param name="result">Normalized plane.</param>
        public static void Normalize(ref Plane plane, out Plane result)
        {
            result = plane;

            float lengthSquared = (result.Normal.X * result.Normal.X) + (result.Normal.Y * result.Normal.Y) + (result.Normal.Z * result.Normal.Z);
            if(lengthSquared != 0.0f)
            {
                float invLength = 1.0f / (float) Math.Sqrt(lengthSquared);
                Vector3.Multiply(ref result.Normal, invLength, out result.Normal);
                result.D *= invLength;
            }
        }

        /// <summary>
        /// Reverses the plane's normal so it is pointing in the opposite direction.
        /// </summary>
        /// <param name="plane">Plane to reverse.</param>
        /// <returns>Reversed plane.</returns>
        public static Plane Negate(Plane plane)
        {
            Negate(ref plane, out Plane result);
            return result;
        }

        /// <summary>
        /// Reverses the plane's normal so it is pointing in the opposite direction.
        /// </summary>
        /// <param name="plane">Plane to reverse</param>
        /// <param name="result">Reversed plane.</param>
        public static void Negate(ref Plane plane, out Plane result)
        {
            result = plane;
            result.Negate();
        }
        
        /// <summary>
        /// Transforms the plane by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="plane">Plane to transform.</param>
        /// <param name="rotation">Quaternion rotation.</param>
        /// <returns>Transformed plane.</returns>
        public static Plane Transform(Plane plane, Quaternion rotation)
        {
            Transform(ref plane, ref rotation, out Plane result);
            return plane;
        }

        /// <summary>
        /// Transforms the plane by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="plane">Plane to transform.</param>
        /// <param name="rotation">Quaternion rotation.</param>
        /// <param name="result">Transformed plane.</param>
        public static void Transform(ref Plane plane, ref Quaternion rotation, out Plane result)
        {
            result.D = plane.D;
            Vector3.Transform(ref plane.Normal, ref rotation, out result.Normal);
        }

        /// <summary>
        /// Transforms the plane by the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="plane">Plane to transform.</param>
        /// <param name="transform">Transformation matrix.</param>
        /// <returns>Transformed plane.</returns>
        public static Plane Transform(Plane plane, Matrix4x4 transform)
        {
            Transform(ref plane, ref transform, out Plane result);
            return result;
        }

        /// <summary>
        /// Transforms the plane by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="plane">Plane to transform.</param>
        /// <param name="transform">Transformation matrix.</param>
        /// <param name="result">Transformed plane.</param>
        public static void Transform(ref Plane plane, ref Matrix4x4 transform, out Plane result)
        {
            Vector3 planeNormal = plane.Normal;
            Vector3 planeOrigin = plane.Origin;

            Vector3.Transform(ref planeOrigin, ref transform, out planeOrigin);
            Vector3.TransformNormal(ref planeNormal, ref transform, out planeNormal);

            result = new Plane(planeNormal, planeOrigin);
        }

        /// <summary>
        /// Translates the plane by the given translation vector.
        /// </summary>
        /// <param name="plane">Plane to transform.</param>
        /// <param name="translation">Translation vector.</param>
        /// <returns>Transformed plane.</returns>
        public static Plane Transform(Plane plane, Vector3 translation)
        {
            Transform(ref plane, ref translation, out Plane result);
            return result;
        }

        /// <summary>
        /// Translates the plane by the given translation vector.
        /// </summary>
        /// <param name="plane">Plane to transform.</param>
        /// <param name="translation">Translation vector.</param>
        /// <param name="result">Transformed plane.</param>
        public static void Transform(ref Plane plane, ref Vector3 translation, out Plane result)
        {
            Vector3 planeOrigin = plane.Origin;
            Vector3.Add(ref planeOrigin, ref translation, out planeOrigin);

            result = new Plane(plane.Normal, planeOrigin);
        }

        #endregion

        #region Equality Operators

        /// <summary>
        /// Checks equality between two planes.
        /// </summary>
        /// <param name="a">First plane</param>
        /// <param name="b">Second plane</param>
        /// <returns>True if the planes are equal, false otherwise.</returns>
        public static bool operator ==(Plane a, Plane b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Checks inequality between two planes.
        /// </summary>
        /// <param name="a">First plane</param>
        /// <param name="b">Second plane</param>
        /// <returns>True if the planes are not equal, false otherwise.</returns>
        public static bool operator !=(Plane a, Plane b)
        {
            return !a.Equals(ref b);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Reverses the plane's normal so it is pointing in the opposite direction.
        /// </summary>
        public void Negate()
        {
            Normal.Negate();
            D = -D;
        }

        /// <summary>
        /// Normalizes the plane's normal to be unit length.
        /// </summary>
        public void Normalize()
        {
            float lengthSquared = (Normal.X * Normal.X) + (Normal.Y * Normal.Y) + (Normal.Z * Normal.Z);
            if(lengthSquared != 0.0f)
            {
                float invLength = 1.0f / (float) Math.Sqrt(lengthSquared);
                Vector3.Multiply(ref Normal, invLength, out Normal);
                D *= invLength;
            }
        }

        /// <summary>
        /// Determines which side of the plane the point lies on.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>Which side of the plane the point lies on.</returns>
        public PlaneIntersectionType WhichSide(Vector3 point)
        {
            return WhichSide(ref point);
        }

        /// <summary>
        /// Determines which side of the plane the point lies on.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>Which side of the plane the point lies on.</returns>
        public PlaneIntersectionType WhichSide(ref Vector3 point)
        {
            Vector3.Dot(ref Normal, ref point, out float dot);

            float distance = dot + D;

            if (distance > 0.0f)
            {
                return PlaneIntersectionType.Front;
            }
            else if (distance < 0.0f)
            {
                return PlaneIntersectionType.Back;
            }

            return PlaneIntersectionType.Intersects;
        }

        /// <summary>
        /// Determines the signed distance between this object and a point. If negative, the point is on the
        /// back of the plane, if positive then on the front.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float SignedDistanceTo(Vector3 point)
        {
            SignedDistanceTo(ref point, out float result);
            return result;
        }

        /// <summary>
        /// Determines the signed distance between this object and a point. If negative, the point is on the
        /// back of the plane, if positive then on the front.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void SignedDistanceTo(ref Vector3 point, out float result)
        {
            float dot;
            Vector3.Dot(ref Normal, ref point, out dot);
            result = dot + D;
        }

        #endregion

        #region Intersects

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Ray ray)
        {
            return GeometricToolsHelper.IntersectRayPlane(ref ray, ref this);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool IntersectsXY(Ray ray, out LineIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectRayPlane(ref ray, ref this, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Ray ray)
        {
            return GeometricToolsHelper.IntersectRayPlane(ref ray, ref this);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Ray ray, out LineIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectRayPlane(ref ray, ref this, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Segment segment)
        {
            return GeometricToolsHelper.IntersectSegmentPlane(ref segment, ref this);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Segment segment, out LineIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectSegmentPlane(ref segment, ref this, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Segment segment)
        {
            return GeometricToolsHelper.IntersectSegmentPlane(ref segment, ref this);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Segment segment, out LineIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectSegmentPlane(ref segment, ref this, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Plane plane)
        {
            return GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Plane plane, out Ray result)
        {
            return GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Plane plane)
        {
            return GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Plane plane, out Ray result)
        {
            return GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Triangle triangle)
        {
            return GeometricToolsHelper.IntersectPlaneTriangle(ref triangle, ref this);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(Triangle triangle, out Segment result)
        {
            return GeometricToolsHelper.IntersectPlaneTriangle(ref triangle, ref this, out result);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Triangle triangle)
        {
            return GeometricToolsHelper.IntersectPlaneTriangle(ref triangle, ref this);
        }

        /// <summary>
        /// Determines whether there is an intersection between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Intersection result between the two objects.</param>
        /// <returns>True if an intersection between the two objects occurs, false otherwise.</returns>
        public bool Intersects(ref Triangle triangle, out Segment result)
        {
            return GeometricToolsHelper.IntersectPlaneTriangle(ref triangle, ref this, out result);
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
        /// <returns>Type of plane intersection.</returns>
        public PlaneIntersectionType Intersects(BoundingVolume volume)
        {
            if (volume == null)
            {
                return PlaneIntersectionType.Front;
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
            GeometricToolsHelper.DistancePointPlane(ref this, ref point, out Vector3 ptOnPlane, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointPlane(ref this, ref point, out Vector3 ptOnPlane, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Ray ray)
        {
            GeometricToolsHelper.DistanceRayPlane(ref this, ref ray, out Vector3 ptOnPlane, out Vector3 ptOnRay, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Ray ray, out float result)
        {
            GeometricToolsHelper.DistanceRayPlane(ref this, ref ray, out Vector3 ptOnPlane, out Vector3 ptOnRay, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Segment segment)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref this, ref segment, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out float sqrDist);
            return (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Segment segment, out float result)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref this, ref segment, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out float sqrDist);
            result = (float)Math.Sqrt(sqrDist);
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Plane plane)
        {
            DistanceTo(ref plane, out float result);
            return result;
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Plane plane, out float result)
        {
            Ray intr;
            if (GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane, out intr))
            {
                result = 0.0f;
            }
            else
            {
                Vector3 p0Origin = Origin;
                Vector3 p1Origin = plane.Origin;
                Vector3.Distance(ref p1Origin, ref p0Origin, out result);
            }
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>Distance between the two objects.</returns>
        public float DistanceTo(Triangle triangle)
        {
            DistanceTo(ref triangle, out float result);
            return result;
        }

        /// <summary>
        /// Determines the distance between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Distance between the two objects.</param>
        public void DistanceTo(ref Triangle triangle, out float result)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnPlane, out float sqrDist);
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
            GeometricToolsHelper.DistancePointPlane(ref this, ref point, out Vector3 ptOnPlane, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointPlane(ref this, ref point, out Vector3 ptOnPlane, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Ray ray)
        {
            GeometricToolsHelper.DistanceRayPlane(ref this, ref ray, out Vector3 ptOnPlane, out Vector3 ptOnRay, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Ray ray, out float result)
        {
            GeometricToolsHelper.DistanceRayPlane(ref this, ref ray, out Vector3 ptOnPlane, out Vector3 ptOnRay, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Segment segment)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref this, ref segment, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Segment segment, out float result)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref this, ref segment, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out result);
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Plane plane)
        {
            DistanceSquaredTo(ref plane, out float result);
            return result;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Plane plane, out float result)
        {
            if (GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane, out Ray intr))
            {
                result = 0.0f;
            }
            else
            {
                Vector3 p0Origin = Origin;
                Vector3 p1Origin = plane.Origin;
                Vector3.DistanceSquared(ref p1Origin, ref p0Origin, out result);
            }
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <returns>Distance squared between the two objects.</returns>
        public float DistanceSquaredTo(Triangle triangle)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnPlane, out float sqrDist);
            return sqrDist;
        }

        /// <summary>
        /// Determines the distance squared between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test.</param>
        /// <param name="result">Distance squared between the two objects.</param>
        public void DistanceSquaredTo(ref Triangle triangle, out float result)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref triangle, ref this, out Vector3 ptOnTriangle, out Vector3 ptOnPlane, out result);
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
            GeometricToolsHelper.DistancePointPlane(ref this, ref point, out Vector3 ptOnPlane, out float sqrDist);
            return ptOnPlane;
        }

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Closest point on this object.</param>
        public void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            GeometricToolsHelper.DistancePointPlane(ref this, ref point, out result, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest point on this object from the point.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="result">Closest point on this object.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestPointTo(ref Vector3 point, out Vector3 result, out float squaredDistance)
        {
            GeometricToolsHelper.DistancePointPlane(ref this, ref point, out result, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Ray ray)
        {
            Segment result;
            GeometricToolsHelper.DistanceRayPlane(ref this, ref ray, out result.StartPoint, out result.EndPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Ray ray, out Segment result)
        {
            GeometricToolsHelper.DistanceRayPlane(ref this, ref ray, out result.StartPoint, out result.EndPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">Ray to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Ray ray, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistanceRayPlane(ref this, ref ray, out result.StartPoint, out result.EndPoint, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Segment segment)
        {
            Segment result;
            GeometricToolsHelper.DistanceSegmentPlane(ref this, ref segment, out result.StartPoint, out result.EndPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Segment segment, out Segment result)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref this, ref segment, out result.StartPoint, out result.EndPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Segment"/>.
        /// </summary>
        /// <param name="segment">Segment to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Segment segment, out Segment result, out float squaredDistance)
        {
            GeometricToolsHelper.DistanceSegmentPlane(ref this, ref segment, out result.StartPoint, out result.EndPoint, out squaredDistance);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Plane plane)
        {
            Segment result;
            if (GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane, out Ray intr))
            {
                result.StartPoint = intr.Origin;
                result.EndPoint = intr.Origin;
            }
            else
            {
                result.StartPoint = Origin;
                result.EndPoint = plane.Origin;
            }

            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Plane plane, out Segment result)
        {
            if (GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane, out Ray intr))
            {
                result.StartPoint = intr.Origin;
                result.EndPoint = intr.Origin;
            }
            else
            {
                result.StartPoint = Origin;
                result.EndPoint = plane.Origin;
            }
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">Plane to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Plane plane, out Segment result, out float squaredDistance)
        {
            if (GeometricToolsHelper.IntersectPlanePlane(ref this, ref plane, out Ray intr))
            {
                result.StartPoint = intr.Origin;
                result.EndPoint = intr.Origin;
                squaredDistance = 0.0f;
            }
            else
            {
                result.StartPoint = Origin;
                result.EndPoint = plane.Origin;
                squaredDistance = result.Length;
                squaredDistance *= squaredDistance;
            }
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <returns>Closest approach segment between the two objects.</returns>
        public Segment ClosestApproachSegment(Triangle triangle)
        {
            Segment result;
            GeometricToolsHelper.DistancePlaneTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
            return result;
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        public void ClosestApproachSegment(ref Triangle triangle, out Segment result)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out float sqrDist);
        }

        /// <summary>
        /// Determines the closest approach segment between this object and a <see cref="Triangle"/>.
        /// </summary>
        /// <param name="triangle">Triangle to test</param>
        /// <param name="result">Closest approach segment between the two objects.</param>
        /// <param name="squaredDistance">Squared distance between the two objects.</param>
        public void ClosestApproachSegment(ref Triangle triangle, out Segment result, float squaredDistance)
        {
            GeometricToolsHelper.DistancePlaneTriangle(ref triangle, ref this, out result.EndPoint, out result.StartPoint, out squaredDistance);
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

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Plane other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(ref Plane other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(Plane other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Checks inequality between the plane and another plane.
        /// </summary>
        /// <param name="other">Other plane</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the planes are equal, false otherwise.</returns>
        public bool Equals(ref Plane other, float tolerance)
        {
            return Normal.Equals(ref other.Normal, tolerance) &&
                   (Math.Abs(D - other.D) <= tolerance);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Plane)
            {
                return Equals((Plane)obj);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Normal.GetHashCode() + D.GetHashCode();
            }
        }
        
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

            return string.Format(formatProvider, "Normal: {0}, D: {1}, Origin: {2}", new object[] { Normal.ToString(format, formatProvider), D.ToString(format, formatProvider), Origin.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("Normal", Normal);
            output.Write("D", D);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            Normal = input.Read<Vector3>();
            D = input.ReadSingle();
        }
    }
}
