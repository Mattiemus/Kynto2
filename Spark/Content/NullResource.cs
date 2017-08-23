namespace Spark.Content
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a null resource.
    /// </summary>
    public sealed class NullResource : IResourceFile
    {
        /// <summary>
        /// Gets the name of the resource (without format extension).
        /// </summary>
        public string Name => string.Empty;

        /// <summary>
        /// Gets the name of the resource (with format extension).
        /// </summary>
        public string NameWithExtension => string.Empty;

        /// <summary>
        /// Gets the resource's format extension.
        /// </summary>
        public string Extension => string.Empty;

        /// <summary>
        /// Gets the full qualified name of the resource - the path to the resource, its name, and its extension.
        /// </summary>
        public string FullName => string.Empty;

        /// <summary>
        /// Gets if the resource does in fact exist in the repository.
        /// </summary>
        public bool Exists => false;

        /// <summary>
        /// Gets if the resource file is read only (if it exists).
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Gets the repository that this resource belongs to.
        /// </summary>
        public IResourceRepository Repository => null;

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the resource was initially created.
        /// </summary>
        public DateTime CreationTime => new DateTime();

        /// <summary>
        /// Gets the time, in coordinated universal time (UTC), that the resource was last accessed.
        /// </summary>
        public DateTime LastAccessTime => new DateTime();

        /// <summary>
        /// A null resource cannot be opened.
        /// </summary>
        /// <returns>The resource stream.</returns>
        /// <exception cref="IOException">Always gets thrown, because this is a null resource.</exception>
        public Stream Open()
        {
            return Open(ResourceFileMode.Open, ResourceFileAccess.Read, ResourceFileShare.Read);
        }

        /// <summary>
        /// A null resource cannot be opened.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <returns>Resource stream</returns>
        /// <exception cref="IOException">Always gets thrown, because this is a null resource.</exception>
        public Stream Open(ResourceFileMode fileMode)
        {
            return Open(fileMode, ResourceFileAccess.Read, ResourceFileShare.Read);
        }

        /// <summary>
        /// A null resource cannot be opened.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <param name="accessMode">Specifies the read or write access of the file.</param>
        /// <returns>Resource stream</returns>
        /// <exception cref="IOException">Always gets thrown, because this is a null resource.</exception>
        public Stream Open(ResourceFileMode fileMode, ResourceFileAccess accessMode)
        {
            return Open(fileMode, accessMode, ResourceFileShare.Read);
        }

        /// <summary>
        /// A null resource cannot be opened.
        /// </summary>
        /// <param name="fileMode">Specifies the mode in which to open the file.</param>
        /// <param name="accessMode">Specifies the read or write access of the file.</param>
        /// <param name="fileShare">Specifies how the same file should be shared with other resource streams</param>
        /// <returns>Resource stream</returns>
        /// <exception cref="IOException">Always gets thrown, because this is a null resource.</exception>
        public Stream Open(ResourceFileMode fileMode, ResourceFileAccess accessMode, ResourceFileShare fileShare)
        {
            throw new IOException("File does not exist");
        }

        /// <summary>
        /// A null resource cannot be opened.
        /// </summary>
        /// <returns>Resource stream</returns>
        /// <exception cref="IOException">Always gets thrown, because this is a null resource.</exception>
        public Stream OpenWrite()
        {
            return Open(ResourceFileMode.Create, ResourceFileAccess.Write, ResourceFileShare.None);
        }

        /// <summary>
        /// A null resource cannot be opened.
        /// </summary>
        /// <returns>Resource stream</returns>
        /// <exception cref="IOException">Always gets thrown, because this is a null resource.</exception>
        public Stream OpenRead()
        {
            return Open(ResourceFileMode.Open, ResourceFileAccess.Read, ResourceFileShare.Read);
        }

        /// <summary>
        /// A null resource cannot be opened.
        /// </summary>
        /// <returns>Resource stream</returns>
        /// <exception cref="IOException">Always gets thrown, because this is a null resource.</exception>
        public Stream Create()
        {
            throw new IOException("File does not exist");
        }

        /// <summary>
        /// Permanently deletes the resource file.
        /// </summary>
        public void Delete()
        {
            // No-op
        }

        /// <summary>
        /// Refreshes information about the resource file.
        /// </summary>
        public void Refresh()
        {
            // No-op
        }
    }
}
