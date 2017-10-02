namespace Spark.Math
{
    using System;
    using System.Diagnostics;

    using Core;
    using Content;

    /// <summary>
    /// Represents an orthogonal view frustum. The frustum is defined by six planes (left, right, top, bottom, near, and far in that order) and looks
    /// like a pyramid with its top sliced off. A frustum is generally represents the viewable volume of a camera system and is used in culling and other
    /// intersection tests to query what is visible or not in a scene.
    /// </summary>
    public sealed class BoundingFrustum : BoundingVolume
    {
        // Made nullable because to calculate we're using the corners, which can be expensive to calculate...so only calculate when
        // needed and cache
        private Vector3? _center;
        private Matrix4x4 _viewProjMatrix;
        private readonly Plane[] _planes;

        /// <summary>
        /// Initialize a new instance of the <see cref="BoundingFrustum"/> class.
        /// </summary>
        public BoundingFrustum()
        {
            _center = null; // Defer calculations
            _viewProjMatrix = Matrix4x4.Identity;
            _planes = new Plane[PlaneCount];
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="BoundingFrustum"/> class.
        /// </summary>
        /// <param name="viewProjMatrix">View-Projection matrix to construct the frustum from.</param>
        public BoundingFrustum(Matrix4x4 viewProjMatrix)
        {
            _center = null; // Defer calculations
            _viewProjMatrix = Matrix4x4.Identity;
            _planes = new Plane[PlaneCount];
            Set(ref viewProjMatrix);
        }

        /// <summary>
        /// Left frustum plane.
        /// </summary>
        public Plane Left => _planes[0];

        /// <summary>
        /// Right frustum plane.
        /// </summary>
        public Plane Right => _planes[1];

        /// <summary>
        /// Top frustum plane.
        /// </summary>
        public Plane Top => _planes[2];

        /// <summary>
        /// Bottom frustum plane.
        /// </summary>
        public Plane Bottom => _planes[3];

        /// <summary>
        /// Near frustum plane.
        /// </summary>
        public Plane Near => _planes[4];

        /// <summary>
        /// Far frustum plane.
        /// </summary>
        public Plane Far => _planes[5];

        /// <summary>
        /// Gets the number of frustum planes.
        /// </summary>
        public int PlaneCount => 6;

        /// <summary>
        /// Gets an individual frustum plane.
        /// </summary>
        /// <param name="plane">Plane to retrieve.</param>
        /// <returns>Specified frustum plane.</returns>
        public Plane this[FrustumPlane plane] => _planes[(int)plane];

        /// <summary>
        /// Gets or sets the view-projection matrix that represents this bounding frustm.
        /// </summary>
        public Matrix4x4 ViewProjectionMatrix
        {
            get => _viewProjMatrix;
            set => Set(ref value);
        }

        /// <summary>
        /// Gets or sets the center of the bounding volume.
        /// </summary>
        public override Vector3 Center
        {
            get
            {
                if (!_center.HasValue || UpdateCorners)
                {
                    ComputeCenter();
                }

                return _center.Value;
            }
            set
            {
                Vector3 center = Center; // Force a calculation
                if (center.Equals(ref value))
                {
                    return;
                }

                // Determine delta translation
                Vector3.Subtract(ref value, ref center, out Vector3 delta);

                // Transform
                Vector3 scale = Vector3.One;
                Quaternion rot = Quaternion.Identity;
                Transform(ref scale, ref rot, ref delta);

                _center = null; // Defer calculations until needed
            }
        }

        /// <summary>
        /// Gets the volume of the bounding volume.
        /// </summary>
        public override float Volume => ComputeVolume(Vector3.Distance(_planes[4].Origin, _planes[5].Origin), Corners);

        /// <summary>
        /// Gets the number of corners.
        /// </summary>
        public override int CornerCount => 8;

        /// <summary>
        /// Gets the bounding type.
        /// </summary>
        public override BoundingType BoundingType => BoundingType.Frustum;

        /// <summary>
        /// Gets the specified frustum plane.
        /// </summary>
        /// <param name="plane">Type of frustum plane to get.</param>
        /// <param name="result">Frustum plane.</param>
        public void GetPlane(FrustumPlane plane, out Plane result)
        {
            result = _planes[(int)plane];
        }

        /// <summary>
        /// Gets the origin and normal of the plane so that it is aligned to the constrained side of the frustum. This is mostly for
        /// convienence for visualization purposes.
        /// </summary>
        /// <param name="plane">Type of frustum plane to get.</param>
        /// <param name="origin">Origin of the plane on the frustum.</param>
        /// <param name="normal">Normal of the plane.</param>
        public void GetPlaneOriginOnFrustum(FrustumPlane plane, out Vector3 origin, out Vector3 normal)
        {
            origin = Vector3.Zero;
            normal = Vector3.Zero;

            IReadOnlyDataBuffer<Vector3> corners = Corners;

            Plane p = _planes[(int)plane];
            normal = p.Normal;

            // Calculate center of the side using corner points
            Vector3 pt0 = Vector3.Zero;
            Vector3 pt1 = Vector3.Zero;
            Vector3 pt2 = Vector3.Zero;
            Vector3 pt3 = Vector3.Zero;

            switch (plane)
            {
                case FrustumPlane.Near:
                    pt0 = corners[0]; pt1 = corners[1]; pt2 = corners[2]; pt3 = corners[3];
                    break;

                case FrustumPlane.Far:
                    pt0 = corners[4]; pt1 = corners[5]; pt2 = corners[6]; pt3 = corners[7];
                    break;

                case FrustumPlane.Left:
                    pt0 = corners[7]; pt1 = corners[6]; pt2 = corners[1]; pt3 = corners[0];
                    break;

                case FrustumPlane.Right:
                    pt0 = corners[3]; pt1 = corners[2]; pt2 = corners[5]; pt3 = corners[4];
                    break;

                case FrustumPlane.Top:
                    pt0 = corners[1]; pt1 = corners[6]; pt2 = corners[5]; pt3 = corners[2];
                    break;

                case FrustumPlane.Bottom:
                    pt0 = corners[4]; pt1 = corners[7]; pt2 = corners[0]; pt3 = corners[3];
                    break;
            }

            // Average the 4 points
            Vector3.Add(ref pt0, ref pt1, out origin);
            Vector3.Add(ref origin, ref pt2, out origin);
            Vector3.Add(ref origin, ref pt3, out origin);
            Vector3.Multiply(ref origin, 0.25f, out origin);
        }

        /// <summary>
        /// Sets the bounding frustum from the specified view-projection matrix.
        /// </summary>
        /// <param name="viewProjMatrix">View-Projection matrix to construct the frustum from.</param>
        public void Set(Matrix4x4 viewProjMatrix)
        {
            _viewProjMatrix = viewProjMatrix;
            ExtractPlanes(ref viewProjMatrix, _planes);
            UpdateCorners = true;
        }

        /// <summary>
        /// Sets the bounding frustum from the specified view-projection matrix.
        /// </summary>
        /// <param name="viewProjMatrix">View-Projection matrix to construct the frustum from.</param>
        public void Set(ref Matrix4x4 viewProjMatrix)
        {
            _viewProjMatrix = viewProjMatrix;
            ExtractPlanes(ref viewProjMatrix, _planes);
            UpdateCorners = true;
        }

        /// <summary>
        /// Sets the bounding volume by either copying the specified bounding volume if its the specified type. For bounding frustums, computing a new frustum to fit the volume
        /// source is not supported.
        /// </summary>
        /// <param name="volume">Bounding volume to copy from</param>
        public override void Set(BoundingVolume volume)
        {
            if (volume is BoundingFrustum)
            {
                Set(ref (volume as BoundingFrustum)._viewProjMatrix);
            }
            else
            {
                Debug.Assert(false, "Cannot set Bounding Frustum from another volume source!");
            }
        }

        /// <summary>
        /// Creates a copy of the bounding volume and returns a new instance.
        /// </summary>
        /// <returns>Copied bounding volume</returns>
        public override BoundingVolume Clone()
        {
            return new BoundingFrustum(_viewProjMatrix);
        }

        /// <summary>
        /// Computes the distance from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distance from the point to the edge of the volume.</param>
        public override void DistanceTo(ref Vector3 point, out float result)
        {
            float minDist = float.MaxValue;
            Vector3 pointOnVolume;

            for (int i = 0; i < 6; i++)
            {
                Plane p = _planes[i];
                p.ClosestPointTo(ref point, out pointOnVolume);

                if (Contains(ref pointOnVolume) == ContainmentType.Outside)
                {
                    continue;
                }
                
                Vector3.DistanceSquared(ref point, ref pointOnVolume, out float dist);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }

            result = (float)Math.Sqrt(minDist);
        }

        /// <summary>
        /// Computes the distance squared from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distanced squared from the point to the edge of the volume.</param>
        public override void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            float minDist = float.MaxValue;
            for (int i = 0; i < 6; i++)
            {
                Plane p = _planes[i];
                p.ClosestPointTo(ref point, out Vector3 pointOnVolume);

                if (Contains(ref pointOnVolume) == ContainmentType.Outside)
                {
                    continue;
                }
                
                Vector3.DistanceSquared(ref point, ref pointOnVolume, out float dist);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }

            result = minDist;
        }

        /// <summary>
        /// Computes the closest point on the volume from the given point in space.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="pointOnVolume">Closest point on the edge of the volume.</param>
        public override void ClosestPointTo(ref Vector3 point, out Vector3 pointOnVolume)
        {
            float minDist = float.MaxValue;
            Vector3 closestPt = new Vector3();
            for (int i = 0; i < 6; i++)
            {
                Plane p = _planes[i];
                p.ClosestPointTo(ref point, out pointOnVolume);

                if (Contains(ref pointOnVolume) == ContainmentType.Outside)
                {
                    continue;
                }
                
                Vector3.DistanceSquared(ref point, ref pointOnVolume, out float dist);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestPt = pointOnVolume;
                }
            }

            pointOnVolume = closestPt;
        }

        /// <summary>
        /// NOT SUPPORTED.
        /// </summary>
        /// <param name="points">Points in space</param>
        /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
        public override void ComputeFromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
        {
            Debug.Assert(false, "Cannot compute a BoundingFrustum from points!");
        }

        /// <summary>
        /// NOT SUPPORTED.
        /// </summary>
        /// <param name="points">Points in space.</param>
        /// <param name="indices">Point indices denoting location in the point buffer.</param>
        /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for.</param>
        public override void ComputeFromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange)
        {
            Debug.Assert(false, "Cannot compute a BoundingFrustum from points!");
        }

        /// <summary>
        /// Determines if the specified point is contained inside the bounding volume.
        /// </summary>
        /// <param name="point">Point to test against.</param>
        /// <returns>Type of containment</returns>
        public override ContainmentType Contains(ref Vector3 point)
        {
            PlaneIntersectionType result = PlaneIntersectionType.Front;
            for (int i = 0; i < 6; i++)
            {
                Plane p = _planes[i];
                switch (p.WhichSide(ref point))
                {
                    case PlaneIntersectionType.Back:
                        return ContainmentType.Outside; // If on negative side, then it's outside (planes pointing inward). Don't need to check the rest.
                    case PlaneIntersectionType.Intersects:
                        result = PlaneIntersectionType.Intersects; // If intersecting, it may still be outside the frustum so we need to continue checking the rest
                        break;
                }
            }

            return (result == PlaneIntersectionType.Intersects) ? ContainmentType.Intersects : ContainmentType.Inside;
        }

        /// <summary>
        /// Determines if the specified segment line is contained inside the bounding volume.
        /// </summary>
        /// <param name="line">Segment to test against.</param>
        /// <returns>Type of containment.</returns>
        public override ContainmentType Contains(ref Segment line)
        {
            ContainmentType startType = Contains(ref line.StartPoint);
            ContainmentType endType = Contains(ref line.EndPoint);

            if (startType == ContainmentType.Outside && endType == ContainmentType.Outside)
            {
                return ContainmentType.Outside;
            }

            if (startType == ContainmentType.Inside && endType == ContainmentType.Inside)
            {
                return ContainmentType.Inside;
            }

            return ContainmentType.Intersects;
        }

        /// <summary>
        /// Determines if the specified triangle is contained inside the bounding volume.
        /// </summary>
        /// <param name="triangle">Triangle to test against.</param>
        /// <returns>Type of containment.</returns>
        public override ContainmentType Contains(ref Triangle triangle)
        {
            ContainmentType aType = Contains(ref triangle.PointA);
            ContainmentType bType = Contains(ref triangle.PointB);
            ContainmentType cType = Contains(ref triangle.PointC);

            // Triangle is completely contained inside
            if (aType == ContainmentType.Inside && bType == ContainmentType.Inside && cType == ContainmentType.Inside)
            {
                return ContainmentType.Inside;
            }

            // Still need to check each edge (line segment) to see if it intersects, we can have all three vertices outside and the triangle may still intersect
            Segment edgeBA = new Segment(triangle.PointB, triangle.PointA);
            Segment edgeBC = new Segment(triangle.PointB, triangle.PointC);
            Segment edgeCA = new Segment(triangle.PointC, triangle.PointA);

            if (Intersects(ref edgeBA) || Intersects(ref edgeBC) || Intersects(ref edgeCA))
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
                        Vector3 center = other.Center;
                        Vector3 extents = other.Extents;

                        return GeometricToolsHelper.FrustumContainsAABB(this, ref center, ref extents);
                    }

                case BoundingType.Sphere:
                case BoundingType.Capsule:
                case BoundingType.OrientedBoundingBox:
                    {
                        bool intersects = false;
                        for (int i = 0; i < 6; i++)
                        {
                            Plane p = _planes[i];
                            switch (volume.Intersects(ref p))
                            {
                                case PlaneIntersectionType.Back:
                                    return ContainmentType.Outside;
                                case PlaneIntersectionType.Intersects:
                                    intersects = true;
                                    break;
                            }
                        }

                        if (intersects)
                        {
                            return ContainmentType.Intersects;
                        }

                        return ContainmentType.Inside;
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
            if (Contains(ref ray.Origin) == ContainmentType.Inside)
            {
                return true;
            }

            for (int i = 0; i < 6; i++)
            {
                Plane p = _planes[i];
                if (p.Intersects(ref ray, out LineIntersectionResult temp))
                {
                    Vector3 pt = temp.Point;
                    if (Contains(ref pt) != ContainmentType.Outside)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public override bool Intersects(ref Ray ray, out BoundingIntersectionResult result)
        {
            LineIntersectionResult? first = null;
            LineIntersectionResult? second = null;

            for (int i = 0; i < 6; i++)
            {
                Plane p = _planes[i];
                if (p.Intersects(ref ray, out LineIntersectionResult temp))
                {
                    Vector3 pt = temp.Point;
                    if (first == null)
                    {
                        if (Contains(ref pt) != ContainmentType.Outside)
                        {
                            first = temp;
                        }

                        continue;
                    }

                    if (second == null && Contains(ref pt) != ContainmentType.Outside)
                    {
                        second = temp;
                        break;
                    }
                }
            }

            BoundingIntersectionResult.FromResults(ref first, ref second, out result);

            return result.IntersectionCount > 0;
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line)
        {
            for (int i = 0; i < 6; i++)
            {
                Plane p = _planes[i];
                if (p.Intersects(ref line, out LineIntersectionResult temp))
                {
                    Vector3 pt = temp.Point;
                    if (Contains(ref pt) != ContainmentType.Outside)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public override bool Intersects(ref Segment line, out BoundingIntersectionResult result)
        {
            LineIntersectionResult? first = null;
            LineIntersectionResult? second = null;

            for (int i = 0; i < 6; i++)
            {
                Plane p = _planes[i];
                if (p.Intersects(ref line, out LineIntersectionResult temp))
                {
                    Vector3 pt = temp.Point;
                    if (first == null)
                    {
                        if (Contains(ref pt) != ContainmentType.Outside)
                        {
                            first = temp;
                        }

                        continue;
                    }

                    if (second == null && Contains(ref pt) != ContainmentType.Outside)
                    {
                        second = temp;
                        break;
                    }
                }
            }

            BoundingIntersectionResult.FromResults(ref first, ref second, out result);

            return result.IntersectionCount > 0;
        }

        /// <summary>
        /// Tests if the specified plane intersects with the bounding volume.
        /// </summary>
        /// <param name="plane">Plane to test against.</param>
        /// <returns>Type of plane intersection.</returns>
        public override PlaneIntersectionType Intersects(ref Plane plane)
        {
            IReadOnlyDataBuffer<Vector3> corners = Corners;
            PlaneIntersectionType interType = plane.WhichSide(corners[0]);

            for (int i = 1; i < 6; i++)
            {
                Vector3 pt = corners[i];

                if (plane.WhichSide(ref pt) != interType)
                {
                    return PlaneIntersectionType.Intersects;
                }
            }

            return interType;
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
                        Vector3 center = other.Center;
                        Vector3 extents = other.Extents;

                        ContainmentType contains = GeometricToolsHelper.FrustumContainsAABB(this, ref center, ref extents);
                        return contains != ContainmentType.Outside;
                    }

                case BoundingType.Sphere:
                case BoundingType.Capsule:
                case BoundingType.OrientedBoundingBox:
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Plane p = _planes[i];
                            if (volume.Intersects(ref p) == PlaneIntersectionType.Back)
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                case BoundingType.Frustum:
                case BoundingType.Mesh:
                default:
                    {
                        return IntersectsGeneral(volume);
                    }
            }
        }

        /// <summary>
        /// NOT SUPPORTED.
        /// </summary>
        /// <param name="volume">Bounding volume to merge with.</param>
        public override void Merge(BoundingVolume volume)
        {
            Debug.Assert(false, "Cannot merge a BoundingFrustum with other bounding volumes!");
        }

        /// <summary>
        /// Transforms the bounding volume by a Scale-Rotation-Translation (SRT) transform.
        /// </summary>
        /// <param name="scale">Scaling</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="translation">Translation</param>
        public override void Transform(ref Vector3 scale, ref Quaternion rotation, ref Vector3 translation)
        {
            // Is this kosher?
            Matrix4x4.CreateTransformationMatrix(scale.X, scale.Y, scale.Z, ref rotation, ref translation, out Matrix4x4 transform);
            Matrix4x4.Multiply(ref transform, ref _viewProjMatrix, out _viewProjMatrix);
            Set(ref _viewProjMatrix);
        }

        /// <summary>
        /// Tests equality between the bounding volume and the other bounding volume.
        /// </summary>
        /// <param name="other">Other bounding volume</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the volume is of same type and same size/shape, false otherwise.</returns>
        public override bool Equals(BoundingVolume other, float tolerance)
        {
            BoundingFrustum bf = other as BoundingFrustum;

            if (bf != null)
            {
                return _viewProjMatrix.Equals(ref bf._viewProjMatrix, tolerance); // If ViewProjMatrices equal, then rest are
            }

            return false;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            _center = input.Read<Vector3>(); // When written, should have been calculated
            input.Read(out _viewProjMatrix);
            input.Read(out _planes[0]);
            input.Read(out _planes[1]);
            input.Read(out _planes[2]);
            input.Read(out _planes[3]);
            input.Read(out _planes[4]);
            input.Read(out _planes[5]);

            UpdateCorners = true;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            output.Write("Center", Center); // Force a calculation
            output.Write("ViewProjectionMatrix", ref _viewProjMatrix);
            output.Write("Left", ref _planes[0]);
            output.Write("Right", ref _planes[1]);
            output.Write("Top", ref _planes[2]);
            output.Write("Bottom", ref _planes[3]);
            output.Write("Near", ref _planes[4]);
            output.Write("Far", ref _planes[5]);
        }

        /// <summary>
        /// Computes the corners that represent the extremal points of this bounding volume.
        /// </summary>
        /// <param name="corners">Databuffer to contain the points, length equal to the corner count.</param>
        protected override void ComputeCorners(IDataBuffer<Vector3> corners)
        {
            // Four vertices on near plane, clockwise from lower left (along near normal)
            GetPlaneIntersectionPoint(ref _planes[4], ref _planes[3], ref _planes[0], out Vector3 temp); // Near, Bottom, Left
            corners.Set(ref temp);

            GetPlaneIntersectionPoint(ref _planes[4], ref _planes[2], ref _planes[0], out temp); // Near, Top, Left
            corners.Set(ref temp);

            GetPlaneIntersectionPoint(ref _planes[4], ref _planes[2], ref _planes[1], out temp); // Near, Top, Right
            corners.Set(ref temp);

            GetPlaneIntersectionPoint(ref _planes[4], ref _planes[3], ref _planes[1], out temp); // Near, Bottom, Right
            corners.Set(ref temp);

            // Four vertices on far plane, counterclockwise from lower right (along near normal)
            GetPlaneIntersectionPoint(ref _planes[5], ref _planes[3], ref _planes[1], out temp); // Far, Bottom, Right
            corners.Set(ref temp);

            GetPlaneIntersectionPoint(ref _planes[5], ref _planes[2], ref _planes[1], out temp); // Far, Top, Right
            corners.Set(ref temp);

            GetPlaneIntersectionPoint(ref _planes[5], ref _planes[2], ref _planes[0], out temp); // Far, Top, Left
            corners.Set(ref temp);

            GetPlaneIntersectionPoint(ref _planes[5], ref _planes[3], ref _planes[0], out temp); // Far, Bottom, Left
            corners.Set(ref temp);
        }

        private void ComputeCenter()
        {
            IReadOnlyDataBuffer<Vector3> corners = Corners;

            Vector3 center = Vector3.Zero;
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 pt = corners[i];
                Vector3.Add(ref center, ref pt, out center);
            }

            Vector3.Multiply(ref center, 1.0f / corners.Length, out center);

            _center = center;
        }

        private static void ExtractPlanes(ref Matrix4x4 viewProjMatrix, Plane[] planes)
        {
            Plane left = new Plane(viewProjMatrix.M14 + viewProjMatrix.M11,
                              viewProjMatrix.M24 + viewProjMatrix.M21,
                              viewProjMatrix.M34 + viewProjMatrix.M31,
                              viewProjMatrix.M44 + viewProjMatrix.M41);
            left.Normalize();
            planes[0] = left;

            Plane right = new Plane(viewProjMatrix.M14 - viewProjMatrix.M11,
                               viewProjMatrix.M24 - viewProjMatrix.M21,
                               viewProjMatrix.M34 - viewProjMatrix.M31,
                               viewProjMatrix.M44 - viewProjMatrix.M41);
            right.Normalize();
            planes[1] = right;

            Plane top = new Plane(viewProjMatrix.M14 - viewProjMatrix.M12,
                             viewProjMatrix.M24 - viewProjMatrix.M22,
                             viewProjMatrix.M34 - viewProjMatrix.M32,
                             viewProjMatrix.M44 - viewProjMatrix.M42);
            top.Normalize();
            planes[2] = top;

            Plane bottom = new Plane(viewProjMatrix.M14 + viewProjMatrix.M12,
                            viewProjMatrix.M24 + viewProjMatrix.M22,
                            viewProjMatrix.M34 + viewProjMatrix.M32,
                            viewProjMatrix.M44 + viewProjMatrix.M42);
            bottom.Normalize();
            planes[3] = bottom;

            Plane near = new Plane(viewProjMatrix.M13, viewProjMatrix.M23, viewProjMatrix.M33, viewProjMatrix.M43);
            near.Normalize();
            planes[4] = near;

            Plane far = new Plane(viewProjMatrix.M14 - viewProjMatrix.M13,
                             viewProjMatrix.M24 - viewProjMatrix.M23,
                             viewProjMatrix.M34 - viewProjMatrix.M33,
                             viewProjMatrix.M44 - viewProjMatrix.M43);
            far.Normalize();
            planes[5] = far;
        }

        private static void GetPlaneIntersectionPoint(ref Plane p1, ref Plane p2, ref Plane p3, out Vector3 result)
        {
            Vector3.Cross(ref p2.Normal, ref p3.Normal, out Vector3 p2Xp3);
            Vector3.Cross(ref p3.Normal, ref p1.Normal, out Vector3 p3Xp1);
            Vector3.Cross(ref p1.Normal, ref p2.Normal, out Vector3 p1Xp2);
            
            Vector3.Dot(ref p1.Normal, ref p2Xp3, out float p1Dot);
            Vector3.Dot(ref p2.Normal, ref p3Xp1, out float p2Dot);
            Vector3.Dot(ref p3.Normal, ref p1Xp2, out float p3Dot);

            Vector3.Multiply(ref p2Xp3, -p1.D / p1Dot, out p2Xp3);
            Vector3.Multiply(ref p3Xp1, -p2.D / p2Dot, out p3Xp1);
            Vector3.Multiply(ref p1Xp2, -p3.D / p3Dot, out p1Xp2);

            Vector3.Add(ref p2Xp3, ref p3Xp1, out result);
            Vector3.Add(ref result, ref p1Xp2, out result);
        }

        private static float ComputeVolume(float height, IReadOnlyDataBuffer<Vector3> points)
        {
            float l = Vector3.Distance(points[0], points[1]);
            float w = Vector3.Distance(points[0], points[3]);

            float areaNear = l * w;

            l = Vector3.Distance(points[4], points[5]);
            w = Vector3.Distance(points[4], points[7]);

            float areaFar = l * w;

            // Orthographic so just a box
            if (MathHelper.IsApproxEquals(areaNear, areaFar))
            {
                return l * w * height;
            }

            // Perspective so an orthogonal frustum
            return MathHelper.OneThird * height * (areaNear + areaFar + (float)Math.Sqrt(areaNear * areaFar));
        }
    }
}
