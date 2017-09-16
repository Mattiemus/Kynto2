namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a two dimensional vector of 32-bit floats.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector2 : IEquatable<Vector2>, IFormattable, IPrimitiveValue
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
        /// Initializes a new instance of the <see cref="Vector2"/> struct.
        /// </summary>
        /// <param name="value">Value to initialize each component to</param>
        public Vector2(float value)
        {
            X = Y = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2"/> struct.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets a <see cref="Vector2"/> set to (0, 0).
        /// </summary>
        public static Vector2 Zero => new Vector2(0.0f, 0.0f);

        /// <summary>
        /// Gets a <see cref="Vector2"/> set to (1, 1).
        /// </summary>
        public static Vector2 One => new Vector2(1.0f, 1.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector2"/> set to (1, 0).
        /// </summary>
        public static Vector2 UnitX => new Vector2(1.0f, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector2"/> set to (0, 1).
        /// </summary>
        public static Vector2 UnitY => new Vector2(0.0f, 1.0f);

        /// <summary>
        /// Gets the size of the <see cref="Vector2"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Vector2>();

        /// <summary>
        /// Gets whether the vector is normalized or not.
        /// </summary>
        public bool IsNormalized => MathHelper.IsApproxEquals(LengthSquared(), 1.0f);

        /// <summary>
        /// Gets whether any of the components of the vector are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y);

        /// <summary>
        /// Gets whether any of the components of the vector are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(X) || float.IsInfinity(Y);

        /// <summary>
        /// Gets or sets individual components of the vector in the order that the components are declared (XY).
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
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
        }
        
        /// <summary>
        /// Compute the acute angle between two vectors in the range of [0, PI / 2]. Assumes that both vectors are already normalized.
        /// </summary>
        /// <param name="a">First unit vector</param>
        /// <param name="b">Second unit vector</param>
        /// <returns>Acute angle between the vectors</returns>
        public static Angle AcuteAngleBetween(Vector2 a, Vector2 b)
        {
            AcuteAngleBetween(ref a, ref b, out Angle angle);
            return angle;
        }

        /// <summary>
        /// Compute the acute angle between two vectors in the range of [0, PI / 2]. Assumes that both vectors are already normalized.
        /// </summary>
        /// <param name="a">First unit vector</param>
        /// <param name="b">Second unit vector</param>
        /// <param name="result">Angle between the vectors</param>
        public static void AcuteAngleBetween(ref Vector2 a, ref Vector2 b, out Angle result)
        {
            result = new Angle((float)Math.Acos(MathHelper.Clamp(Dot(a, b), -1.0f, 1.0f)));
            if (result.Radians > MathHelper.PiOverTwo)
            {
                result.Radians = MathHelper.Pi - result.Radians;
            }
        }

        /// <summary>
        /// Compute the angle between two vectors in the range of [0, PI]. Assumes that both vectors are already normalized.
        /// </summary>
        /// <param name="a">First unit vector</param>
        /// <param name="b">Second unit vector</param>
        /// <returns>Angle between the vectors</returns>
        public static Angle AngleBetween(Vector2 a, Vector2 b)
        {
            AcuteAngleBetween(ref a, ref b, out Angle angle);
            return angle;
        }

        /// <summary>
        /// Compute the angle between two vectors in the range of [0, PI]. Assumes that both vectors are already normalized.
        /// </summary>
        /// <param name="a">First unit vector</param>
        /// <param name="b">Second unit vector</param>
        /// <param name="angle">Angle between the vectors</param>
        public static void AngleBetween(ref Vector2 a, ref Vector2 b, out Angle angle)
        {
            angle = new Angle((float)Math.Acos(MathHelper.Clamp(Dot(a, b), -1.0f, 1.0f)));
        }

        /// <summary>
        /// Computes a signed angle between two vectors in the range of [-PI, PI]. The 3D case would use a plane normal to determine orientation, but since 2D vectors are always on the XY
        /// plane, the orientation vector can be either positive/negative UnitZ (0, 0, 1) or (0, 0, -1). The signed angle is found by taking the cross product of
        /// the two vectors [source X dest] and computing the dot product with the orientation vector. Assumes that the two vectors are already normalized.
        /// </summary>
        /// <param name="source">Start unit vector</param>
        /// <param name="dest">Destination unit vector</param>
        /// <param name="planeNormalUnitZ">True if the plane normal should be UnitZ (0, 0, 1), false if the opposite direction (0, 0, -1). By default this is true.</param>
        /// <returns>Signed angle between the vectors</returns>
        public static Angle SignedAngleBetween(Vector2 source, Vector2 dest, bool planeNormalUnitZ = true)
        {
            SignedAngleBetween(ref source, ref dest, out Angle angle, planeNormalUnitZ);
            return angle;
        }

        /// <summary>
        /// Computes a signed angle between two vectors in the range of [-PI, PI]. The 3D case would use a plane normal to determine orientation, but since 2D vectors are always on the XY
        /// plane, the orientation vector can be either positive/negative UnitZ (0, 0, 1) or (0, 0, -1). The signed angle is found by taking the cross product of
        /// the two vectors [source X dest] and computing the dot product with the orientation vector. Assumes that the two vectors are already normalized.
        /// </summary>
        /// <param name="source">Start unit vector</param>
        /// <param name="dest">Destination unit vector</param>
        /// <param name="signedAngle">Signed angle between the vectors</param>
        /// <param name="planeNormalUnitZ">True if the plane normal should be UnitZ (0, 0, 1), false if the opposite direction (0, 0, -1). By default this is true.</param>
        public static void SignedAngleBetween(ref Vector2 source, ref Vector2 dest, out Angle signedAngle, bool planeNormalUnitZ = true)
        {
            float angleBetween = (float)Math.Acos(MathHelper.Clamp(Dot(source, dest), -1.0f, 1.0f));

            Vector3 source3D = new Vector3(source, 0.0f);
            Vector3 dest3D = new Vector3(dest, 0.0f);
            Vector3 planeNormal = (planeNormalUnitZ) ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
            
            Vector3.Cross(ref source3D, ref dest3D, out Vector3 cross);
            Vector3.Dot(ref cross, ref planeNormal, out float dot);

            // Cross product should either be pointing in same direction as planeNormal or in opposite. If opposite, it will be negative
            signedAngle = new Angle((dot < 0.0f) ? -angleBetween : angleBetween);
        }

        /// <summary>
        /// Computes a signed acute angle between two vectors in the range of [-PI / 2, PI / 2]. The 3D case would use a plane normal to determine orientation, but since 2D vectors are always on the XY
        /// plane, the orientation vector can be either positive/negative UnitZ (0, 0, 1) or (0, 0, -1). The signed angle is found by taking the cross product of
        /// the two vectors [source X dest] and computing the dot product with the orientation vector. Assumes that the two vectors are already normalized.
        /// </summary>
        /// <param name="source">Start unit vector</param>
        /// <param name="dest">Destination unit vector</param>
        /// <param name="planeNormalUnitZ">True if the plane normal should be UnitZ (0, 0, 1), false if the opposite direction (0, 0, -1). By default this is true.</param>
        /// <returns>Signed angle between the vectors</returns>
        public static Angle SignedAcuteAngleBetween(Vector2 source, Vector2 dest, bool planeNormalUnitZ = true)
        {
            SignedAcuteAngleBetween(ref source, ref dest, out Angle signedAngle, planeNormalUnitZ);
            return signedAngle;
        }

        /// <summary>
        /// Computes a signed acute angle between two vectors in the range of [-PI / 2, PI / 2]. The 3D case would use a plane normal to determine orientation, but since 2D vectors are always on the XY
        /// plane, the orientation vector can be either positive/negative UnitZ (0, 0, 1) or (0, 0, -1). The signed angle is found by taking the cross product of
        /// the two vectors [source X dest] and computing the dot product with the orientation vector. Assumes that the two vectors are already normalized.
        /// </summary>
        /// <param name="source">Start unit vector</param>
        /// <param name="dest">Destination unit vector</param>
        /// <param name="signedAngle">Signed angle between the vectors</param>
        /// <param name="planeNormalUnitZ">True if the plane normal should be UnitZ (0, 0, 1), false if the opposite direction (0, 0, -1). By default this is true.</param>
        public static void SignedAcuteAngleBetween(ref Vector2 source, ref Vector2 dest, out Angle signedAngle, bool planeNormalUnitZ = true)
        {
            float angleBetween = (float)Math.Acos(MathHelper.Clamp((source.X * dest.X) + (source.Y * dest.Y), -1.0f, 1.0f));
            if (angleBetween > MathHelper.PiOverTwo)
            {
                angleBetween = MathHelper.Pi - angleBetween;
            }
            
            Vector3 source3D = new Vector3(source, 0.0f);
            Vector3 dest3D = new Vector3(dest, 0.0f);
            Vector3 planeNormal = (planeNormalUnitZ) ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
            
            Vector3.Cross(ref source3D, ref dest3D, out Vector3 cross);
            Vector3.Dot(ref cross, ref planeNormal, out float dot);

            // Cross product should either be pointing in same direction as planeNormal or in opposite. If opposite, it will be negative
            signedAngle = new Angle((dot < 0.0f) ? -angleBetween : angleBetween);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Vector2 Add(Vector2 a, Vector2 b)
        {
            Add(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Sum of the two vectors</param>
        public static void Add(ref Vector2 a, ref Vector2 b, out Vector2 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Vector2 Subtract(Vector2 a, Vector2 b)
        {
            Subtract(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Difference of the two vectors</param>
        public static void Subtract(ref Vector2 a, ref Vector2 b, out Vector2 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector2 Multiply(Vector2 a, Vector2 b)
        {
            Multiply(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Multiplied vector</param>
        public static void Multiply(ref Vector2 a, ref Vector2 b, out Vector2 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Scaled vector</returns>
        public static Vector2 Multiply(Vector2 value, float scale)
        {
            Multiply(ref value, scale, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <param name="result">Scaled vector</param>
        public static void Multiply(ref Vector2 value, float scale, out Vector2 result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
        }

        /// <summary>
        /// Multiplies the matrix and the vector. The vector is treated
        /// as a column vector, so the multiplication is M*v.
        /// </summary>
        /// <param name="m">Matrix to multiply.</param>
        /// <param name="value">Vector to multiply.</param>
        /// <returns>Resulting vector</returns>
        public static Vector2 Multiply(Matrix4x4 m, Vector2 value)
        {
            Multiply(ref m, ref value, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Multiplies the matrix and the vector. The vector is treated
        /// as a column vector, so the multiplication is M*v.
        /// </summary>
        /// <param name="m">Matrix to multiply.</param>
        /// <param name="value">Vector to multiply.</param>
        /// <param name="result">Resulting vector</param>
        public static void Multiply(ref Matrix4x4 m, ref Vector2 value, out Vector2 result)
        {
            result.X = (m.M11 * value.X) + (m.M12 * value.Y);
            result.Y = (m.M21 * value.X) + (m.M22 * value.Y);
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Vector2 Divide(Vector2 a, Vector2 b)
        {
            Divide(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <param name="result">Quotient of the two vectors</param>
        public static void Divide(ref Vector2 a, ref Vector2 b, out Vector2 result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
        }

        /// <summary>
        /// Divides the components of a vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Vector2 Divide(Vector2 value, float divisor)
        {
            Divide(ref value, divisor, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Divides the components of a vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <param name="result">Divided vector</param>
        public static void Divide(ref Vector2 value, float divisor, out Vector2 result)
        {
            float invDiv = 1.0f / divisor;
            result.X = value.X * invDiv;
            result.Y = value.Y * invDiv;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Vector2 Negate(Vector2 value)
        {
            Negate(ref value, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="result">Negated vector</param>
        public static void Negate(ref Vector2 value, out Vector2 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        /// <summary>
        /// Compute the dot product between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Dot product</returns>
        public static float Dot(Vector2 a, Vector2 b)
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
        public static void Dot(ref Vector2 a, ref Vector2 b, out float result)
        {
            result = (a.X * b.X) + (a.Y * b.Y);
        }

        /// <summary>
        /// Normalize the source vector to a unit vector, which results in a vector with a magnitude of 1 but
        /// preserves the direction the vector was pointing in.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Normalized unit vector</returns>
        public static Vector2 Normalize(Vector2 value)
        {
            Normalize(ref value, out Vector2 result);
            return value;
        }

        /// <summary>
        /// Normalize the source vector to a unit vector, which results in a vector with a magnitude of 1 but
        /// preserves the direction the vector was pointing in.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="result">Normalized unit vector</param>
        /// <returns>The magnitude (length) of the vector.</returns>
        public static float Normalize(ref Vector2 value, out Vector2 result)
        {
            float lengthSquared = value.LengthSquared();

            result = value;

            if (lengthSquared > MathHelper.ZeroTolerance)
            {
                float magnitnude = (float)Math.Sqrt(lengthSquared);

                float invLength = 1.0f / magnitnude;
                result.X *= invLength;
                result.Y *= invLength;

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
        public static float Distance(Vector2 start, Vector2 end)
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
        public static void Distance(ref Vector2 start, ref Vector2 end, out float result)
        {
            float dx = start.X - end.X;
            float dy = start.Y - end.Y;
            float distanceSquared = (dx * dx) + (dy * dy);
            result = (float)Math.Sqrt(distanceSquared);
        }

        /// <summary>
        /// Compute the distance squared between two vectors.
        /// </summary>
        /// <param name="start">First vector</param>
        /// <param name="end">Second vector</param>
        /// <returns>Distance squared between the vectors</returns>
        public static float DistanceSquared(Vector2 start, Vector2 end)
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
        public static void DistanceSquared(ref Vector2 start, ref Vector2 end, out float result)
        {
            float dx = start.X - end.X;
            float dy = start.Y - end.Y;
            result = (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Maximum vector</returns>
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            Max(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Maximum vector</param>
        public static void Max(ref Vector2 a, ref Vector2 b, out Vector2 result)
        {
            result.X = (a.X > b.X) ? a.X : b.X;
            result.Y = (a.Y > b.Y) ? a.Y : b.Y;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Minimum vector</returns>
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            Min(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Minimum vector</param>
        public static void Min(ref Vector2 a, ref Vector2 b, out Vector2 result)
        {
            result.X = (a.X < b.X) ? a.X : b.X;
            result.Y = (a.Y < b.Y) ? a.Y : b.Y;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <returns>Clamped vector</returns>
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            Clamp(ref value, ref min, ref max, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">Clamped vector</param>
        public static void Clamp(ref Vector2 value, ref Vector2 min, ref Vector2 max, out Vector2 result)
        {
            float x = value.X;
            float y = value.Y;

            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            result.X = x;
            result.Y = y;
        }

        /// <summary>
        /// Returns a <see cref="Vector2"/> containing the 2D Cartesian coordinates of a point specified
        /// in barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="a">Vector containing 2D cartesian coordinates corresponding to triangle's first vertex</param>
        /// <param name="b">Vector containing 2D cartesian coordinates corresponding to triangle's second vertex</param>
        /// <param name="c">Vector containing 2D cartesian coordinates corresponding to triangle's third vertex</param>
        /// <param name="s">Barycentric coordinate s that is the weighting factor toward the second vertex</param>
        /// <param name="t">Barycentric coordinate t that is the weighting factor toward the third vertex</param>
        /// <returns>Barycentric coordinates</returns>
        public static Vector2 Barycentric(Vector2 a, Vector2 b, Vector2 c, float s, float t)
        {
            Barycentric(ref a, ref b, ref c, s, t, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="Vector2"/> containing the 2D Cartesian coordinates of a point specified
        /// in barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="a">Vector containing 2D cartesian coordinates corresponding to triangle's first vertex</param>
        /// <param name="b">Vector containing 2D cartesian coordinates corresponding to triangle's second vertex</param>
        /// <param name="c">Vector containing 2D cartesian coordinates corresponding to triangle's third vertex</param>
        /// <param name="s">Barycentric coordinate s that is the weighting factor toward the second vertex</param>
        /// <param name="t">Barycentric coordinate t that is the weighting factor toward the third vertex</param>
        /// <param name="result">Barycentric coordinates</param>
        public static void Barycentric(ref Vector2 a, ref Vector2 b, ref Vector2 c, float s, float t, out Vector2 result)
        {
            result.X = (a.X + (s * (b.X - a.X))) + (t * (c.X - a.X));
            result.Y = (a.Y + (s * (b.Y - a.Y))) + (t * (c.Y - a.Y));
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
        public static Vector2 CatmullRom(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float wf)
        {
            CatmullRom(ref a, ref b, ref c, ref d, wf, out Vector2 result);
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
        public static void CatmullRom(ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 d, float wf, out Vector2 result)
        {
            float wfSquared = wf * wf;
            float wfCubed = wf * wfSquared;
            result.X = 0.5f * ((((2.0f * b.X) + ((-a.X + c.X) * wf)) + (((((2.0f * a.X) - (5.0f * b.X)) + (4.0f * c.X)) - d.X) * wfSquared)) + ((((-a.X + (3.0f * b.X)) - (3.0f * c.X)) + d.X) * wfCubed));
            result.Y = 0.5f * ((((2.0f * b.Y) + ((-a.Y + c.Y) * wf)) + (((((2.0f * a.Y) - (5.0f * b.Y)) + (4.0f * c.Y)) - d.Y) * wfSquared)) + ((((-a.Y + (3.0f * b.Y)) - (3.0f * c.Y)) + d.Y) * wfCubed));
        }

        /// <summary>
        /// Compute a Hermite spline interpolation.
        /// </summary>
        /// <param name="a">First position</param>
        /// <param name="tangentA">First vector's tangent</param>
        /// <param name="b">Second position</param>
        /// <param name="tangentB">Second vector's tangent</param>
        /// <param name="wf">Weighting factor</param>
        /// <returns>Hermite interpolated vecto</returns>
        public static Vector2 Hermite(Vector2 a, Vector2 tangentA, Vector2 b, Vector2 tangentB, float wf)
        {
            Hermite(ref a, ref tangentA, ref b, ref tangentB, wf, out Vector2 result);
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
        /// <param name="result">Hermite interpolated vecto</param>
        public static void Hermite(ref Vector2 a, ref Vector2 tangentA, ref Vector2 b, ref Vector2 tangentB, float wf, out Vector2 result)
        {
            float wfSquared = wf * wf;
            float wfCubed = wfSquared * wf;
            float h1 = ((2.0f * wfCubed) - (3.0f * wfSquared)) + 1.0f;
            float h2 = (-2.0f * wfCubed) + (3.0f * wfSquared);
            float h3 = (wfCubed - (2.0f * wfSquared)) + wf;
            float h4 = wfCubed - wfSquared;

            result.X = (((a.X * h1) + (b.X * h2)) + (tangentA.X * h3)) + (tangentB.X * h4);
            result.Y = (((a.Y * h1) + (b.Y * h2)) + (tangentA.Y * h3)) + (tangentB.Y * h4);
        }

        /// <summary>
        /// Compute a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="wf">Weighting factor (between 0 and 1.0)</param>
        /// <returns>Cubic interpolated vector</returns>
        public static Vector2 SmoothStep(Vector2 a, Vector2 b, float wf)
        {
            SmoothStep(ref a, ref b, wf, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Compute a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="wf">Weighting factor (between 0 and 1.0)</param>
        /// <param name="result">Cubic interpolated vector</param>
        public static void SmoothStep(ref Vector2 a, ref Vector2 b, float wf, out Vector2 result)
        {
            float amt = MathHelper.Clamp(wf, 0.0f, 1.0f);
            amt = (amt * amt) * (3.0f - (2.0f * amt));
            result.X = a.X + ((b.X - a.X) * amt);
            result.Y = a.Y + ((b.Y - a.Y) * amt);
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        /// <param name="a">Starting vector</param>
        /// <param name="b">Ending vector</param>
        /// <param name="percent">Amount to interpolate by</param>
        /// <returns>Linear interpolated vector</returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float percent)
        {
            Lerp(ref a, ref b, percent, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        /// <param name="a">Starting vector</param>
        /// <param name="b">Ending vector</param>
        /// <param name="percent">Amount to interpolate by</param>
        /// <param name="result">Linear interpolated vector</param>
        public static void Lerp(ref Vector2 a, ref Vector2 b, float percent, out Vector2 result)
        {
            result.X = a.X + ((b.X - a.X) * percent);
            result.Y = a.Y + ((b.Y - a.Y) * percent);
        }

        /// <summary>
        /// Compute the reflection vector off a surface with the specified normal.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="normal">Surface normal (unit vector)</param>
        /// <returns>Reflected vector</returns>
        public static Vector2 Reflect(Vector2 value, Vector2 normal)
        {
            Reflect(ref value, ref normal, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Compute the reflection vector off a surface with the specified normal.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="normal">Surface normal (unit vector)</param>
        /// <param name="result">Reflected vector</param>
        public static void Reflect(ref Vector2 value, ref Vector2 normal, out Vector2 result)
        {
            float dot = (value.X * normal.X) + (value.Y * normal.Y);
            result.X = value.X - ((2.0f * dot) * normal.X);
            result.Y = value.Y - ((2.0f * dot) * normal.Y);
        }

        /// <summary>
        /// Transfroms the specified vector by the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="m">Transformation matrix</param>
        /// <returns>Transformed vector</returns>
        public static Vector2 Transform(Vector2 value, Matrix4x4 m)
        {
            Transform(ref value, ref m, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Transfroms the specified vector by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="result">Transformed vector</param>
        public static void Transform(ref Vector2 value, ref Matrix4x4 m, out Vector2 result)
        {
            result.X = (value.X * m.M11) + (value.Y * m.M21) + m.M41;
            result.Y = (value.X * m.M12) + (value.Y * m.M22) + m.M42;
        }
        
        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="source">Array of vectors to be transformed</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="destination">Array to store transformed vectors, this may be the same as the source array</param>
        public static void Transform(Vector2[] source, ref Matrix4x4 m, Vector2[] destination)
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
        /// Transfroms the specified vector by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="q">Quaternion rotation</param>
        /// <returns>Transformed vector</returns>
        public static Vector2 Transform(Vector2 value, Quaternion q)
        {
            Transform(ref value, ref q, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Transfroms the specified vector by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="q">Quaternion rotation</param>
        /// <param name="result">Transformed vector</param>
        public static void Transform(ref Vector2 value, ref Quaternion q, out Vector2 result)
        {
            float x2 = q.X + q.X;
            float y2 = q.Y + q.Y;
            float z2 = q.Z + q.Z;

            float wz2 = q.W * z2;

            float xx2 = q.X * x2;
            float xy2 = q.X * y2;

            float yy2 = q.Y * y2;
            float zz2 = q.Z * z2;

            float x = (value.X * ((1.0f - yy2) - zz2)) + (value.Y * (xy2 - wz2));
            float y = (value.X * (xx2 + wz2)) + (value.Y * ((1.0f - xx2) - zz2));

            result.X = x;
            result.Y = y;
        }
        
        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="source">Array of vectors to be transformed</param>
        /// <param name="q">Quaternion rotation</param>
        /// <param name="destination">Array to store transformed vectors, this may be the same as the source array</param>
        public static void Transform(Vector2[] source, ref Quaternion q, Vector2[] destination)
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
        /// Performs a normal transformation using the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="normal">Normal vector to transform</param>
        /// <param name="m">Transformation matrix</param>
        /// <returns>Transformed normal</returns>
        /// <remarks>A normal transform performs the transformation with the assumption that the w component is zero. This causes the fourth
        /// row and fourth column of the matrix to be unused. The end result is a vector that is not translated, but is rotated/scaled. This is preferred for
        /// normalized vectors that act as normals and only represent directions.</remarks>
        public static Vector2 TransformNormal(Vector2 normal, Matrix4x4 m)
        {
            TransformNormal(ref normal, ref m, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Performs a normal transformation using the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="normal">Normal vector to transform</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="result">Transformed normal</param>
        /// <remarks>A normal transform performs the transformation with the assumption that the w component is zero. This causes the fourth
        /// row and fourth column of the matrix to be unused. The end result is a vector that is not translated, but is rotated/scaled. This is preferred for
        /// normalized vectors that act as normals and only represent directions.</remarks>
        public static void TransformNormal(ref Vector2 normal, ref Matrix4x4 m, out Vector2 result)
        {
            result.X = (normal.X * m.M11) + (normal.Y * m.M21);
            result.Y = (normal.X * m.M12) + (normal.Y * m.M22);
        }

        /// <summary>
        /// Performs a normal transformation on an array of normal vectors by the given <see cref="Matrix4x"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="source">Array of normal vectors to be transformed</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="destination">Array to store transformed normal vectors, this may be the same as the source array</param>
        /// <remarks>A normal transform performs the transformation with the assumption that the w component is zero. This causes the fourth
        /// row and fourth column of the matrix to be unused. The end result is a vector that is not translated, but is rotated/scaled. This is preferred for
        /// normalized vectors that act as normals and only represent directions.</remarks>
        public static void TransformNormal(Vector2[] source, ref Matrix4x4 m, Vector2[] destination)
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
                TransformNormal(ref source[i], ref m, out destination[i]);
            }
        }
        
        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="coordinate">Coordinate vector to transform</param>
        /// <param name="m">Transformation matrix</param>
        /// <returns>Transformed coordinate</returns>
        /// <remarks>A coordinate transform performs the transformation with the assumption that the w component is one. The four dimensional vector
        /// obtained from the transformation operation has each component in the vector divided by the w component. This forces the w component to
        /// be one and therefore makes the vector homogeneous. The homogeneous vector is often preferred when working with coordinates as the w component can be safely
        /// ignored.</remarks>
        public static Vector2 TransformCoordinate(Vector2 coordinate, Matrix4x4 m)
        {
            TransformCoordinate(ref coordinate, ref m, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="coordinate">Coordinate vector to transform</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="result">Transformed coordinate</param>
        /// <remarks>A coordinate transform performs the transformation with the assumption that the w component is one. The four dimensional vector
        /// obtained from the transformation operation has each component in the vector divided by the w component. This forces the w component to
        /// be one and therefore makes the vector homogeneous. The homogeneous vector is often preferred when working with coordinates as the w component can be safely
        /// ignored.</remarks>
        public static void TransformCoordinate(ref Vector2 coordinate, ref Matrix4x4 m, out Vector2 result)
        {
            float x = (coordinate.X * m.M11) + (coordinate.Y * m.M21) + m.M41;
            float y = (coordinate.X * m.M12) + (coordinate.Y * m.M22) + m.M42;
            float w = 1.0f / ((coordinate.X * m.M14) + (coordinate.Y * m.M24) + m.M44);

            result.X = x * w;
            result.Y = y * w;
        }

        /// <summary>
        /// Performs a coordinate transformation on an array of coordinate vectors by the given <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="source">Array of coordinate vectors to be transformed</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="destination">Array to store transformed coordinate vectors, this may be the same as the source array</param>
        /// <remarks>A coordinate transform performs the transformation with the assumption that the w component is one. The four dimensional vector
        /// obtained from the transformation operation has each component in the vector divided by the w component. This forces the w component to
        /// be one and therefore makes the vector homogeneous. The homogeneous vector is often preferred when working with coordinates as the w component can be safely
        /// ignored.</remarks>
        public static void TransformCoordinate(Vector2[] source, ref Matrix4x4 m, Vector2[] destination)
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
                TransformCoordinate(ref source[i], ref m, out destination[i]);
            }
        }

        /// <summary>
        /// Orthogonalizes an array of vectors using the modified Gram-Schmidt process. Source and destination should not be the same arrays.
        /// </summary>
        /// <param name="destination">Array to store transformed coordinate vectors</param>
        /// <param name="source">Array of coordinate vectors to be transformed</param>
        /// <remarks>Orthonormalization is the process of making all vectors orthogonal to each other. This means that any given vector will be orthogonal to any other 
        /// given vector in the list. Because this method uses the modified Gram-Schmidt process, the resulting vectors tend to be numerically unstable. 
        /// The numeric stability decreases according to the vectors position in the array, so that the first vector is the most stable and the last vector is the least stable.</remarks>
        public static void Orthogonalize(Vector2[] destination, params Vector2[] source)
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
                Vector2 curr = source[i];
                for (int j = 0; j < i; j++)
                {
                    Vector2 next = destination[j];
                    Dot(ref next, ref curr, out float dotuv);
                    Dot(ref next, ref next, out float dotuu);
                    Multiply(ref next, dotuv / dotuu, out next);
                    Subtract(ref curr, ref next, out curr);
                }

                destination[i] = curr;
            }
        }

        /// <summary>
        /// Orthonormalizes an array of vectors using the modified Gram-Schmidt process. Source and destination should not be the same arrays.
        /// </summary>
        /// <param name="destination">Array to store transformed coordinate vectors</param>
        /// <param name="source">Array of coordinate vectors to be transformed</param>
        /// <remarks>Orthonormalization is the process of making all vectors orthogonal to each other and making all vectors of unit length. This means
        /// that any given vector will be orthogonal to any other given vector in the list. Because this
        /// method uses the modified Gram-Schmidt process, the resulting vectors tend to be numerically unstable. The numeric stability decreases according to the vectors position in
        /// the array, so that the first vector is the most stable and the last vector is the least stable.</remarks>
        public static void Orthonormalize(Vector2[] destination, params Vector2[] source)
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
                Vector2 curr = source[i];
                for (int j = 0; j < i; j++)
                {
                    Vector2 next = destination[j];

                    Dot(ref next, ref curr, out float dotuv);
                    Multiply(ref next, dotuv, out next);
                    Subtract(ref curr, ref next, out curr);
                }

                curr.Normalize();
                destination[i] = curr;
            }
        }

















        /// <summary>
        /// Tests equality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if components are not equal, false otherwise.</returns>
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Adds the two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            Add(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Vector2 operator -(Vector2 value)
        {
            Negate(ref value, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            Subtract(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            Multiply(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Multiplied vector</returns>
        public static Vector2 operator *(Vector2 value, float scale)
        {
            Multiply(ref value, scale, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="scale">Amount to scale</param>
        /// <param name="value">Source vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector2 operator *(float scale, Vector2 value)
        {
            Multiply(ref value, scale, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            Divide(ref a, ref b, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Vector2 operator /(Vector2 value, float divisor)
        {
            Divide(ref value, divisor, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the vector.
        /// </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
        }

        /// <summary>
        /// Compute the length (magnitude) of the vector.
        /// </summary>
        /// <returns>Length</returns>
        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        /// <summary>
        /// Compute the length (magnitude) squared of the vector.
        /// </summary>
        /// <returns>Length squared</returns>
        public float LengthSquared()
        {
            return (X * X) + (Y * Y);
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
                float invLength = 1.0f / (float)Math.Sqrt(lengthSquared);
                X *= invLength;
                Y *= invLength;

                return magnitnude;
            }

            return 0.0f;
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Vector2 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(ref Vector2 other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }
        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(Vector2 other, float tolerance)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(ref Vector2 other, float tolerance)
        {
            return Math.Abs(X - other.X) <= tolerance &&
                   Math.Abs(Y - other.Y) <= tolerance;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                return Equals((Vector2)obj);
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
                return X.GetHashCode() + Y.GetHashCode();
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

            return string.Format(formatProvider, "X: {0} Y: {1}", new object[] { X.ToString(format, formatProvider), Y.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("X", X);
            output.Write("Y", Y);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            X = input.ReadSingle();
            Y = input.ReadSingle();
        }
    }
}
