namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core;
    using Content;
    using Graphics.Geometry;

    /// <summary>
    /// Represents an Axis-Aligned Bounding Box. The box is defined by extents from its center along the XYZ standard axes.
    /// </summary>
    public sealed class BoundingBox : BoundingVolume
    {
        private Vector3 _center;
        private Vector3 _extents;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        public BoundingBox()
        {
            _center = Vector3.Zero;
            _extents = Vector3.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        /// <param name="center">Center of the bounding box.</param>
        /// <param name="extents">Extents of the bounding box.</param>
        public BoundingBox(Vector3 center, Vector3 extents)
        {
            _center = center;
            _extents = extents;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        /// <param name="boundingBoxData">Bounding box data.</param>
        public BoundingBox(Data boundingBoxData)
        {
            _center = boundingBoxData.Center;
            _extents = boundingBoxData.Extents;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        /// <param name="boundingBox">Bounding box to copy from.</param>
        public BoundingBox(BoundingBox boundingBox)
        {
            if (boundingBox == null)
            {
                throw new ArgumentNullException(nameof(boundingBox));
            }

            _center = boundingBox.Center;
            _extents = boundingBox.Extents;
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
        /// Gets or sets the extents of the bounding box (half-lengths along each axis from the center).
        /// </summary>
        public Vector3 Extents
        {
            get => _extents;
            set
            {
                _extents = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets the maximum point of the bounding box.
        /// </summary>
        public Vector3 Max => new Vector3(_center.X + _extents.X, _center.Y + _extents.Y, _center.Z + _extents.Z);

        /// <summary>
        /// Gets the minimum point of the bounding box.
        /// </summary>
        public Vector3 Min => new Vector3(_center.X - _extents.X, _center.Y - _extents.Y, _center.Z - _extents.Z);

        /// <summary>
        /// Gets the volume of the bounding volume.
        /// </summary>
        public override float Volume => (_extents.X * _extents.Y * _extents.Z) * 2.0f;

        /// <summary>
        /// Gets the bounding type.
        /// </summary>
        public override BoundingType BoundingType => BoundingType.AxisAlignedBoundingBox;

        /// <summary>
        /// Gets the number of corners.
        /// </summary>
        public override int CornerCount => 8;

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
            Vector3 max = new Vector3(_center.X + _extents.X, _center.Y + _extents.Y, _center.Z + _extents.Z);
            Vector3 min = new Vector3(_center.X - _extents.X, _center.Y - _extents.Y, _center.Z - _extents.Z);

            Vector3.Min(ref min, ref point, out min);
            Vector3.Max(ref max, ref point, out max);

            // Compute center from min-max
            Vector3.Add(ref max, ref min, out _center);
            Vector3.Multiply(ref _center, 0.5f, out _center);

            // Compute extents from min-max
            Vector3.Subtract(ref max, ref _center, out _extents);
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

            Vector3 max = new Vector3(_center.X + _extents.X, _center.Y + _extents.Y, _center.Z + _extents.Z);
            Vector3 min = new Vector3(_center.X - _extents.X, _center.Y - _extents.Y, _center.Z - _extents.Z);

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 pt = points[i];
                Vector3.Min(ref min, ref pt, out min);
                Vector3.Max(ref max, ref pt, out max);
            }

            // Compute center from min-max
            Vector3.Add(ref max, ref min, out _center);
            Vector3.Multiply(ref _center, 0.5f, out _center);

            // Compute extents from min-max
            Vector3.Subtract(ref max, ref _center, out _extents);
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

            Vector3 max = new Vector3(_center.X + _extents.X, _center.Y + _extents.Y, _center.Z + _extents.Z);
            Vector3 min = new Vector3(_center.X - _extents.X, _center.Y - _extents.Y, _center.Z - _extents.Z);

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 pt = points[i];
                Vector3.Min(ref min, ref pt, out min);
                Vector3.Max(ref max, ref pt, out max);
            }

            // Compute center from min-max
            Vector3.Add(ref max, ref min, out _center);
            Vector3.Multiply(ref _center, 0.5f, out _center);

            // Compute extents from min-max
            Vector3.Subtract(ref max, ref _center, out _extents);
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
                        _center = other._center;
                        _extents = other._extents;
                    }
                    break;

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        float radius = other.Radius;
                        _center = other.Center;
                        _extents = new Vector3(radius, radius, radius);
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
        /// Sets the bounding box from the specified data.
        /// </summary>
        /// <param name="boundingBoxData">Bounding box data to set.</param>
        public void Set(Data boundingBoxData)
        {
            _center = boundingBoxData.Center;
            _extents = boundingBoxData.Extents;
            UpdateCorners = true;
        }

        /// <summary>
        /// Sets the bounding box from the specified data.
        /// </summary>
        /// <param name="center">Center of the bounding box.</param>
        /// <param name="extents">Extents of the bounding box.</param>
        public void Set(Vector3 center, Vector3 extents)
        {
            _center = center;
            _extents = extents;
            UpdateCorners = true;
        }

        /// <summary>
        /// Creates a copy of the bounding volume and returns a new instance.
        /// </summary>
        /// <returns>Copied bounding volume</returns>
        public override BoundingVolume Clone()
        {
            return new BoundingBox(this);
        }

        /// <summary>
        /// Computes the distance from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distance from the point to the edge of the volume.</param>
        public override void DistanceTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointAABB(ref point, ref _center, ref _extents, out result);
            result = (float)Math.Sqrt(result);
        }

        /// <summary>
        /// Computes the distance squared from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distance squared from the point to the edge of the volume.</param>
        public override void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointAABB(ref point, ref _center, ref _extents, out result);
        }

        /// <summary>
        /// Computes the closest point on the volume from the given point in space.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Closest point on the edge of the volume.</param>
        public override void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            Vector3 max = new Vector3(_center.X + _extents.X, _center.Y + _extents.Y, _center.Z + _extents.Z);
            Vector3 min = new Vector3(_center.X - _extents.X, _center.Y - _extents.Y, _center.Z - _extents.Z);

            result = point;

            for (int i = 0; i < 3; i++)
            {
                float v = point[i];
                v = Math.Max(v, min[i]);
                v = Math.Min(v, max[i]);
                result[i] = v;
            }
        }

        /// <summary>
        /// Computes a minimum bounding volume that encloses the specified points in space.
        /// </summary>
        /// <param name="points">Points in space</param>
        /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
        public override void ComputeFromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
        {
            Data.FromPoints(points, subMeshRange, out Data data);

            _center = data.Center;
            _extents = data.Extents;
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
            Data.FromIndexedPoints(points, indices, subMeshRange, out Data data);

            _center = data.Center;
            _extents = data.Extents;
            UpdateCorners = true;
        }

        /// <summary>
        /// Determines if the specified point is contained inside the bounding volume.
        /// </summary>
        /// <param name="point">Point to test against.</param>
        /// <returns>Type of containment</returns>
        public override ContainmentType Contains(ref Vector3 point)
        {
            Vector3.Subtract(ref point, ref _center, out Vector3 diff);

            if ((Math.Abs(diff.X) <= _extents.X) && (Math.Abs(diff.Y) <= _extents.Y) && (Math.Abs(diff.Z) <= _extents.Z))
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
            Vector3.Subtract(ref line.StartPoint, ref _center, out Vector3 startDiff);
            Vector3.Subtract(ref line.EndPoint, ref _center, out Vector3 endDiff);

            // If both end points are inside the bounding box, then it is wholly contained
            if (((Math.Abs(startDiff.X) <= _extents.X) && (Math.Abs(startDiff.Y) <= _extents.Y) && (Math.Abs(startDiff.Z) <= _extents.Z)) && ((Math.Abs(endDiff.X) <= _extents.X) && (Math.Abs(endDiff.Y) <= _extents.Y) && (Math.Abs(endDiff.Z) <= _extents.Z)))
            {
                return ContainmentType.Inside;
            }

            Vector3 xAxis = Vector3.UnitX;
            Vector3 yAxis = Vector3.UnitY;
            Vector3 zAxis = Vector3.UnitZ;

            if (GeometricToolsHelper.IntersectSegmentAABB(ref line, ref _center, ref _extents))
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
            Vector3.Subtract(ref triangle.PointA, ref _center, out Vector3 ptADiff);
            Vector3.Subtract(ref triangle.PointB, ref _center, out Vector3 ptBDiff);
            Vector3.Subtract(ref triangle.PointC, ref _center, out Vector3 ptCDiff);

            // If all three vertices are inside the bounding box, then it is wholly contained
            if (((Math.Abs(ptADiff.X) <= _extents.X) && (Math.Abs(ptADiff.Y) <= _extents.Y) && (Math.Abs(ptADiff.Z) <= _extents.Z)) &&
                ((Math.Abs(ptBDiff.X) <= _extents.X) && (Math.Abs(ptBDiff.Y) <= _extents.Y) && (Math.Abs(ptBDiff.Z) <= _extents.Z)) &&
                ((Math.Abs(ptCDiff.X) <= _extents.X) && (Math.Abs(ptCDiff.Y) <= _extents.Y) && (Math.Abs(ptCDiff.Z) <= _extents.Z)))
            {
                return ContainmentType.Inside;
            }

            Vector3 xAxis = Vector3.UnitX;
            Vector3 yAxis = Vector3.UnitY;
            Vector3 zAxis = Vector3.UnitZ;

            if (GeometricToolsHelper.IntersectTriangleBox(ref triangle, ref _center, ref xAxis, ref yAxis, ref zAxis, ref _extents))
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
                        return GeometricToolsHelper.AABBContainsAABB(ref _center, ref _extents, ref other._center, ref other._extents);
                    }

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        Vector3 sphereCenter = other.Center;
                        float sphereRadius = other.Radius;

                        return GeometricToolsHelper.AABBContainsSphere(ref _center, ref _extents, ref sphereCenter, sphereRadius);
                    }

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;
                        float capsuleRadius = other.Radius;

                        return GeometricToolsHelper.AABBContainsCapsule(ref _center, ref _extents, ref centerLine, capsuleRadius);
                    }

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        Vector3 boxCenter = other.Center;
                        Triad boxAxes = other.Axes;
                        Vector3 boxExtents = other.Extents;

                        Vector3 xAxis = Vector3.UnitX;
                        Vector3 yAxis = Vector3.UnitY;
                        Vector3 zAxis = Vector3.UnitZ;

                        return GeometricToolsHelper.BoxContainsBox(ref _center, ref xAxis, ref yAxis, ref zAxis, ref _extents,
                            ref boxCenter, ref boxAxes.XAxis, ref boxAxes.YAxis, ref boxAxes.ZAxis, ref boxExtents);
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
            return GeometricToolsHelper.IntersectRayAABB(ref ray, ref _center, ref _extents);
        }

        /// <summary>
        /// Determines if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public override bool Intersects(ref Ray ray, out BoundingIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectRayAABB(ref ray, ref _center, ref _extents, out result);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line)
        {
            return GeometricToolsHelper.IntersectSegmentAABB(ref line, ref _center, ref _extents);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line, out BoundingIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectSegmentAABB(ref line, ref _center, ref _extents, out result);
        }

        /// <summary>
        /// Tests if the specified plane intersects with the bounding volume.
        /// </summary>
        /// <param name="plane">Plane to test against.</param>
        /// <returns>Type of plane intersection.</returns>
        public override PlaneIntersectionType Intersects(ref Plane plane)
        {
            // Plane-AABB intersection 
            // Real-Time Collision Detection by Christer Ericson

            Vector3 max = new Vector3(_center.X + _extents.X, _center.Y + _extents.Y, _center.Z + _extents.Z);
            Vector3 min = new Vector3(_center.X - _extents.X, _center.Y - _extents.Y, _center.Z - _extents.Z);

            Vector3 n = max;
            Vector3 p = min;

            if (plane.Normal.X >= 0.0f)
            {
                p.X = max.X;
                n.X = min.X;
            }

            if (plane.Normal.Y >= 0.0f)
            {
                p.Y = max.Y;
                n.Y = min.Y;
            }

            if (plane.Normal.Z >= 0.0f)
            {
                p.Z = max.Z;
                n.Z = min.Z;
            }

            if (plane.WhichSide(ref n) == PlaneIntersectionType.Front)
            {
                return PlaneIntersectionType.Front;
            }

            if (plane.WhichSide(ref p) == PlaneIntersectionType.Back)
            {
                return PlaneIntersectionType.Back;
            }

            return PlaneIntersectionType.Intersects;
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

                        return GeometricToolsHelper.IntersectAABBAABB(ref _center, ref _extents, ref other._center, ref other._extents);
                    }

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        Vector3 sphereCenter = other.Center;
                        float sphereRadius = other.Radius;

                        return GeometricToolsHelper.IntersectSphereAABB(ref sphereCenter, sphereRadius, ref _center, ref _extents);
                    }

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;
                        float capsuleRadius = other.Radius;

                        return GeometricToolsHelper.IntersectCapsuleAABB(ref centerLine, capsuleRadius, ref _center, ref _extents);
                    }

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        Vector3 boxCenter = other.Center;
                        Triad boxAxes = other.Axes;
                        Vector3 boxExtents = other.Extents;

                        Vector3 unitXAxis = Vector3.UnitX;
                        Vector3 unitYAxis = Vector3.UnitY;
                        Vector3 unitZAxis = Vector3.UnitZ;

                        return GeometricToolsHelper.IntersectBoxBox(ref _center, ref unitXAxis, ref unitYAxis, ref unitZAxis, ref _extents, ref boxCenter, ref boxAxes.XAxis, ref boxAxes.YAxis, ref boxAxes.ZAxis, ref boxExtents);
                    }

                case BoundingType.Frustum:
                    {
                        ContainmentType contain = GeometricToolsHelper.FrustumContainsAABB(volume as BoundingFrustum, ref _center, ref _extents);

                        return contain != ContainmentType.Outside;
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

            switch (volume.BoundingType)
            {
                case BoundingType.AxisAlignedBoundingBox:
                    {
                        BoundingBox other = volume as BoundingBox;

                        Data b0;
                        b0.Center = _center;
                        b0.Extents = _extents;

                        Data b1;
                        b1.Center = other._center;
                        b1.Extents = other._extents;
                        
                        GeometricToolsHelper.MergeAABBABB(ref b0, ref b1, out Data merged);

                        _center = merged.Center;
                        _extents = merged.Extents;
                    }
                    break;

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        float sphereRadius = other.Radius;

                        Data b0;
                        b0.Center = _center;
                        b0.Extents = _extents;

                        Data b1;
                        b1.Center = other.Center;
                        b1.Extents = new Vector3(sphereRadius, sphereRadius, sphereRadius);
                        
                        GeometricToolsHelper.MergeAABBABB(ref b0, ref b1, out Data merged);

                        _center = merged.Center;
                        _extents = merged.Extents;
                    }
                    break;

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;
                        float capsuleRadius = other.Radius;

                        Data b0;
                        b0.Center = _center;
                        b0.Extents = _extents;

                        // Treat capsule as an OBB
                        GeometricToolsHelper.CalculateSegmentProperties(ref centerLine, out Vector3 segCenter, out Vector3 segDir, out float segExtent);

                        OrientedBoundingBox.Data b1;
                        b1.Center = segCenter;
                        Triad.FromZComplementBasis(ref segDir, out b1.Axes);
                        b1.Extents = new Vector3(capsuleRadius, capsuleRadius, segExtent + capsuleRadius);
                        
                        GeometricToolsHelper.MergeAABBBox(ref b0, ref b1, out Data merged);

                        _center = merged.Center;
                        _extents = merged.Extents;
                    }
                    break;

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;

                        Data b0;
                        b0.Center = _center;
                        b0.Extents = _extents;

                        OrientedBoundingBox.Data b1;
                        b1.Center = other.Center;
                        b1.Axes = other.Axes;
                        b1.Extents = other.Extents;
                        
                        GeometricToolsHelper.MergeAABBBox(ref b0, ref b1, out Data merged);

                        _center = merged.Center;
                        _extents = merged.Extents;
                    }
                    break;
                    
                default:
                    {
                        Extend(volume.Corners);
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
            // Transform center by the SRT
            Vector3.Multiply(ref _center, ref scale, out Vector3 center);
            Vector3.Transform(ref center, ref rotation, out center);
            Vector3.Add(ref translation, ref center, out center);

            // Scale extents first
            Vector3 extents;
            extents.X = Math.Abs(Math.Abs(scale.X) * _extents.X);
            extents.Y = Math.Abs(Math.Abs(scale.Y) * _extents.Y);
            extents.Z = Math.Abs(Math.Abs(scale.Z) * _extents.Z);

            // Then transform extents by rotation
            Matrix4x4.FromQuaternion(ref rotation, out Matrix4x4 rotMat);

            rotMat.M11 = Math.Abs(rotMat.M11);
            rotMat.M12 = Math.Abs(rotMat.M12);
            rotMat.M13 = Math.Abs(rotMat.M13);
            rotMat.M21 = Math.Abs(rotMat.M21);
            rotMat.M22 = Math.Abs(rotMat.M22);
            rotMat.M23 = Math.Abs(rotMat.M23);
            rotMat.M31 = Math.Abs(rotMat.M31);
            rotMat.M32 = Math.Abs(rotMat.M32);
            rotMat.M33 = Math.Abs(rotMat.M33);

            // We don't care about the last row (translation), so pointless to do extra adds
            Vector3.TransformNormal(ref extents, ref rotMat, out extents);

            _center = center;
            _extents = extents;
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
            BoundingBox bb = other as BoundingBox;
            if (bb != null)
            {
                return _center.Equals(ref bb._center, tolerance) && _extents.Equals(ref bb._extents, tolerance);
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
            _extents = input.Read<Vector3>();
            UpdateCorners = true;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            output.Write("Center", _center);
            output.Write("Extents", _extents);
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

            Vector3 max = new Vector3(_center.X + _extents.X, _center.Y + _extents.Y, _center.Z + _extents.Z);
            Vector3 min = new Vector3(_center.X - _extents.X, _center.Y - _extents.Y, _center.Z - _extents.Z);

            corners.Set(new Vector3(min.X, max.Y, max.Z));
            corners.Set(new Vector3(max.X, max.Y, max.Z));
            corners.Set(new Vector3(max.X, min.Y, max.Z));
            corners.Set(new Vector3(min.X, min.Y, max.Z));
            corners.Set(new Vector3(min.X, max.Y, min.Z));
            corners.Set(new Vector3(max.X, max.Y, min.Z));
            corners.Set(new Vector3(max.X, min.Y, min.Z));
            corners.Set(new Vector3(min.X, min.Y, min.Z));
        }

        #region BoundingBox Data

        /// <summary>
        /// Represents data for a <see cref="BoundingBox"/>.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct Data : IEquatable<Data>, IFormattable, IPrimitiveValue
        {
            /// <summary>
            /// Center of the bounding box.
            /// </summary>
            public Vector3 Center;

            /// <summary>
            /// Extents of the bounding box (half-lengths along each axis from the center).
            /// </summary>
            public Vector3 Extents;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="center">Center of the bounding box.</param>
            /// <param name="extents">Extents of the bounding box.</param>
            public Data(Vector3 center, Vector3 extents)
            {
                Center = center;
                Extents = extents;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="box">Bounding box to copy from.</param>
            public Data(BoundingBox box)
            {
                Center = box._center;
                Extents = box._extents;
            }

            /// <summary>
            /// Determines if the AABB intersects with another.
            /// </summary>
            /// <param name="box">AABB to test against.</param>
            /// <returns>True if the objects intersect, false otherwise.</returns>
            public bool Intersects(ref BoundingBox.Data box)
            {
                return GeometricToolsHelper.IntersectAABBAABB(ref Center, ref Extents, ref box.Center, ref box.Extents);
            }

            /// <summary>
            /// Determines if the AABB intersects with a sphere.
            /// </summary>
            /// <param name="sphere">Sphere to test against</param>
            /// <returns>True if the objects intersect, false otherwise.</returns>
            public bool Intersects(ref BoundingSphere.Data sphere)
            {
                return GeometricToolsHelper.IntersectSphereAABB(ref sphere.Center, sphere.Radius, ref Center, ref Extents);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <returns>Computed bounding box</returns>
            public static Data FromPoints(IReadOnlyDataBuffer<Vector3> points)
            {
                FromPoints(points, null, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
            /// <returns>Computed bounding box</returns>
            public static Data FromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
            {
                FromPoints(points, subMeshRange, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <param name="result">Computed bounding box</param>
            public static void FromPoints(IReadOnlyDataBuffer<Vector3> points, out Data result)
            {
                FromPoints(points, null, out result);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
            /// <param name="result">Computed bounding box</param>
            public static void FromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange, out Data result)
            {
                int offset = 0;
                int count = 0;

                if (!ExtractSubMeshRange(points, subMeshRange, out offset, out count))
                {
                    result.Center = Vector3.Zero;
                    result.Extents = Vector3.Zero;

                    return;
                }

                // Trivial case
                if (count == 1)
                {
                    result.Center = points[offset];
                    result.Extents = Vector3.Zero;
                    return;
                }

                Vector3 min = new Vector3(float.MaxValue);
                Vector3 max = new Vector3(float.MinValue);

                int upperBoundExclusive = offset + count;
                for (int i = offset; i < upperBoundExclusive; i++)
                {
                    Vector3 pt = points[i];
                    Vector3.Min(ref min, ref pt, out min);
                    Vector3.Max(ref max, ref pt, out max);
                }

                // Compute center from min-max
                Vector3.Add(ref max, ref min, out result.Center);
                Vector3.Multiply(ref result.Center, 0.5f, out result.Center);

                // Compute extents from min-max
                Vector3.Subtract(ref max, ref result.Center, out result.Extents);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <returns>Computed bounding box</returns>
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
            /// <returns>Computed bounding box</returns>
            public static Data FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange)
            {
                FromIndexedPoints(points, indices, subMeshRange, out Data result);
                return result;
            }

            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <param name="result">Computed bounding box</param>
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
            /// <param name="result">Computed bounding box</param>
            public static void FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange, out Data result)
            {
                int offset = 0;
                int count = 0;
                int baseVertexOffset = 0;

                if (!ExtractSubMeshRange(points, indices, subMeshRange, out offset, out count, out baseVertexOffset))
                {
                    result.Center = Vector3.Zero;
                    result.Extents = Vector3.Zero;

                    return;
                }

                // Trivial case
                if (count == 1)
                {
                    result.Center = points[indices[offset] + baseVertexOffset];
                    result.Extents = Vector3.Zero;
                }

                Vector3 min = new Vector3(float.MaxValue);
                Vector3 max = new Vector3(float.MinValue);

                int upperBoundExclusive = offset + count;
                for (int i = offset; i < upperBoundExclusive; i++)
                {
                    int index = indices[i];
                    Vector3 pt = points[index + baseVertexOffset];
                    Vector3.Min(ref min, ref pt, out min);
                    Vector3.Max(ref max, ref pt, out max);
                }

                // Compute center from min-max
                Vector3.Add(ref max, ref min, out result.Center);
                Vector3.Multiply(ref result.Center, 0.5f, out result.Center);

                // Compute extents from min-max
                Vector3.Subtract(ref max, ref result.Center, out result.Extents);
            }

            /// <summary>
            /// Determines whether the specified <see cref="object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
            /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
            public override bool Equals(object obj)
            {
                if (obj is Data)
                {
                    return Equals((Data)obj);
                }

                return false;
            }

            /// <summary>
            /// Tests equality between this bounding box and another.
            /// </summary>
            /// <param name="other">Bounding box</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(Data other)
            {
                return Center.Equals(ref other.Center) && Extents.Equals(ref other.Extents);
            }

            /// <summary>
            /// Tests equality between this bounding box and another.
            /// </summary>
            /// <param name="other">Bounding box</param>
            /// <param name="tolerance">Tolerance</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(Data other, float tolerance)
            {
                return Center.Equals(ref other.Center, tolerance) && Extents.Equals(ref other.Extents, tolerance);
            }

            /// <summary>
            /// Tests equality between this bounding box and another.
            /// </summary>
            /// <param name="other">Bounding box</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(ref Data other)
            {
                return Center.Equals(ref other.Center) && Extents.Equals(ref other.Extents);
            }

            /// <summary>
            /// Tests equality between this bounding box and another.
            /// </summary>
            /// <param name="other">Bounding box</param>
            /// <param name="tolerance">Tolerance</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(ref Data other, float tolerance)
            {
                return Center.Equals(ref other.Center, tolerance) && Extents.Equals(ref other.Extents, tolerance);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return Center.GetHashCode() + Extents.GetHashCode();
                }
            }

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

                return string.Format(formatProvider, "Center: {0}, Extents: {1}", new object[] { Center.ToString(format, formatProvider), Extents.ToString(format, formatProvider) });
            }

            /// <summary>
            /// Reads the specified input.
            /// </summary>
            /// <param name="input">The input.</param>
            public void Read(IPrimitiveReader input)
            {
                Center = input.Read<Vector3>();
                Extents = input.Read<Vector3>();
            }

            /// <summary>
            /// Writes the primitive data to the output.
            /// </summary>
            /// <param name="output">Primitive writer</param>
            public void Write(IPrimitiveWriter output)
            {
                output.Write("Center", Center);
                output.Write("Extents", Extents);
            }
        }

        #endregion
    }
}
