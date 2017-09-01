namespace Spark.Content.Binary
{
    using System;
    using System.Linq;
    using System.IO;
    using System.IO.Compression;

    using Utilities;

    /// <summary>
    /// Savable reader that de-serializes content from the TEBO (Tesla Engine Binary Object) format.
    /// </summary>
    public sealed class BinarySavableReader : BinaryPrimitiveReader, ISavableReader
    {
        private BinaryReader _cachedReader;
        private SharedSavableEntry[] _sharedData;
        private ExternalSharedMode _externalSharedMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableReader"/> class. The input stream is not left open when the reader is disposed.
        /// External references will not be resolved.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="inputStream">Input stream to read savable data from.</param>
        public BinarySavableReader(IServiceProvider serviceProvider, Stream inputStream)
            : this(serviceProvider, inputStream, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableReader"/> class. External references will not be resolved.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="inputStream">Input stream to read savable data from.</param>
        /// <param name="leaveOpen">True if the input stream should NOT be disposed/closed when the reader is disposed/closed, false otherwise.</param>
        public BinarySavableReader(IServiceProvider serviceProvider, Stream inputStream, bool leaveOpen)
            : this(serviceProvider, inputStream, null, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableReader"/> class. The input stream is not left open when the reader is disposed.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="inputStream">Input stream to read savable data from.</param>
        /// <param name="externalResolver">An optional external resolver used to load external references. If not present, then external savables cannot be resolved.</param>
        public BinarySavableReader(IServiceProvider serviceProvider, Stream inputStream, IExternalReferenceResolver externalResolver)
            : this(serviceProvider, inputStream, externalResolver, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to locate services that may be required during content loading.</param>
        /// <param name="inputStream">Input stream to read savable data from.</param>
        /// <param name="externalResolver">An optional external resolver used to load external references. If not present, then external savables cannot be resolved.</param>
        /// <param name="leaveOpen">True if the input stream should NOT be disposed/closed when the reader is disposed/closed, false otherwise.</param>
        public BinarySavableReader(IServiceProvider serviceProvider, Stream inputStream, IExternalReferenceResolver externalResolver, bool leaveOpen)
            : base(inputStream, leaveOpen)
        {
            ServiceProvider = serviceProvider;
            if (ServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (inputStream == null || !inputStream.CanRead)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            ExternalResolver = externalResolver;
            PrepareStream();
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

            if (UnderlyingBinaryReader.ReadSByte() == BinaryConstants.NULL_OBJECT)
            {
                return default(T);
            }

            string assemblyQualfiedName = ReadString();
            Type type = SmartActivator.GetType(assemblyQualfiedName);

            object obj = SmartActivator.CreateInstance(type);

            if (obj == null)
            {
                return default(T);
            }

            if (!(obj is T))
            {
                throw new InvalidCastException($"Could not match type {obj.GetType().FullName} with type {typeof(T).FullName}");
            }

            T savable = (T)obj;
            savable.Read(this);

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

            if (UnderlyingBinaryReader.ReadSByte() == BinaryConstants.NULL_OBJECT)
            {
                return null;
            }

            int count = UnderlyingBinaryReader.ReadInt32();
            T[] values = new T[count];

            for (int i = 0; i < count; i++)
            {
                values[i] = ReadSavable<T>();
            }

            return values;
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

            if (UnderlyingBinaryReader.ReadSByte() == BinaryConstants.NULL_OBJECT)
            {
                return null;
            }

            int count = UnderlyingBinaryReader.ReadInt32();
            T[] values = new T[count];

            for (int i = 0; i < count; i++)
            {
                values[i] = ReadSharedSavable<T>();
            }

            return values;
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

            if (UnderlyingBinaryReader.ReadSByte() == BinaryConstants.NULL_OBJECT)
            {
                return null;
            }

            int count = UnderlyingBinaryReader.ReadInt32();
            T[] values = new T[count];

            for (int i = 0; i < count; i++)
            {
                values[i] = ReadExternalSavable<T>();
            }

            return values;
        }

        /// <summary>
        /// Reads a shared savable from the input
        /// </summary>
        /// <typeparam name="T">Type of object to be read</typeparam>
        /// <returns>Shared savable read from the stream</returns>
        private T ReadSharedSavableInternal<T>() where T : ISavable
        {
            if (UnderlyingBinaryReader.ReadSByte() == BinaryConstants.NULL_OBJECT)
            {
                return default(T);
            }

            int sharedIndex = ReadInt32();
            if (_sharedData != null && sharedIndex >= 0 && sharedIndex < _sharedData.Length)
            {
                // If savable is in entry, return it. If not, halt what we're doing, position us in the stream, read it, and position us back
                SharedSavableEntry entry = _sharedData[sharedIndex];
                if (entry.Seeked)
                {
                    return (T)entry.Savable;
                }
                else
                {
                    SeekToSharedSavable<T>(ref entry);
                    _sharedData[sharedIndex] = entry; // Make sure we save back the savable

                    return (T)entry.Savable;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Reads a shared object from the shared objects block
        /// </summary>
        /// <typeparam name="T">Type of object to be read</typeparam>
        /// <returns>Object read from the shared savable block</returns>
        private void SeekToSharedSavable<T>(ref SharedSavableEntry entry) where T : ISavable
        {
            long currPos = InStream.Position;

            InStream.Position = entry.StartPosition;

            entry.Savable = ReadSavable<T>();
            entry.Seeked = true;

            InStream.Position = currPos;
        }

        /// <summary>
        /// Reads a shared object as an external piece of content
        /// </summary>
        /// <typeparam name="T">Type of object to be read</typeparam>
        /// <returns>Object read from an external file</returns>
        private T ReadExternalSavableInternal<T>() where T : ISavable
        {
            if (UnderlyingBinaryReader.ReadSByte() == BinaryConstants.NULL_OBJECT)
            {
                return default(T);
            }

            string resourcePath = ReadString();

            // Read the reference, but if we don't have a resolver we can't load it so abort
            if (ExternalResolver == null)
            {
                return default(T);
            }

            ExternalReference externalRef = new ExternalReference(typeof(T), resourcePath);
            return ExternalResolver.ResolveSavable<T>(externalRef);
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
                // If we read compressed data, switch the cached binary reader back and dispose what we were using
                if (_cachedReader != null)
                {
                    UnderlyingBinaryReader.Dispose();
                    UnderlyingBinaryReader = _cachedReader;
                    _cachedReader = null;
                }

                UnderlyingBinaryReader.Dispose();
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Prepares the input stream to be read
        /// </summary>
        private void PrepareStream()
        {
            // Read header
            byte[] magicWord = UnderlyingBinaryReader.ReadBytes(4);
            short version = UnderlyingBinaryReader.ReadInt16();

            ThrowIfFormatInvalid(magicWord, version);

            _externalSharedMode = (ExternalSharedMode)UnderlyingBinaryReader.ReadByte();

            bool compressed = UnderlyingBinaryReader.ReadBoolean();
            long sharedDataSize = UnderlyingBinaryReader.ReadInt64();
            UnderlyingBinaryReader.ReadInt64();
            long totalCompressedDataSize = UnderlyingBinaryReader.ReadInt64();

            if (compressed)
            {
                _cachedReader = UnderlyingBinaryReader;
                UnderlyingBinaryReader = new BinaryReader(DecompressStream(totalCompressedDataSize, _cachedReader.BaseStream));
            }

            // Read shared data out first, now we're ready to read primary asset data
            if (sharedDataSize > 0)
            {
                ReadSharedData();
            }
        }

        /// <summary>
        /// Creates a stream that decompresses the input stream
        /// </summary>
        /// <param name="compressedTotalSize">Total size of the compressed stream</param>
        /// <param name="inputStream">Input stream to be decompressed</param>
        /// <returns>Stream which reads the input as a decompressed stream</returns>
        private Stream DecompressStream(long compressedTotalSize, Stream inputStream)
        {
            byte[] buffer = new byte[compressedTotalSize];
            inputStream.Read(buffer, 0, buffer.Length);

            MemoryStream decompressedStream = new MemoryStream();

            using (GZipStream compressedStream = new GZipStream(new MemoryStream(buffer), CompressionMode.Decompress))
            {
                compressedStream.CopyTo(decompressedStream);
            }

            decompressedStream.Position = 0;
            return decompressedStream;
        }

        /// <summary>
        /// Reads the shared data section from the input stream
        /// </summary>
        private void ReadSharedData()
        {
            int count = UnderlyingBinaryReader.ReadInt32();
            _sharedData = new SharedSavableEntry[count];

            // Shared savables, especially ones that have nested shared savables, can be a bit tricky. We defer the loading until we actually need them,
            // we only want to know the position in the stream where they start at and the size of the savable. When we look up the savable later, we'll 
            // redirect to that position, read the savable, then reset to the previous position in the stream.
            for (int i = 0; i < count; i++)
            {
                long sizeInBytes = UnderlyingBinaryReader.ReadInt64();
                long startPos = InStream.Position;

                InStream.Position += sizeInBytes;

                _sharedData[i] = new SharedSavableEntry(sizeInBytes, startPos);
            }
        }

        /// <summary>
        /// Throws an exception if the format information is incorrect
        /// </summary>
        /// <param name="magicWord">Magic word at the start of the binary file</param>
        /// <param name="version">Binary file version number</param>
        private void ThrowIfFormatInvalid(byte[] magicWord, short version)
        {
            if (!magicWord.SequenceEqual(BinaryConstants.MAGIC_WORD))
            {
                throw new SparkContentException("Bad binary format magic header");
            }

            if (version != BinaryConstants.VERSION_NUMBER)
            {
                throw new SparkContentException("Bad binary format version number");
            }
        }

        /// <summary>
        /// Representation of a shared savable entry within the shared objects region of the binary blob
        /// </summary>
        private struct SharedSavableEntry
        {
            public long SizeInBytes;
            public long StartPosition;
            public ISavable Savable;
            public bool Seeked;

            /// <summary>
            /// Initializes a new instance of the <see cref="SharedSavableEntry"/> structure.
            /// </summary>
            /// <param name="sizeInBytes">Size of the entry in bytes</param>
            /// <param name="startPos">Start position of the entry</param>
            public SharedSavableEntry(long sizeInBytes, long startPos)
            {
                SizeInBytes = sizeInBytes;
                StartPosition = startPos;
                Savable = null;
                Seeked = false;
            }
        }
    }
}
