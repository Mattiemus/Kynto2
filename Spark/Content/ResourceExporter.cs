namespace Spark.Content
{
    using System;
    using System.IO;

    /// <summary>
    /// A base resource exporter.
    /// </summary>
    /// <typeparam name="T">Type of object which can be exported</typeparam>
    public abstract class ResourceExporter<T> : IResourceExporter where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceExporter{T}"/> class.
        /// </summary>
        /// <param name="extension">The format extension this exporter saves files as.</param>
        protected ResourceExporter(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            }

            Extension = extension;
            TargetType = typeof(T);
        }

        /// <summary>
        /// Gets the format extension that this exporter supports.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Gets the content target type this exporter serves.
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// Writes the specified content to a resource file.
        /// </summary>
        /// <param name="resourceFile">Resource file to write to</param>
        /// <param name="content">Content to write</param>
        public abstract void Save(IResourceFile resourceFile, T content);

        /// <summary>
        /// Writes the specified content to a stream.
        /// </summary>
        /// <param name="output">Output stream to write to</param>
        /// <param name="content">Content to write</param>
        public abstract void Save(Stream output, T content);

        /// <summary>
        /// Writes the specified content to a resource file.
        /// </summary>
        /// <param name="resourceFile">Resource file to write to.</param>
        /// <param name="content">Content to write.</param>
        public void Save(IResourceFile resourceFile, object content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (!CanSaveType(content.GetType()))
            {
                throw new InvalidCastException($"Could not match type {TargetType.FullName} with type {typeof(T).FullName}");
            }

            Save(resourceFile, (T)content);
        }

        /// <summary>
        /// Writes the specified content to a stream.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="content">Content to write.</param>
        public void Save(Stream output, object content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (!CanSaveType(content.GetType()))
            {
                throw new InvalidCastException($"Could not match type {TargetType.FullName} with type {typeof(T).FullName}");
            }

            Save(output, (T)content);
        }

        /// <summary>
        /// Returns true if the specified type can be handled by this exporter, false otherwise.
        /// </summary>
        /// <param name="contentType">Content type to be saved.</param>
        /// <returns>True if the type can be saved by this exporter, false otherwise.</returns>
        public virtual bool CanSaveType(Type contentType)
        {
            if (contentType == null)
            {
                return false;
            }

            // Most exporters will have a target type that will allow downcasting to more concrete types (e.g. Texture -> Texture2D)
            // but upcasting is also valid (Texture -> ISavable)
            return TargetType.IsAssignableFrom(contentType) || contentType.IsAssignableFrom(TargetType);
        }

        /// <summary>
        /// Validates input parameters - whether the resource file and resource to save are not null.
        /// </summary>
        /// <param name="resourceFile">Resource file to be created or to write to</param>
        /// <param name="content">Resource to save</param>
        protected void ValidateParameters(IResourceFile resourceFile, T content)
        {
            if (resourceFile == null)
            {
                throw new ArgumentNullException(nameof(resourceFile));
            }

            if (!resourceFile.Extension.Equals(Extension, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Exporter cannot save a file of extension {resourceFile.Extension}");
            }

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
        }

        /// <summary>
        /// Validates input parameters - whether the output stream or resource to save exist and if the stream is writable.
        /// </summary>
        /// <param name="output">Output stream to write to</param>
        /// <param name="content">Resource to save</param>
        protected void ValidateParameters(Stream output, T content)
        {
            if (output == null || !output.CanWrite)
            {
                throw new ArgumentNullException(nameof(output), "Cannot write to stream");
            }

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
        }
    }
}
