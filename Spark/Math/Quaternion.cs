namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Core.Interop;
    using Content;

    /// <summary>
    /// Defines a 4 dimensional vector that represents a rotation, where the XYZ components is the axis that an object is rotated about 
    /// by the angle theta. The W component is equal to cos(theta/2.0).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Quaternion : IEquatable<Quaternion>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// X component of the vector part of the quaternion.
        /// </summary>
        public float X;

        /// <summary>
        /// Y component of the vector part of the quaternion.
        /// </summary>
        public float Y;

        /// <summary>
        /// Z component of the vector part of the quaternion.
        /// </summary>
        public float Z;

        /// <summary>
        /// Rotation component of the quaternion.
        /// </summary>
        public float W;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="xyz">Vector part</param>
        /// <param name="w">Rotation component</param>
        public Quaternion(Vector3 xyz, float w)
        {
            X = xyz.X;
            Y = xyz.Y;
            Z = xyz.Z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="xy">Vector part containing XY components</param>
        /// <param name="z">Z component of the vector part</param>
        /// <param name="w">Rotation component</param>
        public Quaternion(Vector3 xy, float z, float w)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new Quaternion.
        /// </summary>
        /// <param name="x">X component of the vector part</param>
        /// <param name="y">Y component of the vector part</param>
        /// <param name="z">Z component of the vector part</param>
        /// <param name="w">Rotation component</param>
        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Gets the identity <see cref="Quaternion"/> set to (0, 0, 0, 1).
        /// </summary>
        public static Quaternion Identity => new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets a <see cref="Quaternion"/> set to (1, 1, 1, 1).
        /// </summary>
        public static Quaternion One => new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Gets the size of the <see cref="Quaternion"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Quaternion>();

        /// <summary>
        /// Gets if the quaternion is equal to the identity quaternion.
        /// </summary>
        public bool IsIdentity => Equals(Identity);

        /// <summary>
        /// Gets whether the quaternion is normalized or not.
        /// </summary>
        public bool IsNormalized => MathHelper.IsApproxEquals(LengthSquared(), 1.0f);

        /// <summary>
        /// Gets whether any of the components of the quaternion are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z) || float.IsNaN(W);

        /// <summary>
        /// Gets whether any of the components of the quaternion are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(X) || float.IsInfinity(Y) || float.IsInfinity(Z) || float.IsInfinity(W);

        /// <summary>
        /// Gets the angle of the quaternion.
        /// </summary>
        public Angle Angle
        {
            get
            {
                float length = (X * X) + (Y * Y) + (Z * Z);
                if (length < MathHelper.ZeroTolerance)
                {
                    return Angle.Zero;
                }

                return Angle.FromRadians((float)(2.0 * Math.Acos(MathHelper.Clamp(W, -1.0f, 1.0f))));
            }
        }

        /// <summary>
        /// Gets the axis of the quaternion.
        /// </summary>
        public Vector3 Axis
        {
            get
            {
                float length = (X * X) + (Y * Y) + (Z * Z);
                if (length < MathHelper.ZeroTolerance)
                {
                    return Vector3.UnitX;
                }

                float invLength = 1.0f / length;                
                return new Vector3(X * invLength, Y * invLength, Z * invLength);
            }
        }

        /// <summary>
        /// Gets or sets individual components of the quaternion in the order that the components are declared (XYZW).
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
        /// Adds two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>Sum of the two quaternions</returns>
        public static Quaternion Add(Quaternion a, Quaternion b)
        {
            Add(ref a, ref b, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <param name="result">Sum of the two quaternions</param>
        public static void Add(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
            result.W = a.W + b.W;
        }

        /// <summary>
        /// Subtracts quaternion b from quaternion a.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>Difference of the two quaternions</returns>
        public static Quaternion Subtract(Quaternion a, Quaternion b)
        {
            Subtract(ref a, ref b, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Subtracts quaternion b from quaternion a.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <param name="result">Difference of the two quaternions</param>
        public static void Subtract(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
            result.W = a.W - b.W;
        }

        /// <summary>
        /// Multiplies two quaternions together.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>Product of the two quaternions</returns>
        public static Quaternion Multiply(Quaternion a, Quaternion b)
        {
            Multiply(ref a, ref b, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Multiplies two quaternions together.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <param name="result">Product of the two quaternions</param>
        public static void Multiply(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            float x1 = a.X;
            float y1 = a.Y;
            float z1 = a.Z;
            float w1 = a.W;

            float x2 = b.X;
            float y2 = b.Y;
            float z2 = b.Z;
            float w2 = b.W;

            float a1 = (y1 * z2) - (z1 * y2);
            float a2 = (z1 * x2) - (x1 * z2);
            float a3 = (x1 * y2) - (y1 * x2);
            float a4 = ((x1 * x2) + (y1 * y2)) + (z1 * z2);

            result.X = ((x1 * w2) + (x2 * w1)) + a1;
            result.Y = ((y1 * w2) + (y2 * w1)) + a2;
            result.Z = ((z1 * w2) + (z2 * w1)) + a3;
            result.W = (w1 * w2) - a4;
        }

        /// <summary>
        /// Multiplies a quaternion by a scalar value.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Scaled quaternion</returns>
        public static Quaternion Multiply(Quaternion value, float scale)
        {
            Multiply(ref value, scale, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Multiplies a quaternion by a scalar value.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <param name="scale">Amount to scale</param>
        /// <param name="result">Scaled quaternion</param>
        public static void Multiply(ref Quaternion value, float scale, out Quaternion result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
            result.Z = value.Z * scale;
            result.W = value.W * scale;
        }

        /// <summary>
        /// Divides quaternion a by quaternion b.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Divisor quaternion</param>
        /// <returns>Quotient of the two quaternions</returns>
        public static Quaternion Divide(Quaternion a, Quaternion b)
        {
            Divide(ref a, ref b, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Divides quaternion a by quaternion b.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Divisor quaternion</param>
        /// <param name="result">Quotient of the two quaternions</param>
        public static void Divide(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            float x1 = a.X;
            float y1 = a.Y;
            float z1 = a.Z;
            float w1 = a.W;

            float x2 = b.X;
            float y2 = b.Y;
            float z2 = b.Z;
            float w2 = b.W;

            float num = (((x2 * x2) + (y2 * y2) + (z2 * z2) + (w2 * w2)));
            float invNum = 1.0f / num;

            float m1 = -x2 * invNum;
            float m2 = -y2 * invNum;
            float m3 = -z2 * invNum;
            float m4 = -w2 * invNum;

            float a1 = (y1 * m3) - (z1 * m2);
            float a2 = (z1 * m1) - (x1 * m3);
            float a3 = (x1 * m2) - (y1 * m1);
            float a4 = ((x1 * m1) + (y1 * m2) + (z1 * m3));

            result.X = ((x1 * m4) + (m1 * w1)) + a1;
            result.Y = ((y1 * m4) + (m2 * w1)) + a2;
            result.Z = ((z1 * m4) + (m3 * w1)) + a3;
            result.W = (w1 * m4) - a4;
        }

        /// <summary>
        /// Reverses the direction of the quaternion where it is facing in the opposite direction.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <returns>Negated quaterion</returns>
        public static Quaternion Negate(Quaternion value)
        {
            Negate(ref value, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Reverses the direction of the quaternion where it is facing in the opposite direction.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <param name="result">Negated quaterion</param>
        public static void Negate(ref Quaternion value, out Quaternion result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
        }

        /// <summary>
        /// Conjugates the quaternion.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <returns>Conjugated quaternion</returns>
        public static Quaternion Conjugate(Quaternion value)
        {
            Conjugate(ref value, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Conjugates the quaternion.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <param name="result">Conjugated quaternion</param>
        public static void Conjugate(ref Quaternion value, out Quaternion result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = value.W;
        }

        /// <summary>
        /// Computes the dot product of two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>Dot product</returns>
        public static float Dot(Quaternion a, Quaternion b)
        {
            Dot(ref a, ref b, out float result);
            return result;
        }

        /// <summary>
        /// Computes the dot product of two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <param name="result">Dot product</param>
        public static void Dot(ref Quaternion a, ref Quaternion b, out float result)
        {
            result = (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z) + (a.W * b.W);
        }

        /// <summary>
        /// Gets the row of a 3x3 rotation matrix as a vector from the quaternion as a normalized rotation vector. The quaternion
        /// is normalized if need be.
        /// </summary>
        /// <param name="rot">Quaternion to get the row vector from</param>
        /// <param name="i">Index of row vector to compute, must be in the range [0, 2]</param>
        /// <returns>Rotation vector</returns>
        public static Vector3 GetRotationVector(Quaternion rot, int i)
        {
            GetRotationVector(ref rot, i, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Gets the row of a 3x3 rotation matrix as a vector from the quaternion as a normalized rotation vector. The quaternion
        /// is normalized if need be.
        /// </summary>
        /// <param name="rot">Quaternion to get the row vector from</param>
        /// <param name="i">Index of row vector to compute, must be in the range [0, 2]</param>
        /// <param name="result">Rotation vector</param>
        public static void GetRotationVector(ref Quaternion rot, int i, out Vector3 result)
        {
            float norm = (rot.W * rot.W) + (rot.X * rot.X) + (rot.Y * rot.Y) + (rot.Z * rot.Z);

            if (norm != 1.0f)
            {
                norm = 1.0f / (float)Math.Sqrt(norm);
            }

            float xnorm = rot.X * norm;
            float ynorm = rot.Y * norm;
            float znorm = rot.Z * norm;
            float wnorm = rot.W * norm;

            float xx = rot.X * xnorm;
            float xy = rot.X * ynorm;
            float xw = rot.X * wnorm;

            float yy = rot.Y * ynorm;
            float yz = rot.Y * znorm;
            float yw = rot.Y * wnorm;

            float zx = rot.Z * xnorm;
            float zz = rot.Z * znorm;
            float zw = rot.Z * wnorm;

            switch (i)
            {
                case 0:
                    result.X = 1.0f - 2.0f * (yy + zz);
                    result.Y = 2.0f * (xy + zw);
                    result.Z = 2.0f * (zx - yw);
                    break;
                case 1:
                    result.X = 2.0f * (xy - zw);
                    result.Y = 1.0f - 2.0f * (xx + zz);
                    result.Z = 2.0f * (yz + xw);
                    break;
                case 2:
                    result.X = 2.0f * (zx + yw);
                    result.Y = 2.0f * (yz - xw);
                    result.Z = 1.0f - 2.0f * (xx + yy);
                    break;
                default:
                    result = Vector3.Zero;
                    break;
            }
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <returns>Inverted quaternion</returns>
        public static Quaternion Invert(Quaternion value)
        {
            Invert(ref value, out Quaternion result);
            return value;
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <param name="result">Inverted quaternion</param>
        public static void Invert(ref Quaternion value, out Quaternion result)
        {
            float lengthSquared = value.LengthSquared();
            result = value;
            if (lengthSquared != 0.0f)
            {
                float invLengthSquared = 1.0f / lengthSquared;

                result.X = -value.X * invLengthSquared;
                result.Y = -value.Y * invLengthSquared;
                result.Z = -value.Z * invLengthSquared;
                result.W *= invLengthSquared;
            }
        }

        /// <summary>
        /// Normalizes the quaternion to a unit quaternion, which results in a quaternion with a magnitude of 1.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <returns>Normalized quaternion</returns>
        public static Quaternion Normalize(Quaternion value)
        {
            Normalize(ref value, out Quaternion result);
            return value;
        }

        /// <summary>
        /// Normalizes the quaternion to a unit quaternion, which results in a quaternion with a magnitude of 1.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <param name="result">Normalized quaternion</param>
        public static void Normalize(ref Quaternion value, out Quaternion result)
        {
            float lengthSquared = value.LengthSquared();
            result = value;
            if (lengthSquared != 0.0f)
            {
                float invLength = 1.0f / (float)Math.Sqrt(lengthSquared);

                result.X *= invLength;
                result.Y *= invLength;
                result.Z *= invLength;
                result.W *= invLength;
            }
        }

        /// <summary>
        /// Linearly interpolates between two quaternions.
        /// </summary>
        /// <param name="start">Starting quaternion</param>
        /// <param name="end">Ending quaternion</param>
        /// <param name="percent">Percent to interpolate between the two quaternions (between 0.0 and 1.0)</param>
        /// <returns>Linearly interpolated quaternion</returns>
        public static Quaternion Lerp(Quaternion start, Quaternion end, float percent)
        {
            Lerp(ref start, ref end, percent, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Linearly interpolates between two quaternions.
        /// </summary>
        /// <param name="start">Starting quaternion</param>
        /// <param name="end">Ending quaternion</param>
        /// <param name="percent">Percent to interpolate between the two quaternions (between 0.0 and 1.0)</param>
        /// <param name="result">Linearly interpolated quaternion</param>
        public static void Lerp(ref Quaternion start, ref Quaternion end, float percent, out Quaternion result)
        {
            percent = MathHelper.Clamp(percent, 0.0f, 1.0f);
            float percent2 = 1.0f - percent;

            Dot(ref start, ref end, out float dot);

            if (dot >= 0.0f)
            {
                result.X = (percent2 * start.X) + (percent * end.X);
                result.Y = (percent2 * start.Y) + (percent * end.Y);
                result.Z = (percent2 * start.Z) + (percent * end.Z);
                result.W = (percent2 * start.W) + (percent * end.W);
            }
            else
            {
                result.X = (percent2 * start.X) - (percent * end.X);
                result.Y = (percent2 * start.Y) - (percent * end.Y);
                result.Z = (percent2 * start.Z) - (percent * end.Z);
                result.W = (percent2 * start.W) - (percent * end.W);
            }

            result.Normalize();
        }

        /// <summary>
        /// Interpolates between two quaternions using spherical linear interpolation.
        /// </summary>
        /// <param name="start">Starting quaternion</param>
        /// <param name="end">Ending quaternion</param>
        /// <param name="percent">Percent to interpolate between the two quaternions (between 0.0 and 1.0)</param>
        /// <returns>Spherical linear interpolated quaternion</returns>
        public static Quaternion Slerp(Quaternion start, Quaternion end, float percent)
        {
            Slerp(ref start, ref end, percent, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Interpolates between two quaternions using spherical linear interpolation.
        /// </summary>
        /// <param name="start">Starting quaternion</param>
        /// <param name="end">Ending quaternion</param>
        /// <param name="percent">Percent to interpolate between the two quaternions (between 0.0 and 1.0)</param>
        /// <param name="result">Spherical linear interpolated quaternion</param>
        public static void Slerp(ref Quaternion start, ref Quaternion end, float percent, out Quaternion result)
        {
            float opposite;
            float inverse;

            percent = MathHelper.Clamp(percent, 0.0f, 1.0f);

            Dot(ref start, ref end, out float dot);

            if (Math.Abs(dot) > 1.0f - MathHelper.ZeroTolerance)
            {
                inverse = 1.0f - percent;
                opposite = percent * Math.Sign(dot);
            }
            else
            {
                float acos = (float)Math.Acos(Math.Abs(dot));
                float invSin = (float)(1.0 / Math.Sin(acos));

                inverse = (float)Math.Sin((1.0f - percent) * acos) * invSin;
                opposite = (float)Math.Sin(percent * acos) * invSin * Math.Sign(dot);
            }

            result.X = (inverse * start.X) + (opposite * end.X);
            result.Y = (inverse * start.Y) + (opposite * end.Y);
            result.Z = (inverse * start.Z) + (opposite * end.Z);
            result.W = (inverse * start.W) + (opposite * end.W);
        }

        /// <summary>
        /// Creates a quaternion from an axis and an angle to rotate about that axis. The axis is assumed to be normalized unit vector.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="axis">Axis to rotate about</param>
        /// <returns>Quaternion rotation</returns>
        public static Quaternion FromAngleAxis(Angle angle, Vector3 axis)
        {
            FromAngleAxis(angle, ref axis, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion from an axis and an angle to rotate about that axis. The axis is assumed to be normalized unit vector.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="axis">Axis to rotate about</param>
        /// <param name="result">Quaternion rotation</param>
        public static void FromAngleAxis(Angle angle, ref Vector3 axis, out Quaternion result)
        {
            float halfAngle = angle.Radians * 0.5f;
            float sin = (float)Math.Sin(halfAngle);
            float cos = (float)Math.Cos(halfAngle);

            result.X = axis.X * sin;
            result.Y = axis.Y * sin;
            result.Z = axis.Z * sin;
            result.W = cos;
        }

        /// <summary>
        /// Creates a quaternion from a matrix that contains a rotation.
        /// </summary>
        /// <param name="m">Source matrix</param>
        /// <returns>Quaternion rotation</returns>
        public static Quaternion FromRotationMatrix(Matrix4x4 m)
        {
            FromRotationMatrix(ref m, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion from a matrix that contains a rotation.
        /// </summary>
        /// <param name="m">Source matrix</param>
        /// <param name="result">Quaternion rotation</param>
        public static void FromRotationMatrix(ref Matrix4x4 m, out Quaternion result)
        {
            float m11 = m.M11;
            float m12 = m.M12;
            float m13 = m.M13;

            float m21 = m.M21;
            float m22 = m.M22;
            float m23 = m.M23;

            float m31 = m.M31;
            float m32 = m.M32;
            float m33 = m.M33;

            float sqrt, half;
            float trace = m11 + m22 + m33;
            if (trace > 0.0f)
            {
                sqrt = (float)Math.Sqrt((trace + 1.0f));
                half = 0.5f * sqrt;
                sqrt = 0.5f / sqrt;

                result.X = (m23 - m32) * sqrt;
                result.Y = (m31 - m13) * sqrt;
                result.Z = (m12 - m21) * sqrt;
                result.W = half;

            }
            else if ((m11 >= m22) && (m11 >= m33))
            {
                sqrt = (float)Math.Sqrt((((1.0f + m11) - m22) - m33));
                half = 0.5f / sqrt;

                result.X = 0.5f * sqrt;
                result.Y = (m12 + m21) * half;
                result.Z = (m13 + m31) * half;
                result.W = (m23 - m32) * half;

            }
            else if (m22 > m33)
            {

                sqrt = (float)Math.Sqrt((((1.0f + m22) - m11) - m33));
                half = 0.5f / sqrt;

                result.X = (m21 + m12) * half;
                result.Y = 0.5f * sqrt;
                result.Z = (m32 + m23) * half;
                result.W = (m31 - m13) * half;

            }
            else
            {
                sqrt = (float)Math.Sqrt((((1.0f + m33) - m11) - m22));
                half = 0.5f / sqrt;

                result.X = (m31 + m13) * half;
                result.Y = (m32 + m23) * half;
                result.Z = 0.5f * sqrt;
                result.W = (m12 - m21) * half;
            }
        }

        /// <summary>
        /// Creates a quaternion from euler angles yaw, pitch, roll.
        /// </summary>
        /// <param name="yaw">Rotation about y-axis</param>
        /// <param name="pitch">Rotation about x-axis</param>
        /// <param name="roll">Rotation about z-axis</param>
        /// <returns>Quaternion rotation</returns>
        public static Quaternion FromEulerAngles(Angle yaw, Angle pitch, Angle roll)
        {
            FromEulerAngles(yaw, pitch, roll, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion from euler angles yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Rotation about y-axis</param>
        /// <param name="pitch">Rotation about x-axis</param>
        /// <param name="roll">Rotation about z-axis</param>
        /// <param name="result">Quaternion rotation</param>
        public static void FromEulerAngles(Angle yaw, Angle pitch, Angle roll, out Quaternion result)
        {
            float angle = roll.Radians * 0.5f;
            float rollSin = (float)Math.Sin(angle);
            float rollCos = (float)Math.Cos(angle);

            angle = pitch.Radians * 0.5f;
            float pitchSin = (float)Math.Sin(angle);
            float pitchCos = (float)Math.Cos(angle);

            angle = yaw.Radians * 0.5f;
            float yawSin = (float)Math.Sin(angle);
            float yawCos = (float)Math.Cos(angle);

            float yawCosXpitchSin = yawCos * pitchSin;
            float yawSinXpitchCos = yawSin * pitchCos;
            float yawCosXpitchCos = yawCos * pitchCos;
            float yawSinXpitchSin = yawSin * pitchSin;

            result.X = (yawCosXpitchSin * rollCos) + (yawSinXpitchCos * rollSin);
            result.Y = (yawSinXpitchCos * rollCos) - (yawCosXpitchSin * rollSin);
            result.Z = (yawCosXpitchCos * rollSin) - (yawSinXpitchSin * rollCos);
            result.W = (yawCosXpitchCos * rollCos) + (yawSinXpitchSin * rollSin);
        }

        /// <summary>
        /// Creates a quaternion that represents a rotation formed by three axes. It is assumed 
        /// the axes are orthogonal to one another and represent a proper right-handed system.
        /// </summary>
        /// <param name="xAxis">X axis of the coordinate system (right)</param>
        /// <param name="yAxis">Y axis of the coordinate system (up)</param>
        /// <param name="zAxis">Z axis of the coordinate system (backward)</param>
        /// <returns>Quaternion rotation</returns>
        public static Quaternion FromAxes(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            FromAxes(ref xAxis, ref yAxis, ref zAxis, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion that represents a rotation formed by three axes. It is assumed 
        /// the axes are orthogonal to one another and represent a proper right-handed system.
        /// </summary>
        /// <param name="xAxis">X axis of the coordinate system (right)</param>
        /// <param name="yAxis">Y axis of the coordinate system (up)</param>
        /// <param name="zAxis">Z axis of the coordinate system (backward)</param>
        /// <param name="result">Quaternion rotation</param>
        public static void FromAxes(ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, out Quaternion result)
        {
            Matrix4x4 m = new Matrix4x4(xAxis.X, xAxis.Y, xAxis.Z, 0, yAxis.X, yAxis.Y, yAxis.Z, 0, zAxis.X, zAxis.Y, zAxis.Z, 0, 0, 0, 0, 1);
            FromRotationMatrix(ref m, out result);
        }

        /// <summary>
        /// Gets the axes that represents this rotation. The axes are orthogonal to one another and represent a proper right-handed system.
        /// </summary>
        /// <param name="rot">Quaternion rotation.</param>
        /// <param name="xAxis">X axis of the coordinate system (right)</param>
        /// <param name="yAxis">Y axis of the coordinate system (up)</param>
        /// <param name="zAxis">Z axis of the coordinate system (backward)</param>
        public static void ToAxes(Quaternion rot, out Vector3 xAxis, out Vector3 yAxis, out Vector3 zAxis)
        {
            ToAxes(ref rot, out xAxis, out yAxis, out zAxis);
        }

        /// <summary>
        /// Gets the axes that represents this rotation. The axes are orthogonal to one another and represent a proper right-handed system.
        /// </summary>
        /// <param name="rot">Quaternion rotation.</param>
        /// <param name="xAxis">X axis of the coordinate system (right)</param>
        /// <param name="yAxis">Y axis of the coordinate system (up)</param>
        /// <param name="zAxis">Z axis of the coordinate system (backward)</param>
        public static void ToAxes(ref Quaternion rot, out Vector3 xAxis, out Vector3 yAxis, out Vector3 zAxis)
        {
            Matrix4x4 m;
            Matrix4x4.FromQuaternion(ref rot, out m);

            xAxis = m.Right;
            yAxis = m.Up;
            zAxis = m.Backward;
        }

        /// <summary>
        /// Creates a rotation quaternion where the object is facing the target along its z axis.
        /// If your object's "forward" facing is down -Z axis, then the object will correctly face the target.
        /// </summary>
        /// <param name="position">Position of object</param>
        /// <param name="target">Position of target</param>
        /// <param name="worldUp">World up vector</param>
        /// <returns>Quaternion rotation</returns>
        public static Quaternion LookAt(Vector3 position, Vector3 target, Vector3 worldUp)
        {
            LookAt(ref position, ref target, ref worldUp, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a rotation quaternion where the object is facing the target along its z axis.
        /// If your object's "forward" facing is down -Z axis, then the object will correctly face the target.
        /// </summary>
        /// <param name="position">Position of object</param>
        /// <param name="target">Position of target</param>
        /// <param name="worldUp">World up vector</param>
        /// <param name="result">Quaternion rotation</param>
        public static void LookAt(ref Vector3 position, ref Vector3 target, ref Vector3 worldUp, out Quaternion result)
        {
            Vector3.Subtract(ref position, ref target, out Vector3 zAxis);
            zAxis.Normalize();
            
            Vector3.Cross(ref worldUp, ref zAxis, out Vector3 worldUpXdir);
            Vector3.Normalize(ref worldUpXdir, out Vector3 xAxis);
            Vector3.Cross(ref zAxis, ref xAxis, out Vector3 yAxis);

            FromAxes(ref xAxis, ref yAxis, ref zAxis, out result);
        }

        /// <summary>
        /// Tests equality between two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public static bool operator ==(Quaternion a, Quaternion b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two quaternions
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>True if components are not equal, false otherwise.</returns>
        public static bool operator !=(Quaternion a, Quaternion b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>Sum of the two quaternions</returns>
        public static Quaternion operator +(Quaternion a, Quaternion b)
        {
            Add(ref a, ref b, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Reverses the direction of the quaternion where it is facing in the opposite direction.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <returns>Negated quaterion</returns>
        public static Quaternion operator -(Quaternion value)
        {
            Negate(ref value, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Subtracts quaternion b from quaternion a.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>Difference of the two quaternions</returns>
        public static Quaternion operator -(Quaternion a, Quaternion b)
        {
            Subtract(ref a, ref b, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Multiplies two quaternions together.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>Multiplied quaternion</returns>
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            Multiply(ref a, ref b, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Multiplies a quaternion by a scalar value.
        /// </summary>
        /// <param name="value">Source quaternion</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Scaled quaternion</returns>
        public static Quaternion operator *(Quaternion value, float scale)
        {
            Multiply(ref value, scale, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Multiplies a quaternion by a scalar value.
        /// </summary>
        /// <param name="scale">Amount to scale</param>
        /// <param name="value">Source quaternion</param>
        /// <returns>Scaled quaternion</returns>
        public static Quaternion operator *(float scale, Quaternion value)
        {
            Multiply(ref value, scale, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Divides quaternion a by quaternion b.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Divisor quaternion</param>
        /// <returns>Quotient of the two quaternions</returns>
        public static Quaternion operator /(Quaternion a, Quaternion b)
        {
            Divide(ref a, ref b, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Decomposes a quaternion into euler angles.
        /// </summary>
        /// <param name="yaw">Rotation about y-axis.</param>
        /// <param name="pitch">Rotation about x-axis.</param>
        /// <param name="roll">Rotation about z-axis.</param>
        public void ToEulerAngles(out Angle yaw, out Angle pitch, out Angle roll)
        {
            float test = (X * Y) + (Z * W);
            if (test > 0.499f)
            {
                // North pole singularity
                yaw = new Angle((float)(2.0 * Math.Atan2(X, W)));
                pitch = new Angle(0.0f);
                roll = new Angle(MathHelper.PiOverTwo);
            }
            else if (test < -0.499f)
            {
                // South pole singularity
                yaw = new Angle((float)(-2.0f * Math.Atan2(X, W)));
                pitch = new Angle(0.0f);
                roll = new Angle(-MathHelper.PiOverTwo);
            }
            else
            {
                float xx = X * X;
                float yy = Y * Y;
                float zz = Z * Z;

                yaw = new Angle((float)Math.Atan2((2.0f * Y * W) - (2.0f * X * Z), 1.0f - (2.0f * yy) - (2.0f * zz)));
                pitch = new Angle((float)Math.Atan2((2.0f * X * W) - (2.0f * Y * Z), 1.0f - (2.0f * xx) - (2.0f * zz)));
                roll = new Angle((float)Math.Asin(2.0f * test));
            }

            if (Math.Abs(yaw.Radians) <= MathHelper.ZeroTolerance)
            {
                yaw = new Angle(0.0f);
            }

            if (Math.Abs(pitch.Radians) <= MathHelper.ZeroTolerance)
            {
                pitch = new Angle(0.0f);
            }

            if (Math.Abs(roll.Radians) <= MathHelper.ZeroTolerance)
            {
                roll = new Angle(0.0f);
            }
        }

        /// <summary>
        /// Reverses the direction of the quaternion where it is facing in the opposite direction.
        /// </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
            W = -W;
        }

        /// <summary>
        /// Conjugates the quaternion.
        /// </summary>
        public void Conjugate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        /// <summary>
        /// Compute the length (magnitude) of the quaternion.
        /// </summary>
        /// <returns>Length</returns>
        public float Length()
        {
            double lengthSquared = LengthSquared();
            return (float)Math.Sqrt(lengthSquared);
        }

        /// <summary>
        /// Compute the length (magnitude) squared of the quaternion.
        /// </summary>
        /// <returns>Length squared</returns>
        public float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z) + (W * W);
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        public void Invert()
        {
            float lengthSquared = LengthSquared();
            if (MathHelper.IsApproxZero(lengthSquared))
            {
                float invSqrt = 1.0f / lengthSquared;
                X = -X * invSqrt;
                Y = -Y * invSqrt;
                Z = -Z * invSqrt;
                W *= invSqrt;
            }
        }

        /// <summary>
        /// Normalizes the quaternion to a unit quaternion, which results in a quaternion with a magnitude of 1.
        /// </summary>
        public void Normalize()
        {
            float lengthSquared = LengthSquared();
            if (lengthSquared != 0.0f)
            {
                float invLength = 1.0f / (float)Math.Sqrt(lengthSquared);
                X *= invLength;
                Y *= invLength;
                Z *= invLength;
                W *= invLength;
            }
        }

        /// <summary>
        /// Tests equality between the quaternion and another quaternion.
        /// </summary>
        /// <param name="other">Quaternion to compare</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the quaternion and another quaternion.
        /// </summary>
        /// <param name="other">Quaternion to compare</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(ref Quaternion other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the quaternion and another quaternion.
        /// </summary>
        /// <param name="other">Quaternion to compare</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(Quaternion other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the quaternion and another quaternion.
        /// </summary>
        /// <param name="other">Quaternion to compare</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(ref Quaternion other, float tolerance)
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
        /// <returns>
        /// true if the specified <see cref="object" /> is equal to this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Quaternion)
            {
                return Equals((Quaternion)obj);
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
