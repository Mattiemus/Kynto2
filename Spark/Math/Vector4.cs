namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    using Content;

    /// <summary>
    /// Defines a four dimensional vector of 32-bit floats.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector4 : IEquatable<Vector4>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// X component of the vector.
        /// </summary>
        public float X;

        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public float Y;

        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public float Z;

        /// <summary>
        /// W component of the vector.
        /// </summary>
        public float W;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4"/> struct.
        /// </summary>
        /// <param name="value">Value to initialize each component to</param>
        public Vector4(float value)
        {
            X = Y = Z = W = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4"/> struct.
        /// </summary>
        /// <param name="value">Vector that contains XY components</param>
        /// <param name="z">Z component</param>
        /// <param name="w">W component</param>
        public Vector4(Vector2 value, float z, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4"/> struct.
        /// </summary>
        /// <param name="value">Vector that contains XYZ components</param>
        /// <param name="w">W component</param>
        public Vector4(Vector3 value, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4"/> struct.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        /// <param name="w">W component</param>
        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Gets a <see cref="Vector4"/> set to (0, 0, 0, 0).
        /// </summary>
        public static Vector4 Zero => new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a <see cref="Vector4"/> set to (1, 1, 1, 1).
        /// </summary>
        public static Vector4 One => new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector4"/> set to (1, 0, 0, 0).
        /// </summary>
        public static Vector4 UnitX => new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector4"/> set to (0, 1, 0, 0).
        /// </summary>
        public static Vector4 UnitY => new Vector4(0.0f, 1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector4"/> set to (0, 0, 1, 0).
        /// </summary>
        public static Vector4 UnitZ => new Vector4(0.0f, 0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Gets a <see cref="Vector4"/> set to (0, 0, 0, 1).
        /// </summary>
        public static Vector4 UnitW => new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets the size of the <see cref="Vector4"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Vector4>();

        /// <summary>
        /// Gets whether the vector is normalized or not.
        /// </summary>
        public bool IsNormalized => MathHelper.IsApproxEquals(LengthSquared(), 1.0f);

        /// <summary>
        /// Gets whether any of the components of the vector are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z) || float.IsNaN(W);

        /// <summary>
        /// Gets whether any of the components of the vector are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(X) || float.IsInfinity(Y) || float.IsInfinity(Z) || float.IsInfinity(W);

        /// <summary>
        /// Gets or sets individual components of the vector in the order that the components are declared (XYZW).
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <returns>The value of the specified component.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
        }
        
        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Vector4 Add(Vector4 a, Vector4 b)
        {
            Add(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Sum of the two vectors</param>
        public static void Add(ref Vector4 a, ref Vector4 b, out Vector4 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
            result.W = a.W + b.W;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Vector4 Subtract(Vector4 a, Vector4 b)
        {
            Subtract(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Difference of the two vectors</param>
        public static void Subtract(ref Vector4 a, ref Vector4 b, out Vector4 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
            result.W = a.W - b.W;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vectorr</returns>
        public static Vector4 Multiply(Vector4 a, Vector4 b)
        {
            Multiply(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Multiplied vector</param>
        public static void Multiply(ref Vector4 a, ref Vector4 b, out Vector4 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
            result.W = a.W * b.W;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Scaled vector</returns>
        public static Vector4 Multiply(Vector4 value, float scale)
        {
            Multiply(ref value, scale, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <param name="result">Scaled vector</param>
        public static void Multiply(ref Vector4 value, float scale, out Vector4 result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
            result.Z = value.Z * scale;
            result.W = value.W * scale;
        }

        /// <summary>
        /// Multiplies the matrix and the vector. The vector is treated
        /// as a column vector, so the multiplication is M*v.
        /// </summary>
        /// <param name="m">Matrix to multiply.</param>
        /// <param name="value">Vector to multiply.</param>
        /// <returns>Resulting vector.</returns>
        public static Vector4 Multiply(Matrix4x4 m, Vector4 value)
        {
            Multiply(ref m, ref value, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Multiplies the matrix and the vector. The vector is treated
        /// as a column vector, so the multiplication is M*v.
        /// </summary>
        /// <param name="m">Matrix to multiply.</param>
        /// <param name="value">Vector to multiply.</param>
        /// <param name="result">Resulting vector.</param>
        public static void Multiply(ref Matrix4x4 m, ref Vector4 value, out Vector4 result)
        {
            result.X = (m.M11 * value.X) + (m.M12 * value.Y) + (m.M13 * value.Z) + (m.M14 * value.W);
            result.Y = (m.M21 * value.X) + (m.M22 * value.Y) + (m.M23 * value.Z) + (m.M24 * value.W);
            result.Z = (m.M31 * value.X) + (m.M32 * value.Y) + (m.M33 * value.Z) + (m.M34 * value.W);
            result.W = (m.M41 * value.X) + (m.M42 * value.Y) + (m.M43 * value.Z) + (m.M44 * value.W);
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Vector4 Divide(Vector4 a, Vector4 b)
        {
            Divide(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <param name="result">Quotient of the two vectors</param>
        public static void Divide(ref Vector4 a, ref Vector4 b, out Vector4 result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
            result.Z = a.Z / b.Z;
            result.W = a.W / b.W;
        }

        /// <summary>
        /// Divides the components of one vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Vector4 Divide(Vector4 value, float divisor)
        {
            Divide(ref value, divisor, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Divides the components of one vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <param name="result">Divided Vector</param>
        public static void Divide(ref Vector4 value, float divisor, out Vector4 result)
        {
            float invDiv = 1.0f / divisor;
            result.X = value.X * invDiv;
            result.Y = value.Y * invDiv;
            result.Z = value.Z * invDiv;
            result.W = value.W * invDiv;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Vector4 Negate(Vector4 value)
        {
            Negate(ref value, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="result">Negated vector</param>
        public static void Negate(ref Vector4 value, out Vector4 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
        }

        /// <summary>
        /// Compute the dot product between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Dot product</returns>
        public static float Dot(Vector4 a, Vector4 b)
        {
            Dot(ref a, ref b, out float result);
            return result;
        }

        /// <summary>
        /// Compute the dot product between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Dot product</param>
        public static void Dot(ref Vector4 a, ref Vector4 b, out float result)
        {
            result = (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z) + (a.W * b.W);
        }

        /// <summary>
        /// Normalize the source vector to a unit vector, which results in a vector with a magnitude of 1 but
        /// preserves the direction the vector was pointing in.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Normalized unit vector</returns>
        public static Vector4 Normalize(Vector4 value)
        {
            Normalize(ref value, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Normalize the source vector to a unit vector, which results in a vector with a magnitude of 1 but
        /// preserves the direction the vector was pointing in.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="result">Normalized unit vector</param>
        /// <returns>The magnitude (length) of the vector.</returns>
        public static float Normalize(ref Vector4 value, out Vector4 result)
        {
            float lengthSquared = value.LengthSquared();
            result = value;
            if (lengthSquared > MathHelper.ZeroTolerance)
            {
                float magnitnude = (float)Math.Sqrt(lengthSquared);

                float invLength = 1.0f / magnitnude;
                result.X *= invLength;
                result.Y *= invLength;
                result.Z *= invLength;
                result.W *= invLength;

                return magnitnude;
            }

            return 0.0f;
        }

        /// <summary>
        /// Compute the distance between two vectors.
        /// </summary>
        /// <param name="start">First vector</param>
        /// <param name="end">Second vector</param>
        /// <returns>Distance between the vectors</returns>
        public static float Distance(Vector4 start, Vector4 end)
        {
            Distance(ref start, ref end, out float result);
            return result;
        }

        /// <summary>
        /// Compute the distance between two vectors.
        /// </summary>
        /// <param name="start">First vector</param>
        /// <param name="end">Second vector</param>
        /// <param name="result">Distance between the vectors</param>
        public static void Distance(ref Vector4 start, ref Vector4 end, out float result)
        {
            DistanceSquared(ref start, ref end, out float distanceSquared);
            result = (float)Math.Sqrt(distanceSquared);
        }

        /// <summary>
        /// Compute the distance squared between two vectors.
        /// </summary>
        /// <param name="start">First vector</param>
        /// <param name="end">Second vector</param>
        /// <returns>Distance squared between the vectors</returns>
        public static float DistanceSquared(Vector4 start, Vector4 end)
        {
            DistanceSquared(ref start, ref end, out float result);
            return result;
        }

        /// <summary>
        /// Compute the distance squared between two vectors.
        /// </summary>
        /// <param name="start">First vector</param>
        /// <param name="end">Second vector</param>
        /// <param name="result">Distance squared between the vectors</param>
        public static void DistanceSquared(ref Vector4 start, ref Vector4 end, out float result)
        {
            float dx = start.X - end.X;
            float dy = start.Y - end.Y;
            float dz = start.Z - end.Z;
            float dw = start.W - end.W;
            result = (dx * dx) + (dy * dy) + (dz * dz) + (dw * dw);
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Maximum vector</returns>
        public static Vector4 Max(Vector4 a, Vector4 b)
        {
            Max(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Maximum vector</param>
        public static void Max(ref Vector4 a, ref Vector4 b, out Vector4 result)
        {
            result.X = (a.X > b.X) ? a.X : b.X;
            result.Y = (a.Y > b.Y) ? a.Y : b.Y;
            result.Z = (a.Z > b.Z) ? a.Z : b.Z;
            result.W = (a.W > b.W) ? a.W : b.W;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Minimum vector</returns>
        public static Vector4 Min(Vector4 a, Vector4 b)
        {
            Min(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Minimum vector</param>
        public static void Min(ref Vector4 a, ref Vector4 b, out Vector4 result)
        {
            result.X = (a.X < b.X) ? a.X : b.X;
            result.Y = (a.Y < b.Y) ? a.Y : b.Y;
            result.Z = (a.Z < b.Z) ? a.Z : b.Z;
            result.W = (a.W < b.W) ? a.W : b.W;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <returns>Clamped vector</returns>
        public static Vector4 Clamp(Vector4 value, Vector4 min, Vector4 max)
        {
            Clamp(ref value, ref min, ref max, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">Clamped vector</param>
        public static void Clamp(ref Vector4 value, ref Vector4 min, ref Vector4 max, out Vector4 result)
        {
            float x = value.X;
            float y = value.Y;
            float z = value.Z;
            float w = value.W;

            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            w = (w > max.W) ? max.W : w;
            w = (w < min.W) ? min.W : w;

            result.X = x;
            result.Y = y;
            result.Z = z;
            result.W = w;
        }

        /// <summary>
        /// Returns a Vector4 containing the 4D Cartesian coordinates of a point specified
        ///  in barycentric coordinates relative to a 4D triangle.
        /// </summary>
        /// <param name="a">Vector4 containing 4D cartesian coordinates corresponding to triangle's first vertex</param>
        /// <param name="b">Vector4 containing 4D cartesian coordinates corresponding to triangle's second vertex</param>
        /// <param name="c">Vector4 containing 4D cartesian coordinates corresponding to triangle's third vertex</param>
        /// <param name="s">Barycentric coordinate s that is the weighting factor toward the second vertex</param>
        /// <param name="t">Barycentric coordinate t that is the weighting factor toward the third vertex</param>
        /// <returns>Barycentric coordinates</returns>
        public static Vector4 Barycentric(Vector4 a, Vector4 b, Vector4 c, float s, float t)
        {
            Barycentric(ref a, ref b, ref c, s, t, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Returns a Vector4 containing the 3D Cartesian coordinates of a point specified
        ///  in barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="a">Vector4 containing 3D cartesian coordinates corresponding to triangle's first vertex</param>
        /// <param name="b">Vector4 containing 3D cartesian coordinates corresponding to triangle's second vertex</param>
        /// <param name="c">Vector4 containing 3D cartesian coordinates corresponding to triangle's third vertex</param>
        /// <param name="s">Barycentric coordinate s that is the weighting factor toward the second vertex</param>
        /// <param name="t">Barycentric coordinate t that is the weighting factor toward the third vertex</param>
        /// <param name="result">Barycentric coordinates</param>
        public static void Barycentric(ref Vector4 a, ref Vector4 b, ref Vector4 c, float s, float t, out Vector4 result)
        {
            result.X = (a.X + (s * (b.X - a.X))) + (t * (c.X - a.X));
            result.Y = (a.Y + (s * (b.Y - a.Y))) + (t * (c.Y - a.Y));
            result.Z = (a.Z + (s * (b.Z - a.Z))) + (t * (c.Z - a.Z));
            result.W = (a.W + (s * (b.W - a.W))) + (t * (c.W - a.W));
        }

        /// <summary>
        /// Compute Catmull-Rom interpolation using the the specified positions.
        /// </summary>
        /// <param name="a">First position</param>
        /// <param name="b">Second position</param>
        /// <param name="c">Third position</param>
        /// <param name="d">Fourth position</param>
        /// <param name="wf">Weighting factor</param>
        /// <returns>Catmull-Rom interpolated vector</returns>
        public static Vector4 CatmullRom(Vector4 a, Vector4 b, Vector4 c, Vector4 d, float wf)
        {
            CatmullRom(ref a, ref b, ref c, ref d, wf, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Compute Catmull-Rom interpolation.
        /// </summary>
        /// <param name="a">First position</param>
        /// <param name="b">Second position</param>
        /// <param name="c">Third position</param>
        /// <param name="d">Fourth position</param>
        /// <param name="wf">Weighting factor</param>
        /// <param name="result">Catmull-Rom interpolated vector</param>
        public static void CatmullRom(ref Vector4 a, ref Vector4 b, ref Vector4 c, ref Vector4 d, float wf, out Vector4 result)
        {
            float wfSquared = wf * wf;
            float wfCubed = wf * wfSquared;
            result.X = 0.5f * ((((2.0f * b.X) + ((-a.X + c.X) * wf)) + (((((2.0f * a.X) - (5.0f * b.X)) + (4.0f * c.X)) - d.X) * wfSquared)) + ((((-a.X + (3.0f * b.X)) - (3.0f * c.X)) + d.X) * wfCubed));
            result.Y = 0.5f * ((((2.0f * b.Y) + ((-a.Y + c.Y) * wf)) + (((((2.0f * a.Y) - (5.0f * b.Y)) + (4.0f * c.Y)) - d.Y) * wfSquared)) + ((((-a.Y + (3.0f * b.Y)) - (3.0f * c.Y)) + d.Y) * wfCubed));
            result.Z = 0.5f * ((((2.0f * b.Z) + ((-a.Z + c.Z) * wf)) + (((((2.0f * a.Z) - (5.0f * b.Z)) + (4.0f * c.Z)) - d.Z) * wfSquared)) + ((((-a.Z + (3.0f * b.Z)) - (3.0f * c.Z)) + d.Z) * wfCubed));
            result.W = 0.5f * ((((2.0f * b.W) + ((-a.W + c.W) * wf)) + (((((2.0f * a.W) - (5.0f * b.W)) + (4.0f * c.W)) - d.W) * wfSquared)) + ((((-a.W + (3.0f * b.W)) - (3.0f * c.W)) + d.W) * wfCubed));
        }

        /// <summary>
        /// Compute a Hermite spline interpolation.
        /// </summary>
        /// <param name="a">First position</param>
        /// <param name="tangentA">First vector's tangent</param>
        /// <param name="b">Second position</param>
        /// <param name="tangentB">Second vector's tangent</param>
        /// <param name="wf">Weighting factor</param>
        /// <returns>Hermite interpolated vector</returns>
        public static Vector4 Hermite(Vector4 a, Vector4 tangentA, Vector4 b, Vector4 tangentB, float wf)
        {
            Hermite(ref a, ref tangentA, ref b, ref tangentB, wf, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Compute a Hermite spline interpolation.
        /// </summary>
        /// <param name="a">First position</param>
        /// <param name="tangentA">First vector's tangent</param>
        /// <param name="b">Second position</param>
        /// <param name="tangentB">Second vector's tangent</param>
        /// <param name="wf">Weighting factor</param>
        /// <param name="result">Hermite interpolated vector</param>
        public static void Hermite(ref Vector4 a, ref Vector4 tangentA, ref Vector4 b, ref Vector4 tangentB, float wf, out Vector4 result)
        {
            float wfSquared = wf * wf;
            float wfCubed = wfSquared * wf;
            float h1 = ((2.0f * wfCubed) - (3.0f * wfSquared)) + 1.0f;
            float h2 = (-2.0f * wfCubed) + (3.0f * wfSquared);
            float h3 = (wfCubed - (2.0f * wfSquared)) + wf;
            float h4 = wfCubed - wfSquared;

            result.X = (((a.X * h1) + (b.X * h2)) + (tangentA.X * h3)) + (tangentB.X * h4);
            result.Y = (((a.Y * h1) + (b.Y * h2)) + (tangentA.Y * h3)) + (tangentB.Y * h4);
            result.Z = (((a.Z * h1) + (b.Z * h2)) + (tangentA.Z * h3)) + (tangentB.Z * h4);
            result.W = (((a.W * h1) + (b.W * h2)) + (tangentA.W * h3)) + (tangentB.W * h4);
        }

        /// <summary>
        /// Compute a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="wf">Weighting factor (between 0 and 1.0)</param>
        /// <returns>Cubic interpolated vector</returns>
        public static Vector4 SmoothStep(Vector4 a, Vector4 b, float wf)
        {
            SmoothStep(ref a, ref b, wf, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Compute a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="wf">Weighting factor (between 0 and 1.0)</param>
        /// <param name="result">Cubic interpolated vector</param>
        public static void SmoothStep(ref Vector4 a, ref Vector4 b, float wf, out Vector4 result)
        {
            float amt = MathHelper.Clamp(wf, 0.0f, 1.0f);
            amt = (amt * amt) * (3.0f - (2.0f * amt));
            result.X = a.X + ((b.X - a.X) * amt);
            result.Y = a.Y + ((b.Y - a.Y) * amt);
            result.Z = a.Z + ((b.Z - a.Z) * amt);
            result.W = a.W + ((b.W - a.W) * amt);
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        /// <param name="a">Starting vector</param>
        /// <param name="b">Ending vector</param>
        /// <param name="percent">Amount to interpolate by</param>
        /// <returns>Linear interpolated vector</returns>
        public static Vector4 Lerp(Vector4 a, Vector4 b, float percent)
        {
            Lerp(ref a, ref b, percent, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        /// <param name="a">Starting vector</param>
        /// <param name="b">Ending vector</param>
        /// <param name="percent">Amount to interpolate by</param>
        /// <param name="result">Linear interpolated vector</param>
        public static void Lerp(ref Vector4 a, ref Vector4 b, float percent, out Vector4 result)
        {
            result.X = a.X + ((b.X - a.X) * percent);
            result.Y = a.Y + ((b.Y - a.Y) * percent);
            result.Z = a.Z + ((b.Z - a.Z) * percent);
            result.W = a.W + ((b.W - a.W) * percent);
        }

        /// <summary>
        /// Transforms the specified vector by the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="m">Transformation matrix</param>
        /// <returns>Transformed vector</returns>
        public static Vector4 Transform(Vector4 value, Matrix4x4 m)
        {
            Transform(ref value, ref m, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Transforms the specified vector by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="result">Transformed vector</param>
        public static void Transform(ref Vector4 value, ref Matrix4x4 m, out Vector4 result)
        {
            result.X = (value.X * m.M11) + (value.Y * m.M21) + (value.Z * m.M31) + (value.W * m.M41);
            result.Y = (value.X * m.M12) + (value.Y * m.M22) + (value.Z * m.M32) + (value.W * m.M42);
            result.Z = (value.X * m.M13) + (value.Y * m.M23) + (value.Z * m.M33) + (value.W * m.M43);
            result.W = (value.X * m.M14) + (value.Y * m.M24) + (value.Z * m.M34) + (value.W * m.M44);
        }

        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="source">Array of vectors to be transformed</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="destination">Array to store transformed vectors, this may be the same as the source array</param>
        public static void Transform(Vector4[] source, ref Matrix4x4 m, Vector4[] destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Input array is null");
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination), "Input array is null");
            }

            if (destination.Length < source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "Destination array is smaller than source");
            }

            for (int i = 0; i < source.Length; i++)
            {
                Transform(ref source[i], ref m, out destination[i]);
            }
        }

        /// <summary>
        /// Transforms the specified vector by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="q">Quaternion rotation</param>
        /// <returns>Transformed vector</returns>
        public static Vector4 Transform(Vector4 value, Quaternion q)
        {
            Transform(ref value, ref q, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Transforms the specified vector by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="q">Quaternion rotation</param>
        /// <param name="result">Transformed vector</param>
        public static void Transform(ref Vector4 value, ref Quaternion q, out Vector4 result)
        {
            float x2 = q.X + q.X;
            float y2 = q.Y + q.Y;
            float z2 = q.Z + q.Z;

            float wx2 = q.W * x2;
            float wy2 = q.W * y2;
            float wz2 = q.W * z2;

            float xx2 = q.X * x2;
            float xy2 = q.X * y2;
            float xz2 = q.X * z2;

            float yy2 = q.Y * y2;
            float yz2 = q.Y * z2;

            float zz2 = q.Z * z2;

            float x = ((value.X * ((1.0f - yy2) - zz2)) + (value.Y * (xy2 - wz2))) + (value.Z * (xz2 + wy2));
            float y = ((value.X * (xy2 + wz2)) + (value.Y * ((1.0f - xx2) - zz2))) + (value.Z * (yz2 - wx2));
            float z = ((value.X * (xz2 - wy2)) + (value.Y * (yz2 + wx2))) + (value.Z * ((1.0f - xx2) - yy2));

            result.X = x;
            result.Y = y;
            result.Z = z;
            result.W = value.W;
        }

        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Quaternion"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="source">Array of vectors to be transformed</param>
        /// <param name="q">Quaternion rotation</param>
        /// <param name="destination">Array to store transformed vectors, this may be the same as the source array</param>
        public static void Transform(Vector4[] source, ref Quaternion q, Vector4[] destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Input array is null");
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination), "Input array is null");
            }

            if (destination.Length < source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "Destination array is smaller than source");
            }

            for (int i = 0; i < source.Length; i++)
            {
                Transform(ref source[i], ref q, out destination[i]);
            }
        }

        /// <summary>
        /// Tests equality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public static bool operator ==(Vector4 a, Vector4 b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if components are not equal, false otherwise.</returns>
        public static bool operator !=(Vector4 a, Vector4 b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Adds the two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            Add(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Vector4 operator -(Vector4 value)
        {
            Negate(ref value, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            Subtract(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector4 operator *(Vector4 a, Vector4 b)
        {
            Multiply(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Multiplied vector</returns>
        public static Vector4 operator *(Vector4 value, float scale)
        {
            Multiply(ref value, scale, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="scale">Amount to scale</param>
        /// <param name="value">Source vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector4 operator *(float scale, Vector4 value)
        {
            Multiply(ref value, scale, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Vector4 operator /(Vector4 a, Vector4 b)
        {
            Divide(ref a, ref b, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Vector4 operator /(Vector4 value, float divisor)
        {
            Divide(ref value, divisor, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the vector.
        /// </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
            W = -W;
        }

        /// <summary>
        /// Compute the length (magnitude) of the vector.
        /// </summary>
        /// <returns>Length</returns>
        public float Length()
        {
            double lengthSquared = LengthSquared();
            return (float)Math.Sqrt(lengthSquared);
        }

        /// <summary>
        /// Compute the length (magnitude) squared of the vector.
        /// </summary>
        /// <returns>Length squared</returns>
        public float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z) + (W * W);
        }

        /// <summary>
        /// Normalize the vector to a unit vector, which results in a vector with a magnitude of 1 but
        /// preserves the direction the vector was pointing in.
        /// </summary>
        /// <returns>The magnitude (length) of the vector.</returns>
        public float Normalize()
        {
            float lengthSquared = LengthSquared();
            if (lengthSquared > MathHelper.ZeroTolerance)
            {
                float magnitnude = (float)Math.Sqrt(lengthSquared);

                float invLength = 1.0f / magnitnude;
                X *= invLength;
                Y *= invLength;
                Z *= invLength;
                W *= invLength;

                return magnitnude;
            }

            return 0.0f;
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Vector4 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(ref Vector4 other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(Vector4 other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(ref Vector4 other, float tolerance)
        {
            return (Math.Abs(X - other.X) <= tolerance) &&
                   (Math.Abs(Y - other.Y) <= tolerance) &&
                   (Math.Abs(Z - other.Z) <= tolerance) &&
                   (Math.Abs(W - other.W) <= tolerance);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector4)
            {
                return Equals((Vector4)obj);
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
                return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
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

            return string.Format(formatProvider, "X: {0} Y: {1} Z: {2} W: {3}", new object[] { X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider), W.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            output.Write("X", X);
            output.Write("Y", Y);
            output.Write("Z", Z);
            output.Write("W", W);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            X = input.ReadSingle();
            Y = input.ReadSingle();
            Z = input.ReadSingle();
            W = input.ReadSingle();
        }



















    }
}
