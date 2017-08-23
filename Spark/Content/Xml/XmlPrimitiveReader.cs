namespace Spark.Content.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Core;
    using Core.Interop;
    using Utilities;

    /// <summary>
    /// A reader that can read built-in .NET types / primitive values in both singular and array forms from XML.
    /// </summary>
    public class XmlPrimitiveReader : BaseDisposable, IPrimitiveReader
    {
        private static Dictionary<Type, Action<string, IntPtr>> PrimitiveConversions;

        private byte[] _chunkBuffer;
        private StringBuilder _arrayBuffer;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static XmlPrimitiveReader()
        {
            InitializePrimitiveConversions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveReader"/> class. Use this to do pre-processing before the Xml reader gets created. Call <see cref="InitXmlReaderForStream(Stream, XmlReaderSettings)"/>
        /// to construct the underlying xml reader.
        /// </summary>
        protected XmlPrimitiveReader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveReader"/> class.
        /// </summary>
        /// <param name="input">Input stream containing XML.</param>
        /// <param name="conformanceLevel">Conformance level of the underlying XML document</param>
        public XmlPrimitiveReader(Stream input, ConformanceLevel conformanceLevel = ConformanceLevel.Fragment)
        {
            InitXmlReaderForStream(input, CreateDefaultSettings(conformanceLevel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveReader"/> class.
        /// </summary>
        /// <param name="input">Input stream containing XML.</param>
        /// <param name="leaveOpen">True if the input stream should be left open, false if the reader will close it after reading.</param>
        public XmlPrimitiveReader(Stream input, bool leaveOpen) 
            : this(input, new XmlReaderSettings() { CloseInput = !leaveOpen, ConformanceLevel = ConformanceLevel.Fragment })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveReader"/> class.
        /// </summary>
        /// <param name="input">Input stream containing XML.</param>
        /// <param name="settings">Optional settings for the underlying XML reader. If null, default settings are created.</param>
        public XmlPrimitiveReader(Stream input, XmlReaderSettings settings)
        {
            InitXmlReaderForStream(input, settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveReader"/> class.
        /// </summary>
        /// <param name="xmlText">XML text to parse.</param>
        /// <param name="conformanceLevel">Conformance level of the underlying XML document</param>
        public XmlPrimitiveReader(string xmlText, ConformanceLevel conformanceLevel = ConformanceLevel.Fragment) 
        {
            if (string.IsNullOrEmpty(xmlText))
            {
                throw new ArgumentNullException(nameof(xmlText));
            }

            UnderlyingXmlReader = XmlReader.Create(new StringReader(xmlText), CreateDefaultSettings(conformanceLevel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPrimitiveReader"/> class.
        /// </summary>
        /// <param name="xmlText">XML text to parse.</param>
        /// <param name="settings">Optional settings for the underlying XML reader. If null, default settings are created.</param>
        public XmlPrimitiveReader(string xmlText, XmlReaderSettings settings)
        {
            if (string.IsNullOrEmpty(xmlText))
            {
                throw new ArgumentNullException(nameof(xmlText));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            UnderlyingXmlReader = XmlReader.Create(new StringReader(xmlText), settings);
        }

        /// <summary>
        /// Gets or sets the underlying XML reader.
        /// </summary>
        protected XmlReader UnderlyingXmlReader { get; set; }

        /// <summary>
        /// Gets or sets the XML settings used to create the underlying XML reader.
        /// </summary>
        protected XmlReaderSettings XmlSettings { get; set; }
        
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
            return ReadSingleValue(XmlConvert.ToByte);
        }

        /// <summary>
        /// Reads an array of bytes from the input.
        /// </summary>
        /// <returns>Array of bytes</returns>
        public byte[] ReadByteArray()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToByte);
        }

        /// <summary>
        /// Reads a single sbyte from the input.
        /// </summary>
        /// <returns>SByte value</returns>
        public sbyte ReadSByte()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToSByte);
        }

        /// <summary>
        /// Reads an array of sbytes from the input.
        /// </summary>
        /// <returns>Array of sbytes</returns>
        public sbyte[] ReadSByteArray()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToSByte);
        }

        /// <summary>
        /// Reads a single char from the input.
        /// </summary>
        /// <returns>Char value</returns>
        public char ReadChar()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToChar);
        }

        /// <summary>
        /// Reads an array of chars from the input.
        /// </summary>
        /// <returns>Array of chars</returns>
        public char[] ReadCharArray()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToChar);
        }

        /// <summary>
        /// Reads a single unsigned 16-bit int from the input.
        /// </summary>
        /// <returns>UInt16 value</returns>
        public ushort ReadUInt16()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToUInt16);
        }

        /// <summary>
        /// Reads an array of unsigned 16-bit ints from the input.
        /// </summary>
        /// <returns>Array of UInt16s</returns>
        public ushort[] ReadUInt16Array()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToUInt16);
        }

        /// <summary>
        /// Reads a single unsigned 32-bit int from the input.
        /// </summary>
        /// <returns>UInt32 value</returns>
        public uint ReadUInt32()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToUInt32);
        }

        /// <summary>
        /// Reads an array of unsigned 32-bits int from the input.
        /// </summary>
        /// <returns>Array of UInt32s</returns>
        public uint[] ReadUInt32Array()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToUInt32);
        }

        /// <summary>
        /// Reads a single unsigned 64-bit int from the input.
        /// </summary>
        /// <returns>UInt64 value</returns>
        public ulong ReadUInt64()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToUInt64);
        }

        /// <summary>
        /// Reads an array of unsigned 64-bits int from the input.
        /// </summary>
        /// <returns>Array of UInt64s</returns>
        public ulong[] ReadUInt64Array()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToUInt64);
        }

        /// <summary>
        /// Reads a single 16-bit int from the input.
        /// </summary>
        /// <returns>Int16 value</returns>
        public short ReadInt16()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToInt16);
        }

        /// <summary>
        /// Reads an array of 16-bits int from the input.
        /// </summary>
        /// <returns>Array of Int16s</returns>
        public short[] ReadInt16Array()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToInt16);
        }

        /// <summary>
        /// Reads a single 32-bit int from the input.
        /// </summary>
        /// <returns>Int32 value</returns>
        public int ReadInt32()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToInt32);
        }

        /// <summary>
        /// Reads an array of 32-bits int from the input.
        /// </summary>
        /// <returns>Array of Int32s</returns>
        public int[] ReadInt32Array()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToInt32);
        }

        /// <summary>
        /// Reads a single 64-bit int from the input.
        /// </summary>
        /// <returns>Int64 value</returns>
        public long ReadInt64()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToInt64);
        }

        /// <summary>
        /// Reads an array of 64-bits int from the input.
        /// </summary>
        /// <returns>Array of Int64s</returns>
        public long[] ReadInt64Array()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToInt64);
        }

        /// <summary>
        /// Reads a single float from the input.
        /// </summary>
        /// <returns>Float value</returns>
        public float ReadSingle()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToSingle);
        }

        /// <summary>
        /// Reads an array of floats from the input.
        /// </summary>
        /// <returns>Array of floats</returns>
        public float[] ReadSingleArray()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToSingle);
        }

        /// <summary>
        /// Reads a single double from the input.
        /// </summary>
        /// <returns>Double value</returns>
        public double ReadDouble()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToDouble);
        }

        /// <summary>
        /// Reads an array of doubles from the input.
        /// </summary>
        /// <returns>Array of doubles</returns>
        public double[] ReadDoubleArray()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToDouble);
        }

        /// <summary>
        /// Reads a single decimal from the input.
        /// </summary>
        /// <returns>Decimal value</returns>
        public decimal ReadDecimal()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToDecimal);
        }

        /// <summary>
        /// Reads an array of decimals from the input.
        /// </summary>
        /// <returns>Array of decimals</returns>
        public decimal[] ReadDecimalArray()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToDecimal);
        }

        /// <summary>
        /// Reads a single boolean from the input.
        /// </summary>
        /// <returns>Boolean value</returns>
        public bool ReadBoolean()
        {
            ThrowIfDisposed();
            return ReadSingleValue(XmlConvert.ToBoolean);
        }

        /// <summary>
        /// Reads an array of booleans from the input.
        /// </summary>
        /// <returns>Array of booleans</returns>
        public bool[] ReadBooleanArray()
        {
            ThrowIfDisposed();
            return ReadArray(XmlConvert.ToBoolean);
        }

        /// <summary>
        /// Reads a string from the input.
        /// </summary>
        /// <returns>String value</returns>
        public string ReadString()
        {
            ThrowIfDisposed();
            return ReadSingleValue(x => x);
        }

        /// <summary>
        /// Reads an array of strings from the input.
        /// </summary>
        /// <returns>Array of strings</returns>
        public string[] ReadStringArray()
        {
            ThrowIfDisposed();
            ReadToStartOfNextElement();

            string[] values = null;

            if (!UnderlyingXmlReader.IsEmptyElement && ReadCountAttribute(out int count))
            {
                values = new string[count];

                UnderlyingXmlReader.ReadStartElement();

                for (int i = 0; i < count; i++)
                {
                    values[i] = ReadString();
                }

                UnderlyingXmlReader.ReadEndElement();
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return values;
        }

        /// <summary>
        /// Returns the number of elements if the next object is an array, this does not advance the reader.
        /// </summary>
        /// <returns>Non-zero if the next object is an array with elements, zero if it is null.</returns>
        public int PeekArrayCount()
        {
            ThrowIfDisposed();
            ReadToStartOfNextElement();

            if (ReadCountAttribute(out int count))
            {
                if (ReadElementSizeAttribute(out int elemSize))
                {
                    count = count / elemSize;
                }
                else
                {
                    return count;
                }
            }

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

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            ReadToStartOfNextElement();

            int destElemSize = values.ElementSizeInBytes;
            int remainingLength = values.RemainingLength;

            bool hasCount = ReadCountAttribute(out int count);
            bool hasElemSize = ReadElementSizeAttribute(out int elemSize);

            bool isBinary = hasCount && hasElemSize;
            bool hasData = hasCount && !UnderlyingXmlReader.IsEmptyElement;
            bool readData = false;

            if (hasData)
            {
                if (!isBinary)
                {
                    if (count > values.RemainingLength)
                    {
                        throw new ArgumentOutOfRangeException(nameof(values), "Buffer read overflow");
                    }

                    bool isPrimitiveValue = typeof(IPrimitiveValue).IsAssignableFrom(values.ElementType);

                    if (isPrimitiveValue)
                    {
                        UnderlyingXmlReader.ReadStartElement();

                        for (int i = 0; i < count; i++)
                        {
                            ReadToStartOfNextElement();

                            UnderlyingXmlReader.ReadStartElement();

                            IPrimitiveValue value = (IPrimitiveValue)default(T);
                            value.Read(this);

                            values.Set((T)value);

                            UnderlyingXmlReader.ReadEndElement();

                            readData = true;
                        }

                        UnderlyingXmlReader.ReadEndElement();
                    }
                    else
                    {
                        StringBuilder temp = GetTempStringBuilder();
                        string arrayContent = UnderlyingXmlReader.ReadElementContentAsString();
                        int startIndex = 0;

                        // Element is in fact one of the strongly typed arrays we can write out and not binary data...try and get a converter function,
                        // if the type does not match up, then we read the string content but don't actually push any data to the databuffer
                        Action<string, IntPtr> writeFunc = GetPrimitiveConvert<T>();
                        if (writeFunc != null)
                        {
                            using (MappedDataBuffer dbPtr = values.Map())
                            {
                                // Offset the mapped pointer to the current position
                                IntPtr ptr = dbPtr.Pointer + (values.Position * destElemSize);

                                int readCount = 0;
                                for (int i = 0; i < count; i++)
                                {
                                    if (!GetArrayElement(arrayContent, ref startIndex, temp))
                                    {
                                        break;
                                    }

                                    writeFunc(temp.ToString(), ptr);

                                    ptr += destElemSize;
                                    readCount++;
                                    readData = true;
                                }

                                values.Position += readCount;
                            }
                        }
                    }
                }
                else
                {
                    if (count > (remainingLength * destElemSize))
                    {
                        throw new ArgumentOutOfRangeException(nameof(values), "Buffer read overflow");
                    }

                    byte[] buffer = GetTempBuffer();
                    int remainingBytes = count * elemSize;

                    while (remainingBytes > 0)
                    {
                        int numBytes = Math.Min(buffer.Length, remainingBytes);
                        numBytes = UnderlyingXmlReader.ReadElementContentAsBase64(buffer, 0, numBytes);

                        // Something went wrong, no bytes were read
                        if (numBytes == 0)
                        {
                            break;
                        }

                        values.SetBytes(buffer, numBytes);

                        remainingBytes -= numBytes;
                        readData = true;
                    }
                }
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return readData;
        }

        /// <summary>
        /// Reads a single <see cref="IPrimitiveValue"/> struct from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns><see cref="IPrimitiveValue"/> struct value</returns>
        public T Read<T>() where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();
            ReadToStartOfNextElement();

            UnderlyingXmlReader.ReadStartElement();

            T value = default(T);
            value.Read(this);

            UnderlyingXmlReader.ReadEndElement();

            return value;
        }

        /// <summary>
        /// Reads a single <see cref="IPrimitiveValue"/> struct from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="value"><see cref="IPrimitiveValue"/> struct value</param>
        public void Read<T>(out T value) where T : struct, IPrimitiveValue
        {
            value = Read<T>();
        }

        /// <summary>
        /// Reads an array of <see cref="IPrimitiveValue"/> structs from the input.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Array of <see cref="IPrimitiveValue"/> structs</returns>
        public T[] ReadArray<T>() where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();
            ReadToStartOfNextElement();

            T[] values = null;

            if (!UnderlyingXmlReader.IsEmptyElement && ReadCountAttribute(out int count))
            {
                values = new T[count];

                UnderlyingXmlReader.ReadStartElement();

                for (int i = 0; i < count; i++)
                {
                    Read(out T value);
                    values[i] = value;
                }

                UnderlyingXmlReader.ReadEndElement();
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return values;
        }

        /// <summary>
        /// Reads a single nullable <see cref="IPrimitiveValue"/> struct from the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Nullable <see cref="IPrimitiveValue"/> struct value.</returns>
        public T? ReadNullable<T>() where T : struct, IPrimitiveValue
        {
            ThrowIfDisposed();
            ReadToStartOfNextElement();

            T? value = null;
            if (!UnderlyingXmlReader.IsEmptyElement)
            {
                UnderlyingXmlReader.ReadStartElement();

                T result = default(T);
                result.Read(this);
                value = result;

                UnderlyingXmlReader.ReadEndElement();
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return value;
        }

        /// <summary>
        /// Reads a single nullable <see cref="IPrimitiveValue"/> struct from the output.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="value">Nullable <see cref="IPrimitiveValue"/> struct value.</param>
        public void ReadNullable<T>(out T? value) where T : struct, IPrimitiveValue
        {
            value = ReadNullable<T>();
        }

        /// <summary>
        /// Reads an enum value from the input.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Enum value</returns
        public T ReadEnum<T>() where T : struct, IComparable, IFormattable, IConvertible
        {
            ThrowIfDisposed();
            ReadToStartOfNextElement();

            string str = UnderlyingXmlReader.ReadElementContentAsString();

            Enum.TryParse(str, true, out T enumValue);

            return enumValue;
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

            if (isDisposing)
            {
                UnderlyingXmlReader.Dispose();
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Gets a handler for converting string data into a blittable type.
        /// </summary>
        /// <typeparam name="T">Blittable type.</typeparam>
        /// <returns>Convert handler, or null if not found.</returns>
        protected Action<string, IntPtr> GetPrimitiveConvert<T>() where T : struct
        {
            if (PrimitiveConversions.TryGetValue(typeof(T), out Action<string, IntPtr> convertFunc))
            {
                return convertFunc;
            }

            return null;
        }

        /// <summary>
        /// Keeps reading parts from the XML until we get to an element node
        /// </summary>
        protected void ReadToStartOfNextElement()
        {
            if (UnderlyingXmlReader.NodeType == XmlNodeType.Element)
            {
                return;
            }

            while (UnderlyingXmlReader.Read())
            {
                if (UnderlyingXmlReader.NodeType == XmlNodeType.Element)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Tries to get an attribute with the corresponding name. This is case sensitive.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">Attribute value.</param>
        /// <returns>True if the attribute exists, false otherwise.</returns>
        protected bool TryGetAttribute(string attributeName, out string value)
        {
            value = null;

            if (!UnderlyingXmlReader.HasAttributes)
            {
                return false;
            }

            value = UnderlyingXmlReader.GetAttribute(attributeName);

            return value != null;
        }

        /// <summary>
        /// Tries to read an attribute with the name "Count" that denotes a non-null array. This is case sensitive.
        /// </summary>
        /// <param name="count">Number of items in the array.</param>
        /// <returns>True if the attribute exists, false otherwise.</returns>
        protected bool ReadCountAttribute(out int count)
        {
            if (TryGetAttribute("Count", out string countString))
            {
                count = XmlConvert.ToInt32(countString);
                return count > 0;
            }

            count = 0;
            return false;
        }

        /// <summary>
        /// Tries to read an attribute with the name "ElementSize" that denotes a binary data. This is case sensitive.
        /// </summary>
        /// <param name="count">Size of the element in bytes.</param>
        /// <returns>True if the attribute exists, false otherwise.</returns>
        protected bool ReadElementSizeAttribute(out int elemSize)
        {
            if (TryGetAttribute("ElementSize", out string countString))
            {
                elemSize = XmlConvert.ToInt32(countString);
                return elemSize > 0;
            }

            elemSize = 0;
            return false;
        }

        /// <summary>
        /// Gets an array item by substringing the input string starting at the specified index. Use this to avoid creating new strings for every array item.
        /// </summary>
        /// <param name="arrayContent">Content string containing array information.</param>
        /// <param name="startIndex">Starting index of the array item in the string. If there is another item after the current one, the index is incremented to point to the next item.</param>
        /// <param name="result">StringBuilder that contains the array item string.</param>
        /// <returns>True if an array item was substringed, false otherwise.</returns>
        protected bool GetArrayElement(string arrayContent, ref int startIndex, StringBuilder result)
        {
            result.Clear();

            if (startIndex < 0 || startIndex >= arrayContent.Length)
            {
                return false;
            }

            int nextIndex = arrayContent.IndexOf(" ", startIndex);
            if (nextIndex == -1 && startIndex < arrayContent.Length)
            {
                result.Append(arrayContent, startIndex, arrayContent.Length - startIndex);
                startIndex = arrayContent.Length;

                return true;
            }
            else if (startIndex < nextIndex)
            {
                result.Append(arrayContent, startIndex, nextIndex - startIndex);
                startIndex = nextIndex + 1;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads a single value from the XML stream
        /// </summary>
        /// <typeparam name="T">Type of value to read</typeparam>
        /// <param name="fromString">Function to convert the read string to the value</param>
        /// <returns>Value read from the next position</returns>
        protected T ReadSingleValue<T>(Func<string, T> fromString)
        {
            ReadToStartOfNextElement();
            return fromString(UnderlyingXmlReader.ReadElementContentAsString());
        }

        /// <summary>
        /// Reads an array of values from the XML stream
        /// </summary>
        /// <typeparam name="T">Type of value to read</typeparam>
        /// <param name="fromString">Function to convert the read string to the value</param>
        /// <returns>Values read from the next position</returns>
        protected T[] ReadArray<T>(Func<string, T> fromString) where T : struct
        {
            ReadToStartOfNextElement();

            T[] array = null;

            if (!UnderlyingXmlReader.IsEmptyElement && ReadCountAttribute(out int count))
            {
                array = new T[count];
                StringBuilder temp = GetTempStringBuilder();
                string arrayContent = UnderlyingXmlReader.ReadElementContentAsString();
                int startIndex = 0;

                for (int i = 0; i < count; i++)
                {
                    if (!GetArrayElement(arrayContent, ref startIndex, temp))
                    {
                        break;
                    }

                    array[i] = fromString(temp.ToString());
                }
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return array;
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
        /// Gets and initializes (if necessary) a temporary string builder for reading
        /// array data from the stream.
        /// </summary>
        /// <returns>Temp string builder</returns>
        protected StringBuilder GetTempStringBuilder()
        {
            if (_arrayBuffer == null)
            {
                _arrayBuffer = new StringBuilder();
            }

            _arrayBuffer.Clear();
            return _arrayBuffer;
        }

        /// <summary>
        /// Called if no read settings are passed to the constructor.
        /// </summary>
        /// <param name="conformanceLevel">Level of conformance of the XML document</param>
        /// <returns>Default reader settings.</returns>
        protected XmlReaderSettings CreateDefaultSettings(ConformanceLevel conformanceLevel)
        {
            return new XmlReaderSettings()
            {
                CloseInput = true,
                ConformanceLevel = conformanceLevel
            };
        }

        /// <summary>
        /// Initializes the XML reader with a stream. Use this with the protected parameterless constructor if you need to do pre-processing before the XML reader is
        /// created.
        /// </summary>
        /// <param name="input">Input stream.</param>
        /// <param name="settings">Optional reader settings, if null then default settings are created.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the input stream is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the input stream is write-only.</exception>
        protected void InitXmlReaderForStream(Stream input, XmlReaderSettings settings)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            XmlSettings = settings;

            if (!input.CanRead)
            {
                throw new ArgumentException("Cannot read from stream");
            }

            UnderlyingXmlReader = XmlReader.Create(input, settings);
        }

        /// <summary>
        /// Registers conversion handlers
        /// </summary>
        private static void InitializePrimitiveConversions()
        {
            PrimitiveConversions = new Dictionary<Type, Action<string, IntPtr>>
            {
                {
                    typeof(byte),
                    (string data, IntPtr ptr) =>
                    {
                         byte value = XmlConvert.ToByte(data);
                         MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(sbyte),
                    (string data, IntPtr ptr) =>
                    {
                        sbyte value = XmlConvert.ToSByte(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(char),
                    (string data, IntPtr ptr) =>
                    {
                        char value = XmlConvert.ToChar(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(ushort),
                    (string data, IntPtr ptr) =>
                    {
                        ushort value = XmlConvert.ToUInt16(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(uint),
                    (string data, IntPtr ptr) =>
                    {
                        uint value = XmlConvert.ToUInt32(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(ulong),
                    (string data, IntPtr ptr) =>
                    {
                        ulong value = XmlConvert.ToUInt64(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(short),
                    (string data, IntPtr ptr) =>
                    {
                        short value = XmlConvert.ToInt16(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(int),
                    (string data, IntPtr ptr) =>
                    {
                        int value = XmlConvert.ToInt32(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(long),
                    (string data, IntPtr ptr) =>
                    {
                        long value = XmlConvert.ToInt64(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(float),
                    (string data, IntPtr ptr) =>
                    {
                        float value = XmlConvert.ToSingle(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(double),
                    (string data, IntPtr ptr) =>
                    {
                        double value = XmlConvert.ToDouble(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(decimal),
                    (string data, IntPtr ptr) =>
                    {
                        decimal value = XmlConvert.ToDecimal(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                },
                {
                    typeof(bool),
                    (string data, IntPtr ptr) =>
                    {
                        bool value = XmlConvert.ToBoolean(data);
                        MemoryHelper.Write(ptr, ref value);
                    }
                }
            };
        }
    }
}
