namespace Spark.Math
{
    using System;
    using System.Runtime.CompilerServices;

    using Core;

    /// <summary>
    /// Helper methods for intersection and distance queries. Algorithms adapted from Schneider/Eberly's "Geometric Tools for Computer Graphics" cookbook unless otherwise
    /// noted.
    /// </summary>
    internal sealed class GeometricToolsHelper
    {
        #region Line queries

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayRayXY(ref Ray ray0, ref Ray ray1)
        {
            float s0, s1;
            IntersectionType interType = ClassifyXYIntersection(ref ray0.Origin, ref ray0.Direction, ref ray1.Origin, ref ray1.Direction, out s0, out s1);

            if (interType == IntersectionType.Point)
            {
                //Test whether the line-line intersection is on the rays
                if (s0 >= 0.0f && s1 >= 0.0f)
                    return true;
            }
            else if (interType == IntersectionType.Line)
            {
                //Lines collinear
                float dot;
                Vector3.Dot(ref ray0.Direction, ref ray1.Direction, out dot);

                //Same direction, so must overlap
                if (dot > 0.0f)
                {
                    return true;
                }
                else
                {
                    //Opposite direction, Ray0 has interval [0, +infinity), ray1 has
                    //interval (-infinity, t1] relative to ray0.Direction
                    float t1;
                    Vector3 diff;
                    Vector3.Subtract(ref ray1.Origin, ref ray0.Origin, out diff);
                    Vector3.Dot(ref ray0.Direction, ref diff, out t1);

                    if (t1 < 0.0f)
                        return false; //No overlap

                    return true;
                }
            }

            return false;
        }

