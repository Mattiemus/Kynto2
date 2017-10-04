namespace Spark.Math
{
    using System;
    using System.Collections.Generic;

    using Content;
    using Core;
    using Graphics.Geometry;

    /// <summary>
    /// Base class for all bounding volumes, which are simple logical 3D shapes that encapsulates some volume. Bounding volumes
    /// are used to represent a more complex 3D mesh, offering a variety of containment and intersection methods for collision detection/response,
    /// picking, or culling.
    /// </summary>
    public abstract class BoundingVolume : IEquatable<BoundingVolume>, ISavable, IDeepCloneable, IPickable
    {
        private bool _updateCorners;
        private IDataBuffer<Vector3> _corners;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingVolume"/> class.
        /// </summary>
        protected BoundingVolume()
        {
            _updateCorners = true;
        }

        /// <summary>
        /// Gets or sets the center of the bounding volume.
        /// </summary>
        public abstract Vector3 Center { get; set; }

        /// <summary>
        /// Gets the volume of the bounding volume.
        /// </summary>
        public abstract float Volume { get; }

        /// <summary>
        /// Gets the bounding type.
        /// </summary>
        public abstract BoundingType BoundingType { get; }

        /// <summary>
        /// Gets the number of corners.
        /// </summary>
        public abstract int CornerCount { get; }

        /// <summary>
        /// Gets if the volume is minimum, where it has no volume.
        /// </summary>
        public bool IsMinimumBoundingVolume => MathHelper.IsApproxZero(Volume);

        /// <summary>
        /// Gets the buffer corners that represent the extremal points of this bounding volume.
        /// </summary>
        public IReadOnlyDataBuffer<Vector3> Corners
        {
            get
            {
                if (_corners == null)
                {
                    _corners = new DataBuffer<Vector3>(CornerCount);
                    _updateCorners = true;
                }

                if (_updateCorners)
                {
                    _corners.Position = 0;
                    ComputeCorners(_corners);
                    _corners.Position = 0;
                    _updateCorners = false;
                }

                return _corners;
            }
        }

        /// <summary>
        /// Gets or sets whether or not corners of the bounding volume should be computed.
        /// </summary>
        protected bool UpdateCorners
        {
            get => _updateCorners;
            set => _updateCorners = value;
        }

        /// <summary>
        /// For IPickable, always returns this bounding volume.
        /// </summary>
        public BoundingVolume WorldBounding => this;

        #region Copy methods

        /// <summary>
        /// Sets the bounding volume by either copying the specified bounding volume if its the specified type, or computing a volume required to fully contain it.
        /// </summary>
        /// <param name="volume">Bounding volume to copy from</param>
        public abstract void Set(BoundingVolume volume);

        /// <summary>
        /// Creates a copy of the bounding volume and returns a new instance.
        /// </summary>
        /// <returns>Copied bounding volume</returns>
        public abstract BoundingVolume Clone();

        /// <summary>
        /// Get a copy of the object.
        /// </summary>
        /// <returns>Cloned copy.</returns>
        IDeepCloneable IDeepCloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region DistanceTo methods

        /// <summary>
        /// Computes the distance from the center of the bounding volume to the point.
        /// </summary>
        /// <param name="point">Point in the same space as the bounding volume</param>
        /// <returns>Distance to the center of the bounding volume.</returns>
        public float DistanceToCenter(Vector3 point)
        {
            DistanceToCenter(ref point, out float result);
            return result;
        }

        /// <summary>
        /// Computes the distance from the center of the bounding volume to the point.
        /// </summary>
        /// <param name="point">Point in the same space as the bounding volume</param>
        /// <param name="result">Distance to the center of the bounding volume.</param>
        public void DistanceToCenter(ref Vector3 point, out float result)
        {
            Vector3 center = Center;
            Vector3.Distance(ref center, ref point, out result);
        }

        /// <summary>
        /// Computes the distance squared from the center of the bounding volume to the point.
        /// </summary>
        /// <param name="point">Point in the same space as the bounding volume</param>
        /// <returns>Distance squared to the center of the bounding volume.</returns>
        public float DistanceSquaredToCenter(Vector3 point)
        {
            DistancedSquaredToCenter(ref point, out float result);
            return result;
        }

        /// <summary>
        /// Computes the distance squared from the center of the bounding volume to the point.
        /// </summary>
        /// <param name="point">Point in the same space as the bounding volume</param>
        /// <param name="result">Distance squared to the center of the bounding volume.</param>
        public void DistancedSquaredToCenter(ref Vector3 point, out float result)
        {
            Vector3 center = Center;
            Vector3.DistanceSquared(ref center, ref point, out result);
        }

        /// <summary>
        /// Computes the distance from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <returns>Distance from the point to the edge of the volume.</returns>
        public float DistanceTo(Vector3 point)
        {
            DistanceTo(ref point, out float result);
            return result;
        }

        /// <summary>
        /// Computes the distance squared from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <returns>Distance squared from the point to the edge of the volume.</returns>
        public float DistanceSquaredTo(Vector3 point)
        {
            DistanceSquaredTo(ref point, out float result);
            return result;
        }

        /// <summary>
        /// Computes the closest point on the volume from the given point in space.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <returns>Closest point on the edge of the volume.</returns>
        public Vector3 ClosestPointTo(Vector3 point)
        {
            ClosestPointTo(ref point, out Vector3 ptOnVolume);
            return ptOnVolume;
        }

        /// <summary>
        /// Computes the distance from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distance from the point to the edge of the volume.</param>
        public abstract void DistanceTo(ref Vector3 point, out float result);

        /// <summary>
        /// Computes the distance squared from a given point to the edge of the volume.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Distanced squared from the point to the edge of the volume.</param>
        public abstract void DistanceSquaredTo(ref Vector3 point, out float result);

        /// <summary>
        /// Computes the closest point on the volume from the given point in space.
        /// </summary>
        /// <param name="point">Point in space</param>
        /// <param name="result">Closest point on the edge of the volume.</param>
        public abstract void ClosestPointTo(ref Vector3 point, out Vector3 result);

        #endregion

        #region ComputeFromPoints methods

        /// <summary>
        /// Computes a minimum bounding volume that encloses the specified points in space.
        /// </summary>
        /// <param name="points">Points in space</param>
        public void ComputeFromPoints(IReadOnlyDataBuffer<Vector3> points)
        {
            ComputeFromPoints(points, null);
        }

        /// <summary>
        /// Computes a minimum bounding volume that encloses the specified points in space.
        /// </summary>
        /// <param name="points">Points in space</param>
        /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for. Note: the base vertex offset will be ignored.</param>
        public abstract void ComputeFromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange);

        /// <summary>
        /// Computes a minimum bounding volume that encloses the specified indexed points in space.
        /// </summary>
        /// <param name="points">Points in space.</param>
        /// <param name="indices">Point indices denoting location in the point buffer.</param>
        public void ComputeFromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices)
        {
            ComputeFromIndexedPoints(points, indices, null);
        }

        /// <summary>
        /// Computes a minimum bounding volume that encloses the specified indexed points in space.
        /// </summary>
        /// <param name="points">Points in space.</param>
        /// <param name="indices">Point indices denoting location in the point buffer.</param>
        /// <param name="subMeshRange">Optional range inside the buffer that represents the mesh to compute bounds for.</param>
        public abstract void ComputeFromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange);

        #endregion

        #region Containment methods

        /// <summary>
        /// Determines if the specified set of points are contained inside the bounding volume.
        /// </summary>
        /// <param name="points">Set of points to test against.</param>
        /// <returns>Type of containment</returns>
        public ContainmentType Contains(IReadOnlyDataBuffer<Vector3> points)
        {
            if (points == null || points.Length == 0)
            {
                return ContainmentType.Outside;
            }

            bool outside = false;
            bool inside = false;
            bool intersects = false;
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 point = points[i];
                switch (Contains(ref point))
                {
                    case ContainmentType.Outside:
                        outside = true;
                        break;
                    case ContainmentType.Inside:
                        inside = true;
                        break;
                    case ContainmentType.Intersects:
                        intersects = true;
                        break;
                }
            }

            if ((outside && inside) || intersects)
            {
                return ContainmentType.Intersects;
            }
            else if (inside && !outside)
            {
                return ContainmentType.Inside;
            }
            else
            {
                return ContainmentType.Outside;
            }
        }

        /// <summary>
        /// Determines if the specified set of points are contained inside the bounding volume.
        /// </summary>
        /// <param name="points">Set of points to test against.</param>
        /// <returns>Type of containment</returns>
        public ContainmentType Contains(Vector3[] points)
        {
            if (points == null || points.Length == 0)
            {
                return ContainmentType.Outside;
            }

            bool outside = false;
            bool inside = false;
            bool intersects = false;
            for (int i = 0; i < points.Length; i++)
            {
                switch (Contains(ref points[i]))
                {
                    case ContainmentType.Outside:
                        outside = true;
                        break;
                    case ContainmentType.Inside:
                        inside = true;
                        break;
                    case ContainmentType.Intersects:
                        intersects = true;
                        break;
                }
            }

            if ((outside && inside) || intersects)
            {
                return ContainmentType.Intersects;
            }
            else if (inside && !outside)
            {
                return ContainmentType.Inside;
            }
            else
            {
                return ContainmentType.Outside;
            }
        }

        /// <summary>
        /// Determines if the specified point is contained inside the bounding volume.
        /// </summary>
        /// <param name="point">Point to test against.</param>
        /// <returns>Type of containment</returns>
        public ContainmentType Contains(Vector3 point)
        {
            return Contains(ref point);
        }

        /// <summary>
        /// Determines if the specified segment line is contained inside the bounding volume.
        /// </summary>
        /// <param name="line">Segment to test against.</param>
        /// <returns>Type of containment.</returns>
        public ContainmentType Contains(Segment line)
        {
            return Contains(ref line);
        }

        /// <summary>
        /// Determines if the specified triangle is contained inside the bounding volume.
        /// </summary>
        /// <param name="triangle">Triangle to test against.</param>
        /// <returns>Type of containment.</returns>
        public ContainmentType Contains(Triangle triangle)
        {
            return Contains(ref triangle);
        }

        /// <summary>
        /// Determines if the specified ellipse intersects with the bounding volume.
        /// </summary>
        /// <param name="ellipse">Ellipse to test against.</param>
        /// <returns>True if the bounding volume intersects with the ellipse, false otherwise.</returns>
        public ContainmentType Contains(Ellipse ellipse)
        {
            return Contains(ref ellipse);
        }

        /// <summary>
        /// Determines if the specified point is contained inside the bounding volume.
        /// </summary>
        /// <param name="point">Point to test against.</param>
        /// <returns>Type of containment</returns>
        public abstract ContainmentType Contains(ref Vector3 point);

        /// <summary>
        /// Determines if the specified segment line is contained inside the bounding volume.
        /// </summary>
        /// <param name="line">Segment to test against.</param>
        /// <returns>Type of containment.</returns>
        public abstract ContainmentType Contains(ref Segment line);

        /// <summary>
        /// Determines if the specified triangle is contained inside the bounding volume.
        /// </summary>
        /// <param name="triangle">Triangle to test against.</param>
        /// <returns>Type of containment.</returns>
        public abstract ContainmentType Contains(ref Triangle triangle);

        /// <summary>
        /// Determines if the specified ellipse is contained inside the bounding volume.
        /// </summary>
        /// <param name="ellipse">Ellipse to test against.</param>
        /// <returns>Type of containment.</returns>
        public abstract ContainmentType Contains(ref Ellipse ellipse);

        /// <summary>
        /// Determines if the specified bounding volume is contained inside the bounding volume.
        /// </summary>
        /// <param name="volume">Bounding volume to test against.</param>
        /// <returns>Type of containment.</returns>
        public abstract ContainmentType Contains(BoundingVolume volume);

        #endregion

        #region Intersection methods

        /// <summary>
        /// Tests if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public bool Intersects(Ray ray)
        {
            return Intersects(ref ray);
        }

        /// <summary>
        /// Determines if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public bool Intersects(Ray ray, out BoundingIntersectionResult result)
        {
            return Intersects(ref ray, out result);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public bool Intersects(Segment line)
        {
            return Intersects(ref line);
        }

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public bool Intersects(Segment line, out BoundingIntersectionResult result)
        {
            return Intersects(ref line, out result);
        }

        /// <summary>
        /// Determines if the specified triangle intersects with the bounding volume.
        /// </summary>
        /// <param name="triangle">Triangle to test against.</param>
        /// <returns>True if the bounding volume intersects with the triangle, false otherwise.</returns>
        public bool Intersects(Triangle triangle)
        {
            return Contains(ref triangle) == ContainmentType.Intersects;
        }

        /// <summary>
        /// Determines if the specified triangle intersects with the bounding volume.
        /// </summary>
        /// <param name="triangle">Triangle to test against.</param>
        /// <returns>True if the bounding volume intersects with the triangle, false otherwise.</returns>
        public bool Intersects(ref Triangle triangle)
        {
            return Contains(ref triangle) == ContainmentType.Intersects;
        }

        /// <summary>
        /// Determines if the specified ellipse intersects with the bounding volume.
        /// </summary>
        /// <param name="ellipse">Ellipse to test against.</param>
        /// <returns>True if the bounding volume intersects with the ellipse, false otherwise.</returns>
        public bool Intersects(Ellipse ellipse)
        {
            return Contains(ref ellipse) == ContainmentType.Intersects;
        }

        /// <summary>
        /// Determines if the specified ellipse intersects with the bounding volume.
        /// </summary>
        /// <param name="ellipse">Ellipse to test against.</param>
        /// <returns>True if the bounding volume intersects with the ellipse, false otherwise.</returns>
        public bool Intersects(ref Ellipse ellipse)
        {
            return Contains(ref ellipse) == ContainmentType.Intersects;
        }

        /// <summary>
        /// Tests if the specified plane intersects with the bounding volume.
        /// </summary>
        /// <param name="plane">Plane to test against.</param>
        /// <returns>Type of plane intersection.</returns>
        public PlaneIntersectionType Intersects(Plane plane)
        {
            return Intersects(ref plane);
        }

        /// <summary>
        /// Determines if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public abstract bool Intersects(ref Ray ray);

        /// <summary>
        /// Determines if the specified ray intersects with the bounding volume.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the ray, false otherwise.</returns>
        public abstract bool Intersects(ref Ray ray, out BoundingIntersectionResult result);

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public abstract bool Intersects(ref Segment line);

        /// <summary>
        /// Determines if the specified segment line intersects with the bounding volume.
        /// </summary>
        /// <param name="line">Segment line to test against.</param>
        /// <param name="result">Bounding intersection result.</param>
        /// <returns>True if the bounding volume intersects with the segment line, false otherweise.</returns>
        public abstract bool Intersects(ref Segment line, out BoundingIntersectionResult result);

        /// <summary>
        /// Determines if the specified plane intersects with the bounding volume.
        /// </summary>
        /// <param name="plane">Plane to test against.</param>
        /// <returns>Type of plane intersection.</returns>
        public abstract PlaneIntersectionType Intersects(ref Plane plane);

        /// <summary>
        /// Determines if the specified bounding volume intersects with the bounding volume.
        /// </summary>
        /// <param name="volume">Other bounding volume to test against.</param>
        /// <returns>True if the two volumes intersect with one another, false otherwise.</returns>
        public abstract bool Intersects(BoundingVolume volume);

        #endregion

        #region Merge methods

        /// <summary>
        /// Creates a new instance of the bounding volume and merges it with the specified bounding volume, resulting in a volume
        /// that encloses both.
        /// </summary>
        /// <param name="volume">Bounding volume to merge with.</param>
        /// <returns>New bounding volume instance that encloses both volumes.</returns>
        public BoundingVolume MergeCopy(BoundingVolume volume)
        {
            BoundingVolume bv = Clone();
            bv.Merge(volume);

            return bv;
        }

        /// <summary>
        /// Merges the bounding volume with the specified bounding volume, resulting in a volume that encloses both.
        /// </summary>
        /// <param name="volume">Bounding volume to merge with.</param>
        public abstract void Merge(BoundingVolume volume);

        #endregion

        #region Transform methods

        /// <summary>
        /// Creates a new instance of the bounding volume and transforms it by the Scale-Rotation-Translation (SRT) transform.
        /// </summary>
        /// <param name="transform">SRT transform.</param>
        /// <returns>New transformed bounding volume instance.</returns>
        public BoundingVolume TransformCopy(Transform transform)
        {
            BoundingVolume bv = Clone();

            if (transform != null)
            {
                Vector3 scale = transform.Scale;
                Quaternion rot = transform.Rotation;
                Vector3 trans = transform.Translation;

                bv.Transform(ref scale, ref rot, ref trans);
            }

            return bv;
        }

        /// <summary>
        /// Creates a new instance of the bounding volume and transforms it by the Scale-Rotation-Translation (SRT) transform.
        /// </summary>
        /// <param name="scale">Scaling</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="translation">Translation</param>
        /// <returns>New transformed bounding volume instance.</returns>
        public BoundingVolume TransformCopy(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            BoundingVolume bv = Clone();
            bv.Transform(ref scale, ref rotation, ref translation);

            return bv;
        }

        /// <summary>
        /// Creates a new instance of the bounding volume and transforms it by the Scale-Rotation-Translation (SRT) transform.
        /// </summary>
        /// <param name="scale">Scaling</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="translation">Translation</param>
        /// <returns>New transformed bounding volume instance.</returns>
        public BoundingVolume TransformCopy(ref Vector3 scale, ref Quaternion rotation, ref Vector3 translation)
        {
            BoundingVolume bv = Clone();
            bv.Transform(ref scale, ref rotation, ref translation);

            return bv;
        }

        /// <summary>
        /// Transforms the bounding volume by a Scale-Rotation-Translation (SRT) transform.
        /// </summary>
        /// <param name="transform">SRT transform.</param>
        public void Transform(Transform transform)
        {
            if (transform != null)
            {
                Vector3 scale = transform.Scale;
                Quaternion rot = transform.Rotation;
                Vector3 trans = transform.Translation;

                Transform(ref scale, ref rot, ref trans);
            }
        }

        /// <summary>
        /// Transforms the bounding volume by a Scale-Rotation-Translation (SRT) transform.
        /// </summary>
        /// <param name="scale">Scaling</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="translation">Translation</param>
        public void Transform(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            Transform(ref scale, ref rotation, ref translation);
        }

        /// <summary>
        /// Transforms the bounding volume by a Scale-Rotation-Translation (SRT) transform.
        /// </summary>
        /// <param name="scale">Scaling</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="translation">Translation</param>
        public abstract void Transform(ref Vector3 scale, ref Quaternion rotation, ref Vector3 translation);

        #endregion

        #region IEquatable methods

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is BoundingVolume)
            {
                return Equals(obj as BoundingVolume, MathHelper.ZeroTolerance);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the bounding volume and the other bounding volume.
        /// </summary>
        /// <param name="other">Other bounding volume</param>
        /// <returns>True if the volume is of same type and same size/shape, false otherwise.</returns>
        public bool Equals(BoundingVolume other)
        {
            return Equals(other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the bounding volume and the other bounding volume.
        /// </summary>
        /// <param name="other">Other bounding volume</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if the volume is of same type and same size/shape, false otherwise.</returns>
        public abstract bool Equals(BoundingVolume other, float tolerance);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return BoundingType.GetHashCode() + Volume.GetHashCode() + Center.GetHashCode();
            }
        }

        #endregion

        #region ISavable methods

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public abstract void Read(ISavableReader input);

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public abstract void Write(ISavableWriter output);

        #endregion

        #region Protected Methods

        /// <summary>
        /// Checks if the mesh range and buffer are valid.
        /// </summary>
        /// <param name="points">Buffer of vertices</param>
        /// <param name="subMeshRange">Optional range in the buffer.</param>
        /// <returns>True if the range and buffer are valid, false if otherwise.</returns>
        protected static bool IsValidRange(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
        {
            if (points == null)
            {
                return false;
            }

            if (subMeshRange.HasValue)
            {
                SubMeshRange range = subMeshRange.Value;
                return range.Count > 0 && range.Offset >= 0 && ((range.Offset + range.Count) <= points.Length);
            }
            else
            {
                return points.Length > 0;
            }
        }

        /// <summary>
        /// Checks if the mesh range and buffers are valid.
        /// </summary>
        /// <param name="points">Buffer of vertices</param>
        /// <param name="indices">Buffer of indices</param>
        /// <param name="subMeshRange">Optional range in the index buffer.</param>
        /// <returns>True if the range and buffers are valid, false if otherwise.</returns>
        protected static bool IsValidRange(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange)
        {
            if (points == null || !indices.IsValid)
            {
                return false;
            }

            if (subMeshRange.HasValue)
            {
                SubMeshRange range = subMeshRange.Value;
                return range.Count > 0 && range.Offset >= 0 && range.BaseVertexOffset >= 0 && range.BaseVertexOffset < points.Length && ((range.Offset + range.Count) <= indices.Length);
            }
            else
            {
                return points.Length > 0 && indices.Length > 0;
            }
        }

        /// <summary>
        /// Extracts the valid range in the buffer at which to compute a bounding volume from.
        /// </summary>
        /// <param name="points">Buffer of vertices</param>
        /// <param name="subMeshRange">Optional range in the buffer.</param>
        /// <param name="offset">Offset from the start of the buffer.</param>
        /// <param name="count">Number of positions to read.</param>
        /// <returns>True if the range is valid, false if otherwise.</returns>
        protected static bool ExtractSubMeshRange(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange, out int offset, out int count)
        {
            offset = 0;
            count = 0;

            if (!IsValidRange(points, subMeshRange))
            {
                return false;
            }

            if (subMeshRange.HasValue)
            {
                SubMeshRange range = subMeshRange.Value;
                offset = range.Offset;
                count = range.Count;
            }
            else
            {
                count = points.Length;
            }

            return true;
        }

        /// <summary>
        /// Extracts the valid range in the buffer at which to compute a bounding volume from.
        /// </summary>
        /// <param name="points">Buffer of vertices</param>
        /// <param name="indices">Buffer of indices</param>
        /// <param name="subMeshRange">Optional range in the buffer.</param>
        /// <param name="offset">Offset from the start of the buffer.</param>
        /// <param name="count">Number of positions to read.</param>
        /// <param name="baseVertexOffset">Offset to add to the index, if applicable.</param>
        /// <returns>True if the range is valid, false if otherwise.</returns>
        protected static bool ExtractSubMeshRange(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange, out int offset, out int count, out int baseVertexOffset)
        {
            offset = 0;
            count = 0;
            baseVertexOffset = 0;

            if (!IsValidRange(points, indices, subMeshRange))
            {
                return false;
            }
                
            if (subMeshRange.HasValue)
            {
                SubMeshRange range = subMeshRange.Value;
                offset = range.Offset;
                baseVertexOffset = range.BaseVertexOffset;
                count = range.Count;
            }
            else
            {
                count = indices.Length;
            }

            return true;
        }

        /// <summary>
        /// Determines if the specified unknown bounding volume is contained within the bounding volume. This is intended only as a 
        /// finaly, fool-proof measure as it tests corner containment.
        /// </summary>
        /// <param name="volume">Unknown other bounding volume to test against.</param>
        /// <returns>Type of containment.</returns>
        protected ContainmentType ContainsGeneral(BoundingVolume volume)
        {
            if (volume == null)
            {
                return ContainmentType.Outside;
            }

            return Contains(volume.Corners);
        }

        /// <summary>
        /// Determines if the specified unknown bounding volume intersects with the bounding volume. This
        /// is intended only as a final, fool-proof measure as it tests corner containment.
        /// </summary>
        /// <param name="volume">Unknown other bounding volume to test against.</param>
        /// <returns>True if the two volumes intersect with one another, false otherwise.</returns>
        protected bool IntersectsGeneral(BoundingVolume volume)
        {
            if (volume == null)
            {
                return false;
            }

            IReadOnlyDataBuffer<Vector3> pts = volume.Corners;
            for (int i = 0; i < pts.Length; i++)
            {
                Vector3 pt = pts[i];
                if (Contains(ref pt) != ContainmentType.Outside)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Computes the corners that represent the extremal points of this bounding volume.
        /// </summary>
        /// <param name="corners">Databuffer to contain the points, length equal to the corner count.</param>
        protected abstract void ComputeCorners(IDataBuffer<Vector3> corners);

        /// <summary>
        /// Performs a ray-mesh intersection test.
        /// </summary>
        /// <param name="ray">Ray to test against.</param>
        /// <param name="results">List of results to add to.</param>
        /// <param name="ignoreBackfaces">True if backfaces (relative to the pick ray) should be ignored, false if they should be considered a result.</param>
        /// <returns>True if an intersection occured and results were added to the output list, false if no intersection occured.</returns>
        public bool IntersectsMesh(ref Ray ray, IList<Tuple<LineIntersectionResult, Triangle?>> results, bool ignoreBackfaces)
        {
            return false;
        }

        #endregion
    }
}
