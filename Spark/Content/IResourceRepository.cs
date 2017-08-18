namespace Spark.Content
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface that defines a resource repository. A repository can be as simple as a file directory or something more complex like a compressed archive
    /// or remote web server. It is capable of locating and creating individual resources within the repository, so resource names are generally given to be a relative
    /// path or the file name of the resource. A repository should always be able to return a resource file, although the actual file may or may not exist (if it
    /// does not exist, it may be created via methods on the resource file).
    /// </summary>
    public interface IResourceRepository
    {
        /// <summary>
        /// Gets the root path of the resource repository. This can be the path directory on a file system, an URL, etc.
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// Gets whether the repository is currently opened.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets whether the root path points to a repository that exists.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Gets if the repository is read only (if it exists).
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the repository was initially created.
        /// </summary>
        DateTime CreationTime { get; }

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the repository was last accessed.
        /// </summary>
        DateTime LastAccessTime { get; }

        /// <summary>
        /// Opens a connection to the resource repository to be read-only. Once a connection has been opened, then the repository can be used to located resources.
        /// </summary>
        void OpenConnection();

        /// <summary>
        /// Opens a connection to the resource repository. Once a connection has been opened, then the repository can be used to located resources.
        /// </summary>
        /// <param name="mode">Specifies how the repository should be opened or created</param>
        void OpenConnection(ResourceFileMode mode);

        /// <summary>
        /// Opens a connection to the resource repository. Once a connection has been opened, then the repository can be used to located resources.
        /// </summary>
        /// <param name="mode">Specifies how the repository should be opened or created</param>
        /// <param name="shareMode">Specifies how other repositories can access the repository.</param>
        void OpenConnection(ResourceFileMode mode, ResourceFileShare shareMode);

        /// <summary>
        /// Closes a connection to the resource repository.
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Gets a resource file from the repository with the specified file name.
        /// </summary>
        /// <param name="resourceName">Resource file name, including its extension</param>
        /// <returns>Returns the resource, which may or may not be valid if the resource does not exist.</returns>
        IResourceFile GetResourceFile(string resourceName);

        /// <summary>
        /// Gets a resource file from the repository with the specified file name that is relative to a known resource. Typically the known
        /// resource is a primary content source that references the secondary content source (e.g. a texture or shader).
        /// </summary>
        /// <param name="resourceName">Resource file name, including its extension</param>
        /// <param name="resource">Known resource</param>
        /// <returns>Returns the resource, which may or may not be valid if the resource does not exist.</returns>
        IResourceFile GetResourceFileRelativeTo(string resourceName, IResourceFile resource);

        /// <summary>
        /// Enumerates all resources in the resource repository.
        /// </summary>
        /// <param name="recursive">True if files in sub directories should also be enumerated, false otherwise.</param>
        /// <returns>All resources in the repository</returns>
        IEnumerable<IResourceFile> EnumerateResourceFiles(bool recursive);

        /// <summary>
        /// Queries if the resource is contained in the repository.
        /// </summary>
        /// <param name="resourceName">Resource file name, including its extension</param>
        /// <returns>Returns true if the resource is present in the repository.</returns>
        bool ContainsResource(string resourceName);

        /// <summary>
        /// Gets the fully qualified path to the resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource, including its extension</param>
        /// <returns>The fully qualified path to the resource</returns>
        string GetFullPath(string resourceName);

        /// <summary>
        /// Gets the fully qualified path to a resource that is relative to the specified resource file.
        /// </summary>
        /// <param name="resourceName">Resource file name, including its extension</param>
        /// <param name="resource">Known resource</param>
        /// <returns>Returns the fully qualified path of the resource</returns>
        string GetFullPathRelativeTo(string resourceName, IResourceFile resource);

        /// <summary>
        /// Refreshes information about the repository.
        /// </summary>
        void Refresh();
    }
}
