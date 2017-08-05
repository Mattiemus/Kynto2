namespace Spark.Core.Interop
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Buffer that provides fast read/write access to a raw memory buffer.
    /// </summary>
    public class RawBuffer : IDisposable
    {
        private bool _isDisposed;
        private unsafe byte* _buffer;
        private readonly int _sizeInBytes;
        private readonly bool _ownsBuffer;
        private readonly GCHandle _gcHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawBuffer"/> class with the specified size.
        /// </summary>
        /// <param name="sizeInBytes">Size of the buffer in bytes</param>
        public unsafe RawBuffer(int sizeInBytes)
        {
            if (sizeInBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "Must be greater than zero.");
            }

            _buffer = (byte*)MemoryHelper.AllocateClearedMemory(sizeInBytes);
            _sizeInBytes = sizeInBytes;
            _ownsBuffer = true;

            GC.AddMemoryPressure((long)_sizeInBytes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawBuffer"/> class from an existing user buffer.
        /// </summary>
        /// <param name="userBuffer">User buffer, whose lifetime will not be managed.</param>
        /// <param name="sizeInBytes">Size of the user buffer</param>
        public unsafe RawBuffer(IntPtr userBuffer, int sizeInBytes) 
            : this(userBuffer, sizeInBytes, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawBuffer"/> class from an existing user buffer.
        /// </summary>
        /// <param name="userBuffer">User buffer</param>
        /// <param name="sizeInBytes">Size of the user buffer</param>
        /// <param name="ownsBuffer">True if the unmanaged memory should be managed by this buffer, false otherwise. The buffer should have been allocated from
        public unsafe RawBuffer(IntPtr userBuffer, int sizeInBytes, bool ownsBuffer)
        {
            if (userBuffer == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(userBuffer), "Invalid pointer to user buffer.");
            }

            if (sizeInBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "Must be greater than zero.");
            }

            _buffer = (byte*)userBuffer.ToPointer();
            _sizeInBytes = sizeInBytes;
            _ownsBuffer = ownsBuffer;

            GC.AddMemoryPressure((long)_sizeInBytes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawBuffer"/> class from an existing user buffer.
        /// </summary>
        /// <param name="userBuffer">User buffer</param>
        /// <param name="sizeInBytes">Size of the user buffer</param>
        /// <param name="pinnedBufferHandle">Pinned memory handle</param>
        /// <param name="makeCopy">Value indicating whether a copy of the input buffer should be made</param>
        private unsafe RawBuffer(void* userBuffer, int sizeInBytes, GCHandle pinnedBufferHandle, bool makeCopy)
        {
            if (userBuffer == null)
            {
                throw new ArgumentNullException(nameof(userBuffer), "Invalid pointer to user buffer.");
            }

            if (sizeInBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "Must be greater than zero.");
            }

            if (!makeCopy)
            {
                _buffer = (byte*)userBuffer;
                _sizeInBytes = sizeInBytes;
                _ownsBuffer = false;
                _gcHandle = pinnedBufferHandle;
            }
            else
            {
                _buffer = (byte*)MemoryHelper.AllocateClearedMemory(sizeInBytes);
                _sizeInBytes = sizeInBytes;
                _ownsBuffer = true;

                MemoryHelper.CopyMemory((IntPtr)_buffer, (IntPtr)userBuffer, sizeInBytes);
                GC.AddMemoryPressure((long)_sizeInBytes);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RawBuffer"/> class.
        /// </summary>
        ~RawBuffer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets if the buffer has been disposed or not.
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// Gets the pointer to the unmanaged memory this buffer controls.
        /// </summary>
        public unsafe IntPtr BufferPointer => new IntPtr(_buffer);

        /// <summary>
        /// Gets the total size of the buffer in by tes.
        /// </summary>
        public int SizeInBytes => _sizeInBytes;
        
        /// <summary>
        /// Creates a new buffer from the managed element array. The buffer size will be totalSizeInBytes - (index * elementSizeInBytes).
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="userBuffer">Element array containing the data</param>
        /// <param name="index">Index at which the buffer should start from.</param>
        /// <param name="pinBuffer">True if the userBuffer should be pinned and used as a backing store, otherwise the data is copied into unmanaged memory.</param>
        /// <returns>The created buffer.</returns>
        public static unsafe RawBuffer Create<T>(T[] userBuffer, int index = 0, bool pinBuffer = true) where T : struct
        {
            if (userBuffer == null || userBuffer.Length == 0)
            {
                throw new ArgumentNullException(nameof(userBuffer));
            }

            if (index < 0 || index > userBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range [0, userBuffer.Length - 1]");
            }

            int sizeInBytes = MemoryHelper.SizeOf<T>();
            int totalSizeInBytes = sizeInBytes * userBuffer.Length;

            RawBuffer buffer = null;
            int indexOffset = index * sizeInBytes;

            if (pinBuffer)
            {
                GCHandle handle = GCHandle.Alloc(userBuffer, GCHandleType.Pinned);
                buffer = new RawBuffer((byte*)handle.AddrOfPinnedObject() + indexOffset, totalSizeInBytes - indexOffset, handle, false);
            }
            else
            {
                GCHandle handle = GCHandle.Alloc(userBuffer, GCHandleType.Pinned);
                buffer = new RawBuffer((byte*)handle.AddrOfPinnedObject() + indexOffset, totalSizeInBytes - indexOffset, handle, true);
                handle.Free();
            }

            return buffer;
        }

        /// <summary>
        /// Reads a single value at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be read</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <returns>The read value</returns>
        public unsafe T Read<T>(int positionInBytes) where T : struct
        {
            return MemoryHelper.Read<T>((IntPtr)_buffer + positionInBytes);
        }

        /// <summary>
        /// Reads a single value at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be read</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="value">The read value.</param>
        public unsafe void Read<T>(int positionInBytes, out T value) where T : struct
        {
            value = MemoryHelper.Read<T>((IntPtr)_buffer + positionInBytes);
        }

        /// <summary>
        /// Reads a range of values starting at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be read</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="count">Number of elements to read</param>
        /// <returns>The array of values that were read</returns>
        public unsafe T[] Read<T>(int positionInBytes, int count) where T : struct
        {
            T[] values = new T[count];
            MemoryHelper.Read((IntPtr)_buffer + positionInBytes, values, 0, count);

            return values;
        }

        /// <summary>
        /// Reads a range of values starting at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be read</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="store">Array to store read values in</param>
        /// <param name="count">Number of elements to read</param>
        public unsafe void Read<T>(int positionInBytes, T[] store, int count) where T : struct
        {
            MemoryHelper.Read((IntPtr)_buffer + positionInBytes, store, 0, count);
        }

        /// <summary>
        /// Reads a range of values starting at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be read</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="store">Array to store read values in</param>
        /// <param name="startIndexInArray">Zero-based element index at which to begin storing data in the array.</param>
        /// <param name="count">Number of elements to read</param>
        public unsafe void Read<T>(int positionInBytes, T[] store, int startIndexInArray, int count) where T : struct
        {
            MemoryHelper.Read((IntPtr)_buffer + positionInBytes, store, startIndexInArray, count);
        }

        /// <summary>
        /// Writes a single value at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be written</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="value">The value to write to the buffer</param>
        public unsafe void Write<T>(int positionInBytes, ref T value) where T : struct
        {
            MemoryHelper.Write((IntPtr)_buffer + positionInBytes, value);
        }

        /// <summary>
        /// Writes a single value at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be written</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="value">The value to write to the buffer</param>
        public unsafe void Write<T>(int positionInBytes, T value) where T : struct
        {
            MemoryHelper.Write((IntPtr)_buffer + positionInBytes, value);
        }

        /// <summary>
        /// Writes a range of values starting at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be written</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="data">Array containing data to write to the buffer</param>
        public unsafe void Write<T>(int positionInBytes, T[] data) where T : struct
        {
            MemoryHelper.Write((IntPtr)_buffer + positionInBytes, data, 0, data.Length);
        }

        /// <summary>
        /// Writes a range of values starting at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be written</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="data">Array containing data to write to the buffer</param>
        /// <param name="count">Number of elements to write</param>
        public unsafe void Write<T>(int positionInBytes, T[] data, int count) where T : struct
        {
            MemoryHelper.Write((IntPtr)_buffer + positionInBytes, data, 0, count);
        }

        /// <summary>
        /// Writes a range of values starting at the specified position in the buffer.
        /// </summary>
        /// <typeparam name="T">Type of the value to be written</typeparam>
        /// <param name="positionInBytes">Position in bytes, from the start of the buffer</param>
        /// <param name="data">Array containing data to write to the buffer</param>
        /// <param name="startIndexInArray">Zero-based element index at which to begin copying data in the array.</param>
        /// <param name="count">Number of elements to write</param>
        public unsafe void Write<T>(int positionInBytes, T[] data, int startIndexInArray, int count) where T : struct
        {
            MemoryHelper.Write((IntPtr)_buffer + positionInBytes, data, startIndexInArray, count);
        }

        /// <summary>
        /// Clears the buffer to the specified value.
        /// </summary>
        /// <param name="value">Value that the buffer will be set to</param>
        public unsafe void Clear(byte value = 0)
        {
            MemoryHelper.ClearMemory((IntPtr)_buffer, _sizeInBytes, value);
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
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Pointer: {0}, SizeInbytes: {1}", BufferPointer.ToString(), SizeInBytes.ToString());
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; False to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (_gcHandle.IsAllocated)
            {
                _gcHandle.Free();
            }

            unsafe
            {
                if (_ownsBuffer && _buffer != (byte*)0)
                {
                    MemoryHelper.FreeMemory((IntPtr)_buffer);
                    _buffer = (byte*)0;

                    GC.RemoveMemoryPressure((long)_sizeInBytes);
                }
            }

            _isDisposed = true;
        }
    }
}
