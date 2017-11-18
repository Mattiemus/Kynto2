namespace Spark.Content
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Utilities;

    /// <summary>
    /// Generic external handler for processing external references during savable serialization. The handler is responsible for creating a reference
    /// to a savable that needs to be saved externally. External writers are responsible for doing the actual write out. This particular implementation
    /// queues the savables as they are processed and are written out when the handler is flushed. If no writer is registered for a particular savable type,
    /// a default catch-all writer can be set to the handler.
    /// </summary>
    public class ExternalReferenceHandler : IExternalReferenceHandler
    {
        private readonly Dictionary<Type, IExternalReferenceWriter> _writers;
        private readonly Dictionary<Type, GetResourceNameDelegate> _namingDelegates;

        private readonly Dictionary<Type, int> _defaultNameIndices;
        private readonly Dictionary<string, int> _nameIndices;
        private readonly MultiKeyDictionary<Type, ISavable, ExternalReference> _externalReferences;
        private readonly ConcurrentQueue<Task> _fileIOTasks;
        private readonly Thread _threadCreatedOn;
        private bool _openedRepository;
        private readonly Dictionary<string, ISavable> _resourcePathToObject;

        private bool _inFlushing;

        private readonly IExternalReferenceWriter _defaultWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalReferenceHandler"/> class. By default, the external writers of this handler
        /// will overwrite existing resource files. This behavior can be modified by passing in custom write flags.
        /// </summary>
        /// <param name="parentResourceFile">The resource file that this handler will write external references relative to.</param>
        /// <param name="defaultWriter">Default external writer, serves as a fallback that can handle any savable type. It MUST target ISavable.</param>
        public ExternalReferenceHandler(IResourceFile parentResourceFile, IExternalReferenceWriter defaultWriter) 
            : this(parentResourceFile, SavableWriteFlags.OverwriteExistingResource, defaultWriter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalReferenceHandler"/> class.
        /// </summary>
        /// <param name="parentResourceFile">The resource file that this handler will write external references relative to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="defaultWriter">Default external writer, serves as a fallback that can handle any savable type. It MUST target ISavable.</param>
        public ExternalReferenceHandler(IResourceFile parentResourceFile, SavableWriteFlags writeFlags, IExternalReferenceWriter defaultWriter) 
            : this(parentResourceFile, writeFlags, defaultWriter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalReferenceHandler"/> class.
        /// </summary>
        /// <param name="parentResourceFile">The resource file that this handler will write external references relative to.</param>
        /// <param name="writeFlags">Write flags that specify certain behaviors.</param>
        /// <param name="defaultWriter">Default external writer, serves as a fallback that can handle any savable type. It MUST target ISavable.</param>
        public ExternalReferenceHandler(IResourceFile parentResourceFile, SavableWriteFlags writeFlags, IExternalReferenceWriter defaultWriter, IEqualityComparer<ISavable> comparer)
        {
            if (parentResourceFile == null || parentResourceFile.Repository == null || defaultWriter == null)
            {
                throw new ArgumentNullException(nameof(parentResourceFile));
            }

            if (!parentResourceFile.Repository.Exists)
            {
                throw new ArgumentException("Resource repository does not exist");
            }

            if (parentResourceFile.Repository.IsReadOnly)
            {
                throw new ArgumentException("Resource repository is read only");
            }

            ParentResourceFile = parentResourceFile;
            WriteFlags = writeFlags;

            _fileIOTasks = new ConcurrentQueue<Task>();
            _threadCreatedOn = Thread.CurrentThread;
            _writers = new Dictionary<Type, IExternalReferenceWriter>();
            _namingDelegates = new Dictionary<Type, GetResourceNameDelegate>();
            _externalReferences = new MultiKeyDictionary<Type, ISavable, ExternalReference>(null, new StandardContentComparer(comparer ?? new ReferenceEqualityComparer<ISavable>()));
            _resourcePathToObject = new Dictionary<string, ISavable>();
            _defaultNameIndices = new Dictionary<Type, int>();
            _nameIndices = new Dictionary<string, int>();
            _inFlushing = false;

            if (defaultWriter.TargetType != typeof(ISavable))
            {
                throw new ArgumentException("Writer cannot write ISavable instances", nameof(defaultWriter));
            }

            _defaultWriter = defaultWriter;
            RegisterWriter(_defaultWriter);
            OpenRepositoryIfNecessary();
        }

        /// <summary>
        /// Gets the resource file that this handler processes child resource files for. The resources
        /// that are created/updated are relative to this resource file.
        /// </summary>
        public IResourceFile ParentResourceFile { get; }

        /// <summary>
        /// Gets the write flags that specify certain behaviors during serialization.
        /// </summary>
        public SavableWriteFlags WriteFlags { get; }

        /// <summary>
        /// Registers an external writer that handles a specific savable type.
        /// </summary>
        /// <param name="writer">External savable writer</param>
        public void RegisterWriter(IExternalReferenceWriter writer)
        {
            if (writer == null || writer.TargetType == null)
            {
                return;
            }

            _writers[writer.TargetType] = writer;
        }

        /// <summary>
        /// Removes an external writer based on its target savable type it was registered to.
        /// </summary>
        /// <typeparam name="T">Target savable type</typeparam>
        /// <returns>True if the writer was removed, false otherwise</returns>
        public bool RemoveWriter<T>() where T : ISavable
        {
            Type type = typeof(T);
            bool success = _writers.Remove(type);

            // We can override the default writer, but if we remove it, always ensure we add the default.
            if (success && type == typeof(ISavable) && _defaultWriter != null)
            {
                _writers[type] = _defaultWriter;
            }

            return success;
        }

        /// <summary>
        /// Gets a registered external writer based on its target savable type it was registered to.
        /// </summary>
        /// <typeparam name="T">Target savable type</typeparam>
        /// <returns>The external savable writer, or null if a writer is not registered to the type.</returns>
        public IExternalReferenceWriter GetWriter<T>() where T : ISavable
        {
            if (_writers.TryGetValue(typeof(T), out IExternalReferenceWriter writer))
            {
                return writer;
            }

            return null;
        }

        /// <summary>
        /// Gets all external writers registered to this handler.
        /// </summary>
        /// <returns>Collection of external savable writers.</returns>
        public IEnumerable<IExternalReferenceWriter> GetWriters()
        {
            return _writers.Values.ToArray();
        }

        /// <summary>
        /// Adds a delegate that returns a unique resource name for a given savable, registered to a specific
        /// savable type. This allows for custom name transformations for output files.
        /// </summary>
        /// <typeparam name="T">Target savable type</typeparam>
        /// <param name="getResourceNameDelegate">Get resource name delegate</param>
        public void SetGetResourceNameDelegate<T>(GetResourceNameDelegate getResourceNameDelegate) where T : ISavable
        {
            Type type = typeof(T);
            if (getResourceNameDelegate == null)
            {
                _namingDelegates.Remove(type);
            }
            else
            {
                _namingDelegates[type] = getResourceNameDelegate;
            }
        }

        /// <summary>
        /// Processes a savable into an external reference.
        /// </summary>
        /// <typeparam name="T">Savable type</typeparam>
        /// <param name="value">Savable value</param>
        /// <returns>External reference to the savable</returns>
        public ExternalReference ProcessSavable<T>(T value) where T : ISavable
        {
            if (value == null)
            {
                return ExternalReference.NullReference;
            }

            ExternalReference externalRef;
            Type targetType = typeof(T);
            lock (_externalReferences)
            {
                //First see if we have a reference for this value already
                if (_externalReferences.TryGetValue(targetType, value, out externalRef))
                {
                    return externalRef;
                }

                // Create a reference to this guy
                string resourcePath = GetResourceName(value);
                if (string.IsNullOrEmpty(resourcePath))
                {
                    externalRef = ExternalReference.NullReference;
                }
                else
                {
                    externalRef = new ExternalReference(targetType, resourcePath);
                    if (!_resourcePathToObject.ContainsKey(externalRef.ResourcePath))
                    {
                        _resourcePathToObject.Add(externalRef.ResourcePath, value);
                    }
                }

                _externalReferences.Add(targetType, value, externalRef);
            }
            
            QueueAndRunTask(value, externalRef);

            return externalRef;
        }

        /// <summary>
        /// Checks if the path/resource name is unique of all the references that the handler has processed so far.
        /// </summary>
        /// <param name="resourcePath">Resource path (without extension)</param>
        /// <returns>True if the name is unique, false otherwise.</returns>
        public bool IsUniqueResourcePath(string resourcePath)
        {
            return !_resourcePathToObject.ContainsKey(resourcePath);
        }

        /// <summary>
        /// Flushes any remaining data that needs to be written.
        /// </summary>
        public void Flush()
        {
            if (_inFlushing)
            {
                return;
            }

            if (_threadCreatedOn != Thread.CurrentThread)
            {
                return;
            }

            _inFlushing = true;
            try
            {
                while (_fileIOTasks.TryDequeue(out Task task))
                {
                    if (task != null)
                    {
                        task.Wait();
                    }
                }
            }
            finally
            {
                if (_openedRepository)
                {
                    ParentResourceFile.Repository.CloseConnection();
                }

                _inFlushing = false;
            }
        }

        /// <summary>
        /// Clears cached external file references.
        /// </summary>
        public void Clear()
        {
            _externalReferences.Clear();
            _defaultNameIndices.Clear();
            _nameIndices.Clear();
            _resourcePathToObject.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenRepositoryIfNecessary()
        {
            IResourceRepository repo = ParentResourceFile.Repository;
            bool needToOpen = !repo.IsOpen;

            if (needToOpen)
            {
                repo.OpenConnection(ResourceFileMode.Create);
            }

            _openedRepository = needToOpen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="extReference"></param>
        private void QueueAndRunTask(ISavable obj, ExternalReference extReference)
        {
            bool overwriteExisting = WriteFlags.HasFlag(SavableWriteFlags.OverwriteExistingResource);

            IExternalReferenceWriter writer = FindSuitableWriter(extReference.TargetType);
            IResourceFile outputFile = ParentResourceFile.Repository.GetResourceFileRelativeTo(extReference.ResourcePath, ParentResourceFile);

            if (!overwriteExisting && outputFile.Exists)
            {
                return;
            }

            Task task = new Task(() => WriteExternalFile(outputFile, writer, obj));
            task.Start();

            _fileIOTasks.Enqueue(task);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputFile"></param>
        /// <param name="writer"></param>
        /// <param name="savable"></param>
        private void WriteExternalFile(IResourceFile outputFile, IExternalReferenceWriter writer, ISavable savable)
        {
            writer.WriteSavable(outputFile, this, savable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetResourceName<T>(T value) where T : ISavable
        {
            Type type = typeof(T);
            string resourceName;

            if (!_namingDelegates.TryGetValue(type, out GetResourceNameDelegate resourceNamer))
            {
                //Default naming
                if (value is INamable && !string.IsNullOrEmpty((value as INamable).Name))
                {
                    resourceName = Path.GetFileNameWithoutExtension((value as INamable).Name);

                    int index = 0;
                    if (!_nameIndices.TryGetValue(resourceName, out index))
                    {
                        index = 1;
                        _nameIndices.Add(resourceName, index);
                    }
                    else
                    {
                        _nameIndices[resourceName] = index + 1;
                        resourceName = string.Format("{0}-{1}", resourceName, index.ToString());
                    }
                }
                else
                {
                    if (!_defaultNameIndices.TryGetValue(type, out int index))
                    {
                        resourceName = type.Name;
                        resourceName = string.Format("{0}-{1}", ParentResourceFile.Name, type.Name);
                        _defaultNameIndices[type] = 1;
                    }
                    else
                    {
                        resourceName = string.Format("{0}-{1}-{2}", ParentResourceFile.Name, type.Name, index);
                        _defaultNameIndices[type] = index + 1;
                    }
                }
            }
            else
            {
                resourceName = resourceNamer(this, value);
            }

            IExternalReferenceWriter writer = FindSuitableWriter(type);
            return resourceName + writer.ResourceExtension;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IExternalReferenceWriter FindSuitableWriter(Type type)
        {
            if (!_writers.TryGetValue(type, out IExternalReferenceWriter writer))
            {
                writer = _writers[typeof(ISavable)];
            }
            
            return writer;
        }

        /// <summary>
        /// 
        /// </summary>
        private class StandardContentComparer : IEqualityComparer<ISavable>
        {
            private readonly IEqualityComparer<ISavable> _comparer;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="comparer"></param>
            public StandardContentComparer(IEqualityComparer<ISavable> comparer)
            {
                _comparer = comparer;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(ISavable x, ISavable y)
            {
                return _comparer.Equals(x, y);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(ISavable obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                
                return obj.GetHashCode();
            }
        }
    }
}
