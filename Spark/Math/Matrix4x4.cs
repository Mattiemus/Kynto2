namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    using Content;

    /// <summary>
    /// Defines a 4x4 row-vector Matrix. Memory layout is row major.
    /// </summary>
    /// <remarks>
    /// Matrix functions exhibit the following behaviors by default:
    /// <list type="bullet">
    /// <item>
    /// <description>Right handedness conventions used. By default, +X to the right (row 1), +Y up (row 2), and -Z (row 3).</description>
    /// </item>
    /// <item>
    /// <description>Matrix multiplication order is local space on the left and world target on the right (e.g. SRT - scale * rotation * translation, or WVP - world * view * projection)</description>
    /// </item>
    /// <item>
    /// <description>The vector is the left operand of the transform, as represented by the transform methods in the Vector2/3/4 structures.</description>
    /// </item>
    /// <item>
    /// <description>Vector transforms are dot products between the vector and the column elements of the matrix.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Matrix4x4 : IEquatable<Matrix4x4>, IFormattable, IPrimitiveValue
    {
        /// <summary>
        /// Value at row 1, column 1 of the matrix
        /// </summary>
        public float M11;

        /// <summary>
        /// Value at row 1, column 2 of the matrix
        /// </summary>
        public float M12;

        /// <summary>
        /// Value at row 1, column 3 of the matrix
        /// </summary>
        public float M13;

        /// <summary>
        /// Value at row 1, column 4 of the matrix
        /// </summary>
        public float M14;

        /// <summary>
        /// Value at row 2, column 1 of the matrix
        /// </summary>
        public float M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix
        /// </summary>
        public float M22;

        /// <summary>
        /// Value at row 2, column 3 of the matrix
        /// </summary>
        public float M23;

        /// <summary>
        /// Value at row 2, column 4 of the matrix
        /// </summary>
        public float M24;

        /// <summary>
        /// Value at row 3, column 1 of the matrix
        /// </summary>
        public float M31;

        /// <summary>
        /// Value at row 3, column 2 of the matrix
        /// </summary>
        public float M32;

        /// <summary>
        /// Value at row 3, column 3 of the matrix
        /// </summary>
        public float M33;

        /// <summary>
        /// Value at row 3, column 4 of the matrix
        /// </summary>
        public float M34;

        /// <summary>
        /// Value at row 4, column 1 of the matrix
        /// </summary>
        public float M41;

        /// <summary>
        /// Value at row 4, column 2 of the matrix
        /// </summary>
        public float M42;

        /// <summary>
        /// Value at row 4, column 3 of the matrix
        /// </summary>
        public float M43;

        /// <summary>
        /// Value at row 4, column 4 of the matrix
        /// </summary>
        public float M44;

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4x4"/> struct.
        /// </summary>
        /// <param name="value">The value assigned to all matrix components</param>
        public Matrix4x4(float value)
        {
            M11 = M12 = M13 = M14 = M21 = M22 = M23 = M24 = M31 = M32 = M33 = M34 = M41 = M42 = M43 = M44 = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4x4"/> struct.
        /// </summary>
        /// <param name="row1">First row</param>
        /// <param name="row2">Second row</param>
        /// <param name="row3">Third row</param>
        /// <param name="row4">Fourth row</param>
        public Matrix4x4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4)
        {
            M11 = row1.X;
            M12 = row1.Y;
            M13 = row1.Z;
            M14 = row1.W;

            M21 = row2.X;
            M22 = row2.Y;
            M23 = row2.Z;
            M24 = row3.W;

            M31 = row3.X;
            M32 = row3.Y;
            M33 = row3.Z;
            M34 = row3.W;

            M41 = row4.X;
            M42 = row4.Y;
            M43 = row4.Z;
            M44 = row4.W;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4x4"/> struct.
        /// </summary>
        /// <param name="m11">Element at row 1, column 1</param>
        /// <param name="m12">Element at row 1, column 2</param>
        /// <param name="m13">Element at row 1, column 3</param>
        /// <param name="m14">Element at row 1, column 4</param>
        /// <param name="m21">Element at row 2, column 1</param>
        /// <param name="m22">Element at row 2, column 2</param>
        /// <param name="m23">Element at row 2, column 3</param>
        /// <param name="m24">Element at row 2, column 4</param>
        /// <param name="m31">Element at row 3, column 1</param>
        /// <param name="m32">Element at row 3, column 2</param>
        /// <param name="m33">Element at row 3, column 3</param>
        /// <param name="m34">Element at row 3, column 4</param>
        /// <param name="m41">Element at row 4, column 1</param>
        /// <param name="m42">Element at row 4, column 2</param>
        /// <param name="m43">Element at row 4, column 3</param>
        /// <param name="m44">Element at row 4, column 4</param>
        public Matrix4x4(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;

            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;

            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;

            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        /// <summary>
        /// Get the identity matrix.
        /// </summary>
        public static Matrix4x4 Identity => new Matrix4x4(1.0f, 0.0f, 0.0f, 0.0f,
                                                          0.0f, 1.0f, 0.0f, 0.0f, 
                                                          0.0f, 0.0f, 1.0f, 0.0f, 
                                                          0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets the size of the <see cref="Matrix4x4"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Matrix4x4>();

        /// <summary>
        /// Gets if this matrix is equal to the identity matrix.
        /// </summary>
        public bool IsIdentity => Equals(Identity);

        /// <summary>
        /// Gets whether any of the components of the matrix are NaN (Not A Number).
        /// </summary>
        public bool IsNaN => float.IsNaN(M11) || float.IsNaN(M12) || float.IsNaN(M13) || float.IsNaN(M14) ||
                             float.IsNaN(M21) || float.IsNaN(M22) || float.IsNaN(M23) || float.IsNaN(M24) ||
                             float.IsNaN(M31) || float.IsNaN(M32) || float.IsNaN(M33) || float.IsNaN(M34) ||
                             float.IsNaN(M41) || float.IsNaN(M42) || float.IsNaN(M43) || float.IsNaN(M44);

        /// <summary>
        /// Gets whether any of the components of the matrix are positive or negative infinity.
        /// </summary>
        public bool IsInfinity => float.IsInfinity(M11) || float.IsInfinity(M12) || float.IsInfinity(M13) || float.IsInfinity(M14) ||
                                  float.IsInfinity(M21) || float.IsInfinity(M22) || float.IsInfinity(M23) || float.IsInfinity(M24) ||
                                  float.IsInfinity(M31) || float.IsInfinity(M32) || float.IsInfinity(M33) || float.IsInfinity(M34) ||
                                  float.IsInfinity(M41) || float.IsInfinity(M42) || float.IsInfinity(M43) || float.IsInfinity(M44);

        /// <summary>
        /// Gets the axes that represent the orientation of the matrix.
        /// </summary>
        public Triad Axes
        {
            get
            {
                Triad axes;
                axes.XAxis = Right;
                axes.YAxis = Up;
                axes.ZAxis = Backward;

                return axes;
            }
        }

        /// <summary>
        /// Gets or sets the translation vector (M41, M42, M43) of the matrix.
        /// </summary>
        public Vector3 Translation
        {
            get
            {
                Vector3 v;
                v.X = M41;
                v.Y = M42;
                v.Z = M43;
                return v;
            }
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the up vector (M21, M22, M23) of the matrix.
        /// </summary>
        public Vector3 Up
        {
            get
            {
                Vector3 v;
                v.X = M21;
                v.Y = M22;
                v.Z = M23;
                return v;
            }
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the down vector of the matrix (negation of Up vector).
        /// Setting will negate component values.
        /// </summary>
        public Vector3 Down
        {
            get
            {
                Vector3 v;
                v.X = -M21;
                v.Y = -M22;
                v.Z = -M23;
                return v;
            }
            set
            {
                M21 = -value.X;
                M22 = -value.Y;
                M23 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the backward vector (M31, M32, M33) of the matrix.
        /// </summary>
        public Vector3 Backward
        {
            get
            {
                Vector3 v;
                v.X = M31;
                v.Y = M32;
                v.Z = M33;
                return v;
            }
            set
            {
                M31 = value.X;
                M32 = value.Y;
                M33 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the forward vector of the matrix (negation of backward vector).
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                Vector3 v;
                v.X = -M31;
                v.Y = -M32;
                v.Z = -M33;
                return v;
            }
            set
            {
                M31 = -value.X;
                M32 = -value.Y;
                M33 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the right vector (M11, M12, M13) of the matrix.
        /// </summary>
        public Vector3 Right
        {
            get
            {
                Vector3 v;
                v.X = M11;
                v.Y = M12;
                v.Z = M13;
                return v;
            }
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the left vector of the matrix (negation of right vector).
        /// </summary>
        public Vector3 Left
        {
            get
            {
                Vector3 v;
                v.X = -M11;
                v.Y = -M12;
                v.Z = -M13;
                return v;
            }
            set
            {
                M11 = -value.X;
                M12 = -value.Y;
                M13 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the scale of the matrix (M11, M22, M33).
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                Vector3 v;
                v.X = M11;
                v.Y = M22;
                v.Z = M33;
                return v;
            }
            set
            {
                M11 = value.X;
                M22 = value.Y;
                M33 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the first row of the matrix (M11, M12, M13, M14).
        /// </summary>
        public Vector4 Row1
        {
            get
            {
                Vector4 row;
                row.X = M11;
                row.Y = M12;
                row.Z = M13;
                row.W = M14;

                return row;
            }
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
                M14 = value.W;
            }
        }

        /// <summary>
        /// Gets or sets the second row of the matrix (M21, M22, M23, M24).
        /// </summary>
        public Vector4 Row2
        {
            get
            {
                Vector4 row;
                row.X = M21;
                row.Y = M22;
                row.Z = M23;
                row.W = M24;

                return row;
            }
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
                M24 = value.W;
            }
        }

        /// <summary>
        /// Gets or sets the third row of the matrix (M31, M32, M33, M34).
        /// </summary>
        public Vector4 Row3
        {
            get
            {
                Vector4 row;
                row.X = M31;
                row.Y = M32;
                row.Z = M33;
                row.W = M34;

                return row;
            }
            set
            {
                M31 = value.X;
                M32 = value.Y;
                M33 = value.Z;
                M34 = value.W;
            }
        }

        /// <summary>
        /// Gets or sets the first row of the matrix (M41, M42, M43, M44).
        /// </summary>
        public Vector4 Row4
        {
            get
            {
                Vector4 row;
                row.X = M41;
                row.Y = M42;
                row.Z = M43;
                row.W = M44;

                return row;
            }
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
                M44 = value.W;
            }
        }

        /// <summary>
        /// Gets or sets the first column of the matrix (M11, M21, M31, M41).
        /// </summary>
        public Vector4 Column1
        {
            get
            {
                Vector4 col;
                col.X = M11;
                col.Y = M21;
                col.Z = M31;
                col.W = M41;

                return col;
            }
            set
            {
                M11 = value.X;
                M21 = value.Y;
                M31 = value.Z;
                M41 = value.W;
            }
        }

        /// <summary>
        /// Gets or sets the second column of the matrix (M12, M22, M32, M42).
        /// </summary>
        public Vector4 Column2
        {
            get
            {
                Vector4 col;
                col.X = M12;
                col.Y = M22;
                col.Z = M32;
                col.W = M42;

                return col;
            }
            set
            {
                M12 = value.X;
                M22 = value.Y;
                M32 = value.Z;
                M42 = value.W;
            }
        }

        /// <summary>
        /// Gets or sets the third column of the matrix (M13, M23, M33, M43).
        /// </summary>
        public Vector4 Column3
        {
            get
            {
                Vector4 col;
                col.X = M13;
                col.Y = M23;
                col.Z = M33;
                col.W = M43;

                return col;
            }
            set
            {
                M13 = value.X;
                M23 = value.Y;
                M33 = value.Z;
                M43 = value.W;
            }
        }

        /// <summary>
        /// Gets or sets the fourth column of the matrix (M14, M24, M34, M44).
        /// </summary>
        public Vector4 Column4
        {
            get
            {
                Vector4 col;
                col.X = M14;
                col.Y = M24;
                col.Z = M34;
                col.W = M44;

                return col;
            }
            set
            {
                M14 = value.X;
                M24 = value.Y;
                M34 = value.Z;
                M44 = value.W;
            }
        }

        /// <summary>
        /// Gets or sets the matrix component at the specified index. Indices follow memory layout (first row accessed by first four indices, second row by the next four, etc).
        /// </summary>
        /// <param name="index">Zero-based index of the component between [0, 15].</param>
        /// <returns>The value at the index</returns>
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
                        return M14;
                    case 4:
                        return M21;
                    case 5:
                        return M22;
                    case 6:
                        return M23;
                    case 7:
                        return M24;
                    case 8:
                        return M31;
                    case 9:
                        return M32;
                    case 10:
                        return M33;
                    case 11:
                        return M34;
                    case 12:
                        return M41;
                    case 13:
                        return M42;
                    case 14:
                        return M43;
                    case 15:
                        return M44;
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
                        M14 = value;
                        break;
                    case 4:
                        M21 = value;
                        break;
                    case 5:
                        M22 = value;
                        break;
                    case 6:
                        M23 = value;
                        break;
                    case 7:
                        M24 = value;
                        break;
                    case 8:
                        M31 = value;
                        break;
                    case 9:
                        M32 = value;
                        break;
                    case 10:
                        M33 = value;
                        break;
                    case 11:
                        M34 = value;
                        break;
                    case 12:
                        M41 = value;
                        break;
                    case 13:
                        M42 = value;
                        break;
                    case 14:
                        M43 = value;
                        break;
                    case 15:
                        M44 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
        }

        /// <summary>
        /// Gets or sets an individual component based on its index in the matrix. The indices are zero-based, so
        /// the first element is located at (0,0) and the last element at (3,3). 
        /// </summary>
        /// <param name="row">Zero-based row index valid between [0, 3]</param>
        /// <param name="column">Zerp-based column index valid between [0, 3]</param>
        /// <returns>The matrix value</returns>
        public float this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(row), "Index is out of range");
                }

                if (column < 0 || column > 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(column), "Index is out of range");
                }

                return this[(row * 4) + column];
            }
            set
            {
                if (row < 0 || row > 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(row), "Index is out of range");
                }

                if (column < 0 || column > 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(column), "Index is out of range");
                }

                this[(row * 4) + column] = value;
            }
        }
        
        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>Sum of the two matrices</returns>
        public static Matrix4x4 Add(Matrix4x4 a, Matrix4x4 b)
        {
            Add(ref a, ref b, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <param name="result">Sum of the two matrices</param>
        public static void Add(ref Matrix4x4 a, ref Matrix4x4 b, out Matrix4x4 result)
        {
            result.M11 = a.M11 + b.M11;
            result.M12 = a.M12 + b.M12;
            result.M13 = a.M13 + b.M13;
            result.M14 = a.M14 + b.M14;

            result.M21 = a.M21 + b.M21;
            result.M22 = a.M22 + b.M22;
            result.M23 = a.M23 + b.M23;
            result.M24 = a.M24 + b.M24;

            result.M31 = a.M31 + b.M31;
            result.M32 = a.M32 + b.M32;
            result.M33 = a.M33 + b.M33;
            result.M34 = a.M34 + b.M34;

            result.M41 = a.M41 + b.M41;
            result.M42 = a.M42 + b.M42;
            result.M43 = a.M43 + b.M43;
            result.M44 = a.M44 + b.M44;
        }

        /// <summary>
        /// Subtracts matrix b from matrix a.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>Difference of the two matrices</returns>
        public static Matrix4x4 Subtract(Matrix4x4 a, Matrix4x4 b)
        {
            Subtract(ref a, ref b, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Subtracts matrix b from matrix a.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <param name="result">Difference of the two matrices</param>
        public static void Subtract(ref Matrix4x4 a, ref Matrix4x4 b, out Matrix4x4 result)
        {
            result.M11 = a.M11 - b.M11;
            result.M12 = a.M12 - b.M12;
            result.M13 = a.M13 - b.M13;
            result.M14 = a.M14 - b.M14;

            result.M21 = a.M21 - b.M21;
            result.M22 = a.M22 - b.M22;
            result.M23 = a.M23 - b.M23;
            result.M24 = a.M24 - b.M24;

            result.M31 = a.M31 - b.M31;
            result.M32 = a.M32 - b.M32;
            result.M33 = a.M33 - b.M33;
            result.M34 = a.M34 - b.M34;

            result.M41 = a.M41 - b.M41;
            result.M42 = a.M42 - b.M42;
            result.M43 = a.M43 - b.M43;
            result.M44 = a.M44 - b.M44;
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>Product of the two matrices</returns>
        public static Matrix4x4 Multiply(Matrix4x4 a, Matrix4x4 b)
        {
            Multiply(ref a, ref b, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <param name="result">Product of the two matrices</param>
        public static void Multiply(ref Matrix4x4 a, ref Matrix4x4 b, out Matrix4x4 result)
        {
            float m11 = (((a.M11 * b.M11) + (a.M12 * b.M21)) + (a.M13 * b.M31)) + (a.M14 * b.M41);
            float m12 = (((a.M11 * b.M12) + (a.M12 * b.M22)) + (a.M13 * b.M32)) + (a.M14 * b.M42);
            float m13 = (((a.M11 * b.M13) + (a.M12 * b.M23)) + (a.M13 * b.M33)) + (a.M14 * b.M43);
            float m14 = (((a.M11 * b.M14) + (a.M12 * b.M24)) + (a.M13 * b.M34)) + (a.M14 * b.M44);

            float m21 = (((a.M21 * b.M11) + (a.M22 * b.M21)) + (a.M23 * b.M31)) + (a.M24 * b.M41);
            float m22 = (((a.M21 * b.M12) + (a.M22 * b.M22)) + (a.M23 * b.M32)) + (a.M24 * b.M42);
            float m23 = (((a.M21 * b.M13) + (a.M22 * b.M23)) + (a.M23 * b.M33)) + (a.M24 * b.M43);
            float m24 = (((a.M21 * b.M14) + (a.M22 * b.M24)) + (a.M23 * b.M34)) + (a.M24 * b.M44);

            float m31 = (((a.M31 * b.M11) + (a.M32 * b.M21)) + (a.M33 * b.M31)) + (a.M34 * b.M41);
            float m32 = (((a.M31 * b.M12) + (a.M32 * b.M22)) + (a.M33 * b.M32)) + (a.M34 * b.M42);
            float m33 = (((a.M31 * b.M13) + (a.M32 * b.M23)) + (a.M33 * b.M33)) + (a.M34 * b.M43);
            float m34 = (((a.M31 * b.M14) + (a.M32 * b.M24)) + (a.M33 * b.M34)) + (a.M34 * b.M44);

            float m41 = (((a.M41 * b.M11) + (a.M42 * b.M21)) + (a.M43 * b.M31)) + (a.M44 * b.M41);
            float m42 = (((a.M41 * b.M12) + (a.M42 * b.M22)) + (a.M43 * b.M32)) + (a.M44 * b.M42);
            float m43 = (((a.M41 * b.M13) + (a.M42 * b.M23)) + (a.M43 * b.M33)) + (a.M44 * b.M43);
            float m44 = (((a.M41 * b.M14) + (a.M42 * b.M24)) + (a.M43 * b.M34)) + (a.M44 * b.M44);

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;
            result.M14 = m14;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;
            result.M24 = m24;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
            result.M34 = m34;

            result.M41 = m41;
            result.M42 = m42;
            result.M43 = m43;
            result.M44 = m44;
        }

        /// <summary>
        /// Multiplies a matrix by a scalar value.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Multiplied matrix</returns>
        public static Matrix4x4 Multiply(Matrix4x4 value, float scale)
        {
            Multiply(ref value, scale, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Multiplies a matrix by a scalar value.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="scale">Amount to scale</param>
        /// <param name="result">Multiplied matrix</param>
        public static void Multiply(ref Matrix4x4 value, float scale, out Matrix4x4 result)
        {
            result.M11 = value.M11 * scale;
            result.M12 = value.M12 * scale;
            result.M13 = value.M13 * scale;
            result.M14 = value.M14 * scale;

            result.M21 = value.M21 * scale;
            result.M22 = value.M22 * scale;
            result.M23 = value.M23 * scale;
            result.M24 = value.M24 * scale;

            result.M31 = value.M31 * scale;
            result.M32 = value.M32 * scale;
            result.M33 = value.M33 * scale;
            result.M34 = value.M34 * scale;

            result.M41 = value.M41 * scale;
            result.M42 = value.M42 * scale;
            result.M43 = value.M43 * scale;
            result.M44 = value.M44 * scale;
        }

        /// <summary>
        /// Divides the a matrice's components by the components of another.
        /// </summary>
        /// <param name="a">Source matrix</param>
        /// <param name="b">Divisor matrix</param>
        /// <returns>Quotient of the two matrices</returns>
        public static Matrix4x4 Divide(Matrix4x4 a, Matrix4x4 b)
        {
            Divide(ref a, ref b, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Divides the a matrice's components by the components of another.
        /// </summary>
        /// <param name="a">Source matrix</param>
        /// <param name="b">Divisor matrix</param>
        /// <param name="result">Quotient of the two matrices</param>
        public static void Divide(ref Matrix4x4 a, ref Matrix4x4 b, out Matrix4x4 result)
        {
            result.M11 = a.M11 / b.M11;
            result.M12 = a.M12 / b.M12;
            result.M13 = a.M13 / b.M13;
            result.M14 = a.M14 / b.M14;

            result.M21 = a.M21 / b.M21;
            result.M22 = a.M22 / b.M22;
            result.M23 = a.M23 / b.M23;
            result.M24 = a.M24 / b.M24;

            result.M31 = a.M31 / b.M31;
            result.M32 = a.M32 / b.M32;
            result.M33 = a.M33 / b.M33;
            result.M34 = a.M34 / b.M34;

            result.M41 = a.M41 / b.M41;
            result.M42 = a.M42 / b.M42;
            result.M43 = a.M43 / b.M43;
            result.M44 = a.M44 / b.M44;
        }

        /// <summary>
        /// Divides the a matrice's components by a scalar value.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided matrix</returns>
        public static Matrix4x4 Divide(Matrix4x4 value, float divisor)
        {
            Divide(ref value, divisor, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Divides the a matrice's components by a scalar value.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <param name="result">Divided matrix</param>
        public static void Divide(ref Matrix4x4 value, float divisor, out Matrix4x4 result)
        {
            float invScale = 1.0f / divisor;
            result.M11 = value.M11 * invScale;
            result.M12 = value.M12 * invScale;
            result.M13 = value.M13 * invScale;
            result.M14 = value.M14 * invScale;

            result.M21 = value.M21 * invScale;
            result.M22 = value.M22 * invScale;
            result.M23 = value.M23 * invScale;
            result.M24 = value.M24 * invScale;

            result.M31 = value.M31 * invScale;
            result.M32 = value.M32 * invScale;
            result.M33 = value.M33 * invScale;
            result.M34 = value.M34 * invScale;

            result.M41 = value.M41 * invScale;
            result.M42 = value.M42 * invScale;
            result.M43 = value.M43 * invScale;
            result.M44 = value.M44 * invScale;
        }

        /// <summary>
        /// Computes the inverse of the matrix.
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <returns>Inverse of the matrix</returns>
        public static Matrix4x4 Invert(Matrix4x4 matrix)
        {
            Invert(ref matrix, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Compute the inverse of the specified matrix.
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="result">Inverted matrixt</param>
        public static void Invert(ref Matrix4x4 matrix, out Matrix4x4 result)
        {
            float m11 = matrix.M11;
            float m12 = matrix.M12;
            float m13 = matrix.M13;
            float m14 = matrix.M14;
            float m21 = matrix.M21;
            float m22 = matrix.M22;
            float m23 = matrix.M23;
            float m24 = matrix.M24;
            float m31 = matrix.M31;
            float m32 = matrix.M32;
            float m33 = matrix.M33;
            float m34 = matrix.M34;
            float m41 = matrix.M41;
            float m42 = matrix.M42;
            float m43 = matrix.M43;
            float m44 = matrix.M44;

            float h1 = (m33 * m44) - (m34 * m43);
            float h2 = (m32 * m44) - (m34 * m42);
            float h3 = (m32 * m43) - (m33 * m42);
            float h4 = (m31 * m44) - (m34 * m41);
            float h5 = (m31 * m43) - (m33 * m41);
            float h6 = (m31 * m42) - (m32 * m41);

            float e1 = ((m22 * h1) - (m23 * h2)) + (m24 * h3);
            float e2 = -(((m21 * h1) - (m23 * h4)) + (m24 * h5));
            float e3 = ((m21 * h2) - (m22 * h4)) + (m24 * h6);
            float e4 = -(((m21 * h3) - (m22 * h5)) + (m23 * h6));
            float invDet = 1.0f / ((((m11 * e1) + (m12 * e2)) + (m13 * e3)) + (m14 * e4));

            result.M11 = e1 * invDet;
            result.M21 = e2 * invDet;
            result.M31 = e3 * invDet;
            result.M41 = e4 * invDet;

            result.M12 = -(((m12 * h1) - (m13 * h2)) + (m14 * h3)) * invDet;
            result.M22 = (((m11 * h1) - (m13 * h4)) + (m14 * h5)) * invDet;
            result.M32 = -(((m11 * h2) - (m12 * h4)) + (m14 * h6)) * invDet;
            result.M42 = (((m11 * h3) - (m12 * h5)) + (m13 * h6)) * invDet;

            float h7 = (m23 * m44) - (m24 * m43);
            float h8 = (m22 * m44) - (m24 * m42);
            float h9 = (m22 * m43) - (m23 * m42);
            float h10 = (m21 * m44) - (m24 * m41);
            float h11 = (m21 * m43) - (m23 * m41);
            float h12 = (m21 * m42) - (m22 * m41);

            result.M13 = (((m12 * h7) - (m13 * h8)) + (m14 * h9)) * invDet;
            result.M23 = -(((m11 * h7) - (m13 * h10)) + (m14 * h11)) * invDet;
            result.M33 = (((m11 * h8) - (m12 * h10)) + (m14 * h12)) * invDet;
            result.M43 = -(((m11 * h9) - (m12 * h11)) + (m13 * h12)) * invDet;

            float h13 = (m23 * m34) - (m24 * m33);
            float h14 = (m22 * m34) - (m24 * m32);
            float h15 = (m22 * m33) - (m23 * m32);
            float h16 = (m21 * m34) - (m24 * m31);
            float h17 = (m21 * m33) - (m23 * m31);
            float h18 = (m21 * m32) - (m22 * m31);

            result.M14 = -(((m12 * h13) - (m13 * h14)) + (m14 * h15)) * invDet;
            result.M24 = (((m11 * h13) - (m13 * h16)) + (m14 * h17)) * invDet;
            result.M34 = -(((m11 * h14) - (m12 * h16)) + (m14 * h18)) * invDet;
            result.M44 = (((m11 * h15) - (m12 * h17)) + (m13 * h18)) * invDet;
        }

        /// <summary>
        /// Linearly interpolates between the two specified matrices.
        /// </summary>
        /// <param name="a">Starting matrix</param>
        /// <param name="b">Ending matrix</param>
        /// <param name="percent">Percent to interpolate by</param>
        /// <returns>Linearly interpolated matrix</returns>
        public static Matrix4x4 Lerp(Matrix4x4 a, Matrix4x4 b, float percent)
        {
            Lerp(ref a, ref b, percent, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Linearly interpolates between the two specified matrices.
        /// </summary>
        /// <param name="a">Starting matrix</param>
        /// <param name="b">Ending matrix</param>
        /// <param name="percent">Percent to interpolate by</param>
        /// <param name="result">Linearly interpolated matrix</param>
        public static void Lerp(ref Matrix4x4 a, ref Matrix4x4 b, float percent, out Matrix4x4 result)
        {
            result.M11 = a.M11 + ((b.M11 - a.M11) * percent);
            result.M12 = a.M12 + ((b.M12 - a.M12) * percent);
            result.M13 = a.M13 + ((b.M13 - a.M13) * percent);
            result.M14 = a.M14 + ((b.M14 - a.M14) * percent);

            result.M21 = a.M21 + ((b.M21 - a.M21) * percent);
            result.M22 = a.M22 + ((b.M22 - a.M22) * percent);
            result.M23 = a.M23 + ((b.M23 - a.M23) * percent);
            result.M24 = a.M24 + ((b.M24 - a.M24) * percent);

            result.M31 = a.M31 + ((b.M31 - a.M31) * percent);
            result.M32 = a.M32 + ((b.M32 - a.M32) * percent);
            result.M33 = a.M33 + ((b.M33 - a.M33) * percent);
            result.M34 = a.M34 + ((b.M34 - a.M34) * percent);

            result.M41 = a.M41 + ((b.M41 - a.M41) * percent);
            result.M42 = a.M42 + ((b.M42 - a.M42) * percent);
            result.M43 = a.M43 + ((b.M43 - a.M43) * percent);
            result.M44 = a.M44 + ((b.M44 - a.M44) * percent);
        }

        /// <summary>
        /// Creates a view matrix with specified eye position, a target, and the up vector in the world.
        /// </summary>
        /// <param name="eyePosition">Position of the camera</param>
        /// <param name="target">Camera's target</param>
        /// <param name="up">Up vector of camera</param>
        /// <returns>View matrix</returns>
        public static Matrix4x4 CreateViewMatrix(Vector3 eyePosition, Vector3 target, Vector3 up)
        {
            CreateViewMatrix(ref eyePosition, ref target, ref up, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a view matrix with specified eye position, a target, and the up vector in the world.
        /// </summary>
        /// <param name="position">Position of the camera</param>
        /// <param name="target">Camera's target</param>
        /// <param name="up">Up vector of camera</param>
        /// <param name="result">View matrix</param>
        public static void CreateViewMatrix(ref Vector3 position, ref Vector3 target, ref Vector3 up, out Matrix4x4 result)
        {
            Vector3 direction = Vector3.Normalize(position - target);
            Vector3 left = Vector3.Normalize(Vector3.Cross(up, direction));
            Vector3 newUp = Vector3.Cross(direction, left);
            newUp.Normalize();

            result.M11 = left.X;
            result.M12 = newUp.X;
            result.M13 = direction.X;
            result.M14 = 0.0f;

            result.M21 = left.Y;
            result.M22 = newUp.Y;
            result.M23 = direction.Y;
            result.M24 = 0.0f;

            result.M31 = left.Z;
            result.M32 = newUp.Z;
            result.M33 = direction.Z;
            result.M34 = 0.0f;

            result.M41 = -Vector3.Dot(left, position);
            result.M42 = -Vector3.Dot(newUp, position);
            result.M43 = -Vector3.Dot(direction, position);
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">Width of view volume</param>
        /// <param name="height">Height of view volume</param>
        /// <param name="zNearPlane">Minimum z value of the view volume</param>
        /// <param name="zFarPlane">Maximum z value of the view volume</param>
        /// <returns>Orthographic projection matrix</returns>
        public static Matrix4x4 CreateOrthographicMatrix(float width, float height, float zNearPlane, float zFarPlane)
        {
            CreateOrthographicMatrix(width, height, zNearPlane, zFarPlane, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">Width of view volume</param>
        /// <param name="height">Height of view volume</param>
        /// <param name="zNearPlane">Minimum z value of the view volume</param>
        /// <param name="zFarPlane">Maximum z value of the view volume</param>
        /// <param name="result">Orthographic projection matrix</param>
        public static void CreateOrthographicMatrix(float width, float height, float zNearPlane, float zFarPlane, out Matrix4x4 result)
        {
            result.M11 = 2.0f / width;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = 2.0f / height;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1.0f / (zNearPlane - zFarPlane);
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a custom orthographic projection matrix.
        /// </summary>
        /// <param name="left">Minimum x value of view volume</param>
        /// <param name="right">Maximum x value of view volume</param>
        /// <param name="bottom">Minimum y value of view volume</param>
        /// <param name="top">Maximum y value of view volume</param>
        /// <param name="zNearPlane">Minimum z value of view volume</param>
        /// <param name="zFarPlane">Maximum z value of view volume</param>
        /// <returns>Orthographic projection matrix</returns>
        public static Matrix4x4 CreateOrthographicMatrix(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        {
            CreateOrthographicMatrix(left, right, bottom, top, zNearPlane, zFarPlane, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a custom orthographic projection matrix.
        /// </summary>
        /// <param name="left">Minimum x value of view volume</param>
        /// <param name="right">Maximum x value of view volume</param>
        /// <param name="bottom">Minimum y value of view volume</param>
        /// <param name="top">Maximum y value of view volume</param>
        /// <param name="zNearPlane">Minimum z value of view volume</param>
        /// <param name="zFarPlane">Maximum z value of view volume</param>
        /// <param name="result">Orthographic projection matrix</param>
        public static void CreateOrthographicMatrix(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, out Matrix4x4 result)
        {
            result.M11 = 2.0f / (right - left);
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = 2.0f / (top - bottom);
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1.0f / (zNearPlane - zFarPlane);
            result.M34 = 0.0f;

            result.M41 = (left + right) / (left - right);
            result.M42 = (top + bottom) / (bottom - top);
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="width">Width of the view volume at the near view plane</param>
        /// <param name="height">Height of the view volume at the near view plane</param>
        /// <param name="nearPlaneDistance">Distance to the near view plane</param>
        /// <param name="farPlaneDistance">Distance to the far view plane</param>
        /// <returns>Perspective projection matrix</returns>
        public static Matrix4x4 CreatePerspectiveMatrix(float width, float height, float nearPlaneDistance, float farPlaneDistance)
        {
            CreatePerspectiveMatrix(width, height, nearPlaneDistance, farPlaneDistance, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="width">Width of the view volume at the near view plane</param>
        /// <param name="height">Height of the view volume at the near view plane</param>
        /// <param name="nearPlaneDistance">Distance to the near view plane</param>
        /// <param name="farPlaneDistance">Distance to the far view plane</param>
        /// <param name="result">Perspective projection matrix</param>
        public static void CreatePerspectiveMatrix(float width, float height, float nearPlaneDistance, float farPlaneDistance, out Matrix4x4 result)
        {
            if (nearPlaneDistance <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance), "Plane distance cannot be negative");
            }

            if (farPlaneDistance <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(farPlaneDistance), "Plane distance cannot be negative");
            }

            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance), "Near plane distance larger than far plane");
            }

            result.M11 = (2.0f * nearPlaneDistance) / width;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = (2.0f * nearPlaneDistance) / height;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M34 = -1.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
            result.M44 = 0.0f;
        }

        /// <summary>
        /// Creates a perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fieldOfView">Field of view angle, in the range of [0 and Pi)</param>
        /// <param name="aspectRatio">Aspect ratio, defined as the view space's width divided by height</param>
        /// <param name="nearPlaneDistance">Distance to the near plane</param>
        /// <param name="farPlaneDistance">Distance to the far plane</param>
        /// <returns>Perspective projection matrix</returns>
        public static Matrix4x4 CreatePerspectiveFOVMatrix(Angle fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            CreatePerspectiveFOVMatrix(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fieldOfView">Field of view angle, in the range of [0 and Pi)</param>
        /// <param name="aspectRatio">Aspect ratio, defined as the view space's width divided by height</param>
        /// <param name="nearPlaneDistance">Distance to the near plane</param>
        /// <param name="farPlaneDistance">Distance to the far plane</param>
        /// <param name="result">Perspective projection matrix</param>
        public static void CreatePerspectiveFOVMatrix(Angle fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out Matrix4x4 result)
        {
            if ((fieldOfView.Radians <= 0.0f) || (fieldOfView.Radians >= MathHelper.Pi))
            {
                throw new ArgumentOutOfRangeException(nameof(fieldOfView), "Must be within range [0, Pi]");
            }

            if (nearPlaneDistance <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance), "Plane distance cannot be negative");
            }

            if (farPlaneDistance <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(farPlaneDistance), "Plane distance cannot be negative");
            }

            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance), "Near plane distance larger than far plane");
            }

            float h = 1.0f / ((float)Math.Tan((fieldOfView.Radians * 0.5f)));
            float w = h / aspectRatio;

            result.M11 = w;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = h;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M34 = -1.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
            result.M44 = 0f;
        }

        /// <summary>
        /// Creates a custom perspective projection matrix.
        /// </summary>
        /// <param name="left">Minimum x value of the view volume at the near view plane</param>
        /// <param name="right">Maximum x value of the view volume at the near view plane</param>
        /// <param name="bottom">Minimum y value of the view volume at the near view plane</param>
        /// <param name="top">Maximum y value of the view volume at the near view plane</param>
        /// <param name="nearPlaneDistance">Distance to the near view plane</param>
        /// <param name="farPlaneDistance">Distance to the far view plane</param>
        /// <returns>Perspective projection matrix</returns>
        public static Matrix4x4 CreatePerspectiveMatrix(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
        {
            CreatePerspectiveMatrix(left, right, bottom, top, nearPlaneDistance, farPlaneDistance, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a custom perspective projection matrix.
        /// </summary>
        /// <param name="left">Minimum x value of the view volume at the near view plane</param>
        /// <param name="right">Maximum x value of the view volume at the near view plane</param>
        /// <param name="bottom">Minimum y value of the view volume at the near view plane</param>
        /// <param name="top">Maximum y value of the view volume at the near view plane</param>
        /// <param name="nearPlaneDistance">Distance to the near view plane</param>
        /// <param name="farPlaneDistance">Distance to the far view plane</param>
        /// <param name="result">Perspective projection matrix</param>
        public static void CreatePerspectiveMatrix(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance, out Matrix4x4 result)
        {
            if (nearPlaneDistance <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance), "Plane distance cannot be negative");
            }

            if (farPlaneDistance <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(farPlaneDistance), "Plane distance cannot be negative");
            }

            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance), "Near plane distance larger than far plane");
            }

            result.M11 = (2.0f * nearPlaneDistance) / (right - left);
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = (2.0f * nearPlaneDistance) / (top - bottom);
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = (left + right) / (right - left);
            result.M32 = (top + bottom) / (top - bottom);
            result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M34 = -1.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
            result.M44 = 0.0f;
        }

        /// <summary>
        /// Creates a matrix that can reflect vectors about a plane. The plane is normalized before the matrix is computed.
        /// </summary>
        /// <param name="plane">Plane on which the reflection occurs.</param>
        /// <returns>Reflection matrix</returns>
        public static Matrix4x4 CreateReflectionMatrix(Plane plane)
        {
            CreateReflectionMatrix(ref plane, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a matrix that can reflect vectors about a plane. The plane is normalized before the matrix is computed.
        /// </summary>
        /// <param name="plane">Plane on which the reflection occurs.</param>
        /// <param name="result">Reflection matrix</param>
        public static void CreateReflectionMatrix(ref Plane plane, out Matrix4x4 result)
        {
            Plane plane2 = plane;
            plane2.Normalize();

            float x = plane2.Normal.X;
            float y = plane2.Normal.Y;
            float z = plane2.Normal.Z;

            float x2 = x * -2.0f;
            float y2 = y * -2.0f;
            float z2 = z * -2.0f;

            result.M11 = (x2 * x) + 1.0f;
            result.M12 = y2 * x;
            result.M13 = z2 * x;
            result.M14 = 0.0f;

            result.M21 = x2 * y;
            result.M22 = (y2 * y) + 1.0f;
            result.M23 = (z2 * y);
            result.M24 = 0.0f;

            result.M31 = x2 * z;
            result.M32 = y2 * z;
            result.M33 = (z2 * z) + 1.0f;
            result.M34 = 0.0f;

            result.M41 = x2 * plane2.D;
            result.M42 = y2 * plane2.D;
            result.M43 = z2 * plane2.D;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a matrix that flattens geometry into a shadow. The light is assumed to be a directional (parallel) light.
        /// </summary>
        /// <param name="lightDir">Light direction</param>
        /// <param name="plane">The plane onto which the shadow is projected on. The plane is automatically normalized.</param>
        /// <returns>Shadow matrix</returns>
        public static Matrix4x4 CreateShadowMatrix(Vector3 lightDir, Plane plane)
        {
            CreateShadowMatrix(ref lightDir, ref plane, false, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a matrix that flattens geometry into a shadow.
        /// </summary>
        /// <param name="lightDir">Light direction</param>
        /// <param name="plane">The plane onto which the shadow is projected on. The plane is automatically normalized.</param>
        /// <param name="isPointLight">True if the light is a point light, false if it is a directional (parallel) light.</param>
        /// <returns>Shadow matrix</returns>
        public static Matrix4x4 CreateShadowMatrix(Vector3 lightDir, Plane plane, bool isPointLight)
        {
            CreateShadowMatrix(ref lightDir, ref plane, isPointLight, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a matrix that flattens geometry into a shadow. The light is assumed to be a directional (parallel) light.
        /// </summary>
        /// <param name="lightDir">Light direction</param>
        /// <param name="plane">The plane onto which the shadow is projected on. The plane is automatically normalized.</param>
        /// <param name="result">Shadow matrix</param>
        public static void CreateShadowMatrix(ref Vector3 lightDir, ref Plane plane, out Matrix4x4 result)
        {
            CreateShadowMatrix(ref lightDir, ref plane, false, out result);
        }

        /// <summary>
        /// Creates a matrix that flattens geometry into a shadow.
        /// </summary>
        /// <param name="lightDir">Light direction</param>
        /// <param name="plane">The plane onto which the shadow is projected on. The plane is automatically normalized.</param>
        /// <param name="isPointLight">True if the light is a point light, false if it is a directional (parallel) light.</param>
        /// <param name="result">Shadow matrix</param>
        public static void CreateShadowMatrix(ref Vector3 lightDir, ref Plane plane, bool isPointLight, out Matrix4x4 result)
        {
            Plane plane2 = plane;
            plane2.Normalize();

            Vector4 light = new Vector4(lightDir, isPointLight ? 1 : 0);
            
            Vector3.Dot(ref plane2.Normal, ref lightDir, out float dot);
            float x = -plane.Normal.X;
            float y = -plane.Normal.Y;
            float z = -plane.Normal.Z;
            float d = -plane.D;

            result.M11 = (x * light.X) + dot;
            result.M21 = y * light.X;
            result.M31 = z * light.X;
            result.M41 = d * light.X;

            result.M12 = x * light.Y;
            result.M22 = (y * light.Y) + dot;
            result.M32 = z * light.Y;
            result.M42 = d * light.Y;

            result.M13 = x * light.Z;
            result.M23 = y * light.Z;
            result.M33 = (z * light.Z) + dot;
            result.M43 = d * light.Z;

            result.M14 = x * light.W;
            result.M24 = y * light.W;
            result.M34 = z * light.W;
            result.M44 = (d * light.W) + dot;
        }

        /// <summary>
        /// Creates a spherical billboard that rotates around an object position that is aligned towards the camera.
        /// </summary>
        /// <param name="objectPosition">Position of the object which the billboard will rotate around</param>
        /// <param name="cameraPosition">Position of the camera</param>
        /// <param name="cameraUpVector">Up vector of the camera</param>
        /// <param name="cameraForwardVector">Optional forward vector (direction) of the camera</param>
        /// <returns>Billboard matrix</returns>
        public static Matrix4x4 CreateBillboardMatrix(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3? cameraForwardVector)
        {
            CreateBillboardMatrix(ref objectPosition, ref cameraPosition, ref cameraUpVector, cameraForwardVector, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a spherical billboard that rotates around an object position that is aligned towards the camera.
        /// </summary>
        /// <param name="objectPosition">Position of the object which the billboard will rotate around</param>
        /// <param name="cameraPosition">Position of the camera</param>
        /// <param name="cameraUpVector">Up vector of the camera</param>
        /// <param name="cameraForwardVector">Optional forward vector (direction) of the camera</param>
        /// <param name="result">Billboard matrix</param>
        public static void CreateBillboardMatrix(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, Vector3? cameraForwardVector, out Matrix4x4 result)
        {
            Vector3.Subtract(ref objectPosition, ref cameraPosition, out Vector3 viewDir);
            float diffLengthSquared = viewDir.LengthSquared();

            if (MathHelper.IsApproxZero(diffLengthSquared))
            {
                viewDir = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
            }
            else
            {
                Vector3.Multiply(ref viewDir, (1.0f / (float)Math.Sqrt(diffLengthSquared)), out viewDir);
            }
            
            Vector3.NormalizedCross(ref cameraUpVector, ref viewDir, out Vector3 rightDir);
            Vector3.NormalizedCross(ref viewDir, ref rightDir, out Vector3 billboardUp);

            result.M11 = rightDir.X;
            result.M12 = rightDir.Y;
            result.M13 = rightDir.Z;
            result.M14 = 0.0f;

            result.M21 = billboardUp.X;
            result.M22 = billboardUp.Y;
            result.M23 = billboardUp.Z;
            result.M24 = 0.0f;

            result.M31 = viewDir.X;
            result.M32 = viewDir.Y;
            result.M33 = viewDir.Z;
            result.M34 = 0.0f;

            result.M41 = objectPosition.X;
            result.M42 = objectPosition.Y;
            result.M43 = objectPosition.Z;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a cylindrical billboard that rotates around a specified axis.
        /// </summary>
        /// <param name="objectPosition">Position of the object the billboard will rotate around.</param>
        /// <param name="cameraPosition">Position of the camera.</param>
        /// <param name="rotationAxis">Axis to rotate the billboard around.</param>
        /// <param name="cameraForwardVector">Optional forward vector of the camera.</param>
        /// <param name="objectForwardVector">Optional forward vector of the object.</param>
        /// <returns>Constrained billboard matrix</returns>
        /// <remarks>
        /// This method computes the facing direction of the billboard from the object position and the camera position. If the two positions
        /// are too close, the matrix will not be accurate. To accound for this, supply the optional camera forward vector which would be used in this scenario.
        /// </remarks>
        public static Matrix4x4 CreateConstrainedBillboardMatrix(Vector3 objectPosition, Vector3 cameraPosition, Vector3 rotationAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector)
        {
            CreateConstrainedBillboardMatrix(ref objectPosition, ref cameraPosition, ref rotationAxis, cameraForwardVector, objectForwardVector, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a cylindrical billboard that rotates around a specified axis.
        /// </summary>
        /// <param name="objectPosition">Position of the object the billboard will rotate around.</param>
        /// <param name="cameraPosition">Position of the camera.</param>
        /// <param name="rotationAxis">Axis to rotate the billboard around.</param>
        /// <param name="cameraForwardVector">Optional forward vector of the camera.</param>
        /// <param name="objectForwardVector">Optional forward vector of the object.</param>
        /// <param name="result">Constrained billboard matrix</param>
        /// <remarks>
        /// This method computes the facing direction of the billboard from the object position and the camera position. If the two positions
        /// are too close, the matrix will not be accurate. To accound for this, supply the optional camera forward vector which would be used in this scenario.
        /// </remarks>
        public static void CreateConstrainedBillboardMatrix(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 rotationAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector, out Matrix4x4 result)
        {
            Vector3.Subtract(ref objectPosition, ref cameraPosition, out Vector3 viewDir);
            float diffLengthSquared = viewDir.LengthSquared();

            if (MathHelper.IsApproxZero(diffLengthSquared))
            {
                viewDir = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
            }
            else
            {
                Vector3.Multiply(ref viewDir, (float)(1.0f / (float)Math.Sqrt((double)diffLengthSquared)), out viewDir);
            }

            float tolerance = 1.0f - MathHelper.ZeroTolerance;

            Vector3 billboardForward;
            Vector3 billboardRight;
            
            Vector3.Dot(ref rotationAxis, ref viewDir, out float dot);

            // If viewDir is parallel to rotationAxis, choose a more suitable forward
            if (Math.Abs(dot) >= tolerance)
            {
                if (objectForwardVector.HasValue)
                {
                    billboardForward = objectForwardVector.Value;
                    Vector3.Dot(ref rotationAxis, ref billboardForward, out dot);

                    // Fallback if objForward is parallel
                    if (Math.Abs(dot) >= tolerance)
                    {
                        Vector3 forward = Vector3.Forward;
                        Vector3.Dot(ref rotationAxis, ref forward, out dot);
                        billboardForward = (Math.Abs(dot) > tolerance) ? Vector3.Right : Vector3.Forward;
                    }
                }
                else
                {
                    Vector3 forward = Vector3.Forward;
                    Vector3.Dot(ref rotationAxis, ref forward, out dot);
                    billboardForward = (Math.Abs(dot) > tolerance) ? Vector3.Right : Vector3.Forward;
                }

                // Compute orthonormal basis with chosen forward
                Vector3.NormalizedCross(ref rotationAxis, ref billboardForward, out billboardRight);
                Vector3.NormalizedCross(ref billboardRight, ref rotationAxis, out billboardForward);
            }
            else
            {
                // Compute orthonormal basis with viewDir
                Vector3.NormalizedCross(ref rotationAxis, ref viewDir, out billboardRight);
                Vector3.NormalizedCross(ref billboardRight, ref rotationAxis, out billboardForward);
            }

            result.M11 = billboardRight.X;
            result.M12 = billboardRight.Y;
            result.M13 = billboardRight.Z;
            result.M14 = 0.0f;

            result.M21 = rotationAxis.X;
            result.M22 = rotationAxis.Y;
            result.M23 = rotationAxis.Z;
            result.M24 = 0.0f;

            result.M31 = billboardForward.X;
            result.M32 = billboardForward.Y;
            result.M33 = billboardForward.Z;
            result.M34 = 0.0f;

            result.M41 = objectPosition.X;
            result.M42 = objectPosition.Y;
            result.M43 = objectPosition.Z;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a SRT (Scale-Rotation-Translation) matrix.
        /// </summary>
        /// <param name="scaling">Uniform scaling along X, Y, Z axes</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <returns>SRT (Scale-Rotation-Translation) matrix</returns>
        public static Matrix4x4 CreateTransformationMatrix(float scaling, Quaternion rotation, Vector3 translation)
        {
            CreateTransformationMatrix(scaling, ref rotation, ref translation, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a SRT (Scale-Rotation-Translation) matrix.
        /// </summary>
        /// <param name="scaling">Uniform scaling along X, Y, Z axes</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <param name="result">SRT (Scale-Rotation-Translation) matrix</param>
        public static void CreateTransformationMatrix(float scaling, ref Quaternion rotation, ref Vector3 translation, out Matrix4x4 result)
        {
            FromScale(scaling, out Matrix4x4 scaleMatrix);            
            FromQuaternion(ref rotation, out Matrix4x4 rotMatrix);            
            FromTranslation(ref translation, out Matrix4x4 transMatrix);

            Multiply(ref scaleMatrix, ref rotMatrix, out result);
            Multiply(ref result, ref transMatrix, out result);
        }

        /// <summary>
        /// Creates a SRT (Scale-Rotation-Translation) matrix.
        /// </summary>
        /// <param name="scaleX">Scaling along X axis</param>
        /// <param name="scaleY">Scaling along Y axis</param>
        /// <param name="scaleZ">Scaling along Z axis</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <returns>SRT (Scale-Rotation-Translation) matrix</returns>
        public static Matrix4x4 CreateTransformationMatrix(float scaleX, float scaleY, float scaleZ, Quaternion rotation, Vector3 translation)
        {
            CreateTransformationMatrix(scaleX, scaleY, scaleZ, ref rotation, ref translation, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a SRT (Scale-Rotation-Translation) matrix.
        /// </summary>
        /// <param name="scaleX">Scaling along X axis</param>
        /// <param name="scaleY">Scaling along Y axis</param>
        /// <param name="scaleZ">Scaling along Z axis</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <param name="result">SRT (Scale-Rotation-Translation) matrix</param>
        public static void CreateTransformationMatrix(float scaleX, float scaleY, float scaleZ, ref Quaternion rotation, ref Vector3 translation, out Matrix4x4 result)
        {
            FromScale(scaleX, scaleY, scaleZ, out Matrix4x4 scaleMatrix);
            FromQuaternion(ref rotation, out Matrix4x4 rotMatrix);
            FromTranslation(ref translation, out Matrix4x4 transMatrix);

            Multiply(ref scaleMatrix, ref rotMatrix, out result);
            Multiply(ref result, ref transMatrix, out result);
        }

        /// <summary>
        /// Creates a SRT (Scale-Rotation-Translation) matrix, where the rotation is about a point.
        /// </summary>
        /// <param name="scaling">Uniform scaling along X, Y, Z axes</param>
        /// <param name="rotationCenter">Center of the rotation</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <returns>SRT (Scale-Rotation-Translation) matrix</returns>
        public static Matrix4x4 CreateTransformationMatrix(float scaling, Vector3 rotationCenter, Quaternion rotation, Vector3 translation)
        {
            CreateTransformationMatrix(scaling, ref rotationCenter, ref rotation, ref translation, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a SRT (Scale-Rotation-Translation) matrix, where the rotation is about a point.
        /// </summary>
        /// <param name="scaling">Uniform scaling along X, Y, Z axes</param>
        /// <param name="rotationCenter">Center of the rotation</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <param name="result">SRT (Scale-Rotation-Translation) matrix</param>
        public static void CreateTransformationMatrix(float scaling, ref Vector3 rotationCenter, ref Quaternion rotation, ref Vector3 translation, out Matrix4x4 result)
        {
            FromScale(scaling, out Matrix4x4 scaleMatrix);
            FromQuaternion(ref rotation, out Matrix4x4 rotMatrix);

            Vector3 negRotCenter = new Vector3(-rotationCenter.X, -rotationCenter.Y, -rotationCenter.Z);
            FromTranslation(ref negRotCenter, out Matrix4x4 negTransRotCenterMatrix);
            FromTranslation(ref rotationCenter, out Matrix4x4 transRotCenterMatrix);
            FromTranslation(ref translation, out Matrix4x4 transMatrix);

            Multiply(ref scaleMatrix, ref negTransRotCenterMatrix, out result);
            Multiply(ref result, ref rotMatrix, out result);
            Multiply(ref result, ref transRotCenterMatrix, out result);
            Multiply(ref result, ref transMatrix, out result);
        }

        /// <summary>
        /// Creates a SRT (Scale-Rotation-Translation) matrix, where the rotation is about a point.
        /// </summary>
        /// <param name="scaleX">Scaling along X axis</param>
        /// <param name="scaleY">Scaling along Y axis</param>
        /// <param name="scaleZ">Scaling along Z axis</param>
        /// <param name="rotationCenter">Center of the rotation</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <returns>SRT (Scale-Rotation-Translation) matrix</returns>
        public static Matrix4x4 CreateTransformationMatrix(float scaleX, float scaleY, float scaleZ, Vector3 rotationCenter, Quaternion rotation, Vector3 translation)
        {
            CreateTransformationMatrix(scaleX, scaleY, scaleZ, ref rotationCenter, ref rotation, ref translation, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a SRT (Scale-Rotation-Translation) matrix, where the rotation is about a point.
        /// </summary>
        /// <param name="scaleX">Scaling along X axis</param>
        /// <param name="scaleY">Scaling along Y axis</param>
        /// <param name="scaleZ">Scaling along Z axis</param>
        /// <param name="rotationCenter">Center of the rotation</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <param name="result">SRT (Scale-Rotation-Translation) matrix</param>
        public static void CreateTransformationMatrix(float scaleX, float scaleY, float scaleZ, ref Vector3 rotationCenter, ref Quaternion rotation, ref Vector3 translation, out Matrix4x4 result)
        {
            FromScale(scaleX, scaleY, scaleZ, out Matrix4x4 scaleMatrix);
            FromQuaternion(ref rotation, out Matrix4x4 rotMatrix);

            Vector3 negRotCenter = new Vector3(-rotationCenter.X, -rotationCenter.Y, -rotationCenter.Z);
            FromTranslation(ref negRotCenter, out Matrix4x4 negTransRotCenterMatrix);
            FromTranslation(ref rotationCenter, out Matrix4x4 transRotCenterMatrix);
            FromTranslation(ref translation, out Matrix4x4 transMatrix);

            Multiply(ref scaleMatrix, ref negTransRotCenterMatrix, out result);
            Multiply(ref result, ref rotMatrix, out result);
            Multiply(ref result, ref transRotCenterMatrix, out result);
            Multiply(ref result, ref transMatrix, out result);
        }

        /// <summary>
        /// Creates a RT (Rotation-Translation) matrix.
        /// </summary>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <returns>RT (Rotation-Translation) matrix</returns>
        public static Matrix4x4 CreateTransformationMatrix(Quaternion rotation, Vector3 translation)
        {
            CreateTransformationMatrix(ref rotation, ref translation, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a RT (Rotation-Translation) matrix.
        /// </summary>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <param name="result">RT (Rotation-Translation) matrix</param>
        public static void CreateTransformationMatrix(ref Quaternion rotation, ref Vector3 translation, out Matrix4x4 result)
        {
            FromQuaternion(ref rotation, out Matrix4x4 rotMatrix);
            FromTranslation(ref translation, out Matrix4x4 transMatrix);

            Multiply(ref rotMatrix, ref transMatrix, out result);
        }

        /// <summary>
        /// Creates a RT (Rotation-Translation) matrix, where the rotation is about a point.
        /// </summary>
        /// <param name="rotationCenter">Center of the rotation</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <returns>RT (Rotation-Translation) matrix</returns>
        public static Matrix4x4 CreateTransformationMatrix(Vector3 rotationCenter, Quaternion rotation, Vector3 translation)
        {
            CreateTransformationMatrix(ref rotationCenter, ref rotation, ref translation, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a RT (Rotation-Translation) matrix, where the rotation is about a point.
        /// </summary>
        /// <param name="rotationCenter">Center of the rotation</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        /// <param name="result">RT (Rotation-Translation) matrix</param>
        public static void CreateTransformationMatrix(ref Vector3 rotationCenter, ref Quaternion rotation, ref Vector3 translation, out Matrix4x4 result)
        {
            FromQuaternion(ref rotation, out Matrix4x4 rotMatrix);

            Vector3 negRotCenter = new Vector3(-rotationCenter.X, -rotationCenter.Y, -rotationCenter.Z);
            FromTranslation(ref negRotCenter, out Matrix4x4 negTransRotCenterMatrix);
            FromTranslation(ref rotationCenter, out Matrix4x4 transRotCenterMatrix);
            FromTranslation(ref translation, out Matrix4x4 transMatrix);

            Multiply(ref rotMatrix, ref negTransRotCenterMatrix, out result);
            Multiply(ref result, ref transRotCenterMatrix, out result);
            Multiply(ref result, ref transMatrix, out result);
        }

        /// <summary>
        /// Creates a rotation matrix where the object is facing the target along its z axis.
        /// If your object's "forward" facing is down -Z axis, then the object will correctly face the target.
        /// </summary>
        /// <param name="position">Position of object</param>
        /// <param name="target">Position of target</param>
        /// <param name="worldUp">World up vector</param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 LookAt(Vector3 position, Vector3 target, Vector3 worldUp)
        {
            LookAt(ref position, ref target, ref worldUp, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a rotation matrix where the object is facing the target along its z axis.
        /// If your object's "forward" facing is down -Z axis, then the object will correctly face the target.
        /// </summary>
        /// <param name="position">Position of object</param>
        /// <param name="target">Position of target</param>
        /// <param name="worldUp">World up vector</param>
        /// <param name="result">Rotation matrix</param>
        public static void LookAt(ref Vector3 position, ref Vector3 target, ref Vector3 worldUp, out Matrix4x4 result)
        {
            Vector3.Subtract(ref position, ref target, out Vector3 zAxis);
            zAxis.Normalize();
            
            Vector3.Cross(ref worldUp, ref zAxis, out Vector3 worldUpXdir);
            Vector3.Normalize(ref worldUpXdir, out Vector3 xAxis);
            Vector3.Cross(ref zAxis, ref xAxis, out Vector3 yAxis);

            FromAxes(ref xAxis, ref yAxis, ref zAxis, out result);
        }

        /// <summary>
        /// Creates a rotation matrix from 3 orthogonal axes.
        /// </summary>
        /// <param name="xAxis">X axis (first row, right)</param>
        /// <param name="yAxis">Y axis (second row, up)</param>
        /// <param name="zAxis">Z axis (third row, backward)</param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 FromAxes(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            FromAxes(ref xAxis, ref yAxis, ref zAxis, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a rotation matrix from 3 orthogonal axes.
        /// </summary>
        /// <param name="xAxis">X axis (first row, right)</param>
        /// <param name="yAxis">Y axis (second row, up)</param>
        /// <param name="zAxis">Z axis (third row, backward)</param>
        /// <param name="result">Rotation matrix</param>
        public static void FromAxes(ref Vector3 xAxis, ref Vector3 yAxis, ref Vector3 zAxis, out Matrix4x4 result)
        {
            // Set x axis to xAxis (first row)
            result.M11 = xAxis.X;
            result.M12 = xAxis.Y;
            result.M13 = xAxis.Z;
            result.M14 = 0.0f;

            // Set y axis to yAxis (second row)
            result.M21 = yAxis.X;
            result.M22 = yAxis.Y;
            result.M23 = yAxis.Z;
            result.M24 = 0.0f;

            // Set z axis to zAxis (third row)
            result.M31 = zAxis.X;
            result.M32 = zAxis.Y;
            result.M33 = zAxis.Z;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Transforms the specified matrix by a quaternion rotation.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="rot">Quaternion rotation</param>
        /// <returns>Transformed matrix</returns>
        public static Matrix4x4 Transform(Matrix4x4 value, Quaternion rot)
        {
            Transform(ref value, ref rot, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Transforms the specified matrix by a quaternion rotation.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="rot">Quaternion rotation</param>
        /// <param name="result">Transformed</param>
        public static void Transform(ref Matrix4x4 value, ref Quaternion rot, out Matrix4x4 result)
        {
            float x2 = rot.X + rot.X;
            float y2 = rot.Y + rot.Y;
            float z2 = rot.Z + rot.Z;

            float wx2 = rot.W * x2;
            float wy2 = rot.W * y2;
            float wz2 = rot.W * z2;

            float xx2 = rot.X * x2;
            float xy2 = rot.X * y2;
            float xz2 = rot.X * z2;

            float yy2 = rot.Y * y2;
            float yz2 = rot.Y * z2;

            float zz2 = rot.Z * z2;

            float h1 = (1f - yy2) - zz2;
            float h2 = xy2 - wz2;
            float h3 = xz2 + wy2;
            float h4 = xy2 + wz2;
            float h5 = (1f - xx2) - zz2;
            float h6 = yz2 - wx2;
            float h7 = xz2 - wy2;
            float h8 = yz2 + wx2;
            float h9 = (1f - xx2) - yy2;

            result.M11 = ((value.M11 * h1) + (value.M12 * h2)) + (value.M13 * h3);
            result.M12 = ((value.M11 * h4) + (value.M12 * h5)) + (value.M13 * h6);
            result.M13 = ((value.M11 * h7) + (value.M12 * h8)) + (value.M13 * h9);
            result.M14 = value.M14;

            result.M21 = ((value.M21 * h1) + (value.M22 * h2)) + (value.M23 * h3);
            result.M22 = ((value.M21 * h4) + (value.M22 * h5)) + (value.M23 * h6);
            result.M23 = ((value.M21 * h7) + (value.M22 * h8)) + (value.M23 * h9);
            result.M24 = value.M24;

            result.M31 = ((value.M31 * h1) + (value.M32 * h2)) + (value.M33 * h3);
            result.M32 = ((value.M31 * h4) + (value.M32 * h5)) + (value.M33 * h6);
            result.M33 = ((value.M31 * h7) + (value.M32 * h8)) + (value.M33 * h9);
            result.M34 = value.M34;

            result.M41 = ((value.M41 * h1) + (value.M42 * h2)) + (value.M43 * h3);
            result.M42 = ((value.M41 * h4) + (value.M42 * h5)) + (value.M43 * h6);
            result.M43 = ((value.M41 * h7) + (value.M42 * h8)) + (value.M43 * h9);
            result.M44 = value.M44;
        }

        /// <summary>
        /// Swaps the rows and columns of the specified matrix.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <returns>Transposed matrix</returns>
        public static Matrix4x4 Transpose(Matrix4x4 value)
        {
            Transpose(ref value, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Swaps the rows and columns of the specified matrix.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="result">Transposed matrix</param>
        public static void Transpose(ref Matrix4x4 value, out Matrix4x4 result)
        {
            float m11 = value.M11;
            float m12 = value.M12;
            float m13 = value.M13;
            float m14 = value.M14;
            float m21 = value.M21;
            float m22 = value.M22;
            float m23 = value.M23;
            float m24 = value.M24;
            float m31 = value.M31;
            float m32 = value.M32;
            float m33 = value.M33;
            float m34 = value.M34;
            float m41 = value.M41;
            float m42 = value.M42;
            float m43 = value.M43;
            float m44 = value.M44;

            result.M11 = m11;
            result.M12 = m21;
            result.M13 = m31;
            result.M14 = m41;

            result.M21 = m12;
            result.M22 = m22;
            result.M23 = m32;
            result.M24 = m42;

            result.M31 = m13;
            result.M32 = m23;
            result.M33 = m33;
            result.M34 = m43;

            result.M41 = m14;
            result.M42 = m24;
            result.M43 = m34;
            result.M44 = m44;
        }

        /// <summary>
        /// Flips the signs of each matrix element.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <returns>Negated matrix</returns>
        public static Matrix4x4 Negate(Matrix4x4 value)
        {
            Negate(ref value, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of each matrix element.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="result">Negated matrix</param>
        public static void Negate(ref Matrix4x4 value, out Matrix4x4 result)
        {
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M14 = -value.M14;

            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M24 = -value.M24;

            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            result.M34 = -value.M34;

            result.M41 = -value.M41;
            result.M42 = -value.M42;
            result.M43 = -value.M43;
            result.M44 = -value.M44;
        }

        /// <summary>
        /// Creates a matrix that represents a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 FromRotationX(Angle angle)
        {
            FromRotationX(angle, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a matrix that represents a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="result">Rotation matrix</param>
        public static void FromRotationX(Angle angle, out Matrix4x4 result)
        {
            float cos = (float)Math.Cos((double)angle);
            float sin = (float)Math.Sin((double)angle);

            result.M11 = 1.0f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = cos;
            result.M23 = sin;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = -sin;
            result.M33 = cos;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a matrix that represents a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 FromRotationY(Angle angle)
        {
            FromRotationY(angle, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a matrix that represents a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">Angle to rotates</param>
        /// <param name="result">Rotation matrix</param>
        public static void FromRotationY(Angle angle, out Matrix4x4 result)
        {
            float cos = (float)Math.Cos((double)angle);
            float sin = (float)Math.Sin((double)angle);

            result.M11 = cos;
            result.M12 = 0.0f;
            result.M13 = -sin;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = 1.0f;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = sin;
            result.M32 = 0.0f;
            result.M33 = cos;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a matrix that represents a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 FromRotationZ(Angle angle)
        {
            FromRotationZ(angle, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a matrix that represents a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="result">Rotation matrix</param>
        public static void FromRotationZ(Angle angle, out Matrix4x4 result)
        {
            float cos = (float)Math.Cos((double)angle);
            float sin = (float)Math.Sin((double)angle);

            result.M11 = cos;
            result.M12 = sin;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = -sin;
            result.M22 = cos;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1.0f;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a scaling matrix from the specified scale vector.
        /// </summary>
        /// <param name="scale">Scaling vector where each component corresponds to each axis to scale on</param>
        /// <returns>Scaling matrix</returns>
        public static Matrix4x4 FromScale(Vector3 scale)
        {
            FromScale(ref scale, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a scaling matrix from the specified scale vector.
        /// </summary>
        /// <param name="scale">Scaling vector where each component corresponds to each axis to scale on</param>
        /// <param name="result">Scaling matrix</param>
        public static void FromScale(ref Vector3 scale, out Matrix4x4 result)
        {
            result.M11 = scale.X;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = scale.Y;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = scale.Z;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a scaling matrix from the specified scaling value for uniform scale.
        /// </summary>
        /// <param name="scale">Scaling value for x,y,z axes</param>
        /// <returns>Scaling matrix</returns>
        public static Matrix4x4 FromScale(float scale)
        {
            FromScale(scale, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a scaling matrix from the specified scaling value for uniform scale.
        /// </summary>
        /// <param name="scale">Scaling value for x,y,z axes</param>
        /// <param name="result">Scaling matrix</param>
        public static void FromScale(float scale, out Matrix4x4 result)
        {
            result.M11 = scale;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = scale;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = scale;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a scale matrix from the specified scale values.
        /// </summary>
        /// <param name="x">Amount to scale along x axis</param>
        /// <param name="y">Amount to scale along y axis</param>
        /// <param name="z">Amount to scale along z axis</param>
        /// <returns>Scaling matrix</returns>
        public static Matrix4x4 FromScale(float x, float y, float z)
        {
            FromScale(x, y, z, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a scale matrix from the specified scale values.
        /// </summary>
        /// <param name="x">Amount to scale along x axis</param>
        /// <param name="y">Amount to scale along y axis</param>
        /// <param name="z">Amount to scale along z axis</param>
        /// <param name="result">Scaling matrix</param>
        public static void FromScale(float x, float y, float z, out Matrix4x4 result)
        {
            result.M11 = x;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = y;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = z;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a translation matrix from the specified translation vector.
        /// </summary>
        /// <param name="translation">Translation vector</param>
        /// <returns>Translation matrix</returns>
        public static Matrix4x4 FromTranslation(Vector3 translation)
        {
            FromTranslation(ref translation, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a translation matrix from the specified translation vector.
        /// </summary>
        /// <param name="translation">Translation vector</param>
        /// <param name="result">Translation matrix</param>
        public static void FromTranslation(ref Vector3 translation, out Matrix4x4 result)
        {
            result.M11 = 1.0f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = 1.0f;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1.0f;
            result.M34 = 0.0f;

            result.M41 = translation.X;
            result.M42 = translation.Y;
            result.M43 = translation.Z;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a translation matrix from the specified translation values.
        /// </summary>
        /// <param name="x">Amount to translate along x axis</param>
        /// <param name="y">Amount to translate along y axis</param>
        /// <param name="z">Amount to translate along z axis</param>
        /// <returns>Translation matrix</returns>
        public static Matrix4x4 FromTranslation(float x, float y, float z)
        {
            FromTranslation(x, y, z, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a translation matrix from the the specified translation values.
        /// </summary>
        /// <param name="x">Amount to translate along x axis</param>
        /// <param name="y">Amount to translate along y axis</param>
        /// <param name="z">Amount to translate along z axis</param>
        /// <param name="result">Translation matrix</param>
        public static void FromTranslation(float x, float y, float z, out Matrix4x4 result)
        {
            result.M11 = 1.0f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = 1.0f;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1.0f;
            result.M34 = 0.0f;

            result.M41 = x;
            result.M42 = y;
            result.M43 = z;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a rotation matrix from the specified quaternion.
        /// </summary>
        /// <param name="rot">Rotation quaternion</param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 FromQuaternion(Quaternion rot)
        {
            FromQuaternion(ref rot, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a rotation matrix from the specified quaternion.
        /// </summary>
        /// <param name="rot">Rotation quaternion</param>
        /// <param name="result">Rotation matrix</param>
        public static void FromQuaternion(ref Quaternion rot, out Matrix4x4 result)
        {
            float x = rot.X;
            float y = rot.Y;
            float z = rot.Z;
            float w = rot.W;

            float xx = x * x;
            float xy = x * y;
            float xw = x * w;

            float yy = y * y;
            float yz = y * z;
            float yw = y * w;

            float zx = z * x;
            float zz = z * z;
            float zw = z * w;

            result.M11 = 1.0f - (2.0f * (yy + zz));
            result.M12 = 2.0f * (xy + zw);
            result.M13 = 2.0f * (zx - yw);
            result.M14 = 0.0f;

            result.M21 = 2.0f * (xy - zw);
            result.M22 = 1.0f - (2.0f * (xx + zz));
            result.M23 = 2.0f * (yz + xw);
            result.M24 = 0.0f;

            result.M31 = 2.0f * (zx + yw);
            result.M32 = 2.0f * (yz - xw);
            result.M33 = 1.0f - (2.0f * (xx + yy));
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a rotation matrix from a specified angle in radians and
        /// axis to rotate about.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="axis">Axis to rotate about</param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 FromAngleAxis(Angle angle, Vector3 axis)
        {
            FromAngleAxis(angle, ref axis, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a rotation matrix from a specified angle in radians and
        /// axis to rotate about.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="axis">Axis to rotate about</param>
        /// <param name="result">Rotation matrix</param>
        public static void FromAngleAxis(Angle angle, ref Vector3 axis, out Matrix4x4 result)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;

            float sin = (float)Math.Sin((double)angle);
            float cos = (float)Math.Cos((double)angle);

            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;

            result.M11 = xx + (cos * (1.0f - xx));
            result.M12 = (xy - (cos * xy)) + (sin * z);
            result.M13 = (xz - (cos * xz)) - (sin * y);
            result.M14 = 0.0f;

            result.M21 = (xy - (cos * xy)) - (sin * z);
            result.M22 = yy + (cos * (1.0f - yy));
            result.M23 = (yz - (cos * yz)) + (sin * x);
            result.M24 = 0.0f;

            result.M31 = (xz - (cos * xz)) + (sin * y);
            result.M32 = (yz - (cos * yz)) - (sin * x);
            result.M33 = zz + (cos * (1.0f - zz));

            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a rotation matrix from the corresponding yaw, pitch, and roll
        /// angles. The order of the angles are applied in that order.
        /// </summary>
        /// <param name="yaw">Angle to rotate about the y-axis</param>
        /// <param name="pitch">Angle to rotate about the x-axis</param>
        /// <param name="roll">Angle to rotate about the z-axis</param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 FromEulerAngles(Angle yaw, Angle pitch, Angle roll)
        {
            FromEulerAngles(yaw, pitch, roll, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Creates a rotation matrix from the corresponding yaw, pitch, and roll
        /// angles. The order of the angles are applied in that order.
        /// </summary>
        /// <param name="yaw">Angle to rotate about the y-axis</param>
        /// <param name="pitch">Angle to rotate about the x-axis</param>
        /// <param name="roll">Angle to rotate about the z-axis</param>
        /// <param name="result">Rotation matrix</param>
        public static void FromEulerAngles(Angle yaw, Angle pitch, Angle roll, out Matrix4x4 result)
        {
            Quaternion quaternion;
            Quaternion.FromEulerAngles(yaw, pitch, roll, out quaternion);
            FromQuaternion(ref quaternion, out result);
        }

        /// <summary>
        /// Orthogonalizes the matrix.
        /// </summary>
        /// <param name="value">Matrix to orthogonalize</param>
        /// <returns>Orthogonalized matrix</returns>
        /// <remarks>
        /// Orthogonalization is the process of making the axes (rows) of the matrix orthogonal to each other. Thus any
        /// row in the matrix will be orthogonal to any other row in the Matrix. The method uses the modified Gram-Schmidt process,
        /// which results in the matrix to be numerically unstable where the numerical stability decreases in the order that the rows
        /// are pr ocessed. The first row is the most stable, the fourth row the least.
        /// </remarks>
        public static Matrix4x4 Orthogonalize(Matrix4x4 value)
        {
            Orthogonalize(ref value, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Orthogonalizes the matrix.
        /// </summary>
        /// <param name="value">Matrix to orthogonalize</param>
        /// <param name="result">Orthogonalized matrix</param>
        /// <remarks>
        /// Orthogonalization is the process of making the axes (rows) of the matrix orthogonal to each other. Thus any
        /// row in the matrix will be orthogonal to any other row in the Matrix. The method uses the modified Gram-Schmidt process,
        /// which results in the matrix to be numerically unstable where the numerical stability decreases in the order that the rows
        /// are pr ocessed. The first row is the most stable, the fourth row the least.
        /// </remarks>
        public static void Orthogonalize(ref Matrix4x4 value, out Matrix4x4 result)
        {
            result = value;
            result.Row2 -= (Vector4.Dot(result.Row1, result.Row2) / Vector4.Dot(result.Row1, result.Row1)) * result.Row1;

            result.Row3 -= (Vector4.Dot(result.Row1, result.Row3) / Vector4.Dot(result.Row1, result.Row1)) * result.Row1;
            result.Row3 -= (Vector4.Dot(result.Row2, result.Row3) / Vector4.Dot(result.Row2, result.Row2)) * result.Row2;

            result.Row4 -= (Vector4.Dot(result.Row1, result.Row4) / Vector4.Dot(result.Row1, result.Row1)) * result.Row1;
            result.Row4 -= (Vector4.Dot(result.Row2, result.Row4) / Vector4.Dot(result.Row2, result.Row2)) * result.Row2;
            result.Row4 -= (Vector4.Dot(result.Row3, result.Row4) / Vector4.Dot(result.Row3, result.Row3)) * result.Row3;
        }

        /// <summary>
        /// Tests equality between two matrices.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public static bool operator ==(Matrix4x4 a, Matrix4x4 b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two matrices.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>True if components are not equal, false otherwise.</returns>
        public static bool operator !=(Matrix4x4 a, Matrix4x4 b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>Sum of the two matrices</returns>
        public static Matrix4x4 operator +(Matrix4x4 a, Matrix4x4 b)
        {
            Add(ref a, ref b, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Flips the signs of each matrix element.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <returns>Negated matrix</returns>
        public static Matrix4x4 operator -(Matrix4x4 value)
        {
            Negate(ref value, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Subtracts matrix b from matrix a.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>Difference of the two matrices</returns>
        public static Matrix4x4 operator -(Matrix4x4 a, Matrix4x4 b)
        {
            Subtract(ref a, ref b, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>Product of the two matrices</returns>
        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Multiply(ref a, ref b, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Multiplies a matrix by a scalar value.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="scale">Amount to scale</param>
        /// <returns>Multiplied matrix</returns>
        public static Matrix4x4 operator *(Matrix4x4 value, float scale)
        {
            Multiply(ref value, scale, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Multiplies a matrix by a scalar value.
        /// </summary>
        /// <param name="scale">Amount to scale</param>
        /// <param name="value">Source matrix</param>
        /// <returns>Multiplied matrix</returns>
        public static Matrix4x4 operator *(float scale, Matrix4x4 value)
        {
            Multiply(ref value, scale, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Divides the a matrice's components by the components of another.
        /// </summary>
        /// <param name="a">Source matrix</param>
        /// <param name="b">Divisor matrix</param>
        /// <returns>Quotient of the two matrices</returns>
        public static Matrix4x4 operator /(Matrix4x4 a, Matrix4x4 b)
        {
            Divide(ref a, ref b, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Divides the a matrice's components by a scalar value.
        /// </summary>
        /// <param name="value">Source matrix</param>
        /// <param name="divisor">Divisor scalar</param>
        /// <returns>Divided matrix</returns>
        public static Matrix4x4 operator /(Matrix4x4 value, float divisor)
        {
            Divide(ref value, divisor, out Matrix4x4 result);
            return result;
        }

        /// <summary>
        /// Computes the determinant of the matrix.
        /// </summary>
        /// <returns>Determinant</returns>
        public float Determinant()
        {
            float m11 = M11;
            float m12 = M12;
            float m13 = M13;
            float m14 = M14;
            float m21 = M21;
            float m22 = M22;
            float m23 = M23;
            float m24 = M24;
            float m31 = M31;
            float m32 = M32;
            float m33 = M33;
            float m34 = M34;
            float m41 = M41;
            float m42 = M42;
            float m43 = M43;
            float m44 = M44;

            float h1 = (m33 * m44) - (m34 * m43);
            float h2 = (m32 * m44) - (m34 * m42);
            float h3 = (m32 * m43) - (m33 * m42);
            float h4 = (m31 * m44) - (m34 * m41);
            float h5 = (m31 * m43) - (m33 * m41);
            float h6 = (m31 * m42) - (m32 * m41);
            return ((((m11 * (((m22 * h1) - (m23 * h2)) + (m24 * h3))) - (m12 * (((m21 * h1) - (m23 * h4)) + (m24 * h5)))) + (m13 * (((m21 * h2) - (m22 * h4)) + (m24 * h6)))) - (m14 * (((m21 * h3) - (m22 * h5)) + (m23 * h6))));
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        public void Invert()
        {
            Invert(ref this, out this);
        }

        /// <summary>
        /// Flips the signs of each matrix element.
        /// </summary>
        public void Negate()
        {
            M11 = -M11;
            M12 = -M12;
            M13 = -M13;
            M14 = -M14;

            M21 = -M21;
            M22 = -M22;
            M23 = -M23;
            M24 = -M24;

            M31 = -M31;
            M32 = -M32;
            M33 = -M33;
            M34 = -M34;

            M41 = -M41;
            M42 = -M42;
            M43 = -M43;
            M44 = -M44;
        }

        /// <summary>
        /// Swaps the rows and columns of the matrix.
        /// </summary>
        public void Transpose()
        {
            float m11 = M11;
            float m12 = M12;
            float m13 = M13;
            float m14 = M14;
            float m21 = M21;
            float m22 = M22;
            float m23 = M23;
            float m24 = M24;
            float m31 = M31;
            float m32 = M32;
            float m33 = M33;
            float m34 = M34;
            float m41 = M41;
            float m42 = M42;
            float m43 = M43;
            float m44 = M44;

            M11 = m11;
            M12 = m21;
            M13 = m31;
            M14 = m41;

            M21 = m12;
            M22 = m22;
            M23 = m32;
            M24 = m42;

            M31 = m13;
            M32 = m23;
            M33 = m33;
            M34 = m43;

            M41 = m14;
            M42 = m24;
            M43 = m34;
            M44 = m44;
        }

        /// <summary>
        /// Orthogonalizes the matrix.
        /// </summary>
        /// <remarks>
        /// Orthogonalization is the process of making the axes (rows) of the matrix orthogonal to each other. Thus any
        /// row in the matrix will be orthogonal to any other row in the Matrix. The method uses the modified Gram-Schmidt process,
        /// which results in the matrix to be numerically unstable where the numerical stability decreases in the order that the rows
        /// are pr ocessed. The first row is the most stable, the fourth row the least.
        /// </remarks>
        public void Orthogonalize()
        {
            Row2 -= (Vector4.Dot(Row1, Row2) / Vector4.Dot(Row1, Row1)) * Row1;

            Row3 -= (Vector4.Dot(Row1, Row3) / Vector4.Dot(Row1, Row1)) * Row1;
            Row3 -= (Vector4.Dot(Row2, Row3) / Vector4.Dot(Row2, Row2)) * Row2;

            Row4 -= (Vector4.Dot(Row1, Row4) / Vector4.Dot(Row1, Row1)) * Row1;
            Row4 -= (Vector4.Dot(Row2, Row4) / Vector4.Dot(Row2, Row2)) * Row2;
            Row4 -= (Vector4.Dot(Row3, Row4) / Vector4.Dot(Row3, Row3)) * Row3;
        }

        /// <summary>
        /// Decomposes a 3D rotation matrix into euler angles.
        /// </summary>
        /// <param name="yaw">Angle to rotate about the y-axis</param>
        /// <param name="pitch">Angle to rotate about the x-axis</param>
        /// <param name="roll">Angle to rotate about the z-axis</param>
        public void ToEulerAngles(out Angle yaw, out Angle pitch, out Angle roll)
        {
            if (M21 > 0.998)
            {
                // North pole singularity
                yaw = new Angle((float)Math.Atan2(M13, M33));
                pitch = new Angle(0.0f);
                roll = new Angle(MathHelper.PiOverTwo);
            }
            else if (M21 < -0.998)
            {
                // South pole singularity
                yaw = new Angle((float)Math.Atan2(M13, M33));
                pitch = new Angle(0.0f);
                roll = new Angle(-MathHelper.PiOverTwo);
            }
            else
            {
                yaw = new Angle((float)Math.Atan2(-M31, M11));
                pitch = new Angle((float)Math.Atan2(-M23, M22));
                roll = new Angle((float)Math.Asin(M21));
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
        /// Decomposes a 3D scale/rotation/translation (SRT) matrix into its
        /// scaling, rotation, and translation components.
        /// </summary>
        /// <param name="scale">Decomposed scale</param>
        /// <param name="rotation">Decomposed rotation</param>
        /// <param name="translation">Decomposed translation</param>
        /// <returns>Returns true if rotation was derived, if false the identity quaternion is returned</returns>
        public bool Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            float m11 = M11;
            float m12 = M12;
            float m13 = M13;
            float m14 = M14;
            float m21 = M21;
            float m22 = M22;
            float m23 = M23;
            float m24 = M24;
            float m31 = M31;
            float m32 = M32;
            float m33 = M33;
            float m34 = M34;
            float m41 = M41;
            float m42 = M42;
            float m43 = M43;
            float m44 = M44;

            // Flag for if the operation was a success - only returns false if we can't get a rotation
            bool flag = true;

            // Determine which axis to start with dealing with negative scaling
            int index1;
            int index2;
            int index3;

            // Setup a canonical basis
            Triad canonicalBasis = new Triad(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);

            // Setup a basis for the rotation matrix
            Vector3 basisX;
            basisX.X = m11;
            basisX.Y = m12;
            basisX.Z = m13;

            Vector3 basisY;
            basisY.X = m21;
            basisY.Y = m22;
            basisY.Z = m23;

            Vector3 basisZ;
            basisZ.X = m31;
            basisZ.Y = m32;
            basisZ.Z = m33;

            Triad rotBasis = new Triad(basisX, basisY, basisZ);

            // Get the translation component
            translation.X = m41;
            translation.Y = m42;
            translation.Z = m43;

            // Get scaling component
            float scaleX = basisX.Length();
            float scaleY = basisY.Length();
            float scaleZ = basisZ.Length();

            if (scaleX < scaleY)
            {
                if (scaleY < scaleZ)
                {
                    index1 = 2;
                    index2 = 1;
                    index3 = 0;
                }
                else
                {
                    index1 = 1;
                    if (scaleX < scaleZ)
                    {
                        index2 = 2;
                        index3 = 0;
                    }
                    else
                    {
                        index2 = 0;
                        index3 = 2;
                    }
                }
            }
            else if (scaleX < scaleZ)
            {
                index1 = 2;
                index2 = 0;
                index3 = 1;
            }
            else
            {
                index1 = 0;
                if (scaleY < scaleZ)
                {
                    index2 = 2;
                    index3 = 1;
                }
                else
                {
                    index2 = 1;
                    index3 = 2;
                }
            }

            float temp = 0;
            switch (index1)
            {
                case 0:
                    temp = scaleX;
                    break;
                case 1:
                    temp = scaleY;
                    break;
                case 2:
                    temp = scaleZ;
                    break;
            }

            bool cb = false;
            if (temp < MathHelper.ZeroTolerance)
            {
                cb = true;
                rotBasis[index1] = canonicalBasis[index1];
            }
            rotBasis[index1] = Vector3.Normalize(rotBasis[index1]);

            temp = 0;
            switch (index2)
            {
                case 0:
                    temp = scaleX;
                    break;
                case 1:
                    temp = scaleY;
                    break;
                case 2:
                    temp = scaleZ;
                    break;
            }

            if (temp < MathHelper.ZeroTolerance)
            {
                int index4;
                float absX = (float)Math.Abs((double)rotBasis[index1].X);
                float absY = (float)Math.Abs((double)rotBasis[index1].Y);
                float absZ = (float)Math.Abs((double)rotBasis[index1].Z);

                if (absX < absY)
                {
                    if (absY < absZ)
                    {
                        index4 = 0;
                    }
                    else if (absX < absZ)
                    {
                        index4 = 0;
                    }
                    else
                    {
                        index4 = 2;
                    }
                }
                else if (absX < absZ)
                {
                    index4 = 1;
                }
                else if (absY < absZ)
                {
                    index4 = 1;
                }
                else
                {
                    index4 = 2;
                }
                if (cb)
                {
                    rotBasis[index4] = Vector3.Cross(rotBasis[index2], rotBasis[index1]);
                }
            }
            rotBasis[index2] = Vector3.Normalize(rotBasis[index2]);

            temp = 0;
            switch (index3)
            {
                case 0:
                    temp = scaleX;
                    break;
                case 1:
                    temp = scaleY;
                    break;
                case 2:
                    temp = scaleZ;
                    break;
            }

            if (temp < MathHelper.ZeroTolerance)
            {
                rotBasis[index2] = Vector3.Cross(rotBasis[index3], rotBasis[index1]);
            }
            rotBasis[index3] = Vector3.Normalize(rotBasis[index3]);

            float det = rotBasis.ComputeDeterminant();
            if (det < 0.0f)
            {
                switch (index1)
                {
                    case 0:
                        scaleX = -scaleX;
                        break;
                    case 1:
                        scaleY = -scaleY;
                        break;
                    case 2:
                        scaleZ = -scaleZ;
                        break;
                }
                rotBasis[index1] = -rotBasis[index1];
                det = -det;
            }

            det--;
            det *= det;

            // Grab rotation
            if (MathHelper.ZeroTolerance < det)
            {
                rotation = Quaternion.Identity;
                flag = false;
            }
            else
            {
                Matrix4x4 m = Matrix4x4.Identity;
                m.M11 = rotBasis.XAxis.X;
                m.M12 = rotBasis.XAxis.Y;
                m.M13 = rotBasis.XAxis.Z;
                m.M21 = rotBasis.YAxis.X;
                m.M22 = rotBasis.YAxis.Y;
                m.M23 = rotBasis.YAxis.Z;
                m.M31 = rotBasis.ZAxis.X;
                m.M32 = rotBasis.ZAxis.Y;
                m.M33 = rotBasis.ZAxis.Z;
                Quaternion.FromRotationMatrix(ref m, out rotation);
            }

            // Grab scaling
            scale.X = scaleX;
            scale.Y = scaleY;
            scale.Z = scaleZ;

            return flag;
        }

        /// <summary>
        /// Tests equality between the matrix and another mat rix.
        /// </summary>
        /// <param name="other">Matrix to compare</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(Matrix4x4 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the matrix and another mat rix.
        /// </summary>
        /// <param name="other">Matrix to compare</param>
        /// <returns>True if components are equal, false otherwise.</returns>
        public bool Equals(ref Matrix4x4 other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between the matrix and another matrix.
        /// </summary>
        /// <param name="other">Matrix to compare</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(Matrix4x4 other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between the matrix and another matrix.
        /// </summary>
        /// <param name="other">Matrix to compare</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if components are equal within tolerance, false otherwise.</returns>
        public bool Equals(ref Matrix4x4 other, float tolerance)
        {
            return (Math.Abs(M11 - other.M11) <= tolerance) &&
                   (Math.Abs(M12 - other.M12) <= tolerance) &&
                   (Math.Abs(M13 - other.M13) <= tolerance) &&
                   (Math.Abs(M14 - other.M14) <= tolerance) &&
                   (Math.Abs(M21 - other.M21) <= tolerance) &&
                   (Math.Abs(M22 - other.M22) <= tolerance) &&
                   (Math.Abs(M23 - other.M23) <= tolerance) &&
                   (Math.Abs(M24 - other.M24) <= tolerance) &&
                   (Math.Abs(M31 - other.M31) <= tolerance) &&
                   (Math.Abs(M32 - other.M32) <= tolerance) &&
                   (Math.Abs(M33 - other.M33) <= tolerance) &&
                   (Math.Abs(M34 - other.M34) <= tolerance) &&
                   (Math.Abs(M41 - other.M41) <= tolerance) &&
                   (Math.Abs(M42 - other.M42) <= tolerance) &&
                   (Math.Abs(M43 - other.M43) <= tolerance) &&
                   (Math.Abs(M44 - other.M44) <= tolerance);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix4x4)
            {
                return Equals((Matrix4x4)obj);
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
                return M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() +
                       M21.GetHashCode() + M22.GetHashCode() + M23.GetHashCode() + M24.GetHashCode() +
                       M31.GetHashCode() + M32.GetHashCode() + M33.GetHashCode() + M34.GetHashCode() +
                       M41.GetHashCode() + M42.GetHashCode() + M43.GetHashCode() + M44.GetHashCode();
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

            object[] args = new [] 
            {
                M11.ToString(format, formatProvider), M12.ToString(format, formatProvider), M13.ToString(format, formatProvider), M14.ToString(format, formatProvider),
                M21.ToString(format, formatProvider), M22.ToString(format, formatProvider), M23.ToString(format, formatProvider), M24.ToString(format, formatProvider),
                M31.ToString(format, formatProvider), M32.ToString(format, formatProvider), M33.ToString(format, formatProvider), M34.ToString(format, formatProvider),
                M41.ToString(format, formatProvider), M42.ToString(format, formatProvider), M43.ToString(format, formatProvider), M44.ToString(format, formatProvider)
            };

            return string.Format(formatProvider, "[M11: {0} M12: {1} M13: {2} M14: {3}] [M21: {4} M22: {5} M23: {6} M24: {7}] [M31: {8} M32: {9} M33: {10} M34: {11}] [M41: {12} M42: {13} M43: {14} M44: {15}]", args);
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void IPrimitiveValue.Write(IPrimitiveWriter output)
        {
            Vector4 row1 = new Vector4(M11, M12, M13, M14);
            Vector4 row2 = new Vector4(M21, M22, M23, M24);
            Vector4 row3 = new Vector4(M31, M32, M33, M34);
            Vector4 row4 = new Vector4(M41, M42, M43, M44);

            output.Write("Row1", ref row1);
            output.Write("Row2", ref row2);
            output.Write("Row3", ref row3);
            output.Write("Row4", ref row4);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void IPrimitiveValue.Read(IPrimitiveReader input)
        {
            input.Read(out Vector4 row1);
            input.Read(out Vector4 row2);
            input.Read(out Vector4 row3);
            input.Read(out Vector4 row4);

            M11 = row1.X;
            M12 = row1.Y;
            M13 = row1.Z;
            M13 = row1.W;

            M21 = row2.X;
            M22 = row2.Y;
            M23 = row2.Z;
            M24 = row2.W;

            M31 = row3.X;
            M32 = row3.Y;
            M33 = row3.Z;
            M34 = row3.W;

            M41 = row4.X;
            M42 = row4.Y;
            M43 = row4.Z;
            M44 = row4.W;
        }
    }
}
