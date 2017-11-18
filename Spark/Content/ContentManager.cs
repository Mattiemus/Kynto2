namespace Spark.Content
{
    using System;
    using System.Collections.Generic;
    
    using Utilities;
    using Xml;
    using Binary;

    /// <summary>
    /// A manager that handles loading and caching of resources. Each manager has a primary resource repository that it loads
    /// resources from, which may or may not be owned by the content manager (allowing for multiple managers to utilize the same
    /// repository, that is externally managed). Additional repositories can be added that act as secondary search paths for when a resource
    /// cannot be resolved. Each content manager has a collection of resource importers that are keyed to a format extension and runtime target type
    /// that do the actual import and processing. Optionally, a content manager can be configured to throw exceptions for content that cannot be loaded
    /// or is missing, or be configured with place holder handlers that return non-cached default content.
    /// </summary>
    public sealed class ContentManager : BaseDisposable
    {
        private IResourceRepository _repository;
        private readonly Dictionary<string, object> _loadedResources;
        private readonly Dictionary<string, object> _resourceLockers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentManager"/> class. By default, the repository is a standard file repository
        /// whose root path is the application's root directory.
        /// </summary>
        public ContentManager() 
            : this(null, new FileResourceRepository(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentManager"/> class.
        /// </summary>
        /// <param name="repository">The resource repository that the content manager that loads resources from and also manages.</param>
        public ContentManager(IResourceRepository repository) 
            : this(null, repository, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentManager"/> class. By default, the repository is a standard file repository
        /// whose root path is the application's root directory.
        /// </summary>
        /// <param name="serviceProvider">Service provider the content manager should use to locate services during content loading. If null, then the 
        /// engine service register is used.</param>
        public ContentManager(IServiceProvider serviceProvider) 
            : this(serviceProvider, new FileResourceRepository(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider the content manager should use to locate services during content loading. If null, then the 
        /// engine service register is used.</param>
        /// <param name="repository">The resource repository that the content manager that loads resources from and also manages.</param>
        public ContentManager(IServiceProvider serviceProvider, IResourceRepository repository) 
            : this(serviceProvider, repository, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider the content manager should use to locate services during content loading. If null, then the 
        /// engine service register is used.</param>
        /// <param name="repository">The resource repository that the content manager that loads resources from</param>
        /// <param name="manageRepository">True if the repository should be managed, that is closed when the manager is disposed.</param>
        public ContentManager(IServiceProvider serviceProvider, IResourceRepository repository, bool manageRepository)
        {
            if (serviceProvider == null)
            {
                if (!Engine.IsInitialized)
                {
                    throw new SparkContentException("Engine is not initialized");
                }

                ServiceProvider = Engine.Instance.Services;
            }
            else
            {
                ServiceProvider = serviceProvider;
            }

            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (!repository.Exists)
            {
                throw new ArgumentNullException(nameof(repository), "Resource repository does not exist");
            }

            ResourceRepository = repository;
            ManageRepository = manageRepository;

            if (!ResourceRepository.IsOpen)
            {
                ResourceRepository.OpenConnection(ResourceFileMode.Open, ResourceFileShare.Read);
            }

            ThrowForMissingContent = false;

            ResourceImporters = new ResourceImporterCollection
            {
                new BinaryResourceImporter(),
                new XmlResourceImporter()
            };
            
            MissingContentHandlers = new MissingContentHandlerCollection();
            
            _loadedResources = new Dictionary<string, object>();
            _resourceLockers = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets if the content manager should throw a content exception if a resource cannot be found or loaded.
        /// </summary>
        public bool ThrowForMissingContent { get; set; }

        /// <summary>
        /// Gets or sets if the resource repository should be managed by the content manager, this is when the manager is disposed, should
        /// the connection to the repository also be closed.
        /// </summary>
        public bool ManageRepository { get; set; }

        /// <summary>
        /// Gets or sets the resource repository that the content manager resolves resources from.
        /// </summary>
        public IResourceRepository ResourceRepository
        {
            get => _repository;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _repository = value;
            }
        }

        /// <summary>
        /// Gets the collection of missing content handlers. These handlers are keyed to a specific resource type (e.g. Texture2D), if a handler
        /// is not present then the default value is used (e.g. null for classes).
        /// </summary>
        /// <remarks>Adding/removing content handlers is not thread-safe.</remarks>
        public MissingContentHandlerCollection MissingContentHandlers { get; }

        /// <summary>
        /// Gets the collection of resource importers. These importers are keyed to a specific format extension and a target type. Adding/removing
        /// importers is not thread safe.
        /// </summary>
        /// <remarks>Adding/removing importers is not thread-safe.</remarks>
        public ResourceImporterCollection ResourceImporters { get; }

        /// <summary>
        /// Gets the service provider, used to locate services that may be required during content loading.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }
                
        /// <summary>
        /// Loads and processes a resource into its runtime type.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <typeparam name="T">Content type to load</typeparam>
        /// <param name="resourceName">Full resource name (with its extension)</param>
        /// <returns>The loaded resource</returns>
        public T Load<T>(string resourceName) where T : class
        {
            ThrowIfDisposed();
            return PrepareAndLoadInternal<T>(resourceName, null, false, null);
        }

        /// <summary>
        /// Loads and processes a resource into its runtime type.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <typeparam name="T">Content type to load</typeparam>
        /// <param name="resourceName">Full resource name (with its extension)</param>
        /// <param name="parameters">Optional importer parameters for the resource importer</param>
        /// <returns>The loaded resource</returns>
        public T Load<T>(string resourceName, ImporterParameters parameters) where T : class
        {
            ThrowIfDisposed();
            return PrepareAndLoadInternal<T>(resourceName, null, false, parameters);
        }

        /// <summary>
        /// Loads and processes a resource into its runtime type.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <typeparam name="T">Content type to load</typeparam>
        /// <param name="resourceName">Relative resource name (with its extension)</param>
        /// <param name="resourceFile">The resource file that the resource is relative to.</param>
        /// <returns>The loaded resource</returns>
        public T LoadRelativeTo<T>(string resourceName, IResourceFile resourceFile) where T : class
        {
            ThrowIfDisposed();
            return PrepareAndLoadInternal<T>(resourceName, resourceFile, false, null);
        }

        /// <summary>
        /// Loads and processes a resource into its runtime type.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <typeparam name="T">Content type to load</typeparam>
        /// <param name="resourceName">Relative resource name (with its extension)</param>
        /// <param name="resourceFile">The resource file that the resource is relative to.</param>
        /// <param name="parameters">Optional importer parameters for the resource importer</param>
        /// <returns>The loaded resource</returns>
        public T LoadRelativeTo<T>(string resourceName, IResourceFile resourceFile, ImporterParameters parameters) where T : class
        {
            ThrowIfDisposed();
            return PrepareAndLoadInternal<T>(resourceName, resourceFile, false, parameters);
        }

        /// <summary>
        /// Unloads all cached loaded resources contained in the content manager.
        /// </summary>
        /// <remarks>This method is not thread-safe.</remarks>
        /// <param name="dispose">Optionally dispose of the resources if they are <see cref="IDisposable"/>.</param>
        public void Unload(bool dispose = true)
        {
            ThrowIfDisposed();

            lock (_resourceLockers)
            {
                lock (_loadedResources)
                {
                    try
                    {
                        foreach (KeyValuePair<string, object> kv in _loadedResources)
                        {
                            object obj = kv.Value;
                            IDisposable disposable = kv.Value as IDisposable;
                            if (dispose && disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        _loadedResources.Clear();
                        _resourceLockers.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Unloads the cached loaded resource from the content manager
        /// </summary>
        /// <param name="obj">Object to remove from the content manager.</param>
        /// <param name="dispose">Optionally dispose of the resource if they are <see cref="IDisposable"/>.</param>
        /// <returns>True if the object was found to be contained in the manager, false if otherwise.</returns>
        public bool Unload(object obj, bool dispose = true)
        {
            if (obj == null)
            {
                return false;
            }

            ThrowIfDisposed();

            lock (_resourceLockers)
            {
                lock (_loadedResources)
                {
                    string path = null;
                    foreach (KeyValuePair<string, object> kv in _loadedResources)
                    {
                        if (obj == kv.Value)
                        {
                            path = kv.Key;
                            break;
                        }
                    }

                    if (path != null)
                    {
                        // If in loaded resources, then we had a resource locker to it, so remove that as well
                        _resourceLockers.Remove(path);
                        _loadedResources.Remove(path);

                        IDisposable disposable = obj as IDisposable;
                        if (dispose && disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    return path != null;
                }
            }
        }

        /// <summary>
        /// Selectively unloads a cached loaded resource contained in the content manager.
        /// </summary>
        /// <param name="resourceName">Fully qualified resource name (including subresource name, optionally).</param>
        /// <param name="dispose">Optionally dispose of the resource if they are <see cref="IDisposable"/>.</param>
        /// <returns>True if the resource was removed and possibly disposed of, false otherwise.</returns>
        public bool Unload(string resourceName, bool dispose = true)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                return false;
            }

            ThrowIfDisposed();

            lock (_resourceLockers)
            {
                lock (_loadedResources)
                {
                    if (_loadedResources.TryGetValue(resourceName, out object obj))
                    {
                        IDisposable disposable = obj as IDisposable;
                        if (dispose && disposable != null)
                        {
                            disposable.Dispose();
                        }

                        // If in loaded resources, then we had a resource locker to it, so remove that as well
                        _resourceLockers.Remove(resourceName);
                        _loadedResources.Remove(resourceName);

                        return true;
                    }

                    return false;
                }
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
                try
                {
                    Unload();

                    if (ManageRepository)
                    {
                        ResourceRepository.CloseConnection();
                    }
                }
                finally
                {
                    ResourceImporters.Clear();
                    MissingContentHandlers.Clear();
                    ResourceRepository = null;
                }
            }
                        
            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Resolves a resource file from its name
        /// </summary>
        /// <param name="resourceName">Resource name to resolve</param>
        /// <returns>Resolved resource file</returns>
        private IResourceFile ResolveResourceFile(string resourceName)
        {
            return ResourceRepository.GetResourceFile(resourceName);
        }

        /// <summary>
        /// Resolves a resource file from its name relative to another resource file
        /// </summary>
        /// <param name="resourceName">Resource name to resolve</param>
        /// <param name="relativeTo">Resource to resolve relative to</param>
        /// <returns>Resolved resource file</returns>
        private IResourceFile ResolveResourceFile(string resourceName, IResourceFile relativeTo)
        {
            return ResourceRepository.GetResourceFileRelativeTo(resourceName, relativeTo);
        }

        /// <summary>
        /// Prepares a resource to be loaded
        /// </summary>
        /// <param name="rawResourceName">Name of the resource as requested by the user</param>
        /// <param name="optionalRelativeResource">Optional resource file to load relative to</param>
        /// <param name="fallbackRelativeLoading">True if relative loading can be used as a fall back</param>
        /// <param name="parameters">Loader parameters</param>
        /// <param name="resourceNameWithSubresourceName">Parsed resource name with its subresource</param>
        /// <param name="foundFile">True if the file was found, false otherwise</param>
        /// <returns>Value indicating whether the resource was found</returns>
        private bool PrepareResourceLoading(string rawResourceName, IResourceFile optionalRelativeResource, bool fallbackRelativeLoading, ref ImporterParameters parameters, out string resourceNameWithSubresourceName, out IResourceFile foundFile)
        {
            resourceNameWithSubresourceName = null;
            foundFile = null;

            if (string.IsNullOrEmpty(rawResourceName))
            {
                return false;
            }

            // The final identifier is the fully qualified resource name + subresource, the path used to actually resolve the resource file is the cleaned
            // name. The subresource name will exist on the parameters for the importer to use.
            MassageSubresourceNaming(rawResourceName, ref parameters, out resourceNameWithSubresourceName, out string resourceNameClean);

            if (optionalRelativeResource != null && optionalRelativeResource.Exists)
            {
                // If loading relative to file resource, then try and load it. If we can fallback, try and load as an absolute path
                foundFile = ResolveResourceFile(resourceNameClean, optionalRelativeResource);
                if ((foundFile == null || !foundFile.Exists) && fallbackRelativeLoading)
                {
                    foundFile = ResolveResourceFile(resourceNameClean);
                }
            }
            else
            {
                // Otherwise load as absolute path
                foundFile = ResolveResourceFile(resourceNameClean);
            }

            // If the file exists, then it was successfully resolved
            return foundFile != null && foundFile.Exists;
        }

        /// <summary>
        /// Prepares for loading, then loads a given resource
        /// </summary>
        /// <typeparam name="T">Type of content to load</typeparam>
        /// <param name="rawResourceName">Name of the resource to load</param>
        /// <param name="optionalRelativeResource">Optional resource file to load relative to</param>
        /// <param name="fallbackRelativeLoading">True if relative loading can be used as a fall back</param>
        /// <param name="parameters">Loader parameters</param>
        /// <returns>Loaded resource</returns>
        private T PrepareAndLoadInternal<T>(string rawResourceName, IResourceFile optionalRelativeResource, bool fallbackRelativeLoading, ImporterParameters parameters) where T : class
        {
            if (string.IsNullOrEmpty(rawResourceName))
            {
                return GetPlaceHolderOrThrow<T>("NULLRESOURCE");
            }

            bool successfullyResolved = PrepareResourceLoading(rawResourceName, optionalRelativeResource, fallbackRelativeLoading, ref parameters, out string resourceNameWithSubresourceName, out IResourceFile foundFile);
            if (!successfullyResolved)
            {
                return GetPlaceHolderOrThrow<T>(rawResourceName);
            }

            return LoadInternal<T>(resourceNameWithSubresourceName, foundFile, parameters);
        }

        /// <summary>
        /// Performs the loading of a resource file
        /// </summary>
        /// <typeparam name="T">Type of content to load</typeparam>
        /// <param name="resourceNameWithSubresourceName">Resource name with its sub resource</param>
        /// <param name="fileToLoad">Resource file to be loaded</param>
        /// <param name="parameters">Loader parameters</param>
        /// <returns>Loaded resource</returns>
        private T LoadInternal<T>(string resourceNameWithSubresourceName, IResourceFile fileToLoad, ImporterParameters parameters) where T : class
        {
            // Resolved resource file and final name in a Prepare call before this, so if we hit this everything should be legal
            Type type = typeof(T);

            // Find an importer for the resource
            IResourceImporter importer = ResourceImporters.FindSuitableImporter(fileToLoad.Extension, type);
            if (importer == null)
            {
                return GetPlaceHolderOrThrow<T>(resourceNameWithSubresourceName);
            }

            if (!importer.CanLoadType(type))
            {
                throw new InvalidCastException($"Could not match type {importer.TargetType.FullName} with type {typeof(T).FullName}");
            }

            // Lock on the fully qualified (full path + optional subresource name)
            object obj;
            lock (GetResourceLocker(resourceNameWithSubresourceName))
            {
                // Lock on the loaded resources dictionary to see if we already have it loaded
                lock (_loadedResources)
                {
                    if (_loadedResources.TryGetValue(resourceNameWithSubresourceName, out obj))
                    {
                        if (obj is IContentCastable)
                        {
                            obj = (obj as IContentCastable).CastTo(type, parameters.SubresourceName);
                        }

                        return obj as T;
                    }
                }

                // If we don't have it cached, try and load it
                try
                {
                    obj = importer.Load(fileToLoad, this, parameters);
                }
                catch (Exception e)
                {
                    throw CreateContentLoadException(resourceNameWithSubresourceName, e);
                }

                // If we couldn't load it but no exception, either return placeholder content or throw
                if (obj == null)
                {
                    return GetPlaceHolderOrThrow<T>(resourceNameWithSubresourceName);
                }

                // Lock again to add the loaded resource to the cache
                lock (_loadedResources)
                {
                    _loadedResources.Add(resourceNameWithSubresourceName, obj);
                }

                if (obj is INamable)
                {
                    (obj as INamable).Name = fileToLoad.Name;
                }

                if (obj is IContentCastable)
                {
                    obj = (obj as IContentCastable).CastTo(type, parameters.SubresourceName);
                }

                return obj as T;
            }
        }

        /// <summary>
        /// Pulls apart a resource file path
        /// </summary>
        /// <param name="rawFilePath">Raw resource file path to parse</param>
        /// <param name="importParams">Importer parameters</param>
        /// <param name="filePathWithSubresourceName">Full file path with sub resource name</param>
        /// <param name="filePathClean">Name of the file without the sub resource name</param>
        /// <remarks>
        /// We support either [filePath].[extesion]::[subresourcename] syntax OR by setting the subresourcename in the importer parameters
        /// </remarks>
        private void MassageSubresourceNaming(string rawFilePath, ref ImporterParameters importParams, out string filePathWithSubresourceName, out string filePathClean)
        {
            ContentHelper.ParseSubresourceName(rawFilePath, out string filePath, out string subresourceName);
            filePathClean = filePath;

            // No subresource name in the raw file path... check import params
            if (string.IsNullOrEmpty(subresourceName))
            {
                if (importParams == null)
                {
                    // No parameters so use the non-pooled default None and use the filepath normally
                    importParams = new ImporterParameters();
                    filePathWithSubresourceName = filePathClean;
                }
                else
                {
                    // Import params may have it
                    if (!string.IsNullOrEmpty(importParams.SubresourceName))
                    {
                        filePathWithSubresourceName = string.Concat(filePath, "::", importParams.SubresourceName);
                    }
                    else
                    {
                        // Else no subresource name so use the filepath normally
                        filePathWithSubresourceName = filePath;
                    }
                }
            }
            else
            {
                // Got a subresource name, set it on the import params
                if (importParams == null)
                {
                    importParams = new ImporterParameters()
                    {
                        SubresourceName = subresourceName
                    };

                    filePathWithSubresourceName = rawFilePath;
                }
                else
                {
                    // Importer parameters exist and may have a name, but since the subresource name in the file path is not null that takes precedence
                    filePathWithSubresourceName = rawFilePath;
                    importParams.SubresourceName = subresourceName;
                }
            }
        }

        /// <summary>
        /// Gets the placeholder content for the given resource
        /// </summary>
        /// <typeparam name="T">Type of content that should be loaded</typeparam>
        /// <param name="resourceName">Name of the resource that was missing</param>
        /// <returns>Place holder content for the given resource</returns>
        private T GetPlaceHolderOrThrow<T>(string resourceName) where T : class
        {
            if (ThrowForMissingContent)
            {
                throw CreateContentLoadException(resourceName, null);
            }

            IMissingContentHandler handler = MissingContentHandlers[typeof(T)];
            if (handler == null)
            {
                return default(T);
            }

            return handler.GetPlaceHolderContent<T>();
        }

        /// <summary>
        /// Creates a content load exception for a given resource
        /// </summary>
        /// <param name="resourceName">Name of the resource that caused the exception</param>
        /// <param name="inner">Optional inner exception</param>
        /// <returns>Content exception</returns>
        private SparkContentException CreateContentLoadException(string resourceName, Exception inner)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                resourceName = "<null>";
            }

            if (inner == null)
            {
                return new SparkContentException($"Could not load content: {resourceName}");
            }
            else
            {
                return new SparkContentException($"Could not load content: {resourceName}", inner);
            }
        }

        /// <summary>
        /// Gets the lock object to use against a given resource name
        /// </summary>
        /// <param name="resourceName">Name of the resource to get the locker object for</param>
        /// <returns>Resource specific locker object instance</returns>
        private object GetResourceLocker(string resourceName)
        {
            object obj;
            lock (_resourceLockers)
            {
                if (!_resourceLockers.TryGetValue(resourceName, out obj))
                {
                    obj = new object();
                    _resourceLockers.Add(resourceName, obj);
                }
            }

            return obj;
        }
    }
}
