namespace Spark.Content.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using Utilities;

    /// <summary>
    /// Savable reader that deserializes content from an XML format.
    /// </summary>
    public class XmlSavableReader : XmlPrimitiveReader, ISavableReader
    {
        private short _version;
        private ExternalSharedMode _externalSharedMode;
        private readonly Dictionary<string, ISavable> _sharedSavables;
        private readonly Dictionary<string, Tuple<ExternalReference, ISavable>> _externalSavables;

        private readonly Stream _originalInput;
        private readonly long _startLocInInput;
        private readonly string _originalXmlInput;
        private XmlReader _cachedReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="input">Input stream containing XML.</param>
        public XmlSavableReader(IServiceProvider serviceProvider, Stream input) 
            : this(serviceProvider, input, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="input">Input stream containing XML.</param>
        /// <param name="leaveOpen">True if the input stream should be left open, false if the reader will close it after reading.</param>
        public XmlSavableReader(IServiceProvider serviceProvider, Stream input, bool leaveOpen) 
            : this(serviceProvider, input, null, new XmlReaderSettings() { CloseInput = !leaveOpen, ConformanceLevel = ConformanceLevel.Document })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="input">Input stream containing XML.</param>
        /// <param name="externalResolver">An optional external resolver used to load external references. If not present, then external savables cannot be resolved.</param>
        public XmlSavableReader(IServiceProvider serviceProvider, Stream input, IExternalReferenceResolver externalResolver) 
            : this(serviceProvider, input, externalResolver, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="input">Input stream containing XML.</param>
        /// <param name="externalResolver">An optional external resolver used to load external references. If not present, then external savables cannot be resolved.</param>
        /// <param name="leaveOpen">True if the input stream should be left open, false if the reader will close it after reading.</param>
        public XmlSavableReader(IServiceProvider serviceProvider, Stream input, IExternalReferenceResolver externalResolver, bool leaveOpen) 
            : this(serviceProvider, input, externalResolver, new XmlReaderSettings() { CloseInput = !leaveOpen, ConformanceLevel = ConformanceLevel.Document })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="input">Input stream containing XML.</param>
        /// <param name="externalResolver">An optional external resolver used to load external references. If not present, then external savables cannot be resolved.</param>
        /// <param name="settings">Optional settings for the underlying XML reader. If null, default settings are created.</param>
        public XmlSavableReader(IServiceProvider serviceProvider, Stream input, IExternalReferenceResolver externalResolver, XmlReaderSettings settings)
        {
            ServiceProvider = serviceProvider;
            if (ServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (input == null || !input.CanRead)
            {
                throw new ArgumentNullException(nameof(input));
            }

            ExternalResolver = externalResolver;
            _originalInput = input;
            _startLocInInput = input.Position;
            _originalXmlInput = null;
            _sharedSavables = new Dictionary<string, ISavable>();
            _externalSavables = new Dictionary<string, Tuple<ExternalReference, ISavable>>();
            _externalSharedMode = ExternalSharedMode.Default;

            InitXmlReaderForStream(input, settings);
            InitReading();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="xmlText">The XML text.</param>
        public XmlSavableReader(IServiceProvider serviceProvider, string xmlText) 
            : this(serviceProvider, xmlText, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="xmlText">The XML text.</param>
        /// <param name="settings">The settings.</param>
        public XmlSavableReader(IServiceProvider serviceProvider, string xmlText, IExternalReferenceResolver externalResolver) 
            : this(serviceProvider, xmlText, externalResolver, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="xmlText">The XML text.</param>
        /// <param name="externalResolver">An optional external resolver used to load external references. If not present, then external savables cannot be resolved.</param>
        /// <param name="settings">Optional settings for the underlying XML reader. If null, default settings are created.</param>
        public XmlSavableReader(IServiceProvider serviceProvider, string xmlText, IExternalReferenceResolver externalResolver, XmlReaderSettings settings)
            : base(xmlText, settings)
        {
            ServiceProvider = serviceProvider;
            if (ServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (string.IsNullOrEmpty(xmlText))
            {
                throw new ArgumentNullException(nameof(xmlText));
            }

            ExternalResolver = externalResolver;
            _originalInput = null;
            _startLocInInput = 0;
            _originalXmlInput = xmlText;
            _sharedSavables = new Dictionary<string, ISavable>();
            _externalSavables = new Dictionary<string, Tuple<ExternalReference, ISavable>>();
            _externalSharedMode = ExternalSharedMode.Default;

            InitReading();
        }

        /// <summary>
        /// Gets the service provider, used to locate any services that may be needed during content loading.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the external reference resolver of the reader.
        /// </summary>
        public IExternalReferenceResolver ExternalResolver { get; }

        /// <summary>
        /// Reads a savable object from the input.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Object read</returns>
        public T ReadSavable<T>() where T : ISavable
        {
            ThrowIfDisposed();

            ReadToStartOfNextElement();
            
            T savable = default(T);            
            if (!UnderlyingXmlReader.IsEmptyElement && TryGetAttribute("Type", out string typeName))
            {
                // Consume the node element
                UnderlyingXmlReader.ReadStartElement();

                Type type = SmartActivator.GetType(typeName);
                object obj = SmartActivator.CreateInstance(type);                
                if (obj != null)
                {
                    if (!(obj is T))
                    {
                        throw new InvalidCastException($"Could not match type {obj.GetType().FullName} with type {typeof(T).FullName}");
                    }

                    savable = (T)obj;
                    savable.Read(this);
                }

                UnderlyingXmlReader.ReadEndElement();
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return savable;
        }

        /// <summary>
        /// Reads an array of savable objects from the input.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Array of objects read</returns>
        public T[] ReadSavableArray<T>() where T : ISavable
        {
            ThrowIfDisposed();

            ReadToStartOfNextElement();

            int count;
            T[] array = null;

            if (!UnderlyingXmlReader.IsEmptyElement && ReadCountAttribute(out count))
            {
                array = new T[count];

                UnderlyingXmlReader.ReadStartElement();

                for (int i = 0; i < count; i++)
                {
                    array[i] = ReadSavable<T>();
                }

                UnderlyingXmlReader.ReadEndElement();
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return array;
        }

        /// <summary>
        /// Reads a shared savable object from the input.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Object read</returns>
        public T ReadSharedSavable<T>() where T : ISavable
        {
            ThrowIfDisposed();

            if (_externalSharedMode == ExternalSharedMode.AllExternal)
            {
                return ReadExternalSavableInternal<T>();
            }
            else
            {
                return ReadSharedSavableInternal<T>();
            }
        }

        /// <summary>
        /// Reads an array of shared savable objects from the input.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Array of objects read</returns>
        public T[] ReadSharedSavableArray<T>() where T : ISavable
        {
            ThrowIfDisposed();

            ReadToStartOfNextElement();
            
            T[] array = null;
            if (!UnderlyingXmlReader.IsEmptyElement && ReadCountAttribute(out int count))
            {
                array = new T[count];

                UnderlyingXmlReader.ReadStartElement();

                for (int i = 0; i < count; i++)
                {
                    array[i] = ReadSharedSavable<T>();
                }

                UnderlyingXmlReader.ReadEndElement();
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return array;
        }

        /// <summary>
        /// Reads a savable object that is external from the input, e.g. a separate file.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Object read</returns>
        public T ReadExternalSavable<T>() where T : ISavable
        {
            ThrowIfDisposed();

            if (_externalSharedMode == ExternalSharedMode.AllShared)
            {
                return ReadSharedSavableInternal<T>();
            }
            else
            {
                return ReadExternalSavableInternal<T>();
            }
        }

        /// <summary>
        /// Reads an array of savable objects that is external from the input, e.g. a separate file.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Array of savables</returns>
        public T[] ReadExternalSavableArray<T>() where T : ISavable
        {
            ThrowIfDisposed();

            ReadToStartOfNextElement();
            
            T[] array = null;
            if (!UnderlyingXmlReader.IsEmptyElement && ReadCountAttribute(out int count))
            {
                array = new T[count];

                UnderlyingXmlReader.ReadStartElement();

                for (int i = 0; i < count; i++)
                {
                    array[i] = ReadExternalSavable<T>();
                }

                UnderlyingXmlReader.ReadEndElement();
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return array;
        }
        
        /// <summary>
        /// Read an external savable
        /// </summary>
        /// <typeparam name="T">Type of content to read</typeparam>
        /// <returns>Content as read from an external source</returns>
        private T ReadExternalSavableInternal<T>() where T : ISavable
        {
            ReadToStartOfNextElement();

            T value = default(T);
            if (!UnderlyingXmlReader.IsEmptyElement)
            {
                string id = UnderlyingXmlReader.ReadElementContentAsString();
                if (ExternalResolver != null && _externalSavables.TryGetValue(id, out Tuple<ExternalReference, ISavable> pair))
                {
                    if (pair.Item2 == null)
                    {
                        ISavable savableValue = ExternalResolver.ResolveSavable<ISavable>(pair.Item1);
                        if (savableValue != null)
                        {
                            _externalSavables[id] = Tuple.Create(pair.Item1, savableValue);
                        }
                    }

                    value = (T)pair.Item2;
                }
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return value;
        }

        /// <summary>
        /// Read a shared savable 
        /// </summary>
        /// <typeparam name="T">Type of content to read</typeparam>
        /// <returns>Content as read from the shared savables block</returns>
        private T ReadSharedSavableInternal<T>() where T : ISavable
        {
            ReadToStartOfNextElement();

            T value = default(T);
            if (!UnderlyingXmlReader.IsEmptyElement)
            {
                string id = UnderlyingXmlReader.ReadElementContentAsString();
                if (_sharedSavables.TryGetValue(id, out ISavable obj))
                {
                    value = (T)obj;
                }
            }
            else
            {
                UnderlyingXmlReader.Skip();
            }

            return value;
        }

        /// <summary>
        /// Reads up until a given node
        /// </summary>
        /// <param name="nodeName"></param>
        private void ReadToNode(string nodeName)
        {
            bool foundRoot = false;

            // Consume the document and get versioning
            while (UnderlyingXmlReader.Read())
            {
                if (UnderlyingXmlReader.NodeType == XmlNodeType.Element && UnderlyingXmlReader.Name.Equals(nodeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    foundRoot = true;
                    break;
                }
            }

            if (!foundRoot)
            {
                throw new SparkContentException("Invalid XML document contents");
            }
        }

        /// <summary>
        /// Initializes the reader
        /// </summary>
        private void InitReading()
        {
            // Read to document root
            ReadToNode("Content");
            
            if (TryGetAttribute("Version", out string versionAttr))
            {
                _version = XmlConvert.ToInt16(versionAttr);
            }
            else
            {
                _version = XmlConstants.VERSION_NUMBER;
            }

            if (TryGetAttribute("ExternalSharedMode", out string modeAttr))
            {
                if (!Enum.TryParse(modeAttr, true, out _externalSharedMode))
                {
                    _externalSharedMode = ExternalSharedMode.Default;
                }
            }
            else
            {
                _externalSharedMode = ExternalSharedMode.Default;
            }

            // First do a pre-pass to get all external references.
            ReadFirstPass();
            ResetReader();

            // Then a second pass to get all shared resources.
            ReadSecondPass();
            ResetReader();

            // Read to savable object
            ReadToNode("Object");
        }

        /// <summary>
        /// Resets the reader stream
        /// </summary>
        private void ResetReader()
        {
            // After we do a pass... need to reset before we get to the actual content...

            // Then reset the input with a new reader and read the object
            XmlReaderSettings settings = XmlSettings.Clone();

            if (_originalInput != null)
            {
                if (_cachedReader == null)
                {
                    // Only care about holding onto the previous reader for the sake of it not closing the stream on us, we'll clean it up in Dispose()
                    _cachedReader = UnderlyingXmlReader;
                }
                else
                {
                    UnderlyingXmlReader.Dispose();
                }

                settings.CloseInput = false;
                _originalInput.Seek(_startLocInInput - _originalInput.Position, SeekOrigin.Current);
                UnderlyingXmlReader = XmlReader.Create(_originalInput, settings);
            }
            else
            {
                // Otherwise if a string source, we can just dispose of the current reader
                UnderlyingXmlReader.Dispose();
                UnderlyingXmlReader = XmlReader.Create(new StringReader(_originalXmlInput), settings);
            }
        }

        /// <summary>
        /// Performs the first pass of the reader, searching for any external references
        /// </summary>
        private void ReadFirstPass()
        {
            XmlReader reader = UnderlyingXmlReader;
            while (reader.Read())
            {
                if (reader.IsStartElement() && !reader.IsEmptyElement && reader.Name.Equals("ExternalReferences", StringComparison.InvariantCultureIgnoreCase))
                {
                    ReadExternalReferences();
                }
            }
        }

        /// <summary>
        /// Performs the second pass of the reader, searching for any shared savables
        /// </summary>
        private void ReadSecondPass()
        {
            XmlReader reader = UnderlyingXmlReader;
            while (reader.Read())
            {
                if (reader.IsStartElement() && !reader.IsEmptyElement && reader.Name.Equals("Resources", StringComparison.InvariantCultureIgnoreCase))
                {
                    ReadSharedData();
                }
            }
        }

        /// <summary>
        /// Reads the shared data block
        /// </summary>
        private void ReadSharedData()
        {
            // Reader is already on the root element for shared data
            if (!ReadCountAttribute(out int count))
            {
                return;
            }

            UnderlyingXmlReader.ReadStartElement();

            for (int i = 0; i < count; i++)
            {
                UnderlyingXmlReader.Read();

                if (!UnderlyingXmlReader.IsEmptyElement && TryGetAttribute("Id", out string id) && TryGetAttribute("Type", out string type))
                {
                    UnderlyingXmlReader.ReadStartElement();

                    Type ttype = SmartActivator.GetType(type);
                    ISavable obj = SmartActivator.CreateInstance(ttype) as ISavable;

                    if (obj != null)
                    {
                        obj.Read(this);
                        _sharedSavables.Add(id, obj);
                    }
                }

                UnderlyingXmlReader.ReadEndElement();
            }

            UnderlyingXmlReader.ReadEndElement();
        }

        /// <summary>
        /// Reads the external references block
        /// </summary>
        private void ReadExternalReferences()
        {
            // Reader is already on the root element for external references
            if (!ReadCountAttribute(out int count))
            {
                return;
            }

            UnderlyingXmlReader.ReadStartElement();

            for (int i = 0; i < count; i++)
            {
                UnderlyingXmlReader.Read();

                if (!UnderlyingXmlReader.IsEmptyElement && TryGetAttribute("Id", out string id) && TryGetAttribute("TargetType", out string targetType))
                {
                    ExternalReference extRef = new ExternalReference(SmartActivator.GetType(targetType), UnderlyingXmlReader.ReadElementContentAsString());
                    _externalSavables.Add(id, Tuple.Create(extRef, (ISavable)null));
                }
            }

            UnderlyingXmlReader.ReadEndElement();
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
                if (_cachedReader != null)
                {
                    _cachedReader.Dispose();
                    _cachedReader = null;
                }

                UnderlyingXmlReader.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}
