namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    using Core;
    using Content;
    using Graphics.Geometry;

    /// <summary>
    /// Represents a Bounding Box with arbitrary orientation defined by three axes. The box is defined by extents from its center along the three axes.
    /// </summary>
    public sealed class OrientedBoundingBox : BoundingVolume
    {
        private Vector3 _center;
        private Triad _axes;
        private Vector3 _extents;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrientedBoundingBox"/> class.
        /// </summary>
        public OrientedBoundingBox()
        {
            _center = Vector3.Zero;
            _axes = Triad.UnitAxes;
            _extents = Vector3.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrientedBoundingBox"/> class.
        /// </summary>
        /// <param name="center">Center of the bounding box.</param>
        /// <param name="extents">Extents of the bounding box.</param>
        public OrientedBoundingBox(Vector3 center, Vector3 extents)
        {
            _center = center;
            _axes = Triad.UnitAxes;
            _extents = extents;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrientedBoundingBox"/> class.
        /// </summary>
        /// <param name="center">Center of the bounding box.</param>
        /// <param name="xAxis">X-Axis of the bounding box.</param>
        /// <param name="yAxis">Y-Axis of the bounding box.</param>
        /// <param name="zAxis">Z-Axis of the bounding box.</param>
        /// <param name="extents">Extents of the bounding box.</param>
        public OrientedBoundingBox(Vector3 center, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, Vector3 extents)
        {
            _center = center;
            _axes.XAxis = xAxis;
            _axes.YAxis = yAxis;
            _axes.ZAxis = zAxis;
            _extents = extents;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrientedBoundingBox"/> class.
        /// </summary>
        /// <param name="center">Center of the bounding box.</param>
        /// <param name="axes">Axes of the bounding box.</param>
        /// <param name="extents">Extents of the bounding box.</param>
        public OrientedBoundingBox(Vector3 center, Triad axes, Vector3 extents)
        {
            _center = center;
            _axes = axes;
            _extents = extents;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrientedBoundingBox"/> class.
        /// </summary>
        /// <param name="orientedBoundingBoxData">Oriented bounding box data.</param>
        public OrientedBoundingBox(Data orientedBoundingBoxData)
        {
            _center = orientedBoundingBoxData.Center;
            _axes = orientedBoundingBoxData.Axes;
            _extents = orientedBoundingBoxData.Extents;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrientedBoundingBox"/> class.
        /// </summary>
        /// <param name="orientedBoundingBox">Oriented bounding box to copy from.</param>
        public OrientedBoundingBox(OrientedBoundingBox orientedBoundingBox)
        {
            if (orientedBoundingBox == null)
            {
                throw new ArgumentNullException(nameof(orientedBoundingBox));
            }

            _center = orientedBoundingBox._center;
            _axes = orientedBoundingBox._axes;
            _extents = orientedBoundingBox._extents;
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
        /// Gets or sets the X-Axis of the bounding box.
        /// </summary>
        public Vector3 XAxis
        {
            get => _axes.XAxis;
            set
            {
                _axes.XAxis = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets or sets the Y-Axis of the bounding box.
        /// </summary>
        public Vector3 YAxis
        {
            get => _axes.YAxis;
            set
            {
                _axes.YAxis = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets or sets the Z-Axis of the bounding box.
        /// </summary>
        public Vector3 ZAxis
        {
            get => _axes.ZAxis;
            set
            {
                _axes.ZAxis = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets or sets the axes of the bounding box;
        /// </summary>
        public Triad Axes
        {
            get => _axes;
            set => _axes = Axes;
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
        /// Gets the volume of the bounding volume.
        /// </summary>
        public override float Volume => (_extents.X * _extents.Y * _extents.Z) * 2.0f;

        /// <summary>
        /// Gets the number of corners.
        /// </summary>
        public override int CornerCount => 8;

        /// <summary>
        /// Gets the bounding type.
        /// </summary>
        public override BoundingType BoundingType => BoundingType.OrientedBoundingBox;

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
                        _axes = Triad.UnitAxes;
                        _extents = other.Extents;
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

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;

                        GeometricToolsHelper.BoxFromCapsule(ref centerLine, other.Radius, out _center, out _axes.XAxis, out _axes.YAxis, out _axes.ZAxis, out _extents);
                    }
                    break;

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        _center = other._center;
                        _axes = other._axes;
                        _extents = other._extents;
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
        /// <param name="orientedBoundingBoxData">Oriented bounding box data.</param>
        public void Set(Data orientedBoundingBoxData)
        {
            _center = orientedBoundingBoxData.Center;
            _axes = orientedBoundingBoxData.Axes;
            _extents = orientedBoundingBoxData.Extents;
            UpdateCorners = true;
        }

        /// <summary>
        /// Sets the bounding box from the specified data.
        /// </summary>
        /// <param name="center">Center of the bounding box.</param>
        /// <param name="xAxis">X-Axis of the bounding box.</param>
        /// <param name="yAxis">Y-Axis of the bounding box.</param>
        /// <param name="zAxis">Z-Axis of the bounding box.</param>
        /// <param name="extents">Extents of the bounding box.</param>
        public void Set(Vector3 center, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, Vector3 extents)
        {
            _center = center;
            _axes.XAxis = xAxis;
            _axes.YAxis = yAxis;
            _axes.ZAxis = zAxis;
            _extents = extents;
            UpdateCorners = true;
        }

        /// <summary>
        /// Sets the bounding box from the specified data.
        /// </summary>
        /// <param name="center">Center of the bounding box.</param>
        /// <param name="axes">Axes of the bounding box.</param>
        /// <param name="extents">Extents of the bounding box.</param>
        public void Set(Vector3 center, Triad axes, Vector3 extents)
        {
            _center = center;
            _axes = axes;
            _extents = extents;
            UpdateCorners = true;
        }

        /// <summary>
        /// Creates a copy of the bounding volume and returns a new instance.
        /// </summary>
        /// <returns>Copied bounding volume</returns>
        public override BoundingVolume Clone()
        {
            return new OrientedBoundingBox(this);
        }

        /// <summary>
        /// Computes the distance from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distance from the point to the edge of the volume.</param>
        public override void DistanceTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointBox(ref point, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, out result);
            result = (float)Math.Sqrt(result);
        }

        /// <summary>
        /// Computes the distance squared from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distanced squared from the point to the edge of the volume.</param>
        public override void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointBox(ref point, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, out result);
        }

        /// <summary>
        /// Computes the closest point on the volume from the given point in space.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Closest point on the edge of the volume.</param>
        public override void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            GeometricToolsHelper.ClosestPointToBox(ref point, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, out result);
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
            _axes = data.Axes;
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
            _axes = data.Axes;
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

            for (int i = 0; i < 3; i++)
            {
                _axes.GetAxis(i, out Vector3 axis);
                
                Vector3.Dot(ref diff, ref axis, out float coeff);

                // If projected dist <= extent on each axis then it's inside
                if (Math.Abs(coeff) > _extents[i])
                {
                    return ContainmentType.Outside;
                }
            }

            return ContainmentType.Inside;
        }

        /// <summary>
        /// Determines if the specified segment line is contained inside the bounding volume.
        /// </summary>
        /// <param name="line">Segment to test against.</param>
        /// <returns>Type of containment.</returns>
        public override ContainmentType Contains(ref Segment line)
        {
            // If both end points are inside bounding box, then it is wholly contained
            if (Contains(ref line.StartPoint) == ContainmentType.Inside && Contains(ref line.EndPoint) == ContainmentType.Inside)
            {
                return ContainmentType.Inside;
            }

            if (GeometricToolsHelper.IntersectSegmentBox(ref line, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents))
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
            // If all three vertices are inside bounding box, then it is wholly contained
            if (Contains(ref triangle.PointA) == ContainmentType.Inside && Contains(ref triangle.PointB) == ContainmentType.Inside && Contains(ref triangle.PointC) == ContainmentType.Inside)
            {
                return ContainmentType.Inside;
            }

            if (GeometricToolsHelper.IntersectTriangleBox(ref triangle, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents))
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

                        Vector3 xAxis = Vector3.UnitX;
                        Vector3 yAxis = Vector3.UnitY;
                        Vector3 zAxis = Vector3.UnitZ;

                        return GeometricToolsHelper.BoxContainsBox(ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, ref boxCenter, ref xAxis, ref yAxis, ref zAxis, ref boxExtents);
                    }

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        Vector3 sphereCenter = other.Center;
                        float sphereRadius = other.Radius;

                        return GeometricToolsHelper.BoxContainsSphere(ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, ref sphereCenter, sphereRadius);
                    }

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;
                        float capsuleRadius = other.Radius;

                        return GeometricToolsHelper.BoxContainsCapsule(ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, ref centerLine, capsuleRadius);
                    }

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;

                        return GeometricToolsHelper.BoxContainsBox(ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents,
                            ref other._center, ref other._axes.XAxis, ref other._axes.YAxis, ref other._axes.ZAxis, ref other._extents);
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
            return GeometricToolsHelper.IntersectRayBox(ref ray, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents);
        }

        /// <summary>
        /// Determines if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public override bool Intersects(ref Ray ray, out BoundingIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectRayBox(ref ray, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, out result);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line)
        {
            return GeometricToolsHelper.IntersectSegmentBox(ref line, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line, out BoundingIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectSegmentBox(ref line, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, out result);
        }

        /// <summary>
        /// Tests if the specified plane intersects with the bounding volume.
        /// </summary>
        /// <param name="plane">Plane to test against.</param>
        /// <returns>Type of plane intersection.</returns>
        public override PlaneIntersectionType Intersects(ref Plane plane)
        {
            Vector3.Dot(ref plane.Normal, ref _axes.XAxis, out float xRadius);
            xRadius *= _extents.X;
            
            Vector3.Dot(ref plane.Normal, ref _axes.YAxis, out float yRadius);
            yRadius *= _extents.Y;
            
            Vector3.Dot(ref plane.Normal, ref _axes.ZAxis, out float zRadius);
            zRadius *= _extents.Z;

            float radius = Math.Abs(xRadius) + Math.Abs(yRadius) + Math.Abs(zRadius);
            plane.SignedDistanceTo(ref _center, out float signedDistance);

            if (signedDistance > radius)
            {
                return PlaneIntersectionType.Front;
            }
            else if (signedDistance < -radius)
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
        /// <returns> True if the two volumes intersect with one another, false otherwise.</returns>
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

                        Vector3 unitXAxis = Vector3.UnitX;
                        Vector3 unitYAxis = Vector3.UnitY;
                        Vector3 unitZAxis = Vector3.UnitZ;

                        return GeometricToolsHelper.IntersectBoxBox(ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, ref boxCenter, ref unitXAxis, ref unitYAxis, ref unitZAxis, ref boxExtents);
                    }

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        Vector3 sphereCenter = other.Center;
                        float sphereRadius = other.Radius;

                        return GeometricToolsHelper.IntersectSphereBox(ref sphereCenter, sphereRadius, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents);
                    }

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;
                        float capsuleRadius = other.Radius;

                        return GeometricToolsHelper.IntersectCapsuleBox(ref centerLine, capsuleRadius, ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents);
                    }

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;

                        return GeometricToolsHelper.IntersectBoxBox(ref _center, ref _axes.XAxis, ref _axes.YAxis, ref _axes.ZAxis, ref _extents, ref other._center, ref other._axes.XAxis, ref other._axes.YAxis, ref other._axes.ZAxis, ref other._extents);
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

            // Treat every other volume as an OBB and merge with that
            Data b0;
            b0.Center = _center;
            b0.Axes = _axes;
            b0.Extents = _extents;

            Data b1;
            Data merged;

            switch (volume.BoundingType)
            {
                case BoundingType.AxisAlignedBoundingBox:
                    {
                        BoundingBox other = volume as BoundingBox;

                        b1.Center = other.Center;
                        b1.Axes = Triad.UnitAxes;
                        b1.Extents = other.Extents;
                    }
                    break;

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        float sphereRadius = other.Radius;

                        b1.Center = other.Center;
                        b1.Axes = Triad.UnitAxes;
                        b1.Extents = new Vector3(sphereRadius, sphereRadius, sphereRadius);
                    }
                    break;

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        Segment centerLine = other.CenterLine;
                        float capsuleRadius = other.Radius;
                        
                        GeometricToolsHelper.CalculateSegmentProperties(ref centerLine, out Vector3 segCenter, out Vector3 segDir, out float segExtent);

                        b1.Center = segCenter;
                        Triad.FromYComplementBasis(ref segDir, out b1.Axes);
                        b1.Extents = new Vector3(capsuleRadius, capsuleRadius, segExtent + capsuleRadius);
                    }
                    break;

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;

                        b1.Center = other._center;
                        b1.Axes = other._axes;
                        b1.Extents = other._extents;
                    }
                    break;
                    
                default:
                    {
                        Data.FromPoints(volume.Corners, out b1);
                    }
                    break;
            }

            // We get a better fit by recomputing an OBB using the corners. Might be something wrong in GaussPointsFit...
            GeometricToolsHelper.MergeBoxBoxUsingCorners(ref b0, ref b1, out merged);

            _center = merged.Center;
            _axes = merged.Axes;
            _extents = merged.Extents;
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

            _center = center;

            // Scale extents
            _extents.X = Math.Abs(Math.Abs(scale.X) * _extents.X);
            _extents.Y = Math.Abs(Math.Abs(scale.Y) * _extents.Y);
            _extents.Z = Math.Abs(Math.Abs(scale.Z) * _extents.Z);

            // Transform axes
            Triad.Transform(ref _axes, ref rotation, out _axes);

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
            OrientedBoundingBox obb = other as OrientedBoundingBox;
            if (obb != null)
            {
                return _center.Equals(ref obb._center, tolerance) && _axes.Equals(ref obb._axes, tolerance) && _extents.Equals(ref obb._extents, tolerance);
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
            _axes = input.Read<Triad>();
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
            output.Write("Axes", _axes);
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
                throw new ArgumentNullException("corners");
            }

            if (!corners.HasNextFor(CornerCount))
            {
                throw new ArgumentOutOfRangeException(nameof(corners), "Not enough space for bounding corners");
            }

            Vector3.Multiply(ref _axes.XAxis, _extents.X, out Vector3 exAxis);
            Vector3.Multiply(ref _axes.YAxis, _extents.Y, out Vector3 eyAxis);
            Vector3.Multiply(ref _axes.ZAxis, _extents.Z, out Vector3 ezAxis);
            
            Vector3.Subtract(ref _center, ref exAxis, out Vector3 temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            corners.Set(ref temp);

            Vector3.Add(ref _center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            corners.Set(ref temp);

            Vector3.Add(ref _center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            corners.Set(ref temp);

            Vector3.Subtract(ref _center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            corners.Set(ref temp);

            Vector3.Add(ref _center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            corners.Set(ref temp);

            Vector3.Subtract(ref _center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            corners.Set(ref temp);

            Vector3.Add(ref _center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            corners.Set(ref temp);

            Vector3.Subtract(ref _center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            corners.Set(ref temp);
        }

        #region OrientedBoundingBox Data

        /// <summary>
        /// Represents data for a <see cref="OrientedBoundingBox"/>.
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
            /// Axes of the bounding box.
            /// </summary>
            public Triad Axes;

            /// <summary>
            /// Extents of the bounding box (half-lengths along each axis from the center).
            /// </summary>
            public Vector3 Extents;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="center">Center of the bounding box.</param>
            /// <param name="xAxis">X-Axis of the bounding box.</param>
            /// <param name="yAxis">Y-Axis of the bounding box.</param>
            /// <param name="zAxis">Z-Axis of the bounding box.</param>
            /// <param name="extents">Extents of the bounding box.</param>
            public Data(Vector3 center, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, Vector3 extents)
            {
                Center = center;
                Axes.XAxis = xAxis;
                Axes.YAxis = yAxis;
                Axes.ZAxis = zAxis;
                Extents = extents;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="center">Center of the bounding box.</param>
            /// <param name="axes">Axes of the bounding box.</param>
            /// <param name="extents">Extents of the bounding box.</param>
            public Data(Vector3 center, Triad axes, Vector3 extents)
            {
                Center = center;
                Axes = axes;
                Extents = extents;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="box">Oriented bounding box to copy from.</param>
            public Data(OrientedBoundingBox box)
            {
                Center = box._center;
                Axes = box._axes;
                Extents = box._extents;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <returns>Computed oriented bounding box.</returns>
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
            /// <returns>Computed oriented bounding box.</returns>
            public static Data FromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
            {
                FromPoints(points, null, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <param name="result">Computed oriented bounding box.</param>
            public static void FromPoints(IReadOnlyDataBuffer<Vector3> points, out Data result)
            {
                FromPoints(points, null, out result);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
            /// <param name="result">Computed oriented bounding box.</param>
            public static void FromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange, out Data result)
            {
                if (!ExtractSubMeshRange(points, subMeshRange, out int offset, out int count))
                {
                    result = new Data();
                    return;
                }

                Vector3 pt = points[offset];

                // Trivial case
                if (count == 1)
                {
                    result.Center = pt;
                    result.Axes = Triad.UnitAxes;
                    result.Extents = Vector3.Zero;
                    return;
                }

                GeometricToolsHelper.GaussPointsFit(points, offset, count, out result);

                Vector3 pMin, pMax;
                Vector3.Subtract(ref pt, ref result.Center, out Vector3 diff);
                Vector3.Dot(ref diff, ref result.Axes.XAxis, out pMin.X);
                Vector3.Dot(ref diff, ref result.Axes.YAxis, out pMin.Y);
                Vector3.Dot(ref diff, ref result.Axes.ZAxis, out pMin.Z);
                pMax = pMin;

                int upperBoundExclusive = offset + count;
                for (int i = offset + 1; i < upperBoundExclusive; i++)
                {
                    pt = points[i];
                    Vector3.Subtract(ref pt, ref result.Center, out diff);
                    for (int j = 0; j < 3; j++)
                    {
                        result.Axes.GetAxis(j, out Vector3 axis);                        
                        Vector3.Dot(ref diff, ref axis, out float dot);

                        if (dot < pMin[j])
                        {
                            pMin[j] = dot;
                        }
                        else if (dot > pMax[j])
                        {
                            pMax[j] = dot;
                        }
                    }
                }

                // Calculate new center
                for (int j = 0; j < 3; j++)
                {
                    Vector3 axis = result.Axes[j];
                    Vector3.Multiply(ref axis, 0.5f * (pMin[j] + pMax[j]), out Vector3 temp);
                    Vector3.Add(ref result.Center, ref temp, out result.Center);
                }

                result.Extents.X = 0.5f * (pMax[0] - pMin[0]);
                result.Extents.Y = 0.5f * (pMax[1] - pMin[1]);
                result.Extents.Z = 0.5f * (pMax[2] - pMin[2]);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <returns>Computed oriented bounding box.</returns>
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
            /// <param name="subMeshRange">Optional range in the index buffer.</param>
            /// <returns>Computed oriented bounding box.</returns>
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
            /// <param name="result">Computed oriented bounding box.</param>
            public static void FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, out Data result)
            {
                FromIndexedPoints(points, indices, null, out result);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <param name="subMeshRange">Optional range in the index buffer.</param>
            /// <param name="result">Computed oriented bounding box.</param>
            public static void FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange, out Data result)
            {
                if (!ExtractSubMeshRange(points, indices, subMeshRange, out int offset, out int count, out int baseVertexOffset))
                {
                    result = new Data();
                    return;
                }

                int index = indices[offset];
                Vector3 pt = points[index + baseVertexOffset];

                // Trivial case
                if (count == 1)
                {
                    result.Center = pt;
                    result.Axes = Triad.UnitAxes;
                    result.Extents = Vector3.Zero;
                    return;
                }

                GeometricToolsHelper.GaussPointsFit(points, indices, offset, count, baseVertexOffset, out result);

                Vector3 pMin, pMax;
                Vector3.Subtract(ref pt, ref result.Center, out Vector3 diff);
                Vector3.Dot(ref diff, ref result.Axes.XAxis, out pMin.X);
                Vector3.Dot(ref diff, ref result.Axes.YAxis, out pMin.Y);
                Vector3.Dot(ref diff, ref result.Axes.ZAxis, out pMin.Z);
                pMax = pMin;

                int upperBoundExclusive = offset + count;
                for (int i = offset + 1; i < upperBoundExclusive; i++)
                {
                    index = indices[i];
                    pt = points[index + baseVertexOffset];
                    Vector3.Subtract(ref pt, ref result.Center, out diff);
                    for (int j = 0; j < 3; j++)
                    {
                        result.Axes.GetAxis(j, out Vector3 axis);
                        Vector3.Dot(ref diff, ref axis, out float dot);

                        if (dot < pMin[j])
                        {
                            pMin[j] = dot;
                        }
                        else if (dot > pMax[j])
                        {
                            pMax[j] = dot;
                        }
                    }
                }

                // Calculate new center
                for (int j = 0; j < 3; j++)
                {
                    Vector3 axis = result.Axes[j];
                    Vector3.Multiply(ref axis, 0.5f * (pMin[j] + pMax[j]), out Vector3 temp);
                    Vector3.Add(ref result.Center, ref temp, out result.Center);
                }

                result.Extents.X = 0.5f * (pMax[0] - pMin[0]);
                result.Extents.Y = 0.5f * (pMax[1] - pMin[1]);
                result.Extents.Z = 0.5f * (pMax[2] - pMin[2]);
            }

            #region OBB Calculations

            // TODO - Eberly's Min box implementation using Convex Hulls
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static unsafe void ComputeMinBox(Vector3* points, int numPoints, IndexData? indices, out Data result)
            {
                throw new NotImplementedException();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static unsafe void ComputeCorners(ref Data box, Vector3* points)
            {
                Vector3.Multiply(ref box.Axes.XAxis, box.Extents.X, out Vector3 exAxis);
                Vector3.Multiply(ref box.Axes.YAxis, box.Extents.Y, out Vector3 eyAxis);
                Vector3.Multiply(ref box.Axes.ZAxis, box.Extents.Z, out Vector3 ezAxis);
                
                Vector3.Subtract(ref box.Center, ref exAxis, out Vector3 temp);
                Vector3.Subtract(ref temp, ref eyAxis, out temp);
                Vector3.Subtract(ref temp, ref ezAxis, out temp);
                points[0] = temp;

                Vector3.Add(ref box.Center, ref exAxis, out temp);
                Vector3.Subtract(ref temp, ref eyAxis, out temp);
                Vector3.Subtract(ref temp, ref ezAxis, out temp);
                points[1] = temp;

                Vector3.Add(ref box.Center, ref exAxis, out temp);
                Vector3.Add(ref temp, ref eyAxis, out temp);
                Vector3.Subtract(ref temp, ref ezAxis, out temp);
                points[2] = temp;

                Vector3.Subtract(ref box.Center, ref exAxis, out temp);
                Vector3.Add(ref temp, ref eyAxis, out temp);
                Vector3.Subtract(ref temp, ref ezAxis, out temp);
                points[3] = temp;

                Vector3.Add(ref box.Center, ref exAxis, out temp);
                Vector3.Subtract(ref temp, ref eyAxis, out temp);
                Vector3.Add(ref temp, ref ezAxis, out temp);
                points[4] = temp;

                Vector3.Subtract(ref box.Center, ref exAxis, out temp);
                Vector3.Subtract(ref temp, ref eyAxis, out temp);
                Vector3.Add(ref temp, ref ezAxis, out temp);
                points[5] = temp;

                Vector3.Add(ref box.Center, ref exAxis, out temp);
                Vector3.Add(ref temp, ref eyAxis, out temp);
                Vector3.Add(ref temp, ref ezAxis, out temp);
                points[6] = temp;

                Vector3.Subtract(ref box.Center, ref exAxis, out temp);
                Vector3.Add(ref temp, ref eyAxis, out temp);
                Vector3.Add(ref temp, ref ezAxis, out temp);
                points[7] = temp;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static unsafe void FromPoints(Vector3* points, int numPts, out Data result)
            {
                if (points == null || numPts == 0)
                {
                    result = new Data();
                    return;
                }

                Vector3 pt = points[0];

                if (numPts == 1)
                {
                    result.Center = pt;
                    result.Axes = Triad.UnitAxes;
                    result.Extents = Vector3.Zero;
                    return;
                }

                GeometricToolsHelper.GaussPointsFit(points, numPts, out result);

                Vector3 pMin, pMax;
                Vector3.Subtract(ref pt, ref result.Center, out Vector3 diff);
                Vector3.Dot(ref diff, ref result.Axes.XAxis, out pMin.X);
                Vector3.Dot(ref diff, ref result.Axes.YAxis, out pMin.Y);
                Vector3.Dot(ref diff, ref result.Axes.ZAxis, out pMin.Z);
                pMax = pMin;

                for (int i = 1; i < numPts; i++)
                {
                    pt = points[i];
                    Vector3.Subtract(ref pt, ref result.Center, out diff);
                    for (int j = 0; j < 3; j++)
                    {
                        result.Axes.GetAxis(j, out Vector3 axis);
                        Vector3.Dot(ref diff, ref axis, out float dot);

                        if (dot < pMin[j])
                        {
                            pMin[j] = dot;
                        }
                        else if (dot > pMax[j])
                        {
                            pMax[j] = dot;
                        }
                    }
                }

                // Calculate new center
                for (int j = 0; j < 3; j++)
                {
                    Vector3 axis = result.Axes[j];
                    Vector3.Multiply(ref axis, 0.5f * (pMin[j] + pMax[j]), out Vector3 temp);
                    Vector3.Add(ref result.Center, ref temp, out result.Center);
                }

                result.Extents.X = 0.5f * (pMax[0] - pMin[0]);
                result.Extents.Y = 0.5f * (pMax[1] - pMin[1]);
                result.Extents.Z = 0.5f * (pMax[2] - pMin[2]);
            }

            #endregion

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
            /// Tests equality between this oriented bounding box and another.
            /// </summary>
            /// <param name="other">Oriented bounding box</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(Data other)
            {
                return Center.Equals(ref other.Center) && Axes.Equals(ref other.Axes) && Extents.Equals(ref other.Extents);
            }

            /// <summary>
            /// Tests equality between this oriented bounding box and another.
            /// </summary>
            /// <param name="other">Oriented bounding box</param>
            /// <param name="tolerance">Tolerance</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(Data other, float tolerance)
            {
                return Center.Equals(ref other.Center, tolerance) && Axes.Equals(ref other.Axes, tolerance) && Extents.Equals(ref other.Extents, tolerance);
            }

            /// <summary>
            /// Tests equality between this oriented bounding box and another.
            /// </summary>
            /// <param name="other">Oriented bounding box</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(ref Data other)
            {
                return Center.Equals(ref other.Center) && Axes.Equals(ref other.Axes) && Extents.Equals(ref other.Extents);
            }

            /// <summary>
            /// Tests equality between this oriented bounding box and another.
            /// </summary>
            /// <param name="other">Oriented bounding box</param>
            /// <param name="tolerance">Tolerance</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(ref Data other, float tolerance)
            {
                return Center.Equals(ref other.Center, tolerance) && Axes.Equals(ref other.Axes, tolerance) && Extents.Equals(ref other.Extents, tolerance);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return Center.GetHashCode() + Axes.GetHashCode() + Extents.GetHashCode();
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

                return string.Format(formatProvider, "Center: {0}, {1}, Extents: {2}", new object[] { Center.ToString(format, formatProvider), Axes.ToString(format, formatProvider), Extents.ToString(format, formatProvider) });
            }

            /// <summary>
            /// Reads the specified input.
            /// </summary>
            /// <param name="input">The input.</param>
            void IPrimitiveValue.Read(IPrimitiveReader input)
            {
                Center = input.Read<Vector3>();
                Axes = input.Read<Triad>();
                Extents = input.Read<Vector3>();
            }

            /// <summary>
            /// Writes the primitive data to the output.
            /// </summary>
            /// <param name="output">Primitive writer</param>
            void IPrimitiveValue.Write(IPrimitiveWriter output)
            {
                output.Write("Center", Center);
                output.Write("Axes", Axes);
                output.Write("Extents", Extents);
            }
        }

        #endregion
    }
}
