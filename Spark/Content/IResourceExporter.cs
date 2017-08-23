namespace Spark.Content
{
    using System;
    using System.IO;

    /// <summary>
    /// A resource exporter capable of saving a piece of content to some output. Since content can be saved asynchronously, the exporter should
    /// be designed to be stateless. Exporters should be capable of handling content types that either match the target type or types that can be assignable
    /// to it (e.g. an ISavable exporter should be able to handle ALL types that implement the ISavable interface).
    /// </summary>
    public interface IResourceExporter
    {
        /// <summary>
        /// Gets the format extension that this exporter supports.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Gets the content target type this exporter serves.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Writes the specified content to a resource file.
        /// </summary>
        /// <param name="resourceFile">Resource file to write to.</param>
        /// <param name="content">Content to write.</param>
        void Save(IResourceFile resourceFile, object content);

        /// <summary>
        /// Writes the specified content to a stream.
        /// </summary>
        /// <param name="output">Output stream to write to.</param>
        /// <param name="content">Content to write.</param>
        void Save(Stream output, object content);

        /// <summary>
        /// Returns true if the specified type can be handled by this exporter, false otherwise.
        /// </summary>
        /// <param name="contentType">Content type to be saved.</param>
        /// <returns>True if the type can be saved by this exporter, false otherwise.</returns>
        bool CanSaveType(Type contentType);
    }
}
