namespace Spark.Content
{
    using System;
    using System.IO;

    /// <summary>
    /// Interface that defines access to an individual resource that can be read or written to. Typically the resource is a single file that holds a piece of content. 
    /// Since this object represents the access and not the actual resource object, the underlying asset may or may not actually exist.
    /// </summary>
    public interface IResourceFile
    {
        /// <summary>
        /// Gets the name of the resource file (without format extension).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the resource file's format extension.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Gets the full qualified name of the resource file - the path to the resource, its name, and its extension.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets if the resource file does in fact exist in the repository.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Gets if the resource file is read only (if it exists).
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the resource was initially created.
        /// </summary>
        DateTime CreationTime { get; }

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the resource was last accessed.
        /// </summary>
        DateTime LastAccessTime { get; }

        /// <summary>
        /// Gets the repository that this resource belongs to.
        /// </summary>
        IResourceRepository Repository { get; }

        /// <summary>
        /// Opens a resource stream to be read-only.
        /// </summary>
        /// <returns>Resource stream</returns>
        Stream Open();

        /// <summary>
        /// Opens a resource stream.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <returns>Resource stream</returns>
        Stream Open(ResourceFileMode fileMode);

        /// <summary>
        /// Opens a resource stream.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <param name="accessMode">Specifies the read or write access of the file.</param>
        /// <returns>Resource stream</returns>
        Stream Open(ResourceFileMode fileMode, ResourceFileAccess accessMode);

        /// <summary>
        /// Opens a resource steam.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <param name="accessMode">Specifies the read or write access of the file.</param>
        /// <param name="fileShare">Specifies how the same file should be shared with other resource streams</param>
        /// <returns>Resource stream</returns>
        Stream Open(ResourceFileMode fileMode, ResourceFileAccess accessMode, ResourceFileShare fileShare);

        /// <summary>
        /// Opens a resource stream for writing. If the resource file does not exist, it will be created.
        /// </summary>
        /// <returns>Resource stream</returns>
        Stream OpenWrite();

        /// <summary>
        /// Opens a resource stream for reading.
        /// </summary>
        /// <returns>Resource stream</returns>
        Stream OpenRead();

        /// <summary>
        /// Creates a new file with the resource name and returns a writable stream.
        /// </summary>
        /// <returns>Resource stream</returns>
        Stream Create();

        /// <summary>
        /// Permanently deletes the resource file.
        /// </summary>
        void Delete();

        /// <summary>
        /// Refreshes information about the resource file.
        /// </summary>
        void Refresh();
    }
}
