namespace Spark.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Content;

    /// <summary>
    /// Interface for a read-only buffer that contains typed elements of a certain size that allows for both relative/absolute get operations that generically
    /// treats the data as a blob of bytes.
    /// </summary>
    public interface IReadOnlyDataBuffer : ISavable, IDisposable, IEnumerable
    {
        /// <summary>
        /// Gets or sets the current position pointer of the buffer. This is used and incremented by relative get/set operations.
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// Gets the number of elements that the buffer can contain.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the remaining length in the buffer (Length - Position).
        /// </summary>
        int RemainingLength { get; }

        /// <summary>
        /// Gets the total size of the buffer in bytes.
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// Gets the size of a single element in the buffer.
        /// </summary>
        int ElementSizeInBytes { get; }

        /// <summary>
        /// Gets the type of the element that the buffer contains.
        /// </summary>
        Type ElementType { get; }

        /// <summary>
        /// Gets if the data buffer has been mapped for direct access.
        /// </summary>
        bool IsMapped { get; }

        /// <summary>
        /// Gets if the buffer has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets if the position pointer can be incremented.
        /// </summary>
        bool HasNext { get; }

        /// <summary>
        /// Maps the data buffer for direct pointer access.
        /// </summary>
        /// <returns>Mapped pointer.</returns>
        MappedDataBuffer Map();

        /// <summary>
        /// Unamps the data buffer pointer obtained when the databuffer was mapped.
        /// </summary>
        void Unmap();

        /// <summary>
        /// Checks if the position pointer can be incremented N more times.
        /// </summary>
        /// <param name="numElements">Number of elements in the sequence.</param>
        /// <returns>True if the buffer has enough room to add the next N elements.</returns>
        bool HasNextFor(int numElements);

        /// <summary>
        /// Allocates a new (writable) data buffer and copies the data to it.
        /// </summary>
        /// <returns>Cloned data buffer.</returns>
        IDataBuffer Clone();

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Byte array to store the data in.</param>
        void GetBytes(byte[] data);

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        void GetBytes(byte[] data, int byteCount);

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="startIndexInByteArray">Zero-based index in the byte array to start copying data to.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        void GetBytes(byte[] data, int startIndexInByteArray, int byteCount);

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Byte array to store the data in.</param>
        void GetBytes(int index, byte[] data);

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        void GetBytes(int index, byte[] data, int byteCount);

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Byte array to store the data in.</param>
        /// <param name="startIndexInByteArray">Zero-based index in the byte array to start copying data to.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        void GetBytes(int index, byte[] data, int startIndexInByteArray, int byteCount);

        /// <summary>
        /// Relative bulk get operation that will read the specified number of bytes from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        /// <returns>The byte array containing the requested data.</returns>
        byte[] GetBytes(int byteCount);

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of bytes from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="byteCount">Number of bytes to read from the buffer.</param>
        /// <returns>The byte array containing the requested data.</returns>
        byte[] GetBytes(int index, int byteCount);
    }

    ///<summary>
    /// Interface for a typed read-only buffer that allows for both relative/absolute get operations.
    /// </summary>
    /// <typeparam name="T">Type of data in buffer.</typeparam>
    public interface IReadOnlyDataBuffer<T> : IEnumerable<T>, IReadOnlyDataBuffer where T : struct
    {
        /// <summary>
        /// Gets an element at the specified index in the buffer.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        T this[int index] { get; }
        
        /// <summary>
        /// Relative get operation that reads the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <returns>The value read from the buffer.</returns>
        T Get();

        /// <summary>
        /// Relative get operation that reads the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value read from the buffer.</param>
        void Get(out T value);
        
        /// <summary>
        /// Absolute get operation that reads the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to read the element from.</param>
        /// <returns>The value read from the buffer.</returns>
        T Get(int index);

        /// <summary>
        /// Absolute get operation that reads the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to read the element from.</param>
        /// <param name="value">The value read from the buffer.</param>
        void Get(int index, out T value);
        
        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        void GetRange(T[] data);

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        void GetRange(T[] data, int count);

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data to.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        void GetRange(T[] data, int startIndexInArray, int count);

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Element array to store the data in.</param>
        void GetRange(int index, T[] data);

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        void GetRange(int index, T[] data, int count);

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="data">Element array to store the data in.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data to.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        void GetRange(int index, T[] data, int startIndexInArray, int count);

        /// <summary>
        /// Relative bulk get operation that will read the specified number of elements from the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements read.
        /// </summary>
        /// <param name="count">Number of elements to read from the buffer.</param>
        /// <returns>The element array containing the requested data.</returns>
        T[] GetRange(int count);

        /// <summary>
        /// Absolute bulk get operation that will read the specified number of elements from the buffer starting at the specified element index. This does not
        /// advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start reading from.</param>
        /// <param name="count">Number of elements to read from the buffer.</param>
        /// <returns>The element array containing the requested data.</returns>
        T[] GetRange(int index, int count);
    }
}
