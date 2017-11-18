namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    using Content;
    using Graphics;

    /// <summary>
    /// Represents a Bounding Capsule that is a cylinder capped with two half spheres. The capsule is defined by a center line segment and a radius.
    /// </summary>
    public sealed class BoundingCapsule : BoundingVolume
    {
        private Segment _centerLine;
        private float _radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingCapsule"/> class.
        /// </summary>
        public BoundingCapsule()
        {
            _centerLine.StartPoint = Vector3.Zero;
            _centerLine.EndPoint = Vector3.Zero;
            _radius = 0.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingCapsule"/> class.
        /// </summary>
        /// <param name="startPoint">Center line start point of the bounding capsule.</param>
        /// <param name="endPoint">Center line end point of the bounding capsule.</param>
        /// <param name="radius">Radius of the bounding capsule.</param>
        public BoundingCapsule(Vector3 startPoint, Vector3 endPoint, float radius)
        {
            _centerLine.StartPoint = startPoint;
            _centerLine.EndPoint = endPoint;
            _radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingCapsule"/> class.
        /// </summary>
        /// <param name="centerLine">Center line segment of the bounding capsule</param>
        /// <param name="radius">Radius of the bounding capsule.</param>
        public BoundingCapsule(Segment centerLine, float radius)
        {
            _centerLine = centerLine;
            _radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingCapsule"/> class.
        /// </summary>
        /// <param name="boundingCapsuleData">Bounding capsule data.</param>
        public BoundingCapsule(Data boundingCapsuleData)
        {
            _centerLine = boundingCapsuleData.CenterLine;
            _radius = boundingCapsuleData.Radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingCapsule"/> class.
        /// </summary>
        /// <param name="boundingCapsule">Bounding capsule to copy from.</param>
        public BoundingCapsule(BoundingCapsule boundingCapsule)
        {
            if (boundingCapsule == null)
            {
                throw new ArgumentNullException(nameof(boundingCapsule));
            }

            _centerLine = boundingCapsule._centerLine;
            _radius = boundingCapsule._radius;
        }

        /// <summary>
        /// Gets or sets the center of the bounding volume.
        /// </summary>
        public override Vector3 Center
        {
            get => _centerLine.Center;
            set
            {
                _centerLine.Center = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets or sets the center line start point of the bounding capsule.
        /// </summary>
        public Vector3 StartPoint
        {
            get => _centerLine.StartPoint;
            set
            {
                _centerLine.StartPoint = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets or sets the center line end point of the bounding capsule.
        /// </summary>
        public Vector3 EndPoint
        {
            get => _centerLine.EndPoint;
            set
            {
                _centerLine.EndPoint = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets or sets the center line segment of the bounding capsule.
        /// </summary>
        public Segment CenterLine
        {
            get => _centerLine;
            set
            {
                _centerLine = value;
                UpdateCorners = true;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the bounding capsule.
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
        public override float Volume => MathHelper.Pi * (_radius * _radius) * ((MathHelper.FourThirds * _radius) + _centerLine.Length);

        /// <summary>
        /// Gets the number of corners.
        /// </summary>
        public override int CornerCount => 8;

        /// <summary>
        /// Gets the bounding type.
        /// </summary>
        public override BoundingType BoundingType => BoundingType.Capsule;

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
                        Vector3 center = other.Center;
                        Vector3 extents = other.Extents;
                        Triad axes = Triad.UnitAxes;

                        GeometricToolsHelper.CapsuleFromBox(ref center, ref axes.XAxis, ref axes.YAxis, ref axes.ZAxis, ref extents, out _centerLine, out _radius);
                    }
                    break;

                case BoundingType.Sphere:
                    {
                        // Sphere is just a degenerate capsule
                        BoundingSphere other = volume as BoundingSphere;
                        Vector3 center = other.Center;
                        _centerLine.StartPoint = center;
                        _centerLine.EndPoint = center;
                        _radius = other.Radius;
                    }
                    break;

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;
                        _centerLine = other._centerLine;
                        _radius = other._radius;
                    }
                    break;

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        Vector3 center = other.Center;
                        Vector3 extents = other.Extents;
                        Triad axes = other.Axes;

                        GeometricToolsHelper.CapsuleFromBox(ref center, ref axes.XAxis, ref axes.YAxis, ref axes.ZAxis, ref extents, out _centerLine, out _radius);
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
        /// Creates a copy of the bounding volume and returns a new instance.
        /// </summary>
        /// <returns>Copied bounding volume</returns>
        public override BoundingVolume Clone()
        {
            return new BoundingCapsule(this);
        }

        /// <summary>
        /// Computes the distance from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distance from the point to the edge of the volume.</param>
        public override void DistanceTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointSegment(ref point, ref _centerLine, out Vector3 ptOnCenterLine, out float segParameter, out float distSquared);
            result = Math.Max(0.0f, (float)Math.Sqrt(distSquared) - _radius);
        }

        /// <summary>
        /// Computes the distance squared from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distanced squared from the point to the edge of the volume.</param>
        public override void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            GeometricToolsHelper.DistancePointSegment(ref point, ref _centerLine, out Vector3 ptOnCenterLine, out float segParameter, out float distSquared);
            result = Math.Max(0.0f, distSquared - (_radius * _radius));
        }

        /// <summary>
        /// Computes the closest point on the volume from the given point in space.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Closest point on the edge of the volume.</param>
        public override void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            GeometricToolsHelper.DistancePointSegment(ref point, ref _centerLine, out Vector3 ptOnCenterLine, out float segParameter, out float distSquared);

            // Essentially becomes like bounding sphere, but with a point along the center line that the input point is closest to
            Vector3.Subtract(ref point, ref ptOnCenterLine, out Vector3 v);
            v.Normalize();

            Vector3.Multiply(ref v, _radius, out v);
            Vector3.Add(ref ptOnCenterLine, ref v, out result);
        }

        /// <summary>
        /// Computes a minimum bounding volume that encloses the specified points in space.
        /// </summary>
        /// <param name="points">Points in space</param>
        /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
        public override void ComputeFromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
        {
            Data.FromPoints(points, subMeshRange, out Data data);

            _centerLine = data.CenterLine;
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
            Data.FromIndexedPoints(points, indices, subMeshRange, out Data data);

            _centerLine = data.CenterLine;
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
            GeometricToolsHelper.DistancePointSegment(ref point, ref _centerLine, out Vector3 ptOnCenterLine, out float segParameter, out float distSquared);

            if (distSquared <= (_radius * _radius))
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
            GeometricToolsHelper.DistancePointSegment(ref line.StartPoint, ref _centerLine, out Vector3 ptOnCenterLine, out float segParameter, out float distStart);
            GeometricToolsHelper.DistancePointSegment(ref line.EndPoint, ref _centerLine, out ptOnCenterLine, out segParameter, out float distEnd);

            float radiusSquared = _radius * _radius;
            if (distStart <= radiusSquared && distEnd <= radiusSquared)
            {
                return ContainmentType.Inside;
            }

            if (GeometricToolsHelper.IntersectSegmentCapsule(ref line, ref _centerLine, _radius))
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
            GeometricToolsHelper.DistancePointSegment(ref triangle.PointA, ref _centerLine, out Vector3 ptOnCenterLine, out float segParameter, out float distA);
            GeometricToolsHelper.DistancePointSegment(ref triangle.PointB, ref _centerLine, out ptOnCenterLine, out segParameter, out float distB);
            GeometricToolsHelper.DistancePointSegment(ref triangle.PointC, ref _centerLine, out ptOnCenterLine, out segParameter, out float distC);

            float radiusSquared = _radius * _radius;
            if (distA <= radiusSquared && distB <= radiusSquared && distC <= radiusSquared)
            {
                return ContainmentType.Inside;
            }

            if (GeometricToolsHelper.IntersectTriangleCapsule(ref triangle, ref _centerLine, _radius))
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

                        return GeometricToolsHelper.CapsuleContainsAABB(ref _centerLine, _radius, ref boxCenter, ref boxExtents);
                    }

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        Vector3 sphereCenter = other.Center;
                        float sphereRadius = other.Radius;

                        return GeometricToolsHelper.CapsuleContainsSphere(ref _centerLine, _radius, ref sphereCenter, sphereRadius);
                    }

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;

                        return GeometricToolsHelper.CapsuleContainsCapsule(ref _centerLine, _radius, ref other._centerLine, other._radius);
                    }

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        Vector3 boxCenter = other.Center;
                        Triad boxAxes = other.Axes;
                        Vector3 boxExtents = other.Extents;

                        return GeometricToolsHelper.CapsuleContainsBox(ref _centerLine, _radius, ref boxCenter, ref boxAxes.XAxis, ref boxAxes.YAxis, ref boxAxes.ZAxis, ref boxExtents);
                    }

                case BoundingType.Frustum:
                case BoundingType.Mesh:
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
            return GeometricToolsHelper.IntersectRayCapsule(ref ray, ref _centerLine, _radius);
        }

        /// <summary>
        /// Determines if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public override bool Intersects(ref Ray ray, out BoundingIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectRayCapsule(ref ray, ref _centerLine, _radius, out result);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line)
        {
            return GeometricToolsHelper.IntersectSegmentCapsule(ref line, ref _centerLine, _radius);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line, out BoundingIntersectionResult result)
        {
            return GeometricToolsHelper.IntersectSegmentCapsule(ref line, ref _centerLine, _radius, out result);
        }

        /// <summary>
        /// Tests if the specified plane intersects with the bounding volume.
        /// </summary>
        /// <param name="plane">Plane to test against.</param>
        /// <returns>Type of plane intersection.</returns>
        public override PlaneIntersectionType Intersects(ref Plane plane)
        {
            plane.SignedDistanceTo(ref _centerLine.EndPoint, out float pDist);
            plane.SignedDistanceTo(ref _centerLine.StartPoint, out float nDist);
            if ((pDist * nDist) <= 0.0f)
            {
                // Capsule end points on opposite sides of the plane
                return PlaneIntersectionType.Intersects;
            }

            // Endpoints on same side, but may still overlap
            if (Math.Abs(pDist) <= _radius || Math.Abs(nDist) <= _radius)
            {
                return PlaneIntersectionType.Intersects;
            }
            // No overlap, so front or back. This is signed distance, so use the Bounding sphere test? > radius == front, doesn't
            // matter which since both are on the same side
            else if (pDist > _radius)
            {
                return PlaneIntersectionType.Front;
            }
            else
            {
                return PlaneIntersectionType.Back;
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

                        return GeometricToolsHelper.IntersectCapsuleAABB(ref _centerLine, _radius, ref boxCenter, ref boxExtents);
                    }

                case BoundingType.Sphere:
                    {
                        BoundingSphere other = volume as BoundingSphere;
                        Vector3 sphereCenter = other.Center;
                        float sphereRadius = other.Radius;

                        return GeometricToolsHelper.IntersectCapsuleSphere(ref _centerLine, _radius, ref sphereCenter, sphereRadius);
                    }

                case BoundingType.Capsule:
                    {
                        BoundingCapsule other = volume as BoundingCapsule;

                        return GeometricToolsHelper.IntersectCapsuleCapsule(ref _centerLine, _radius, ref other._centerLine, other._radius);
                    }

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;
                        Vector3 boxCenter = other.Center;
                        Triad boxAxes = other.Axes;
                        Vector3 boxExtents = other.Extents;

                        return GeometricToolsHelper.IntersectCapsuleBox(ref _centerLine, _radius, ref boxCenter, ref boxAxes.XAxis, ref boxAxes.YAxis, ref boxAxes.ZAxis, ref boxExtents);
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

            // Treat capsule as an OBB, treat the others as OBB and merge, then extract capsule from merged OBB
            GeometricToolsHelper.CalculateSegmentProperties(ref _centerLine, out Vector3 segCenter, out Vector3 segDir, out float segExtent);

            OrientedBoundingBox.Data b0;
            b0.Center = segCenter;
            Triad.FromZComplementBasis(ref segDir, out b0.Axes);
            b0.Extents = new Vector3(_radius, _radius, segExtent + _radius);

            OrientedBoundingBox.Data b1;
            OrientedBoundingBox.Data merged;

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

                        GeometricToolsHelper.CalculateSegmentProperties(ref centerLine, out segCenter, out segDir, out segExtent);

                        b1.Center = segCenter;
                        Triad.FromZComplementBasis(ref segDir, out b1.Axes);
                        b1.Extents = new Vector3(capsuleRadius, capsuleRadius, segExtent + capsuleRadius);
                    }
                    break;

                case BoundingType.OrientedBoundingBox:
                    {
                        OrientedBoundingBox other = volume as OrientedBoundingBox;

                        b1.Center = other.Center;
                        b1.Axes = other.Axes;
                        b1.Extents = other.Extents;
                    }
                    break;
                    
                default:
                    {
                        OrientedBoundingBox.Data.FromPoints(volume.Corners, out b1);
                    }
                    break;
            }

            GeometricToolsHelper.MergeBoxBoxUsingCorners(ref b0, ref b1, out merged);
            GeometricToolsHelper.CapsuleFromBox(ref merged.Center, ref merged.Axes.XAxis, ref merged.Axes.YAxis, ref merged.Axes.ZAxis, ref merged.Extents, out _centerLine, out _radius);
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
            // Scale centerline extent and radius
            float maxScale = Math.Max(Math.Abs(scale.X), Math.Max(Math.Abs(scale.Y), Math.Abs(scale.Z)));
            if (!MathHelper.IsApproxEquals(maxScale, 1.0f))
            {
                Segment.Transform(ref _centerLine, maxScale, out _centerLine);
                _radius = _radius * maxScale;
            }

            // Transform centerline by Rotation-Translation
            Matrix4x4.FromQuaternion(ref rotation, out Matrix4x4 rotTranslate);
            Matrix4x4.FromTranslation(ref translation, out Matrix4x4 temp);
            Matrix4x4.Multiply(ref rotTranslate, ref temp, out rotTranslate);

            Segment.Transform(ref _centerLine, ref rotTranslate, out _centerLine);

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
            BoundingCapsule capsule = other as BoundingCapsule;
            if (capsule != null)
            {
                return _centerLine.Equals(ref capsule._centerLine, tolerance) && MathHelper.IsApproxEquals(_radius, capsule._radius, tolerance);
            }

            return false;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            _centerLine = input.Read<Segment>();
            _radius = input.ReadSingle();
            UpdateCorners = true;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            output.Write("CenterLine", _centerLine);
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

            // Axes of the capsule
            Vector3 xAxis, yAxis, zAxis;
            if (_centerLine.IsDegenerate)
            {
                zAxis = Vector3.UnitZ;
            }
            else
            {
                Vector3.Subtract(ref _centerLine.EndPoint, ref _centerLine.StartPoint, out zAxis);
            }

            Vector3.ComplementBasis(ref zAxis, out xAxis, out yAxis);

            Vector3.Multiply(ref xAxis, _radius, out xAxis);
            Vector3.Multiply(ref yAxis, _radius, out yAxis);
            Vector3.Multiply(ref zAxis, _radius, out zAxis);

            // Sphere at start point (-R along Z axis)
            Vector3.Subtract(ref _centerLine.StartPoint, ref xAxis, out Vector3 temp);
            Vector3.Add(ref temp, ref yAxis, out temp);
            Vector3.Subtract(ref temp, ref zAxis, out temp);
            corners.Set(ref temp);

            Vector3.Add(ref _centerLine.StartPoint, ref xAxis, out temp);
            Vector3.Add(ref temp, ref yAxis, out temp);
            Vector3.Subtract(ref temp, ref zAxis, out temp);
            corners.Set(ref temp);

            Vector3.Add(ref _centerLine.StartPoint, ref xAxis, out temp);
            Vector3.Subtract(ref temp, ref yAxis, out temp);
            Vector3.Subtract(ref temp, ref zAxis, out temp);
            corners.Set(ref temp);

            Vector3.Subtract(ref _centerLine.StartPoint, ref xAxis, out temp);
            Vector3.Subtract(ref temp, ref yAxis, out temp);
            Vector3.Subtract(ref temp, ref zAxis, out temp);
            corners.Set(ref temp);

            // Sphere at end point (+R along Z axis)
            Vector3.Subtract(ref _centerLine.EndPoint, ref xAxis, out temp);
            Vector3.Add(ref temp, ref yAxis, out temp);
            Vector3.Add(ref temp, ref zAxis, out temp);
            corners.Set(ref temp);

            Vector3.Add(ref _centerLine.EndPoint, ref xAxis, out temp);
            Vector3.Add(ref temp, ref yAxis, out temp);
            Vector3.Add(ref temp, ref zAxis, out temp);
            corners.Set(ref temp);

            Vector3.Add(ref _centerLine.EndPoint, ref xAxis, out temp);
            Vector3.Subtract(ref temp, ref yAxis, out temp);
            Vector3.Add(ref temp, ref zAxis, out temp);
            corners.Set(ref temp);

            Vector3.Subtract(ref _centerLine.EndPoint, ref xAxis, out temp);
            Vector3.Subtract(ref temp, ref yAxis, out temp);
            Vector3.Add(ref temp, ref zAxis, out temp);
            corners.Set(ref temp);
        }

        #region BoundingCapsule Data

        /// <summary>
        /// Represents data for a <see cref="BoundingCapsule"/>.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct Data : IEquatable<Data>, IFormattable, IPrimitiveValue
        {
            /// <summary>
            /// Center line segment of the bounding capsule.
            /// </summary>
            public Segment CenterLine;

            /// <summary>
            /// Radius of the bounding capsule.
            /// </summary>
            public float Radius;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="centerLine">Center line segment of the bounding capsule.</param>
            /// <param name="radius">Radius of the bounding capsule.</param>
            public Data(Segment centerLine, float radius)
            {
                CenterLine = centerLine;
                Radius = radius;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data"/> struct.
            /// </summary>
            /// <param name="capsule">Bounding capsule to copy from.</param>
            public Data(BoundingCapsule capsule)
            {
                CenterLine = capsule._centerLine;
                Radius = capsule._radius;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <returns>Computed bounding capsule.</returns>
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
            /// <returns>Computed bounding capsule.</returns>
            public static Data FromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
            {
                FromPoints(points, subMeshRange, out Data result);
                return result;
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <param name="result">Computed bounding capsule</param>
            public static void FromPoints(IReadOnlyDataBuffer<Vector3> points, out Data result)
            {
                FromPoints(points, null, out result);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified points in space.
            /// </summary>
            /// <param name="points">Points in space</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
            /// <param name="result">Computed bounding capsule</param>
            public static void FromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange, out Data result)
            {
                if (!ExtractSubMeshRange(points, subMeshRange, out int offset, out int count))
                {
                    result.CenterLine = new Segment(Vector3.Zero, Vector3.Zero);
                    result.Radius = 0.0f;

                    return;
                }

                // Trivial case
                if (count == 1)
                {
                    Vector3 pt = points[offset];
                    result.CenterLine = new Segment(pt, pt);
                    result.Radius = 0.0f;
                    return;
                }
                
                GeometricToolsHelper.OrthogonalLineFit(points, offset, count, out Vector3 lineOrigin, out Vector3 lineDir);

                int upperBoundExclusive = offset + count;
                float maxRadiusSqr = 0.0f;
                for (int i = offset; i < upperBoundExclusive; i++)
                {
                    Vector3 pt = points[i];
                    GeometricToolsHelper.DistancePointLine(ref pt, ref lineOrigin, ref lineDir, out Vector3 ptOnLine, out float lineParameter, out float rSqr);

                    if (rSqr > maxRadiusSqr)
                    {
                        maxRadiusSqr = rSqr;
                    }
                }

                Vector3 W = lineDir;

                Vector3.ComplementBasis(ref W, out Vector3 U, out Vector3 V);

                float minValue = float.MaxValue;
                float maxValue = float.MinValue;
                for (int i = offset; i < upperBoundExclusive; i++)
                {
                    Vector3 pt = points[i];
                    Vector3.Subtract(ref pt, ref lineOrigin, out Vector3 diff);
                    
                    Vector3.Dot(ref U, ref diff, out float UdDiff);
                    Vector3.Dot(ref V, ref diff, out float VdDiff);
                    Vector3.Dot(ref W, ref diff, out float WdDiff);

                    float discr = maxRadiusSqr - ((UdDiff * UdDiff) + (VdDiff * VdDiff));
                    float radical = (float)Math.Sqrt(Math.Abs(discr));

                    float test = WdDiff + radical;
                    if (test < minValue)
                    {
                        minValue = test;
                    }

                    test = WdDiff - radical;
                    if (test > maxValue)
                    {
                        maxValue = test;
                    }
                }
                
                Vector3.Multiply(ref lineDir, 0.5f * (minValue + maxValue), out Vector3 center);
                Vector3.Add(ref center, ref lineOrigin, out center);

                // max > min, is a capsule, otherwise degenerates into a sphere
                float extent = (maxValue > minValue) ? (0.5f * (maxValue - minValue)) : 0.0f;

                result.Radius = (float)Math.Sqrt(maxRadiusSqr);
                Segment.FromCenterExtent(ref center, ref lineDir, extent, out result.CenterLine);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <returns>Computed bounding capsule.</returns>
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
            /// <returns>Computed bounding capsule.</returns>
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
            /// <param name="result">Computed bounding capsule.</param>
            public static void FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, out Data result)
            {
                FromIndexedPoints(points, indices, null, out result);
            }

            /// <summary>
            /// Computes a minimum bounding volume that encloses the specified indexed points in space.
            /// </summary>
            /// <param name="points">Points in space.</param>
            /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for.</param>
            /// <param name="indices">Point indices denoting location in the point buffer.</param>
            /// <param name="result">Computed bounding capsule.</param>
            public static void FromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange, out Data result)
            {
                if (!ExtractSubMeshRange(points, indices, subMeshRange, out int offset, out int count, out int baseVertexOffset))
                {
                    result.CenterLine = new Segment(Vector3.Zero, Vector3.Zero);
                    result.Radius = 0.0f;

                    return;
                }

                // Trivial case
                if (count == 1)
                {
                    Vector3 pt = points[indices[offset] + baseVertexOffset];
                    result.CenterLine = new Segment(pt, pt);
                    result.Radius = 0.0f;
                    return;
                }

                GeometricToolsHelper.OrthogonalLineFit(points, indices, offset, count, baseVertexOffset, out Vector3 lineOrigin, out Vector3 lineDir);

                int upperBoundExclusive = offset + count;
                float maxRadiusSqr = 0.0f;
                for (int i = offset; i < upperBoundExclusive; i++)
                {
                    int index = indices[i];
                    Vector3 pt = points[index + baseVertexOffset];
                    GeometricToolsHelper.DistancePointLine(ref pt, ref lineOrigin, ref lineDir, out Vector3 ptOnLine, out float lineParameter, out float rSqr);

                    if (rSqr > maxRadiusSqr)
                    {
                        maxRadiusSqr = rSqr;
                    }
                }

                Vector3 W = lineDir;

                Vector3.ComplementBasis(ref W, out Vector3 U, out Vector3 V);

                float minValue = float.MaxValue;
                float maxValue = float.MinValue;
                for (int i = offset; i < upperBoundExclusive; i++)
                {
                    int index = indices[i];
                    Vector3 pt = points[index + baseVertexOffset];
                    Vector3.Subtract(ref pt, ref lineOrigin, out Vector3 diff);
                    
                    Vector3.Dot(ref U, ref diff, out float UdDiff);
                    Vector3.Dot(ref V, ref diff, out float VdDiff);
                    Vector3.Dot(ref W, ref diff, out float WdDiff);

                    float discr = maxRadiusSqr - ((UdDiff * UdDiff) + (VdDiff * VdDiff));
                    float radical = (float)Math.Sqrt(Math.Abs(discr));

                    float test = WdDiff + radical;
                    if (test < minValue)
                    {
                        minValue = test;
                    }

                    test = WdDiff - radical;
                    if (test > maxValue)
                    {
                        maxValue = test;
                    }
                }
                
                Vector3.Multiply(ref lineDir, 0.5f * (minValue + maxValue), out Vector3 center);
                Vector3.Add(ref center, ref lineOrigin, out center);

                // max > min, is a capsule, otherwise degenerates into a sphere
                float extent = (maxValue > minValue) ? (0.5f * (maxValue - minValue)) : 0.0f;

                result.Radius = (float)Math.Sqrt(maxRadiusSqr);
                Segment.FromCenterExtent(ref center, ref lineDir, extent, out result.CenterLine);
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
            /// Tests equality between this bounding capsule and another.
            /// </summary>
            /// <param name="other">Bounding capsule</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(Data other)
            {
                return CenterLine.Equals(ref other.CenterLine) && MathHelper.IsApproxEquals(Radius, other.Radius);
            }

            /// <summary>
            /// Tests equality between this bounding capsule and another.
            /// </summary>
            /// <param name="other">Bounding capsule</param>
            /// <param name="tolerance">Tolerance</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(Data other, float tolerance)
            {
                return CenterLine.Equals(ref other.CenterLine, tolerance) && MathHelper.IsApproxEquals(Radius, other.Radius, tolerance);
            }

            /// <summary>
            /// Tests equality between this bounding capsule and another.
            /// </summary>
            /// <param name="other">Bounding capsule</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(ref Data other)
            {
                return CenterLine.Equals(ref other.CenterLine) && MathHelper.IsApproxEquals(Radius, other.Radius);
            }

            /// <summary>
            /// Tests equality between this bounding capsule and another.
            /// </summary>
            /// <param name="other">Bounding capsule</param>
            /// <param name="tolerance">Tolerance</param>
            /// <returns>True if equal, false otherwise.</returns>
            public bool Equals(ref Data other, float tolerance)
            {
                return CenterLine.Equals(ref other.CenterLine, tolerance) && MathHelper.IsApproxEquals(Radius, other.Radius, tolerance);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return CenterLine.GetHashCode() + Radius.GetHashCode();
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

                return string.Format(formatProvider, "CenterLine: {0}, Radius: {1}", new object[] { CenterLine.ToString(format, formatProvider), Radius.ToString(format, formatProvider) });
            }

            /// <summary>
            /// Reads the specified input.
            /// </summary>
            /// <param name="input">The input.</param>
            void IPrimitiveValue.Read(IPrimitiveReader input)
            {
                CenterLine = input.Read<Segment>();
                Radius = input.ReadSingle();
            }

            /// <summary>
            /// Writes the primitive data to the output.
            /// </summary>
            /// <param name="output">Primitive writer</param>
            void IPrimitiveValue.Write(IPrimitiveWriter output)
            {
                output.Write("CenterLine", CenterLine);
                output.Write("Radius", Radius);
            }
        }

        #endregion
    }
}
