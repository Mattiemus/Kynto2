namespace Spark.Core
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    using Content;
    using Interop;
    using Math;

    /// <summary>
    /// A DataBuffer is a generic way to buffer typed data while gaining the ability to treat the data as a stream of bytes. The DataBuffer supports both
    /// absolute (by index) and relative (by position pointer) operations to read/write data from/to the underlying raw buffer.
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public sealed class DataBuffer<T> : IDataBuffer<T>, IDeepCloneable where T : struct
    {
        private int _version = 0;
        private int _position;
        private int _length;
        private int _elementSizeInBytes;
        private Type _elementType;
        private RawBuffer _data;
        private bool _isDisposed;
        private bool _mapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBuffer{T}"/> class
        /// </summary>
        private DataBuffer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBuffer{T}"/> class with the specified length.
        /// </summary>
        /// <param name="length">Length of the buffer.</param>
        public DataBuffer(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive");
            }

            _position = 0;
            _length = length;
            _elementSizeInBytes = MemoryHelper.SizeOf<T>();
            _elementType = typeof(T);
            _data = new RawBuffer(_length * _elementSizeInBytes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBuffer{T}"/> class that will initialize the buffer with the data from the
        /// specified element array.
        /// </summary>
        /// <param name="data">Element array</param>
        public DataBuffer(params T[] data)
            : this(data, false)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DataBuffer{T}"/> class.
        /// </summary>
        /// <param name="data">Element array</param>
        /// <param name="pinDataForBackingStore">True if the input data should be pinned and used as the backing store for the databufer, false otherwise (data will be copied
        /// into a newly allocated buffer).</param>
        public DataBuffer(T[] data, bool pinDataForBackingStore)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            _position = 0;
            _length = data.Length;
            _elementSizeInBytes = MemoryHelper.SizeOf<T>();
            _elementType = typeof(T);
            _data = RawBuffer.Create<T>(data, 0, pinDataForBackingStore);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBuffer{T}"/> class.
        /// </summary>
        /// <param name="data">Unmanaged memory buffer.</param>
        /// <param name="sizeInBytes">Size of unmanaged memory buffer, in bytes.</param>
        /// <param name="owned">True if the unmanaged memory buffer is owned by the databuffer, false otherwise.</param>
        public DataBuffer(IntPtr data, int sizeInBytes, bool owned)
        {
            if (data == IntPtr.Zero || sizeInBytes == 0)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            _position = 0;
            _elementSizeInBytes = MemoryHelper.SizeOf<T>();
            _length = sizeInBytes / _elementSizeInBytes;
            _elementType = typeof(T);
            _data = new RawBuffer(data, sizeInBytes, owned);
        }

        /// <summary>
        /// Gets or sets the current position pointer of the buffer. This is used and incremented by relative get/set operations. If the position
        /// is out of range, it is clamped within the range of [0, Length].
        /// </summary>
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = MathHelper.Clamp(value, 0, _length);
            }
        }

        /// <summary>
        /// Gets the number of elements that the buffer can contain.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Gets the remaining length in the buffer (Length - Position).
        /// </summary>
        public int RemainingLength => _length - _position;

        /// <summary>
        /// Gets the size of a single element in the buffer.
        /// </summary>
        public int ElementSizeInBytes => _elementSizeInBytes;

        /// <summary>
        /// Gets the total size of the buffer in bytes.
        /// </summary>
        public int SizeInBytes => _data.SizeInBytes;

        /// <summary>
        /// Gets the type of the element that the buffer contains.
        /// </summary>
        public Type ElementType => _elementType;

        /// <summary>
        /// Gets if the data buffer has been mapped for direct access.
        /// </summary>
        public bool IsMapped => _mapped;

        /// <summary>
        /// Gets if the buffer has been disposed or not.
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// Gets if the position pointer can be incremented.
        /// </summary>
        public bool HasNext => _position < _length;

        /// <summary>
        /// Gets or sets an element at the specified index in the buffer.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public T this[int index]
        {
            get
            {
                CheckDisposed();
                return _data.Read<T>(CheckIndex(index) * _elementSizeInBytes);
            }
            set
            {
                CheckDisposed();
                _data.Write(CheckIndex(index) * _elementSizeInBytes, ref value);
                _version++;
            }
        }

        /// <summary>
        /// Creates a new data buffer with the specified type and capacity.
        /// is returned.
        /// </summary>
        /// <typeparam name="T">Struct type.</typeparam>
        /// <param name="count">Number of elements the buffer should hold.</param>
        /// <returns>Created data buffer.</returns>
        public static IDataBuffer<T> Create(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return new DataBuffer<T>(count);
        }

        /// <summary>
        /// Clears the buffer to default values.
        /// </summary>
        public void Clear()
        {
            CheckDisposed();
            _data.Clear();
            _version++;
        }

        /// <summary>
        /// Resizes the buffer to the new size.
        /// </summary>
        /// <param name="capacity">New capacity of the databuffer, if less than length then the buffer is trimmed. If greater, then it is expanded. Must be greater than zero.</param>
        /// <returns>True if the buffer was resized.</returns>
        public void Resize(int capacity)
        {
            CheckDisposed();

            if (_mapped)
            {
                throw new InvalidOperationException("DataBuffer is already mapped");
            }

            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Value must be greater than zero");
            }

            RawBuffer newBuffer = new RawBuffer(capacity * _elementSizeInBytes);
            MemoryHelper.CopyMemory(newBuffer.BufferPointer, _data.BufferPointer, Math.Min(newBuffer.SizeInBytes, _data.SizeInBytes));

            _length = capacity;
            _data.Dispose();
            _data = newBuffer;
            _position = Math.Min(_position, _length);

            _version++;
        }

        /// <summary>
        /// Maps the data buffer for direct pointer access.
        /// </summary>
        /// <returns>Mapped pointer.</returns>
        public MappedDataBuffer Map()
        {
            CheckDisposed();

            if (_mapped)
            {
                throw new InvalidOperationException("DataBuffer is already mapped");
            }

            _mapped = true;
            return new MappedDataBuffer(_data.BufferPointer, this);
        }

        /// <summary>
        /// Unamps the data buffer pointer obtained when the databuffer was mapped.
        /// </summary>
        public void Unmap()
        {
            CheckDisposed();

            if (!_mapped)
            {
                throw new InvalidOperationException("DataBuffer is not mapped");
            }

            _mapped = false;
        }

        /// <summary>
        /// Checks if the position pointer can be incremented N more times.
        /// </summary>
        /// <param name="numElements">Number of elements in the sequence.</param>
        /// <returns>True if the buffer has enough room to add the next N elements.</returns>
        public bool HasNextFor(int numElements)
        {
            CheckDisposed();

            if (numElements <= 0)
            {
                return false;
            }

            return (_position + numElements) <= _length;
        }

        /// <summary>
        /// Allocates a new (writable) data buffer and copies the data to it.
        /// </summary>
        /// <returns>Cloned data buffer.</returns>
        public IDataBuffer Clone()
        {
            DataBuffer<T> db = new DataBuffer<T>(_length);
            MemoryHelper.CopyMemory(db._data.BufferPointer, _data.BufferPointer, SizeInBytes);
            return db;
        }

        /// <summary>
        /// Get a copy of the object.
        /// </summary>
        /// <returns>Cloned copy.</returns>
        IDeepCloneable IDeepCloneable.Clone()
        {
            return Clone() as IDeepCloneable;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable input</param>
        public void Read(ISavableReader input)
        {
            _position = 0;
            _elementType = typeof(T);
            _elementSizeInBytes = input.ReadInt32();

            // Validate type size
            if (_elementSizeInBytes != MemoryHelper.SizeOf<T>())
            {                
                throw new SparkContentException($"Type size mismatch: Actual: {_elementSizeInBytes}. Expected: {MemoryHelper.SizeOf<T>()}");
            }

            _length = input.PeekArrayCount();
            _data = new RawBuffer(_length * _elementSizeInBytes);

            input.ReadArrayData(this);
            _position = 0;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable output</param>
        public void Write(ISavableWriter output)
        {
            CheckDisposed();

            int oldPos = _position;
            _position = 0;

            output.Write("ElementSizeInBytes", _elementSizeInBytes);
            output.Write<T>("Data", this);

            _position = oldPos;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            CheckDisposed();

            int version = _version;
            for (int i = 0; i < _length; i++)
            {
                if (version != _version)
                {
                    throw new InvalidOperationException("Collection has been modified during enumeration");
                }
                
                yield return Get(i);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            CheckDisposed();

            int version = _version;
            for (int i = 0; i < _length; i++)
            {
                if (version != _version)
                {
                    throw new InvalidOperationException("Collection has been modified during enumeration");
                }

                yield return Get(i);
            }
        }
        
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns> A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}[{1}] - Position: {2}, ElementSizeInBytes: {3}", new object[] { _elementType.Name, _length, _position, _elementSizeInBytes });
        }
        
        /// <summary>
        /// Relative get operation that reads the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <returns>The value read from the buffer.</returns>
        public T Get()
        {
            CheckDisposed();
            return _data.Read<T>(GetNextIndex() * _elementSizeInBytes);
        }

        /// <summary>
        /// Relative get operation that reads the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value read from the buffer.</param>
        public void Get(out T value)
        {
            CheckDisposed();
            value = Get();
        }
        
        /// <summary>
        /// Absolute get operation that reads the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to read the element from.</param>
        /// <returns>The value read from the buffer.</returns>
        public T Get(int index)
        {
            CheckDisposed();
            return _data.Read<T>(CheckIndex(index) * _elementSizeInBytes);
        }

        /// <summary>
        /// Absolute get operation that reads the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to read the element from.</param>
        /// <param name="value">The value read from the buffer.</param>
        public void Get(int index, out T value)
        {
            CheckDisposed();
            value = Get(index);
        }
        
        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        public void GetRange(T[] data)
        {
            CheckDisposed();
            GetRange(data, data.Length);
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        public void GetRange(T[] data, int count)
        {
            CheckDisposed();
            GetRange(data, 0, count);
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data to.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        public void GetRange(T[] data, int startIndexInArray, int count)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInArray, count);

            if (_position + count > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer overflow, number of elements to read from buffer is too many.");
            }

            _data.Read(_position * _elementSizeInBytes, data, startIndexInArray, count);
            _position += count;
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Element array to store the data in.</param>
        public void GetRange(int index, T[] data)
        {
            CheckDisposed();
            GetRange(index, data.Length);
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        public void GetRange(int index, T[] data, int count)
        {
            CheckDisposed();
            GetRange(index, data, 0, count);
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data to.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        public void GetRange(int index, T[] data, int startIndexInArray, int count)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInArray, count);

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            if (index + count > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer overflow, number of elements to read from buffer is too many.");
            }

            _data.Read(index * _elementSizeInBytes, data, startIndexInArray, count);
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="count">Number of elements to read from the buffer.</param>
        /// <returns>The element array containing the requested data.</returns>
        public T[] GetRange(int count)
        {
            CheckDisposed();

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to zero.");
            }

            if (_position + count > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer overflow, number of elements to read from buffer is too many.");
            }

            T[] data = new T[count];

            _data.Read(_position * _elementSizeInBytes, data, 0, count);
            _position += count;

            return data;
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        /// <returns>The element array containing the requested data.</returns>
        public T[] GetRange(int index, int count)
        {
            CheckDisposed();
            CheckArrayIndex(_length, index, count);

            T[] data = new T[count];
            _data.Read(index * _elementSizeInBytes, data, 0, count);

            return data;
        }

        /// <summary>
        /// Relative set operation that writes the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Set(T value)
        {
            CheckDisposed();
            _data.Write(GetNextIndex() * _elementSizeInBytes, ref value);
            _version++;
        }

        /// <summary>
        /// Relative set operation that writes the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Set(ref T value)
        {
            CheckDisposed();
            _data.Write(GetNextIndex() * _elementSizeInBytes, ref value);
            _version++;
        }
        
        /// <summary>
        /// Absolute set operation that writes the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to write the element to.</param>
        /// <param name="value">The value to write.</param>
        public void Set(int index, T value)
        {
            CheckDisposed();
            _data.Write(CheckIndex(index) * _elementSizeInBytes, ref value);
            _version++;
        }

        /// <summary>
        /// Absolute set operation that writes the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to write the element to.</param>
        /// <param name="value">The value to write.</param>
        public void Set(int index, ref T value)
        {
            CheckDisposed();
            _data.Write(CheckIndex(index) * _elementSizeInBytes, ref value);
            _version++;
        }
        
        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        public void SetRange(T[] data)
        {
            CheckDisposed();
            SetRange(data, data.Length);
        }

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        public void SetRange(T[] data, int count)
        {
            CheckDisposed();
            SetRange(data, 0, data.Length);
        }

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data from.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        public void SetRange(T[] data, int startIndexInArray, int count)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInArray, count);

            if (_position + count > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer overflow, number of elements to write to the buffer is too many.");
            }

            _data.Write(_position * _elementSizeInBytes, data, startIndexInArray, count);
            _position += count;

            _version++;
        }

        /// <summary>
        /// Relative bulk set operation that will copy the enumerable collection and write the data to the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Enumerable collection containing data to write to the buffer.</param>
        public void SetRange(IEnumerable<T> data)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            int count = data.Count();
            if (_position + count > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer overflow, number of elements to write to the buffer is too many.");
            }

            using (IEnumerator<T> enumerator = data.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    T value = enumerator.Current;
                    _data.Write(GetNextIndex() * _elementSizeInBytes, ref value);
                }
            }

            _version++;
        }
        
        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        public void SetRange(int index, T[] data)
        {
            CheckDisposed();
            SetRange(index, data, data.Length);
        }

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        public void SetRange(int index, T[] data, int count)
        {
            CheckDisposed();
            SetRange(index, data, 0, data.Length);
        }

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data from.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        public void SetRange(int index, T[] data, int startIndexInArray, int count)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInArray, count);

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            if (index + count > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer overflow, number of elements to write to the buffer is too many.");
            }

            _data.Write(index * _elementSizeInBytes, data, startIndexInArray, count);

            _version++;
        }

        /// <summary>
        /// Absolute bulk set operation that will copy the enumerable collection and write the data to the buffer starting at the specified element index. This does not advance
        /// the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Enumerable collection containing data to write to the buffer.</param>
        public void SetRange(int index, IEnumerable<T> data)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            int count = data.Count();
            if (index + count > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Buffer overflow, number of elements to write to the buffer is too many.");
            }

            using (IEnumerator<T> enumerator = data.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    T value = enumerator.Current;
                    _data.Write(index * _elementSizeInBytes, ref value);
                    index++;
                }
            }

            _version++;
        }
        
        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Byte array to store the data in.</param>
        public void GetBytes(byte[] data)
        {
            CheckDisposed();
            GetBytes(data, data.Length);
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        public void GetBytes(byte[] data, int byteCount)
        {
            CheckDisposed();
            GetBytes(data, 0, data.Length);
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="startIndexInByteArray">Zero-based index in the byte array to start copying data to.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        public void GetBytes(byte[] data, int startIndexInByteArray, int byteCount)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            int elemCount = byteCount / _elementSizeInBytes;

            CheckArrayIndex(data.Length, startIndexInByteArray, byteCount);

            if (_position + elemCount > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Buffer overflow, number of elements to read from buffer is too many.");
            }

            _data.Read(_position * _elementSizeInBytes, data, startIndexInByteArray, byteCount);
            _position += elemCount;
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        /// <returns>The byte array containing the requested data.</returns>
        public byte[] GetBytes(int byteCount)
        {
            CheckDisposed();

            int elemCount = byteCount / _elementSizeInBytes;

            if (byteCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Count must be greater than or equal to zero");
            }

            if (_position + elemCount > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Buffer overflow, number of elements to read from buffer is too many.");
            }

            byte[] data = new byte[byteCount];

            _data.Read(_position * _elementSizeInBytes, data, 0, byteCount);
            _position += elemCount;

            return data;
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        /// <returns>The byte array containing the requested data.</returns>
        public byte[] GetBytes(int index, int byteCount)
        {
            CheckDisposed();

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            if (byteCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Count must be greater than or equal to zero");
            }

            if (index + (byteCount / _elementSizeInBytes) > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Buffer overflow, number of elements to read from buffer is too many.");
            }

            byte[] store = new byte[byteCount];

            _data.Read(index * _elementSizeInBytes, store, 0, byteCount);

            return store;
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Byte array to store the data in.</param>
        public void GetBytes(int index, byte[] data)
        {
            CheckDisposed();
            GetBytes(index, data, data.Length);
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        public void GetBytes(int index, byte[] data, int byteCount)
        {
            CheckDisposed();
            GetBytes(index, data, 0, byteCount);
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="startIndexInByteArray">Zero-based index in the byte array to start copying data to.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        public void GetBytes(int index, byte[] data, int startIndexInByteArray, int byteCount)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInByteArray, byteCount);

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            if (index + (byteCount / _elementSizeInBytes) > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Buffer overflow, number of elements to read from buffer is too many.");
            }

            _data.Read(index * _elementSizeInBytes, data, startIndexInByteArray, byteCount);
        }
        
        /// <summary>
        /// Relative bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        public void SetBytes(byte[] data)
        {
            CheckDisposed();
            SetBytes(data, data.Length);
        }

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer, starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        public void SetBytes(byte[] data, int byteCount)
        {
            CheckDisposed();
            SetBytes(data, 0, byteCount);
        }

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="startIndexInByteArray">Zero-based index in the byte array to start copying data from.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        public void SetBytes(byte[] data, int startIndexInByteArray, int byteCount)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            int elemCount = byteCount / _elementSizeInBytes;

            CheckArrayIndex(data.Length, startIndexInByteArray, byteCount);

            if (_position + elemCount > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Buffer overflow, number of elements to write to the buffer is too many.");
            }

            _data.Write(_position * _elementSizeInBytes, data, startIndexInByteArray, byteCount);
            _position += elemCount;

            _version++;
        }
        
        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the specified 
        /// element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        public void SetBytes(int index, byte[] data)
        {
            CheckDisposed();
            SetBytes(index, data, data.Length);
        }

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the specified 
        /// element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        public void SetBytes(int index, byte[] data, int byteCount)
        {
            CheckDisposed();
            SetBytes(index, data, 0, byteCount);
        }

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the specified 
        /// element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="startIndexInByteArray">Zero-based index in the byte array to start copying data from.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        public void SetBytes(int index, byte[] data, int startIndexInByteArray, int byteCount)
        {
            CheckDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInByteArray, byteCount);

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            if (index + (byteCount / _elementSizeInBytes) > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Buffer overflow, number of elements to write to the buffer is too many.");
            }

            _data.Write(index * _elementSizeInBytes, data, startIndexInByteArray, byteCount);
            _version++;
        }
        
        /// <summary>
        /// Checks an index is within the bounds of the buffer
        /// </summary>
        /// <param name="index">Index to validate</param>
        /// <returns>True if the index is within the bounds of the buffer, false if otherwise</returns>
        private bool CheckBounds(int index)
        {
            if ((index >= _length) || (index < 0))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates an index if within the bounds of the buffer
        /// </summary>
        /// <param name="index">Index to validate</param>
        /// <returns>The input index</returns>
        private int CheckIndex(int index)
        {
            if (CheckBounds(index))
            {
                return index;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }
        }

        /// <summary>
        /// Checks a range of indices is within the bounds of the buffer
        /// </summary>
        /// <param name="length">Length of the buffer</param>
        /// <param name="index">Index in the buffer</param>
        /// <param name="count">Number of elements that are to be read or written</param>
        private void CheckArrayIndex(int length, int index, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to zero");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            if (index + count > length)
            {
                throw new ArgumentOutOfRangeException($"{nameof(index)} + {nameof(count)}", "Index and count are out of range");
            }
        }

        /// <summary>
        /// Gets the next index within the buffer
        /// </summary>
        /// <returns>Next index</returns>
        private int GetNextIndex()
        {
            if (_position >= _length)
            {
                throw new ArgumentOutOfRangeException(nameof(_position), "Cannot advance buffer beyond its length");
            }

            return _position++; // Returns current position, stores current position + 1
        }

        /// <summary>
        /// Checks the buffer has not been disposed, throwing an exception if it is
        /// </summary>
        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Object is disposed");
            }
        }

        /// <summary>
        /// Disposes the data buffer instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false is called from the finalizer</param>
        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _data.Dispose();
                _data = null;
                _position = 0;
                _length = 0;
            }

            _isDisposed = true;
        }
    }
}
