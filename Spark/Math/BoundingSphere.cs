namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    
    using Content;
    using Graphics;

    /// <summary>
    /// Represents a Bounding Sphere. The sphere is defined by a center point and a radius.
    /// </summary>
    public sealed class BoundingSphere : BoundingVolume
    {
        private Vector3 _center;
        private float _radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingSphere"/> class.
        /// </summary>
        public BoundingSphere()
        {
            _center = Vector3.Zero;
            _radius = 0.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingSphere"/> class.
        /// </summary>
        /// <param name="center">Center of the bounding sphere.</param>
        /// <param name="radius">Radius of the bounding sphere.</param>
        public BoundingSphere(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingSphere"/> class.
        /// </summary>
        /// <param name="boundingSphereData">Bounding sphere data.</param>
        public BoundingSphere(Data boundingSphereData)
        {
            _center = boundingSphereData.Center;
            _radius = boundingSphereData.Radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingSphere"/> class.
        /// </summary>
        /// <param name="boundingSphere">Bounding sphere.</param>
        public BoundingSphere(BoundingSphere boundingSphere)
        {
            if (boundingSphere == null)
            {
                throw new ArgumentNullException(nameof(boundingSphere));
            }

            _center = boundingSphere.Center;
            _radius = boundingSphere.Radius;
        }

        /// <summary>
        /// Gets or sets the center of the bounding volume.
        /// </summary>
        public override Vector3 Center
        {
            get => _center;
            set
            {
                _center = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the bounding sphere.
        /// </summary>
        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets the volume of the bounding volume.
        /// </summary>
        public override float Volume => 4.0f * MathHelper.OneThird * MathHelper.Pi * _radius * _radius * _radius;

        /// <summary>
        /// Gets the number of corners.
        /// </summary>
        public override int CornerCount => 8;

        /// <summary>
        /// Gets the bounding type.
        /// </summary>
        public override BoundingType BoundingType => BoundingType.Sphere;

        /// <summary>
        /// Extends the bounding volume to include the specified point.
        /// </summary>
        /// <param name="point">Point to contain.</param>
        public void Extend(Vector3 point)
        {
            Extend(ref point);
        }

        /// <summary>
        /// Extends the bounding volume to include the specified point.
        /// </summary>
        /// <param name="point">Point to contain.</param>
        public void Extend(ref Vector3 point)
        {
            // Average center and point
            Vector3.Add(ref _center, ref point, out _center);
            Vector3.Multiply(ref _center, 0.5f, out _center);

            // Get max radius
            Vector3.Subtract(ref point, ref _center, out Vector3 diff);
            double testRadius = diff.LengthSquared();
            if (testRadius > (_radius * _radius))
            {
                _radius = (float)Math.Sqrt(testRadius);
            }

            UpdateCorners = true;
        }

        /// <summary>
        /// Extends the bounding volume to include the specified points.
        /// </summary>
        /// <param name="points">Points to contain.</param>
        public void Extend(IReadOnlyDataBuffer<Vector3> points)
        {
            if (points == null || points.Length == 0)
            {
                return;
            }

            // Average center and points
            int numPts = points.Length;
            for (int i = 0; i < numPts; i++)
            {
                Vector3 pt = points[i];
                Vector3.Add(ref _center, ref pt, out _center);
            }

            Vector3.Multiply(ref _center, 1.0f / (float)numPts, out _center);

            // Get max radius
            double radiusSqr = _radius * _radius;

            for (int i = 0; i < numPts; i++)
            {
                Vector3 pt = points[i];
                Vector3.Subtract(ref pt, ref _center, out Vector3 diff);
                double testRadius = diff.LengthSquared();
                if (testRadius > radiusSqr)
                {
                    radiusSqr = testRadius;
                }
            }

            _radius = (float)Math.Sqrt(radiusSqr);
            UpdateCorners = true;
        }

        /// <summary>
        /// Extends the bounding volume to include the specified points.
        /// </summary>
        /// <param name="points">Points to contain.</param>
        public void Extend(Vector3[] points)
        {
            if (points == null || points.Length == 0)
            {
                return;
            }

            // Average center and points
            int numPts = points.Length;
            for (int i = 0; i < numPts; i++)
            {
                Vector3 pt = points[i];
                Vector3.Add(ref _center, ref pt, out _center);
            }

            Vector3.Multiply(ref _center, 1.0f / (float)numPts, out _center);

            // Get max radius
            double radiusSqr = _radius * _radius;

            for (int i = 0; i < numPts; i++)
            {
                Vector3 pt = points[i];
                Vector3.Subtract(ref pt, ref _center, out Vector3 diff);
                double testRadius = diff.LengthSquared();
                if (testRadius > radiusSqr)
                {
                    radiusSqr = testRadius;
                }
            }

            _radius = (float)Math.Sqrt(radiusSqr);
            UpdateCorners = true;
        }

        /// <summary>
        /// Sets the bounding volume by either copying the specified bounding volume if its the specified type, or computing a volume required to fully contain it.
        /// </summary>
        /// <param name="volume">Bounding volume to copy from</param>
        public override void Set(BoundingVolume volume)
        {
            if (volume == null)
            {
                return;
            }

            switch (volume.BoundingType)
            {
                case BoundingType.AxisAlignedBoundingBox:
                    {
                        BoundingBox other = volume as BoundingBox;
                        _center = other.Center;
                        _radius = other.Extents.Length();
                    }
                    break;

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        _center = other._center;
                        _radius = other._radius;
                    }
                    break;

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        _center = other.Center;
                        _radius = other.CenterLine.Extent + other.Radius;
                    }
                    break;

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        _center = other.Center;

                        Vector3 extents = other.Extents;
                        Triad axes = other.Axes;

                        Vector3.Multiply(ref axes.XAxis, extents.X, out axes.XAxis);
                        Vector3.Multiply(ref axes.YAxis, extents.Y, out axes.YAxis);
                        Vector3.Multiply(ref axes.ZAxis, extents.Z, out axes.ZAxis);
                        
                        Vector3.Add(ref axes.XAxis, ref axes.YAxis, out Vector3 diagonal);
                        Vector3.Add(ref diagonal, ref axes.ZAxis, out diagonal);

                        _radius = diagonal.Length();
                    }
                    break;

                default:
                    {
                        ComputeFromPoints(volume.Corners, null);
                    }
                    break;
            }

            UpdateCorners = true;
        }

        /// <summary>
        /// Sets the bounding sphere from the specified data.
        /// </summary>
        /// <param name="boundingSphereData">Bounding sphere data.</param>
        public void Set(Data boundingSphereData)
        {
            _center = boundingSphereData.Center;
            _radius = boundingSphereData.Radius;
            UpdateCorners = true;
        }

        /// <summary>
        /// Sets the bounding sphere from the specified data.
        /// </summary>
        /// <param name="center">Center of the bounding sphere.</param>
        /// <param name="radius">Radius of the bounding sphere.</param>
        public void Set(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
            UpdateCorners = true;
        }

        /// <summary>
        /// Creates a copy of the bounding volume and returns a new instance.
        /// </summary>
        /// <returns>Copied bounding volume</returns>
        public override BoundingVolume Clone()
        {
            return new BoundingSphere(this);
        }

        /// <summary>
        /// Computes the distance from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distance from the point to the edge of the volume.</param>
        public override void DistanceTo(ref Vector3 point, out float result)
        {
            Vector3.Distance(ref _center, ref point, out float dist);
            result = Math.Max(0.0f, dist - _radius);
        }

        /// <summary>
        /// Computes the distance squared from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distanced squared from the point to the edge of the volume.</param>
        public override void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            Vector3.Distance(ref _center, ref point, out float dist);

            result = Math.Max(0.0f, dist - _radius);
            result = result * result;
        }

        /// <summary>
        /// Computes the closest point on the volume from the given point in space.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Closest point on the edge of the volume.</param>
        public override void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            Vector3.Subtract(ref point, ref _center, out Vector3 v);
            v.Normalize();

            Vector3.Multiply(ref v, _radius, out v);
            Vector3.Add(ref _center, ref v, out result);
        }

        /// <summary>
        /// Computes a minimum bounding volume that encloses the specified points in space.
        /// </summary>
        /// <param name="points">Points in space</param>
        /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
        public override void ComputeFromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
        {
            // TODO - Take a second look at Min sphere computation
            Data.FromAveragePoints(points, subMeshRange, out Data data);

            _center = data.Center;
            _radius = data.Radius;
            UpdateCorners = true;
        }

        /// <summary>
        /// Computes a minimum bounding volume that encloses the specified indexed points in space.
        /// </summary>
        /// <param name="points">Points in space.</param>
        /// <param name="indices">Point indices denoting location in the point buffer.</param>
        /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for.</param>
        public override void ComputeFromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange)
        {
            // TODO - Take a second look at Min sphere computation
            Data.FromAverageIndexedPoints(points, indices, subMeshRange, out Data data);

            _center = data.Center;
            _radius = data.Radius;
            UpdateCorners = true;
        }

        /// <summary>
        /// Determines if the specified point is contained inside the bounding volume.
        /// </summary>
        /// <param name="point">Point to test against.</param>
        /// <returns>Type of containment</returns>
        public override ContainmentType Contains(ref Vector3 point)
        {
            Vector3.DistanceSquared(ref _center, ref point, out float distSquared);

            if (distSquared <= _radius * _radius)
            {
                return ContainmentType.Inside;
            }

            return ContainmentType.Outside;
        }

        /// <summary>
        /// Determines if the specified segment line is contained inside the bounding volume.
        /// </summary>
        /// <param name="line">Segment to test against.</param>
        /// <returns>Type of containment.</returns>
        public override ContainmentType Contains(ref Segment line)
        {
            Vector3.DistanceSquared(ref _center, ref line.StartPoint, out float distStart);
            Vector3.DistanceSquared(ref _center, ref line.EndPoint, out float distEnd);

            float radiusSquared = _radius * _radius;

            if (distStart <= radiusSquared && distEnd <= radiusSquared)
            {
                return ContainmentType.Inside;
            }

            if (GeometricToolsHelper.IntersectSegmentSphere(ref line, ref _center, _radius))
            {
                return ContainmentType.Intersects;
            }

            return ContainmentType.Outside;
        }

        /// <summary>
        /// Determines if the specified triangle is contained inside the bounding volume.
        /// </summary>
        /// <param name="triangle">Triangle to test against.</param>
        /// <returns>Type of containment.</returns>
        public override ContainmentType Contains(ref Triangle triangle)
        {
            Vector3.DistanceSquared(ref _center, ref triangle.PointA, out float distA);
            Vector3.DistanceSquared(ref _center, ref triangle.PointB, out float distB);
            Vector3.DistanceSquared(ref _center, ref triangle.PointC, out float distC);

            float radiusSquared = _radius * _radius;

            if (distA <= radiusSquared && distB <= radiusSquared && distC <= radiusSquared)
            {
                return ContainmentType.Inside;
            }

            if (GeometricToolsHelper.IntersectTriangleSphere(ref triangle, ref _center, _radius))
            {
                return ContainmentType.Intersects;
            }

            return ContainmentType.Outside;
        }

        /// <summary>
        /// Determines if the specified ellipse intersects with the bounding volume.
        /// </summary>
        /// <param name="ellipse">Ellipse to test against.</param>
        /// <returns>True if the bounding volume intersects with the ellipse, false otherwise.</returns>
        public override ContainmentType Contains(ref Ellipse ellipse)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines if the specified bounding volume is contained inside the bounding volume.
        /// </summary>
        /// <param name="volume">Bounding volume to test against.</param>
        /// <returns>Type of containment.</returns>
        public override ContainmentType Contains(BoundingVolume volume)
        {
            if (volume == null)
            {
                return ContainmentType.Outside;
            }

            switch (volume.BoundingType)
            {
                case BoundingType.AxisAlignedBoundingBox:
                    {
                        BoundingBox other = volume as BoundingBox;
                        Vector3 boxCenter = other.Center;
                        Vector3 boxExtents = other.Extents;

                        return GeometricToolsHelper.SphereContainsAABB(ref _center, _radius, ref boxCenter, ref boxExtents);
                    }

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;

                        return GeometricToolsHelper.SphereContainsSphere(ref _center, _radius, ref other._center, other._radius);
                    }

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;
                        float capsuleRadius = other.Radius;

                        return GeometricToolsHelper.SphereContainsCapsule(ref _center, _radius, ref centerLine, capsuleRadius);
                    }

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        Vector3 boxCenter = other.Center;
                        Triad boxAxes = other.Axes;
                        Vector3 boxExtents = other.Extents;

                        return GeometricToolsHelper.SphereContainsBox(ref _center, _radius, ref boxCenter, ref boxAxes.XAxis, ref boxAxes.YAxis, ref boxAxes.ZAxis, ref boxExtents);
                    }

                default:
                    {
                        return Contains(volume.Corners);
                    }
            }
        }

        /// <summary>
        /// Tests if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public override bool Intersects(ref Ray ray)
        {
            return GeometricToolsHelper.IntersectRaySphere(ref ray, ref _center, _radius);
        }

        /// <summary>
        /// Determines if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public override bool Intersects(ref Ray ray, out BoundingIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectRaySphere(ref ray, ref _center, _radius, out result);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line)
        {
            return GeometricToolsHelper.IntersectSegmentSphere(ref line, ref _center, _radius);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line, out BoundingIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectSegmentSphere(ref line, ref _center, _radius, out result);
        }

        /// <summary>
        /// Tests if the specified plane intersects with the bounding volume.
        /// </summary>
        /// <param name="plane">Plane to test against.</param>
        /// <returns>Type of plane intersection.</returns>
        public override PlaneIntersectionType Intersects(ref Plane plane)
        {
            plane.SignedDistanceTo(ref _center, out float signedDistance);

            if (signedDistance > _radius)
            {
                return PlaneIntersectionType.Front;
            }
            else if (signedDistance < -_radius)
            {
                return PlaneIntersectionType.Back;
            }
            else
            {
                return PlaneIntersectionType.Intersects;
            }
        }

        /// <summary>
        /// Determines if the specified bounding volume intersects with the bounding volume.
        /// </summary>
        /// <param name="volume">Other bounding volume to test against.</param>
        /// <returns>True if the two volumes intersect with one another, false otherwise.</returns>
        public override bool Intersects(BoundingVolume volume)
        {
            if (volume == null)
            {
                return false;
            }

            switch (volume.BoundingType)
            {
                case BoundingType.AxisAlignedBoundingBox:
                    {
                        BoundingBox other = volume as BoundingBox;
                        Vector3 boxCenter = other.Center;
                        Vector3 boxExtents = other.Extents;

                        return GeometricToolsHelper.IntersectSphereAABB(ref _center, _radius, ref boxCenter, ref boxExtents);
                    }

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;

                        return GeometricToolsHelper.IntersectSphereSphere(ref _center, _radius, ref other._center, other._radius);
                    }

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;
                        float capsuleRadius = other.Radius;

                        return GeometricToolsHelper.IntersectCapsuleSphere(ref centerLine, capsuleRadius, ref _center, _radius);
                    }

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        Vector3 boxCenter = other.Center;
                        Triad boxAxes = other.Axes;
                        Vector3 boxExtents = other.Extents;

                        return GeometricToolsHelper.IntersectSphereBox(ref _center, _radius, ref boxCenter, ref boxAxes.XAxis, ref boxAxes.YAxis, ref boxAxes.ZAxis, ref boxExtents);
                    }

                case BoundingType.Frustum:
                case BoundingType.Mesh:
                    {
                        return volume.Intersects(this);
                    }

                default:
                    return IntersectsGeneral(volume);
            }
        }

        /// <summary>
        /// Merges the bounding volume with the specified bounding volume, resulting in a volume that encloses both.
        /// </summary>
        /// <param name="volume">Bounding volume to merge with.</param>
        public override void Merge(BoundingVolume volume)
        {
            if (volume == null)
            {
                return;
            }

            Data s0;
            s0.Center = _center;
            s0.Radius = _radius;

            switch (volume.BoundingType)
            {
                case BoundingType.AxisAlignedBoundingBox:
                    {
                        BoundingBox other = volume as BoundingBox;

                        Data s1;
                        s1.Center = other.Center;
                        s1.Radius = other.Extents.Length();
                        
                        GeometricToolsHelper.MergeSphereSphere(ref s0, ref s1, out Data merged);

                        _center = merged.Center;
                        _radius = merged.Radius;
                    }
                    break;

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;

                        Data s1;
                        s1.Center = other.Center;
                        s1.Radius = other.Radius;
                        
                        GeometricToolsHelper.MergeSphereSphere(ref s0, ref s1, out Data merged);

                        _center = merged.Center;
                        _radius = merged.Radius;
                    }
                    break;

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;

                        // Treat capsule as a sphere
                        Data b1;
                        b1.Center = other.Center;
                        b1.Radius = other.CenterLine.Extent + other.Radius;

                        Data b0;
                        b0.Center = _center;
                        b0.Radius = _radius;
                        
                        GeometricToolsHelper.MergeSphereSphere(ref b0, ref b1, out Data merged);

                        _center = merged.Center;
                        _radius = merged.Radius;
                    }
                    break;

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;

                        OrientedBoundingBox.Data b1;
                        b1.Center = other.Center;
                        b1.Axes = other.Axes;
                        b1.Extents = other.Extents;
                        GeometricToolsHelper.MergeSphereBox(ref s0, ref b1, out Data merged);

                        _center = merged.Center;
                        _radius = merged.Radius;
                    }
                    break;
                    
                default:
                    {
                        Data.FromPoints(volume.Corners, out Data s1);
                        GeometricToolsHelper.MergeSphereSphere(ref s0, ref s1, out Data merged);

                        _center = merged.Center;
                        _radius = merged.Radius;
                    }
                    break;
            }

            UpdateCorners = true;
        }

        /// <summary>
        /// Transforms the bounding volume by a Scale-Rotation-Translation (SRT) transform.
        /// </summary>
        /// <param name="scale">Scaling</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="translation">Translation</param>
        public override void Transform(ref Vector3 scale, ref Quaternion rotation, ref Vector3 translation)
        {
            Vector3.Transform(ref _center, ref rotation, out _center);
            Vector3.Add(ref _center, ref translation, out _center);

            float maxScale = Math.Max(Math.Abs(scale.X), Math.Max(Math.Abs(scale.Y), Math.Abs(scale.Z)));

            _radius = _radius * maxScale;
            UpdateCorners = true;
        }

        /// <summary>
        /// Tests equality between the bounding volume and the other bounding volume.
        /// </summary>
        /// <param name="other">Other bounding volume</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the volume is of same type and same size/shape, false otherwise.</returns>
        public override bool Equals(BoundingVolume other, float tolerance)
        {
            BoundingSphere bs = other as BoundingSphere;
            if (bs != null)
            {
                return _center.Equals(ref bs._center, tolerance) && MathHelper.IsApproxEquals(_radius, bs._radius, tolerance);
            }

            return false;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            _center = input.Read<Vector3>();
            _radius = input.ReadSingle();
            UpdateCorners = true;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            output.Write("Center", _center);
            output.Write("Radius", _radius);
        }

        /// <summary>
        /// Computes the corners that represent the extremal points of this bounding volume.
        /// </summary>
        /// <param name="corners">Databuffer to contain the points, length equal to the corner count.</param>
        protected override void ComputeCorners(IDataBuffer<Vector3> corners)
        {
            if (corners == null)
            {
                throw new ArgumentNullException(nameof(corners));
            }

            if (!corners.HasNextFor(CornerCount))
            {
                throw new ArgumentOutOfRangeException(nameof(corners), "Not enough space for bounding corners");
            }

            Vector3 max = new Vector3(_center.X + _radius, _center.Y + _radius, _center.Z + _radius);
            Vector3 min = new Vector3(_center.X - _radius, _center.Y - _radius, _center.Z - _radius);

            corners.Set(new Vector3(min.X, max.Y, max.Z));
            corners.Set(new Vector3(max.X, max.Y, max.Z));
            corners.Set(new Vector3(max.X, min.Y, max.Z));
            corners.Set(new Vector3(min.X, min.Y, max.Z));
            corners.Set(new Vector3(min.X, max.Y, min.Z));
            corners.Set(new Vector3(max.X, max.Y, min.Z));
            corners.Set(new Vector3(max.X, min.Y, min.Z));
            corners.Set(new Vector3(min.X, min.Y, min.Z));
        }

        #region BoundingSphere Data

        /// <summary>
        /// Represents data for a <see cref="BoundingSphere"/>.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct Data : IEquatable<Data>, IFormattable, IPrimitiveValue
        {
            /// <summary>
            /// Center of the bounding sphere.
            /// </summary>
            public Vector3 Center;

            /// <summary>
            /// Radius of the bounding sphere.
            /// </summary>
            public float Radius;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="center">Center of the bounding sphere.</param>
            /// <param name="radius">Radius of the bounding sphere.</param>
            public Data(Vector3 center, float radius)
            {
                Center = center;
                Radius = radius;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="sphere">Bounding sphere to copy from</param>
            public Data(BoundingSphere sphere)
            {
                Center = sphere._center;
                Radius = sphere._radius;
            }

            /// <summary>
            /// Determines if the AABB intersects with another.
            /// </summary>
            /// <param name="box">AABB to test against.</param>
            /// <returns>True if the objects intersect, false otherwise.</returns>
            public bool Intersects(ref BoundingBox.Data box)
            {
                return GeometricToolsHelper.IntersectSphereAABB(ref Center, Radius, ref box.Center, ref box.Extents);
            }

            /// <summary>
            /// Determines if the AABB intersects with a sphere.
            /// </summary>
            /// <param name="sphere">Sphere to test against</param>
            /// <returns>True if the objects intersect, false otherwise.</returns>
            public bool Intersects(ref Data sphere)
            {
                return GeometricToolsHelper.IntersectSphereSphere(ref Center, Radius, ref sphere.Center, Radius);
            }

            /// <summary>
            /// Computes a bounding volume that encloses the specified points in space by averaging the points.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <returns>Computed bounding sphere.</returns>
            public static Data FromAveragePoints(IReadOnlyDataBuffer<Vector3> points)
            {
                FromAveragePoints(points, null, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a bounding volume that encloses the specified points in space by averaging the points.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
            /// <returns>Computed bounding sphere.</returns>
            public static Data FromAveragePoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
            {
                FromAveragePoints(points, subMeshRange, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a bounding volume that encloses the specified points in space by averaging the points.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="result">Computed bounding sphere.</param>
            public static void FromAveragePoints(IReadOnlyDataBuffer<Vector3> points, out Data result)
            {
                FromAveragePoints(points, null, out result);
            }

            /// <summary>
            /// Computes a bounding volume that encloses the specified points in space by averaging the points.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
            /// <param name="result">Computed bounding sphere.</param>
            public static void FromAveragePoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange, out Data result)
            {
                if (!ExtractSubMeshRange(points, subMeshRange, out int offset, out int count))
                {
                    result = new Data();
                    return;
                }

                result.Center = points[offset];

                int upperBoundExclusive = offset + count;
                for (int i = offset + 1; i < upperBoundExclusive; i++)
                {
                    Vector3 pt = points[i];
                    Vector3.Add(ref result.Center, ref pt, out result.Center);
                }

                Vector3.Multiply(ref result.Center, 1.0f / count, out result.Center);

                float radiusSqr = 0.0f;
                for (int i = offset; i < upperBoundExclusive; i++)
                {
                    Vector3 pt = points[i];

                    Vector3.Subtract(ref pt, ref result.Center, out Vector3 diff);
                    float radiusTest = diff.LengthSquared();
                    if (radiusTest > radiusSqr)
                    {
                        radiusSqr = radiusTest;
                    }
                }

                result.Radius = (float)Math.Sqrt(radiusSqr);
            }

            /// <summary>
            /// Computes a bounding volume that encloses the specified indexed points in space by averaging the points.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <returns>Computed bounding sphere.</returns>
            public static Data FromAverageIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices)
            {
                FromAverageIndexedPoints(points, indices, null, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a bounding volume that encloses the specified indexed points in space by averaging the points.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for.</param>
            /// <returns>Computed bounding sphere.</returns>
            public static Data FromAverageIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange)
            {
                FromAverageIndexedPoints(points, indices, subMeshRange, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a bounding volume that encloses the specified indexed points in space by averaging the points.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <param name="result">Computed bounding sphere.</param>
            public static void FromAverageIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, out Data result)
            {
                FromAverageIndexedPoints(points, indices, null, out result);
            }

            /// <summary>
            /// Computes a bounding volume that encloses the specified indexed points in space by averaging the points.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for.</param>
            /// <param name="result">Computed bounding sphere.</param>
            public static void FromAverageIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange, out Data result)
            {
                if (!ExtractSubMeshRange(points, indices, subMeshRange, out int offset, out int count, out int baseVertexOffset))
                {
                    result = new Data();
                    return;
                }

                int index = indices[offset];
                result.Center = points[index + baseVertexOffset];

                int upperBoundExclusive = offset + count;
                for (int i = offset + 1; i < upperBoundExclusive; i++)
                {
                    index = indices[i];
                    Vector3 pt = points[index + baseVertexOffset];
                    Vector3.Add(ref result.Center, ref pt, out result.Center);
                }

                Vector3.Multiply(ref result.Center, 1.0f / (float)count, out result.Center);

                float radiusSqr = 0.0f;
                for (int i = offset; i < upperBoundExclusive; i++)
                {
                    index = indices[i];
                    Vector3 pt = points[index + baseVertexOffset];

                    Vector3.Subtract(ref pt, ref result.Center, out Vector3 diff);
                    float radiusTest = diff.LengthSquared();
                    if (radiusTest > radiusSqr)
                    {
                        radiusSqr = radiusTest;
                    }
                }

                result.Radius = (float)Math.Sqrt(radiusSqr);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <returns>Computed bounding sphere.</returns>
            public static Data FromPoints(IReadOnlyDataBuffer<Vector3> points)
            {
                FromPoints(points, null, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
            /// <returns>Computed bounding sphere.</returns>
            public static Data FromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
            {
                FromPoints(points, subMeshRange, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="result">Computed bounding sphere.</param>
            public static void FromPoints(IReadOnlyDataBuffer<Vector3> points, out Data result)
            {
                FromPoints(points, null, out result);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
            /// <param name="result">Computed bounding sphere.</param>
            public static void FromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange, out Data result)
            {
                if (!ExtractSubMeshRange(points, subMeshRange, out int offset, out int count))
                {
                    result = new Data();
                    return;
                }

                using (MappedDataBuffer mPtr = points.Map())
                {
                    IntPtr posPtr = mPtr + offset;
                    unsafe
                    {
                        ComputeMinSphere((Vector3*)posPtr.ToPointer(), count, null, 0, out result);
                    }
                }
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <returns>Computed bounding sphere.</returns>
            public static Data FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices)
            {
                FromIndexedPoints(points, indices, null, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for.</param>
            /// <returns>Computed bounding sphere.</returns>
            public static Data FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange)
            {
                FromIndexedPoints(points, indices, subMeshRange, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <param name="result">Computed bounding sphere.</param>
            public static void FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, out Data result)
            {
                FromIndexedPoints(points, indices, null, out result);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for.</param>
            /// <param name="result">Computed bounding sphere.</param>
            public static void FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange, out Data result)
            {
                if (!ExtractSubMeshRange(points, indices, subMeshRange, out int offset, out int count, out int baseVertexOffset))
                {
                    result = new Data();
                    return;
                }

                using (MappedDataBuffer mPtr = points.Map())
                {
                    IntPtr posPtr = mPtr + offset;
                    unsafe
                    {
                        ComputeMinSphere((Vector3*)posPtr.ToPointer(), count, indices, baseVertexOffset, out result);
                    }
                }
            }

            #region Sphere Calculations

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static unsafe void ComputeMinSphere(Vector3* points, int numPoints, IndexData? indices, int baseVertexOffset, out Data result)
            {
                result = new Data();

                Vector3* ptsCopy = null;
                try
                {
                    if (indices == null)
                    {
                        int sizeInBytes = numPoints * Vector3.SizeInBytes;
                        ptsCopy = (Vector3*)MemoryHelper.AllocateMemory(sizeInBytes).ToPointer();
                        MemoryHelper.CopyMemory(new IntPtr(ptsCopy), new IntPtr(points), sizeInBytes);
                    }
                    else
                    {
                        IndexData indicesToCopy = indices.Value;
                        numPoints = indicesToCopy.Length;
                        int sizeInBytes = numPoints * Vector3.SizeInBytes;
                        ptsCopy = (Vector3*)MemoryHelper.AllocateMemory(sizeInBytes).ToPointer();

                        for (int i = 0; i < numPoints; i++)
                        {
                            ptsCopy[i] = points[indicesToCopy[i] + baseVertexOffset];
                        }
                    }

                    CalculateWelzl(ptsCopy, numPoints, 0, 0, ref result);
                }
                finally
                {
                    if (ptsCopy != null)
                    {
                        MemoryHelper.FreeMemory(new IntPtr(ptsCopy));
                    }
                }
            }

            private static unsafe void CalculateWelzl(Vector3* points, int numPoints, int supportCount, int index, ref Data result)
            {
                switch (index)
                {
                    case 0:
                        result.Center = Vector3.Zero;
                        result.Radius = 0.0f;
                        break;

                    case 1:
                        ExactSphere1(ref points[index - 1], MathHelper.ZeroTolerance, out result);
                        break;

                    case 2:
                        ExactSphere2(ref points[index - 1], ref points[index - 2], out result);
                        break;

                    case 3:
                        ExactSphere3(ref points[index - 1], ref points[index - 2], ref points[index - 3], out result);
                        break;

                    case 4:
                        ExactSphere4(ref points[index - 1], ref points[index - 2], ref points[index - 3], ref points[index - 4], out result);
                        return;
                }

                for (int i = 0; i < numPoints; i++)
                {
                    Vector3.DistanceSquared(ref points[i + index], ref result.Center, out float distSqr);
                    if ((distSqr - (result.Radius * result.Radius)) > 0.0f)
                    {
                        for (int j = i; j > 0; j--)
                        {
                            // Swap
                            Vector3 tmp = points[j];
                            points[j] = points[j - 1];
                            points[j - 1] = tmp;
                        }

                        CalculateWelzl(points, i, supportCount + 1, index + 1, ref result);
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void ExactSphere1(ref Vector3 P, float radius, out Data result)
            {
                result.Center = P;
                result.Radius = radius;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void ExactSphere2(ref Vector3 P0, ref Vector3 P1, out Data result)
            {
                Vector3.Add(ref P0, ref P1, out result.Center);
                Vector3.Multiply(ref result.Center, 0.5f, out result.Center);
                
                Vector3.Subtract(ref P1, ref P0, out Vector3 diff);

                result.Radius = 0.25f * diff.LengthSquared();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void ExactSphere3(ref Vector3 P0, ref Vector3 P1, ref Vector3 P2, out Data result)
            {
                Vector3.Subtract(ref P0, ref P2, out Vector3 A);
                Vector3.Subtract(ref P1, ref P2, out Vector3 B);
                
                Vector3.Dot(ref A, ref A, out float AdA);
                Vector3.Dot(ref A, ref B, out float AdB);
                Vector3.Dot(ref B, ref B, out float BdB);

                float det = (AdA * BdB) - (AdB * AdB);
                if (Math.Abs(det) > 0.0f)
                {
                    float m00, m01, m10, m11, d0, d1;
                    if (AdA >= BdB)
                    {
                        m00 = 1.0f;
                        m01 = AdB / AdA;
                        m10 = m01;
                        m11 = BdB / AdA;
                        d0 = 0.5f;
                        d1 = 0.5f * m11;
                    }
                    else
                    {
                        m00 = AdA / BdB;
                        m01 = AdB / BdB;
                        m10 = m01;
                        m11 = 1.0f;
                        d0 = 0.5f * m00;
                        d1 = 0.5f;
                    }

                    // Compute center using barycentric coordinates of the inscribed triangle
                    float invDet = 1.0f / ((m00 * m11) - (m01 * m10));
                    float u0 = invDet * ((m11 * d0) - (m01 * d1));
                    float u1 = invDet * ((m00 * d1) - (m10 * d0));
                    float u2 = 1.0f - u0 - u1;
                    
                    Vector3.Multiply(ref P0, u0, out result.Center);
                    Vector3.Multiply(ref P1, u1, out Vector3 tmp);
                    Vector3.Add(ref result.Center, ref tmp, out result.Center);
                    Vector3.Multiply(ref P2, u2, out tmp);
                    Vector3.Add(ref result.Center, ref tmp, out result.Center);

                    Vector3.Multiply(ref A, u0, out Vector3 rV);
                    Vector3.Multiply(ref B, u1, out tmp);
                    Vector3.Add(ref rV, ref tmp, out rV);

                    result.Radius = rV.LengthSquared();
                }
                else
                {
                    result.Center = Vector3.Zero;
                    result.Radius = 0.0f;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void ExactSphere4(ref Vector3 P0, ref Vector3 P1, ref Vector3 P2, ref Vector3 P3, out Data result)
            {
                Vector3.Subtract(ref P0, ref P3, out Vector3 A);
                Vector3.Subtract(ref P1, ref P3, out Vector3 B);
                Vector3.Subtract(ref P2, ref P3, out Vector3 C);

                Matrix4x4.FromAxes(ref A, ref B, ref C, out Matrix4x4 M);

                Vector3 D;
                Vector3.Dot(ref A, ref A, out D.X);
                Vector3.Dot(ref B, ref B, out D.Y);
                Vector3.Dot(ref C, ref C, out D.Z);

                Vector3.Multiply(ref D, 0.5f, out D);

                if (Math.Abs(M.Determinant()) > MathHelper.ZeroTolerance)
                {
                    Matrix4x4.Invert(ref M, out Matrix4x4 invM);
                    
                    Vector3.Multiply(ref invM, ref D, out Vector3 V); // invM * D (opposite of eberly, invM is row major so transpose)
                    Vector3.TransformNormal(ref V, ref invM, out Vector3 U); // V * invM

                    float U3 = 1.0f - U[0] - U[1] - U[2];
                    
                    // Calculate center
                    Vector3.Multiply(ref P0, U[0], out result.Center);
                    Vector3.Multiply(ref P1, U[1], out Vector3 tmp);
                    Vector3.Add(ref result.Center, ref tmp, out result.Center);
                    Vector3.Multiply(ref P2, U[2], out tmp);
                    Vector3.Add(ref result.Center, ref tmp, out result.Center);
                    Vector3.Multiply(ref P3, U3, out tmp);
                    Vector3.Add(ref result.Center, ref tmp, out result.Center);

                    // Calculate radius
                    Vector3.Multiply(ref A, U[0], out Vector3 rV);
                    Vector3.Multiply(ref B, U[1], out tmp);
                    Vector3.Add(ref rV, ref tmp, out rV);
                    Vector3.Multiply(ref C, U[2], out tmp);
                    Vector3.Add(ref rV, ref tmp, out rV);

                    result.Radius = rV.LengthSquared();
                }
                else
                {
                    result.Center = Vector3.Zero;
                    result.Radius = 0.0f;
                }
            }

            #endregion

            /// <summary>
            /// Determines whether the specified <see cref="object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
            /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
            public override bool Equals(object obj)
            {
                if (obj is BoundingBox.Data)
                {
                    return Equals((BoundingBox.Data)obj);
                }

                return false;
            }

            /// <summary>
            /// Tests equality between this bounding sphere and another.
            /// </summary>
            /// <param name="other">Bounding sphere</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(Data other)
            {
                return Center.Equals(ref other.Center) && MathHelper.IsApproxEquals(Radius, other.Radius);
            }

            /// <summary>
            /// Tests equality between this bounding sphere and another.
            /// </summary>
            /// <param name="other">Bounding sphere</param>
            /// <param name="tolerance">Tolerance</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(Data other, float tolerance)
            {
                return Center.Equals(ref other.Center, tolerance) && MathHelper.IsApproxEquals(Radius, other.Radius, tolerance);
            }

            /// <summary>
            /// Tests equality between this bounding sphere and another.
            /// </summary>
            /// <param name="other">Bounding sphere</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(ref Data other)
            {
                return Center.Equals(ref other.Center) && MathHelper.IsApproxEquals(Radius, other.Radius);
            }

            /// <summary>
            /// Tests equality between this bounding sphere and another.
            /// </summary>
            /// <param name="other">Bounding sphere</param>
            /// <param name="tolerance">Tolerance</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(ref Data other, float tolerance)
            {
                return Center.Equals(ref other.Center, tolerance) && MathHelper.IsApproxEquals(Radius, other.Radius, tolerance);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return Center.GetHashCode() + Radius.GetHashCode();
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

                return string.Format(formatProvider, "Center: {0}, Radius: {1}", new object[] { Center.ToString(format, formatProvider), Radius.ToString(format, formatProvider) });
            }

            /// <summary>
            /// Reads the specified input.
            /// </summary>
            /// <param name="input">The input.</param>
            public void Read(IPrimitiveReader input)
            {
                Center = input.Read<Vector3>();
                Radius = input.ReadSingle();
            }

            /// <summary>
            /// Writes the primitive data to the output.
            /// </summary>
            /// <param name="output">Primitive writer</param>
            public void Write(IPrimitiveWriter output)
            {
                output.Write("Center", Center);
                output.Write("Radius", Radius);
            }
        }

        #endregion
    }
}
