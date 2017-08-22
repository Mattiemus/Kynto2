namespace Spark.Content
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// A resource file that can access files on a disk.
    /// </summary>
    public sealed class FileResourceFile : IResourceFile
    {
        private readonly FileInfo _fileInfo;
        private readonly IResourceRepository _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileResourceFile"/> class.
        /// </summary>
        /// <param name="fullPathToFile">The full path to file.</param>
        /// <param name="parent">Parent repository</param>
        internal FileResourceFile(string fullPathToFile, IResourceRepository parent)
        {
            _fileInfo = new FileInfo(fullPathToFile);
            _parent = parent;
        }

        /// <summary>
        /// Gets the name of the resource (without format extension).
        /// </summary>
        public string Name => Path.GetFileNameWithoutExtension(_fileInfo.Name);

        /// <summary>
        /// Gets the name of the resource (with format extension).
        /// </summary>
        public string NameWithExtension => _fileInfo.Name;

        /// <summary>
        /// Gets the resource's format extension.
        /// </summary>
        public string Extension => _fileInfo.Extension;

        /// <summary>
        /// Gets the full qualified name of the resource - the path to the resource, its name, and its extension.
        /// </summary>
        public string FullName => _fileInfo.FullName;

        /// <summary>
        /// Gets if the resource file is read only (if it exists).
        /// </summary>
        public bool IsReadOnly => _fileInfo.IsReadOnly;

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the resource was initially created.
        /// </summary>
        public DateTime CreationTime => _fileInfo.CreationTimeUtc;

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the resource was last accessed.
        /// </summary>
        public DateTime LastAccessTime => _fileInfo.LastAccessTimeUtc;

        /// <summary>
        /// Gets if the resource does in fact exist in the repository.
        /// </summary>
        public bool Exists => _fileInfo.Exists;

        /// <summary>
        /// Gets the repository that this resource belongs to.
        /// </summary>
        public IResourceRepository Repository => _parent;

        /// <summary>
        /// Opens a resource stream to be read-only.
        /// </summary>
        /// <returns>Resource stream</returns>
        public Stream Open() => Open(ResourceFileMode.Open, ResourceFileAccess.Read, ResourceFileShare.Read);

        /// <summary>
        /// Opens a resource stream.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <returns>Resource stream</returns>
        public Stream Open(ResourceFileMode fileMode) => Open(fileMode, ResourceFileAccess.Read, ResourceFileShare.Read);

        /// <summary>
        /// Opens a resource stream.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <param name="accessMode">Specifies the read or write access of the file.</param>
        /// <returns>Resource stream</returns>
        public Stream Open(ResourceFileMode fileMode, ResourceFileAccess accessMode) => Open(fileMode, accessMode, ResourceFileShare.Read);

        /// <summary>
        /// Opens a resource steam.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <param name="accessMode">Specifies the read or write access of the file.</param>
        /// <param name="fileShare">Specifies how the same file should be shared with other resource streams</param>
        /// <returns>Resource stream</returns>
        public Stream Open(ResourceFileMode fileMode, ResourceFileAccess accessMode, ResourceFileShare fileShare)
        {
            ThrowIfParentNotOpen();

            try
            {
                FileStream fs = _fileInfo.Open((FileMode)fileMode, (FileAccess)accessMode, (FileShare)fileShare);
                _fileInfo.Refresh();

                return fs;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }
        }

        /// <summary>
        /// Opens a resource stream for writing. If the resource file does not exist, it will be created.
        /// </summary>
        /// <returns>Resource stream</returns>
        public Stream OpenWrite() => Open(ResourceFileMode.Create, ResourceFileAccess.Write, ResourceFileShare.None);

        /// <summary>
        /// Opens a resource stream for reading.
        /// </summary>
        /// <returns>Resource stream</returns>
        public Stream OpenRead() => Open(ResourceFileMode.Open, ResourceFileAccess.Read, ResourceFileShare.Read);

        /// <summary>
        /// Creates a new file with the resource name and returns a writable stream.
        /// </summary>
        /// <returns>Resource stream</returns>
        public Stream Create()
        {
            ThrowIfParentNotOpen();

            try
            {
                return _fileInfo.Create();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }
        }

        /// <summary>
        /// Permanently deletes the resource file.
        /// </summary>
        /// <exception cref="TeslaContentException">Thrown if the parent repository is not opened or if there was an error in deleting the file.</exception> 
        public void Delete()
        {
            ThrowIfParentNotOpen();

            try
            {
                _fileInfo.Delete();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }
        }

        /// <summary>
        /// Refreshes information about the resource file.
        /// </summary>
        /// <exception cref="TeslaContentException">Thrown if the parent repository is not opened or if there was an error in refreshing the file.</exception> 
        public void Refresh()
        {
            ThrowIfParentNotOpen();

            try
            {
                _fileInfo.Refresh();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }
        }

        /// <summary>
        /// Throws an exception if the parent repository is not open
        /// </summary>
        private void ThrowIfParentNotOpen()
        {
            if (!_parent.IsOpen)
            {
                throw new SparkContentException("Resource repository has not been opened");
            }
        }
    }
}
