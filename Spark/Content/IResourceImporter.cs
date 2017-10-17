namespace Spark.Content
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    /// <summary>
    /// A resource importer capable of loading a piece of content from a resource source. Since resources can be asynchronously loaded,
    /// importers should be designed to be stateless. Importers should be capable of handling content types that either match the target type or types that
    /// can be assigned to it (e.g. an ISavable importer should be able to handle ALL types that implement the ISavable interface).
    /// </summary>
    public interface IResourceImporter
    {
        /// <summary>
        /// Gets the format extensions that this importer supports.
        /// </summary>
        IEnumerable<string> Extensions { get; }

        /// <summary>
        /// Gets the content target type this importer serves.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Loads content from the specified resource.
        /// </summary>
        /// <param name="resourceFile">Resource file to read from.</param>
        /// <param name="contentManager">Calling content manager.</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <returns>The loaded object or null if it could not be loaded.</returns>
        object Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters);

        /// <summary>
        /// Loads content from the specified stream.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="contentManager">Calling content manager.</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <returns>The loaded object or null if it could not be loaded.</returns>
        object Load(Stream input, ContentManager contentManager, ImporterParameters parameters);

        /// <summary>
        /// Returns true if the specified type can be handled by this importer, false otherwise.
        /// </summary>
        /// <param name="contentType">Content type to be loaded.</param>
        /// <returns>True if the type can be loaded by this importer, false otherwise.</returns>
        bool CanLoadType(Type contentType);
    }
}
