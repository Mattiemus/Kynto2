namespace Spark.Content.Binary
{
    using System;
    using System.IO;
    using System.Text;

    using Utilities;
    using Core;

    /// <summary>
    /// A writer that can write built-in .NET types / primitive values in both singular and array forms to binary.
    /// </summary>
    public class BinaryPrimitiveWriter : BaseDisposable, IPrimitiveWriter
    {
        private readonly SwappableBinaryWriter _binaryWriter;
        private byte[] _chunkBuffer;

        /// <summary>
        /// Constructs a new instance of the <see cref="BinaryPrimitiveWriter"/> class. Default character encoding is
        /// UTF8 and the underlying stream will be disposed/closed when the writer is.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        public BinaryPrimitiveWriter(Stream output)
            : this(output, new UTF8Encoding(false, true), false)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BinaryPrimitiveWriter"/> class. The underlying stream will be disposed/closed when
        /// the writer is disposed/closed.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="encoding">Character encoding.</param>
        public BinaryPrimitiveWriter(Stream output, Encoding encoding)
            : this(output, encoding, false)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BinaryPrimitiveWriter"/> class. Default character encoding is
        /// UTF8.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public BinaryPrimitiveWriter(Stream output, bool leaveOpen)
            : this(output, new UTF8Encoding(false, true), leaveOpen)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BinaryPrimitiveWriter"/> class.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="encoding">Character encoding.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public BinaryPrimitiveWriter(Stream output, Encoding encoding, bool leaveOpen)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (!output.CanWrite)
            {
                throw new ArgumentException("Cannot write to stream");
            }

            _binaryWriter = new SwappableBinaryWriter(output, encoding, leaveOpen);
        }

        /// <summary>
        /// Gets or sets the underlying outpt stream.
        /// </summary>
        protected Stream OutStream
        {
            get
            {
                return _binaryWriter.BaseStream;
            }
            set
            {
                _binaryWriter.SetOutStream(value);
            }
        }

        /// <summary>
        /// Gets the underlying binary writer.
        /// </summary>
        protected BinaryWriter UnderlyingBinaryWriter => _binaryWriter;

        /// <summary>
        /// Closes the underlying stream.
        /// </summary>
        public void Close()
        {
            ThrowIfDisposed();
            Dispose(true);
        }

        /// <summary>
        /// Clears all buffers and causes any buffered data to be written.
        /// </summary>
        public void Flush()
        {
            ThrowIfDisposed();
            _binaryWriter.BaseStream.Flush();
        }

        /// <summary>
        /// Writes a single byte to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Byte value</param>
        public void Write(string name, byte value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of bytes to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of bytes</param>
        public void Write(string name, byte[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single sbyte to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">SByte value</param>
        public void Write(string name, sbyte value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of sbytes to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of sbytes</param>
        public void Write(string name, sbyte[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single char to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Char value</param>
        public void Write(string name, char value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of chars to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of chars</param>
        public void Write(string name, char[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single unsigned 16-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt16 value</param>
        public void Write(string name, ushort value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of unsigned 16-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt16s</param>
        public void Write(string name, ushort[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single unsigned 32-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt32 value</param>
        public void Write(string name, uint value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of unsigned 32-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt32s</param>
        public void Write(string name, uint[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single unsigned 64-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt64 value</param>
        public void Write(string name, ulong value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of unsigned 64-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt64s</param>
        public void Write(string name, ulong[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single 16-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int16 value</param>
        public void Write(string name, short value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of 16-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int16s</param>
        public void Write(string name, short[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single 32-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int32 value</param>
        public void Write(string name, int value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of 32-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int32s</param>
        public void Write(string name, int[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single 64-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int64 value</param>
        public void Write(string name, long value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of 64-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int64s</param>
        public void Write(string name, long[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single float value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Float value</param>
        public void Write(string name, float value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of floats to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of floats</param>
        public void Write(string name, float[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single double value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Double value</param>
        public void Write(string name, double value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of doubles to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of doubles</param>
        public void Write(string name, double[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single decimal value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Decimal value</param>
        public void Write(string name, decimal value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of decimals to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of decimals</param>
        public void Write(string name, decimal[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single boolean to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Boolean value</param>
        public void Write(string name, bool value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of booleans to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of booleans</param>
        public void Write(string name, bool[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, _binaryWriter.Write);
        }

        /// <summary>
        /// Writes a single string to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">string value</param>
        public void Write(string name, string value)
        {
            ThrowIfDisposed();
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an array of strings to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of strings</param>
        public void Write(string name, string[] values)
        {
            ThrowIfDisposed();
            WriteArray(values, value => Write(null, value));
        }

        /// <summary>
        /// Writes an array of values to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Databuffer containing the values.</param>
        /// <param name="count">Number of values to write out, starting from the data buffer's current position.</param>
        public void Write<T>(string name, IDataBuffer<T> values, int count) where T : struct
        {
            ThrowIfDisposed();

            if (values == null || count == 0)
            {
                _binaryWriter.Write(BinaryConstants.NULL_OBJECT);
                return;
            }
            else
            {
                _binaryWriter.Write(BinaryConstants.A_OK);
            }

            if (count > values.RemainingLength)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Write buffer overflow");
            }

            _binaryWriter.Write(count);

            byte[] buffer = GetTempBuffer();
            int elemSizeInBytes = values.ElementSizeInBytes;
            int remainingBytes = count * elemSizeInBytes;

            // Calculate, based on the temp buffer's length, how many "whole" elements we can copy at a given time to avoid alignment issues
            int bufferElementMax = buffer.Length / elemSizeInBytes;
            int alignedBufferSizeInBytes = bufferElementMax * elemSizeInBytes;

            while (remainingBytes > 0)
            {
                int numBytes = Math.Min(alignedBufferSizeInBytes, remainingBytes);
                values.GetBytes(buffer, numBytes);
                _binaryWriter.Write(buffer, 0, numBytes);

                remainingBytes -= numBytes;
            }
        }

        /// <summary>
        /// Writes single <see cref="IPrimitiveValue"/> struct to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value"><see cref="IPrimitiveValue"/> struct value</param>
        public void Write<T>(string name, T value) where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();
            value.Write(this);
        }

        /// <summary>
        /// Writes single <see cref="IPrimitiveValue"/> struct to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value"><see cref="IPrimitiveValue"/> struct value</param>
        public void Write<T>(string name, ref T value) where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();
            value.Write(this);
        }

        /// <summary>
        /// Writes an array of <see cref="IPrimitiveValue"/> structs to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of <see cref="IPrimitiveValue"/> structs</param>
        public void Write<T>(string name, T[] values) where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();
            WriteArray(values, value => Write(null, value));
        }

        /// <summary>
        /// Writes a single nullable <see cref="IPrimitiveValue"/> struct to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Nullable <see cref="IPrimitiveValue"/> struct value.</param>
        public void WriteNullable<T>(string name, T? value) where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();

            _binaryWriter.Write(value.HasValue);

            if (value.HasValue)
            {
                value.Value.Write(this);
            }
        }

        /// <summary>
        /// Writes a single nullable <see cref="IPrimitiveValue"/> struct to the output.
        /// </summary>
        /// <typeparam name="T">Struct type.</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Nullable <see cref="IPrimitiveValue"/> struct value.</param>
        public void WriteNullable<T>(string name, ref T? value) where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();

            _binaryWriter.Write(value.HasValue);

            if (value.HasValue)
            {
                value.Value.Write(this);
            }
        }

        /// <summary>
        /// Writes an enum value to the output.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="enumValue">Enum value</param>
        public void WriteEnum<T>(string name, T enumValue) where T : struct, IComparable, IFormattable, IConvertible
        {
            ThrowIfDisposed();

            TypeCode typeCode = enumValue.GetTypeCode();
            object cast = enumValue;

            switch (typeCode)
            {
                case TypeCode.Byte:
                    _binaryWriter.Write((byte)cast);
                    return;
                case TypeCode.SByte:
                    _binaryWriter.Write((sbyte)cast);
                    return;
                case TypeCode.Int16:
                    _binaryWriter.Write((short)cast);
                    return;
                case TypeCode.UInt16:
                    _binaryWriter.Write((ushort)cast);
                    return;
                case TypeCode.Int32:
                    _binaryWriter.Write((int)cast);
                    return;
                case TypeCode.UInt32:
                    _binaryWriter.Write((uint)cast);
                    return;
                case TypeCode.Int64:
                    _binaryWriter.Write((long)cast);
                    return;
                case TypeCode.UInt64:
                    _binaryWriter.Write((ulong)cast);
                    return;
                default:
                    throw new InvalidOperationException("Enum typecode not valid");
            }
        }

        /// <summary>
        /// Writes an array of values to the 
        /// </summary>
        /// <typeparam name="T">Type of values to write</typeparam>
        /// <param name="values">Array of values to write</param>
        /// <param name="writeFunc">Action to write a value to the underlying buffer</param>
        protected void WriteArray<T>(T[] values, Action<T> writeFunc)
        {
            if (IsArrayNull(values))
            {
                _binaryWriter.Write(BinaryConstants.NULL_OBJECT);
                return;
            }
            else
            {
                _binaryWriter.Write(BinaryConstants.A_OK);
            }

            _binaryWriter.Write(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                writeFunc(values[i]);
            }
        }

        /// <summary>
        /// Checks if the array is null or has zero length.
        /// </summary>
        /// <param name="array">Array to write</param>
        /// <returns>True if null/empty, false otherwise.</returns>
        protected bool IsArrayNull(Array array)
        {
            return array == null || array.Length == 0;
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

        /// <summary>
        /// Performs the dispose action
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void DisposeInternal(bool isDisposing)
        {
            if (isDisposing)
            {
                Flush();
                _binaryWriter.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class SwappableBinaryWriter : BinaryWriter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="output"></param>
            /// <param name="encoding"></param>
            /// <param name="leaveOpen"></param>
            public SwappableBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) 
                : base(output, encoding, leaveOpen)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="outStream"></param>
            public void SetOutStream(Stream outStream)
            {
                OutStream = outStream;
            }
        }
    }
}
