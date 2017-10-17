namespace Spark.Core
{
    using System;
    using System.Globalization;

    using Interop;
    using Utilities;

    /// <summary>
    /// Represents an IDataBuffer mapped for direct access via a pointer.
    /// </summary>
    public class MappedDataBuffer : BaseDisposable
    {
        private IntPtr _pointer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappedDataBuffer"/> struct.
        /// </summary>
        /// <param name="pointer">The mapped pointer.</param>
        /// <param name="dataBuffer">The data buffer that is mapped.</param>
        public MappedDataBuffer(IntPtr pointer, IDataBuffer dataBuffer)
        {
            _pointer = pointer;
            DataBuffer = dataBuffer;
        }

        /// <summary>
        /// Gets if the mapped pointer is valid. When the data buffer is unmapped, it will not be valid.
        /// </summary>
        public bool IsValid => DataBuffer != null && DataBuffer.IsMapped;

        /// <summary>
        /// Gets the mapped pointer.
        /// </summary>
        public IntPtr Pointer => IsValid ? _pointer : IntPtr.Zero;

        /// <summary>
        /// Gets the data buffer size in bytes.
        /// </summary>
        public int SizeInBytes => IsValid ? DataBuffer.SizeInBytes : 0;

        /// <summary>
        /// Gets the data buffer that is mapped for direct access.
        /// </summary>
        public IDataBuffer DataBuffer { get; }

        /// <summary>
        /// Writes a single element to the databuffer and increments the pointer by the number of bytes written. This does
        /// no range checking or other validation and should be considered to be unsafe.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="data">Data to write.</param>
        public void Write<T>(T data) where T : struct
        {
            MemoryHelper.Write<T>(_pointer, data);
            _pointer += MemoryHelper.SizeOf<T>();
        }

        /// <summary>
        /// Writes a single element to the databuffer and increments the pointer by the number of bytes specified. This does
        /// no range checking or other validation and should be considered to be unsafe.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="numBytesToIncrement">Number of bytes to increment the pointer by.</param>
        /// <param name="data">Data to write.</param>
        public void Write<T>(int numBytesToIncrement, T data) where T : struct
        {
            MemoryHelper.Write<T>(_pointer, data);
            _pointer += numBytesToIncrement;
        }

        /// <summary>
        /// Reads a single element from the databuffer and increments the pointer by the number of bytes read. This does no range
        /// checking or other validation and should be considered to be unsafe.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="data">Data to read.</param>
        public void Read<T>(out T data) where T : struct
        {
            MemoryHelper.Read<T>(_pointer, out data);
            _pointer += MemoryHelper.SizeOf<T>();
        }

        /// <summary>
        /// Reads a single element from the databuffer and increments the pointer by the number of bytes read. This does no range
        /// checking or other validation and should be considered to be unsafe.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="numBytesToIncrement">Number of bytes to increment the pointer by.</param>
        /// <param name="data">Data to read.</param>
        public void Read<T>(int numBytesToIncrement, out T data) where T : struct
        {
            MemoryHelper.Read<T>(_pointer, out data);
            _pointer += numBytesToIncrement;
        }

        /// <summary>
        /// Implicitly converts the mapped data buffer to an IntPtr.
        /// </summary>
        /// <param name="mappedDataBuffer">The mapped data buffer pointer.</param>
        /// <returns></returns>
        public static implicit operator IntPtr(MappedDataBuffer mappedDataBuffer)
        {
            return mappedDataBuffer.Pointer;
        }

        /// <summary>
        /// Adds an offset to the mapped data buffer pointer and returns the result as an IntPtr.
        /// </summary>
        /// <param name="mappedDataBuffer">Mapped data pointer</param>
        /// <param name="offset">Offset to add</param>
        /// <returns>The resulting IntPtr.</returns>
        public static IntPtr operator +(MappedDataBuffer mappedDataBuffer, int offset)
        {
            if (!mappedDataBuffer.IsValid)
            {
                return IntPtr.Zero;
            }

            return mappedDataBuffer.Pointer + offset;
        }

        /// <summary>
        /// Adds an offset to the mapped data buffer pointer and returns the result as an IntPtr.
        /// </summary>
        /// <param name="offset">Offset to add</param>
        /// <param name="mappedDataBuffer">Mapped data pointer</param>
        /// <returns>The resulting IntPtr.</returns>
        public static IntPtr operator +(int offset, MappedDataBuffer mappedDataBuffer)
        {
            if (!mappedDataBuffer.IsValid)
            {
                return IntPtr.Zero;
            }

            return mappedDataBuffer.Pointer + offset;
        }
        
        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            ThrowIfDisposed();
            return string.Format(CultureInfo.CurrentCulture, "IsValid: {0}, Pointer: {1}, SizeInBytes: {2}", IsValid.ToString(), Pointer.ToString(), SizeInBytes.ToString());
        }
        
        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (IsValid)
            {
                DataBuffer.Unmap();
            }

            base.Dispose(isDisposing);
        }
    }
}
