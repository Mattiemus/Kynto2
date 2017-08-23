namespace Spark.Content.Binary
{
    using System;
    using System.IO;
    using System.Text;

    using Utilities;
    using Core;

    /// <summary>
    /// A reader that can read built-in .NET types / primitive values in both singular and array forms from binary.
    /// </summary>
    public class BinaryPrimitiveReader : BaseDisposable, IPrimitiveReader
    {        
        private byte[] _chunkBuffer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryPrimitiveReader"/> class. Default character encoding
        /// is UTF8 and the underlying stream will be disposed/closed when the reader is.
        /// </summary>
        /// <param name="input">Input stream to read from</param>
        public BinaryPrimitiveReader(Stream input) 
            : this(input, new UTF8Encoding(), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryPrimitiveReader"/> class. The underlying stream will be
        /// disposed/closed when the reader is.
        /// </summary>
        /// <param name="input">Input stream to read from</param>
        /// <param name="encoding">Character encoding.</param>
        /// 
        public BinaryPrimitiveReader(Stream input, Encoding encoding) 
            : this(input, encoding, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryPrimitiveReader"/> class. Default character encoding is
        /// UTF8.
        /// </summary>
        /// <param name="input">Input stream to read from</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the reader is disposed/closed, false otherwise.</param>
        public BinaryPrimitiveReader(Stream input, bool leaveOpen) 
            : this(input, new UTF8Encoding(), leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryPrimitiveReader"/> class.
        /// </summary>
        /// <param name="input">Input stream to read from</param>
        /// <param name="encoding">Character encoding.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the reader is disposed/closed, false otherwise.</param>
        public BinaryPrimitiveReader(Stream input, Encoding encoding, bool leaveOpen)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (!input.CanRead)
            {
                throw new ArgumentException("Cannot read from stream");
            }

            UnderlyingBinaryReader = new BinaryReader(input, encoding, leaveOpen);
        }

        /// <summary>
        /// Gets the underlying input stream.
        /// </summary>
        protected Stream InStream => UnderlyingBinaryReader.BaseStream;

        /// <summary>
        /// Gets or sets the underlying binary reader.
        /// </summary>
        protected BinaryReader UnderlyingBinaryReader { get; set; }
        
        /// <summary>
        /// Closes the underlying stream.
        /// </summary>
        public void Close()
        {
            ThrowIfDisposed();
            Dispose(true);
        }

        /// <summary>
        /// Reads a single byte from the input.
        /// </summary>
        /// <returns>Byte value</returns>
        public byte ReadByte()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadByte();
        }

        /// <summary>
        /// Reads an array of bytes from the input.
        /// </summary>
        /// <returns>Array of bytes</returns>
        public byte[] ReadByteArray()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadByte);
        }

        /// <summary>
        /// Reads a single sbyte from the input.
        /// </summary>
        /// <returns>SByte value</returns>
        public sbyte ReadSByte()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadSByte();
        }

        /// <summary>
        /// Reads an array of sbytes from the input.
        /// </summary>
        /// <returns>Array of sbytes</returns>
        public sbyte[] ReadSByteArray()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadSByte);
        }

        /// <summary>
        /// Reads a single char from the input.
        /// </summary>
        /// <returns>Char value</returns>
        public char ReadChar()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadChar();
        }

        /// <summary>
        /// Reads an array of chars from the input.
        /// </summary>
        /// <returns>Array of chars</returns>
        public char[] ReadCharArray()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadChar);
        }

        /// <summary>
        /// Reads a single unsigned 16-bit int from the input.
        /// </summary>
        /// <returns>UInt16 value</returns>
        public ushort ReadUInt16()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadUInt16();
        }

        /// <summary>
        /// Reads an array of unsigned 16-bit ints from the input.
        /// </summary>
        /// <returns>Array of UInt16s</returns>
        public ushort[] ReadUInt16Array()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadUInt16);
        }

        /// <summary>
        /// Reads a single unsigned 32-bit int from the input.
        /// </summary>
        /// <returns>UInt32 value</returns>
        public uint ReadUInt32()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadUInt32();
        }

        /// <summary>
        /// Reads an array of unsigned 32-bits int from the input.
        /// </summary>
        /// <returns>Array of UInt32s</returns>
        public uint[] ReadUInt32Array()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadUInt32);
        }

        /// <summary>
        /// Reads a single unsigned 64-bit int from the input.
        /// </summary>
        /// <returns>UInt64 value</returns>
        public ulong ReadUInt64()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadUInt64();
        }

        /// <summary>
        /// Reads an array of unsigned 64-bits int from the input.
        /// </summary>
        /// <returns>Array of UInt64s</returns>
        public ulong[] ReadUInt64Array()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadUInt64);
        }

        /// <summary>
        /// Reads a single 16-bit int from the input.
        /// </summary>
        /// <returns>Int16 value</returns>
        public short ReadInt16()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadInt16();
        }

        /// <summary>
        /// Reads an array of 16-bits int from the input.
        /// </summary>
        /// <returns>Array of Int16s</returns>
        public short[] ReadInt16Array()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadInt16);
        }

        /// <summary>
        /// Reads a single 32-bit int from the input.
        /// </summary>
        /// <returns>Int32 value</returns>
        public int ReadInt32()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadInt32();
        }

        /// <summary>
        /// Reads an array of 32-bits int from the input.
        /// </summary>
        /// <returns>Array of Int32s</returns>
        public int[] ReadInt32Array()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadInt32);
        }

        /// <summary>
        /// Reads a single 64-bit int from the input.
        /// </summary>
        /// <returns>Int64 value</returns>
        public long ReadInt64()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadInt64();
        }

        /// <summary>
        /// Reads an array of 64-bits int from the input.
        /// </summary>
        /// <returns>Array of Int64s</returns>
        public long[] ReadInt64Array()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadInt64);
        }

        /// <summary>
        /// Reads a single float from the input.
        /// </summary>
        /// <returns>Float value</returns>
        public float ReadSingle()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadSingle();
        }

        /// <summary>
        /// Reads an array of floats from the input.
        /// </summary>
        /// <returns>Array of floats</returns>
        public float[] ReadSingleArray()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadSingle);
        }

        /// <summary>
        /// Reads a single double from the input.
        /// </summary>
        /// <returns>Double value</returns>
        public double ReadDouble()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadDouble();
        }

        /// <summary>
        /// Reads an array of doubles from the input.
        /// </summary>
        /// <returns>Array of doubles</returns>
        public double[] ReadDoubleArray()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadDouble);
        }

        /// <summary>
        /// Reads a single decimal from the input.
        /// </summary>
        /// <returns>Decimal value</returns>
        public decimal ReadDecimal()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadDecimal();
        }

        /// <summary>
        /// Reads an array of decimals from the input.
        /// </summary>
        /// <returns>Array of decimals</returns>
        public decimal[] ReadDecimalArray()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadDecimal);
        }

        /// <summary>
        /// Reads a single boolean from the input.
        /// </summary>
        /// <returns>Boolean value</returns>
        public bool ReadBoolean()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadBoolean();
        }

        /// <summary>
        /// Reads an array of booleans from the input.
        /// </summary>
        /// <returns>Array of booleans</returns>
        public bool[] ReadBooleanArray()
        {
            ThrowIfDisposed();
            return ReadArray(UnderlyingBinaryReader.ReadBoolean);
        }

        /// <summary>
        /// Reads a string from the input.
        /// </summary>
        /// <returns>String value</returns>
        public string ReadString()
        {
            ThrowIfDisposed();
            return UnderlyingBinaryReader.ReadString();
        }

        /// <summary>
        /// Reads an array of strings from the input.
        /// </summary>
        /// <returns>Array of strings</returns>
        public string[] ReadStringArray()
        {
            ThrowIfDisposed();
            return ReadArray(ReadString);
        }
        
        /// <summary>
        /// Returns the number of elements if the next object is an array, this does not advance the reader.
        /// </summary>
        /// <returns>Non-zero if the next object is an array with elements, zero if it is null.</returns>
        public int PeekArrayCount()
        {
            ThrowIfDisposed();

            if (!UnderlyingBinaryReader.BaseStream.CanSeek)
            {
                throw new InvalidOperationException("Cannot seek in stream");
            }

            long oldPos = UnderlyingBinaryReader.BaseStream.Position;
            int count = 0;

            if (!IsNullObject())
            {
                count = UnderlyingBinaryReader.ReadInt32();
            }

            UnderlyingBinaryReader.BaseStream.Seek(oldPos - UnderlyingBinaryReader.BaseStream.Position, SeekOrigin.Current);

            return count;
        }

        /// <summary>
        /// Reads an array of elements into the data buffer at the current position of the data buffer. The buffer
        /// must have enough position to read the entire array object.
        /// </summary>
        /// <typeparam name="T">Struct type.</typeparam>
        /// <param name="values">Data buffer to hold the array.</param>
        public bool ReadArrayData<T>(IDataBuffer<T> values) where T : struct
        {
            ThrowIfDisposed();

            if (IsNullObject())
            {
                return false;
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            int num = UnderlyingBinaryReader.ReadInt32();

            if (num > values.RemainingLength)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Read buffer overflow");
            }

            byte[] buffer = GetTempBuffer();
            int elemSizeInBytes = values.ElementSizeInBytes;
            int remainingBytes = num * elemSizeInBytes;

            // Calculate, based on the temp buffer's length, how many "whole" elements we can copy at a given time to avoid alignment issues
            int bufferElementMax = buffer.Length / elemSizeInBytes;
            int alignedBufferSizeInBytes = bufferElementMax * elemSizeInBytes;

            while (remainingBytes > 0)
            {
                int numBytes = Math.Min(alignedBufferSizeInBytes, remainingBytes);
                numBytes = UnderlyingBinaryReader.Read(buffer, 0, numBytes);

                // Something went wrong, no bytes were read
                if (numBytes == 0)
                {
                    break;
                }

                values.SetBytes(buffer, numBytes);

                remainingBytes -= numBytes;
            }

            return true;
        }

        /// <summary>
        /// Reads a single <see cref="IPrimitiveValue"/> struct from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns><see cref="IPrimitiveValue"/> struct value</returns>
        public T Read<T>() where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();

            T value = default(T);
            value.Read(this);

            return value;
        }

        /// <summary>
        /// Reads a single <see cref="IPrimitiveValue"/> struct from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="value"><see cref="IPrimitiveValue"/> struct value</param>
        public void Read<T>(out T value) where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();

            value = default(T);
            value.Read(this);
        }

        /// <summary>
        /// Reads an array of <see cref="IPrimitiveValue"/> structs from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Array of <see cref="IPrimitiveValue"/> structs</returns>
        public T[] ReadArray<T>() where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();

            return ReadArray(() => 
            {
                T value = default(T);
                value.Read(this);
                return value;
            });
        }

        /// <summary>
        /// Reads a single nullable <see cref="IPrimitiveValue"/> struct from the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Nullable <see cref="IPrimitiveValue"/> struct value.</returns>
        public T? ReadNullable<T>() where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();

            bool hasValue = UnderlyingBinaryReader.ReadBoolean();

            if (!hasValue)
            {
                return null;
            }

            T value = default(T);
            value.Read(this);

            return value;
        }

        /// <summary>
        /// Reads a single nullable <see cref="IPrimitiveValue"/> struct from the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="value">Nullable <see cref="IPrimitiveValue"/> struct value.</param>
        public void ReadNullable<T>(out T? value) where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();
            value = ReadNullable<T>();
        }

        /// <summary>
        /// Reads an enum value from the input.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Enum value</returns>
        public T ReadEnum<T>() where T : struct, IComparable, IFormattable, IConvertible
        {
            ThrowIfDisposed();

            TypeCode typeCode = Type.GetTypeCode(typeof(T));

            switch (typeCode)
            {
                case TypeCode.Byte:
                    return (T)((object)UnderlyingBinaryReader.ReadByte());
                case TypeCode.SByte:
                    return (T)((object)UnderlyingBinaryReader.ReadSByte());
                case TypeCode.Int16:
                    return (T)((object)UnderlyingBinaryReader.ReadInt16());
                case TypeCode.UInt16:
                    return (T)((object)UnderlyingBinaryReader.ReadUInt16());
                case TypeCode.Int32:
                    return (T)((object)UnderlyingBinaryReader.ReadInt32());
                case TypeCode.UInt32:
                    return (T)((object)UnderlyingBinaryReader.ReadUInt32());
                case TypeCode.Int64:
                    return (T)((object)UnderlyingBinaryReader.ReadInt64());
                case TypeCode.UInt64:
                    return (T)((object)UnderlyingBinaryReader.ReadUInt64());
                default:
                    throw new InvalidOperationException("Enum typecode not valid");
            }
        }
        
        /// <summary>
        /// Read an array of values from the underlying buffer
        /// </summary>
        /// <typeparam name="T">Type of values to read</typeparam>
        /// <param name="readValue">Function to read a value from the buffer</param>
        /// <returns>Array of values that were read</returns>
        protected T[] ReadArray<T>(Func<T> readValue)
        {
            if (IsNullObject())
            {
                return null;
            }

            int num = UnderlyingBinaryReader.ReadInt32();
            T[] values = new T[num];
            for (int i = 0; i < num; i++)
            {
                values[i] = readValue();
            }

            return values;
        }

        /// <summary>
        /// Performs the dispose action
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void DisposeInternal(bool isDisposing)
        {
            if (isDisposing)
            {
                UnderlyingBinaryReader.Dispose();
            }
        }

        /// <summary>
        /// Checks if the next signed byte is a NULL_OBJECT or not.
        /// </summary>
        /// <returns></returns>
        protected bool IsNullObject()
        {
            return UnderlyingBinaryReader.ReadSByte() == BinaryConstants.NULL_OBJECT;
        }

        /// <summary>
        /// Gets and initializes (if necessary) a temporary buffer for reading from the stream.
        /// </summary>
        /// <returns>Temp byte buffer.</returns>
        protected byte[] GetTempBuffer()
        {
            if (_chunkBuffer == null)
            {
                _chunkBuffer = new byte[4096];
            }

            return _chunkBuffer;
        }
    }
}
