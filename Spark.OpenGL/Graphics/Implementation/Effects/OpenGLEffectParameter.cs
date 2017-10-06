namespace Spark.OpenGL.Graphics.Implementation.Effects
{
    using System;
    using Spark.Graphics;
    using Spark.Math;

    /// <summary>
    /// Implementation of a parameter within an OpenGL effect
    /// </summary>
    public sealed class OpenGLEffectParameter : IEffectParameter
    {
        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the containing constant buffer, if it exists. This will never exist for resource parameters.
        /// </summary>
        public IEffectConstantBuffer ContainingBuffer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the elements of the array, if the parameter is one.
        /// </summary>
        public EffectParameterCollection Elements
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the struct members, if the parameter is a struct type.
        /// </summary>
        public EffectParameterCollection StructureMembers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the class of the parameter (e.g. scalar, vector, matrix, object and so on).
        /// </summary>
        public EffectParameterClass ParameterClass
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the data type of the parameter (e.g. int, float, texture2d and so on).
        /// </summary>
        public EffectParameterType ParameterType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the .NET data type of the parameter, may be null (e.g. parameter is a structure).
        /// </summary>
        public Type DefaultNetType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the number of columns if the parameter class is not an object. E.g. a Matrix4x4 would have 4 rows and 4 columns.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the number of rows if the parameter class is not an object. E.g. a Matrix4x4 would have 4 rows and 4 columns.
        /// </summary>
        public int RowCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the parameter's size in bytes.
        /// </summary>
        public int SizeInBytes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the parameter's starting offset in its containing buffer, in bytes.
        /// </summary>
        public int StartOffsetInBytes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the index of the element in its parent's array element collection.
        /// </summary>
        public int ElementIndex
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the index of the member in its parent's struct member collection.
        /// </summary>
        public int StructureMemberIndex
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets if the parameter is an array.
        /// </summary>
        public bool IsArray
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets if the parameter is an element in an array.
        /// </summary>
        public bool IsElementInArray
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets if the parameter is a structure member.
        /// </summary>
        public bool IsStructureMember
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets if the parameter is a uniform constant value and not a resource value.
        /// </summary>
        public bool IsValueType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Checks if this part belongs to the given effect.
        /// </summary>
        /// <param name="effect">Effect to check against</param>
        /// <returns>True if the effect is the parent of this part, false otherwise.</returns>
        public bool IsPartOf(Effect effect)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the parameter, if it is a value type.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <returns>Parameter value.</returns>
        public T GetValue<T>() where T : struct
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the array of values of the parameter, if it is an array value type.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="count">Number of values of the array to return.</param>
        /// <returns>Array of parameter values.</returns>
        public T[] GetValueArray<T>(int count) where T : struct
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value to the parameter, if it is a value type.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value to set.</param>
        public void SetValue<T>(T value) where T : struct
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value to the parameter, if it is a value type.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value to set.</param>
        public void SetValue<T>(ref T value) where T : struct
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the array of values to the parameter, if it is an array value type. This is a more efficient operation than setting individual value
        /// to each array element individually.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="values">Values to set.</param>
        public void SetValue<T>(params T[] values) where T : struct
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the matrix of the parameter, if it is a matrix type.
        /// </summary>
        /// <returns>Matrix value.</returns>
        public Matrix4x4 GetMatrixValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the transposed matrix of the parameter, if it is a matrix type.
        /// </summary>
        /// <returns>Transposed matrix value.</returns>
        public Matrix4x4 GetMatrixValueTranspose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the array of matrices of the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="count">Number of matrices of the array to return.</param>
        /// <returns>Array of matrices.</returns>
        public Matrix4x4[] GetMatrixValueArray(int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the array of transposed matrices of the parameter, if it is an array matrix type.
        /// </summary>
        /// <param name="count">Number of matrices of the array to return.</param>
        /// <returns>Array of transposed matrices.</returns>
        public Matrix4x4[] GetMatrixValueArrayTranspose(int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the matrix to the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="value">Matrix to set.</param>
        public void SetValue(Matrix4x4 value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the matrix to the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="value">Matrix to set.</param>
        public void SetValue(ref Matrix4x4 value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the array of matrices to the parameter, if it is an array matrix type. This is a more efficient operation than setting individual matrices
        /// to each array element individually.
        /// </summary>
        /// <param name="values">Array of matrices to set.</param>
        public void SetValue(params Matrix4x4[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transposes the matrix then sets it to the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="value">Matrix to set.</param>
        public void SetValueTranspose(Matrix4x4 value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transposes the matrix then sets it to the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="value">Matrix to set.</param>
        public void SetValueTranspose(ref Matrix4x4 value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transposes the array of matrices then sets them to the parameter, if it is an array matrix type. This is a more efficient operation than setting individual matrices
        /// to each array element individually.
        /// </summary>
        /// <param name="values">Array of matrices to set.</param>
        public void SetValueTranspose(params Matrix4x4[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a shader resource set to the parameter, if it is a resource type.
        /// </summary>
        /// <typeparam name="T">Shader resource type.</typeparam>
        /// <returns>Shader resource set to the parameter. Null is returned if none is set.</returns>
        public T GetResource<T>() where T : IShaderResource
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an array of shader resources set to the parameter, if it is an array resource type.
        /// </summary>
        /// <typeparam name="T">Shader resource type.</typeparam>
        /// <param name="count">Number of shader resources of the array to return.</param>
        /// <returns>Array of shader resources set to the parameter. The array may have null elements if no resource is set at those indices.</returns>
        public T[] GetResourceArray<T>(int count) where T : IShaderResource
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a shader resource to the parameter, if it is a resource type.
        /// </summary>
        /// <typeparam name="T">Shader resource type.</typeparam>
        /// <param name="resource">Shader resource to set. Null is an acceptable value.</param>
        public void SetResource<T>(T resource) where T : IShaderResource
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets an array of shader resources to the parameter, if it is an array resource type. This is a more efficient operation than setting individual shader resources 
        /// to each array element individually.
        /// </summary>
        /// <typeparam name="T">Shader resource type.</typeparam>
        /// <param name="resources">Array of shader resources to set. The array must exist, but null values in the array are acceptable.</param>
        public void SetResource<T>(params T[] resources) where T : IShaderResource
        {
            throw new NotImplementedException();
        }
    }
}
