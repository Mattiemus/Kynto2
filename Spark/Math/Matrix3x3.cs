namespace Spark.Math
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x3
    {
        public float M11;
        public float M12;
        public float M13;
        public float M21;
        public float M22;
        public float M23;
        public float M31;
        public float M32;
        public float M33;

        public Matrix3x3(float m11, float m12, float m13,
                         float m21, float m22, float m23,
                         float m31, float m32, float m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        public static Matrix3x3 Identity => new Matrix3x3(1.0f, 0.0f, 0.0f,
                                                          0.0f, 1.0f, 0.0f,
                                                          0.0f, 0.0f, 1.0f);

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return M11;
                    case 1:
                        return M12;
                    case 2:
                        return M13;
                    case 3:
                        return M21;
                    case 4:
                        return M22;
                    case 5:
                        return M23;
                    case 6:
                        return M31;
                    case 7:
                        return M32;
                    case 8:
                        return M33;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        M11 = value;
                        break;
                    case 1:
                        M12 = value;
                        break;
                    case 2:
                        M13 = value;
                        break;
                    case 3:
                        M21 = value;
                        break;
                    case 4:
                        M22 = value;
                        break;
                    case 5:
                        M23 = value;
                        break;
                    case 6:
                        M31 = value;
                        break;
                    case 7:
                        M32 = value;
                        break;
                    case 8:
                        M33 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
        }

        public float this[int row, int column]
        {
            get => this[(row * 3) + column];
            set => this[(row * 3) + column] = value;
        }

        public static void Multiply(ref Matrix3x3 a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            float m11 = (((a.M11 * b.M11) + (a.M12 * b.M21)) + (a.M13 * b.M31));
            float m12 = (((a.M11 * b.M12) + (a.M12 * b.M22)) + (a.M13 * b.M32));
            float m13 = (((a.M11 * b.M13) + (a.M12 * b.M23)) + (a.M13 * b.M33));

            float m21 = (((a.M21 * b.M11) + (a.M22 * b.M21)) + (a.M23 * b.M31));
            float m22 = (((a.M21 * b.M12) + (a.M22 * b.M22)) + (a.M23 * b.M32));
            float m23 = (((a.M21 * b.M13) + (a.M22 * b.M23)) + (a.M23 * b.M33));

            float m31 = (((a.M31 * b.M11) + (a.M32 * b.M21)) + (a.M33 * b.M31));
            float m32 = (((a.M31 * b.M12) + (a.M32 * b.M22)) + (a.M33 * b.M32));
            float m33 = (((a.M31 * b.M13) + (a.M32 * b.M23)) + (a.M33 * b.M33));

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        public void Transpose()
        {
            float m11 = M11;
            float m12 = M12;
            float m13 = M13;
            float m21 = M21;
            float m22 = M22;
            float m23 = M23;
            float m31 = M31;
            float m32 = M32;
            float m33 = M33;

            M11 = m11;
            M12 = m21;
            M13 = m31;

            M21 = m12;
            M22 = m22;
            M23 = m32;

            M31 = m13;
            M32 = m23;
            M33 = m33;
        }
    }
}
