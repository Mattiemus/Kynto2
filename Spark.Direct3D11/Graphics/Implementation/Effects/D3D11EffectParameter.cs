namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Math;
    using Spark.Graphics;

    public sealed class D3D11EffectParameter : IEffectParameter
    {
        private delegate void SetMatrixDelegate(int offset, ref Matrix4x4 matrix);
        private delegate void GetMatrixDelegate(int offset, out Matrix4x4 matrix);

        private D3D11EffectConstantBuffer _constantBuffer;
        private EffectParameterCollection _elements;
        private EffectParameterCollection _structureMembers;
        private string _name;
        private int _columnCount;
        private int _rowCount;
        private EffectParameterClass _paramClass;
        private EffectParameterType _paramType;
        private int _sizeInBytes;
        private int _matrixSizeInBytes; // Only valid for matrices, basically our IsAlignedToFloat4 for matrices - cache it upfront
        private int _startOffsetInBytes; // Includes element index
        private int _structureMemberIndex;
        private int _elementIndex;
        private int _totalElementCount;
        private bool _isArray;
        private bool _isValueType;
        private bool _isStructureMember;
        private Type _type;

        private D3D11EffectImplementation _impl;
        private EffectData.ValueVariable _valueVariable;
        private EffectData.ValueVariableMember _memberVariable;
        private EffectData.ResourceVariable _resourceVariable;
        private D3D11EffectResourceManager _resourceManager;
        private int _startResourceIndex; //Includes element index

        private SetMatrixDelegate _setMatrixDelegate;
        private GetMatrixDelegate _getMatrixDelegate;

        private D3D11EffectParameter()
        {
        }

        public string Name => _name;

        public IEffectConstantBuffer ContainingBuffer => _constantBuffer;

        public EffectParameterCollection Elements => _elements;

        public EffectParameterCollection StructureMembers => _structureMembers;

        public EffectParameterClass ParameterClass => _paramClass;

        public EffectParameterType ParameterType => _paramType;

        public Type DefaultNetType => _type;

        public int ColumnCount => _columnCount;

        public int RowCount => _rowCount;

        public int SizeInBytes => _sizeInBytes;

        public int StartOffsetInBytes => _startOffsetInBytes;

        public int ElementIndex => _elementIndex;

        public int StructureMemberIndex => _structureMemberIndex;

        public bool IsArray => _isArray;

        public bool IsElementInArray => !_isArray && _totalElementCount > 0;

        public bool IsStructureMember => _isStructureMember;

        public bool IsValueType => _isValueType;

        internal static D3D11EffectParameter CreateResourceVariable(D3D11EffectImplementation impl, D3D11EffectResourceManager resourceManager, int startResourceIndex, EffectData.ResourceVariable variable)
        {
            return CreateResourceVariable(impl, resourceManager, startResourceIndex, 0, variable, variable.ElementCount > 0);
        }

        private static D3D11EffectParameter CreateResourceVariable(D3D11EffectImplementation impl, D3D11EffectResourceManager resourceManager, int startResourceIndex, int elementIndex, EffectData.ResourceVariable variable, bool isArrayRoot)
        {
            var p = new D3D11EffectParameter();
            p._impl = impl;
            p._resourceManager = resourceManager;
            p._resourceVariable = variable;

            // Set properties
            p._name = variable.Name;
            p._columnCount = 0;
            p._rowCount = 0;
            p._paramClass = EffectParameterClass.Object;
            p._paramType = (variable.ResourceType == D3DShaderInputType.Sampler) ? EffectParameterType.SamplerState : Direct3DHelper.FromD3DResourceDimension((Graphics.D3DResourceDimension)variable.ResourceDimension);
            p._sizeInBytes = 0;
            p._structureMemberIndex = 0;
            p._elementIndex = elementIndex;
            p._totalElementCount = variable.ElementCount; // Keep track of array max range for validation
            p._isArray = isArrayRoot;
            p._isValueType = false;
            p._startOffsetInBytes = 0;
            p._startResourceIndex = startResourceIndex + elementIndex;
            p._type = GetDataType(p._paramType, p._paramClass, 0);

            // Set unused properties
            p._memberVariable = null;
            p._valueVariable = null;
            p._constantBuffer = null;
            p._isStructureMember = false;

            bool collectElements = isArrayRoot;

            if (collectElements)
            {
                var elements = new D3D11EffectParameter[p._totalElementCount];
                for (int i = 0; i < p._totalElementCount; i++)
                {
                    elements[i] = CreateResourceVariable(impl, resourceManager, startResourceIndex, i, variable, false);
                }

                p._elements = new EffectParameterCollection(elements);
            }
            else
            {
                p._elements = EffectParameterCollection.EmptyCollection;
            }

            p._structureMembers = EffectParameterCollection.EmptyCollection;

            return p;
        }

        /// <summary>
        /// For creating a complete value variable. May be a scalar, vector, struct and/or may be an array of elements.
        /// </summary>
        /// <param name="impl"></param>
        /// <param name="constantBuffer"></param>
        /// <param name="variable"></param>
        /// <param name="isCloning"></param>
        /// <returns></returns>
        internal static D3D11EffectParameter CreateValueVariable(D3D11EffectImplementation impl, D3D11EffectConstantBuffer constantBuffer, EffectData.ValueVariable variable, bool isCloning)
        {
            return CreateValueVariable(impl, constantBuffer, 0, variable, isCloning, variable.ElementCount > 0);
        }

        private static D3D11EffectParameter CreateValueVariable(D3D11EffectImplementation impl, D3D11EffectConstantBuffer constantBuffer, int elemIndex, EffectData.ValueVariable variable, bool isCloning, bool isArrayRoot)
        {
            var p = new D3D11EffectParameter();
            p._impl = impl;
            p._constantBuffer = constantBuffer;
            p._valueVariable = variable;

            // Set properties
            p._name = variable.Name;
            p._columnCount = variable.ColumnCount;
            p._rowCount = variable.RowCount;
            p._paramClass = Direct3DHelper.FromD3DShaderVariableClass((Graphics.D3DShaderVariableClass)variable.VariableClass);
            p._paramType = Direct3DHelper.FromD3DShaderVariableType((Graphics.D3DShaderVariableType)variable.VariableType);
            p._sizeInBytes = variable.SizeInBytes;
            p._structureMemberIndex = 0;
            p._elementIndex = elemIndex;
            p._totalElementCount = variable.ElementCount; // Keep track of array max range for validation
            p._isArray = isArrayRoot;
            p._isValueType = true;
            p._isStructureMember = false;
            p._type = GetDataType(p._paramType, p._paramClass, p._columnCount);

            p._startOffsetInBytes = variable.StartOffset;

            if (elemIndex > 0)
            {
                p._startOffsetInBytes += (elemIndex * variable.AlignedElementSizeInBytes);
            }

            p.FixupSizeForElementInArray(variable.AlignedElementSizeInBytes);

            p.SetMatrixImplementation();

            // Set unused properties
            p._memberVariable = null;
            p._resourceVariable = null;
            p._resourceManager = null;
            p._startResourceIndex = -1;

            p.PopulateSubParameters(isCloning, isArrayRoot, variable);

            if (!isCloning && !p.IsElementInArray)
            {
                p.SetDefaultValue(variable.DefaultValue);
            }

            return p;
        }

        /// <summary>
        /// For creating a complete structure member. A member is contained in a parent structure, but may be an array of elements or have other structure members too.
        /// </summary>
        /// <param name="impl"></param>
        /// <param name="constantBuffer"></param>
        /// <param name="elemIndex"></param>
        /// <param name="memberIndex"></param>
        /// <param name="startOffsetOfParentInBytes"></param>
        /// <param name="variableMember"></param>
        /// <param name="isCloning"></param>
        /// <returns></returns>
        private static D3D11EffectParameter CreateStructureMember(D3D11EffectImplementation impl, D3D11EffectConstantBuffer constantBuffer, int elemIndex, int memberIndex, int startOffsetOfParentInBytes, EffectData.ValueVariableMember variableMember, bool isCloning)
        {
            return CreateStructureMember(impl, constantBuffer, elemIndex, memberIndex, startOffsetOfParentInBytes, variableMember, isCloning, variableMember.ElementCount > 0);
        }

        private static D3D11EffectParameter CreateStructureMember(D3D11EffectImplementation impl, D3D11EffectConstantBuffer constantBuffer, int elemIndex, int memberIndex, int startOffsetOfParentInBytes, EffectData.ValueVariableMember variableMember, bool isCloning, bool isArrayRoot, bool isSubArrayElement = false)
        {
            var p = new D3D11EffectParameter();
            p._impl = impl;
            p._constantBuffer = constantBuffer;
            p._memberVariable = variableMember;

            // Set properties
            p._name = variableMember.Name;
            p._columnCount = variableMember.ColumnCount;
            p._rowCount = variableMember.RowCount;
            p._paramClass = Direct3DHelper.FromD3DShaderVariableClass((Graphics.D3DShaderVariableClass)variableMember.VariableClass);
            p._paramType = Direct3DHelper.FromD3DShaderVariableType((Graphics.D3DShaderVariableType)variableMember.VariableType);
            p._sizeInBytes = variableMember.SizeInBytes;
            p._structureMemberIndex = memberIndex;
            p._elementIndex = elemIndex;
            p._totalElementCount = variableMember.ElementCount; // Keep track of array max range for validation
            p._isArray = isArrayRoot;
            p._isValueType = true;
            p._isStructureMember = true;
            p._type = GetDataType(p._paramType, p._paramClass, p._columnCount);

            // If the start of an array that is embedded inside a struct member, ignore the offset from the parent structure as it'll be repeated
            // from the previous call, the startOffset supplied should already be at the correct location
            if (isSubArrayElement)
            {
                p._startOffsetInBytes = startOffsetOfParentInBytes;

                if (elemIndex > 0)
                {
                    p._startOffsetInBytes += (elemIndex * variableMember.AlignedElementSizeInBytes);
                }
            }
            else
            {
                p._startOffsetInBytes = startOffsetOfParentInBytes + variableMember.OffsetFromParentStructure;
            }

            p.FixupSizeForElementInArray(variableMember.AlignedElementSizeInBytes);

            p.SetMatrixImplementation();

            // Set unused properties
            p._valueVariable = null;
            p._resourceVariable = null;
            p._resourceManager = null;
            p._startResourceIndex = -1;

            p.PopulateSubParameters(isCloning, isArrayRoot, variableMember);

            return p;
        }

        private void FixupSizeForElementInArray(int alignedElementSizeInBytes)
        {
            // If an element of an array, fix up the size to match a single element. Array elements are padded (basically using float4s), except
            // for the last element which is not padded!
            if (IsElementInArray)
            {
                if (_elementIndex == _totalElementCount - 1)
                {
                    _sizeInBytes = _sizeInBytes - (alignedElementSizeInBytes * (_totalElementCount - 1));
                }
                else
                {
                    _sizeInBytes = alignedElementSizeInBytes;
                }
            }
        }

        private void PopulateSubParameters(bool isCloning, bool isArrayRoot, object variable)
        {
            // Passing in EffectData.ValueVariable or ValueVariableMember to reduce code duplication
            EffectData.ValueVariableMember[] structMembers = GetStructMembers(variable);

            bool isValueVariable = variable is EffectData.ValueVariable;

            //Collect elements if the parameter is an array type
            bool collectElements = isArrayRoot && _totalElementCount > 0;

            // Do not collect structure members for an array type, only do it for the parameter that represents each array index (the parent parameter of each element is considered the
            // array object, not the first index. However the array object can be set directly with raw data much like a memcpy)
            bool collectMembers = !collectElements && (_paramClass == EffectParameterClass.Struct) && (structMembers != null) && (structMembers.Length > 0);

            // Collect elements
            if (collectElements)
            {
                var elements = new D3D11EffectParameter[_totalElementCount];
                for (int i = 0; i < _totalElementCount; i++)
                {
                    if (isValueVariable)
                    {
                        elements[i] = CreateValueVariable(_impl, _constantBuffer, i, variable as EffectData.ValueVariable, isCloning, false);
                    }
                    else
                    {
                        elements[i] = CreateStructureMember(_impl, _constantBuffer, i, 0, _startOffsetInBytes, variable as EffectData.ValueVariableMember, isCloning, false, true);
                    }
                }
                _elements = new EffectParameterCollection(elements);
            }
            else
            {
                _elements = EffectParameterCollection.EmptyCollection;
            }

            // Collect structure members
            if (collectMembers)
            {
                var members = new D3D11EffectParameter[structMembers.Length];
                for (int i = 0; i < structMembers.Length; i++)
                {
                    members[i] = CreateStructureMember(_impl, _constantBuffer, _elementIndex, i, _startOffsetInBytes, structMembers[i], isCloning);
                }
                _structureMembers = new EffectParameterCollection(members);
            }
            else
            {
                _structureMembers = EffectParameterCollection.EmptyCollection;
            }
        }

        private EffectData.ValueVariableMember[] GetStructMembers(object variable)
        {
            var vvarm = variable as EffectData.ValueVariableMember;
            if (vvarm != null)
            {
                return vvarm.Members;
            }

            var vvar = variable as EffectData.ValueVariable;
            if (vvar != null)
            {
                return vvar.Members;
            }

            return null;
        }

        public T GetValue<T>() where T : struct
        {
            ValidateValue(MemoryHelper.SizeOf<T>());
            
            MemoryHelper.Read(_constantBuffer.RawBufferPointer + _startOffsetInBytes, out T value);

            return value;
        }

        public T[] GetValueArray<T>(int count) where T : struct
        {
            ValidateValueArray(MemoryHelper.SizeOf<T>(), count);

            int sizeInBytes;
            T[] values = new T[count];
            IntPtr pDest = _constantBuffer.RawBufferPointer;
            if (IsAlignedToFloat4<T>(out sizeInBytes))
            {
                int offset = _startOffsetInBytes;
                for (int i = 0; i < values.Length; i++)
                {
                    MemoryHelper.Read(pDest + offset, out values[i]);
                    offset += sizeInBytes;
                }
            }
            else
            {
                MemoryHelper.Read(pDest + _startOffsetInBytes, values, 0, count);
            }

            return values;
        }

        public Matrix4x4 GetMatrixValue()
        {
            ValidateValue(_matrixSizeInBytes);

            _getMatrixDelegate(_startOffsetInBytes, out Matrix4x4 value);

            return value;
        }

        public Matrix4x4 GetMatrixValueTranspose()
        {
            ValidateValue(_matrixSizeInBytes);

            _getMatrixDelegate(_startOffsetInBytes, out Matrix4x4 value);

            value.Transpose();
            return value;
        }

        public Matrix4x4[] GetMatrixValueArray(int count)
        {
            ValidateValueArray(_matrixSizeInBytes, count);

            var values = new Matrix4x4[count];
            int offset = _startOffsetInBytes;
            for (int i = 0; i < count; i++)
            {
                Matrix4x4 value;
                _getMatrixDelegate(offset, out value);
                values[i] = value;
                offset += _matrixSizeInBytes;
            }

            return values;
        }

        public Matrix4x4[] GetMatrixValueArrayTranspose(int count)
        {
            ValidateValueArray(_matrixSizeInBytes, count);

            var values = new Matrix4x4[count];
            int offset = _startOffsetInBytes;
            for (int i = 0; i < count; i++)
            {
                Matrix4x4 value;
                _getMatrixDelegate(offset, out value);
                value.Transpose();
                values[i] = value;
                offset += _matrixSizeInBytes;
            }

            return values;
        }

        public T GetResource<T>() where T : IShaderResource
        {
            ValidateResource();

            return (T)_resourceManager.Resources[_startResourceIndex];
        }

        public T[] GetResourceArray<T>(int count) where T : IShaderResource
        {
            ValidateResourceArray(count);

            var values = new T[count];
            int startIndex = _startResourceIndex;
            for (int i = 0; i < count; i++)
            {
                values[i] = (T)_resourceManager.Resources[startIndex + i];
            }

            return values;
        }

        public void SetValue<T>(T value) where T : struct
        {
            ValidateValue(MemoryHelper.SizeOf<T>());

            MemoryHelper.Write(_constantBuffer.RawBufferPointer + _startOffsetInBytes, ref value);
            _constantBuffer.IsDirty = true;
        }

        public void SetValue<T>(ref T value) where T : struct
        {
            ValidateValue(MemoryHelper.SizeOf<T>());

            MemoryHelper.Write(_constantBuffer.RawBufferPointer + _startOffsetInBytes, ref value);
            _constantBuffer.IsDirty = true;
        }

        public void SetValue<T>(params T[] values) where T : struct
        {
            ValidateValueArray(MemoryHelper.SizeOf<T>(), (values == null) ? 0 : values.Length);

            int sizeInBytes;
            IntPtr pDest = _constantBuffer.RawBufferPointer;

            // If a 16-byte vector already, can just write directly. Otherwise, we need to calculate the correct stride to step by
            // for each element.
            if (IsAlignedToFloat4<T>(out sizeInBytes))
            {
                int offset = _startOffsetInBytes;
                for (int i = 0; i < values.Length; i++)
                {
                    MemoryHelper.Write(pDest + offset, ref values[i]);
                    offset += sizeInBytes;
                }
            }
            else
            {
                MemoryHelper.Write(pDest + _startOffsetInBytes, values, 0, values.Length);
            }

            _constantBuffer.IsDirty = true;
        }

        public void SetValue(Matrix4x4 value)
        {
            ValidateValue(_matrixSizeInBytes);

            _setMatrixDelegate(_startOffsetInBytes, ref value);
            _constantBuffer.IsDirty = true;
        }

        public void SetValue(ref Matrix4x4 value)
        {
            ValidateValue(_matrixSizeInBytes);

            _setMatrixDelegate(_startOffsetInBytes, ref value);
            _constantBuffer.IsDirty = true;
        }

        public void SetValue(params Matrix4x4[] values)
        {
            ValidateValueArray(_matrixSizeInBytes, (values == null) ? 0 : values.Length);

            int offset = _startOffsetInBytes;
            for (int i = 0; i < values.Length; i++)
            {
                _setMatrixDelegate(offset, ref values[i]);
                offset += _matrixSizeInBytes;
            }

            _constantBuffer.IsDirty = true;
        }

        public void SetValueTranspose(Matrix4x4 value)
        {
            ValidateValue(_matrixSizeInBytes);

            value.Transpose();
            _setMatrixDelegate(_startOffsetInBytes, ref value);
            _constantBuffer.IsDirty = true;
        }

        public void SetValueTranspose(ref Matrix4x4 value)
        {
            ValidateValue(_matrixSizeInBytes);

            Matrix4x4 transposed;
            Matrix4x4.Transpose(ref value, out transposed);
            _setMatrixDelegate(_startOffsetInBytes, ref transposed);
            _constantBuffer.IsDirty = true;
        }

        public void SetValueTranspose(params Matrix4x4[] values)
        {
            ValidateValueArray(_matrixSizeInBytes, (values == null) ? 0 : values.Length);

            int offset = _startOffsetInBytes;
            for (int i = 0; i < values.Length; i++)
            {
                Matrix4x4 transposed;
                Matrix4x4.Transpose(ref values[i], out transposed);

                _setMatrixDelegate(offset, ref transposed);
                offset += _matrixSizeInBytes;
            }

            _constantBuffer.IsDirty = true;
        }

        public void SetResource<T>(T resource) where T : IShaderResource
        {
            ValidateResource();

            _resourceManager.Resources[_startResourceIndex] = resource;
        }

        public void SetResource<T>(params T[] resources) where T : IShaderResource
        {
            ValidateResourceArray((resources == null) ? 0 : resources.Length);

            Array.Copy(resources, 0, _resourceManager.Resources, _startResourceIndex, resources.Length);
        }

        public bool IsPartOf(Effect effect)
        {
            if (effect == null)
            {
                return false;
            }

            return ReferenceEquals(effect.Implementation, _impl);
        }

        private void ValidateResource()
        {
            bool isResource;
            switch (_paramType)
            {
                case EffectParameterType.Texture1D:
                case EffectParameterType.Texture1DArray:
                case EffectParameterType.Texture2D:
                case EffectParameterType.Texture2DArray:
                case EffectParameterType.Texture2DMS:
                case EffectParameterType.Texture2DMSArray:
                case EffectParameterType.Texture3D:
                case EffectParameterType.TextureCube:
                case EffectParameterType.SamplerState:
                    isResource = true;
                    break;
                default:
                    isResource = false;
                    break;
            }

            if (_paramClass != EffectParameterClass.Object || !isResource)
            {
                throw new InvalidCastException("Parameter is not a resource");
            }
        }

        private void ValidateResourceArray(int count)
        {
            ValidateResource();

            if (!IsArray || count <= _elementIndex || count > (_totalElementCount - _elementIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Value is too large");
            }
        }

        private void ValidateValue(int sizeOfValue)
        {
            bool isValue;
            switch (_paramType)
            {
                case EffectParameterType.Bool:
                case EffectParameterType.Int32:
                case EffectParameterType.Single:
                    isValue = true;
                    break;
                default:
                    isValue = false;
                    break;
            }

            bool isVectorOrMatrix;
            switch (_paramClass)
            {
                case EffectParameterClass.MatrixColumns:
                case EffectParameterClass.MatrixRows:
                case EffectParameterClass.Scalar:
                case EffectParameterClass.Vector:
                    isVectorOrMatrix = true;
                    break;
                default:
                    isVectorOrMatrix = false;
                    break;
            }

            if (!isVectorOrMatrix || !isValue)
            {
                throw new InvalidCastException("Parameter is not a value type");
            }

            if (sizeOfValue > _sizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeOfValue), "Value is too large");
            }
        }

        private void ValidateValueArray(int sizeOfValue, int count)
        {
            ValidateValue(sizeOfValue * count);

            if (!IsArray || count <= _elementIndex || count > (_totalElementCount - _elementIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Element count is too large");
            }
        }

        private unsafe void SetMatrixColumMajor(int offset, ref Matrix4x4 matrix)
        {
            float* buffPtr = (float*)((byte*)_constantBuffer.RawBufferPointer + offset);
            fixed (void* pMatrix = &matrix)
            {
                float* matrixPtr = (float*)pMatrix;

                // Transpose
                for (int col = 0; col < _columnCount; col++)
                {
                    for (int row = 0; row < _rowCount; row++)
                    {
                        buffPtr[row] = matrixPtr[row * 4];
                    }

                    matrixPtr++;
                    buffPtr += 4;
                }
            }

            _constantBuffer.IsDirty = true;
        }

        private unsafe void GetMatrixColumnMajor(int offset, out Matrix4x4 matrix)
        {
            matrix = new Matrix4x4();
            float* buffPtr = (float*)((byte*)_constantBuffer.RawBufferPointer + offset);
            fixed (void* pMatrix = &matrix)
            {
                float* matrixPtr = (float*)pMatrix;

                // Transpose
                for (int col = 0; col < _columnCount; col++)
                {
                    for (int row = 0; row < _rowCount; row++)
                    {
                        matrixPtr[row * 4] = buffPtr[row];
                    }

                    matrixPtr++;
                    buffPtr += 4;
                }
            }
        }

        private unsafe void SetMatrixRowMajor(int offset, ref Matrix4x4 matrix)
        {
            float* buffPtr = (float*)((byte*)_constantBuffer.RawBufferPointer + offset);
            fixed (void* pMatrix = &matrix)
            {
                float* matrixPtr = (float*)pMatrix;

                // Only if matrix is expecting less columns/rows than our 4x4 row-major matrix
                for (int row = 0; row < _rowCount; row++)
                {
                    for (int col = 0; col < _columnCount; col++)
                    {
                        buffPtr[col] = matrixPtr[col * 4];
                    }

                    matrixPtr++;
                    buffPtr += 4;
                }
            }

            _constantBuffer.IsDirty = true;
        }

        private unsafe void GetMatrixRowMajor(int offset, out Matrix4x4 matrix)
        {
            matrix = new Matrix4x4();
            float* buffPtr = (float*)((byte*)_constantBuffer.RawBufferPointer + offset);
            fixed (void* pMatrix = &matrix)
            {
                float* matrixPtr = (float*)pMatrix;

                // Only if matrix is expecting less columns/rows than our 4x4 row-major matrix
                for (int row = 0; row < _rowCount; row++)
                {
                    for (int col = 0; col < _columnCount; col++)
                    {
                        matrixPtr[col * 4] = buffPtr[col];
                    }

                    matrixPtr++;
                    buffPtr += 4;
                }
            }
        }

        private unsafe void SetMatrixDirect(int offset, ref Matrix4x4 matrix)
        {
            _constantBuffer.Set(offset, ref matrix);
        }

        private unsafe void GetMatrixDirect(int offset, out Matrix4x4 matrix)
        {
            matrix = _constantBuffer.Get<Matrix4x4>(offset);
        }

        private static bool IsAlignedToFloat4<T>(out int sizeInBytes) where T : struct
        {
            sizeInBytes = MemoryHelper.SizeOf<T>();
            int offset = sizeInBytes % 16;
            bool notAligned = offset != 0;
            if (notAligned)
            {
                sizeInBytes = sizeInBytes + (16 - offset);
            }

            return notAligned;
        }

        private void SetMatrixImplementation()
        {
            bool isColumnMatrix = _paramClass == EffectParameterClass.MatrixColumns;
            bool isRowMatrix = _paramClass == EffectParameterClass.MatrixRows;

            if (isColumnMatrix || isRowMatrix)
            {
                _matrixSizeInBytes = ((isColumnMatrix) ? _columnCount : _rowCount) * 4 * sizeof(float);
                bool directCopy = _columnCount == 4 && _rowCount == 4 && isRowMatrix;

                if (directCopy)
                {
                    _setMatrixDelegate = new SetMatrixDelegate(SetMatrixDirect);
                    _getMatrixDelegate = new GetMatrixDelegate(GetMatrixDirect);
                }
                else
                {
                    _setMatrixDelegate = (isColumnMatrix) ? new SetMatrixDelegate(SetMatrixColumMajor) : new SetMatrixDelegate(SetMatrixRowMajor);
                    _getMatrixDelegate = (isColumnMatrix) ? new GetMatrixDelegate(GetMatrixColumnMajor) : new GetMatrixDelegate(GetMatrixRowMajor);
                }
            }
        }

        private void SetDefaultValue(byte[] defaultValue)
        {
            if (defaultValue == null || defaultValue.Length == 0)
            {
                return;
            }

            MemoryHelper.Write(_constantBuffer.RawBufferPointer + _startOffsetInBytes, defaultValue, 0, defaultValue.Length);
            _constantBuffer.IsDirty = true;
        }

        private static Type GetDataType(EffectParameterType paramType, EffectParameterClass classType, int columnCount)
        {
            switch (classType)
            {
                case EffectParameterClass.Scalar:
                case EffectParameterClass.Vector:
                    switch (paramType)
                    {
                        case EffectParameterType.Int32:
                            switch (columnCount)
                            {
                                case 1:
                                    return typeof(int);
                                case 2:
                                    return typeof(Int2);
                                case 3:
                                    return typeof(Int3);
                                case 4:
                                    return typeof(Int4);
                                default:
                                    return null;
                            }
                        case EffectParameterType.Single:
                            switch (columnCount)
                            {
                                case 1:
                                    return typeof(float);
                                case 2:
                                    return typeof(Vector2);
                                case 3:
                                    return typeof(Vector3);
                                case 4:
                                    return typeof(Vector4);
                                default:
                                    return null;
                            }
                        default:
                            return null;
                    }
                case EffectParameterClass.MatrixColumns:
                case EffectParameterClass.MatrixRows:
                    return typeof(Matrix4x4);
                case EffectParameterClass.Object:
                    switch (paramType)
                    {
                        case EffectParameterType.SamplerState:
                            return typeof(SamplerState);
                        case EffectParameterType.Texture1D:
                            return typeof(Texture1D);
                        case EffectParameterType.Texture1DArray:
                            return typeof(Texture1DArray);
                        case EffectParameterType.Texture2D:
                            return typeof(Texture2D);
                        case EffectParameterType.Texture2DArray:
                            return typeof(Texture2DArray);
                        case EffectParameterType.Texture3D:
                            return typeof(Texture3D);
                        case EffectParameterType.TextureCube:
                            return typeof(TextureCube);
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }
    }
}
