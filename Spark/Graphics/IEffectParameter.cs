namespace Spark.Graphics
{
    using System;

    using Math;

    /// <summary>
    /// Represents a parameter used by a shader in the effect. Parameters may be uniforms (constant values) or resources (e.g. textures). A parameter
    /// may be a single value or an array of parameters. 
    /// </summary>
    public interface IEffectParameter : IEffectPart
    {
        /// <summary>
        /// Gets the containing constant buffer, if it exists. This will never exist for resource parameters.
        /// </summary>
        IEffectConstantBuffer ContainingBuffer { get; }

        /// <summary>
        /// Gets the elements of the array, if the parameter is one.
        /// </summary>
        EffectParameterCollection Elements { get; }

        /// <summary>
        /// Gets the struct members, if the parameter is a struct type.
        /// </summary>
        EffectParameterCollection StructureMembers { get; }

        /// <summary>
        /// Gets the class of the parameter (e.g. scalar, vector, matrix, object and so on).
        /// </summary>
        EffectParameterClass ParameterClass { get; }

        /// <summary>
        /// Gets the data type of the parameter (e.g. int, float, texture2d and so on).
        /// </summary>
        EffectParameterType ParameterType { get; }

        /// <summary>
        /// Gets the .NET data type of the parameter, may be null (e.g. parameter is a structure).
        /// </summary>
        Type DefaultNetType { get; }

        /// <summary>
        /// Gets the number of columns if the parameter class is not an object. E.g. a Matrix4x4 would have 4 rows and 4 columns.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Gets the number of rows if the parameter class is not an object. E.g. a Matrix4x4 would have 4 rows and 4 columns.
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Gets the parameter's size in bytes.
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// Gets the parameter's starting offset in its containing buffer, in bytes.
        /// </summary>
        int StartOffsetInBytes { get; }

        /// <summary>
        /// Gets the index of the element in its parent's array element collection.
        /// </summary>
        int ElementIndex { get; }

        /// <summary>
        /// Gets the index of the member in its parent's struct member collection.
        /// </summary>
        int StructureMemberIndex { get; }

        /// <summary>
        /// Gets if the parameter is an array.
        /// </summary>
        bool IsArray { get; }

        /// <summary>
        /// Gets if the parameter is an element in an array.
        /// </summary>
        bool IsElementInArray { get; }

        /// <summary>
        /// Gets if the parameter is a structure member.
        /// </summary>
        bool IsStructureMember { get; }

        /// <summary>
        /// Gets if the parameter is a uniform constant value and not a resource value.
        /// </summary>
        bool IsValueType { get; }

        /// <summary>
        /// Gets the value of the parameter, if it is a value type.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <returns>Parameter value.</returns>
        T GetValue<T>() where T : struct;

        /// <summary>
        /// Gets the array of values of the parameter, if it is an array value type.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="count">Number of values of the array to return.</param>
        /// <returns>Array of parameter values.</returns>
        T[] GetValueArray<T>(int count) where T : struct;

        /// <summary>
        /// Sets the value to the parameter, if it is a value type.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value to set.</param>
        void SetValue<T>(T value) where T : struct;

        /// <summary>
        /// Sets the value to the parameter, if it is a value type.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value to set.</param>
        void SetValue<T>(ref T value) where T : struct;

        /// <summary>
        /// Sets the array of values to the parameter, if it is an array value type. This is a more efficient operation than setting individual value
        /// to each array element individually.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="values">Values to set.</param>
        void SetValue<T>(params T[] values) where T : struct;

        /// <summary>
        /// Gets the matrix of the parameter, if it is a matrix type.
        /// </summary>
        /// <returns>Matrix value.</returns>
        Matrix4x4 GetMatrixValue();

        /// <summary>
        /// Gets the transposed matrix of the parameter, if it is a matrix type.
        /// </summary>
        /// <returns>Transposed matrix value.</returns>
        Matrix4x4 GetMatrixValueTranspose();

        /// <summary>
        /// Gets the array of matrices of the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="count">Number of matrices of the array to return.</param>
        /// <returns>Array of matrices.</returns>
        Matrix4x4[] GetMatrixValueArray(int count);

        /// <summary>
        /// Gets the array of transposed matrices of the parameter, if it is an array matrix type.
        /// </summary>
        /// <param name="count">Number of matrices of the array to return.</param>
        /// <returns>Array of transposed matrices.</returns>
        Matrix4x4[] GetMatrixValueArrayTranspose(int count);

        /// <summary>
        /// Sets the matrix to the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="value">Matrix to set.</param>
        void SetValue(Matrix4x4 value);

        /// <summary>
        /// Sets the matrix to the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="value">Matrix to set.</param>
        void SetValue(ref Matrix4x4 value);

        /// <summary>
        /// Sets the array of matrices to the parameter, if it is an array matrix type. This is a more efficient operation than setting individual matrices
        /// to each array element individually.
        /// </summary>
        /// <param name="values">Array of matrices to set.</param>
        void SetValue(params Matrix4x4[] values);

        /// <summary>
        /// Transposes the matrix then sets it to the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="value">Matrix to set.</param>
        void SetValueTranspose(Matrix4x4 value);

        /// <summary>
        /// Transposes the matrix then sets it to the parameter, if it is a matrix type.
        /// </summary>
        /// <param name="value">Matrix to set.</param>
        void SetValueTranspose(ref Matrix4x4 value);

        /// <summary>
        /// Transposes the array of matrices then sets them to the parameter, if it is an array matrix type. This is a more efficient operation than setting individual matrices
        /// to each array element individually.
        /// </summary>
        /// <param name="values">Array of matrices to set.</param>
        void SetValueTranspose(params Matrix4x4[] values);

        /// <summary>
        /// Gets a shader resource set to the parameter, if it is a resource type.
        /// </summary>
        /// <typeparam name="T">Shader resource type.</typeparam>
        /// <returns>Shader resource set to the parameter. Null is returned if none is set.</returns>
        T GetResource<T>() where T : IShaderResource;

        /// <summary>
        /// Gets an array of shader resources set to the parameter, if it is an array resource type.
        /// </summary>
        /// <typeparam name="T">Shader resource type.</typeparam>
        /// <param name="count">Number of shader resources of the array to return.</param>
        /// <returns>Array of shader resources set to the parameter. The array may have null elements if no resource is set at those indices.</returns>
        T[] GetResourceArray<T>(int count) where T : IShaderResource;

        /// <summary>
        /// Sets a shader resource to the parameter, if it is a resource type.
        /// </summary>
        /// <typeparam name="T">Shader resource type.</typeparam>
        /// <param name="resource">Shader resource to set. Null is an acceptable value.</param>
        void SetResource<T>(T resource) where T : IShaderResource;

        /// <summary>
        /// Sets an array of shader resources to the parameter, if it is an array resource type. This is a more efficient operation than setting individual shader resources 
        /// to each array element individually.
        /// </summary>
        /// <typeparam name="T">Shader resource type.</typeparam>
        /// <param name="resources">Array of shader resources to set. The array must exist, but null values in the array are acceptable.</param>
        void SetResource<T>(params T[] resources) where T : IShaderResource;
    }
}
