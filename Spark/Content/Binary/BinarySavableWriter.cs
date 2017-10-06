namespace Spark.Content.Binary
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    using Utilities;

    /// <summary>
    /// Savable writer that serializes content into the SPBO (Spark Binary Object) format.
    /// </summary>
    public sealed class BinarySavableWriter : BinaryPrimitiveWriter, ISavableWriter
    {
        private readonly CyclicReferenceDetector _cyclicDetector;
        private readonly ExternalSharedMode _externalSharedMode;

        private int _currSharedIndex;
        private readonly Queue<ISavable> _sharedQueue;
        private readonly Dictionary<ISavable, int> _sharedIndices;
        private readonly Stream _finalOutput;

        private readonly MemoryStream _primaryData;
        private MemoryStream _sharedData;
        private MemoryStream _compressedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableWriter"/> class. The output stream is not left open when the writer is disposed. All
        /// external references are automatically treated as shared and written to the output stream.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        public BinarySavableWriter(Stream output) 
            : this(output, SavableWriteFlags.None, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableWriter"/> class. All external references are automatically treated as 
        /// shared and written to the output stream.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public BinarySavableWriter(Stream output, bool leaveOpen) 
            : this(output, SavableWriteFlags.None, null, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableWriter"/> class. The output stream is not left open when the writer is disposed.
        /// All external references are automatically treated as shared and written to the output stream.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        public BinarySavableWriter(Stream output, SavableWriteFlags writeFlags) 
            : this(output, writeFlags, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableWriter"/> class. All external references are automatically treated as 
        /// shared and written to the output stream.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public BinarySavableWriter(Stream output, SavableWriteFlags writeFlags, bool leaveOpen) 
            : this(output, writeFlags, null, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableWriter"/> class. The output stream is not left open when the writer is disposed.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="externalHandler">External reference handler, if null then all external references are treated as shared and are written to the output stream.</param>
        public BinarySavableWriter(Stream output, IExternalReferenceHandler externalHandler)
            : this(output, SavableWriteFlags.None, externalHandler, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableWriter"/> class.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="externalHandler">External reference handler, if null then all external references are treated as shared and are written to the output stream.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public BinarySavableWriter(Stream output, IExternalReferenceHandler externalHandler, bool leaveOpen) 
            : this(output, SavableWriteFlags.None, externalHandler, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableWriter"/> class. The output stream is not left open when the writer is disposed.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="externalHandler">External reference handler, if null then all external references are treated as shared and are written to the output stream.</param>
        public BinarySavableWriter(Stream output, SavableWriteFlags writeFlags, IExternalReferenceHandler externalHandler) 
            : this(output, writeFlags, externalHandler, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySavableWriter"/> class.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="externalHandler">External reference handler, if null then all external references are treated as shared and are written to the output stream.</param>
        /// <param name="leaveOpen">True if the output stream should NOT be disposed/closed when the writer is disposed/closed, false otherwise.</param>
        public BinarySavableWriter(Stream output, SavableWriteFlags writeFlags, IExternalReferenceHandler externalHandler, bool leaveOpen)
            : base(output, leaveOpen)
        {
            if (output == null || !output.CanWrite)
            {
                throw new ArgumentNullException(nameof(output));
            }

            WriteFlags = writeFlags;
            ExternalHandler = externalHandler;
            _sharedQueue = new Queue<ISavable>();
            _sharedIndices = new Dictionary<ISavable, int>(new ReferenceEqualityComparer<ISavable>());
            _currSharedIndex = 0;
            _cyclicDetector = new CyclicReferenceDetector();

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
            
            // We set the out stream to the output stream which was validated, swap it out for the primary data stream while we write data
            _primaryData = new MemoryStream();
            _finalOutput = OutStream;
            OutStream = _primaryData;
        }

        /// <summary>
        /// Gets the external handler that controls how savables are to be written externally from the output. If no handler is set,
        /// then external savables are always handled as shared references.
        /// </summary>
        public IExternalReferenceHandler ExternalHandler { get; }

        /// <summary>
        /// Gets the write flags that specify certain behaviors during serialization.
        /// </summary>
        public SavableWriteFlags WriteFlags { get; }

        /// <summary>
        /// Clears all buffers and causes any buffered data to be written.
        /// </summary>
        public override void Flush()
        {
            ThrowIfDisposed();
            FlushInternal(true);
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

            if (value == null)
            {
                UnderlyingBinaryWriter.Write(BinaryConstants.NULL_OBJECT);
                return;
            }

            _cyclicDetector.AddReference(value);

            UnderlyingBinaryWriter.Write(BinaryConstants.A_OK);
            Write("Type", SmartActivator.GetAssemblyQualifiedName(value.GetType()));

            value.Write(this);

            _cyclicDetector.RemoveReference(value);
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

            if (values == null || values.Length == 0)
            {
                UnderlyingBinaryWriter.Write(BinaryConstants.NULL_OBJECT);
                return;
            }

            UnderlyingBinaryWriter.Write(BinaryConstants.A_OK);
            Write(null, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                WriteSavable("Item", values[i]);
            }
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

            if (_externalSharedMode == ExternalSharedMode.AllExternal)
            {
                WriteExternalSavableInternal(value);
            }
            else
            {
                WriteSharedSavableInternal(value);
            }
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

            if (values == null || values.Length == 0)
            {
                UnderlyingBinaryWriter.Write(BinaryConstants.NULL_OBJECT);
                return;
            }

            Write(null, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                WriteSharedSavable("Item", values[i]);
            }
        }

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

            if (_externalSharedMode == ExternalSharedMode.AllShared)
            {
                WriteSharedSavableInternal(value);
            }
            else
            {
                WriteExternalSavableInternal(value);
            }
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

            if (values == null || values.Length == 0)
            {
                UnderlyingBinaryWriter.Write(BinaryConstants.NULL_OBJECT);
                return;
            }

            Write(null, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                WriteExternalSavable("Item", values[i]);
            }
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
                FlushInternal(false);
                OutStream = _finalOutput; // Make sure we set outstream to final output
                UnderlyingBinaryWriter.Dispose();
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Writes a shared savable resource
        /// </summary>
        /// <typeparam name="T">Resource type</typeparam>
        /// <param name="value">Resource value to be written</param>
        private void WriteSharedSavableInternal<T>(T value) where T : ISavable
        {
            if (value == null)
            {
                UnderlyingBinaryWriter.Write(BinaryConstants.NULL_OBJECT);
                return;
            }

            int index;
            if (!_sharedIndices.TryGetValue(value, out index))
            {
                index = _currSharedIndex;
                _sharedQueue.Enqueue(value);
                _sharedIndices.Add(value, index);

                _currSharedIndex++;
            }

            UnderlyingBinaryWriter.Write(BinaryConstants.A_OK);
            Write("SharedIndex", index);
        }

        /// <summary>
        /// Writes an external savable resource
        /// </summary>
        /// <typeparam name="T">Resource type</typeparam>
        /// <param name="value">Resource value to be written</param>
        private void WriteExternalSavableInternal<T>(T value) where T : ISavable
        {
            ExternalReference externalRef = ExternalHandler.ProcessSavable(value);

            if (externalRef == null || externalRef == ExternalReference.NullReference)
            {
                UnderlyingBinaryWriter.Write(BinaryConstants.NULL_OBJECT);
                return;
            }

            UnderlyingBinaryWriter.Write(BinaryConstants.A_OK);
            Write("ResourcePath", externalRef.ResourcePath);
        }

        /// <summary>
        /// Performs a flush of all streams associated with this writer
        /// </summary>
        /// <param name="reset">True if the stream is to be reset, false otherwise</param>
        private void FlushInternal(bool reset)
        {
            // Process queued shared savables and prepare data
            ProcessSharedData();
            PrepareData();

            // Set outstream back to final output
            OutStream = _finalOutput;

            // Write header, and shared + primary data
            WriteHeader();
            WriteData();

            // Flush externals last, so any shared data having externals get written out
            if (ExternalHandler != null)
            {
                ExternalHandler.Flush();
            }

            // Flushes final output
            base.Flush();

            // If reset, set the primary data back to out stream - all other buffers are disposed and primary data
            // is set to length of zero, waiting for additional input
            if (reset)
            {
                OutStream = _primaryData;
            }
        }

        /// <summary>
        /// Writes all shared data out to the stream
        /// </summary>
        private void ProcessSharedData()
        {
            if (_sharedQueue.Count == 0)
            {
                return;
            }

            _sharedData = new MemoryStream();
            OutStream = _sharedData;

            // Write out the current shared count - after we're done, this will be updated again, for now its a place holder
            UnderlyingBinaryWriter.Write(_currSharedIndex);

            // Process queued shared resources, each shared resource may also write other shared resources
            while (_sharedQueue.Count > 0)
            {
                ISavable savable = _sharedQueue.Dequeue();

                UnderlyingBinaryWriter.Write(0L); // Size of shared savable
                long prevPos = OutStream.Position;

                WriteSavable("SharedObject", savable);

                // Compute and write out size of the shared savable (EXCLUDING number to hold the size)
                long newPos = OutStream.Position;
                long size = newPos - prevPos;

                OutStream.Position = prevPos - sizeof(long);
                UnderlyingBinaryWriter.Write(size);
                OutStream.Position = newPos;
            }

            // Write our curr shared index, which reflects the total count
            OutStream.Position = 0;
            UnderlyingBinaryWriter.Write(_currSharedIndex);
            OutStream.Position = 0;

            // Reset shared object tracking
            _currSharedIndex = 0;
            _sharedIndices.Clear();
        }

        /// <summary>
        /// Prepares the data to be written, compressing it if requested
        /// </summary>
        private void PrepareData()
        {
            if (!WriteFlags.HasFlag(SavableWriteFlags.UseCompression))
            {
                return;
            }

            _compressedData = new MemoryStream();
            using (GZipStream compressionStream = new GZipStream(_compressedData, CompressionMode.Compress, true))
            {
                OutStream = compressionStream;

                if (_sharedData != null)
                {
                    _sharedData.WriteTo(compressionStream);
                }

                if (_primaryData != null)
                {
                    _primaryData.WriteTo(compressionStream);
                }
            }

            _compressedData.Position = 0;
        }

        /// <summary>
        /// Write the binary file header
        /// </summary>
        private void WriteHeader()
        {
            UnderlyingBinaryWriter.Write(BinaryConstants.MAGIC_WORD.ToArray());
            UnderlyingBinaryWriter.Write(BinaryConstants.VERSION_NUMBER);
            UnderlyingBinaryWriter.Write((byte)_externalSharedMode);
            UnderlyingBinaryWriter.Write(WriteFlags.HasFlag(SavableWriteFlags.UseCompression));
            UnderlyingBinaryWriter.Write(GetSharedDataSize());
            UnderlyingBinaryWriter.Write(GetPrimaryDataSize());
            UnderlyingBinaryWriter.Write(GetCompressedDataSize());
        }

        /// <summary>
        /// Writes the data out to the stream
        /// </summary>
        private void WriteData()
        {
            if (WriteFlags.HasFlag(SavableWriteFlags.UseCompression))
            {
                WriteCompressedData();

                if (_sharedData != null)
                {
                    _sharedData.Dispose();
                    _sharedData = null;
                }

                if (_primaryData != null)
                {
                    _primaryData.SetLength(0);
                }

            }
            else
            {
                WriteSharedData();
                WritePrimaryData();
            }
        }

        /// <summary>
        /// Gets the size of the primary data stream in bytes
        /// </summary>
        /// <returns>Size of the primary data stream in bytes</returns>
        private long GetPrimaryDataSize()
        {
            if (_primaryData == null)
            {
                return 0L;
            }

            return _primaryData.Length;
        }

        /// <summary>
        /// Gets the size of the shared data stream in bytes
        /// </summary>
        /// <returns>Size of the shared data stream in bytes</returns>
        private long GetSharedDataSize()
        {
            if (_sharedData == null)
            {
                return 0L;
            }

            return _sharedData.Length;
        }

        /// <summary>
        /// Gets the size of the compressed data stream in bytes
        /// </summary>
        /// <returns>Size of the compressed data stream in bytes</returns>
        private long GetCompressedDataSize()
        {
            if (_compressedData == null)
            {
                return 0L;
            }

            return _compressedData.Length;
        }

        /// <summary>
        /// Writes compressed data out to the stream
        /// </summary>
        private void WriteCompressedData()
        {
            if (_compressedData == null)
            {
                return;
            }

            _compressedData.WriteTo(OutStream);
        }

        /// <summary>
        /// Writes the primary data out to the stream
        /// </summary>
        private void WritePrimaryData()
        {
            if (_primaryData == null)
            {
                return;
            }

            _primaryData.WriteTo(OutStream);
            _primaryData.SetLength(0);
        }

        /// <summary>
        /// Writes the shared data out to the stream
        /// </summary>
        private void WriteSharedData()
        {
            if (_sharedData == null)
            {
                return;
            }

            _sharedData.WriteTo(OutStream);
            _sharedData.Dispose();
            _sharedData = null;
        }
    }
}
