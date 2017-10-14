namespace Spark.Content
{
    using Core;

    using System;

    /// <summary>
    /// Interface that defines a reader that can read primitive data from an input as single values, arrays, and two dimensional arrays. This
    /// data includes the built-in runtime types, enums, strings, and custom primitive data types.
    /// </summary>
    public interface IPrimitiveReader : IDisposable
    {
        /// <summary>
        /// Gets if this reader has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Closes the underlying stream.
        /// </summary>
        void Close();

        /// <summary>
        /// Reads a single byte from the input.
        /// </summary>
        /// <returns>Byte value</returns>
        byte ReadByte();

        /// <summary>
        /// Reads an array of bytes from the input.
        /// </summary>
        /// <returns>Array of bytes</returns>
        byte[] ReadByteArray();

        /// <summary>
        /// Reads a single sbyte from the input.
        /// </summary>
        /// <returns>SByte value</returns>
        sbyte ReadSByte();

        /// <summary>
        /// Reads an array of sbytes from the input.
        /// </summary>
        /// <returns>Array of sbytes</returns>
        sbyte[] ReadSByteArray();

        /// <summary>
        /// Reads a single char from the input.
        /// </summary>
        /// <returns>Char value</returns>
        char ReadChar();

        /// <summary>
        /// Reads an array of chars from the input.
        /// </summary>
        /// <returns>Array of chars</returns>
        char[] ReadCharArray();

        /// <summary>
        /// Reads a single unsigned 16-bit int from the input.
        /// </summary>
        /// <returns>UInt16 value</returns>
        ushort ReadUInt16();

        /// <summary>
        /// Reads an array of unsigned 16-bit ints from the input.
        /// </summary>
        /// <returns>Array of UInt16s</returns>
        ushort[] ReadUInt16Array();

        /// <summary>
        /// Reads a single unsigned 32-bit int from the input.
        /// </summary>
        /// <returns>UInt32 value</returns>
        uint ReadUInt32();

        /// <summary>
        /// Reads an array of unsigned 32-bits int from the input.
        /// </summary>
        /// <returns>Array of UInt32s</returns>
        uint[] ReadUInt32Array();

        /// <summary>
        /// Reads a single unsigned 64-bit int from the input.
        /// </summary>
        /// <returns>UInt64 value</returns>
        ulong ReadUInt64();

        /// <summary>
        /// Reads an array of unsigned 64-bits int from the input.
        /// </summary>
        /// <returns>Array of UInt64s</returns>
        ulong[] ReadUInt64Array();

        /// <summary>
        /// Reads a single 16-bit int from the input.
        /// </summary>
        /// <returns>Int16 value</returns>
        short ReadInt16();

        /// <summary>
        /// Reads an array of 16-bits int from the input.
        /// </summary>
        /// <returns>Array of Int16s</returns>
        short[] ReadInt16Array();

        /// <summary>
        /// Reads a single 32-bit int from the input.
        /// </summary>
        /// <returns>Int32 value</returns>
        int ReadInt32();

        /// <summary>
        /// Reads an array of 32-bits int from the input.
        /// </summary>
        /// <returns>Array of Int32s</returns>
        int[] ReadInt32Array();

        /// <summary>
        /// Reads a single 64-bit int from the input.
        /// </summary>
        /// <returns>Int64 value</returns>
        long ReadInt64();

        /// <summary>
        /// Reads an array of 64-bits int from the input.
        /// </summary>
        /// <returns>Array of Int64s</returns>
        long[] ReadInt64Array();

        /// <summary>
        /// Reads a single float from the input.
        /// </summary>
        /// <returns>Float value</returns>
        float ReadSingle();

        /// <summary>
        /// Reads an array of floats from the input.
        /// </summary>
        /// <returns>Array of floats</returns>
        float[] ReadSingleArray();

        /// <summary>
        /// Reads a single double from the input.
        /// </summary>
        /// <returns>Double value</returns>
        double ReadDouble();

        /// <summary>
        /// Reads an array of doubles from the input.
        /// </summary>
        /// <returns>Array of doubles</returns>
        double[] ReadDoubleArray();

        /// <summary>
        /// Reads a single decimal from the input.
        /// </summary>
        /// <returns>Decimal value</returns>
        decimal ReadDecimal();

        /// <summary>
        /// Reads an array of decimals from the input.
        /// </summary>
        /// <returns>Array of decimals</returns>
        decimal[] ReadDecimalArray();

        /// <summary>
        /// Reads a single boolean from the input.
        /// </summary>
        /// <returns>Boolean value</returns>
        bool ReadBoolean();

        /// <summary>
        /// Reads an array of booleans from the input.
        /// </summary>
        /// <returns>Array of booleans</returns>
        bool[] ReadBooleanArray();

        /// <summary>
        /// Reads a string from the input.
        /// </summary>
        /// <returns>String value</returns>
        string ReadString();

        /// <summary>
        /// Reads an array of strings from the input.
        /// </summary>
        /// <returns>Array of strings</returns>
        string[] ReadStringArray();

        /// <summary>
        /// Returns the number of elements if the next object is an array, this does not advance the reader.
        /// </summary>
        /// <returns>Non-zero if the next object is an array with elements, zero if it is null.</returns>
        int PeekArrayCount();

        /// <summary>
        /// Reads the group header and returns the number of elements, if the next object is a group. After reading each element, be sure to call <see cref="EndReadGroup"/>. Every begin must be paired with an end.
        /// </summary>
        /// <returns>Non-zero if the next object is an array with elements, zero if it is null.</returns>
        int BeginReadGroup();

        /// <summary>
        /// Finishes reading the group. Every begin must be paired with an end.
        /// </summary>
        void EndReadGroup();

        /// <summary>
        /// Reads an array of elements into the data buffer at the current position of the data buffer. The buffer
        /// must have enough position to read the entire array object.
        /// </summary>
        /// <typeparam name="T">Struct type.</typeparam>
        /// <param name="values">Data buffer to hold the array.</param>
        bool ReadArrayData<T>(IDataBuffer<T> values) where T : struct;

        /// <summary>
        /// Reads a single <see cref="IPrimitiveValue"/> struct from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns><see cref="IPrimitiveValue"/> struct value</returns>
        T Read<T>() where T : struct, IPrimitiveValue;

        /// <summary>
        /// Reads a single <see cref="IPrimitiveValue"/> struct from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="value"><see cref="IPrimitiveValue"/> struct value</param>
        void Read<T>(out T value) where T : struct, IPrimitiveValue;

        /// <summary>
        /// Reads an array of <see cref="IPrimitiveValue"/> structs from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Array of <see cref="IPrimitiveValue"/> structs</returns>
        T[] ReadArray<T>() where T : struct, IPrimitiveValue;

        /// <summary>
        /// Reads a single nullable <see cref="IPrimitiveValue"/> struct from the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Nullable <see cref="IPrimitiveValue"/> struct value.</returns>
        T? ReadNullable<T>() where T : struct, IPrimitiveValue;

        /// <summary>
        /// Reads a single nullable <see cref="IPrimitiveValue"/> struct from the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="value">Nullable <see cref="IPrimitiveValue"/> struct value.</param>
        void ReadNullable<T>(out T? value) where T : struct, IPrimitiveValue;

        /// <summary>
        /// Reads an enum value from the input.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Enum value</returns>
        T ReadEnum<T>() where T : struct, IComparable, IFormattable, IConvertible;
    }
}
