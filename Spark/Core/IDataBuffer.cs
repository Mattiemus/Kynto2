namespace Spark.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a writable buffer that contains typed elements of a certain size that allows for both relative/absolute get and set operations that generically
    /// treats the data as a blob of bytes.
    /// </summary>
    public interface IDataBuffer : IReadOnlyDataBuffer
    {
        /// <summary>
        /// Clears the buffer to default values.
        /// </summary>
        void Clear();

        /// <summary>
        /// Resizes the buffer to the new size.
        /// </summary>
        /// <param name="capacity">New capacity of the databuffer, if less than length then the buffer is trimmed. If greater, then it is expanded. Must be greater than zero.</param>
        /// <returns>True if the buffer was resized.</returns>
        void Resize(int capacity);
        
        /// <summary>
        /// Relative bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        void SetBytes(byte[] data);

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        void SetBytes(byte[] data, int byteCount);

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="startIndexInByteArray">Zero-based index in the byte array to start copying data from.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        void SetBytes(byte[] data, int startIndexInByteArray, int byteCount);
        
        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the specified 
        /// element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        void SetBytes(int index, byte[] data);

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the specified 
        /// element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        void SetBytes(int index, byte[] data, int byteCount);

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of bytes from the input byte array and write the data to the buffer starting at the specified 
        /// element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Byte array containing data to write to the buffer.</param>
        /// <param name="startIndexInByteArray">Zero-based index in the byte array to start copying data from.</param>
        /// <param name="byteCount">Number of bytes to write to the buffer.</param>
        void SetBytes(int index, byte[] data, int startIndexInByteArray, int byteCount);
    }

    ///<summary>
    /// Interface for a typed writable buffer that allows for both relative/absolute get and set operations.
    /// </summary>
    /// <typeparam name="T">Type of data in buffer.</typeparam>
    public interface IDataBuffer<T> : IReadOnlyDataBuffer<T>, IDataBuffer where T : struct
    {
        /// <summary>
        /// Gets or sets an element at the specified index in the buffer.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        new T this[int index] { get; set; }
        
        /// <summary>
        /// Relative set operation that writes the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void Set(T value);

        /// <summary>
        /// Relative set operation that writes the value to the buffer at the current position pointer. The position pointer is advanced by one.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void Set(ref T value);
        
        /// <summary>
        /// Absolute set operation that writes the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to write the element to.</param>
        /// <param name="value">The value to write.</param>
        void Set(int index, ref T value);

        /// <summary>
        /// Absolute set operation that writes the value to the buffer at the specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer at which to write the element to.</param>
        /// <param name="value">The value to write.</param>
        void Set(int index, T value);
        
        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        void SetRange(T[] data);

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        void SetRange(T[] data, int count);

        /// <summary>
        /// Relative bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// current position pointer. The position pointer is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data from.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        void SetRange(T[] data, int startIndexInArray, int count);

        /// <summary>
        /// Relative bulk set operation that will copy the enumerable collection and write the data to the buffer starting at the current position pointer. The position pointer
        /// is advanced by the number of elements written.
        /// </summary>
        /// <param name="data">Enumerable collection containing data to write to the buffer.</param>
        void SetRange(IEnumerable<T> data);
        
        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        void SetRange(int index, T[] data);

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        void SetRange(int index, T[] data, int count);

        /// <summary>
        /// Absolute bulk set operation that will copy the specified number of elements from the input element array and write the data to the buffer starting at the 
        /// specified element index. This does not advance the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Element array containing data to write to the buffer.</param>
        /// <param name="startIndexInArray">Zero-based index in the element array to start copying data from.</param>
        /// <param name="count">Number of elements to write to the buffer.</param>
        void SetRange(int index, T[] data, int startIndexInArray, int count);

        /// <summary>
        /// Absolute bulk set operation that will copy the enumerable collection and write the data to the buffer starting at the specified element index. This does not advance
        /// the buffer position pointer.
        /// </summary>
        /// <param name="index">Zero-based element index in the buffer to start writing to.</param>
        /// <param name="data">Enumerable collection containing data to write to the buffer.</param>
        void SetRange(int index, IEnumerable<T> data);
    }
}
