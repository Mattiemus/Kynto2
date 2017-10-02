namespace Spark.Math
{
    using System;

    using Core;
    using Content;

    public sealed class BoundingMesh : BoundingVolume
    {
        public override Vector3 Center
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override float Volume
        {
            get { throw new NotImplementedException(); }
        }

        public override int CornerCount
        {
            get { throw new NotImplementedException(); }
        }

        public override BoundingType BoundingType
        {
            get { throw new NotImplementedException(); }
        }

        public override void Set(BoundingVolume volume)
        {
            throw new NotImplementedException();
        }

        public override BoundingVolume Clone()
        {
            throw new NotImplementedException();
        }

        public override void DistanceTo(ref Vector3 point, out float result)
        {
            throw new NotImplementedException();
        }

        public override void DistanceSquaredTo(ref Vector3 point, out float result)
        {
            throw new NotImplementedException();
        }

        public override void ClosestPointTo(ref Vector3 point, out Vector3 result)
        {
            throw new NotImplementedException();
        }

        public override void ComputeFromPoints(IReadOnlyDataBuffer<Vector3> points, SubMeshRange? subMeshRange)
        {
            throw new NotImplementedException();
        }

        public override void ComputeFromIndexedPoints(IReadOnlyDataBuffer<Vector3> points, IndexData indices, SubMeshRange? subMeshRange)
        {
            throw new NotImplementedException();
        }

        public override ContainmentType Contains(ref Vector3 point)
        {
            throw new NotImplementedException();
        }

        public override ContainmentType Contains(ref Segment line)
        {
            throw new NotImplementedException();
        }

        public override ContainmentType Contains(ref Triangle triangle)
        {
            throw new NotImplementedException();
        }

        public override ContainmentType Contains(ref Ellipse ellipse)
        {
            throw new NotImplementedException();
        }

        public override ContainmentType Contains(BoundingVolume volume)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(ref Ray ray)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(ref Ray ray, out BoundingIntersectionResult result)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(ref Segment line)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(ref Segment line, out BoundingIntersectionResult result)
        {
            throw new NotImplementedException();
        }

        public override PlaneIntersectionType Intersects(ref Plane plane)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(BoundingVolume volume)
        {
            throw new NotImplementedException();
        }

        public override void Merge(BoundingVolume volume)
        {
            throw new NotImplementedException();
        }

        public override void Transform(ref Vector3 scale, ref Quaternion rotation, ref Vector3 translation)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(BoundingVolume other, float tolerance)
        {
            throw new NotImplementedException();
        }

        public override void Read(ISavableReader input)
        {
            throw new NotImplementedException();
        }

        public override void Write(ISavableWriter output)
        {
            throw new NotImplementedException();
        }

        protected override void ComputeCorners(IDataBuffer<Vector3> corners)
        {
            throw new NotImplementedException();
        }
    }
}
