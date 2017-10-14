namespace Spark.Content
{
    using Core;

    using System;

    /// <summary>
    /// Interface that defines a writer that can write primitive data to an output as single values, arrays, and two dimensional arrays. 
    /// This data includes the built-in runtime types, enums, strings, and custom primitive data types.
    /// </summary>
    public interface IPrimitiveWriter : IDisposable
    {
        /// <summary>
        /// Gets if the writer has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Closes the underlying stream.
        /// </summary>
        void Close();

        /// <summary>
        /// Clears all buffers and causes any buffered data to be written.
        /// </summary>
        void Flush();

        /// <summary>
        /// Writes a single byte to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Byte value</param>
        void Write(string name, byte value);

        /// <summary>
        /// Writes an array of bytes to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of bytes</param>
        void Write(string name, byte[] values);

        /// <summary>
        /// Writes a single sbyte to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">SByte value</param>
        void Write(string name, sbyte value);

        /// <summary>
        /// Writes an array of sbytes to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of sbytes</param>
        void Write(string name, sbyte[] values);

        /// <summary>
        /// Writes a single char to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Char value</param>
        void Write(string name, char value);

        /// <summary>
        /// Writes an array of chars to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of chars</param>
        void Write(string name, char[] values);

        /// <summary>
        /// Writes a single unsigned 16-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt16 value</param>
        void Write(string name, ushort value);

        /// <summary>
        /// Writes an array of unsigned 16-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt16s</param>
        void Write(string name, ushort[] values);

        /// <summary>
        /// Writes a single unsigned 32-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt32 value</param>
        void Write(string name, uint value);

        /// <summary>
        /// Writes an array of unsigned 32-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt32s</param>
        void Write(string name, uint[] values);

        /// <summary>
        /// Writes a single unsigned 64-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt64 value</param>
        void Write(string name, ulong value);

        /// <summary>
        /// Writes an array of unsigned 64-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt64s</param>
        void Write(string name, ulong[] values);

        /// <summary>
        /// Writes a single 16-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int16 value</param>
        void Write(string name, short value);

        /// <summary>
        /// Writes an array of 16-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int16s</param>
        void Write(string name, short[] values);

        /// <summary>
        /// Writes a single 32-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int32 value</param>
        void Write(string name, int value);

        /// <summary>
        /// Writes an array of 32-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int32s</param>
        void Write(string name, int[] values);

        /// <summary>
        /// Writes a single 64-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int64 value</param>
        void Write(string name, long value);

        /// <summary>
        /// Writes an array of 64-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int64s</param>
        void Write(string name, long[] values);

        /// <summary>
        /// Writes a single float value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Float value</param>
        void Write(string name, float value);

        /// <summary>
        /// Writes an array of floats to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of floats</param>
        void Write(string name, float[] values);

        /// <summary>
        /// Writes a single double value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Double value</param>
        void Write(string name, double value);

        /// <summary>
        /// Writes an array of doubles to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of doubles</param>
        void Write(string name, double[] values);

        /// <summary>
        /// Writes a single decimal value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Decimal value</param>
        void Write(string name, decimal value);

        /// <summary>
        /// Writes an array of decimals to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of decimals</param>
        void Write(string name, decimal[] values);

        /// <summary>
        /// Writes a single boolean to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Boolean value</param>
        void Write(string name, bool value);

        /// <summary>
        /// Writes an array of booleans to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of booleans</param>
        void Write(string name, bool[] values);

        /// <summary>
        /// Writes a single string to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">string value</param>
        void Write(string name, string value);

        /// <summary>
        /// Writes an array of strings to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of strings</param>
        void Write(string name, string[] values);

        /// <summary>
        /// Writes an array of values to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Databuffer containing the values.</param>
        /// <param name="count">Number of values to write out, starting from the data buffer's current position.</param>
        void Write<T>(string name, IDataBuffer<T> values, int count) where T : struct;

        /// <summary>
        /// Writes single <see cref="IPrimitiveValue"/> struct to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value"><see cref="IPrimitiveValue"/> struct value</param>
        void Write<T>(string name, T value) where T : struct, IPrimitiveValue;

        /// <summary>
        /// Writes single <see cref="IPrimitiveValue"/> struct to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value"><see cref="IPrimitiveValue"/> struct value</param>
        void Write<T>(string name, ref T value) where T : struct, IPrimitiveValue;

        /// <summary>
        /// Writes an array of <see cref="IPrimitiveValue"/> structs to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of <see cref="IPrimitiveValue"/> structs</param>
        void Write<T>(string name, T[] values) where T : struct, IPrimitiveValue;

        /// <summary>
        /// Writes a single nullable <see cref="IPrimitiveValue"/> struct to the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Nullable <see cref="IPrimitiveValue"/> struct value.</param>
        void WriteNullable<T>(string name, T? value) where T : struct, IPrimitiveValue;

        /// <summary>
        /// Writes a single nullable <see cref="IPrimitiveValue"/> struct to the output.
        /// </summary>
        /// <typeparam name="T">Struct type.</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Nullable <see cref="IPrimitiveValue"/> struct value.</param>
        void WriteNullable<T>(string name, ref T? value) where T : struct, IPrimitiveValue;

        /// <summary>
        /// Writes an enum value to the output.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="enumValue">Enum value</param>
        void WriteEnum<T>(string name, T enumValue) where T : struct, IComparable, IFormattable, IConvertible;

        /// <summary>
        /// Writes a group header. Subsequent calls fill the contents of the group, be sure to call <see cref="EndWriteGroup"/> when done writing the grouped elements.
        /// </summary>
        /// <param name="name">Name of the group</param>
        void BeginWriteGroup(String name);

        /// <summary>
        /// Writes a group header that represents a collection. Subsequent calls fill the contents of the group, be sure to call <see cref="EndWriteGroup"/> when done writing the grouped elements.
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <param name="count">Number of elements in the group.</param>
        void BeginWriteGroup(String name, int count);
        
        /// <summary>
        /// Ends a grouping. Every begin must be paired with an end.
        /// </summary>
        void EndWriteGroup();
    }
}
