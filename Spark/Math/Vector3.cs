namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a three dimensional vector of 32-bit floats.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector3 : IEquatable<Vector3>, IFormattable, IPrimitiveValue
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
        /// Initializes a new instance of the <see cref="Vector3"/> struct.
        /// </summary>
        /// <param name="value">Value to initialize each component to</param>
        public Vector3(float value)
        {
            X = Y = Z = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> struct.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> struct.
        /// </summary>
        /// <param name="xy">Vector containing XY components</param>
        /// <param name="z">Z component</param>
        public Vector3(Vector2 xy, float z)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> set to (0, 0, 0).
        /// </summary>
        public static Vector3 Zero => new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a <see cref="Vector3"/> set to (1, 1, 1).
        /// </summary>
        public static Vector3 One => new Vector3(1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector3"/> set to (1, 0, 0).
        /// </summary>
        public static Vector3 UnitX => new Vector3(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector3"/> set to (0, 1, 0).
        /// </summary>
        public static Vector3 UnitY => new Vector3(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Gets a unit <see cref="Vector3"/> set to (0, 0, 1).
        /// </summary>
        public static Vector3 UnitZ => new Vector3(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets the <see cref="Vector3"/> set to (0, 1, 0) designating "up" in the right-handed coordinate system.
        /// </summary>
        public static Vector3 Up => new Vector3(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Gets the <see cref="Vector3"/> set to (0, -1, 0) designating "down" in the right-handed coordinate system.
        /// </summary>
        public static Vector3 Down => new Vector3(0.0f, -1.0f, 0.0f);

        /// <summary>
        /// Gets the <see cref="Vector3"/> set to (-1, 0, 0) designating "left" in the right-handed coordinate system.
        /// </summary>
        public static Vector3 Left => new Vector3(-1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets the <see cref="Vector3"/> set to (1, 0, 0) designating "right" in the right-handed coordinate system.
        /// </summary>
        public static Vector3 Right => new Vector3(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Gets the <see cref="Vector3"/> set to (0, 0, -1) designating
        /// "forward" in the right-handed coordinate system.
        /// </summary>
        public static Vector3 Forward => new Vector3(0.0f, 0.0f, -1.0f);

        /// <summary>
        /// Gets the <see cref="Vector3"/> set to (0, 0, 1) designating
        /// "backward" in the right-handed coordinate system.
        /// </summary>
        public static Vector3 Backward => new Vector3(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets the size of the <see cref="Vector3"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Vector3>();

        /// <summary>
        /// Gets whether the vector is normalized or not.
        /// </summary>
        public bool IsNormalized => MathHelper.IsApproxEquals(LengthSquared(), 1.0f);

        /// <summary>
        /// Gets whether any of the components of the vector are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z);

        /// <summary>
        /// Gets whether any of the components of the vector are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(X) || float.IsInfinity(Y) || float.IsInfinity(Z);

        /// <summary>
        /// Gets or sets individual components of the vector in the order that the components are declared (XYZ).
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
        public static Angle AcuteAngleBetween(Vector3 a, Vector3 b)
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
        public static void AcuteAngleBetween(ref Vector3 a, ref Vector3 b, out Angle result)
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
        public static Angle AngleBetween(Vector3 a, Vector3 b)
        {
            AngleBetween(ref a, ref b, out Angle angle);
            return angle;
        }

        /// <summary>
        /// Compute the angle between two vectors in the range of [0, PI]. Assumes that both vectors are already normalized.
        /// </summary>
        /// <param name="a">First unit vector</param>
        /// <param name="b">Second unit vector</param>
        /// <param name="result">Angle between the vectors</param>
        public static void AngleBetween(ref Vector3 a, ref Vector3 b, out Angle result)
        {
            result = new Angle((float)Math.Acos(MathHelper.Clamp(Dot(a, b), -1.0f, 1.0f)));
        }

        /// <summary>
        /// Computes a signed angle between two vectors in the range of [-PI, PI]. This uses a normal to the plane that the source/destination vectors both lie 
        /// on in order to determine orientation. The signed angle is found by taking the cross product of the two vectors [source X dest] and 
        /// computing the dot product with the orientation vector. Assumes that all three vectors are already normalized.
        /// </summary>
        /// <param name="source">Start unit vector</param>
        /// <param name="dest">Destination unit vector</param>
        /// <param name="planeNormal">Normal of the plane that the source and destination vectors both lie upon.</param>
        /// <returns>Signed angle between the vectors</returns>
        public static Angle SignedAngleBetween(Vector3 source, Vector3 dest, Vector3 planeNormal)
        {
            SignedAngleBetween(ref source, ref dest, ref planeNormal, out Angle angle);
            return angle;
        }

        /// <summary>
        /// Computes a signed angle between two vectors in the range of [-PI, PI]. This uses a normal to the plane that the source/destination vectors both lie 
        /// on in order to determine orientation. The signed angle is found by taking the cross product of the two vectors [source X dest] and computing 
        /// the dot product with the orientation vector. Assumes that all three vectors are already normalized.
        /// </summary>
        /// <param name="source">Start unit vector</param>
        /// <param name="dest">Destination unit vector</param>
        /// <param name="planeNormal">Normal of the plane that the source and destination vectors both lie upon.</param>
        /// <param name="signedAngle">Signed angle between the vectors</param>
        public static void SignedAngleBetween(ref Vector3 source, ref Vector3 dest, ref Vector3 planeNormal, out Angle signedAngle)
        {
            float angleBetween = (float)Math.Acos(MathHelper.Clamp(Dot(source, dest), -1.0f, 1.0f));

            Cross(ref source, ref dest, out Vector3 cross);
            Dot(ref cross, ref planeNormal, out float dot);

            // Cross product should either be pointing in same direction as planeNormal or in opposite. If opposite, it will be negative
            signedAngle = new Angle((dot < 0.0f) ? -angleBetween : angleBetween);
        }

        /// <summary>
        /// Computes a signed acute angle between two vectors in the range of [-PI / 2, PI / 2]. This uses a normal to the plane that the source/destination vectors both lie 
        /// on in order to determine orientation. The signed angle is found by taking the cross product of the two vectors [source X dest] and 
        /// computing the dot product with the orientation vector. Assumes that all three vectors are already normalized.
        /// </summary>
        /// <param name="source">Start unit vector</param>
        /// <param name="dest">Destination unit vector</param>
        /// <param name="planeNormal">Normal of the plane that the source and destination vectors both lie upon.</param>
        /// <returns>Signed angle between the vectors</returns>
        public static Angle SignedAcuteAngleBetween(Vector3 source, Vector3 dest, Vector3 planeNormal)
        {
            SignedAcuteAngleBetween(ref source, ref dest, ref planeNormal, out Angle angle);
            return angle;
        }

        /// <summary>
        /// Computes a signed acute angle between two vectors in the range of [-PI / 2, PI / 2]. This uses a normal to the plane that the source/destination vectors both lie 
        /// on in order to determine orientation. The signed angle is found by taking the cross product of the two vectors [source X dest] and 
        /// computing the dot product with the orientation vector. Assumes that all three vectors are already normalized.
        /// </summary>
        /// <param name="source">Start unit vector</param>
        /// <param name="dest">Destination unit vector</param>
        /// <param name="planeNormal">Normal of the plane that the source and destination vectors both lie upon.</param>
        /// <param name="signedAngle">Signed angle between the vectors</param>
        public static void SignedAcuteAngleBetween(ref Vector3 source, ref Vector3 dest, ref Vector3 planeNormal, out Angle signedAngle)
        {
            float angleBetween = (float)Math.Acos(MathHelper.Clamp(Dot(source, dest), -1.0f, 1.0f));
            if (angleBetween > MathHelper.PiOverTwo)
            {
                angleBetween = MathHelper.Pi - angleBetween;
            }

            Cross(ref source, ref dest, out Vector3 cross);
            Dot(ref cross, ref planeNormal, out float dot);

            // Cross product should either be pointing in same direction as planeNormal or in opposite. If opposite, it will be negative
            signedAngle = new Angle((dot < 0.0f) ? -angleBetween : angleBetween);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Vector3 Add(Vector3 a, Vector3 b)
        {
            Add(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Sum of the two vectors</param>
        public static void Add(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Vector3 Subtract(Vector3 a, Vector3 b)
        {
            Subtract(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Difference of the two vectors</param>
        public static void Subtract(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            Multiply(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Multiplied vector</param>
        public static void Multiply(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
        }
        
        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Scaled vector</returns>
        public static Vector3 Multiply(Vector3 value, float scale)
        {
            Multiply(ref value, scale, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiplies components of a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <param name="result">Scaled vector</param>
        public static void Multiply(ref Vector3 value, float scale, out Vector3 result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
            result.Z = value.Z * scale;
        }

        /// <summary>
        /// Multiplies the matrix and the vector. The vector is treated
        /// as a column vector, so the multiplication is M*v.
        /// </summary>
        /// <param name="m">Matrix to multiply.</param>
        /// <param name="value">Vector to multiply.</param>
        /// <returns>Resulting vector</returns>
        public static Vector3 Multiply(Matrix4x4 m, Vector3 value)
        {
            Multiply(ref m, ref value, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiplies the matrix and the vector. The vector is treated
        /// as a column vector, so the multiplication is M*v.
        /// </summary>
        /// <param name="m">Matrix to multiply.</param>
        /// <param name="value">Vector to multiply.</param>
        /// <param name="result">Resulting vector</param>
        public static void Multiply(ref Matrix4x4 m, ref Vector3 value, out Vector3 result)
        {
            result.X = (m.M11 * value.X) + (m.M12 * value.Y) + (m.M13 * value.Z);
            result.Y = (m.M21 * value.X) + (m.M22 * value.Y) + (m.M23 * value.Z);
            result.Z = (m.M31 * value.X) + (m.M32 * value.Y) + (m.M33 * value.Z);
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Vector3 Divide(Vector3 a, Vector3 b)
        {
            Divide(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <param name="result">Quotient of the two vectors</param>
        public static void Divide(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
            result.Z = a.Z / b.Z;
        }

        /// <summary>
        /// Divides the components of a vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Vector3 Divide(Vector3 value, float divisor)
        {
            Divide(ref value, divisor, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Divides the components of a vector by a scalar.
        /// </summary>
        /// <param name="value">First vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <param name="result">Divided vector</param>
        public static void Divide(ref Vector3 value, float divisor, out Vector3 result)
        {
            float invDiv = 1.0f / divisor;
            result.X = value.X * invDiv;
            result.Y = value.Y * invDiv;
            result.Z = value.Z * invDiv;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Vector3 Negate(Vector3 value)
        {
            Negate(ref value, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="result">Negated vector</param>
        public static void Negate(ref Vector3 value, out Vector3 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }
        
        /// <summary>
        /// Compute the dot product between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Dot product</returns>
        /// <remarks>
        /// <para>The dot product is also known as the inner product, where it is equal to:</para>
        /// <para>Dot = Length(Vector1) * Length(Vector2) * Cos(theta)</para>
        /// <para>where theta is the angle between the two vectors. When the two vectors are unit vectors, their length is equal to
        /// one, therefore in this case the dot product is equal to the cosine of theta.</para>
        /// <para>When vectors a and b are unit vectors, then the dot product (not accounting for floating point error) can mean the following:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>If dot &gt; 0, the angle between the two vectors is less than 90 degrees.</description>
        /// </item>
        /// <item>
        /// <description>If dot &lt; 0, the angle between the two vectors is greater than 90 degrees.</description>
        /// </item>
        /// <item>
        /// <description>If dot == 0, the angle between the two vectors is 90 degrees (vectors are orthogonal).</description>
        /// </item>
        /// <item>
        /// <description>If dot == 1, the angle between the two vectors is 0 degrees (vectors are parallel and point in the same direction).</description>
        /// </item>
        /// <item>
        /// <description>If dot == -1, the angle between the two vectors is 180 degrees (vectors are parallel and point in opposite directions).</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static float Dot(Vector3 a, Vector3 b)
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
        /// <remarks>
        /// <para>The dot product is also known as the inner product, where it is equal to:</para>
        /// <para>Dot = Length(Vector1) * Length(Vector2) * Cos(theta)</para>
        /// <para>where theta is the angle between the two vectors. When the two vectors are unit vectors, their length is equal to
        /// one, therefore in this case the dot product is equal to the cosine of theta.</para>
        /// <para>When vectors a and b are unit vectors, then the dot product (not accounting for floating point error) can mean the following:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>If dot &gt; 0, the angle between the two vectors is less than 90 degrees.</description>
        /// </item>
        /// <item>
        /// <description>If dot &lt; 0, the angle between the two vectors is greater than 90 degrees.</description>
        /// </item>
        /// <item>
        /// <description>If dot == 0, the angle between the two vectors is 90 degrees (vectors are orthogonal).</description>
        /// </item>
        /// <item>
        /// <description>If dot == 1, the angle between the two vectors is 0 degrees (vectors are parallel and point in the same direction).</description>
        /// </item>
        /// <item>
        /// <description>If dot == -1, the angle between the two vectors is 180 degrees (vectors are parallel and point in opposite directions).</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static void Dot(ref Vector3 a, ref Vector3 b, out float result)
        {
            result = (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
        }
        
        /// <summary>
        /// Compute the cross product between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Cross product</returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            Cross(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Compute the cross product between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Cross product</param>
        public static void Cross(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = (a.Y * b.Z) - (a.Z * b.Y);
            result.Y = (a.Z * b.X) - (a.X * b.Z);
            result.Z = (a.X * b.Y) - (a.Y * b.X);
        }

        /// <summary>
        /// Compute the cross product between two vectors and normalize the result.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Normalized cross product</returns>
        public static Vector3 NormalizedCross(Vector3 a, Vector3 b)
        {
            NormalizedCross(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Compute the cross product between two vectors and normalize the result.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Normalized cross product</param>
        /// <returns>The magnitude (length) of the vector.</returns>
        public static float NormalizedCross(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = (a.Y * b.Z) - (a.Z * b.Y);
            result.Y = (a.Z * b.X) - (a.X * b.Z);
            result.Z = (a.X * b.Y) - (a.Y * b.X);
            return result.Normalize();
        }
        
        /// <summary>
        /// Normalize the source vector to a unit vector, which results in a vector with a magnitude of 1 but
        /// preserves the direction the vector was pointing in.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Normalized unit vector</returns>
        public static Vector3 Normalize(Vector3 value)
        {
            Normalize(ref value, out value);
            return value;
        }

        /// <summary>
        /// Normalize the source vector to a unit vector, which results in a vector with a magnitude of 1 but
        /// preserves the direction the vector was pointing in.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="result">Normalized unit vector</param>
        /// <returns>The magnitude (length) of the vector.</returns>
        public static float Normalize(ref Vector3 value, out Vector3 result)
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
        public static float Distance(Vector3 start, Vector3 end)
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
        public static void Distance(ref Vector3 start, ref Vector3 end, out float result)
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
        public static float DistanceSquared(Vector3 start, Vector3 end)
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
        public static void DistanceSquared(ref Vector3 start, ref Vector3 end, out float result)
        {
            float dx = start.X - end.X;
            float dy = start.Y - end.Y;
            float dz = start.Z - end.Z;
            result = (dx * dx) + (dy * dy) + (dz * dz);
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Maximum vector</returns>
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            Max(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the maximum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Maximum vector</param>
        public static void Max(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = (a.X > b.X) ? a.X : b.X;
            result.Y = (a.Y > b.Y) ? a.Y : b.Y;
            result.Z = (a.Z > b.Z) ? a.Z : b.Z;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Minimum vector</returns>
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            Max(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Gets the vector that contains the mininum value from each of the components of the 
        /// two supplied vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Minimum vector</param>
        public static void Min(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = (a.X < b.X) ? a.X : b.X;
            result.Y = (a.Y < b.Y) ? a.Y : b.Y;
            result.Z = (a.Z < b.Z) ? a.Z : b.Z;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <returns>Clamped vector</returns>
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            Clamp(ref value, ref min, ref max, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Restricts the source vector in the range of the minimum and maximum vectors.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">Clamped vector</param>
        public static void Clamp(ref Vector3 value, ref Vector3 min, ref Vector3 max, out Vector3 result)
        {
            float x = value.X;
            float y = value.Y;
            float z = value.Z;

            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        /// <summary>
        /// Computes an orthonormal basis from a single vector.
        /// </summary>
        /// <param name="zAxis">Z axis to form an orthonormal basis to.</param>
        /// <param name="xAxis">X axis orthogonal to Z and Y axes.</param>
        /// <param name="yAxis">Y axis orthogonal to X and Z axes.</param>
        public static void ComplementBasis(ref Vector3 zAxis, out Vector3 xAxis, out Vector3 yAxis)
        {
            if (Math.Abs(zAxis.X) >= Math.Abs(zAxis.Y))
            {
                //X or Z of zAxis is largest magnitude component, swap them
                float invLength = 1.0f / (float)Math.Sqrt(zAxis.X * zAxis.X + zAxis.Z * zAxis.Z);

                xAxis.X = -zAxis.Z * invLength;
                xAxis.Y = 0.0f;
                xAxis.Z = zAxis.X * invLength;

                yAxis.X = zAxis.Y * xAxis.Z;
                yAxis.Y = (zAxis.Z * xAxis.X) - (zAxis.X * xAxis.Z);
                yAxis.Z = -zAxis.Y * xAxis.X;
            }
            else
            {
                //Y or Z is the largest magnitude component, swap them
                float invLength = 1.0f / (float)Math.Sqrt(zAxis.Y * zAxis.Y + zAxis.Z * zAxis.Z);

                xAxis.X = 0.0f;
                xAxis.Y = zAxis.Z * invLength;
                xAxis.Z = -zAxis.Y * invLength;

                yAxis.X = (zAxis.Y * xAxis.Z) - (zAxis.Z * xAxis.Y);
                yAxis.Y = -zAxis.X * xAxis.Z;
                yAxis.Z = zAxis.X * xAxis.Y;
            }

            //Ensure axes are normalized
            xAxis.Normalize();
            yAxis.Normalize();
            zAxis.Normalize();
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> containing the 3D Cartesian coordinates of a point specified
        ///  in barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="a">Vector containing 3D cartesian coordinates corresponding to triangle's first vertex</param>
        /// <param name="b">Vector containing 3D cartesian coordinates corresponding to triangle's second vertex</param>
        /// <param name="c">Vector containing 3D cartesian coordinates corresponding to triangle's third vertex</param>
        /// <param name="s">Barycentric coordinate s that is the weighting factor toward the second vertex</param>
        /// <param name="t">Barycentric coordinate t that is the weighting factor toward the third vertex</param>
        /// <returns>Barycentric coordinates</returns>
        public static Vector3 Barycentric(Vector3 a, Vector3 b, Vector3 c, float s, float t)
        {
            Barycentric(ref a, ref b, ref c, s, t, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> containing the 3D Cartesian coordinates of a point specified
        ///  in barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="a">Vector containing 3D cartesian coordinates corresponding to triangle's first vertex</param>
        /// <param name="b">Vector containing 3D cartesian coordinates corresponding to triangle's second vertex</param>
        /// <param name="c">Vector containing 3D cartesian coordinates corresponding to triangle's third vertex</param>
        /// <param name="s">Barycentric coordinate s that is the weighting factor toward the second vertex</param>
        /// <param name="t">Barycentric coordinate t that is the weighting factor toward the third vertex</param>
        /// <param name="result">Barycentric coordinates</param>
        public static void Barycentric(ref Vector3 a, ref Vector3 b, ref Vector3 c, float s, float t, out Vector3 result)
        {
            result.X = (a.X + (s * (b.X - a.X))) + (t * (c.X - a.X));
            result.Y = (a.Y + (s * (b.Y - a.Y))) + (t * (c.Y - a.Y));
            result.Z = (a.Z + (s * (b.Z - a.Z))) + (t * (c.Z - a.Z));
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
        public static Vector3 CatmullRom(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float wf)
        {
            CatmullRom(ref a, ref b, ref c, ref d, wf, out Vector3 result);
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
        public static void CatmullRom(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, float wf, out Vector3 result)
        {
            float wfSquared = wf * wf;
            float wfCubed = wf * wfSquared;
            result.X = 0.5f * ((((2.0f * b.X) + ((-a.X + c.X) * wf)) + (((((2.0f * a.X) - (5.0f * b.X)) + (4.0f * c.X)) - d.X) * wfSquared)) + ((((-a.X + (3.0f * b.X)) - (3.0f * c.X)) + d.X) * wfCubed));
            result.Y = 0.5f * ((((2.0f * b.Y) + ((-a.Y + c.Y) * wf)) + (((((2.0f * a.Y) - (5.0f * b.Y)) + (4.0f * c.Y)) - d.Y) * wfSquared)) + ((((-a.Y + (3.0f * b.Y)) - (3.0f * c.Y)) + d.Y) * wfCubed));
            result.Z = 0.5f * ((((2.0f * b.Z) + ((-a.Z + c.Z) * wf)) + (((((2.0f * a.Z) - (5.0f * b.Z)) + (4.0f * c.Z)) - d.Z) * wfSquared)) + ((((-a.Z + (3.0f * b.Z)) - (3.0f * c.Z)) + d.Z) * wfCubed));
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
        public static Vector3 Hermite(Vector3 a, Vector3 tangentA, Vector3 b, Vector3 tangentB, float wf)
        {
            Hermite(ref a, ref tangentA, ref b, ref tangentB, wf, out Vector3 result);
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
        public static void Hermite(ref Vector3 a, ref Vector3 tangentA, ref Vector3 b, ref Vector3 tangentB, float wf, out Vector3 result)
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
        }
        
        /// <summary>
        /// Compute a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="wf">Weighting factor (between 0 and 1.0)</param>
        /// <returns>Cubic interpolated vector</returns>
        public static Vector3 SmoothStep(Vector3 a, Vector3 b, float wf)
        {
            SmoothStep(ref a, ref b, wf, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Compute a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="wf">Weighting factor (between 0 and 1.0)</param>
        /// <param name="result">Cubic interpolated vector</param>
        public static void SmoothStep(ref Vector3 a, ref Vector3 b, float wf, out Vector3 result)
        {
            float amt = MathHelper.Clamp(wf, 0.0f, 1.0f);
            amt = (amt * amt) * (3.0f - (2.0f * amt));
            result.X = a.X + ((b.X - a.X) * amt);
            result.Y = a.Y + ((b.Y - a.Y) * amt);
            result.Z = a.Z + ((b.Z - a.Z) * amt);
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        /// <param name="a">Starting vector</param>
        /// <param name="b">Ending vector</param>
        /// <param name="percent">Amount to interpolate by</param>
        /// <returns>Linear interpolated vector</returns>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float percent)
        {
            Lerp(ref a, ref b, percent, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        /// <param name="a">Starting vector</param>
        /// <param name="b">Ending vector</param>
        /// <param name="percent">Amount to interpolate by</param>
        /// <param name="result">Linear interpolated vector</param>
        public static void Lerp(ref Vector3 a, ref Vector3 b, float percent, out Vector3 result)
        {
            result.X = a.X + ((b.X - a.X) * percent);
            result.Y = a.Y + ((b.Y - a.Y) * percent);
            result.Z = a.Z + ((b.Z - a.Z) * percent);
        }

        /// <summary>
        /// Compute the reflection vector off a surface with the specified normal.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="normal">Surface normal (unit vector)</param>
        /// <returns>Reflected vector</returns>
        public static Vector3 Reflect(Vector3 value, Vector3 normal)
        {
            Reflect(ref value, ref normal, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Compute the reflection vector off a surface with the specified normal.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="normal">Surface normal (unit vector)</param>
        /// <param name="result">Reflected vector</param>
        public static void Reflect(ref Vector3 value, ref Vector3 normal, out Vector3 result)
        {
            float dot = (value.X * normal.X) + (value.Y * normal.Y) + (value.Z * normal.Z);
            result.X = value.X - ((2.0f * dot) * normal.X);
            result.Y = value.Y - ((2.0f * dot) * normal.Y);
            result.Z = value.Z - ((2.0f * dot) * normal.Z);
        }

        /// <summary>
        /// Transfroms the specified vector by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="m">Transformation matrix</param>
        /// <returns>Transformed vector</returns>
        public static Vector3 Transform(Vector3 value, Matrix4x4 m)
        {
            Transform(ref value, ref m, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Transfroms the specified vector by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="result">Transformed vector</param>
        public static void Transform(ref Vector3 value, ref Matrix4x4 m, out Vector3 result)
        {
            result.X = (value.X * m.M11) + (value.Y * m.M21) + (value.Z * m.M31) + m.M41;
            result.Y = (value.X * m.M12) + (value.Y * m.M22) + (value.Z * m.M32) + m.M42;
            result.Z = (value.X * m.M13) + (value.Y * m.M23) + (value.Z * m.M33) + m.M43;
        }

        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="source">Array of vectors to be transformed</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="destination">Array to store transformed vectors, this may be the same as the source array</param>
        public static void Transform(Vector3[] source, ref Matrix4x4 m, Vector3[] destination)
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
        public static Vector3 Transform(Vector3 value, Quaternion q)
        {
            Transform(ref value, ref q, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Transfroms the specified vector by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="q">Quaternion rotation</param>
        /// <param name="result">Transformed vector</param>
        public static void Transform(ref Vector3 value, ref Quaternion q, out Vector3 result)
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
        }

        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="source">Array of vectors to be transformed</param>
        /// <param name="q">Quaternion rotation</param>
        /// <param name="destination">Array to store transformed vectors, this may be the same as the source array</param>
        public static void Transform(Vector3[] source, ref Quaternion q, Vector3[] destination)
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
        public static Vector3 TransformNormal(Vector3 normal, Matrix4x4 m)
        {
            TransformNormal(ref normal, ref m, out Vector3 result);
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
        public static void TransformNormal(ref Vector3 normal, ref Matrix4x4 m, out Vector3 result)
        {
            float x = (normal.X * m.M11) + (normal.Y * m.M21) + (normal.Z * m.M31);
            float y = (normal.X * m.M12) + (normal.Y * m.M22) + (normal.Z * m.M32);
            float z = (normal.X * m.M13) + (normal.Y * m.M23) + (normal.Z * m.M33);

            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        /// <summary>
        /// Performs a normal transformation on an array of normal vectors by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="source">Array of normal vectors to be transformed</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="destination">Array to store transformed normal vectors, this may be the same as the source array</param>
        /// <remarks>A normal transform performs the transformation with the assumption that the w component is zero. This causes the fourth
        /// row and fourth column of the matrix to be unused. The end result is a vector that is not translated, but is rotated/scaled. This is preferred for
        /// normalized vectors that act as normals and only represent directions.</remarks>
        public static void TransformNormal(Vector3[] source, ref Matrix4x4 m, Vector3[] destination)
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
        /// Performs a coordinate transformation using the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="coordinate">Coordinate vector to transform</param>
        /// <param name="m">Transformation matrix</param>
        /// <returns>Transformed coordinate</returns>
        /// <remarks>A coordinate transform performs the transformation with the assumption that the w component is one. The four dimensional vector
        /// obtained from the transformation operation has each component in the vector divided by the w component. This forces the w component to
        /// be one and therefore makes the vector homogeneous. The homogeneous vector is often preferred when working with coordinates as the w component can be safely
        /// ignored.</remarks>
        public static Vector3 TransformCoordinate(Vector3 coordinate, Matrix4x4 m)
        {
            TransformCoordinate(ref coordinate, ref m, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="coordinate">Coordinate vector to transform</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="result">Transformed coordinate</param>
        /// <remarks>A coordinate transform performs the transformation with the assumption that the w component is one. The four dimensional vector
        /// obtained from the transformation operation has each component in the vector divided by the w component. This forces the w component to
        /// be one and therefore makes the vector homogeneous. The homogeneous vector is often preferred when working with coordinates as the w component can be safely
        /// ignored.</remarks>
        public static void TransformCoordinate(ref Vector3 coordinate, ref Matrix4x4 m, out Vector3 result)
        {
            float x = (coordinate.X * m.M11) + (coordinate.Y * m.M21) + (coordinate.Z * m.M31) + m.M41;
            float y = (coordinate.X * m.M12) + (coordinate.Y * m.M22) + (coordinate.Z * m.M32) + m.M42;
            float z = (coordinate.X * m.M13) + (coordinate.Y * m.M23) + (coordinate.Z * m.M33) + m.M43;
            float w = 1.0f / ((coordinate.X * m.M14) + (coordinate.Y * m.M24) + (coordinate.Z * m.M34) + m.M44);

            result.X = x * w;
            result.Y = y * w;
            result.Z = z * w;
        }

        /// <summary>
        /// Performs a coordinate transformation on an array of coordinate vectors by the given <see cref="Matrix4x4"/>. The vector is treated
        /// as a row vector, so the multiplication is v*M.
        /// </summary>
        /// <param name="source">Array of coordinate vectors to be transformed</param>
        /// <param name="m">Transformation matrix</param>
        /// <param name="destination">Array to store transformed coordinate vectors, this may be the same as the source array</param>
        /// <remarks>A coordinate transform performs the transformation with the assumption that the w component is one. The four dimensional vector
        /// obtained from the transformation operation has each component in the vector divided by the w component. This forces the w component to
        /// be one and therefore makes the vector homogeneous. The homogeneous vector is often preferred when working with coordinates as the w component can be safely
        /// ignored.</remarks>
        public static void TransformCoordinate(Vector3[] source, ref Matrix4x4 m, Vector3[] destination)
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
        public static void Orthogonalize(Vector3[] destination, params Vector3[] source)
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
                Vector3 curr = source[i];
                for (int j = 0; j < i; j++)
                {
                    Vector3 next = destination[j];

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
        public static void Orthonormalize(Vector3[] destination, params Vector3[] source)
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
                Vector3 curr = source[i];
                for (int j = 0; j < i; j++)
                {
                    Vector3 next = destination[j];

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
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two vectors.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if components are not equal, false otherwise.</returns>
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Adds the two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of the two vectors</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            Add(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of the components of the specified vector.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <returns>Negated vector</returns>
        public static Vector3 operator -(Vector3 value)
        {
            Negate(ref value, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Subtracts vector b from vector a.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of the two vectors</returns>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            Subtract(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            Multiply(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Multiplied vector</returns>
        public static Vector3 operator *(Vector3 value, float scale)
        {
            Multiply(ref value, scale, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scaling factor.
        /// </summary>
        /// <param name="scale">Amount to scale</param>
        /// <param name="value">Source vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector3 operator *(float scale, Vector3 value)
        {
            Multiply(ref value, scale, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Divides the components of vector a by those of vector b.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Divisor vector</param>
        /// <returns>Quotient of the two vectors</returns>
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            Divide(ref a, ref b, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided vector</returns>
        public static Vector3 operator /(Vector3 value, float divisor)
        {
            Divide(ref value, divisor, out Vector3 result);
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
            return (X * X) + (Y * Y) + (Z * Z);
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

                return magnitnude;
            }

            return 0.0f;
        }
        
        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Vector3 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(ref Vector3 other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(Vector3 other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the vector and another vector.
        /// </summary>
        /// <param name="other">Vector to test against</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(ref Vector3 other, float tolerance)
        {
            return (Math.Abs(X - other.X) <= tolerance) && 
                   (Math.Abs(Y - other.Y) <= tolerance) &&
                   (Math.Abs(Z - other.Z) <= tolerance);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                return Equals((Vector3)obj);
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
                return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
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

            return string.Format(formatProvider, "X: {0} Y: {1} Z: {2}", new object[] { X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider) });
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
        }
    }
}
