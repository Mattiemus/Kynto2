namespace Spark.Content
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Represents a file resource repository - a simple directory.
    /// </summary>
    public class FileResourceRepository : IResourceRepository
    {
        private bool _isOpen;
        private readonly DirectoryInfo _directoryInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileResourceRepository"/> class. Uses the default location of
        /// the application as the root path.
        /// </summary>
        public FileResourceRepository() 
            : this(ContentHelper.AppLocation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileResourceRepository"/> class.
        /// </summary>
        /// <param name="pathToDirectory">The directory path. If it is not a rooted path, it is assumed to be relative to the app location.</param>
        public FileResourceRepository(string pathToDirectory)
        {
            try
            {
                // If not rooted, we assume its relative to the app location.
                if (!Path.IsPathRooted(pathToDirectory))
                {
                    _directoryInfo = new DirectoryInfo(Path.Combine(ContentHelper.AppLocation, pathToDirectory));
                }
                else
                {
                    _directoryInfo = new DirectoryInfo(pathToDirectory);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }

            _isOpen = false;
        }

        /// <summary>
        /// Gets the root path of the resource repository. This can be the path directory on a file system, an URL, etc.
        /// </summary>
        public string RootPath => _directoryInfo.FullName;

        /// <summary>
        /// Gets whether the repository is currently opened.
        /// </summary>
        public bool IsOpen => _isOpen;

        /// <summary>
        /// Gets whether the repository does infact exist.
        /// </summary>
        public bool Exists => _directoryInfo.Exists;

        /// <summary>
        /// Gets if the repository is read only (if it exists).
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the repository was initially created.
        /// </summary>
        public DateTime CreationTime => _directoryInfo.CreationTimeUtc;

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the repository was last accessed.
        /// </summary>
        public DateTime LastAccessTime => _directoryInfo.LastAccessTimeUtc;

        /// <summary>
        /// Opens a connection to the resource repository to be read-only. Once a connection has been opened, then the repository can be used to located resources.
        /// </summary>
        public void OpenConnection()
        {
            OpenConnection(ResourceFileMode.Open, ResourceFileShare.Read);
        }

        /// <summary>
        /// Opens a connection to the resource repository. Once a connection has been opened, then the repository can be used to located resources.
        /// </summary>
        /// <param name="mode">Specifies how the repository should be opened or created</param>
        public void OpenConnection(ResourceFileMode mode)
        {
            OpenConnection(mode, ResourceFileShare.None);
        }

        /// <summary>
        /// Opens a connection to the resource repository. Once a connection has been opened, then the repository can be used to located resources.
        /// </summary>
        /// <param name="mode">Specifies how the repository should be opened or created</param>
        /// <param name="shareMode">Specifies how other repositories can access the repository.</param>
        public void OpenConnection(ResourceFileMode mode, ResourceFileShare shareMode)
        {
            ThrowIfAlreadyOpen();

            _directoryInfo.Refresh();
            bool dirExists = _directoryInfo.Exists;

            // Throw exceptions to follow expected behavior if the directory is there or not
            switch (mode)
            {
                case ResourceFileMode.CreateNew:
                    if (dirExists)
                    {
                        throw new ArgumentException("Resource repository already exists", nameof(mode));
                    }
                    break;

                case ResourceFileMode.Open:
                    if (!dirExists)
                    {
                        throw new ArgumentException("Resource repository does not exist", nameof(mode));
                    }
                    break;

                case ResourceFileMode.OpenOrCreate:
                case ResourceFileMode.Create:
                    if (!dirExists)
                    {
                        try
                        {
                            _directoryInfo.Create();
                            _directoryInfo.Refresh();
                        }
                        catch (IOException e)
                        {
                            Trace.WriteLine(e);
                            throw new SparkContentException(e.Message, e);
                        }
                    }
                    break;
            }

            _isOpen = true;
        }

        /// <summary>
        /// Closes a connection to the resource repository.
        /// </summary>
        public void CloseConnection()
        {
            ThrowIfNotOpen();
            _isOpen = false;
        }

        /// <summary>
        /// Gets a resource file from the repository with the specified file name.
        /// </summary>
        /// <param name="resourceName">Resource file name, including its extension</param>
        /// <returns>Returns the resource, which may or may not be valid if the resource does not exist.</returns>
        public IResourceFile GetResourceFile(string resourceName)
        {
            ThrowIfNotOpen();

            if (string.IsNullOrEmpty(resourceName))
            {
                return new NullResource();
            }

            if (!Path.IsPathRooted(resourceName))
            {
                resourceName = GetFullPath(resourceName);
            }

            return new FileResourceFile(resourceName, this);
        }

        /// <summary>
        /// Gets a resource file from the repository with the specified file name that is relative to a known resource. Typically the known
        /// resource is a primary content source that references the secondary content source (e.g. a texture or shader).
        /// </summary>
        /// <param name="resource">Known resource</param>
        /// <param name="resourceName">Resource file name, including its extension</param>
        /// <returns>Returns the resource, which may or may not be valid if the resource does not exist.</returns>
        public IResourceFile GetResourceFileRelativeTo(string resourceName, IResourceFile resource)
        {
            ThrowIfNotOpen();

            if (resource == null || !(resource is FileResourceFile) || string.IsNullOrEmpty(resourceName))
            {
                return new NullResource();
            }

            try
            {
                string dir = Path.GetDirectoryName(resource.FullName);
                return new FileResourceFile(Path.GetFullPath(Path.Combine(dir, resourceName)), this);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }
        }

        /// <summary>
        /// Queries if the resource is contained in the repository.
        /// </summary>
        /// <param name="resourceName">Resource file name, including its extension</param>
        /// <returns>Returns true if the resource is present in the repository.</returns>
        public bool ContainsResource(string resourceName)
        { 
            ThrowIfNotOpen();

            if (string.IsNullOrEmpty(resourceName))
            {
                return false;
            }

            return File.Exists(GetFullPath(resourceName));
        }

        /// <summary>
        /// Gets the fully qualified path to the resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource, including its extension</param>
        /// <returns>The fully qualified path to the resource</returns>
        public string GetFullPath(string resourceName)
        {
            ThrowIfNotOpen();

            if (string.IsNullOrEmpty(resourceName))
            {
                return string.Empty;
            }

            try
            {
                return Path.GetFullPath(Path.Combine(RootPath, resourceName));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the fully qualified path to a resource that is relative to the specified resource file.
        /// </summary>
        /// <param name="resource">Known resource</param>
        /// <param name="resourceName">Resource file name, including its extension</param>
        /// <returns>Returns the fully qualified path of the resource</returns>
        public string GetFullPathRelativeTo(string resourceName, IResourceFile resource)
        {
            ThrowIfNotOpen();

            if (resource == null || !(resource is FileResourceFile) || string.IsNullOrEmpty(resourceName))
            {
                return string.Empty;
            }

            try
            {
                string dir = Path.GetDirectoryName(resource.FullName);
                return Path.GetFullPath(Path.Combine(dir, resourceName));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }
        }

        /// <summary>
        /// Enumerates all resources in the resource repository.
        /// </summary>
        /// <param name="recursive">True if files in sub directories should also be enumerated, false otherwise.</param>
        /// <returns>All resources in the repository</returns>
        public IEnumerable<IResourceFile> EnumerateResourceFiles(bool recursive)
        {
            ThrowIfNotOpen();

            List<IResourceFile> files = new List<IResourceFile>();

            try
            {
                SearchOption searchOption = (recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                string[] fileNames = Directory.GetFiles(RootPath, "*", searchOption);

                foreach (string fileName in fileNames)
                {
                    files.Add(new FileResourceFile(fileName, this));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }

            return files;
        }

        /// <summary>
        /// Refreshes information about the repository.
        /// </summary>
        public void Refresh()
        {
            ThrowIfNotOpen();

            try
            {
                _directoryInfo.Refresh();
            }
            catch (IOException e)
            {
                Trace.WriteLine(e);
                throw new SparkContentException(e.Message, e);
            }
        }

        /// <summary>
        /// Throws an exception if the repository is already open
        /// </summary>
        private void ThrowIfAlreadyOpen()
        {
            if (_isOpen)
            {
                throw new SparkContentException("Resource repository is already open");
            }
        }

        /// <summary>
        /// Throws an exception if the repository is not open
        /// </summary>
        private void ThrowIfNotOpen()
        {
            if (!_isOpen)
            {
                throw new SparkContentException("Resource repository has not been opened");
            }
        }
    }
}