        //Ray parameters from origin respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayRayXY(ref Ray ray0, ref Ray ray1, out Vector3 intersectionPoint, out float ray0Parameter, out float ray1Parameter)
        {
            intersectionPoint = Vector3.Zero;
            IntersectionType interType = ClassifyXYIntersection(ref ray0.Origin, ref ray0.Direction, ref ray1.Origin, ref ray1.Direction, out ray0Parameter, out ray1Parameter);

            if (interType == IntersectionType.Point)
            {
                //Test whether the line-line intersection is on the rays
                if (ray0Parameter >= 0.0f && ray1Parameter >= 0.0f)
                {
                    Vector3.Multiply(ref ray0.Direction, ray0Parameter, out intersectionPoint);
                    Vector3.Add(ref intersectionPoint, ref ray0.Origin, out intersectionPoint);
                }
            }
            else if (interType == IntersectionType.Line)
            {
                //Lines collinear
                float dot;
                Vector3.Dot(ref ray0.Direction, ref ray1.Direction, out dot);

                //Ray0 has interval [0, +infinity), ray1 has
                //interval (-infinity, t1] relative to ray0.Direction
                float t1;
                Vector3 diff;
                Vector3.Subtract(ref ray1.Origin, ref ray0.Origin, out diff);
                Vector3.Dot(ref ray0.Direction, ref diff, out t1);

                //Same direction, so must overlap
                if (dot > 0.0f)
                {
                    //Choose which origin is on the interval of the overlap
                    intersectionPoint = (t1 > 0.0f) ? ray1.Origin : ray0.Origin;
                    return true;
                }
                else
                {
                    if (t1 < 0.0f)
                        return false; //No overlap

                    //Either origins intersect or intersection is a segment from each origin,
                    //just choose the first ray origin
                    intersectionPoint = ray0.Origin;
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRaySegmentXY(ref Ray ray, ref Segment segment)
        {
            float s0, s1, segExtent;
            Vector3 segCenter, segDir;
            CalculateSegmentProperties(ref segment, out segCenter, out segDir, out segExtent);

            IntersectionType interType = ClassifyXYIntersection(ref ray.Origin, ref ray.Direction, ref segCenter, ref segDir, out s0, out s1);

            if (interType == IntersectionType.Point)
            {
                //Test whether the line-line intersection is on the ray and segment
                if (s0 >= 0.0f && Math.Abs(s1) <= segExtent)
                    return true;
            }
            else if (interType == IntersectionType.Line)
            {
                //Compute location of the segment center relative to the ray;
                Vector3 diff;
                Vector3.Subtract(ref segCenter, ref ray.Origin, out diff);

                float t1;
                Vector3.Dot(ref ray.Direction, ref diff, out t1);

                //Compute location of right most point of the segment relative to ray direction
                float tMax = t1 + segExtent;

                if (tMax < 0.0f)
                    return false; //No overlap

                return true;
            }

            return false;
        }

        //Ray and segment parameters from origin and start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRaySegmentXY(ref Ray ray, ref Segment segment, out Vector3 intersectionPoint, out float rayParameter, out float segParameter)
        {
            intersectionPoint = Vector3.Zero;

            float segExtent;
            Vector3 segCenter, segDir;
            CalculateSegmentProperties(ref segment, out segCenter, out segDir, out segExtent);

            IntersectionType interType = ClassifyXYIntersection(ref ray.Origin, ref ray.Direction, ref segCenter, ref segDir, out rayParameter, out segParameter);

            if (interType == IntersectionType.Point)
            {
                //Test whether the line-line intersection is on the ray and segment
                if (rayParameter >= 0.0f && Math.Abs(segParameter) <= segExtent)
                {
                    Vector3.Multiply(ref ray.Direction, rayParameter, out intersectionPoint);
                    Vector3.Add(ref intersectionPoint, ref ray.Origin, out intersectionPoint);

                    //Fixup segment parameter
                    segParameter += segExtent;
                    return true;
                }
            }
            else if (interType == IntersectionType.Line)
            {
                //Compute location of the segment center relative to the ray;
                Vector3 diff;
                Vector3.Subtract(ref segCenter, ref ray.Origin, out diff);

                float t1;
                Vector3.Dot(ref ray.Direction, ref diff, out t1);

                //Compute location of the segment endpoints relative to the ray direction
                float tMin = t1 - segExtent;
                float tMax = t1 + segExtent;

                //Compute intersection of[0, +infinity) and [tmin, tmax]
                float s0, s1;
                interType = IntersectInterval(0.0f, float.MaxValue, tMin, tMax, out s0, out s1);

                if (interType == IntersectionType.Line)
                {
                    Vector3.Multiply(ref ray.Direction, s0, out intersectionPoint);
                    Vector3.Add(ref intersectionPoint, ref ray.Origin, out intersectionPoint);

                    rayParameter = s0;
                    Vector3.Distance(ref intersectionPoint, ref segment.StartPoint, out segParameter);
                    return true;
                }
                else if (interType == IntersectionType.Point)
                {
                    intersectionPoint = ray.Origin;

                    rayParameter = 0.0f;
                    segParameter = segExtent; //Center overlaps with ray origin
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentSegmentXY(ref Segment seg0, ref Segment seg1)
        {
            float segExtent0, segExtent1;
            Vector3 segCenter0, segDir0, segCenter1, segDir1;
            CalculateSegmentProperties(ref seg0, out segCenter0, out segDir0, out segExtent0);
            CalculateSegmentProperties(ref seg1, out segCenter1, out segDir1, out segExtent1);

            float s0, s1;
            IntersectionType interType = ClassifyXYIntersection(ref segCenter0, ref segDir0, ref segCenter1, ref segDir1, out s0, out s1);

            if (interType == IntersectionType.Point)
            {
                //Test whether the line-line intersection is on the segments
                if (Math.Abs(s0) <= segExtent0 && Math.Abs(s1) <= segExtent1)
                    return true;
            }
            else if (interType == IntersectionType.Line)
            {
                //Compute location of seg1 endpoints relative to seg0
                Vector3 diff;
                Vector3.Subtract(ref segCenter1, ref segCenter0, out diff);

                float t1;
                Vector3.Dot(ref segDir0, ref diff, out t1);

                float tMin = t1 - segExtent1;
                float tMax = t1 + segExtent1;

                interType = IntersectInterval(-segExtent0, segExtent0, tMin, tMax, out s0, out s1);

                //If point or line overlap, then intersects
                return interType != IntersectionType.Empty;
            }

            return false;
        }

        //Segment parameters from start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentSegmentXY(ref Segment seg0, ref Segment seg1, out Vector3 intersectionPoint, out float seg0Parameter, out float seg1Parameter)
        {
            intersectionPoint = Vector3.Zero;

            float segExtent0, segExtent1;
            Vector3 segCenter0, segDir0, segCenter1, segDir1;
            CalculateSegmentProperties(ref seg0, out segCenter0, out segDir0, out segExtent0);
            CalculateSegmentProperties(ref seg1, out segCenter1, out segDir1, out segExtent1);

            IntersectionType interType = ClassifyXYIntersection(ref segCenter0, ref segDir0, ref segCenter1, ref segDir1, out seg0Parameter, out seg1Parameter);

            if (interType == IntersectionType.Point)
            {
                if (Math.Abs(seg0Parameter) <= segExtent0 && Math.Abs(seg1Parameter) <= segExtent1)
                {
                    Vector3.Multiply(ref segDir0, seg0Parameter, out intersectionPoint);
                    Vector3.Add(ref intersectionPoint, ref segCenter0, out intersectionPoint);

                    //Fixup segment parameters
                    seg0Parameter += segExtent0;
                    seg1Parameter += segExtent1;
                    return true;
                }
            }
            else if (interType == IntersectionType.Line)
            {
                //Compute location of seg1 endpoints relative to seg0
                Vector3 diff;
                Vector3.Subtract(ref segCenter1, ref segCenter0, out diff);

                float t1;
                Vector3.Dot(ref segDir0, ref diff, out t1);

                float tMin = t1 - segExtent1;
                float tMax = t1 + segExtent1;

                float s0, s1;
                interType = IntersectInterval(-segExtent0, segExtent0, tMin, tMax, out s0, out s1);

                if (interType != IntersectionType.Empty)
                {
                    Vector3.Multiply(ref segDir0, s0, out intersectionPoint);
                    Vector3.Add(ref intersectionPoint, ref segCenter0, out intersectionPoint);

                    seg0Parameter = s0 + segExtent0;
                    Vector3.Distance(ref intersectionPoint, ref seg1.StartPoint, out seg1Parameter);
                    return true;
                }
            }

            return false;
        }

        //Line parameters from origin respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistancePointLine(ref Vector3 point, ref Vector3 lineOrigin, ref Vector3 lineDir, out Vector3 ptOnLine, out float lineParameter, out float squaredDistance)
        {
            Vector3 diff;
            Vector3.Subtract(ref point, ref lineOrigin, out diff);

            Vector3.Dot(ref lineDir, ref diff, out lineParameter);

            Vector3.Multiply(ref lineDir, lineParameter, out ptOnLine);
            Vector3.Add(ref ptOnLine, ref lineOrigin, out ptOnLine);

            Vector3.Subtract(ref ptOnLine, ref point, out diff);

            squaredDistance = diff.LengthSquared();
        }

        //Ray parameters from origin respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistancePointRay(ref Vector3 point, ref Ray ray, out Vector3 ptOnRay, out float rayParameter, out float squaredDistance)
        {
            Vector3 diff;
            Vector3.Subtract(ref point, ref ray.Origin, out diff);

            Vector3.Dot(ref ray.Direction, ref diff, out rayParameter);

            if (rayParameter > 0.0f)
            {
                Vector3.Multiply(ref ray.Direction, rayParameter, out ptOnRay);
                Vector3.Add(ref ptOnRay, ref ray.Origin, out ptOnRay);
            }
            else
            {
                ptOnRay = ray.Origin;
                rayParameter = 0.0f;
            }

            Vector3.Subtract(ref ptOnRay, ref point, out diff);
            squaredDistance = diff.LengthSquared();
        }

        //Segment parameters from Start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistancePointSegment(ref Vector3 point, ref Segment segment, out Vector3 ptOnSegment, out float segParameter, out float squaredDistance)
        {
            Vector3 segCenter, segDir;
            float segExtent;
            CalculateSegmentProperties(ref segment, out segCenter, out segDir, out segExtent);

            Vector3 diff;
            Vector3.Subtract(ref point, ref segCenter, out diff);

            Vector3.Dot(ref segDir, ref diff, out segParameter);

            if (-segExtent < segParameter)
            {
                if (segParameter < segExtent)
                {
                    Vector3.Multiply(ref segDir, segParameter, out ptOnSegment);
                    Vector3.Add(ref ptOnSegment, ref segCenter, out ptOnSegment);
                }
                else
                {
                    ptOnSegment = segment.EndPoint;
                    segParameter = segExtent;
                }
            }
            else
            {
                ptOnSegment = segment.StartPoint;
                segParameter = -segExtent;
            }

            segParameter += segExtent;

            Vector3.Subtract(ref ptOnSegment, ref point, out diff);
            squaredDistance = diff.LengthSquared();
        }

        //Segment parameters from Start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceSegmentSegment(ref Segment segA, ref Segment segB, out float segAParameter, out float segBParameter, out float squaredDistance)
        {
            Vector3 segACenter, segADir, segBCenter, segBDir;
            float segAExtent, segBExtent;
            CalculateSegmentProperties(ref segA, out segACenter, out segADir, out segAExtent);
            CalculateSegmentProperties(ref segB, out segBCenter, out segBDir, out segBExtent);

            Vector3 diff;
            Vector3.Subtract(ref segACenter, ref segBCenter, out diff);

            float a01;
            Vector3.Dot(ref segADir, ref segBDir, out a01);
            a01 = -a01;

            float b0;
            Vector3.Dot(ref diff, ref segADir, out b0);

            float b1;
            Vector3.Dot(ref diff, ref segBDir, out b1);
            b1 = -b1;

            float c = diff.LengthSquared();
            float det = Math.Abs(1.0f - (a01 * a01));
            float s0, s1, extDet0, extDet1, tmpS0, tmpS1;

            if (det >= MathHelper.ZeroTolerance)
            {
                //Segments are not parallel
                s0 = (a01 * b1) - b0;
                s1 = (a01 * b0) - b1;
                extDet0 = segAExtent * det;
                extDet1 = segBExtent * det;

                if (s0 >= -extDet0)
                {
                    if (s0 <= extDet0)
                    {
                        if (s1 >= -extDet1)
                        {
                            if (s1 <= extDet1) //region 0 (interior)
                            {
                                //Minimum at interior points of segments
                                float invDet = 1.0f / det;
                                s0 *= invDet;
                                s1 *= invDet;

                                squaredDistance = (s0 * (s0 + (a01 * s1) + (2.0f * b0))) + (s1 * ((a01 * s0) + s1 + (2.0f * b1))) + c;
                            }
                            else //region 3 (side)
                            {
                                s1 = segBExtent;
                                tmpS0 = -((a01 * s1) + b0);
                                if (tmpS0 < -segAExtent)
                                {
                                    s0 = -segAExtent;
                                    squaredDistance = (s0 * (s0 - (2.0f * tmpS0))) + (s1 * (s1 + (2.0f * b1))) + c;
                                }
                                else if (tmpS0 <= segAExtent)
                                {
                                    s0 = tmpS0;
                                    squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                                }
                                else
                                {
                                    s0 = segAExtent;
                                    squaredDistance = (s0 * (s0 - (2.0f * tmpS0))) + (s1 * (s1 + (2.0f * b1))) + c;
                                }
                            }
                        }
                        else //region 7 (side)
                        {
                            s1 = -segBExtent;
                            tmpS0 = -((a01 * s1) + b0);
                            if (tmpS0 < -segAExtent)
                            {
                                s0 = -segAExtent;
                                squaredDistance = (s0 * (s0 - (2.0f * tmpS0))) + (s1 * (s1 + (2.0f * b1))) + c;
                            }
                            else if (tmpS0 <= segAExtent)
                            {
                                s0 = tmpS0;
                                squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                            }
                            else
                            {
                                s0 = segAExtent;
                                squaredDistance = (s0 * (s0 - (2.0f * tmpS0))) + (s1 * (s1 + (2.0f * b1))) + c;
                            }
                        }
                    }
                    else
                    {
                        if (s1 >= -extDet1)
                        {
                            if (s1 <= extDet1) //region 1 (side)
                            {
                                s0 = segAExtent;
                                tmpS1 = -((a01 * s0) + b1);
                                if (tmpS1 < -segBExtent)
                                {
                                    s1 = -segBExtent;
                                    squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                                else if (tmpS1 <= segBExtent)
                                {
                                    s1 = tmpS1;
                                    squaredDistance = (-s1 * s1) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                                else
                                {
                                    s1 = segBExtent;
                                    squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                            }
                            else //region 2 (corner)
                            {
                                s1 = segBExtent;
                                tmpS0 = -((a01 * s1) + b0);
                                if (tmpS0 < -segAExtent)
                                {
                                    s0 = -segAExtent;
                                    squaredDistance = (s0 * (s0 - (2.0f * tmpS0))) + (s1 * (s1 + (2.0f * b1))) + c;
                                }
                                else if (tmpS0 <= segAExtent)
                                {
                                    s0 = tmpS0;
                                    squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                                }
                                else
                                {
                                    s0 = segAExtent;
                                    tmpS1 = -((a01 * s0) + b1);
                                    if (tmpS1 < -segBExtent)
                                    {
                                        s1 = -segBExtent;
                                        squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                                    }
                                    else if (tmpS1 <= segBExtent)
                                    {
                                        s1 = tmpS1;
                                        squaredDistance = (-s1 * s1) + (s0 * (s0 + (2.0f * b0))) + c;
                                    }
                                    else
                                    {
                                        s1 = segBExtent;
                                        squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                                    }
                                }
                            }
                        }
                        else //region 8 (corner)
                        {
                            s1 = -segBExtent;
                            tmpS0 = -((a01 * s1) + b0);
                            if (tmpS0 < -segAExtent)
                            {
                                s0 = -segAExtent;
                                squaredDistance = (s0 * (s0 - (2.0f * tmpS0))) + (s1 * (s1 + (2.0f * b1))) + c;
                            }
                            else if (tmpS0 <= segAExtent)
                            {
                                s0 = tmpS0;
                                squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                            }
                            else
                            {
                                s0 = segAExtent;
                                tmpS1 = -((a01 * s0) + b1);
                                if (tmpS1 > segBExtent)
                                {
                                    s1 = segBExtent;
                                    squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                                else if (tmpS1 >= -segBExtent)
                                {
                                    s1 = tmpS1;
                                    squaredDistance = (-s1 * s1) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                                else
                                {
                                    s1 = -segBExtent;
                                    squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (s1 >= -extDet1)
                    {
                        if (s1 <= extDet1) //region 5 (side)
                        {
                            s0 = -segAExtent;
                            tmpS1 = -((a01 * s0) + b1);
                            if (tmpS1 < -segBExtent)
                            {
                                s1 = -segBExtent;
                                squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                            }
                            else if (tmpS1 <= segBExtent)
                            {
                                s1 = tmpS1;
                                squaredDistance = (-s1 * s1) + (s0 * (s0 + (2.0f * b0))) + c;
                            }
                            else
                            {
                                s1 = segBExtent;
                                squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                            }
                        }
                        else //region 4 (corner)
                        {
                            s1 = segBExtent;
                            tmpS0 = -((a01 * s1) + b0);
                            if (tmpS0 > segAExtent)
                            {
                                s0 = segAExtent;
                                squaredDistance = (s0 * (s0 - (2.0f * tmpS0))) + (s1 * (s1 + (2.0f * b1))) + c;
                            }
                            else if (tmpS0 >= -segAExtent)
                            {
                                s0 = tmpS0;
                                squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                            }
                            else
                            {
                                s0 = -segAExtent;
                                tmpS1 = -((a01 * s0) + b1);
                                if (tmpS1 < -segBExtent)
                                {
                                    s1 = -segBExtent;
                                    squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                                else if (tmpS1 <= segBExtent)
                                {
                                    s1 = tmpS1;
                                    squaredDistance = (-s1 * s1) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                                else
                                {
                                    s1 = segBExtent;
                                    squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                                }
                            }
                        }
                    }
                    else //region 6 (corner)
                    {
                        s1 = -segBExtent;
                        tmpS0 = -((a01 * s1) + b0);
                        if (tmpS0 > segAExtent)
                        {
                            s0 = segAExtent;
                            squaredDistance = (s0 * (s0 - (2.0f * tmpS0))) + (s1 * (s1 + (2.0f * b1))) + c;
                        }
                        else if (tmpS0 >= -segAExtent)
                        {
                            s0 = tmpS0;
                            squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                        }
                        else
                        {
                            s0 = -segAExtent;
                            tmpS1 = -((a01 * s0) + b1);
                            if (tmpS1 < -segBExtent)
                            {
                                s1 = -segBExtent;
                                squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                            }
                            else if (tmpS1 <= segBExtent)
                            {
                                s1 = tmpS1;
                                squaredDistance = (-s1 * s1) + (s0 * (s0 + (2.0f * b0))) + c;
                            }
                            else
                            {
                                s1 = segBExtent;
                                squaredDistance = (s1 * (s1 - (2.0f * tmpS1))) + (s0 * (s0 + (2.0f * b0))) + c;
                            }
                        }
                    }
                }
            }
            else
            {
                //The segments are parallel
                float extentSum = segAExtent + segBExtent;
                float sign = (a01 > 0.0f) ? -1.0f : 1.0f;
                float b0Average = 0.5f * (b0 - (sign * b1));
                float lambda = MathHelper.Clamp(-b0Average, -extentSum, extentSum);

                s1 = -sign * lambda * segBExtent / extentSum;
                s0 = lambda + (sign * s1);
                squaredDistance = (lambda * (lambda + (2.0f * b0Average))) + c;
            }

            //Account for numerical round off error
            if (squaredDistance < 0.0f)
                squaredDistance = 0.0f;

            segAParameter = s0 + segAExtent;
            segBParameter = s1 + segBExtent;
        }

        //Ray parameters from origin
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceRayRay(ref Ray ray0, ref Ray ray1, out float ray0Parameter, out float ray1Parameter, out float squaredDistance)
        {
            Vector3 diff;
            Vector3.Subtract(ref ray0.Origin, ref ray1.Origin, out diff);

            float a01, b0;
            Vector3.Dot(ref ray0.Direction, ref ray1.Direction, out a01);
            Vector3.Dot(ref diff, ref ray0.Direction, out b0);
            a01 = -a01;
            float c = diff.LengthSquared();
            float det = Math.Abs(1.0f - (a01 * a01));
            float b1, s0, s1;

            if (det >= MathHelper.ZeroTolerance)
            {
                //Rays are not parallel
                Vector3.Dot(ref diff, ref ray1.Direction, out b1);
                b1 = -b1;
                s0 = (a01 * b1) - b0;
                s1 = (a01 * b0) - b1;

                if (s0 >= 0.0f)
                {
                    if (s1 >= 0.0f) //Region 0 (interior)
                    {
                        //Minimum at two interior points of ray
                        float invDet = 1.0f / det;
                        s0 *= invDet;
                        s1 *= invDet;
                        squaredDistance = (s0 * (s0 + (a01 * s1) + (2.0f * b0))) + (s1 * ((a01 * s0) + s1 + (2.0f * b1))) + c;
                    }
                    else //Region 3 (side)
                    {
                        s1 = 0.0f;
                        if (b0 >= 0.0f)
                        {
                            s0 = 0.0f;
                            squaredDistance = c;
                        }
                        else
                        {
                            s0 = -b0;
                            squaredDistance = (b0 * s0) + c;
                        }
                    }
                }
                else
                {
                    if (s1 >= 0.0f) //Region 1 (side)
                    {
                        s0 = 0.0f;
                        if (b1 >= 0.0f)
                        {
                            s1 = 0.0f;
                            squaredDistance = c;
                        }
                        else
                        {
                            s1 = -b1;
                            squaredDistance = (b1 * s1) + c;
                        }
                    }
                    else //Region 2 (corner)
                    {
                        if (b0 < 0.0f)
                        {
                            s0 = -b0;
                            s1 = 0.0f;
                            squaredDistance = (b0 * s0) + c;
                        }
                        else
                        {
                            s0 = 0.0f;
                            if (b1 >= 0.0f)
                            {
                                s1 = 0.0f;
                                squaredDistance = c;
                            }
                            else
                            {
                                s1 = -b1;
                                squaredDistance = (b1 * s1) + c;
                            }
                        }
                    }
                }
            }
            else
            {
                //Rays are parallel
                if (a01 > 0.0f)
                {
                    //Opposite direction
                    s1 = 0.0f;
                    if (b0 >= 0.0f)
                    {
                        s0 = 0.0f;
                        squaredDistance = c;
                    }
                    else
                    {
                        s0 = -b0;
                        squaredDistance = (b0 * s0) + c;
                    }
                }
                else
                {
                    //Same direction
                    if (b0 >= 0.0f)
                    {
                        Vector3.Dot(ref diff, ref ray1.Direction, out b1);
                        b1 = -b1;
                        s0 = 0.0f;
                        s1 = -b1;
                        squaredDistance = (b1 * s1) + c;
                    }
                    else
                    {
                        s0 = -b0;
                        s1 = 0.0f;
                        squaredDistance = (b0 * s0) + c;
                    }
                }
            }

            ray0Parameter = s0;
            ray1Parameter = s1;

            //Account for numerical round-off errors
            if (squaredDistance < 0.0f)
                squaredDistance = 0.0f;
        }

        //Ray and Segment parameters from origin and Start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceRaySegment(ref Ray ray, ref Segment segment, out float rayParameter, out float segParameter, out float squaredDistance)
        {
            Vector3 segCenter, segDir;
            float segExtent;
            CalculateSegmentProperties(ref segment, out segCenter, out segDir, out segExtent);

            Vector3 diff;
            Vector3.Subtract(ref ray.Origin, ref segCenter, out diff);

            float a01;
            Vector3.Dot(ref ray.Direction, ref segDir, out a01);
            a01 = -a01;

            float b0;
            Vector3.Dot(ref diff, ref ray.Direction, out b0);

            float b1;
            Vector3.Dot(ref diff, ref segDir, out b1);
            b1 = -b1;

            float c = diff.LengthSquared();
            float det = Math.Abs(1.0f - (a01 * a01));
            float s0, s1, extDet;

            if (det >= MathHelper.ZeroTolerance)
            {
                //Ray and segment are not parallel
                s0 = (a01 * b1) - b0;
                s1 = (a01 * b0) - b1;
                extDet = segExtent * det;

                if (s0 >= 0.0f)
                {
                    if (s1 >= -extDet)
                    {
                        if (s1 <= extDet) //region 0
                        {
                            //Minimum at interior points of ray and segment
                            float invDet = 1.0f / det;
                            s0 *= invDet;
                            s1 *= invDet;

                            squaredDistance = (s0 * (s0 + (a01 * s1) + (2.0f * b0))) + (s1 * ((a01 * s0) + s1 + (2.0f * b1))) + c;
                        }
                        else //region 1
                        {
                            s1 = segExtent;
                            s0 = -((a01 * s1) + b0);
                            if (s0 > 0.0f)
                            {
                                squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                            }
                            else
                            {
                                s0 = 0.0f;
                                squaredDistance = (s1 * (s1 + (2.0f * b1))) + c;
                            }
                        }
                    }
                    else //region 5
                    {
                        s1 = -segExtent;
                        s0 = -((a01 * s1) + b0);

                        if (s0 > 0.0f)
                        {
                            squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                        }
                        else
                        {
                            s0 = 0.0f;
                            squaredDistance = (s1 * (s1 + (2.0f * b1))) + c;
                        }
                    }
                }
                else
                {
                    if (s1 <= -extDet) //region 4
                    {
                        s0 = -((-a01 * segExtent) + b0);
                        if (s0 > 0.0f)
                        {
                            s1 = -segExtent;
                            squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                        }
                        else
                        {
                            s0 = 0.0f;
                            s1 = MathHelper.Clamp(-b1, -segExtent, segExtent);
                            squaredDistance = (s1 * (s1 + (2.0f * b1))) + c;
                        }
                    }
                    else if (s1 <= extDet) //region 3
                    {
                        s0 = 0.0f;
                        s1 = MathHelper.Clamp(-b1, -segExtent, segExtent);
                        squaredDistance = (s1 * (s1 + (2.0f * b1))) + c;
                    }
                    else //region 2
                    {
                        s0 = -((a01 * segExtent) + b0);
                        if (s0 > 0.0f)
                        {
                            s1 = segExtent;
                            squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                        }
                        else
                        {
                            s0 = 0.0f;
                            s1 = MathHelper.Clamp(-b1, -segExtent, segExtent);
                            squaredDistance = (s1 * (s1 + (2.0f * b1))) + c;
                        }
                    }
                }
            }
            else
            {
                //Ray and segment are parallel
                if (a01 > 0.0f)
                {
                    //Opposite direction vectors
                    s1 = -segExtent;
                }
                else
                {
                    //Same direction vectors
                    s1 = segExtent;
                }

                s0 = -((a01 * s1) + b0);
                if (s0 > 0.0f)
                {
                    squaredDistance = (-s0 * s0) + (s1 * (s1 + (2.0f * b1))) + c;
                }
                else
                {
                    s0 = 0.0f;
                    squaredDistance = (s1 * (s1 + (2.0f * b1))) + c;
                }
            }

            //Account for numerical round off error
            if (squaredDistance < 0.0f)
                squaredDistance = 0.0f;

            rayParameter = s0;
            segParameter = s1 + segExtent;
        }

        //Line and segment parameters from origin and Start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceLineSegment(ref Vector3 lineOrigin, ref Vector3 lineDir, ref Segment segment, out float lineParameter, out float segmentParameter, out float squaredDistance)
        {
            Vector3 segDir, segCenter;
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out segDir);

            //Calculate segment extent
            float segHalfLength = segDir.Length() * 0.5f;
            segDir.Normalize();

            //Calculate segmnet center
            Vector3.Multiply(ref segDir, segHalfLength, out segCenter);
            Vector3.Add(ref segment.StartPoint, ref segCenter, out segCenter);

            //Line-Segment distance query
            Vector3 diff;
            Vector3.Subtract(ref lineOrigin, ref segCenter, out diff);

            float a01, b0;
            Vector3.Dot(ref lineDir, ref segDir, out a01);
            Vector3.Dot(ref diff, ref lineDir, out b0);

            a01 = -a01;
            float c = diff.LengthSquared();
            float det = (float)Math.Abs(1.0f - (a01 * a01));
            float b1, s0, s1, extDet;

            if (det >= MathHelper.ZeroTolerance)
            {
                //Line and segment are not parallel.
                Vector3.Dot(ref diff, ref segDir, out b1);
                b1 = -b1;
                s1 = (a01 * b0) - b1;
                extDet = segHalfLength * det;

                if (s1 >= -extDet)
                {
                    if (s1 <= extDet)
                    {
                        //Two interior points are closest
                        float invDet = 1.0f / det;
                        s0 = (a01 * b1 - b0) * invDet;
                        s1 *= invDet;
                        squaredDistance = s0 * (s0 + (a01 * s1) + (2.0f * b0)) + s1 * ((a01 * s0) + s1 + (2.0f * b1)) + c;
                    }
                    else
                    {
                        //End point of the segment and an interior point of line are closest
                        s1 = segHalfLength;
                        s0 = -((a01 * s1) + b0);
                        squaredDistance = -(s0 * s0) + s1 * (s1 + (2.0f * b1)) + c;
                    }
                }
                else
                {
                    //Start point of segment and interior point of line are closest
                    s1 = -segHalfLength;
                    s0 = -((a01 * s1) + b0);
                    squaredDistance = -(s0 * s0) + s1 * (s1 + (2.0f * b1)) + c;
                }
            }
            else
            {
                //End point of segment and interior point of line are closest
                s1 = 0.0f;
                s0 = -b0;
                squaredDistance = (b0 * s0) + c;
            }

            //Account for numerical round off error
            if (squaredDistance < 0.0f)
                squaredDistance = 0.0f;

            lineParameter = s0;
            segmentParameter = s1 + segHalfLength;
        }

        #endregion

        #region Plane queries

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceRayPlane(ref Plane plane, ref Ray ray, out Vector3 ptOnPlane, out Vector3 ptOnRay, out float squaredDistance)
        {
            LineIntersectionResult interResult;
            if (!IntersectRayPlane(ref ray, ref plane, out interResult))
            {
                //If not intersects, then ray is parallel to the plane, so many solutions, or ray is pointing away. Take distance
                //from the origin
                DistancePointPlane(ref plane, ref ray.Origin, out ptOnPlane, out squaredDistance);
                ptOnRay = ray.Origin;
            }

            ptOnPlane = interResult.Point;
            ptOnRay = interResult.Point;
            squaredDistance = 0.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceSegmentPlane(ref Plane plane, ref Segment segment, out Vector3 ptOnPlane, out Vector3 ptOnSegment, out float squaredDistance)
        {
            LineIntersectionResult interResult;
            if (!IntersectSegmentPlane(ref segment, ref plane, out interResult))
            {
                //If not intersects, check each of the endpoints
                Vector3 tempPt1, tempPt2;
                float sqrDist1, sqrDist2;
                DistancePointPlane(ref plane, ref segment.StartPoint, out tempPt1, out sqrDist1);
                DistancePointPlane(ref plane, ref segment.EndPoint, out tempPt2, out sqrDist2);

                if (sqrDist1 < sqrDist2)
                {
                    squaredDistance = sqrDist1;
                    ptOnPlane = tempPt1;
                    ptOnSegment = segment.StartPoint;
                }
                else
                {
                    squaredDistance = sqrDist2;
                    ptOnPlane = tempPt2;
                    ptOnSegment = segment.EndPoint;
                }
            }

            ptOnPlane = interResult.Point;
            ptOnSegment = interResult.Point;
            squaredDistance = 0.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistancePointPlane(ref Plane plane, ref Vector3 point, out Vector3 ptOnPlane, out float squaredDistance)
        {
            float signedDistance;
            plane.SignedDistanceTo(ref point, out signedDistance);

            Vector3 tmp;
            Vector3.Multiply(ref plane.Normal, signedDistance, out tmp);
            Vector3.Subtract(ref point, ref tmp, out ptOnPlane);

            squaredDistance = signedDistance * signedDistance;
        }

        //Line and Segment parameters from origin and Start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectLinePlane(ref Vector3 lineOrigin, ref Vector3 lineDir, ref Plane plane, out float lineParam)
        {
            float DdN;
            Vector3.Dot(ref lineDir, ref plane.Normal, out DdN);

            float signedDistance;
            plane.SignedDistanceTo(ref lineOrigin, out signedDistance);
            if (!MathHelper.IsApproxZero(DdN))
            {
                //Line is not parallel to the plane, so they must intersect
                lineParam = -signedDistance / DdN;
                return true;
            }

            //Line and plane are parallel, determine if they are coincident
            if (MathHelper.IsApproxZero(signedDistance))
            {
                //Choose line origin as intersection, can choose any point on line
                lineParam = 0.0f;
                return true;
            }

            lineParam = 0.0f;
            return false;
        }

        //Ray parameters from origin respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayPlane(ref Ray ray, ref Plane plane)
        {
            LineIntersectionResult result;
            return IntersectRayPlane(ref ray, ref plane, out result);
        }

        //Ray parameters from origin respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayPlane(ref Ray ray, ref Plane plane, out LineIntersectionResult result)
        {
            float t;
            if (IntersectLinePlane(ref ray.Origin, ref ray.Direction, ref plane, out t))
            {
                //Line intersects plane, but possibly at a point not on ray
                if (t >= 0.0f)
                {
                    Vector3 pt;
                    Vector3.Multiply(ref ray.Direction, t, out pt);
                    Vector3.Add(ref pt, ref ray.Origin, out pt);

                    result = new LineIntersectionResult(pt, t, plane.Normal);
                    return true;
                }
            }

            result = new LineIntersectionResult();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentPlane(ref Segment segment, ref Plane plane)
        {
            float squaredDistance0;
            plane.SignedDistanceTo(ref segment.StartPoint, out squaredDistance0);
            if (Math.Abs(squaredDistance0) <= MathHelper.ZeroTolerance)
                squaredDistance0 = 0.0f;

            float squaredDistance1;
            plane.SignedDistanceTo(ref segment.EndPoint, out squaredDistance1);
            if (Math.Abs(squaredDistance1) <= MathHelper.ZeroTolerance)
                squaredDistance1 = 0.0f;

            float prod = squaredDistance0 * squaredDistance1;
            if (prod < 0.0f)
            {
                //Segment passes through the plane
                return true;
            }

            if (prod > 0.0f)
            {
                //Segment is on one side of the plane
                return false;
            }

            if (squaredDistance0 != 0.0f || squaredDistance1 != 0.0f)
            {
                //Segment end point touches the plane
                return true;
            }

            //Segment is coincident with plane
            return true;
        }

        //Segment parameters from Start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentPlane(ref Segment segment, ref Plane plane, out LineIntersectionResult result)
        {
            Vector3 segCenter, segDir;
            float segExtent;
            CalculateSegmentProperties(ref segment, out segCenter, out segDir, out segExtent);

            float t;
            if (IntersectLinePlane(ref segCenter, ref segDir, ref plane, out t))
            {
                //Line intersects plane, but possibly at a point not on segment
                if (Math.Abs(t) <= segExtent)
                {
                    Vector3 pt;
                    Vector3.Multiply(ref segDir, t, out pt);
                    Vector3.Add(ref pt, ref segCenter, out pt);

                    result = new LineIntersectionResult(pt, t + segExtent, plane.Normal);
                    return true;
                }
            }

            result = new LineIntersectionResult();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectPlanePlane(ref Plane plane0, ref Plane plane1)
        {
            float dot;
            Plane.DotNormal(ref plane0, ref plane1, out dot);

            if (Math.Abs(dot) < 1.0f)
                return true;

            //Planes are parallel check if they are coplanar. If >= then normals are in same direction, else in opposite direction
            float cDiff = (dot >= 0.0f) ? plane0.D - plane1.D : plane0.D + plane1.D;

            return MathHelper.IsApproxZero(cDiff);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectPlanePlane(ref Plane plane0, ref Plane plane1, out Ray result)
        {
            float dot;
            Plane.DotNormal(ref plane0, ref plane1, out dot);

            if (Math.Abs(dot) >= 1.0f)
            {
                //Planes are parallel check if they are coplanar. If >= then normals are in same direction, else in opposite direction
                float cDiff = (dot >= 0.0f) ? plane0.D - plane1.D : plane0.D + plane1.D;

                if (MathHelper.IsApproxZero(cDiff))
                {
                    //Planes are coplanar
                    result.Origin = plane0.Origin;
                    Vector3.NormalizedCross(ref plane0.Normal, ref plane1.Normal, out result.Direction);
                    return true;
                }

                //Parallel but distinct
                result = new Ray();
                return false;
            }

            float invDet = 1.0f / (1.0f - (dot * dot));
            float c0 = (plane0.D - (dot * plane1.D)) * invDet;
            float c1 = (plane1.D - (dot * plane0.D)) * invDet;

            Vector3 tmp;
            Vector3.Multiply(ref plane0.Normal, c0, out tmp);
            Vector3.Multiply(ref plane1.Normal, c1, out result.Origin);
            Vector3.Add(ref result.Origin, ref tmp, out result.Origin);

            Vector3.NormalizedCross(ref plane0.Normal, ref plane1.Normal, out result.Direction);
            return true;
        }

        #endregion

        #region Triangle queries

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistancePlaneTriangle(ref Triangle tri, ref Plane plane, out Vector3 ptOnTriangle, out Vector3 ptOnPlane, out float squaredDistance)
        {
            squaredDistance = float.MaxValue;
            ptOnPlane = Vector3.Zero;
            ptOnTriangle = Vector3.Zero;

            //Compare edges of triangle to plane
            for (int i0 = 2, i1 = 0; i1 < 3; i0 = i1++)
            {
                Segment seg = new Segment(tri[i1], tri[i0]);
                Vector3 ptOnPlaneTmp, ptOnSegment;
                float sqrDistTemp;
                DistanceSegmentPlane(ref plane, ref seg, out ptOnPlaneTmp, out ptOnSegment, out sqrDistTemp);

                if (sqrDistTemp < squaredDistance)
                {
                    squaredDistance = sqrDistTemp;
                    ptOnTriangle = ptOnSegment;
                    ptOnPlane = ptOnPlaneTmp;

                    //Intersection detected!
                    if (squaredDistance <= MathHelper.ZeroTolerance)
                    {
                        squaredDistance = 0.0f;
                        return;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectPlaneTriangle(ref Triangle tri, ref Plane plane)
        {
            // Compute signed distance from vertices of triangle to the plane;
            plane.SignedDistanceTo(ref tri.PointA, out float pADist);
            plane.SignedDistanceTo(ref tri.PointB, out float pBDist);
            plane.SignedDistanceTo(ref tri.PointC, out float pCDist);

            if (Math.Abs(pADist) <= MathHelper.ZeroTolerance)
            {
                pADist = 0.0f;
            }

            if (Math.Abs(pBDist) <= MathHelper.ZeroTolerance)
            {
                pBDist = 0.0f;
            }

            if (Math.Abs(pCDist) <= MathHelper.ZeroTolerance)
            {
                pCDist = 0.0f;
            }

            // Triangle intersects the plane if not all vertices are on the positive side of the plane and not all vertices are on the negative side
            return !(pADist > 0.0f && pBDist > 0.0f && pCDist > 0.0f) && !(pADist < 0.0f && pBDist < 0.0f && pCDist < 0.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectPlaneTriangle(ref Triangle tri, ref Plane plane, out Segment intersection)
        {
            intersection = new Segment();

            //Compute signed distance from vertices of triangle to the plane;
            float pADist, pBDist, pCDist;
            plane.SignedDistanceTo(ref tri.PointA, out pADist);
            plane.SignedDistanceTo(ref tri.PointB, out pBDist);
            plane.SignedDistanceTo(ref tri.PointC, out pCDist);

            if (Math.Abs(pADist) <= MathHelper.ZeroTolerance)
                pADist = 0.0f;

            if (Math.Abs(pBDist) <= MathHelper.ZeroTolerance)
                pBDist = 0.0f;

            if (Math.Abs(pCDist) <= MathHelper.ZeroTolerance)
                pCDist = 0.0f;

            if (pADist > 0.0f)
            {
                if (pBDist > 0.0f)
                {
                    if (pCDist > 0.0f)
                    {
                        // ppp
                        return false;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // ppm
                        float factor = pADist / (pADist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        factor = pBDist / (pBDist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointB, out intersection.EndPoint);
                        Vector3.Multiply(ref intersection.EndPoint, factor, out intersection.EndPoint);
                        Vector3.Add(ref intersection.EndPoint, ref tri.PointB, out intersection.EndPoint);

                        return true;
                    }
                    else
                    {
                        // ppz
                        intersection.StartPoint = tri.PointC;
                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                }
                else if (pBDist < 0.0f)
                {
                    if (pCDist > 0.0f)
                    {
                        // pmp
                        float factor = pADist / (pADist - pBDist);
                        Vector3.Subtract(ref tri.PointB, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        factor = pBDist / (pBDist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointB, out intersection.EndPoint);
                        Vector3.Multiply(ref intersection.EndPoint, factor, out intersection.EndPoint);
                        Vector3.Add(ref intersection.EndPoint, ref tri.PointB, out intersection.EndPoint);

                        return true;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // pmm
                        float factor = pADist / (pADist - pBDist);
                        Vector3.Subtract(ref tri.PointB, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        factor = pADist / (pADist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        return true;
                    }
                    else
                    {
                        // pmz
                        float factor = pADist / (pADist - pBDist);
                        Vector3.Subtract(ref tri.PointB, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                }
                else
                {
                    if (pCDist > 0.0f)
                    {
                        // pzp
                        intersection.StartPoint = tri.PointB;
                        intersection.EndPoint = tri.PointB;

                        return true;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // pzm
                        float factor = pADist / (pADist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        intersection.EndPoint = tri.PointB;

                        return true;
                    }
                    else
                    {
                        // pzz
                        intersection.StartPoint = tri.PointB;
                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                }
            }
            else if (pADist < 0.0f)
            {
                if (pBDist > 0.0f)
                {
                    if (pCDist > 0.0f)
                    {
                        // mpp
                        float factor = pADist / (pADist - pBDist);
                        Vector3.Subtract(ref tri.PointB, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        factor = pADist / (pADist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointA, out intersection.EndPoint);
                        Vector3.Multiply(ref intersection.EndPoint, factor, out intersection.EndPoint);
                        Vector3.Add(ref intersection.EndPoint, ref tri.PointA, out intersection.EndPoint);

                        return true;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // mpm
                        float factor = pADist / (pADist - pBDist);
                        Vector3.Subtract(ref tri.PointB, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        factor = pBDist / (pBDist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointB, out intersection.EndPoint);
                        Vector3.Multiply(ref intersection.EndPoint, factor, out intersection.EndPoint);
                        Vector3.Add(ref intersection.EndPoint, ref tri.PointB, out intersection.EndPoint);

                        return true;
                    }
                    else
                    {
                        // mpz
                        float factor = pADist / (pADist - pBDist);
                        Vector3.Subtract(ref tri.PointB, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                }
                else if (pBDist < 0.0f)
                {
                    if (pCDist > 0.0f)
                    {
                        // mmp
                        float factor = pADist / (pADist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        factor = pBDist / (pBDist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointB, out intersection.EndPoint);
                        Vector3.Multiply(ref intersection.EndPoint, factor, out intersection.EndPoint);
                        Vector3.Add(ref intersection.EndPoint, ref tri.PointB, out intersection.EndPoint);

                        return true;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // mmm
                        return false;
                    }
                    else
                    {
                        // mmz
                        intersection.StartPoint = tri.PointC;
                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                }
                else
                {
                    if (pCDist > 0.0f)
                    {
                        // mzp
                        float factor = pADist / (pADist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointA, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointA, out intersection.StartPoint);

                        intersection.EndPoint = tri.PointB;

                        return true;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // mzm
                        intersection.StartPoint = tri.PointB;
                        intersection.EndPoint = tri.PointB;

                        return true;
                    }
                    else
                    {
                        // mzz
                        intersection.StartPoint = tri.PointB;
                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                }
            }
            else
            {
                if (pBDist > 0.0f)
                {
                    if (pCDist > 0.0f)
                    {
                        // zpp
                        intersection.StartPoint = tri.PointA;
                        intersection.EndPoint = tri.PointA;

                        return true;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // zpm
                        float factor = pBDist / (pBDist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointB, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointB, out intersection.StartPoint);

                        intersection.EndPoint = tri.PointB;

                        return true;
                    }
                    else
                    {
                        // zpz
                        intersection.StartPoint = tri.PointA;
                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                }
                else if (pBDist < 0.0f)
                {
                    if (pCDist > 0.0f)
                    {
                        // zmp
                        float factor = pBDist / (pBDist - pCDist);
                        Vector3.Subtract(ref tri.PointC, ref tri.PointB, out intersection.StartPoint);
                        Vector3.Multiply(ref intersection.StartPoint, factor, out intersection.StartPoint);
                        Vector3.Add(ref intersection.StartPoint, ref tri.PointB, out intersection.StartPoint);

                        intersection.EndPoint = tri.PointA;

                        return true;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // zmm

                        intersection.StartPoint = tri.PointA;
                        intersection.EndPoint = tri.PointB;

                        return true;
                    }
                    else
                    {
                        // zmz
                        intersection.StartPoint = tri.PointA;
                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                }
                else
                {
                    if (pCDist > 0.0f)
                    {
                        // zzp
                        intersection.StartPoint = tri.PointA;
                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                    else if (pCDist < 0.0f)
                    {
                        // zzm
                        intersection.StartPoint = tri.PointA;
                        intersection.EndPoint = tri.PointC;

                        return true;
                    }
                    else
                    {
                        // zzz

                        //Triangle lies in the plane, so the intersection is the triangle itself
                        intersection.StartPoint = tri.PointA;
                        intersection.EndPoint = tri.PointB;

                        return true;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IntersectTriangleTriangle(ref Triangle firstTri, ref Triangle secondTri)
        {
            //Edge vectors for first
            Vector3* E0 = stackalloc Vector3[3];
            E0[0] = firstTri.EdgeCA;
            E0[1] = firstTri.EdgeBA;
            E0[2] = firstTri.EdgeBC;

            Vector3 firstNormal = firstTri.Normal;

            //Project second triangle onto normal line of first, test for separation
            float dot;
            Vector3.Dot(ref firstNormal, ref firstTri.PointA, out dot);
            float min1, max1;
            ProjectOntoAxis(ref secondTri, ref firstNormal, out min1, out max1);
            if (dot < min1 || dot > max1)
                return false;

            //Edge vectors for second
            Vector3* E1 = stackalloc Vector3[3];
            E1[0] = secondTri.EdgeCA;
            E1[1] = secondTri.EdgeBA;
            E1[2] = secondTri.EdgeBC;

            Vector3 secondNormal = secondTri.Normal;

            Vector3 dir;
            float min0, max0;

            Vector3 crossNormals;
            Vector3.NormalizedCross(ref firstNormal, ref secondNormal, out crossNormals);
            Vector3.Dot(ref crossNormals, ref crossNormals, out dot);

            if (dot >= MathHelper.ZeroTolerance)
            {
                //Triangles are not parallel

                //Project first onto normal of second, test for separation
                Vector3.Dot(ref secondNormal, ref secondTri.PointA, out dot);
                ProjectOntoAxis(ref firstTri, ref secondNormal, out min0, out max0);
                if (dot < min0 || dot > max0)
                    return false;

                //Test each edge vector projection
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector3.NormalizedCross(ref E0[j], ref E1[i], out dir);
                        ProjectOntoAxis(ref firstTri, ref dir, out min0, out max0);
                        ProjectOntoAxis(ref secondTri, ref dir, out min1, out max1);
                        if (max0 < min1 || max1 < min0)
                            return false;
                    }
                }
            }
            else //Triangles are parallel and coplanar
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3.NormalizedCross(ref firstNormal, ref E0[i], out dir);
                    ProjectOntoAxis(ref firstTri, ref dir, out min0, out max0);
                    ProjectOntoAxis(ref secondTri, ref dir, out min1, out max1);
                    if (max0 < min1 || max1 < min0)
                        return false;
                }

                for (int i = 0; i < 3; i++)
                {
                    Vector3.NormalizedCross(ref secondNormal, ref E1[i], out dir);
                    ProjectOntoAxis(ref firstTri, ref dir, out min0, out max0);
                    ProjectOntoAxis(ref secondTri, ref dir, out min1, out max1);
                    if (max0 < min1 || max1 < min0)
                        return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectTriangleTriangle(ref Triangle firstTri, ref Triangle secondTri, out Segment result)
        {
            //TODO
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentTriangle(ref Triangle tri, ref Segment segment, bool ignoreBackface, out LineIntersectionResult result)
        {
            result = new LineIntersectionResult();

            Vector3 segCenter, segDir;
            float segExtent;
            CalculateSegmentProperties(ref segment, out segCenter, out segDir, out segExtent);

            //Triangle type represents in clockwise ordering (imagine ABC where A is lower left and goes clockwise). Eberly's algorithm assumes a counterclockwise
            //triangle ABC where A is lower left and goes counterclockwise.

            Vector3 diff, edge0, edge1, normal;
            Vector3.Subtract(ref segCenter, ref tri.PointA, out diff);
            Vector3.Subtract(ref tri.PointC, ref tri.PointA, out edge0);
            Vector3.Subtract(ref tri.PointB, ref tri.PointA, out edge1);
            Vector3.Cross(ref edge0, ref edge1, out normal);

            //Solve Q + t*D = b1*E0 + b2*E1 (Q = diff, D = segment direction, E0 = edge0, E1 = edge2, N = Cross(E1, E0))
            float dirDotNormal;
            Vector3.Dot(ref segDir, ref normal, out dirDotNormal);
            float sign;

            if (dirDotNormal > 0.0f)
            {
                sign = 1.0f;

                if (ignoreBackface)
                    return false;
            }
            else if (dirDotNormal < 0.0f)
            {
                sign = -1.0f;
                dirDotNormal = -dirDotNormal;
            }
            else
            {
                //Segment and triangle are parallel, return false even if they may be coincident.
                return false;
            }

            float dirDotQXE1;
            Vector3 temp;
            Vector3.Cross(ref diff, ref edge1, out temp);
            Vector3.Dot(ref segDir, ref temp, out dirDotQXE1);
            dirDotQXE1 *= sign;

            if (dirDotQXE1 >= 0.0f)
            {
                float dirDotE0XQ;
                Vector3.Cross(ref edge0, ref diff, out temp);
                Vector3.Dot(ref segDir, ref temp, out dirDotE0XQ);
                dirDotE0XQ *= sign;

                if (dirDotE0XQ >= 0.0f)
                {
                    if (dirDotQXE1 + dirDotE0XQ <= dirDotNormal)
                    {
                        //Line intersects triangle, check if segment does
                        float QDotNormal;
                        Vector3.Dot(ref diff, ref normal, out QDotNormal);
                        QDotNormal *= -sign;

                        float extDirDotNormal = segExtent * dirDotNormal;

                        if (-extDirDotNormal <= QDotNormal && QDotNormal <= extDirDotNormal)
                        {
                            //Segment intersects triangle;
                            float segParam = QDotNormal * (1.0f / dirDotNormal);

                            Vector3 pt;
                            Vector3.Multiply(ref segDir, segParam, out pt);
                            Vector3.Add(ref pt, ref segCenter, out pt);

                            normal.Normalize();
                            result = new LineIntersectionResult(pt, segParam + segExtent, normal);
                            return true;
                        }
                        //else: |t| > extent, no intersection
                    }
                    //else: b1 + b2 > 1, no intersection
                }
                //else: b2 < 0, no intersection
            }
            //else: b1 < 0, no intersection

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayTriangle(ref Triangle tri, ref Ray ray, bool ignoreBackface, out LineIntersectionResult result)
        {
            result = new LineIntersectionResult();

            //Triangle type represents in clockwise ordering (imagine ABC where A is lower left and goes clockwise). Eberly's algorithm assumes a counterclockwise
            //triangle ABC where A is lower left and goes counterclockwise.

            Vector3 diff, edge0, edge1, normal;
            Vector3.Subtract(ref ray.Origin, ref tri.PointA, out diff);
            Vector3.Subtract(ref tri.PointC, ref tri.PointA, out edge0);
            Vector3.Subtract(ref tri.PointB, ref tri.PointA, out edge1);
            Vector3.Cross(ref edge0, ref edge1, out normal);

            //Solve Q + t*D = b1*E0 + b2*E1 (Q = diff, D = ray direction, E0 = edge0, E1 = edge2, N = Cross(E1, E0))
            float dirDotNormal;
            Vector3.Dot(ref ray.Direction, ref normal, out dirDotNormal);
            float sign;

            if (dirDotNormal > 0.0f)
            {
                sign = 1.0f;

                if (ignoreBackface)
                    return false;
            }
            else if (dirDotNormal < 0.0f)
            {
                sign = -1.0f;
                dirDotNormal = -dirDotNormal;
            }
            else
            {
                //Ray and triangle are parallel, return false even if they may be coincident.
                return false;
            }

            float dirDotQXE1;
            Vector3 temp;
            Vector3.Cross(ref diff, ref edge1, out temp);
            Vector3.Dot(ref ray.Direction, ref temp, out dirDotQXE1);
            dirDotQXE1 *= sign;

            if (dirDotQXE1 >= 0.0f)
            {
                float dirDotE0XQ;
                Vector3.Cross(ref edge0, ref diff, out temp);
                Vector3.Dot(ref ray.Direction, ref temp, out dirDotE0XQ);
                dirDotE0XQ *= sign;

                if (dirDotE0XQ >= 0.0f)
                {
                    if (dirDotQXE1 + dirDotE0XQ <= dirDotNormal)
                    {
                        //Line intersects triangle, check if ray does
                        float QDotNormal;
                        Vector3.Dot(ref diff, ref normal, out QDotNormal);
                        QDotNormal *= -sign;

                        if (QDotNormal >= 0.0f)
                        {
                            //Ray intersects triangle
                            float rayParam = QDotNormal * (1.0f / dirDotNormal);

                            Vector3 pt;
                            Vector3.Multiply(ref ray.Direction, rayParam, out pt);
                            Vector3.Add(ref pt, ref ray.Origin, out pt);

                            normal.Normalize();
                            result = new LineIntersectionResult(pt, rayParam, normal);
                            return true;
                        }
                        //else: t < 0, no intersection
                    }
                    //else: b1 + b2 > 1, no intersection
                }
                //else: b2 < 0, no intersection
            }
            //else: b1 < 0, no intersection

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceTriangleTriangle(ref Triangle firstTri, ref Triangle secondTri, out Vector3 ptOnFirstTriangle, out Vector3 ptOnSecondTriangle, out float squaredDistance)
        {
            float sqrDistTemp;
            squaredDistance = float.MaxValue;

            ptOnFirstTriangle = Vector3.Zero;
            ptOnSecondTriangle = Vector3.Zero;

            //Compare edges of first triangle to interior of the second
            for (int i0 = 2, i1 = 0; i1 < 3; i0 = i1++)
            {
                Segment seg = new Segment(firstTri[i1], firstTri[i0]);
                Vector3 ptOnSecondTriTemp, ptOnSegment;
                DistanceSegmentTriangle(ref secondTri, ref seg, out ptOnSecondTriTemp, out ptOnSegment, out sqrDistTemp);

                if (sqrDistTemp < squaredDistance)
                {
                    squaredDistance = sqrDistTemp;
                    ptOnFirstTriangle = ptOnSegment;
                    ptOnSecondTriangle = ptOnSecondTriTemp;

                    //Intersection detected!
                    if (squaredDistance <= MathHelper.ZeroTolerance)
                    {
                        squaredDistance = 0.0f;
                        return;
                    }
                }
            }

            //Compare edges of second triangle to interior of the first
            for (int i0 = 2, i1 = 0; i1 < 3; i0 = i1++)
            {
                Segment seg = new Segment(secondTri[i1], secondTri[i0]);
                Vector3 ptOnFirstTriTemp, ptOnSegment;
                DistanceSegmentTriangle(ref firstTri, ref seg, out ptOnFirstTriTemp, out ptOnSegment, out sqrDistTemp);

                if (sqrDistTemp < squaredDistance)
                {
                    squaredDistance = sqrDistTemp;
                    ptOnFirstTriangle = ptOnFirstTriTemp;
                    ptOnSecondTriangle = ptOnSegment;

                    //Intersection detected!
                    if (squaredDistance <= MathHelper.ZeroTolerance)
                    {
                        squaredDistance = 0.0f;
                        return;
                    }
                }
            }
        }

        //Line and Segment parameters from origin and Start point respectively
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceLineTriangle(ref Triangle tri, ref Vector3 lineOrigin, ref Vector3 lineDir, out Vector3 ptOnTriangle, out float lineParameter, out float squaredDistance)
        {
            Vector3 edge0, edge1, normal;
            Vector3.Subtract(ref tri.PointB, ref tri.PointA, out edge0);
            Vector3.Subtract(ref tri.PointC, ref tri.PointA, out edge1);
            Vector3.NormalizedCross(ref edge1, ref edge0, out normal); //Clockwise ordering

            float normalDotDir;
            Vector3.Dot(ref normal, ref lineDir, out normalDotDir);

            if (Math.Abs(normalDotDir) > MathHelper.ZeroTolerance)
            {
                //Line and triangle are not parallel, so the line intersects the plane of the triangle
                Vector3 diff, U, V;
                Vector3.Subtract(ref lineOrigin, ref tri.PointA, out diff);
                Vector3.ComplementBasis(ref lineDir, out U, out V);

                float UdotE0, UdotE1, UdotDiff, VdotE0, VdotE1, VdotDiff;
                Vector3.Dot(ref U, ref edge0, out UdotE0);
                Vector3.Dot(ref U, ref edge1, out UdotE1);
                Vector3.Dot(ref U, ref diff, out UdotDiff);

                Vector3.Dot(ref V, ref edge0, out VdotE0);
                Vector3.Dot(ref V, ref edge1, out VdotE1);
                Vector3.Dot(ref V, ref diff, out VdotDiff);

                float invDet = 1.0f / ((UdotE0 * VdotE1) - (UdotE1 * VdotE0));

                //Barycentric coordinates for the point of intersection
                float b1 = ((VdotE1 * UdotDiff) - (UdotE1 * VdotDiff)) * invDet;
                float b2 = ((UdotE0 * VdotDiff) - (VdotE0 * UdotDiff)) * invDet;
                float b0 = 1.0f - b1 - b2; //Barycentric coordinates add to one

                //If barycentric coordinates valid, then we really do intersect the triangle.
                if (b0 >= 0.0f && b1 >= 0.0f && b2 >= 0.0f)
                {
                    float dirDotE0, dirDotE1, dirDotDiff;
                    Vector3.Dot(ref lineDir, ref edge0, out dirDotE0);
                    Vector3.Dot(ref lineDir, ref edge1, out dirDotE1);
                    Vector3.Dot(ref lineDir, ref diff, out dirDotDiff);

                    lineParameter = (b1 * dirDotE0) + (b2 * dirDotE1) - dirDotDiff;

                    //Calculate intersection pt on triangle
                    Vector3.Multiply(ref lineDir, lineParameter, out ptOnTriangle);
                    Vector3.Add(ref lineOrigin, ref ptOnTriangle, out ptOnTriangle);

                    squaredDistance = 0.0f;
                    return;
                }
            }

            //Either the line is not parallel to the triangle and it misses intersecting it. Or they are parallel. Either way the closest point on the triangle
            //is on the edge, compare the line to all three edges of the triangle.
            squaredDistance = float.MaxValue;
            lineParameter = 0.0f;
            ptOnTriangle = Vector3.Zero;

            for (int i0 = 2, i1 = 0; i1 < 3; i0 = i1++)
            {
                Segment seg = new Segment(tri[i1], tri[i0]);
                float lineParamTemp, segmentParamTemp, sqrDistTemp;

                DistanceLineSegment(ref lineOrigin, ref lineDir, ref seg, out lineParamTemp, out segmentParamTemp, out sqrDistTemp);

                if (sqrDistTemp < squaredDistance)
                {
                    squaredDistance = sqrDistTemp;
                    lineParameter = lineParamTemp;

                    Vector3 temp = lineDir;
                    Vector3.Multiply(ref lineDir, segmentParamTemp, out ptOnTriangle);
                    Vector3.Add(ref ptOnTriangle, ref seg.StartPoint, out ptOnTriangle);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceRayTriangle(ref Triangle tri, ref Ray ray, out Vector3 ptOnTriangle, out Vector3 ptOnRay, out float squaredDistance)
        {
            float lineParameter;
            DistanceLineTriangle(ref tri, ref ray.Origin, ref ray.Direction, out ptOnTriangle, out lineParameter, out squaredDistance);

            //If line parameter is greater or equal to zero (origin or along direction) then the solution is on the ray. Otherwise it's on the extension of the line,
            //so our solution will by the origin point.
            if (lineParameter >= 0.0f)
            {
                //On ray - calculate point from line parameter;
                Vector3.Multiply(ref ray.Direction, lineParameter, out ptOnRay);
                Vector3.Add(ref ray.Origin, ref ptOnRay, out ptOnRay);
            }
            else
            {
                //Off the ray, take origin as solution
                DistancePointTriangle(ref tri, ref ray.Origin, out ptOnTriangle, out squaredDistance);
                ptOnRay = ray.Origin;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistanceSegmentTriangle(ref Triangle tri, ref Segment segment, out Vector3 ptOnTriangle, out Vector3 ptOnSegment, out float squaredDistance)
        {
            Vector3 segCenter, segDir;
            float segExtent;
            CalculateSegmentProperties(ref segment, out segCenter, out segDir, out segExtent);

            float lineParameter;
            DistanceLineTriangle(ref tri, ref segCenter, ref segDir, out ptOnTriangle, out lineParameter, out squaredDistance);

            //If line parameter is [-extent, extent] then the solution is on the segment. Otherwise it's on the extension of the line, so our solution
            //will be one of the end points.
            if (lineParameter >= -segExtent)
            {
                if (lineParameter <= segExtent)
                {
                    //On the segment - calculate point from line parameter
                    Vector3.Multiply(ref segDir, lineParameter, out ptOnSegment);
                    Vector3.Add(ref ptOnSegment, ref segCenter, out ptOnSegment);
                }
                else
                {
                    //Off the segment, take end point as solution
                    DistancePointTriangle(ref tri, ref segment.EndPoint, out ptOnTriangle, out squaredDistance);
                    ptOnSegment = segment.EndPoint;
                }
            }
            else
            {
                //Off the segment, take start point as solution
                DistancePointTriangle(ref tri, ref segment.StartPoint, out ptOnTriangle, out squaredDistance);
                ptOnSegment = segment.StartPoint;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistancePointTriangle(ref Triangle tri, ref Vector3 point, out Vector3 ptOnTriangle, out float squaredDistance)
        {
            Vector3 diff, edge0, edge1;
            Vector3.Subtract(ref tri.PointA, ref point, out diff);
            Vector3.Subtract(ref tri.PointB, ref tri.PointA, out edge0);
            Vector3.Subtract(ref tri.PointC, ref tri.PointC, out edge1);

            float a00, a01, a11, b0, b1, c, det, s, t;

            a00 = edge0.LengthSquared();
            a11 = edge1.LengthSquared();
            c = diff.LengthSquared();
            Vector3.Dot(ref edge0, ref edge1, out a01);
            Vector3.Dot(ref diff, ref edge0, out b0);
            Vector3.Dot(ref diff, ref edge1, out b1);

            det = (float)Math.Abs((a00 * a11) - (a01 * a01));
            s = (a01 * b1) - (a11 * b0);
            t = (a01 * b0) - (a00 * b1);

            if (s + t <= det)
            {
                if (s < 0.0f)
                {
                    if (t < 0.0f) //Region 4
                    {
                        if (b0 < 0.0f)
                        {
                            t = 0.0f;
                            if (-b0 >= a00)
                            {
                                s = 1.0f;
                                squaredDistance = a00 + (2.0f * b0) + c;
                            }
                            else
                            {
                                s = -b0 / a00;
                                squaredDistance = (b0 * s) + c;
                            }
                        }
                        else
                        {
                            s = 0.0f;
                            if (b1 >= 0.0f)
                            {
                                t = 0.0f;
                                squaredDistance = c;
                            }
                            else if (-b1 >= a11)
                            {
                                t = 1.0f;
                                squaredDistance = a11 + (2.0f * b1) + c;
                            }
                            else
                            {
                                t = -b1 / a11;
                                squaredDistance = (b1 * t) + c;
                            }
                        }
                    }
                    else //Region 3
                    {
                        s = 0.0f;
                        if (b1 >= 0.0f)
                        {
                            t = 0.0f;
                            squaredDistance = c;
                        }
                        else if (-b1 >= a11)
                        {
                            t = 1.0f;
                            squaredDistance = a11 + (2.0f * b1) + c;
                        }
                        else
                        {
                            t = -b1 / a11;
                            squaredDistance = (b1 * t) + c;
                        }
                    }
                }
                else if (t < 0.0f) //Region 5
                {
                    t = 0.0f;
                    if (b0 >= 0.0f)
                    {
                        s = 0.0f;
                        squaredDistance = c;
                    }
                    else if (-b0 >= a00)
                    {
                        s = 1.0f;
                        squaredDistance = a00 + (2.0f * b0) + c;
                    }
                    else
                    {
                        s = -b0 / a00;
                        squaredDistance = (b0 * s) + c;
                    }
                }
                else //Region 0 (Interior point)
                {
                    float invDet = 1.0f / det;
                    s *= invDet;
                    t *= invDet;
                    squaredDistance = s * ((a00 * s) + (a01 * t) + (2.0f * b0)) + t * ((a01 * s) + (a11 * t) + (2.0f * b1)) + c;
                }
            }
            else
            {
                float temp0, temp1, numer, denom;

                if (s < 0.0f) //Region 2
                {
                    temp0 = a01 + b0;
                    temp1 = a11 + b1;
                    if (temp1 > temp0)
                    {
                        numer = temp1 - temp0;
                        denom = a00 - (2.0f * a01) + a11;
                        if (numer >= denom)
                        {
                            s = 1.0f;
                            t = 0.0f;
                            squaredDistance = a00 + (2.0f * b0) + c;
                        }
                        else
                        {
                            s = numer / denom;
                            t = 1.0f - s;
                            squaredDistance = s * ((a00 * s) + (a01 * t) + (2.0f * b0)) + t * ((a01 * s) + (a11 * t) + (2.0f * b1)) + c;
                        }
                    }
                    else
                    {
                        s = 0.0f;
                        if (temp1 <= 0.0f)
                        {
                            t = 1.0f;
                            squaredDistance = a11 + (2.0f * b1) + c;
                        }
                        else if (b1 >= 0.0f)
                        {
                            t = 0.0f;
                            squaredDistance = c;
                        }
                        else
                        {
                            t = -b1 / a11;
                            squaredDistance = (b1 * t) + c;
                        }
                    }
                }
                else if (t < 0.0f) //Region 6
                {
                    temp0 = a01 + b1;
                    temp1 = a00 + b0;
                    if (temp1 > temp0)
                    {
                        numer = temp1 - temp0;
                        denom = a00 - (2.0f * a01) + a11;
                        if (numer >= denom)
                        {
                            t = 1.0f;
                            s = 0.0f;
                            squaredDistance = a11 + (2.0f * b1) + c;
                        }
                        else
                        {
                            t = numer / denom;
                            s = 1.0f - t;
                            squaredDistance = s * ((a00 * s) + (a01 * t) + (2.0f * b0)) + t * ((a01 * s) + (a11 * t) + (2.0f * b1)) + c;
                        }
                    }
                    else
                    {
                        t = 0.0f;
                        if (temp1 <= 0.0f)
                        {
                            s = 1.0f;
                            squaredDistance = a00 + (2.0f * b0) + c;
                        }
                        else if (b0 >= 0.0f)
                        {
                            s = 0.0f;
                            squaredDistance = c;
                        }
                        else
                        {
                            s = -b0 / a00;
                            squaredDistance = (b0 * s) + c;
                        }
                    }
                }
                else //Region 1
                {
                    numer = a11 + b1 - a01 - b0;
                    if (numer <= 0.0f)
                    {
                        s = 0.0f;
                        t = 1.0f;
                        squaredDistance = a11 + (2.0f * b1) + c;
                    }
                    else
                    {
                        denom = a00 - (2.0f * a01) + a11;
                        if (numer >= denom)
                        {
                            s = 1.0f;
                            t = 0.0f;
                            squaredDistance = a00 + (2.0f * b0) + c;
                        }
                        else
                        {
                            s = numer / denom;
                            t = 1.0f - s;
                            squaredDistance = s * ((a00 * s) + (a01 * t) + (2.0f * b0)) + t * ((a01 * s) + (a11 * t) + (2.0f * b1)) + c;
                        }
                    }
                }
            }

            //Account for numerical round off error
            if (squaredDistance < 0.0f)
                squaredDistance = 0.0f;

            Vector3.Multiply(ref edge0, s, out edge0);
            Vector3.Multiply(ref edge1, t, out edge1);
            Vector3.Add(ref tri.PointA, ref edge0, out ptOnTriangle);
            Vector3.Add(ref ptOnTriangle, ref edge1, out ptOnTriangle);
        }

        #endregion

        #region Bounding Box (AAB + OBB) queries

        //From Real-time Collision Detection by Christer Ericson
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistancePointBox(ref Vector3 point, ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents, out float squaredDistance)
        {
            Vector3 v;
            Vector3.Subtract(ref point, ref center, out v);

            squaredDistance = 0.0f;
            Triad axisItr;
            Triad.FromAxes(ref xAxis, ref yAxis, ref zAxis, out axisItr);

            for (int i = 0; i < 3; i++)
            {
                Vector3 axis;
                axisItr.GetAxis(i, out axis);

                //Project vector from box center to point on each axis, getting the distance of point along that axis and counting any excess distance outside box extents
                float dist;
                float excess = 0.0f;
                float extent = extents[i];
                Vector3.Dot(ref v, ref axis, out dist);

                if (dist < -extent)
                    excess = dist + extent;
                else if (dist > extent)
                    excess = dist - extent;

                squaredDistance += excess * excess;
            }
        }

        //From Real-time Collision Detection by Christer Ericson
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DistancePointAABB(ref Vector3 point, ref Vector3 center, ref Vector3 extents, out float squaredDistance)
        {
            Vector3 max = new Vector3(center.X + extents.X, center.Y + extents.Y, center.Z + extents.Z);
            Vector3 min = new Vector3(center.X - extents.X, center.Y - extents.Y, center.Z - extents.Z);

            squaredDistance = 0.0f;

            //For each axis count any excess distance outside box extents
            for (int i = 0; i < 3; i++)
            {
                float v = point[i];
                float mini = min[i];
                float maxi = max[i];

                if (v < mini)
                    squaredDistance += (mini - v) * (mini - v);

                if (v > maxi)
                    squaredDistance += (v - maxi) * (v - maxi);
            }
        }

        //From Real-time Collision Detection by Christer Ericson
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClosestPointToBox(ref Vector3 point, ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents, out Vector3 result)
        {
            Vector3 diff;
            Vector3.Subtract(ref point, ref center, out diff);

            //Start result at center
            result = center;
            float dist;

            Triad axisItr;
            Triad.FromAxes(ref xAxis, ref yAxis, ref zAxis, out axisItr);

            //For each axis, calculate distance along that axis and step the result along it
            for (int i = 0; i < 3; i++)
            {
                Vector3 axis;
                axisItr.GetAxis(i, out axis);

                Vector3.Dot(ref diff, ref axis, out dist);

                float extent = extents[i];

                if (dist > extent)
                    dist = extent;

                if (dist < -extent)
                    dist = -extent;

                Vector3.Multiply(ref axis, dist, out axis);
                Vector3.Add(ref axis, ref result, out result);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentBox(ref Segment segment, ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents)
        {
            float AWdU0, AWdU1, AWdU2;
            float ADdU0, ADdU1, ADdU2;
            float AWxDdU0, AWxDdU1, AWxDdU2;

            Vector3 segDir, segCenter;
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out segDir);

            //Calculate segment extent
            float segHalfLength = segDir.Length() * 0.5f;
            segDir.Normalize();

            //Calculate segmnet center
            Vector3.Multiply(ref segDir, segHalfLength, out segCenter);
            Vector3.Add(ref segment.StartPoint, ref segCenter, out segCenter);

            Vector3 diff;
            Vector3.Subtract(ref segCenter, ref center, out diff);

            //Check X axis
            Vector3.Dot(ref segDir, ref xAxis, out AWdU0);
            AWdU0 = Math.Abs(AWdU0);

            Vector3.Dot(ref diff, ref xAxis, out ADdU0);
            ADdU0 = Math.Abs(ADdU0);

            if (ADdU0 > (extents.X + (segHalfLength * AWdU0)))
                return false;

            //Check Y axis
            Vector3.Dot(ref segDir, ref yAxis, out AWdU1);
            AWdU1 = Math.Abs(AWdU1);

            Vector3.Dot(ref diff, ref yAxis, out ADdU1);
            ADdU1 = Math.Abs(ADdU1);

            if (ADdU1 > (extents.Y + (segHalfLength * AWdU1)))
                return false;

            //Check Z axis
            Vector3.Dot(ref segDir, ref zAxis, out AWdU2);
            AWdU2 = Math.Abs(AWdU2);

            Vector3.Dot(ref diff, ref zAxis, out ADdU2);
            ADdU2 = Math.Abs(ADdU2);

            if (ADdU2 > (extents.Z + (segHalfLength * AWdU2)))
                return false;

            Vector3 WxD;
            Vector3.Cross(ref segDir, ref diff, out WxD);

            //Check X axis
            Vector3.Dot(ref WxD, ref xAxis, out AWxDdU0);
            AWxDdU0 = Math.Abs(AWxDdU0);

            if (AWxDdU0 > ((extents.Y * AWdU2) + (extents.Z * AWdU1)))
                return false;

            //Check Y axis
            Vector3.Dot(ref WxD, ref yAxis, out AWxDdU1);
            AWxDdU1 = Math.Abs(AWxDdU1);

            if (AWxDdU1 > ((extents.X * AWdU2) + (extents.Z * AWdU0)))
                return false;

            //Check Z axis
            Vector3.Dot(ref WxD, ref zAxis, out AWxDdU2);
            AWxDdU2 = Math.Abs(AWxDdU2);

            if (AWxDdU2 > ((extents.X * AWdU1) + (extents.Y * AWdU0)))
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentBox(ref Segment segment, ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents, out BoundingIntersectionResult result)
        {
            Vector3 segDir, segCenter;
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out segDir);

            //Calculate segment extent
            float segHalfLength = segDir.Length() * 0.5f;
            segDir.Normalize();

            //Calculate segment center
            Vector3.Multiply(ref segDir, segHalfLength, out segCenter);
            Vector3.Add(ref segment.StartPoint, ref segCenter, out segCenter);

            float t0, t1;
            int numIntersections;

            if (ClipLineBox(ref segCenter, ref segDir, ref center, ref xAxis, ref yAxis, ref zAxis, ref extents, out t0, out t1, out numIntersections))
            {
                return FindOverlapOnPrimitive(t0, t1, numIntersections, ref segCenter, ref segDir, -segHalfLength, segHalfLength, out result);
            }

            result = new BoundingIntersectionResult();
            return false;
        }

        //From Eberly's GeometricTools
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentAABB(ref Segment segment, ref Vector3 center, ref Vector3 extents)
        {
            Vector3 segDir, segCenter;
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out segDir);

            //Calculate segment extent
            float segHalfLength = segDir.Length() * 0.5f;
            segDir.Normalize();

            //Calculate segment center
            Vector3.Multiply(ref segDir, segHalfLength, out segCenter);
            Vector3.Add(ref segment.StartPoint, ref segCenter, out segCenter);

            //Translate segment to AABB coordinate system
            Vector3.Subtract(ref segCenter, ref center, out segCenter);

            //Try world coordinate axes as separating axes
            Vector3 absSegDir = new Vector3(Math.Abs(segDir.X), Math.Abs(segDir.Y), Math.Abs(segDir.Z));

            float adx = absSegDir.X * segHalfLength;
            if (Math.Abs(segCenter.X) > (extents.X + adx))
                return false;

            float ady = absSegDir.Y * segHalfLength;
            if (Math.Abs(segCenter.Y) > (extents.Y + ady))
                return false;

            float adz = absSegDir.Z * segHalfLength;
            if (Math.Abs(segCenter.Z) > (extents.Z + adz))
                return false;

            //Try cross products of segment direction vector with coordinate axes
            Vector3 WxD;
            Vector3.Cross(ref segDir, ref segCenter, out WxD);

            if (Math.Abs(WxD.X) > (extents.Y * absSegDir.Z + extents.Z * absSegDir.Y))
                return false;

            if (Math.Abs(WxD.Y) > (extents.X * absSegDir.Z + extents.Z * absSegDir.X))
                return false;

            if (Math.Abs(WxD.Z) > (extents.X * absSegDir.Y + extents.Y * absSegDir.X))
                return false;

            //No separating axis found, must be intersecting
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentAABB(ref Segment segment, ref Vector3 center, ref Vector3 extents, out BoundingIntersectionResult result)
        {
            Vector3 segDir, segCenter;
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out segDir);

            //Calculate segment extent
            float segHalfLength = segDir.Length() * 0.5f;
            segDir.Normalize();

            //Calculate segment center
            Vector3.Multiply(ref segDir, segHalfLength, out segCenter);
            Vector3.Add(ref segment.StartPoint, ref segCenter, out segCenter);

            int numIntersections;
            float t0, t1;
            if (ClipLineAABB(ref segCenter, ref segDir, ref center, ref extents, out t0, out t1, out numIntersections))
            {
                return FindOverlapOnPrimitive(t0, t1, numIntersections, ref segCenter, ref segDir, -segHalfLength, segHalfLength, out result);
            }

            result = new BoundingIntersectionResult();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayBox(ref Ray ray, ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents)
        {
            float WdU0, WdU1, WdU2;
            float AWdU0, AWdU1, AWdU2;
            float DdU0, DdU1, DdU2;
            float ADdU0, ADdU1, ADdU2;
            float AWxDdU0, AWxDdU1, AWxDdU2;

            Vector3 rayDir = ray.Direction;

            Vector3 diff;
            Vector3.Subtract(ref ray.Origin, ref center, out diff);

            //Check X axis
            Vector3.Dot(ref rayDir, ref xAxis, out WdU0);
            AWdU0 = Math.Abs(WdU0);

            Vector3.Dot(ref diff, ref xAxis, out DdU0);
            ADdU0 = Math.Abs(DdU0);

            if ((ADdU0 > extents.X) && ((DdU0 * WdU0) >= 0.0f))
                return false;

            //Check Y axis
            Vector3.Dot(ref rayDir, ref yAxis, out WdU1);
            AWdU1 = Math.Abs(WdU1);

            Vector3.Dot(ref diff, ref yAxis, out DdU1);
            ADdU1 = Math.Abs(DdU1);

            if ((ADdU1 > extents.Y) && ((DdU1 * WdU1) >= 0.0f))
                return false;

            //Check Z axis
            Vector3.Dot(ref rayDir, ref zAxis, out WdU2);
            AWdU2 = Math.Abs(WdU2);

            Vector3.Dot(ref diff, ref zAxis, out DdU2);
            ADdU2 = Math.Abs(DdU2);

            if ((ADdU2 > extents.Z) && ((DdU2 * WdU2) >= 0.0f))
                return false;

            Vector3 WxD;
            Vector3.Cross(ref rayDir, ref diff, out WxD);

            //Check X axis
            Vector3.Dot(ref WxD, ref xAxis, out AWxDdU0);
            AWxDdU0 = Math.Abs(AWxDdU0);
            if (AWxDdU0 > ((extents.Y * AWdU2) + (extents.Z * AWdU1)))
                return false;

            //Check Y axis
            Vector3.Dot(ref WxD, ref yAxis, out AWxDdU1);
            AWxDdU1 = Math.Abs(AWxDdU1);
            if (AWxDdU1 > ((extents.X * AWdU2) + (extents.Z * AWdU0)))
                return false;

            //check Z axis
            Vector3.Dot(ref WxD, ref zAxis, out AWxDdU2);
            AWxDdU2 = Math.Abs(AWxDdU2);
            if (AWxDdU2 > ((extents.X * AWdU1) + (extents.Y * AWdU0)))
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayBox(ref Ray ray, ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents, out BoundingIntersectionResult result)
        {
            float t0, t1;
            int numIntersections;

            if (ClipLineBox(ref ray.Origin, ref ray.Direction, ref center, ref xAxis, ref yAxis, ref zAxis, ref extents, out t0, out t1, out numIntersections))
            {
                return FindOverlapOnPrimitive(t0, t1, numIntersections, ref ray.Origin, ref ray.Direction, 0.0f, float.MaxValue, out result);
            }

            result = new BoundingIntersectionResult();
            return false;
        }

        //From Real-time Collision Detection by Christer Ericson
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayAABB(ref Ray ray, ref Vector3 center, ref Vector3 extents)
        {
            float tmin = 0.0f;
            float tmax = float.MaxValue;

            Vector3 rayDir = ray.Direction;
            Vector3 rayOrigin = ray.Origin;
            Vector3 max = new Vector3(center.X + extents.X, center.Y + extents.Y, center.Z + extents.Z);
            Vector3 min = new Vector3(center.X - extents.X, center.Y - extents.Y, center.Z - extents.Z);

            //For all three slabs
            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs(rayDir[i]) < MathHelper.ZeroTolerance)
                {
                    if (rayOrigin[i] < min[i] || rayOrigin[i] > max[i])
                        return false;
                }
                else
                {
                    //Compute intersection t value of ray with near and far plane of slab
                    float ood = 1.0f / rayDir[i];
                    float t1 = (min[i] - rayOrigin[i]) * ood;
                    float t2 = (max[i] - rayOrigin[i]) * ood;

                    //Make t1 be intersection with near plane, t2 with far plane
                    if (t1 > t2)
                    {
                        float temp = t2;
                        t2 = t1;
                        t1 = temp;
                    }

                    //Compute the intersection of slab intersection intervals
                    tmin = Math.Max(tmin, t1);
                    tmax = Math.Min(tmax, t2);

                    //Exit as soon as slab intersection becomes empty
                    if (tmin > tmax)
                        return false;
                }
            }

            //Ray intersects all 3 slabs
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayAABB(ref Ray ray, ref Vector3 center, ref Vector3 extents, out BoundingIntersectionResult result)
        {
            float t0, t1;
            int numIntersections;

            if (ClipLineAABB(ref ray.Origin, ref ray.Direction, ref center, ref extents, out t0, out t1, out numIntersections))
            {
                return FindOverlapOnPrimitive(t0, t1, numIntersections, ref ray.Origin, ref ray.Direction, 0.0f, float.MaxValue, out result);
            }

            result = new BoundingIntersectionResult();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectTriangleBox(ref Triangle triangle, ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents)
        {
            float min0, max0, min1, max1;
            Vector3 D, edge0, edge1, edge2;

            //Test direction of triangle normal
            Vector3.Subtract(ref triangle.PointB, ref triangle.PointA, out edge0);
            Vector3.Subtract(ref triangle.PointC, ref triangle.PointA, out edge1);
            Vector3.Cross(ref edge0, ref edge1, out D);
            Vector3.Dot(ref D, ref triangle.PointA, out min0);
            max0 = min0;

            GetProjectionBox(ref D, ref center, ref xAxis, ref yAxis, ref zAxis, ref extents, out min1, out max1);
            if (max1 < min0 || max0 < min1)
                return false;

            //Test direction of box faces

            Triad axisItr;
            Triad.FromAxes(ref xAxis, ref yAxis, ref zAxis, out axisItr);

            for (int i = 0; i < 3; i++)
            {
                float extent = extents[i];
                axisItr.GetAxis(i, out D);

                GetProjectionTriangle(ref D, ref triangle, out min0, out max0);

                float DdC;
                Vector3.Dot(ref D, ref center, out DdC);
                min1 = DdC - extent;
                max1 = DdC - extent;

                if (max1 < min0 || max0 < min1)
                    return false;
            }

            //Test direction of triangle-box edge cross products
            Vector3.Subtract(ref edge1, ref edge0, out edge2);

            Triad edgeItr;
            Triad.FromAxes(ref edge0, ref edge1, ref edge2, out edgeItr);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 edge;
                    edgeItr.GetAxis(i, out edge);

                    Vector3 axis;
                    axisItr.GetAxis(j, out axis);

                    Vector3.Cross(ref edge, ref axis, out D);

                    GetProjectionTriangle(ref D, ref triangle, out min0, out max0);
                    GetProjectionBox(ref D, ref center, ref xAxis, ref yAxis, ref zAxis, ref extents, out min1, out max1);
                    if (max1 < min0 || max0 < min1)
                        return false;
                }
            }

            return true;
        }

        //From Real-time Collision Detection by Christer Ericson
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectAABBAABB(ref Vector3 b0Center, ref Vector3 b0Extents, ref Vector3 b1Center, ref Vector3 b1Extents)
        {
            if (Math.Abs(b0Center.X - b1Center.X) > (b0Extents.X + b1Extents.X))
                return false;

            if (Math.Abs(b0Center.Y - b1Center.Y) > (b0Extents.Y + b1Extents.Y))
                return false;

            if (Math.Abs(b0Center.Z - b1Center.Z) > (b0Extents.Z + b1Extents.Z))
                return false;

            return true;
        }

        //From Real-time Collision Detection by Christer Ericson
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectBoxBox(ref Vector3 b0Center, ref Vector3 b0XAxis, ref Vector3 b0YAxis, ref Vector3 b0ZAxis, ref Vector3 b0Extents,
            ref Vector3 b1Center, ref Vector3 b1XAxis, ref Vector3 b1YAxis, ref Vector3 b1ZAxis, ref Vector3 b1Extents)
        {
            float ra, rb;
            Matrix4x4 RotMat = new Matrix4x4();
            Matrix4x4 AbsRotMat = new Matrix4x4();

            Triad b0Axes, b1Axes;
            Triad.FromAxes(ref b0XAxis, ref b0YAxis, ref b0ZAxis, out b0Axes);
            Triad.FromAxes(ref b1XAxis, ref b1YAxis, ref b1ZAxis, out b1Axes);

            //Compute rotation matrix expressing b1 in b0's coordinate frame
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 b0Axis, b1Axis;
                    b0Axes.GetAxis(i, out b0Axis);
                    b1Axes.GetAxis(j, out b1Axis);

                    float dot;
                    Vector3.Dot(ref b0Axis, ref b1Axis, out dot);
                }
            }

            //Compute translation vector t
            Vector3 t;
            Vector3.Subtract(ref b1Center, ref b0Center, out t);

            //Bring translation into b0's coordinate frame
            float t0, t1, t2;
            Vector3.Dot(ref t, ref b0XAxis, out t0);
            Vector3.Dot(ref t, ref b0YAxis, out t1);
            Vector3.Dot(ref t, ref b0ZAxis, out t2);

            t = new Vector3(t0, t1, t2);

            //Compute common subexpressions. Add in an epsilon term to counteract arithmetic errors when
            //two edges are parallel and their cross product is nearly zero.
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    AbsRotMat[i, j] = Math.Abs(RotMat[i, j] + MathHelper.ZeroTolerance);

            //Test axes L = B0-X, L = B0-Y, L = B0-Z
            for (int i = 0; i < 3; i++)
            {
                ra = b0Extents[i];
                rb = (b1Extents.X * AbsRotMat[i, 0]) + (b1Extents.Y * AbsRotMat[i, 1]) + (b1Extents.Z * AbsRotMat[i, 2]);
                if (Math.Abs(t[i]) > (ra + rb))
                    return false;
            }

            //Test axes L = B1-X, L = B1-Y, L = B1-Z
            for (int i = 0; i < 3; i++)
            {
                ra = (b0Extents.X * AbsRotMat[0, i]) + (b0Extents.Y * AbsRotMat[1, i]) + (b0Extents.Z * AbsRotMat[2, i]);
                rb = b1Extents[i];
                if (Math.Abs((t.X * RotMat[0, i]) + (t.Y * RotMat[1, i]) + (t.Z * RotMat[2, i])) > (ra + rb))
                    return false;
            }

            //Test axis L = B0-X x B1-X
            ra = (b0Extents.Y * AbsRotMat[2, 0]) + (b0Extents.Z * AbsRotMat[1, 0]);
            rb = (b1Extents.Y * AbsRotMat[0, 2]) + (b1Extents.Z * AbsRotMat[0, 1]);
            if (Math.Abs((t.Z * RotMat[1, 0]) - (t.Y * RotMat[2, 0])) > (ra + rb))
                return false;

            //Test axis L = B0-X x B1-Y
            ra = (b0Extents.Y * AbsRotMat[2, 1]) + (b0Extents.Z * AbsRotMat[1, 1]);
            rb = (b1Extents.X * AbsRotMat[0, 2]) + (b1Extents.Z * AbsRotMat[0, 0]);
            if (Math.Abs((t.Z * RotMat[1, 1]) - (t.Y * RotMat[2, 1])) > (ra + rb))
                return false;

            //Test axis L = B0-X x B1-Z
            ra = (b0Extents.Y * AbsRotMat[2, 2]) + (b0Extents.Z * AbsRotMat[1, 2]);
            rb = (b1Extents.X * AbsRotMat[0, 1]) + (b1Extents.Z * AbsRotMat[0, 0]);
            if (Math.Abs((t.Z * RotMat[1, 2]) - (t.Y * RotMat[2, 2])) > (ra + rb))
                return false;

            //Test axis L = B0-Y x B1-X
            ra = (b0Extents.X * AbsRotMat[2, 0]) + (b0Extents.Z * AbsRotMat[0, 0]);
            rb = (b1Extents.Y * AbsRotMat[1, 2]) + (b1Extents.Z * AbsRotMat[1, 1]);
            if (Math.Abs((t.X * RotMat[2, 0]) - (t.Z * RotMat[0, 0])) > (ra + rb))
                return false;

            //Test axis L = B0-Y x B1-Y
            ra = (b0Extents.X * AbsRotMat[2, 1]) + (b0Extents.Z * AbsRotMat[0, 1]);
            rb = (b1Extents.X * AbsRotMat[1, 2]) + (b1Extents.Z * AbsRotMat[1, 0]);
            if (Math.Abs((t.X * RotMat[2, 1]) - (t.Z * RotMat[0, 1])) > (ra + rb))
                return false;

            //Test axis L = B0-Y x B1-Z
            ra = (b0Extents.X * AbsRotMat[2, 2]) + (b0Extents.Z * AbsRotMat[0, 2]);
            rb = (b1Extents.X * AbsRotMat[1, 1]) + (b1Extents.Y * AbsRotMat[1, 0]);
            if (Math.Abs((t.X * RotMat[2, 2]) - (t.Z * RotMat[0, 2])) > (ra + rb))
                return false;

            //Test axis L = B0-Z x B1-X
            ra = (b0Extents.X * AbsRotMat[1, 0]) + (b0Extents.Y * AbsRotMat[0, 0]);
            rb = (b1Extents.Y * AbsRotMat[2, 2]) + (b1Extents.Z * AbsRotMat[2, 1]);
            if (Math.Abs((t.Y * RotMat[0, 0]) - (t.X * RotMat[1, 0])) > (ra + rb))
                return false;

            //Test axis L = B0-Z x B1-Y
            ra = (b0Extents.X * AbsRotMat[1, 1]) + (b0Extents.Y * AbsRotMat[0, 1]);
            rb = (b1Extents.X * AbsRotMat[2, 2]) + (b1Extents.Z * AbsRotMat[2, 0]);
            if (Math.Abs((t.Y * RotMat[0, 1]) - (t.X * RotMat[1, 1])) > (ra + rb))
                return false;

            //Test axis L = B0-Z x B1-Z
            ra = (b0Extents.X * AbsRotMat[1, 2]) + (b0Extents.Y * AbsRotMat[0, 2]);
            rb = (b1Extents.X * AbsRotMat[2, 1]) + (b1Extents.Y * AbsRotMat[2, 0]);
            if (Math.Abs((t.Y * RotMat[0, 2]) - (t.X * RotMat[1, 2])) > (ra + rb))
                return false;

            //No separating axis is found, boxes must be overlapping
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ClipLineBox(ref Vector3 lineOrigin, ref Vector3 lineDir, ref Vector3 boxCenter, ref Vector3 boxXAxis, ref Vector3 boxYAxis, ref Vector3 boxZAxis, ref Vector3 boxExtents, out float t0, out float t1, out int numIntersections)
        {
            //Convert linear component to box coordinates
            Vector3 diff;
            Vector3.Subtract(ref lineOrigin, ref boxCenter, out diff);

            float bOriginX, bOriginY, bOriginZ;
            Vector3.Dot(ref diff, ref boxXAxis, out bOriginX);
            Vector3.Dot(ref diff, ref boxYAxis, out bOriginY);
            Vector3.Dot(ref diff, ref boxZAxis, out bOriginZ);

            float bDirX, bDirY, bDirZ;
            Vector3.Dot(ref lineDir, ref boxXAxis, out bDirX);
            Vector3.Dot(ref lineDir, ref boxYAxis, out bDirY);
            Vector3.Dot(ref lineDir, ref boxZAxis, out bDirZ);

            t0 = -float.MaxValue;
            t1 = float.MaxValue;
            numIntersections = 0;

            bool notCulled =
                Clip(+bDirX, -bOriginX - boxExtents.X, ref t0, ref t1) &&
                Clip(-bDirX, +bOriginX - boxExtents.X, ref t0, ref t1) &&
                Clip(+bDirY, -bOriginY - boxExtents.Y, ref t0, ref t1) &&
                Clip(-bDirY, +bOriginY - boxExtents.Y, ref t0, ref t1) &&
                Clip(+bDirZ, -bOriginZ - boxExtents.Z, ref t0, ref t1) &&
                Clip(-bDirZ, +bOriginZ - boxExtents.Z, ref t0, ref t1);

            if (notCulled)
            {
                if (t1 > t0)
                {
                    numIntersections = 2;
                    return true;
                }
                else
                {
                    numIntersections = 1;
                    t1 = t0;
                    return true;
                }
            }

            return false;
        }

        private static bool FindOverlapOnPrimitive(float t0, float t1, int numIntersections, ref Vector3 origin, ref Vector3 dir, float minRange, float maxRange, out BoundingIntersectionResult result)
        {
            switch (numIntersections)
            {
                case 2:
                    {
                        LineIntersectionResult? firstInter = null;
                        LineIntersectionResult? secondInter = null;
                        bool inRange = false;

                        if (MathHelper.InInterval(t0, minRange, maxRange))
                        {
                            Vector3 p0;
                            Vector3.Multiply(ref dir, t0, out p0);
                            Vector3.Add(ref p0, ref origin, out p0);

                            firstInter = new LineIntersectionResult(p0, t0);
                            inRange = true;
                        }

                        if (MathHelper.InInterval(t1, minRange, maxRange))
                        {
                            Vector3 p1;
                            Vector3.Multiply(ref dir, t1, out p1);
                            Vector3.Add(ref p1, ref origin, out p1);

                            if (firstInter == null)
                                firstInter = new LineIntersectionResult(p1, t1);
                            else
                                secondInter = new LineIntersectionResult(p1, t1);

                            inRange = true;
                        }

                        BoundingIntersectionResult.FromResults(ref firstInter, ref secondInter, out result);
                        return inRange;
                    }
                case 1:
                    {
                        if (MathHelper.InInterval(t0, minRange, maxRange))
                        {
                            Vector3 p0;
                            Vector3.Multiply(ref dir, t0, out p0);
                            Vector3.Add(ref p0, ref origin, out p0);

                            result = new BoundingIntersectionResult(new LineIntersectionResult(p0, t0));
                            return true;
                        }
                    }
                    break;
            }

            result = new BoundingIntersectionResult();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ClipLineAABB(ref Vector3 lineOrigin, ref Vector3 lineDir, ref Vector3 boxCenter, ref Vector3 boxExtents, out float t0, out float t1, out int numIntersections)
        {
            //Convert linear component to box coordinates
            Vector3 diff;
            Vector3.Subtract(ref lineOrigin, ref boxCenter, out diff);

            //Note - Main difference as the general Box one is AABB will be the unit standard axes, so the dot products are extra work

            t0 = -float.MaxValue;
            t1 = float.MaxValue;
            numIntersections = 0;

            bool notCulled =
                Clip(+lineDir.X, -diff.X - boxExtents.X, ref t0, ref t1) &&
                Clip(-lineDir.X, +diff.X - boxExtents.X, ref t0, ref t1) &&
                Clip(+lineDir.Y, -diff.Y - boxExtents.Y, ref t0, ref t1) &&
                Clip(-lineDir.Y, +diff.Y - boxExtents.Y, ref t0, ref t1) &&
                Clip(+lineDir.Z, -diff.Z - boxExtents.Z, ref t0, ref t1) &&
                Clip(-lineDir.Z, +diff.Z - boxExtents.Z, ref t0, ref t1);

            if (notCulled)
            {
                if (t1 > t0)
                {
                    numIntersections = 2;
                    return true;
                }
                else
                {
                    numIntersections = 1;
                    t1 = t0;
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Clip(float denom, float numer, ref float t0, ref float t1)
        {
            //Return true if line segment intersects current test plane. Otherwise return false and the line segment
            //is entirely  clipped

            if (denom > 0.0f)
            {
                if (numer > (denom * t1))
                    return false;

                if (numer > (denom * t0))
                    t0 = numer / denom;

                return true;
            }
            else if (denom < 0.0f)
            {
                if (numer > (denom * t0))
                    return false;

                if (numer > (denom * t1))
                    t1 = numer / denom;

                return true;
            }
            else
            {
                return numer <= 0.0f;
            }
        }

        #endregion

        #region Bounding Sphere queries

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRaySphere(ref Ray ray, ref Vector3 center, float radius)
        {
            Vector3 diff;
            Vector3.Subtract(ref ray.Origin, ref center, out diff);

            float a0;
            Vector3.Dot(ref diff, ref diff, out a0);

            a0 = a0 - (radius * radius);

            if (a0 <= 0.0f)
                return true; //origin is inside sphere

            //Else origin is outside sphere
            float a1;
            Vector3.Dot(ref ray.Direction, ref diff, out a1);
            if (a1 >= 0.0f)
                return false;

            //Quadratic has a real root if discriminant is nonnegative;
            return a1 * a1 >= a0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRaySphere(ref Ray ray, ref Vector3 center, float radius, out BoundingIntersectionResult result)
        {
            Vector3 diff;
            Vector3.Subtract(ref ray.Origin, ref center, out diff);

            float a0;
            Vector3.Dot(ref diff, ref diff, out a0);

            a0 = a0 - (radius * radius);

            float a1, discr, root;
            if (a0 <= 0.0f)
            {
                //Origin is inside the sphere
                Vector3.Dot(ref ray.Direction, ref diff, out a1);
                discr = (a1 * a1) - a0;
                root = (float)Math.Sqrt(discr);

                float t = -a1 + root;
                Vector3 pt;
                Vector3.Multiply(ref ray.Direction, t, out pt);
                Vector3.Add(ref pt, ref ray.Origin, out pt);

                result = new BoundingIntersectionResult(new LineIntersectionResult(pt, t));
                return true;
            }

            //else origin is outside the sphere
            Vector3.Dot(ref ray.Direction, ref diff, out a1);
            if (a1 >= 0.0f)
            {
                //No intersections at all
                result = new BoundingIntersectionResult();
                return false;
            }

            discr = (a1 * a1) - a0;
            if (discr < 0.0f)
            {
                //Two complex roots, no intersections
                result = new BoundingIntersectionResult();
                return false;
            }
            else if (discr >= MathHelper.ZeroTolerance)
            {
                //Two distinct real roots, two intersections
                root = (float)Math.Sqrt(discr);
                float t0 = -a1 - root;
                float t1 = -a1 + root;

                Vector3 pt0;
                Vector3.Multiply(ref ray.Direction, t0, out pt0);
                Vector3.Add(ref pt0, ref ray.Origin, out pt0);

                Vector3 pt1;
                Vector3.Multiply(ref ray.Direction, t1, out pt1);
                Vector3.Add(ref pt1, ref ray.Origin, out pt1);

                result = new BoundingIntersectionResult(new LineIntersectionResult(pt0, t0), new LineIntersectionResult(pt1, t1));
                return true;
            }
            else
            {
                //Ray intersects at exactly one point on sphere
                float t = -a1;
                Vector3 pt;
                Vector3.Multiply(ref ray.Direction, t, out pt);
                Vector3.Add(ref pt, ref ray.Origin, out pt);

                result = new BoundingIntersectionResult(new LineIntersectionResult(pt, t));
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentSphere(ref Segment segment, ref Vector3 center, float radius)
        {
            Vector3 segDir, segCenter;
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out segDir);

            //Calculate segment extent
            float segHalfLength = segDir.Length() * 0.5f;
            segDir.Normalize();

            //Calculate segment center
            Vector3.Multiply(ref segDir, segHalfLength, out segCenter);
            Vector3.Add(ref segment.StartPoint, ref segCenter, out segCenter);

            Vector3 diff;
            Vector3.Subtract(ref segCenter, ref center, out diff);

            float a0;
            Vector3.Dot(ref diff, ref diff, out a0);
            a0 = a0 - (radius * radius);

            float a1;
            Vector3.Dot(ref segDir, ref diff, out a1);

            float discr = (a1 * a1) - a0;
            if (discr < 0.0f)
                return false;

            float tmp0 = (segHalfLength * segHalfLength) + a0;
            float tmp1 = 2.0f * a1 * segHalfLength;
            float qm = tmp0 - tmp1;
            float qp = tmp0 + tmp1;

            if ((qm * qp) <= 0.0f)
                return true;

            return qm > 0.0f && Math.Abs(a1) < segHalfLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectTriangleSphere(ref Triangle triangle, ref Vector3 center, float radius)
        {
            Vector3 ptOnTriangle;
            triangle.ClosestPointTo(ref center, out ptOnTriangle);

            //Sphere and triangle intersect if the squared distance from the sphere center to point on triangle is less than the squared sphere radius
            Vector3 v;
            Vector3.Subtract(ref ptOnTriangle, ref center, out v);

            float dist;
            Vector3.Dot(ref v, ref v, out dist);

            return dist <= (radius * radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentSphere(ref Segment segment, ref Vector3 center, float radius, out BoundingIntersectionResult result)
        {
            Vector3 segDir, segCenter;
            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out segDir);

            //Calculate segment extent
            float segHalfLength = segDir.Length() * 0.5f;
            segDir.Normalize();

            //Calculate segment center
            Vector3.Multiply(ref segDir, segHalfLength, out segCenter);
            Vector3.Add(ref segment.StartPoint, ref segCenter, out segCenter);

            Vector3 diff;
            Vector3.Subtract(ref segCenter, ref center, out diff);

            float a0;
            Vector3.Dot(ref diff, ref diff, out a0);
            a0 = a0 - (radius * radius);

            float a1;
            Vector3.Dot(ref segDir, ref diff, out a1);

            float discr = (a1 * a1) - a0;
            if (discr < 0.0f)
            {
                //No intersections
                result = new BoundingIntersectionResult();
                return false;
            }

            float tmp0 = (segHalfLength * segHalfLength) + a0;
            float tmp1 = 2.0f * a1 * segHalfLength;
            float qm = tmp0 - tmp1;
            float qp = tmp0 + tmp1;

            if ((qm * qp) <= 0.0f)
            {
                //One intersection, segment begins
                float root = (float)Math.Sqrt(discr);

                float t = (qm > 0.0f) ? (-a1 - root) : (-a1 + root);

                Vector3 pt;
                Vector3.Multiply(ref segDir, t, out pt);
                Vector3.Add(ref pt, ref segCenter, out pt);

                result = new BoundingIntersectionResult(new LineIntersectionResult(pt, t + segHalfLength));
                return true;
            }

            if (qm > 0.0f && Math.Abs(a1) < segHalfLength)
            {
                if (discr >= MathHelper.ZeroTolerance)
                {
                    //Two distinct intersections
                    float root = (float)Math.Sqrt(discr);
                    float t0 = -a1 - root;
                    float t1 = -a1 + root;

                    Vector3 pt0;
                    Vector3.Multiply(ref segDir, t0, out pt0);
                    Vector3.Add(ref pt0, ref segCenter, out pt0);

                    Vector3 pt1;
                    Vector3.Multiply(ref segDir, t1, out pt1);
                    Vector3.Add(ref pt1, ref segCenter, out pt1);

                    result = new BoundingIntersectionResult(new LineIntersectionResult(pt0, t0 + segHalfLength), new LineIntersectionResult(pt1, t1 + segHalfLength));
                    return true;
                }
                else
                {
                    //One intersection
                    float t = -a1;

                    Vector3 pt;
                    Vector3.Multiply(ref segDir, t, out pt);
                    Vector3.Add(ref pt, ref segCenter, out pt);

                    result = new BoundingIntersectionResult(new LineIntersectionResult(pt, t + segHalfLength));
                    return true;
                }
            }
            else
            {
                //No intersection
                result = new BoundingIntersectionResult();
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSphereSphere(ref Vector3 firstCenter, float firstRadius, ref Vector3 secondCenter, float secondRadius)
        {
            Vector3 diff;
            Vector3.Subtract(ref secondCenter, ref firstCenter, out diff);

            float rSum = firstRadius + secondRadius;

            return diff.LengthSquared() <= (rSum * rSum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSphereAABB(ref Vector3 sphereCenter, float sphereRadius, ref Vector3 boxCenter, ref Vector3 boxExtents)
        {
            return Math.Abs(sphereCenter.X - boxCenter.X) <= (sphereRadius + boxExtents.X) &&
                Math.Abs(sphereCenter.Y - boxCenter.Y) <= (sphereRadius + boxExtents.Y) &&
                Math.Abs(sphereCenter.Z - boxCenter.Z) <= (sphereRadius + boxExtents.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSphereBox(ref Vector3 sphereCenter, float sphereRadius, ref Vector3 boxCenter, ref Vector3 boxXAxis, ref Vector3 boxYAxis, ref Vector3 boxZAxis, ref Vector3 boxExtents)
        {
            //Test for intersection in the coordinate system of the box by transforming the sphere into that coordinate system
            Vector3 diff;
            Vector3.Subtract(ref sphereCenter, ref boxCenter, out diff);

            float ax, ay, az;
            Vector3.Dot(ref diff, ref boxXAxis, out ax);
            Vector3.Dot(ref diff, ref boxYAxis, out ay);
            Vector3.Dot(ref diff, ref boxZAxis, out az);

            ax = Math.Abs(ax);
            ay = Math.Abs(ay);
            az = Math.Abs(az);

            float dx = ax - boxExtents.X;
            float dy = ay - boxExtents.Y;
            float dz = az - boxExtents.Z;

            if (ax <= boxExtents.X)
            {
                if (ay <= boxExtents.Y)
                {
                    if (az <= boxExtents.Z)
                    {
                        //Sphere center inside box
                        return true;
                    }
                    else
                    {
                        //otential sphere-face intersection with face z
                        return dz <= sphereRadius;
                    }
                }
                else
                {
                    if (az <= boxExtents.Z)
                    {
                        //Potential sphere-face intersection with face y
                        return dy <= sphereRadius;
                    }
                    else
                    {
                        //Potential sphere-edge intersection with edge formed by faces y and az
                        return ((dy * dy) + (dz * dz)) <= (sphereRadius * sphereRadius);
                    }
                }
            }
            else
            {
                if (ay <= boxExtents.Y)
                {
                    if (az <= boxExtents.Z)
                    {
                        //Potential sphere-face intersection with face x
                        return dx <= sphereRadius;
                    }
                    else
                    {
                        //Potential sphere-edge intersection with edge formed by faces x and z
                        return ((dx * dx) + (dz * dz)) <= (sphereRadius * sphereRadius);
                    }
                }
                else
                {
                    if (az <= boxExtents.Z)
                    {
                        //Potential sphere-edge intersection with edge formed by faces x and y
                        return ((dx * dx) + (dy * dy)) <= (sphereRadius * sphereRadius);
                    }
                    else
                    {
                        //Potential sphere-vertex intersection at corner formed by faces x y z
                        return ((dx * dx) + (dy * dy) + (dz * dz)) <= (sphereRadius * sphereRadius);
                    }
                }
            }
        }

        #endregion

        #region Bounding Capsule queries

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CapsuleFromBox(ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents, out Segment centerLine, out float radius)
        {
            Triad axes;
            Triad.FromAxes(ref xAxis, ref yAxis, ref zAxis, out axes);

            //Get max axis, which will be the capsule Z axis. Other two axes determine the radius. If the box is a cube then the
            //capsule degenerates into a sphere
            int maxAxis, uAxis, vAxis;
            GetMaxAxis(ref extents, out maxAxis, out uAxis, out vAxis);

            Vector3 uDir, vDir;
            axes.GetAxis(uAxis, out uDir);
            axes.GetAxis(vAxis, out vDir);

            Vector3.Multiply(ref uDir, extents[uAxis], out uDir);
            Vector3.Multiply(ref vDir, extents[vAxis], out vDir);

            Vector3 diagonal;
            Vector3.Add(ref uDir, ref vDir, out diagonal);

            radius = diagonal.Length();

            float segExtent = extents[maxAxis];
            Vector3 segDir;
            axes.GetAxis(maxAxis, out segDir);
            Segment.FromCenterExtent(ref center, ref segDir, segExtent, out centerLine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BoxFromCapsule(ref Segment centerLine, float radius, out Vector3 center, out Vector3 xAxis, out Vector3 yAxis, out Vector3 zAxis, out Vector3 extents)
        {
            Vector3 segCenter, segDir;
            float segExtent;
            GeometricToolsHelper.CalculateSegmentProperties(ref centerLine, out segCenter, out segDir, out segExtent);

            //Axis of capsule is the longest axis of the box
            center = segCenter;
            zAxis = segDir;
            Vector3.ComplementBasis(ref zAxis, out xAxis, out yAxis);

            extents = new Vector3(radius, radius, segExtent + radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayCapsule(ref Ray ray, ref Segment centerLine, float radius)
        {
            float distSquared, rayParam, segParam;
            DistanceRaySegment(ref ray, ref centerLine, out rayParam, out segParam, out distSquared);

            return distSquared <= (radius * radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectRayCapsule(ref Ray ray, ref Segment centerLine, float radius, out BoundingIntersectionResult result)
        {
            BoundingIntersectionResult tempResult;
            if (IntersectLineCapsule(ref ray.Origin, ref ray.Direction, ref centerLine, radius, out tempResult))
            {
                switch (tempResult.IntersectionCount)
                {
                    case 0:
                        break;
                    case 1:
                        {
                            LineIntersectionResult ir0 = tempResult.ClosestIntersection.Value;
                            if (ir0.Distance >= 0.0f)
                            {
                                result = new BoundingIntersectionResult(ir0);
                                return true;
                            }
                        }
                        break;
                    case 2:
                        {
                            LineIntersectionResult ir0 = tempResult.ClosestIntersection.Value;
                            LineIntersectionResult ir1 = tempResult.FarthestIntersection.Value;

                            bool ir0Valid = ir0.Distance >= 0.0f;
                            bool ir1Valid = ir1.Distance >= 0.0f;

                            if (ir0Valid && ir1Valid)
                            {
                                result = new BoundingIntersectionResult(ir0, ir1);
                                return true;
                            }
                            else if (ir0Valid)
                            {
                                result = new BoundingIntersectionResult(ir0);
                                return true;
                            }
                            else if (ir1Valid)
                            {
                                result = new BoundingIntersectionResult(ir1);
                                return true;
                            }
                        }
                        break;
                }
            }

            result = new BoundingIntersectionResult();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentCapsule(ref Segment segment, ref Segment centerLine, float radius)
        {
            float distSquared, segAParam, segBParam;
            DistanceSegmentSegment(ref segment, ref centerLine, out segAParam, out segBParam, out distSquared);

            return distSquared <= (radius * radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectSegmentCapsule(ref Segment segment, ref Segment centerLine, float radius, out BoundingIntersectionResult result)
        {
            Vector3 segCenter, segDir;
            float segExtent;
            CalculateSegmentProperties(ref segment, out segCenter, out segDir, out segExtent);

            BoundingIntersectionResult tempResult;
            if (IntersectLineCapsule(ref segCenter, ref segDir, ref centerLine, radius, out tempResult))
            {
                switch (tempResult.IntersectionCount)
                {
                    case 0:
                        break;
                    case 1:
                        {
                            LineIntersectionResult ir0 = tempResult.ClosestIntersection.Value;
                            if (Math.Abs(ir0.Distance) <= segExtent)
                            {
                                ir0 = new LineIntersectionResult(ir0.Point, ir0.Distance + segExtent);
                                result = new BoundingIntersectionResult(ir0);
                                return true;
                            }
                        }
                        break;
                    case 2:
                        {
                            LineIntersectionResult ir0 = tempResult.ClosestIntersection.Value;
                            LineIntersectionResult ir1 = tempResult.FarthestIntersection.Value;

                            bool ir0Valid = Math.Abs(ir0.Distance) <= segExtent;
                            bool ir1Valid = Math.Abs(ir1.Distance) <= segExtent;

                            if (ir0Valid && ir1Valid)
                            {
                                ir0 = new LineIntersectionResult(ir0.Point, ir0.Distance + segExtent);
                                ir1 = new LineIntersectionResult(ir1.Point, ir1.Distance + segExtent);
                                result = new BoundingIntersectionResult(ir0, ir1);
                                return true;
                            }
                            else if (ir0Valid)
                            {
                                ir0 = new LineIntersectionResult(ir0.Point, ir0.Distance + segExtent);
                                result = new BoundingIntersectionResult(ir0);
                                return true;
                            }
                            else if (ir1Valid)
                            {
                                ir1 = new LineIntersectionResult(ir1.Point, ir1.Distance + segExtent);
                                result = new BoundingIntersectionResult(ir1);
                                return true;
                            }
                        }
                        break;
                }
            }

            result = new BoundingIntersectionResult();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IntersectLineCapsule(ref Vector3 lineOrigin, ref Vector3 lineDir, ref Segment centerLine, float radius, out BoundingIntersectionResult result)
        {
            result = new BoundingIntersectionResult();

            Vector3 capsuleSegCenter, capsuleSegDir;
            float capsuleSegExtent;
            CalculateSegmentProperties(ref centerLine, out capsuleSegCenter, out capsuleSegDir, out capsuleSegExtent);

            //Coordinate system of capsule: Center of CenterLine segment is origin (C) and direction (axis of capsule, W) is the z-axis. U and V are the other axis directions
            Vector3 W = lineDir;
            Vector3 U, V;
            Vector3.ComplementBasis(ref W, out U, out V);

            float radiusSquared = radius * radius;

            //Convert incoming line origin to capsule coordinates
            Vector3 diff;
            Vector3.Subtract(ref lineOrigin, ref capsuleSegCenter, out diff);

            Vector3 P;
            Vector3.Dot(ref U, ref diff, out P.X);
            Vector3.Dot(ref V, ref diff, out P.Y);
            Vector3.Dot(ref W, ref diff, out P.Z);

            //Get the z-value, in capsule coordinates, of the incoming line's unit length direction
            float dz;
            Vector3.Dot(ref W, ref lineDir, out dz);
            if (Math.Abs(dz) >= (1.0f - MathHelper.ZeroTolerance))
            {
                //Line is parallel to the capsule axis, determine whether the line intersects the capsule hemispheres
                float radialSqrDist = radiusSquared - (P.X * P.X) - (P.Y * P.Y);
                if (radialSqrDist < 0.0f)
                {
                    //Line is outside cylinder of capsule, no intersection
                    return false;
                }

                //Line intersects the hemispherical caps
                float zOffset = (float)Math.Sqrt(radialSqrDist) + capsuleSegExtent;
                float t0, t1;
                if (dz > 0.0f)
                {
                    t0 = -P.Z - zOffset;
                    t1 = -P.Z + zOffset;
                }
                else
                {
                    t0 = P.Z - zOffset;
                    t1 = P.Z + zOffset;
                }

                Vector3 pt0;
                Vector3.Multiply(ref lineDir, t0, out pt0);
                Vector3.Add(ref pt0, ref lineOrigin, out pt0);

                Vector3 pt1;
                Vector3.Multiply(ref lineDir, t1, out pt1);
                Vector3.Add(ref pt1, ref lineOrigin, out pt1);

                result = new BoundingIntersectionResult(new LineIntersectionResult(pt0, t0), new LineIntersectionResult(pt1, t1));
                return true;
            }

            //Convert incoming line unit-length direction to capsule coordinates
            Vector3 D;
            Vector3.Dot(ref U, ref lineDir, out D.X);
            Vector3.Dot(ref V, ref lineDir, out D.Y);
            D.Z = dz;

            //Test intersection of line P + t*D with infinite cylinder x^2 + y^2 = r^2
            float a0 = (P.X * P.X) + (P.Y * P.Y) - radiusSquared;
            float a1 = (P.X * D.X) + (P.Y * D.Y);
            float a2 = (D.X * D.X) + (D.Y * D.Y);
            float discr = (a1 * a1) - (a0 * a2);

            if (discr < 0.0f)
            {
                //Line does not intersect infinite cylinder
                return false;
            }

            float root, inv, tValue, zValue;
            float* t = stackalloc float[2];
            int count = 0;

            if (discr > MathHelper.ZeroTolerance)
            {
                //Line intersects infinite cylinder in two places
                root = (float)Math.Sqrt(discr);
                inv = 1.0f / a2;
                tValue = (-a1 - root) * inv;
                zValue = P.X + (tValue * D.Z);
                if (Math.Abs(zValue) <= capsuleSegExtent)
                    t[count++] = tValue;

                tValue = (-a1 + root) * inv;
                zValue = P.Z + (tValue * D.Z);
                if (Math.Abs(zValue) <= capsuleSegExtent)
                    t[count++] = tValue;

                if (count == 2)
                {
                    //Line intersects capsule wall in two places
                    Vector3 pt0;
                    Vector3.Multiply(ref lineDir, t[0], out pt0);
                    Vector3.Add(ref pt0, ref lineOrigin, out pt0);

                    Vector3 pt1;
                    Vector3.Multiply(ref lineDir, t[1], out pt1);
                    Vector3.Add(ref pt1, ref lineOrigin, out pt1);

                    result = new BoundingIntersectionResult(new LineIntersectionResult(pt0, t[0]), new LineIntersectionResult(pt1, t[1]));
                    return true;
                }
            }
            else
            {
                //Line is tangent to infinite cylinder
                tValue = -a1 / a2;
                zValue = P.Z + (tValue * D.Z);
                if (Math.Abs(zValue) <= capsuleSegExtent)
                {
                    Vector3 pt;
                    Vector3.Multiply(ref lineDir, tValue, out pt);
                    Vector3.Add(ref pt, ref lineOrigin, out pt);

                    result = new BoundingIntersectionResult(new LineIntersectionResult(pt, tValue));
                    return true;
                }
            }

            //Test intersection with bottom hemisphere
            float PZpE = P.Z + capsuleSegExtent;
            a1 += PZpE * D.Z;
            a0 += PZpE * PZpE;
            discr = (a1 * a1) - a0;

            if (discr > MathHelper.ZeroTolerance)
            {
                root = (float)Math.Sqrt(discr);
                tValue = -a1 - root;
                zValue = P.Z + (tValue * D.Z);
                if (zValue <= -capsuleSegExtent)
                {
                    t[count++] = tValue;

                    if (CheckHitCountAndCalculate(t, count, 2, ref lineOrigin, ref lineDir, ref result))
                        return true;
                }

                tValue = -a1 + root;
                zValue = P.Z + (tValue * D.Z);
                if (zValue <= -capsuleSegExtent)
                {
                    t[count++] = tValue;

                    if (CheckHitCountAndCalculate(t, count, 2, ref lineOrigin, ref lineDir, ref result))
                        return true;
                }
            }
            else if (Math.Abs(discr) <= MathHelper.ZeroTolerance)
            {
                tValue = -a1;
                zValue = P.Z + (tValue * D.Z);
                if (zValue <= -capsuleSegExtent)
                {
                    t[count++] = tValue;

                    if (CheckHitCountAndCalculate(t, count, 2, ref lineOrigin, ref lineDir, ref result))
                        return true;
                }
            }

            //Test intersection with top hemisphere
            a1 -= 2.0f * capsuleSegExtent * D.Z;
            a0 -= 4.0f * capsuleSegExtent * P.Z;
            discr = (a1 * a1) - a0;

            if (discr > MathHelper.ZeroTolerance)
            {
                root = (float)Math.Sqrt(discr);
                tValue = -a1 - root;
                zValue = P.Z + (tValue * D.Z);
                if (zValue >= capsuleSegExtent)
                {
                    t[count++] = tValue;

                    if (CheckHitCountAndCalculate(t, count, 2, ref lineOrigin, ref lineDir, ref result))
                        return true;
                }

                tValue = -a1 + root;
                zValue = P.Z + (tValue * D.Z);
                if (zValue >= capsuleSegExtent)
                {
                    t[count++] = tValue;

                    if (CheckHitCountAndCalculate(t, count, 2, ref lineOrigin, ref lineDir, ref result))
                        return true;
                }
            }
            else if (Math.Abs(discr) <= MathHelper.ZeroTolerance)
            {
                tValue = -a1;
                zValue = P.Z + (tValue * D.Z);
                if (zValue >= capsuleSegExtent)
                {
                    t[count++] = tValue;

                    if (CheckHitCountAndCalculate(t, count, 2, ref lineOrigin, ref lineDir, ref result))
                        return true;
                }
            }

            if (count == 0)
                return false;

            return CheckHitCountAndCalculate(t, count, count, ref lineOrigin, ref lineDir, ref result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectCapsuleCapsule(ref Segment c0CenterLine, float c0Radius, ref Segment c1CenterLine, float c1Radius)
        {
            float rSum = c0Radius + c1Radius;
            float distSquared, seg0Param, seg1Param;
            DistanceSegmentSegment(ref c0CenterLine, ref c1CenterLine, out seg0Param, out seg1Param, out distSquared);

            return distSquared <= (rSum * rSum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectTriangleCapsule(ref Triangle triangle, ref Segment centerLine, float radius)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectCapsuleSphere(ref Segment centerLine, float radius, ref Vector3 sphereCenter, float sphereRadius)
        {
            float distSquared, segParameter;
            Vector3 ptOnCenterLine;
            DistancePointSegment(ref sphereCenter, ref centerLine, out ptOnCenterLine, out segParameter, out distSquared);

            float rSum = sphereRadius + radius;
            return distSquared <= (rSum * rSum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectCapsuleAABB(ref Segment centerLine, float radius, ref Vector3 boxCenter, ref Vector3 boxExtents)
        {
            float distSquared, segParameter;
            Vector3 ptOnCenterLine;
            DistancePointSegment(ref boxCenter, ref centerLine, out ptOnCenterLine, out segParameter, out distSquared);

            //Becomes Sphere-AABB test
            return Math.Abs(ptOnCenterLine.X - boxCenter.X) <= (radius + boxExtents.X) &&
                Math.Abs(ptOnCenterLine.Y - boxCenter.Y) <= (radius + boxExtents.Y) &&
                Math.Abs(ptOnCenterLine.Z - boxCenter.Z) <= (radius + boxExtents.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectCapsuleBox(ref Segment centerLine, float radius, ref Vector3 boxCenter, ref Vector3 boxXAxis, ref Vector3 boxYAxis, ref Vector3 boxZAxis, ref Vector3 boxExtents)
        {
            float distSquared, segParameter;
            Vector3 ptOnCenterLine;
            DistancePointSegment(ref boxCenter, ref centerLine, out ptOnCenterLine, out segParameter, out distSquared);

            //Becomes Sphere-Box test
            return IntersectSphereBox(ref ptOnCenterLine, radius, ref boxCenter, ref boxXAxis, ref boxYAxis, ref boxZAxis, ref boxExtents);
        }

        #endregion

        #region Bounding Containment queries

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType BoxContainsBox(ref Vector3 b0Center, ref Vector3 b0XAxis, ref Vector3 b0YAxis, ref Vector3 b0ZAxis, ref Vector3 b0Extents,
            ref Vector3 b1Center, ref Vector3 b1XAxis, ref Vector3 b1YAxis, ref Vector3 b1ZAxis, ref Vector3 b1Extents)
        {
            //If not intersects then outside
            if (!IntersectBoxBox(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref b1Center, ref b1XAxis, ref b1YAxis, ref b1ZAxis, ref b1Extents))
                return ContainmentType.Outside;

            Vector3 exAxis;
            Vector3 eyAxis;
            Vector3 ezAxis;

            Vector3.Multiply(ref b1XAxis, b1Extents.X, out exAxis);
            Vector3.Multiply(ref b1YAxis, b1Extents.Y, out eyAxis);
            Vector3.Multiply(ref b1ZAxis, b1Extents.Z, out ezAxis);

            Vector3 temp;

            //Check if all corners are contained in the box, if one is not then it must be intersecting

            Vector3.Subtract(ref b1Center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            if (!BoxContainsPoint(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref temp))
                return ContainmentType.Intersects;

            Vector3.Add(ref b1Center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            if (!BoxContainsPoint(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref temp))
                return ContainmentType.Intersects;

            Vector3.Add(ref b1Center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            if (!BoxContainsPoint(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref temp))
                return ContainmentType.Intersects;

            Vector3.Subtract(ref b1Center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            if (!BoxContainsPoint(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref temp))
                return ContainmentType.Intersects;

            Vector3.Add(ref b1Center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            if (!BoxContainsPoint(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref temp))
                return ContainmentType.Intersects;

            Vector3.Subtract(ref b1Center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            if (!BoxContainsPoint(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref temp))
                return ContainmentType.Intersects;

            Vector3.Add(ref b1Center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            if (!BoxContainsPoint(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref temp))
                return ContainmentType.Intersects;

            Vector3.Subtract(ref b1Center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            if (!BoxContainsPoint(ref b0Center, ref b0XAxis, ref b0YAxis, ref b0ZAxis, ref b0Extents, ref temp))
                return ContainmentType.Intersects;

            //Otherwise box is fully contained
            return ContainmentType.Inside;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BoxContainsPoint(ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents, ref Vector3 point)
        {
            Vector3 diff;
            Vector3.Subtract(ref point, ref center, out diff);

            float xCoeff, yCoeff, zCoeff;

            Vector3.Dot(ref diff, ref xAxis, out xCoeff);
            if (Math.Abs(xCoeff) > extents.X)
                return false;

            Vector3.Dot(ref diff, ref yAxis, out yCoeff);
            if (Math.Abs(yCoeff) > extents.Y)
                return false;

            Vector3.Dot(ref diff, ref zAxis, out zCoeff);
            if (Math.Abs(zCoeff) > extents.Z)
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType AABBContainsCapsule(ref Vector3 boxCenter, ref Vector3 boxExtents, ref Segment centerLine, float radius)
        {
            //If not intersecting then know its outside
            if (!IntersectCapsuleAABB(ref centerLine, radius, ref boxCenter, ref boxExtents))
                return ContainmentType.Outside;

            //If both capsule ends are inside, then the entire capsule is contained
            if (AABBContainsSphere(ref boxCenter, ref boxExtents, ref centerLine.StartPoint, radius) == ContainmentType.Inside &&
                AABBContainsSphere(ref boxCenter, ref boxExtents, ref centerLine.EndPoint, radius) == ContainmentType.Inside)
                return ContainmentType.Inside;

            //Otherwise intersects
            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType CapsuleContainsAABB(ref Segment centerLine, float radius, ref Vector3 boxCenter, ref Vector3 boxExtents)
        {
            float distSquared, segParameter;
            Vector3 ptOnCenterLine;
            DistancePointSegment(ref boxCenter, ref centerLine, out ptOnCenterLine, out segParameter, out distSquared);

            //Becomes Sphere-AABB contains
            return SphereContainsAABB(ref ptOnCenterLine, radius, ref boxCenter, ref boxExtents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType CapsuleContainsBox(ref Segment centerLine, float radius, ref Vector3 boxCenter, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 boxExtents)
        {
            float distSquared, segParameter;
            Vector3 ptOnCenterLine;
            DistancePointSegment(ref boxCenter, ref centerLine, out ptOnCenterLine, out segParameter, out distSquared);

            //Becomes Sphere-Box contains
            return SphereContainsBox(ref ptOnCenterLine, radius, ref boxCenter, ref xAxis, ref yAxis, ref zAxis, ref boxExtents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType BoxContainsCapsule(ref Vector3 boxCenter, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 boxExtents, ref Segment centerLine, float radius)
        {
            //If not intersecting then know its outside
            if (!IntersectCapsuleBox(ref centerLine, radius, ref boxCenter, ref xAxis, ref yAxis, ref zAxis, ref boxExtents))
                return ContainmentType.Outside;

            //If both capsule ends are inside, then the entire capsule is contained
            if (BoxContainsSphere(ref boxCenter, ref xAxis, ref yAxis, ref zAxis, ref boxExtents, ref centerLine.StartPoint, radius) == ContainmentType.Inside &&
                BoxContainsSphere(ref boxCenter, ref xAxis, ref yAxis, ref zAxis, ref boxExtents, ref centerLine.EndPoint, radius) == ContainmentType.Inside)
                return ContainmentType.Inside;

            //Otherwise intersects
            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType SphereContainsCapsule(ref Vector3 sphereCenter, float sphereRadius, ref Segment centerLine, float capsuleRadius)
        {
            //If not intersecting then know its outside
            if (!IntersectCapsuleSphere(ref centerLine, capsuleRadius, ref sphereCenter, sphereRadius))
                return ContainmentType.Outside;

            //If both capsule ends are inside, then the entire capsule is contained
            if (SphereContainsSphere(ref sphereCenter, sphereRadius, ref centerLine.StartPoint, capsuleRadius) == ContainmentType.Inside &&
                SphereContainsSphere(ref sphereCenter, sphereRadius, ref centerLine.EndPoint, capsuleRadius) == ContainmentType.Inside)
                return ContainmentType.Inside;

            //Otherwise intersects
            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType CapsuleContainsSphere(ref Segment centerLine, float capsuleRadius, ref Vector3 sphereCenter, float sphereRadius)
        {
            float distSquared, segParameter;
            Vector3 ptOnCenterLine;
            DistancePointSegment(ref sphereCenter, ref centerLine, out ptOnCenterLine, out segParameter, out distSquared);

            //Becomes Sphere-Sphere contains
            return SphereContainsSphere(ref ptOnCenterLine, capsuleRadius, ref sphereCenter, sphereRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType CapsuleContainsCapsule(ref Segment c0CenterLine, float c0Radius, ref Segment c1CenterLine, float c1Radius)
        {
            float rSum = c0Radius + c1Radius;
            float distSquared, seg0Param, seg1Param;
            DistanceSegmentSegment(ref c0CenterLine, ref c1CenterLine, out seg0Param, out seg1Param, out distSquared);

            //Test if outside, IntersectCapsuleCapsule returns true if distSquared <= (rsum * rsum)
            if (distSquared > (rSum * rSum))
                return ContainmentType.Outside;

            Vector3 ptOnC0;
            c0CenterLine.PointAtDistance(seg0Param, out ptOnC0);

            //Becomes Sphere-Capsule contains, if both capsule ends are inside of sphere centered at ptOnC0 then it is contained
            if (SphereContainsSphere(ref ptOnC0, c0Radius, ref c1CenterLine.StartPoint, c1Radius) == ContainmentType.Inside &&
                SphereContainsSphere(ref ptOnC0, c0Radius, ref c1CenterLine.EndPoint, c1Radius) == ContainmentType.Inside)
                return ContainmentType.Inside;

            //Otherwise intersects
            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType BoxContainsSphere(ref Vector3 boxCenter, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents, ref Vector3 sphereCenter, float sphereRadius)
        {
            Vector3 locSphereCenter;

            //Transform sphere center into the OBB coordinates (axes are Unit, center is origin), effectively making this a Sphere-AABB test
            Matrix4x4.FromAxes(ref xAxis, ref yAxis, ref zAxis, out Matrix4x4 rot);
            rot.Invert();

            Vector3.Transform(ref sphereCenter, ref rot, out locSphereCenter);

            //Sphere-AABB test
            Vector3 max = extents;
            Vector3 min;
            Vector3.Negate(ref extents, out min);

            Vector3 temp;
            float dist;
            Vector3.Clamp(ref locSphereCenter, ref min, ref max, out temp);
            Vector3.Distance(ref locSphereCenter, ref temp, out dist);

            //Check if outside
            if (dist > (sphereRadius * sphereRadius))
                return ContainmentType.Outside;

            //Or contained
            if (((min.X + sphereRadius) <= locSphereCenter.X) && (locSphereCenter.X <= (max.X - sphereRadius)) && ((max.X - min.X) > sphereRadius) &&
                ((min.Y + sphereRadius) <= locSphereCenter.Y) && (locSphereCenter.Y <= (max.Y - sphereRadius)) && ((max.Y - min.Y) > sphereRadius) &&
                ((min.Z + sphereRadius) <= locSphereCenter.Z) && (locSphereCenter.Z <= (max.Z - sphereRadius)) && ((max.X - min.X) > sphereRadius))
            {
                return ContainmentType.Inside;
            }

            //Otherwise we overlap
            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType SphereContainsBox(ref Vector3 sphereCenter, float sphereRadius, ref Vector3 boxCenter, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents)
        {
            Vector3 locSphereCenter;

            //Transform sphere center into the OBB coordinates (axes are Unit, center is origin), effectively making this a Sphere-AABB test
            Matrix4x4.FromAxes(ref xAxis, ref yAxis, ref zAxis, out Matrix4x4 rot);
            rot.Invert();

            Vector3.Transform(ref sphereCenter, ref rot, out locSphereCenter);

            //Sphere-AABB test
            Vector3 origin = Vector3.Zero;
            return SphereContainsAABB(ref locSphereCenter, sphereRadius, ref origin, ref extents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType AABBContainsAABB(ref Vector3 b0Center, ref Vector3 b0Extents, ref Vector3 b1Center, ref Vector3 b1Extents)
        {
            Vector3 max0 = new Vector3(b0Center.X + b0Extents.X, b0Center.Y + b0Extents.Y, b0Center.Z + b0Extents.Z);
            Vector3 min0 = new Vector3(b0Center.X - b0Extents.X, b0Center.Y - b0Extents.Y, b0Center.Z - b0Extents.Z);

            Vector3 max1 = new Vector3(b1Center.X + b1Extents.X, b1Center.Y + b1Extents.Y, b1Center.Z + b1Extents.Z);
            Vector3 min1 = new Vector3(b1Center.X - b1Extents.X, b1Center.Y - b1Extents.Y, b1Center.Z - b1Extents.Z);

            //Check if outside
            if (max0.X < min1.X || min0.X > max1.X)
                return ContainmentType.Outside;

            if (max0.Y < min1.Y || min0.Y > max1.Y)
                return ContainmentType.Outside;

            if (max0.Z < min1.Z || min0.Z > max1.Z)
                return ContainmentType.Outside;

            //Or if contained
            if ((min0.X <= min1.X && max1.X <= max0.X) &&
                (min0.Y <= min1.Y && max1.Y <= max0.Y) &&
                (min0.Z <= min1.Z && max1.Z <= max0.Z))
            {
                return ContainmentType.Inside;
            }

            //Otherwise must intersect
            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType SphereContainsSphere(ref Vector3 s0Center, float s0Radius, ref Vector3 s1Center, float s1Radius)
        {
            float dist;
            Vector3.Distance(ref s0Center, ref s1Center, out dist);

            //Check if outside
            if ((s0Radius + s1Radius) < dist)
                return ContainmentType.Outside;

            //Or intersects
            if ((s0Radius - s1Radius) < dist)
                return ContainmentType.Intersects;

            //Otherwise must be inside
            return ContainmentType.Inside;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType AABBContainsSphere(ref Vector3 boxCenter, ref Vector3 boxExtents, ref Vector3 sphereCenter, float sphereRadius)
        {
            Vector3 max = new Vector3(boxCenter.X + boxExtents.X, boxCenter.Y + boxExtents.Y, boxCenter.Z + boxExtents.Z);
            Vector3 min = new Vector3(boxCenter.X - boxExtents.X, boxCenter.Y - boxExtents.Y, boxCenter.Z - boxExtents.Z);

            Vector3 temp;
            float dist;
            Vector3.Clamp(ref sphereCenter, ref min, ref max, out temp);
            Vector3.DistanceSquared(ref sphereCenter, ref temp, out dist);

            //Check if outside
            if (dist > (sphereRadius * sphereRadius))
                return ContainmentType.Outside;

            //Or contained
            if (((min.X + sphereRadius) <= sphereCenter.X) && (sphereCenter.X <= (max.X - sphereRadius)) && ((max.X - min.X) > sphereRadius) &&
                ((min.Y + sphereRadius) <= sphereCenter.Y) && (sphereCenter.Y <= (max.Y - sphereRadius)) && ((max.Y - min.Y) > sphereRadius) &&
                ((min.Z + sphereRadius) <= sphereCenter.Z) && (sphereCenter.Z <= (max.Z - sphereRadius)) && ((max.X - min.X) > sphereRadius))
            {
                return ContainmentType.Inside;
            }

            //Otherwise we overlap
            return ContainmentType.Intersects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType SphereContainsAABB(ref Vector3 sphereCenter, float sphereRadius, ref Vector3 boxCenter, ref Vector3 boxExtents)
        {
            //Check if we do intersect
            if (!IntersectSphereAABB(ref sphereCenter, sphereRadius, ref boxCenter, ref boxExtents))
                return ContainmentType.Outside;

            Vector3 max = new Vector3(boxCenter.X + boxExtents.X, boxCenter.Y + boxExtents.Y, boxCenter.Z + boxExtents.Z);
            Vector3 min = new Vector3(boxCenter.X - boxExtents.X, boxCenter.Y - boxExtents.Y, boxCenter.Z - boxExtents.Z);

            float sqrRadius = sphereRadius * sphereRadius;
            Vector3 diff;

            //If any of the AABB corners are outside sphere, it intersects
            diff.X = sphereCenter.X - min.X;
            diff.Y = sphereCenter.Y - max.Y;
            diff.Z = sphereCenter.Z - max.Z;
            if (diff.LengthSquared() > sqrRadius)
                return ContainmentType.Intersects;

            diff.X = sphereCenter.X - max.X;
            diff.Y = sphereCenter.Y - max.Y;
            diff.Z = sphereCenter.Z - max.Z;
            if (diff.LengthSquared() > sqrRadius)
                return ContainmentType.Intersects;

            diff.X = sphereCenter.X - max.X;
            diff.Y = sphereCenter.Y - min.Y;
            diff.Z = sphereCenter.Z - max.Z;
            if (diff.LengthSquared() > sqrRadius)
                return ContainmentType.Intersects;

            diff.X = sphereCenter.X - min.X;
            diff.Y = sphereCenter.Y - min.Y;
            diff.Z = sphereCenter.Z - max.Z;
            if (diff.LengthSquared() > sqrRadius)
                return ContainmentType.Intersects;

            diff.X = sphereCenter.X - min.X;
            diff.Y = sphereCenter.Y - max.Y;
            diff.Z = sphereCenter.Z - min.Z;
            if (diff.LengthSquared() > sqrRadius)
                return ContainmentType.Intersects;

            diff.X = sphereCenter.X - max.X;
            diff.Y = sphereCenter.Y - max.Y;
            diff.Z = sphereCenter.Z - min.Z;
            if (diff.LengthSquared() > sqrRadius)
                return ContainmentType.Intersects;

            diff.X = sphereCenter.X - max.X;
            diff.Y = sphereCenter.Y - min.Y;
            diff.Z = sphereCenter.Z - min.Z;
            if (diff.LengthSquared() > sqrRadius)
                return ContainmentType.Intersects;

            diff.X = sphereCenter.X - min.X;
            diff.Y = sphereCenter.Y - min.Y;
            diff.Z = sphereCenter.Z - min.Z;
            if (diff.LengthSquared() > sqrRadius)
                return ContainmentType.Intersects;

            //Otherwise AABB is fully contained
            return ContainmentType.Inside;
        }

        #endregion

        #region Bounding Frustum queries

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ContainmentType FrustumContainsAABB(BoundingFrustum frustum, ref Vector3 center, ref Vector3 extents)
        {
            Vector3 max = new Vector3(center.X + extents.X, center.Y + extents.Y, center.Z + extents.Z);
            Vector3 min = new Vector3(center.X - extents.X, center.Y - extents.Y, center.Z - extents.Z);

            ContainmentType result = ContainmentType.Inside;

            for (int i = 0; i < frustum.PlaneCount; i++)
            {
                Plane plane;
                frustum.GetPlane((FrustumPlane)i, out plane);

                Vector3 p = min;
                Vector3 n = max;

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

                if (plane.WhichSide(ref p) == PlaneIntersectionType.Back)
                    return ContainmentType.Outside;

                if (plane.WhichSide(ref n) == PlaneIntersectionType.Back)
                    result = ContainmentType.Intersects;
            }

            return result;
        }

        #endregion

        #region Merge

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MergeAABBABB(ref BoundingBox.Data b0, ref BoundingBox.Data b1, out BoundingBox.Data result)
        {
            //Calculate min-max of both boxes
            Vector3 b0Max = new Vector3(b0.Center.X + b0.Extents.X, b0.Center.Y + b0.Extents.Y, b0.Center.Z + b0.Extents.Z);
            Vector3 b0Min = new Vector3(b0.Center.X - b0.Extents.X, b0.Center.Y - b0.Extents.Y, b0.Center.Z - b0.Extents.Z);

            Vector3 b1Max = new Vector3(b1.Center.X + b1.Extents.X, b1.Center.Y + b1.Extents.Y, b1.Center.Z + b1.Extents.Z);
            Vector3 b1Min = new Vector3(b1.Center.X - b1.Extents.X, b1.Center.Y - b1.Extents.Y, b1.Center.Z - b1.Extents.Z);

            //Find absolute min and max
            Vector3.Min(ref b1Min, ref b0Min, out b0Min);
            Vector3.Max(ref b1Max, ref b0Max, out b0Max);

            //Compute center from min-max
            Vector3.Add(ref b0Max, ref b0Min, out result.Center);
            Vector3.Multiply(ref result.Center, 0.5f, out result.Center);

            //Compute extents from min-max
            Vector3.Subtract(ref b0Max, ref result.Center, out result.Extents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MergeAABBBox(ref BoundingBox.Data b0, ref OrientedBoundingBox.Data b1, out BoundingBox.Data result)
        {
            Vector3 max = new Vector3(b0.Center.X + b0.Extents.X, b0.Center.Y + b0.Extents.Y, b0.Center.Z + b0.Extents.Z);
            Vector3 min = new Vector3(b0.Center.X - b0.Extents.X, b0.Center.Y - b0.Extents.Y, b0.Center.Z - b0.Extents.Z);

            Vector3 center = b1.Center;
            Vector3 extents = b1.Extents;
            Triad axes = b1.Axes;

            Vector3 exAxis;
            Vector3 eyAxis;
            Vector3 ezAxis;

            Vector3.Multiply(ref axes.XAxis, extents.X, out exAxis);
            Vector3.Multiply(ref axes.YAxis, extents.Y, out eyAxis);
            Vector3.Multiply(ref axes.ZAxis, extents.Z, out ezAxis);

            Vector3 temp;

            Vector3.Subtract(ref center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            Vector3.Min(ref min, ref temp, out min);
            Vector3.Max(ref max, ref temp, out max);

            Vector3.Add(ref center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            Vector3.Min(ref min, ref temp, out min);
            Vector3.Max(ref max, ref temp, out max);

            Vector3.Add(ref center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            Vector3.Min(ref min, ref temp, out min);
            Vector3.Max(ref max, ref temp, out max);

            Vector3.Subtract(ref center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Subtract(ref temp, ref ezAxis, out temp);
            Vector3.Min(ref min, ref temp, out min);
            Vector3.Max(ref max, ref temp, out max);

            Vector3.Add(ref center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            Vector3.Min(ref min, ref temp, out min);
            Vector3.Max(ref max, ref temp, out max);

            Vector3.Subtract(ref center, ref exAxis, out temp);
            Vector3.Subtract(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            Vector3.Min(ref min, ref temp, out min);
            Vector3.Max(ref max, ref temp, out max);

            Vector3.Add(ref center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            Vector3.Min(ref min, ref temp, out min);
            Vector3.Max(ref max, ref temp, out max);

            Vector3.Subtract(ref center, ref exAxis, out temp);
            Vector3.Add(ref temp, ref eyAxis, out temp);
            Vector3.Add(ref temp, ref ezAxis, out temp);
            Vector3.Min(ref min, ref temp, out min);
            Vector3.Max(ref max, ref temp, out max);

            //Compute center from min-max
            Vector3.Add(ref max, ref min, out result.Center);
            Vector3.Multiply(ref result.Center, 0.5f, out result.Center);

            //Compute extents from min-max
            Vector3.Subtract(ref max, ref result.Center, out result.Extents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MergeSphereBox(ref BoundingSphere.Data sphere, ref OrientedBoundingBox.Data box, out BoundingSphere.Data result)
        {
            Vector3* pts = stackalloc Vector3[8];
            OrientedBoundingBox.Data.ComputeCorners(ref box, pts);

            BoundingSphere.Data s1;
            BoundingSphere.Data.ComputeMinSphere(pts, 8, null, 0, out s1);

            MergeSphereSphere(ref sphere, ref s1, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MergeSphereSphere(ref BoundingSphere.Data s0, ref BoundingSphere.Data s1, out BoundingSphere.Data result)
        {
            Vector3 centerDiff;
            Vector3.Subtract(ref s1.Center, ref s0.Center, out centerDiff);

            float lengthSqr;
            Vector3.Dot(ref centerDiff, ref centerDiff, out lengthSqr);

            float rDiff = s1.Radius - s0.Radius;
            float rDiffSqr = rDiff * rDiff;

            //One of the spheres is already contained, return the largest
            if (rDiffSqr >= lengthSqr)
            {
                if (rDiff >= 0.0f)
                {
                    result = s1;
                }
                else
                {
                    result = s0;
                }
            }
            else
            {
                float length = (float)Math.Sqrt(lengthSqr);

                //Calculate new center
                if (length > 0.0f)
                {
                    float coeff = (length + rDiff) / (2.0f * length);
                    Vector3.Multiply(ref centerDiff, coeff, out result.Center);
                    Vector3.Add(ref result.Center, ref s0.Center, out result.Center);
                }
                else
                {
                    result.Center = s0.Center;
                }

                //Calculate new radius
                result.Radius = 0.5f * (length + s0.Radius + s1.Radius);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MergeBoxBoxUsingCorners(ref OrientedBoundingBox.Data b0, ref OrientedBoundingBox.Data b1, out OrientedBoundingBox.Data result)
        {
            //Something's up with my implementation of MergeBoxBox or GaussPointsFit, so for now just recompute an OBB from the two sets of corners
            Vector3* vertices = stackalloc Vector3[16];
            OrientedBoundingBox.Data.ComputeCorners(ref b0, &vertices[0]);
            OrientedBoundingBox.Data.ComputeCorners(ref b1, &vertices[8]);

            OrientedBoundingBox.Data.FromPoints(vertices, 16, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MergeBoxBox(ref OrientedBoundingBox.Data b0, ref OrientedBoundingBox.Data b1, out OrientedBoundingBox.Data result)
        {
            result.Extents = Vector3.Zero;

            //First guess at box center. Later will be updated when input box vertices are projected onto axes determined by the average of the axes
            Vector3.Add(ref b0.Center, ref b1.Center, out result.Center);
            Vector3.Multiply(ref result.Center, 0.5f, out result.Center);

            //Box axes, when viewed as the rows of a matrix, form a rotation matrix. Transform to quarternions and compute the average and normalize.
            //The result is the slerp of the two quaternions with a t-value of .5. Result is converted back to a rotation matrix and rows are taken as 
            //the merged box's axes
            Quaternion q0, q1;
            Quaternion.FromAxes(ref b0.Axes.XAxis, ref b0.Axes.YAxis, ref b0.Axes.ZAxis, out q0);
            Quaternion.FromAxes(ref b1.Axes.XAxis, ref b1.Axes.YAxis, ref b1.Axes.ZAxis, out q1);

            float dot;
            Quaternion.Dot(ref q0, ref q1, out dot);

            if (dot < 0.0f)
                q1.Negate();

            Quaternion avg;
            Quaternion.Add(ref q0, ref q1, out avg);
            avg.Normalize();

            //Get axes of the averaged rotation
            Quaternion.ToAxes(ref avg, out result.Axes.XAxis, out result.Axes.YAxis, out result.Axes.ZAxis);

            //Project input box vertices onto the merged box axes. Each axis containing the current center has a minimum projected value
            //and a maximum project value. The actual box center will be adjusted to be the midpoint of each interval.
            Vector3* vertices = stackalloc Vector3[8];
            Vector3 pMin = Vector3.Zero;
            Vector3 pMax = Vector3.Zero;

            OrientedBoundingBox.Data.ComputeCorners(ref b0, vertices);
            for (int i = 0; i < 8; i++)
            {
                Vector3 diff;
                Vector3.Subtract(ref vertices[i], ref result.Center, out diff);
                for (int j = 0; j < 3; j++)
                {
                    Vector3 axis;
                    result.Axes.GetAxis(j, out axis);
                    Vector3.Dot(ref diff, ref axis, out dot);

                    if (dot > pMax[j])
                    {
                        pMax[j] = dot;
                    }
                    else if (dot < pMin[j])
                    {
                        pMin[j] = dot;
                    }
                }
            }

            OrientedBoundingBox.Data.ComputeCorners(ref b1, vertices);
            for (int i = 0; i < 8; i++)
            {
                Vector3 diff;
                Vector3.Subtract(ref vertices[i], ref result.Center, out diff);
                for (int j = 0; j < 3; j++)
                {
                    Vector3 axis;
                    result.Axes.GetAxis(j, out axis);
                    Vector3.Dot(ref diff, ref axis, out dot);
                    if (dot > pMax[j])
                    {
                        pMax[j] = dot;
                    }
                    else if (dot < pMin[j])
                    {
                        pMin[j] = dot;
                    }
                }
            }

            //[Min, Max] is the axis aligned box in the coordinate system of the merged box axes. Update center and extents.
            for (int j = 0; j < 3; j++)
            {
                Vector3 temp;
                Vector3 axis;
                result.Axes.GetAxis(j, out axis);
                Vector3.Multiply(ref axis, (pMax[j] + pMin[j]) * 0.5f, out temp);
                Vector3.Add(ref temp, ref result.Center, out result.Center);

                result.Extents[j] = 0.5f * (pMax[j] - pMin[j]);
            }
        }

        #endregion

        #region Utility

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetProjectionBox(ref Vector3 axis, ref Vector3 center, ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, ref Vector3 extents, out float min, out float max)
        {
            float origin;
            Vector3.Dot(ref axis, ref center, out origin);

            float AdU0, AdU1, AdU2;
            Vector3.Dot(ref axis, ref xAxis, out AdU0);
            Vector3.Dot(ref axis, ref yAxis, out AdU1);
            Vector3.Dot(ref axis, ref zAxis, out AdU2);

            float maxExtent = Math.Abs(extents.X * AdU0) + Math.Abs(extents.Y * AdU1) + Math.Abs(extents.Z * AdU2);

            min = origin - maxExtent;
            max = origin + maxExtent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetProjectionTriangle(ref Vector3 axis, ref Triangle triangle, out float min, out float max)
        {
            float AdA, AdB, AdC;
            Vector3.Dot(ref axis, ref triangle.PointA, out AdA);
            Vector3.Dot(ref axis, ref triangle.PointB, out AdB);
            Vector3.Dot(ref axis, ref triangle.PointC, out AdC);

            min = AdA;
            max = min;

            if (AdB < min)
            {
                min = AdB;
            }
            else if (AdB > max)
            {
                max = AdB;
            }

            if (AdC < min)
            {
                min = AdC;
            }
            else if (AdC > max)
            {
                max = AdC;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CalculateSegmentProperties(ref Segment segment, out Vector3 segCenter, out Vector3 segDir, out float segExtent)
        {
            if (segment.IsDegenerate)
            {
                segCenter = segment.StartPoint;
                segDir = Vector3.UnitZ;
                segExtent = 0.0f;
                return;
            }

            Vector3.Subtract(ref segment.EndPoint, ref segment.StartPoint, out segDir);

            //Calculate segment extent
            segExtent = segDir.Length() * 0.5f;
            segDir.Normalize();

            //Calculate segment center
            Vector3.Multiply(ref segDir, segExtent, out segCenter);
            Vector3.Add(ref segment.StartPoint, ref segCenter, out segCenter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool CheckHitCountAndCalculate(float* t, int count, int maxCount, ref Vector3 lineOrigin, ref Vector3 lineDir, ref BoundingIntersectionResult result)
        {
            switch (maxCount)
            {
                case 0:
                    return true;
                case 1:
                    {
                        if (count == 1)
                        {
                            Vector3 pt;
                            Vector3.Multiply(ref lineDir, t[0], out pt);
                            Vector3.Add(ref pt, ref lineOrigin, out pt);

                            result = new BoundingIntersectionResult(new LineIntersectionResult(pt, t[0]));
                            return true;
                        }
                    }
                    break;
                case 2:
                    {
                        if (count == 2)
                        {
                            Vector3 pt0;
                            Vector3.Multiply(ref lineDir, t[0], out pt0);
                            Vector3.Add(ref pt0, ref lineOrigin, out pt0);

                            Vector3 pt1;
                            Vector3.Multiply(ref lineDir, t[1], out pt1);
                            Vector3.Add(ref pt1, ref lineOrigin, out pt1);

                            result = new BoundingIntersectionResult(new LineIntersectionResult(pt0, t[0]), new LineIntersectionResult(pt1, t[1]));
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetMaxAxis(ref Vector3 v, out int maxAxis, out int uAxis, out int vAxis)
        {
            float x = Math.Abs(v.X);
            float y = Math.Abs(v.Y);
            float z = Math.Abs(v.Z);

            if (x >= y)
            {
                if (x >= z)
                {
                    maxAxis = 0;
                    uAxis = 1;
                    vAxis = 2;
                    return;
                }

                maxAxis = 2;
                uAxis = 0;
                vAxis = 1;
                return;
            }

            if (y >= z)
            {
                maxAxis = 1;
                uAxis = 0;
                vAxis = 2;
                return;
            }

            maxAxis = 2;
            uAxis = 0;
            vAxis = 1;
        }

        #endregion

        #region Approximation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GaussPointsFit(IReadOnlyDataBuffer<Vector3> points, out OrientedBoundingBox.Data result)
        {
            return GaussPointsFit(points, 0, (points != null) ? points.Length : 0, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GaussPointsFit(IReadOnlyDataBuffer<Vector3> points, int offset, int count, out OrientedBoundingBox.Data result)
        {
            result.Center = Vector3.Zero;
            result.Axes = Triad.UnitAxes;
            result.Extents = Vector3.Zero;

            if (points == null || count < 2 || offset < 0 || ((offset + count) > points.Length))
                return false;

            //Compute the mean of the points
            result.Center = points[offset];

            int upperBoundExclusive = offset + count;
            for (int i = offset + 1; i < upperBoundExclusive; i++)
            {
                Vector3 pt = points[i];
                Vector3.Add(ref result.Center, ref pt, out result.Center);
            }

            float invNumPoints = 1.0f / (float)count;
            Vector3.Multiply(ref result.Center, invNumPoints, out result.Center);

            //Compute the covariance matrix of the points
            float sumXX = 0.0f, sumXY = 0.0f, sumXZ = 0.0f;
            float sumYY = 0.0f, sumYZ = 0.0f, sumZZ = 0.0f;

            for (int i = offset; i < upperBoundExclusive; i++)
            {
                Vector3 diff;
                Vector3 pt = points[i];
                Vector3.Subtract(ref pt, ref result.Center, out diff);

                sumXX += diff.X * diff.X;
                sumXY += diff.X * diff.Y;
                sumXZ += diff.X * diff.Z;
                sumYY += diff.Y * diff.Y;
                sumYZ += diff.Y * diff.Z;
                sumZZ += diff.Z * diff.Z;
            }

            sumXX *= invNumPoints;
            sumXY *= invNumPoints;
            sumXZ *= invNumPoints;
            sumYY *= invNumPoints;
            sumYZ *= invNumPoints;
            sumZZ *= invNumPoints;

            Matrix3x3 covarianceMatrix = new Matrix3x3(sumXX, sumXY, sumXZ,
                                                       sumXY, sumYY, sumYZ,
                                                       sumXZ, sumYZ, sumZZ);

            //Solve eigen system, smallest eigenvalue is in last position
            Matrix3x3 eigenResult;
            Vector3 diagonal, subdiagonal;
            SolveEigenDecomposition(ref covarianceMatrix, true, out eigenResult, out diagonal, out subdiagonal);

            result.Extents.X = diagonal.X;
            result.Axes.XAxis = new Vector3(eigenResult[0], eigenResult[3], eigenResult[6]); //Col0

            result.Extents.Z = diagonal.Y;
            result.Axes.ZAxis = new Vector3(eigenResult[1], eigenResult[4], eigenResult[7]); //Col1

            result.Extents.Y = diagonal.Z;
            result.Axes.YAxis = new Vector3(eigenResult[2], eigenResult[5], eigenResult[8]); //Col2

            return true;
        }

        //Vector3* flavor of GaussPointsFit
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool GaussPointsFit(Vector3* points, int numPts, out OrientedBoundingBox.Data result)
        {
            result.Center = Vector3.Zero;
            result.Axes = Triad.UnitAxes;
            result.Extents = Vector3.Zero;

            if (points == null || numPts < 2)
                return false;

            //Compute the mean of the points
            result.Center = points[0];

            for (int i = 1; i < numPts; i++)
            {
                Vector3 pt = points[i];
                Vector3.Add(ref result.Center, ref pt, out result.Center);
            }

            float invNumPoints = 1.0f / (float)numPts;
            Vector3.Multiply(ref result.Center, invNumPoints, out result.Center);

            //Compute the covariance matrix of the points
            float sumXX = 0.0f, sumXY = 0.0f, sumXZ = 0.0f;
            float sumYY = 0.0f, sumYZ = 0.0f, sumZZ = 0.0f;

            for (int i = 0; i < numPts; i++)
            {
                Vector3 diff;
                Vector3 pt = points[i];
                Vector3.Subtract(ref pt, ref result.Center, out diff);

                sumXX += diff.X * diff.X;
                sumXY += diff.X * diff.Y;
                sumXZ += diff.X * diff.Z;
                sumYY += diff.Y * diff.Y;
                sumYZ += diff.Y * diff.Z;
                sumZZ += diff.Z * diff.Z;
            }

            sumXX *= invNumPoints;
            sumXY *= invNumPoints;
            sumXZ *= invNumPoints;
            sumYY *= invNumPoints;
            sumYZ *= invNumPoints;
            sumZZ *= invNumPoints;

            Matrix3x3 covarianceMatrix = new Matrix3x3(sumXX, sumXY, sumXZ,
                                                       sumXY, sumYY, sumYZ,
                                                       sumXZ, sumYZ, sumZZ);

            //Solve eigen system, smallest eigenvalue is in last position
            Matrix3x3 eigenResult;
            Vector3 diagonal, subdiagonal;
            SolveEigenDecomposition(ref covarianceMatrix, true, out eigenResult, out diagonal, out subdiagonal);

            result.Extents.X = diagonal.X;
            result.Axes.XAxis = new Vector3(eigenResult[0], eigenResult[3], eigenResult[6]); //Col0

            result.Extents.Z = diagonal.Y;
            result.Axes.ZAxis = new Vector3(eigenResult[1], eigenResult[4], eigenResult[7]); //Col1

            result.Extents.Y = diagonal.Z;
            result.Axes.YAxis = new Vector3(eigenResult[2], eigenResult[5], eigenResult[8]); //Col2

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GaussPointsFit(IReadOnlyDataBuffer<Vector3> points, IndexData indices, out OrientedBoundingBox.Data result)
        {
            return GaussPointsFit(points, indices, 0, (!indices.IsValid) ? indices.Length : 0, 0, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GaussPointsFit(IReadOnlyDataBuffer<Vector3> points, IndexData indices, int offset, int count, int baseVertexOffset, out OrientedBoundingBox.Data result)
        {
            result.Center = Vector3.Zero;
            result.Axes = Triad.UnitAxes;
            result.Extents = Vector3.Zero;

            if (points == null || !indices.IsValid || count < 2 || offset < 0 || baseVertexOffset < 0 || baseVertexOffset >= points.Length
                    || ((offset + count) > indices.Length))
                return false;

            //Compute the mean of the points
            result.Center = points[indices[offset] + baseVertexOffset];

            int upperBoundExclusive = offset + count;
            for (int i = offset + 1; i < upperBoundExclusive; i++)
            {
                int index = indices[i];
                Vector3 pt = points[index + baseVertexOffset];
                Vector3.Add(ref result.Center, ref pt, out result.Center);
            }

            float invNumPoints = 1.0f / (float)count;
            Vector3.Multiply(ref result.Center, invNumPoints, out result.Center);

            //Compute the covariance matrix of the points
            float sumXX = 0.0f, sumXY = 0.0f, sumXZ = 0.0f;
            float sumYY = 0.0f, sumYZ = 0.0f, sumZZ = 0.0f;

            for (int i = offset; i < upperBoundExclusive; i++)
            {
                Vector3 diff;

                int index = indices[i];
                Vector3 pt = points[index + baseVertexOffset];
                Vector3.Subtract(ref pt, ref result.Center, out diff);

                sumXX += diff.X * diff.X;
                sumXY += diff.X * diff.Y;
                sumXZ += diff.X * diff.Z;
                sumYY += diff.Y * diff.Y;
                sumYZ += diff.Y * diff.Z;
                sumZZ += diff.Z * diff.Z;
            }

            sumXX *= invNumPoints;
            sumXY *= invNumPoints;
            sumXZ *= invNumPoints;
            sumYY *= invNumPoints;
            sumYZ *= invNumPoints;
            sumZZ *= invNumPoints;

            Matrix3x3 covarianceMatrix = new Matrix3x3(sumXX, sumXY, sumXZ,
                                                       sumXY, sumYY, sumYZ,
                                                       sumXZ, sumYZ, sumZZ);

            //Solve eigen system, smallest eigenvalue is in last position
            Matrix3x3 eigenResult;
            Vector3 diagonal, subdiagonal;
            SolveEigenDecomposition(ref covarianceMatrix, true, out eigenResult, out diagonal, out subdiagonal);

            result.Extents.X = diagonal.X;
            result.Axes.XAxis = new Vector3(eigenResult[0], eigenResult[3], eigenResult[6]); //Col0

            result.Extents.Y = diagonal.Y;
            result.Axes.YAxis = new Vector3(eigenResult[1], eigenResult[4], eigenResult[7]); //Col1

            result.Extents.Z = diagonal.Z;
            result.Axes.ZAxis = new Vector3(eigenResult[2], eigenResult[5], eigenResult[8]); //Col2

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OrthogonalLineFit(IReadOnlyDataBuffer<Vector3> points, out Vector3 lineOrigin, out Vector3 lineDir)
        {
            return OrthogonalLineFit(points, 0, (points != null) ? points.Length : 0, out lineOrigin, out lineDir);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OrthogonalLineFit(IReadOnlyDataBuffer<Vector3> points, int offset, int count, out Vector3 lineOrigin, out Vector3 lineDir)
        {
            lineOrigin = Vector3.Zero;
            lineDir = Vector3.Zero;

            if (points == null || count <= 0 || offset < 0 || ((offset + count) > points.Length))
                return false;

            lineOrigin = points[offset];

            int upperBoundExclusive = offset + count;
            for (int i = offset + 1; i < upperBoundExclusive; i++)
            {
                Vector3 pt = points[i];
                Vector3.Add(ref lineOrigin, ref pt, out lineOrigin);
            }

            float invNumPoints = 1.0f / (float)count;
            Vector3.Multiply(ref lineOrigin, invNumPoints, out lineOrigin);

            //Compute the covariance matrix of the points
            float sumXX = 0.0f;
            float sumXY = 0.0f;
            float sumXZ = 0.0f;
            float sumYY = 0.0f;
            float sumYZ = 0.0f;
            float sumZZ = 0.0f;

            for (int i = offset; i < upperBoundExclusive; i++)
            {
                Vector3 pt = points[i];
                Vector3 diff;
                Vector3.Subtract(ref pt, ref lineOrigin, out diff);

                sumXX += diff.X * diff.X;
                sumXY += diff.X * diff.Y;
                sumXZ += diff.X * diff.Z;
                sumYY += diff.Y * diff.Y;
                sumYZ += diff.Y * diff.Z;
                sumZZ += diff.Z * diff.Z;
            }

            sumXX *= invNumPoints;
            sumXY *= invNumPoints;
            sumXZ *= invNumPoints;
            sumYY *= invNumPoints;
            sumYZ *= invNumPoints;
            sumZZ *= invNumPoints;

            //Set up the eigensolver
            Matrix3x3 eigenSystem = new Matrix3x3(sumYY + sumZZ, -sumXY, -sumXZ,
                                                  -sumXY, sumXX + sumZZ, -sumYZ,
                                                  -sumXZ, -sumYZ, sumXX + sumYY);

            //Solve eigen system, largest eigenvalue is in last position
            Matrix3x3 eigenResult;
            Vector3 diagonal, subdiagonal;
            SolveEigenDecomposition(ref eigenSystem, false, out eigenResult, out diagonal, out subdiagonal);

            //Unit length direction for best-fit line (column 2)
            lineDir.X = eigenResult[2];
            lineDir.Y = eigenResult[5];
            lineDir.Z = eigenResult[8];

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OrthogonalLineFit(IReadOnlyDataBuffer<Vector3> points, IndexData indices, int offset, int count, int baseVertexOffset, out Vector3 lineOrigin, out Vector3 lineDir)
        {
            lineOrigin = Vector3.Zero;
            lineDir = Vector3.Zero;

            if (points == null || !indices.IsValid || count <= 0 || offset < 0 || baseVertexOffset < 0 || baseVertexOffset >= points.Length
                || ((offset + count) > indices.Length))
                return false;

            lineOrigin = points[indices[offset] + baseVertexOffset];

            int upperBoundExclusive = offset + count;
            for (int i = offset + 1; i < upperBoundExclusive; i++)
            {
                int index = indices[i];
                Vector3 pt = points[index + baseVertexOffset];
                Vector3.Add(ref lineOrigin, ref pt, out lineOrigin);
            }

            float invNumPoints = 1.0f / (float)count;
            Vector3.Multiply(ref lineOrigin, invNumPoints, out lineOrigin);

            //Compute the covariance matrix of the points
            float sumXX = 0.0f;
            float sumXY = 0.0f;
            float sumXZ = 0.0f;
            float sumYY = 0.0f;
            float sumYZ = 0.0f;
            float sumZZ = 0.0f;

            for (int i = offset; i < upperBoundExclusive; i++)
            {
                int index = indices[i];
                Vector3 pt = points[index + baseVertexOffset];
                Vector3 diff;
                Vector3.Subtract(ref pt, ref lineOrigin, out diff);

                sumXX += diff.X * diff.X;
                sumXY += diff.X * diff.Y;
                sumXZ += diff.X * diff.Z;
                sumYY += diff.Y * diff.Y;
                sumYZ += diff.Y * diff.Z;
                sumZZ += diff.Z * diff.Z;
            }

            sumXX *= invNumPoints;
            sumXY *= invNumPoints;
            sumXZ *= invNumPoints;
            sumYY *= invNumPoints;
            sumYZ *= invNumPoints;
            sumZZ *= invNumPoints;

            //Set up the eigensolver
            Matrix3x3 eigenSystem = new Matrix3x3(sumYY + sumZZ, -sumXY, -sumXZ,
                                                  -sumXY, sumXX + sumZZ, -sumYZ,
                                                  -sumXZ, -sumYZ, sumXX + sumYY);

            //Solve eigen system, largest eigenvalue is in last position
            Matrix3x3 eigenResult;
            Vector3 diagonal, subdiagonal;
            SolveEigenDecomposition(ref eigenSystem, false, out eigenResult, out diagonal, out subdiagonal);

            //Unit length direction for best-fit line (column 2)
            lineDir.X = eigenResult[2];
            lineDir.Y = eigenResult[5];
            lineDir.Z = eigenResult[8];

            return true;
        }

        #endregion

        #region Eigen Decomposition

        private static void JacobiMethod(ref Matrix3x3 input, out Vector3 eigenValues, out Matrix3x3 eigenVectors)
        {
            eigenValues = Vector3.Zero;
            eigenVectors = new Matrix3x3();

            Matrix3x3 diagonals = input;
            eigenVectors = Matrix3x3.Identity;

            int p, q;
            float prevoff = 0.0f;
            float cos, sin;
            Matrix3x3 jacobi = Matrix3x3.Identity;
            int maxIterations = 50;

            for (int n = 0; n < maxIterations; n++)
            {
                //Find largest off-diagonal absolute element diagonals[p, q]
                p = 0;
                q = 1;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (i == j)
                            continue;

                        if (Math.Abs(diagonals[i, j]) > Math.Abs(diagonals[p, q]))
                        {
                            p = i;
                            q = j;
                        }
                    }
                }

                //Compute the Jacobi rotation matrix jacobi(p, q, theta)
                SymmetricSchurDecomposition(ref diagonals, p, q, out cos, out sin);

                jacobi[p, p] = cos;
                jacobi[p, q] = sin;
                jacobi[q, p] = -sin;
                jacobi[q, q] = cos;

                //Cumulate rotations into what will contain eigenvectors
                Matrix3x3.Multiply(ref eigenVectors, ref jacobi, out eigenVectors);

                //Make diagonal more...diagonal, until just eigenvalues remain on the diagonal
                jacobi.Transpose();
                Matrix3x3.Multiply(ref jacobi, ref diagonals, out diagonals);
                Matrix3x3.Multiply(ref diagonals, ref jacobi, out diagonals);

                //Comptue norm of off-diagonal elements
                float off = 0.0f;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (i == j)
                            continue;

                        off += diagonals[i, j] * diagonals[i, j];
                    }
                }

                //Stop when norm no longer decreasing
                if (n > 2 && off >= prevoff)
                {
                    eigenValues = new Vector3(diagonals.M11, diagonals.M22, diagonals.M33);
                    return;
                }

                prevoff = off;
            }
        }

        //2-by-2 symmetric schur decomposition. Given n-by-n symmetric matrix and indices p, q such that 1 <= p < q <= n, computes a sine-cosine
        //(sin, cos) that will serve to form a Jacobi rotation matrix
        private static void SymmetricSchurDecomposition(ref Matrix3x3 a, int p, int q, out float cos, out float sin)
        {
            if (Math.Abs(a[p, q]) > 0.0001f)
            {
                float r = (a[q, q] - a[p, p]) / (2.0f * a[p, q]);
                float t;
                if (r >= 0.0f)
                {
                    t = 1.0f / (r + (float)Math.Sqrt(1.0f + r * r));
                }
                else
                {
                    t = -1.0f / (-r + (float)Math.Sqrt(1.0f + r * r));
                }

                cos = 1.0f / (float)Math.Sqrt(1.0f + t * t);
                sin = t * cos;
            }
            else
            {
                cos = 1.0f;
                sin = 0.0f;
            }
        }

        //Eberly + Numerical Recipes in C
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SolveEigenDecomposition(ref Matrix3x3 eigenSystem, bool increasingSort, out Matrix3x3 result, out Vector3 diagonal, out Vector3 subdiagonal)
        {
            diagonal = Vector3.Zero;
            subdiagonal = Vector3.Zero;
            result = eigenSystem;
            bool isRotation = false;

            Tridiagonal3(ref result, ref diagonal, ref subdiagonal, ref isRotation);

            QLAgorithm(ref result, 3, ref diagonal, ref subdiagonal);

            if (increasingSort)
            {
                IncreasingSort(ref result, 3, ref diagonal, ref subdiagonal, ref isRotation);
            }
            else
            {
                DecreasingSort(ref result, 3, ref diagonal, ref subdiagonal, ref isRotation);
            }

            GuaranteeRotation(ref result, 3, isRotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Tridiagonal3(ref Matrix3x3 matrix, ref Vector3 diagonal, ref Vector3 subDiagonal, ref bool isRotation)
        {
            float m00 = matrix.M11;
            float m01 = matrix.M12;
            float m02 = matrix.M13;
            float m11 = matrix.M22;
            float m12 = matrix.M23;
            float m22 = matrix.M33;

            diagonal.X = m00;
            subDiagonal.Z = 0.0f;

            if (Math.Abs(m02) > MathHelper.ZeroTolerance)
            {
                float length = (float)Math.Sqrt((m01 * m01) + (m02 * m02));
                float invLength = 1.0f / length;
                m01 *= invLength;
                m02 *= invLength;

                float q = (2.0f * m01 * m12) + (m02 * (m22 - m11));
                diagonal.Y = m11 + (m02 * q);
                diagonal.Z = m22 - (m02 * q);
                subDiagonal.X = length;
                subDiagonal.Y = m12 - (m01 * q);
                matrix.M11 = 1.0f;
                matrix.M12 = 0.0f;
                matrix.M13 = 0.0f;
                matrix.M21 = 0.0f;
                matrix.M22 = m01;
                matrix.M23 = m02;
                matrix.M31 = 0.0f;
                matrix.M32 = m02;
                matrix.M33 = -m01;
                isRotation = false;
            }
            else
            {
                diagonal.Y = m11;
                diagonal.Z = m22;
                subDiagonal.X = m01;
                subDiagonal.Y = m12;
                matrix.M11 = 1.0f;
                matrix.M12 = 0.0f;
                matrix.M13 = 0.0f;
                matrix.M21 = 0.0f;
                matrix.M22 = 1.0f;
                matrix.M23 = 0.0f;
                matrix.M31 = 0.0f;
                matrix.M32 = 0.0f;
                matrix.M33 = 1.0f;
                isRotation = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool QLAgorithm(ref Matrix3x3 matrix, int size, ref Vector3 diagonal, ref Vector3 subDiagonal)
        {
            int imaxItr = 32;

            for (int i0 = 0; i0 < size; ++i0)
            {
                int i1;
                for (i1 = 0; i1 < imaxItr; ++i1)
                {
                    int i2;
                    for (i2 = i0; i2 <= (size - 2); ++i2)
                    {
                        float tmp = Math.Abs(diagonal[i2]) + Math.Abs(diagonal[i2 + 1]);

                        if (Math.Abs(subDiagonal[i2]) + tmp == tmp)
                            break;
                    }

                    if (i2 == i0)
                        break;

                    float value0 = diagonal[i0 + 1] - (diagonal[i0] / (2.0f * subDiagonal[i0]));
                    float value1 = (float)Math.Sqrt((value0 * value0) + 1.0f);
                    if (value0 < 0.0f)
                    {
                        value0 = diagonal[i2] - diagonal[i0] + (subDiagonal[i0] / (value0 - value1));
                    }
                    else
                    {
                        value0 = diagonal[i2] - diagonal[i0] + (subDiagonal[i0] / (value0 + value1));
                    }

                    float sn = 1.0f;
                    float cs = 1.0f;
                    float value2 = 0.0f;
                    for (int i3 = i2 - 1; i3 >= i0; --i3)
                    {
                        float value3 = sn * subDiagonal[i3];
                        float value4 = cs * subDiagonal[i3];
                        if (Math.Abs(value3) >= Math.Abs(value0))
                        {
                            cs = value0 / value3;
                            value1 = (float)Math.Sqrt((cs * cs) + 1.0f);
                            subDiagonal[i3 + 1] = value3 * value1;
                            sn = 1.0f / value1;
                            cs *= sn;
                        }
                        else
                        {
                            sn = value3 / value0;
                            value1 = (float)Math.Sqrt((sn * sn) + 1.0f);
                            subDiagonal[i3 + 1] = value0 * value1;
                            cs = 1.0f / value1;
                            sn *= cs;
                        }

                        value0 = diagonal[i3 + 1] - value2;
                        value1 = ((diagonal[i3] - value0) * sn) + (2.0f * value4 * cs);
                        value2 = sn * value1;
                        diagonal[i3 + 1] = value0 + value2;
                        value0 = (cs * value1) - value4;

                        for (int i4 = 0; i4 < size; ++i4)
                        {
                            value3 = matrix[i4, i3 + 1];
                            matrix[i4, i3 + 1] = (sn * matrix[i4, i3]) + (cs * value3);
                            matrix[i4, i3] = (cs * matrix[i4, i3]) - (sn * value3);
                        }
                    }

                    diagonal[i0] -= value2;
                    subDiagonal[i0] = value0;
                    subDiagonal[i2] = 0.0f;
                }

                if (i1 == imaxItr)
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void IncreasingSort(ref Matrix3x3 matrix, int size, ref Vector3 diagonal, ref Vector3 subDiagonal, ref bool isRotation)
        {
            //Sort the eigenvalues in creasing order, e[0] <= ... <= e[size - 1]
            for (int i0 = 0, i1 = 0; i0 <= (size - 2); ++i0)
            {
                //Locate min eigenvalue
                i1 = i0;

                float minValue = diagonal[i1];
                int i2;
                for (i2 = i0 + 1; i2 < size; ++i2)
                {
                    if (diagonal[i2] < minValue)
                    {
                        i1 = i2;
                        minValue = diagonal[i1];
                    }
                }

                if (i1 != i0)
                {
                    //Swap eigen values
                    diagonal[i1] = diagonal[i0];
                    diagonal[i0] = minValue;

                    //Swap eigen vectors corresponding to the values
                    for (i2 = 0; i2 < size; ++i2)
                    {
                        float tmp = matrix[i2, i0];
                        matrix[i2, i0] = matrix[i2, i1];
                        matrix[i2, i1] = tmp;
                        isRotation = !isRotation;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DecreasingSort(ref Matrix3x3 matrix, int size, ref Vector3 diagonal, ref Vector3 subDiagonal, ref bool isRotation)
        {
            //Sort the eigen values in decreasing order, e[0] >= ... >= e[size - 1]
            for (int i0 = 0, i1 = 0; i0 <= (size - 2); ++i0)
            {
                //Locate the max eigen value
                i1 = i0;

                float maxValue = diagonal[i1];
                int i2;
                for (i2 = i0 + 1; i2 < size; ++i2)
                {
                    if (diagonal[i2] > maxValue)
                    {
                        i1 = i2;
                        maxValue = diagonal[i1];
                    }
                }

                if (i1 != i0)
                {
                    //Swap the eigen values
                    diagonal[i1] = diagonal[i0];
                    diagonal[i0] = maxValue;

                    //Swap eigen vectors corresponding to the eigen values
                    for (i2 = 0; i2 < size; ++i2)
                    {
                        float tmp = matrix[i2, i0];
                        matrix[i2, i0] = matrix[i2, i1];
                        matrix[i2, i1] = tmp;
                        isRotation = !isRotation;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GuaranteeRotation(ref Matrix3x3 matrix, int size, bool isRotation)
        {
            if (!isRotation)
            {
                //Change sign on first column
                for (int row = 0; row < size; row++)
                    matrix[row, 0] = -matrix[row, 0];
            }
        }

        #endregion

        #region Line Intersection Classify

        private enum IntersectionType
        {
            Empty,
            Point,
            Line
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IntersectionType ClassifyXYIntersection(ref Vector3 line0Origin, ref Vector3 line0Dir, ref Vector3 line1Origin, ref Vector3 line1Dir, out float s0, out float s1)
        {
            s0 = 0.0f;
            s1 = 0.0f;
            
            Vector3.Subtract(ref line1Origin, ref line0Origin, out Vector3 diff);

            float D0DotPerpD1 = (line0Dir.X * line1Dir.Y) - (line0Dir.Y * line1Dir.X);
            if (Math.Abs(D0DotPerpD1) > MathHelper.ZeroTolerance)
            {
                // Line intersects at a single point
                float invD0DotPerpD1 = 1.0f / D0DotPerpD1;
                float diffDotPerpD0 = (diff.X * line0Dir.Y) - (diff.Y * line0Dir.X);
                float diffDotPerpD1 = (diff.X * line1Dir.Y) - (diff.Y * line1Dir.X);
                s0 = diffDotPerpD0 * invD0DotPerpD1;
                s1 = diffDotPerpD1 * invD0DotPerpD1;

                return IntersectionType.Point;
            }

            // Lines are parallel
            // Test if colinear
            diff.Normalize();
            float diffNDotPerpD1 = (diff.X * line1Dir.Y) - (diff.Y * line1Dir.X);
            if (Math.Abs(diffNDotPerpD1) <= MathHelper.ZeroTolerance)
            {
                return IntersectionType.Line;
            }

            // Parallel but distinct
            return IntersectionType.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IntersectionType IntersectInterval(float u0, float u1, float v0, float v1, out float s0, out float s1)
        {
            s0 = 0.0f;
            s1 = 0.0f;

            if (u1 < v0 || u0 > v1)
            {
                return IntersectionType.Empty;
            }
            else if (u1 > v0)
            {
                if (u0 < v1)
                {
                    s0 = (u0 < v0) ? v0 : u0;
                    s1 = (u1 > v1) ? v1 : u1;

                    if (s0 == s1)
                    {
                        return IntersectionType.Point;
                    }

                    return IntersectionType.Line;
                }
                else // u0 == v1
                {
                    s0 = s1 = u0;
                    return IntersectionType.Point;
                }
            }
            else // u1 == v0
            {
                s0 = s1 = u1;
                return IntersectionType.Point;
            }
        }

        #endregion

        #region Triangle Intersection Utility

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ProjectOntoAxis(ref Triangle triangle, ref Vector3 axis, out float fMin, out float fMax)
        {
            Vector3.Dot(ref axis, ref triangle.PointA, out float dot0);
            Vector3.Dot(ref axis, ref triangle.PointB, out float dot1);
            Vector3.Dot(ref axis, ref triangle.PointC, out float dot2);

            fMin = dot0;
            fMax = dot0;

            if (dot1 < fMin)
            {
                fMin = dot1;
            }
            else if (dot1 > fMax)
            {
                fMax = dot1;
            }

            if (dot2 < fMin)
            {
                fMin = dot2;
            }
            else if (dot2 > fMax)
            {
                fMax = dot2;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void TrianglePlaneRelations(ref Triangle triangle, ref Plane plane, float* distance, int* sign, out int positive, out int negative, out int zero)
        {
            // Compute signed distances of triangle vertices to plane.
            positive = 0;
            negative = 0;
            zero = 0;

            for (int i = 0; i < 3; i++)
            {
                distance[i] = plane.SignedDistanceTo(triangle[i]);
                if (distance[i] > MathHelper.ZeroTolerance)
                {
                    sign[i] = 1;
                    positive++;
                }
                else if (distance[i] < -MathHelper.ZeroTolerance)
                {
                    sign[i] = -1;
                    negative++;
                }
                else
                {
                    distance[i] = 0.0f;
                    sign[i] = 0;
                    zero++;
                }
            }
        }

        #endregion
    }
}
