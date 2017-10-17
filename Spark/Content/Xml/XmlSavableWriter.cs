namespace Spark.Content.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Utilities;

    /// <summary>
    /// Savable writer that serializes content into a XML format.
    /// </summary>
    public class XmlSavableWriter : XmlPrimitiveWriter, ISavableWriter
    {
        private CyclicReferenceDetector _cyclicDetector;
        private ExternalSharedMode _externalSharedMode;

        private Queue<ISavable> _sharedQueue;
        private Dictionary<ISavable, Tuple<string, int>> _sharedIndices;
        private int _currSharedIndex;
        private bool _closeDoc;

        private List<Tuple<string, ExternalReference>> _externalRefs;
        private Dictionary<ISavable, string> _externalRefNameMap;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        public XmlSavableWriter(Stream output) 
            : base(output, ConformanceLevel.Document)
        {
            Init(SavableWriteFlags.None, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public XmlSavableWriter(Stream output, SavableWriteFlags writeFlags, bool leaveOpen)
            : base(output, leaveOpen, ConformanceLevel.Document)
        {
            Init(writeFlags, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="externalHandler">External reference handler, if null then all external references are treated as shared and are written to the output stream.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public XmlSavableWriter(Stream output, SavableWriteFlags writeFlags, IExternalReferenceHandler externalHandler, bool leaveOpen)
            : this(output, writeFlags, externalHandler, new XmlWriterSettings() { CloseOutput = !leaveOpen, Encoding = Encoding.UTF8, Indent = true, ConformanceLevel = ConformanceLevel.Document })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="externalHandler">External reference handler, if null then all external references are treated as shared and are written to the output stream.</param>
        public XmlSavableWriter(Stream output, SavableWriteFlags writeFlags, IExternalReferenceHandler externalHandler) 
            : this(output, writeFlags, externalHandler, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="externalHandler">External reference handler, if null then all external references are treated as shared and are written to the output stream.</param>
        /// <param name="settings">Optional settings for the underlying XML writer. If null, default settings are created.</param>
        public XmlSavableWriter(Stream output, SavableWriteFlags writeFlags, IExternalReferenceHandler externalHandler, XmlWriterSettings settings)
            : base(output, settings)
        {
            Init(writeFlags, externalHandler);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output"></param>
        public XmlSavableWriter(StringBuilder output) 
            : this(output, SavableWriteFlags.None, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="writeFlags"></param>
        /// <param name="leaveOpen"></param>
        public XmlSavableWriter(StringBuilder output, SavableWriteFlags writeFlags, bool leaveOpen) 
            : this(output, writeFlags, null, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="writeFlags"></param>
        /// <param name="externalHandler"></param>
        /// <param name="leaveOpen"></param>
        public XmlSavableWriter(StringBuilder output, SavableWriteFlags writeFlags, IExternalReferenceHandler externalHandler, bool leaveOpen)
            : this(output, writeFlags, externalHandler, new XmlWriterSettings() { CloseOutput = !leaveOpen, Encoding = Encoding.UTF8, Indent = true, ConformanceLevel = ConformanceLevel.Document })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="writeFlags"></param>
        /// <param name="externalHandler"></param>
        public XmlSavableWriter(StringBuilder output, SavableWriteFlags writeFlags, IExternalReferenceHandler externalHandler) 
            : this(output, writeFlags, externalHandler, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSavableWriter"/> class.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="writeFlags"></param>
        /// <param name="externalhandler"></param>
        /// <param name="settings"></param>
        public XmlSavableWriter(StringBuilder output, SavableWriteFlags writeFlags, IExternalReferenceHandler externalhandler, XmlWriterSettings settings)
            : base(output, settings)
        {
            Init(writeFlags, externalhandler);
        }

        /// <summary>
        /// Gets the external handler that controls how savables are to be written externally from the output. If no handler is set,
        /// then external savables are always handled as shared references.
        /// </summary>
        public IExternalReferenceHandler ExternalHandler { get; private set; }

        /// <summary>
        /// Gets the write flags that specify certain behaviors during serialization.
        /// </summary>
        public SavableWriteFlags WriteFlags { get; private set; }

        /// <summary>
        /// Writes a savable object to an external output that possibly is a different format. This behavior may be overridden where the savable is
        /// instead written as a shared object.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Savable object</param>
        public void WriteExternalSavable<T>(string name, T value) where T : ISavable
        {
            ThrowIfDisposed();

            UnderlyingXmlWriter.WriteStartElement(name);

            if (_externalSharedMode == ExternalSharedMode.AllShared)
            {
                WriteSharedSavableInternal(value);
            }
            else
            {
                WriteExternalSavableInternal(value);
            }

            UnderlyingXmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes an array of savable objects to an external output that possibly is a different format. This behavior may be overridden where the savable is
        /// instead written as a shared object.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of savables</param>
        public void WriteExternalSavable<T>(string name, T[] values) where T : ISavable
        {
            ThrowIfDisposed();

            UnderlyingXmlWriter.WriteStartElement(name);

            if (!IsArrayNull(values))
            {
                int count = values.Length;
                UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(count));

                for (int i = 0; i < count; i++)
                {
                    WriteExternalSavable("Item", values[i]);
                }
            }

            UnderlyingXmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes a savable object to the output.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Savable object</param>
        public void WriteSavable<T>(string name, T value) where T : ISavable
        {
            ThrowIfDisposed();

            UnderlyingXmlWriter.WriteStartElement(name);

            if (value != null)
            {
                _cyclicDetector.AddReference(value);

                UnderlyingXmlWriter.WriteAttributeString("Type", SmartActivator.GetAssemblyQualifiedName(value.GetType()));

                value.Write(this);

                _cyclicDetector.RemoveReference(value);
            }

            UnderlyingXmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes an array of savable objects to the output.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of savables</param>
        public void WriteSavable<T>(string name, T[] values) where T : ISavable
        {
            ThrowIfDisposed();

            UnderlyingXmlWriter.WriteStartElement(name);

            if (!IsArrayNull(values))
            {
                int count = values.Length;
                UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(count));

                for (int i = 0; i < count; i++)
                {
                    WriteSavable("Item", values[i]);
                }
            }

            UnderlyingXmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes a savable object to the output as a shared resource. This ensures an object that is used
        /// in multiple places is only ever written once to the output. This behavior may be overridden where the savable is instead written
        /// as an external reference.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Savable object</param>
        public void WriteSharedSavable<T>(string name, T value) where T : ISavable
        {
            ThrowIfDisposed();

            UnderlyingXmlWriter.WriteStartElement(name);

            if (_externalSharedMode == ExternalSharedMode.AllExternal)
            {
                WriteExternalSavableInternal(value);
            }
            else
            {
                WriteSharedSavableInternal(value);
            }

            UnderlyingXmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes aan array of savable object to the output where each item is a shared resource. This ensures an object that is used
        /// in multiple places is only ever written once to the output. This behavior may be overridden where each individual savable is instead written
        /// as an external reference.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of savables</param>
        public void WriteSharedSavable<T>(string name, T[] values) where T : ISavable
        {
            ThrowIfDisposed();

            UnderlyingXmlWriter.WriteStartElement(name);

            if (!IsArrayNull(values))
            {
                int count = values.Length;
                UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(count));

                for (int i = 0; i < count; i++)
                {
                    WriteSharedSavable("Item", values[i]);
                }
            }

            UnderlyingXmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Clears all buffers and causes any buffered data to be written.
        /// </summary>
        public override void Flush()
        {
            ThrowIfDisposed();

            WriteSharedResources();
            WriteExternalReferences();

            UnderlyingXmlWriter.WriteEndElement();

            if (_closeDoc)
            {
                UnderlyingXmlWriter.WriteEndDocument();
            }

            UnderlyingXmlWriter.Flush();
            
            ExternalHandler?.Flush();
        }

        /// <summary>
        /// Writes out a shared savable
        /// </summary>
        /// <typeparam name="T">Type of content to write</typeparam>
        /// <param name="value">Content to be written</param>
        private void WriteSharedSavableInternal<T>(T value) where T : ISavable
        {
            if (value == null)
            {
                return;
            }

            // The index we assign becomes part of the shared resource ID
            if (!_sharedIndices.TryGetValue(value, out Tuple<string, int> id))
            {
                _sharedQueue.Enqueue(value);
                _sharedIndices.Add(value, Tuple.Create($"{XmlConstants.BASE_SHAREDRESOURCE_NAME}{XmlConvert.ToString(_currSharedIndex)}", _currSharedIndex));

                _currSharedIndex++;
            }

            UnderlyingXmlWriter.WriteString(id.Item1);
        }

        /// <summary>
        /// Writes out an external savable
        /// </summary>
        /// <typeparam name="T">Type of content to write</typeparam>
        /// <param name="value">Content to be written</param>
        private void WriteExternalSavableInternal<T>(T value) where T : ISavable
        {
            if (value == null)
            {
                return;
            }

            // If we already processed this external reference, then it has an ID. Write that out
            if (_externalRefNameMap.TryGetValue(value, out string externalRefName))
            {
                UnderlyingXmlWriter.WriteString(externalRefName);
                return;
            }

            // Otherwise, this is the first time we're processing it. If it's not null, then assign it an ID
            ExternalReference externalRef = ExternalHandler.ProcessSavable(value);
            if (externalRef == null || externalRef == ExternalReference.NullReference)
            {
                return;
            }

            // Use the current count as the index
            externalRefName = $"{XmlConstants.BASE_EXTERNALREFERENCE_NAME}{_externalRefs.Count}";
            _externalRefs.Add(Tuple.Create(externalRefName, externalRef));
            _externalRefNameMap.Add(value, externalRefName);

            UnderlyingXmlWriter.WriteString(externalRefName);
        }

        /// <summary>
        /// Writes out all shared resources
        /// </summary>
        private void WriteSharedResources()
        {
            // Optional element, if no references then don't write out an empty element
            if (_sharedQueue.Count == 0)
            {
                return;
            }

            XmlWriter oldWriter = UnderlyingXmlWriter;
            int finalCount = 0;

            StringBuilder sharedXml = new StringBuilder();

            XmlWriterSettings settings = XmlSettings.Clone();
            settings.CloseOutput = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            // Create a xml writer to be used in isolation for the shared data
            using (XmlWriter sharedWriter = XmlWriter.Create(sharedXml, settings))
            {
                UnderlyingXmlWriter = sharedWriter;

                while (_sharedQueue.Count > 0)
                {
                    ISavable savable = _sharedQueue.Dequeue();
                    if (_sharedIndices.TryGetValue(savable, out Tuple<string, int> id))
                    {
                        finalCount++;

                        // Write out the shared savable like normal, but add the ID as an attribute
                        _cyclicDetector.AddReference(savable);

                        UnderlyingXmlWriter.WriteStartElement("Resource");
                        UnderlyingXmlWriter.WriteAttributeString("Id", id.Item1);
                        UnderlyingXmlWriter.WriteAttributeString("Type", SmartActivator.GetAssemblyQualifiedName(savable.GetType()));

                        savable.Write(this);

                        _cyclicDetector.RemoveReference(savable);

                        UnderlyingXmlWriter.WriteEndElement();
                    }
                }
            }

            // Revert back to the old writer, write out the shared element XML
            UnderlyingXmlWriter = oldWriter;

            UnderlyingXmlWriter.WriteStartElement("Resources");
            UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(finalCount));
            UnderlyingXmlWriter.WriteWhitespace("\n");
            UnderlyingXmlWriter.WriteRaw(sharedXml.ToString());
            UnderlyingXmlWriter.WriteWhitespace("\n");
            UnderlyingXmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes out all external references
        /// </summary>
        private void WriteExternalReferences()
        {
            // Optional element, if no references then don't write out an empty element
            if (_externalRefs.Count == 0)
            {
                return;
            }

            UnderlyingXmlWriter.WriteStartElement("ExternalReferences");

            int count = _externalRefs.Count;
            UnderlyingXmlWriter.WriteAttributeString("Count", XmlConvert.ToString(count));

            for (int i = 0; i < count; i++)
            {
                UnderlyingXmlWriter.WriteStartElement("ExternalReference");

                Tuple<string, ExternalReference> extRef = _externalRefs[i];

                UnderlyingXmlWriter.WriteAttributeString("Id", extRef.Item1);
                UnderlyingXmlWriter.WriteAttributeString("TargetType", SmartActivator.GetAssemblyQualifiedName(extRef.Item2.TargetType));

                UnderlyingXmlWriter.WriteString(extRef.Item2.ResourcePath);

                UnderlyingXmlWriter.WriteEndElement();
            }

            UnderlyingXmlWriter.WriteEndElement();

            _externalRefs.Clear();
            _externalRefNameMap.Clear();
        }

        /// <summary>
        /// Initializes the writer
        /// </summary>
        /// <param name="writeFlags">Writer flags</param>
        /// <param name="externalHandler">External reference handler</param>
        private void Init(SavableWriteFlags writeFlags, IExternalReferenceHandler externalHandler)
        {
            WriteFlags = writeFlags;
            ExternalHandler = externalHandler;
            _cyclicDetector = new CyclicReferenceDetector();

            _sharedQueue = new Queue<ISavable>();
            _sharedIndices = new Dictionary<ISavable, Tuple<string, int>>();
            _currSharedIndex = 0;
            _externalRefNameMap = new Dictionary<ISavable, string>();
            _externalRefs = new List<Tuple<string, ExternalReference>>();
            _closeDoc = false;

            // Determine external/shared write mode
            if (externalHandler == null)
            {
                _externalSharedMode = ExternalSharedMode.AllShared;
            }
            else if (writeFlags.HasFlag(SavableWriteFlags.SharedAsExternal))
            {
                _externalSharedMode = ExternalSharedMode.AllExternal;
            }
            else
            {
                _externalSharedMode = ExternalSharedMode.Default;
            }

            if (UnderlyingXmlWriter.Settings.ConformanceLevel != ConformanceLevel.Fragment)
            {
                _closeDoc = true;
                UnderlyingXmlWriter.WriteStartDocument();
            }

            UnderlyingXmlWriter.WriteStartElement("Content");
            UnderlyingXmlWriter.WriteAttributeString("Version", XmlConvert.ToString(XmlConstants.VERSION_NUMBER)); 
            UnderlyingXmlWriter.WriteAttributeString("ExternalSharedMode", _externalSharedMode.ToString());
        }
    }
}
