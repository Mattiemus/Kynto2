namespace Spark.Graphics
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    using Content;

    /// <summary>
    /// A lightweight wrapper for index data, which may be in a 32-bit or 16-bit format. This wrapper allows clients to iterate over index data regardless of
    /// format, if the wrapper holds short data there may be some overhead as the data is cast to 32-bit integers and vice versa. Enumeration and
    /// Get/SetRange do not create garbage.
    /// </summary>
    public struct IndexData : IDataBuffer<int>, IDeepCloneable
    {
        private IDataBuffer<int> _intBuffer;
        private IDataBuffer<short> _shortBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexData"/> struct.
        /// </summary>
        /// <param name="intIndexData">Index data in 32-bit (Int) format.</param>
        public IndexData(IDataBuffer<int> intIndexData)
        {
            _intBuffer = intIndexData;
            _shortBuffer = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexData"/> struct.
        /// </summary>
        /// <param name="shortIndexData">Index data in 16-bit (Short) format.</param>
        public IndexData(IDataBuffer<short> shortIndexData)
        {
            _intBuffer = null;
            _shortBuffer = shortIndexData;
        }

        /// <summary>
        /// Gets or sets the current position pointer of the buffer. This is used and incremented by relative get/set operations.
        /// </summary>
        public int Position
        {
            get => UnderlyingDataBuffer.Position;
            set => UnderlyingDataBuffer.Position = value;
        }

        /// <summary>
        /// Gets the number of elements that the buffer can contain.
        /// </summary>
        public int Length => UnderlyingDataBuffer.Length;

        /// <summary>
        /// Gets the length of the remaining.
        /// </summary>
        public int RemainingLength => UnderlyingDataBuffer.RemainingLength;

        /// <summary>
        /// Gets the total size of the buffer in bytes.
        /// </summary>
        public int SizeInBytes => UnderlyingDataBuffer.SizeInBytes;

        /// <summary>
        /// Gets the size of a single element in the buffer.
        /// </summary>
        public int ElementSizeInBytes => UnderlyingDataBuffer.ElementSizeInBytes;

        /// <summary>
        /// Gets the type of the element that the buffer contains.
        /// </summary>
        public Type ElementType => UnderlyingDataBuffer.ElementType;

        /// <summary>
        /// Gets if the data buffer has been mapped for direct access.
        /// </summary>
        public bool IsMapped => UnderlyingDataBuffer.IsMapped;

        /// <summary>
        /// Gets if the buffer has been disposed or not.
        /// </summary>
        public bool IsDisposed => UnderlyingDataBuffer.IsDisposed;

        /// <summary>
        /// Gets if the position pointer can be incremented.
        /// </summary>
        public bool HasNext => UnderlyingDataBuffer.HasNext;

        /// <summary>
        /// Gets or sets an element at the specified index in the buffer.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public int this[int index]
        {
            get
            {
                if (_intBuffer != null)
                {
                    return _intBuffer[index];
                }

                return _shortBuffer[index];
            }
            set
            {
                if (_intBuffer != null)
                {
                    _intBuffer[index] = value;
                }
                else
                {
                    _shortBuffer[index] = (short)value;
                }
            }
        }

        /// <summary>
        /// Gets the index format.
        /// </summary>
        public IndexFormat IndexFormat => (_intBuffer != null) ? IndexFormat.ThirtyTwoBits : IndexFormat.SixteenBits;

        /// <summary>
        /// Gets if the index data is actually valid.
        /// </summary>
        public bool IsValid => UnderlyingDataBuffer != null;

        /// <summary>
        /// Gets the underlying data buffer.
        /// </summary>
        public IDataBuffer UnderlyingDataBuffer
        {
            get
            {
                if (_intBuffer != null)
                {
                    return _intBuffer;
                }

                return _shortBuffer;
            }
        }

        /// <summary>
        /// Implicitly converts a data buffer containing integers to IndexData.
        /// </summary>
        /// <param name="data">Data buffer</param>
        /// <returns>Index data</returns>
        public static implicit operator IndexData(DataBuffer<int> data)
        {
            return new IndexData(data);
        }
        
        /// <summary>
        /// Implicitly converts a data buffer containing shorts to IndexData.
        /// </summary>
        /// <param name="data">Data buffer</param>
        /// <returns>Index data</returns>
        public static implicit operator IndexData(DataBuffer<short> data)
        {
            return new IndexData(data);
        }
        
        /// <summary>
        /// Gets the underlying databuffer as an int data buffer. This will be null if the index format is 16-bit.
        /// </summary>
        public IDataBuffer<int> AsIntDataBuffer()
        {
            return _intBuffer;
        }

        /// <summary>
        /// Gets the underlying databuffer as a short data buffer. This will be null if the index format is 32-bit.
        /// </summary>
        public IDataBuffer<short> AsShortDataBuffer()
        {
            return _shortBuffer;
        }

        /// <summary>
        /// Clears the buffer to default values.
        /// </summary>
        public void Clear()
        {
            UnderlyingDataBuffer.Clear();
        }

        /// <summary>
        /// Resizes the buffer to the new size.
        /// </summary>
        /// <param name="capacity">New capacity of the databuffer, if less than length then the buffer is trimmed. If greater, then it is expanded. Must be greater than zero.</param>
        /// <returns>True if the buffer was resized.</returns>
        public void Resize(int capacity)
        {
            UnderlyingDataBuffer.Resize(capacity);
        }

        /// <summary>
        /// Maps the data buffer for direct pointer access.
        /// </summary>
        /// <returns>Mapped pointer.</returns>
        public MappedDataBuffer Map()
        {
            return UnderlyingDataBuffer.Map();
        }

        /// <summary>
        /// Unamps the data buffer pointer obtained when the databuffer was mapped.
        /// </summary>
        public void Unmap()
        {
            UnderlyingDataBuffer.Unmap();
        }

        /// <summary>
        /// Checks if the position pointer can be incremented N more times.
        /// </summary>
        /// <param name="numElements">Number of elements in the sequence.</param>
        /// <returns>True if the buffer has enough room to add the next N elements.</returns>
        public bool HasNextFor(int numElements)
        {
            return UnderlyingDataBuffer.HasNextFor(numElements);
        }

        /// <summary>
        /// Allocates a new (writable) data buffer and copies the data to it.
        /// </summary>
        /// <returns>Cloned data buffer.</returns>
        public IndexData Clone()
        {
            IDataBuffer db = UnderlyingDataBuffer;
            if (db == null)
            {
                return new IndexData();
            }

            IDataBuffer newDb = db.Clone();
            if (IndexFormat == IndexFormat.ThirtyTwoBits)
            {
                return new IndexData(newDb as IDataBuffer<int>);
            }
            else
            {
                return new IndexData(newDb as IDataBuffer<short>);
            }
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
        /// Allocates a new (writable) data buffer and copies the data to it.
        /// </summary>
        /// <returns>Cloned data buffer.</returns>
        IDataBuffer IReadOnlyDataBuffer.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<int> GetEnumerator()
        {
            if (_intBuffer != null)
            {
                return _intBuffer.GetEnumerator();
            }

            return _shortBuffer.Cast<int>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_intBuffer != null)
            {
                return _intBuffer.GetEnumerator();
            }

            return _shortBuffer.Cast<int>().GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns> A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return UnderlyingDataBuffer.ToString();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            UnderlyingDataBuffer.Dispose();
        }

        #region Relative Get

        /// <summary>
        /// Relative get operation that reads the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <returns>The value read from the buffer.</returns>
        public int Get()
        {
            if (_intBuffer != null)
            {
                return _intBuffer.Get();
            }

            return _shortBuffer.Get();
        }

        /// <summary>
        /// Relative get operation that reads the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value read from the buffer.</param>
        public void Get(out int value)
        {
            if (_intBuffer != null)
            {
                _intBuffer.Get(out value);
                return;
            }
            
            _shortBuffer.Get(out short sValue);
            value = sValue;
        }

        #endregion

        #region Absolute Get

        /// <summary>
        /// Absolute get operation that reads the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to read the element from.</param>
        /// <returns>The value read from the buffer.</returns>
        public int Get(int index)
        {
            if (_intBuffer != null)
            {
                return _intBuffer.Get(index);
            }

            return _shortBuffer.Get(index);
        }

        /// <summary>
        /// Absolute get operation that reads the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to read the element from.</param>
        /// <param name="value">The value read from the buffer.</param>
        public void Get(int index, out int value)
        {
            if (_intBuffer != null)
            {
                _intBuffer.Get(index, out value);
                return;
            }
            
            _shortBuffer.Get(index, out short sValue);
            value = sValue;
        }

        #endregion

        #region Relative GetRange

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="count">Number of elements to read from the buffer.</param>
        /// <returns>The element array containing the requested data.</returns>
        public int[] GetRange(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to zero");
            }

            int[] data = new int[count];
            GetRange(data, 0, data.Length);

            return data;
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        public void GetRange(int[] data)
        {
            int count = (data != null) ? data.Length : 0;
            GetRange(data, 0, data.Length);
        }


        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        public void GetRange(int[] data, int count)
        {
            GetRange(data, 0, count);
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data to.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        public void GetRange(int[] data, int startIndexInArray, int count)
        {
            if (_intBuffer != null)
            {
                _intBuffer.GetRange(data, startIndexInArray, count);
                return;
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Index array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInArray, count);

            if (_shortBuffer.Position + count > _shortBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Read buffer overflow");
            }

            while (count >= 0)
            {
                data[startIndexInArray] = _shortBuffer.Get();
                startIndexInArray++;
                count--;
            }
        }

        #endregion

        #region Absolute GetRange

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        /// <returns>The element array containing the requested data.</returns>
        public int[] GetRange(int index, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to zero");
            }

            int[] data = new int[count];
            GetRange(index, data, count);

            return data;
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Element array to store the data in.</param>
        public void GetRange(int index, int[] data)
        {
            int count = (data != null) ? data.Length : 0;
            GetRange(index, data, 0, count);
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        public void GetRange(int index, int[] data, int count)
        {
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
        public void GetRange(int index, int[] data, int startIndexInArray, int count)
        {
            if (_intBuffer != null)
            {
                _intBuffer.GetRange(index, data, startIndexInArray, count);
                return;
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInArray, count);

            if (_shortBuffer.Position + count > _shortBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Read buffer overflow");
            }

            while (count >= 0)
            {
                data[startIndexInArray] = _shortBuffer.Get(index);
                index++;
                startIndexInArray++;
                count--;
            }
        }

        #endregion

        #region Relative Set

        /// <summary>
        /// Relative set operation that writes the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Set(int value)
        {
            if (_intBuffer != null)
            {
                _intBuffer.Set(ref value);
                return;
            }

            _shortBuffer.Set((short)value);
        }

        /// <summary>
        /// Relative set operation that writes the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Set(ref int value)
        {
            if (_intBuffer != null)
            {
                _intBuffer.Set(ref value);
                return;
            }

            _shortBuffer.Set((short)value);
        }

        #endregion

        #region Absolute Set

        /// <summary>
        /// Absolute set operation that writes the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to write the element to.</param>
        /// <param name="value">The value to write.</param>
        public void Set(int index, ref int value)
        {
            if (_intBuffer != null)
            {
                _intBuffer.Set(index, ref value);
                return;
            }

            _shortBuffer.Set(index, (short)value);
        }

        /// <summary>
        /// Absolute set operation that writes the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to write the element to.</param>
        /// <param name="value">The value to write.</param>
        public void Set(int index, int value)
        {
            if (_intBuffer != null)
            {
                _intBuffer.Set(index, value);
                return;
            }

            _shortBuffer.Set(index, (short)value);
        }

        #endregion

        #region Relative SetRange

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        public void SetRange(int[] data)
        {
            int count = (data != null) ? data.Length - 1 : 0;
            SetRange(data, 0, count);
        }

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        public void SetRange(int[] data, int count)
        {
            SetRange(data, 0, count);
        }

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data from.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        public void SetRange(int[] data, int startIndexInArray, int count)
        {
            if (_intBuffer != null)
            {
                _intBuffer.SetRange(data, startIndexInArray, count);
                return;
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInArray, count);

            if (_shortBuffer.Position + count > _shortBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Write buffer overflow");
            }

            while (count >= 0)
            {
                _shortBuffer.Set((short)data[startIndexInArray]);
                startIndexInArray++;
                count--;
            }
        }

        /// <summary>
        /// Relative bulk set operation that will copy the enumerable collection and write the data to the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Enumerable collection containing data to write to the buffer.</param>
        public void SetRange(IEnumerable<int> data)
        {
            if (_intBuffer != null)
            {
                _intBuffer.SetRange(data);
                return;
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            int count = data.Count();
            if (_shortBuffer.Position + count > _shortBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Write buffer overflow");
            }

            foreach (int value in data)
            {
                _shortBuffer.Set((short)value);
            }
        }

        #endregion

        #region Absolute SetRange

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        public void SetRange(int index, int[] data)
        {
            int count = (data != null) ? data.Length - 1 : 0;
            SetRange(index, data, 0, count);
        }

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        public void SetRange(int index, int[] data, int count)
        {
            SetRange(index, data, 0, count);
        }

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data from.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        public void SetRange(int index, int[] data, int startIndexInArray, int count)
        {
            if (_intBuffer != null)
            {
                _intBuffer.SetRange(index, data, startIndexInArray, count);
                return;
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            CheckArrayIndex(data.Length, startIndexInArray, count);

            if (index + count > _shortBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Write buffer overflow");
            }

            while (count >= 0)
            {
                _shortBuffer.Set(index, (short)data[startIndexInArray]);
                index++;
                startIndexInArray++;
                count--;
            }
        }

        /// <summary>
        /// Absolute bulk set operation that will copy the enumerable collection and write the data to the buffer starting at the specified element index. This does not advance
        /// the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Enumerable collection containing data to write to the buffer.</param>
        public void SetRange(int index, IEnumerable<int> data)
        {
            if (_intBuffer != null)
            {
                _intBuffer.SetRange(index, data);
                return;
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input array is null or empty");
            }

            int count = data.Count();
            if (index + count > _shortBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Write buffer overflow");
            }

            foreach (int value in data)
            {
                _shortBuffer.Set(index, (short)value);
                index++;
            }
        }

        #endregion

        #region Relative GetBytes

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        /// <returns>The byte array containing the requested data.</returns>
        public byte[] GetBytes(int byteCount)
        {
            return UnderlyingDataBuffer.GetBytes(byteCount);
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Byte array to store the data in.</param>
        public void GetBytes(byte[] data)
        {
            UnderlyingDataBuffer.GetBytes(data);
        }

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        public void GetBytes(byte[] data, int byteCount)
        {
            UnderlyingDataBuffer.GetBytes(data, byteCount);
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
            UnderlyingDataBuffer.GetBytes(data, startIndexInByteArray, byteCount);
        }

        #endregion

        #region Absolute GetBytes

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        /// <returns>The byte array containing the requested data.</returns>
        public byte[] GetBytes(int index, int byteCount)
        {
            return UnderlyingDataBuffer.GetBytes(index, byteCount);
        }

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Byte array to store the data in.</param>
        public void GetBytes(int index, byte[] data)
        {
            UnderlyingDataBuffer.GetBytes(index, data);
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
            UnderlyingDataBuffer.GetBytes(index, data, byteCount);
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
            UnderlyingDataBuffer.GetBytes(index, data, startIndexInByteArray, byteCount);
        }

        #endregion

        #region Relative SetBytes

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        public void SetBytes(byte[] data)
        {
            UnderlyingDataBuffer.SetBytes(data);
        }

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        public void SetBytes(byte[] data, int byteCount)
        {
            UnderlyingDataBuffer.SetBytes(data, byteCount);
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
            UnderlyingDataBuffer.SetBytes(data, startIndexInByteArray, byteCount);
        }

        #endregion

        #region Absolute SetBytes

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the specified
        /// element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        public void SetBytes(int index, byte[] data)
        {
            UnderlyingDataBuffer.SetBytes(index, data);
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
            UnderlyingDataBuffer.SetBytes(index, data, byteCount);
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
            UnderlyingDataBuffer.SetBytes(index, data, startIndexInByteArray, byteCount);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Make sure a given index and count is valid for the given buffer length
        /// </summary>
        /// <param name="length">Length of the buffer</param>
        /// <param name="index">Index within the buffer</param>
        /// <param name="count">Number of elements</param>
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
                throw new ArgumentOutOfRangeException(nameof(count), "Index and count out of range");
            }
        }

        #endregion
    }
}
