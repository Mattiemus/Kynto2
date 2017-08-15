namespace Spark.Content.Xml
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Core;
    using Utilities;

    /// <summary>
    /// A writer that can write built-in .NET types / primitive values in both singular and array forms to XML.
    /// </summary>
    public class XmlPrimitiveWriter : BaseDisposable, IPrimitiveWriter
    {
        private byte[] _chunkBuffer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveWriter"/> class. Use this to do pre-processing before the Xml 
        /// writer gets created. Call <see cref="InitXmlWriterForStream(Stream, XmlWriterSettings)"/> to construct the underlying
        /// xml writer.
        /// </summary>
        protected XmlPrimitiveWriter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveWriter"/> class.
        /// </summary>
        /// <param name="output">Output to write to.</param>
        /// <param name="output">Output to write to.</param>
        /// <param name="conformanceLevel">Conformance level of the underlying XML document</param>
        public XmlPrimitiveWriter(Stream output, ConformanceLevel conformanceLevel = ConformanceLevel.Fragment) 
        {
            InitXmlWriterForStream(output, CreateDefaultSettings(conformanceLevel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveWriter"/> class.
        /// </summary>
        /// <param name="output">Output to write to.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public XmlPrimitiveWriter(Stream output, bool leaveOpen) 
            : this(output, new XmlWriterSettings() { CloseOutput = !leaveOpen, Encoding = Encoding.UTF8, Indent = true, ConformanceLevel = ConformanceLevel.Fragment })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveWriter"/> class.
        /// </summary>
        /// <param name="output">Output to write to.</param>
        /// <param name="settings">Optional settings for the underlying XML writer. If null, default settings are created.</param>
        public XmlPrimitiveWriter(Stream output, XmlWriterSettings settings)
        {
            InitXmlWriterForStream(output, settings);
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="XmlPrimitiveWriter"/> class.
        /// </summary>
        /// <param name="output">Output to write to.</param>
        /// <param name="conformanceLevel">Conformance level of the underlying XML document</param>
        public XmlPrimitiveWriter(StringBuilder output, ConformanceLevel conformanceLevel = ConformanceLevel.Fragment)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            
            UnderlyingXmlWriter = XmlWriter.Create(output, CreateDefaultSettings(conformanceLevel));
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="XmlPrimitiveWriter"/> class.
        /// </summary>
        /// <param name="output">Output to write to.</param>
        /// <param name="settings">Optional settings for the underlying XML writer. If null, default settings are created.</param>
        public XmlPrimitiveWriter(StringBuilder output, XmlWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            UnderlyingXmlWriter = XmlWriter.Create(output, settings);
        }

        /// <summary>
        /// Gets or sets the underlying XML writer.
        /// </summary>
        protected XmlWriter UnderlyingXmlWriter { get; set; }

        /// <summary>
        /// Gets or sets the XML settings used to create the underlying XML writer.
        /// </summary>
        protected XmlWriterSettings XmlSettings { get; set; }
        
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
            UnderlyingXmlWriter.Flush();
        }

        /// <summary>
        /// Writes a single byte to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Byte value</param>
        public void Write(string name, byte value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of bytes to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of bytes</param>
        public void Write(string name, byte[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single sbyte to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">SByte value</param>
        public void Write(string name, sbyte value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of sbytes to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of sbytes</param>
        public void Write(string name, sbyte[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single char to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Char value</param>
        public void Write(string name, char value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of chars to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of chars</param>
        public void Write(string name, char[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single unsigned 16-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt16 value</param>
        public void Write(string name, ushort value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of unsigned 16-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt16s</param>
        public void Write(string name, ushort[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single unsigned 32-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt32 value</param>
        public void Write(string name, uint value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of unsigned 32-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt32s</param>
        public void Write(string name, uint[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single unsigned 64-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">UInt64 value</param>
        public void Write(string name, ulong value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of unsigned 64-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of UInt64s</param>
        public void Write(string name, ulong[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single 16-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int16 value</param>
        public void Write(string name, short value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of 16-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int16s</param>
        public void Write(string name, short[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single 32-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int32 value</param>
        public void Write(string name, int value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of 32-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int32s</param>
        public void Write(string name, int[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single 64-bit int to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Int64 value</param>
        public void Write(string name, long value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of 64-bit ints to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of Int64s</param>
        public void Write(string name, long[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single float value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Float value</param>
        public void Write(string name, float value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of floats to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of floats</param>
        public void Write(string name, float[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single double value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Double value</param>
        public void Write(string name, double value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of doubles to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of doubles</param>
        public void Write(string name, double[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single decimal value to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Decimal value</param>
        public void Write(string name, decimal value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of decimals to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of decimals</param>
        public void Write(string name, decimal[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single boolean to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Boolean value</param>
        public void Write(string name, bool value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes an array of booleans to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of booleans</param>
        public void Write(string name, bool[] values)
        {
            ThrowIfDisposed();
            WriteArray(name, values, XmlConvert.ToString);
        }

        /// <summary>
        /// Writes a single string to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">string value</param>
        public void Write(string name, string value)
        {
            ThrowIfDisposed();
            WriteSingleValue(name, value, x => x);
        }

        /// <summary>
        /// Writes an array of strings to the output.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of strings</param>
        public void Write(string name, string[] values)
        {
            ThrowIfDisposed();

            UnderlyingXmlWriter.WriteStartElement(name);

            if (!IsArrayNull(values))
            {
                int count = values.Length;
                UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(count));

                for (int i = 0; i < count; i++)
                {
                    Write("Item", values[i]);
                }
            }

            UnderlyingXmlWriter.WriteEndElement();
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

            UnderlyingXmlWriter.WriteStartElement(name);
            if (values != null && count > 0)
            {
                if (count > values.RemainingLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Buffer write overflow");
                }

                int elemSizeInBytes = values.ElementSizeInBytes;
                int byteCount = count * elemSizeInBytes;

                UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(byteCount));
                UnderlyingXmlWriter.WriteAttributeString("ElementSize", XmlConvert.ToString(elemSizeInBytes));

                byte[] buffer = GetTempBuffer();

                while (byteCount > 0)
                {
                    int numBytes = Math.Min(buffer.Length, byteCount);
                    values.GetBytes(buffer, numBytes);
                    UnderlyingXmlWriter.WriteBase64(buffer, 0, numBytes);

                    byteCount -= numBytes;
                }
            }
            UnderlyingXmlWriter.WriteEndElement();
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

            UnderlyingXmlWriter.WriteStartElement(name);
            value.Write(this);
            UnderlyingXmlWriter.WriteEndElement();
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

            UnderlyingXmlWriter.WriteStartElement(name);
            value.Write(this);
            UnderlyingXmlWriter.WriteEndElement();
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

            UnderlyingXmlWriter.WriteStartElement(name);
            if (!IsArrayNull(values))
            {
                int count = values.Length;
                UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(count));

                for (int i = 0; i < count; i++)
                {
                    Write("Item", ref values[i]);
                }
            }
            UnderlyingXmlWriter.WriteEndElement();
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

            UnderlyingXmlWriter.WriteStartElement(name);
            if (value.HasValue)
            {
                value.Value.Write(this);
            }
            UnderlyingXmlWriter.WriteEndElement();
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

            UnderlyingXmlWriter.WriteStartElement(name);
            if (value.HasValue)
            {
                value.Value.Write(this);
            }
            UnderlyingXmlWriter.WriteEndElement();
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
            WriteSingleValue(name, enumValue, x => x.ToString());
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
                UnderlyingXmlWriter.Close();
            }
        }

        /// <summary>
        /// Writes a single value to the XML stream
        /// </summary>
        /// <typeparam name="T">Type of value to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Value to write</param>
        /// <param name="toString">Function to convert the value to a string</param>
        protected void WriteSingleValue<T>(string name, T value, Func<T, string> toString)
        {
            UnderlyingXmlWriter.WriteStartElement(name);
            UnderlyingXmlWriter.WriteString(toString(value));
            UnderlyingXmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes an array of values to the XML stream
        /// </summary>
        /// <typeparam name="T">Type of values to write</typeparam>
        /// <param name="name">Name of the set of values</param>
        /// <param name="values">Values to be written</param>
        /// <param name="toString">Function to convert a value to a string</param>
        protected void WriteArray<T>(string name, T[] values, Func<T, string> toString)
        {
            UnderlyingXmlWriter.WriteStartElement(name);

            if (!IsArrayNull(values))
            {
                int count = values.Length;
                UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(count));

                for (int i = 0; i < count; i++)
                {
                    if (i != 0)
                    {
                        WriteArraySeparator();
                    }

                    UnderlyingXmlWriter.WriteString(toString(values[i]));
                }
            }

            UnderlyingXmlWriter.WriteEndElement();
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
        /// Writes an array separator as a string.
        /// </summary>
        protected void WriteArraySeparator()
        {
            UnderlyingXmlWriter.WriteString(" ");
        }

        /// <summary>
        /// Called if no write settings are passed to the constructor.
        /// </summary>
        /// <param name="conformanceLevel">Level of conformance of the XML document</param>
        /// <returns>Default writer settings.</returns>
        protected XmlWriterSettings CreateDefaultSettings(ConformanceLevel conformanceLevel)
        {
            return new XmlWriterSettings
            {
                CloseOutput = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                ConformanceLevel = conformanceLevel
            };
        }

        /// <summary>
        /// Initializes the XML writer with a stream. Use this with the protected parameterless constructor if you need to do pre-processing before the XML writer is
        /// created.
        /// </summary>
        /// <param name="input">Output stream.</param>
        /// <param name="settings">Optional writer settings, if null then default settings are created.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the output stream is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the output stream is read-only.</exception>
        protected void InitXmlWriterForStream(Stream output, XmlWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            XmlSettings = settings;

            if (!output.CanWrite)
            {
                throw new ArgumentException("Cannot write to stream");
            }

            UnderlyingXmlWriter = XmlWriter.Create(output, settings);
        }
    }
}
