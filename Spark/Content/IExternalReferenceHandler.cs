namespace Spark.Content
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Delegate for generating a custom resource name from a given savable object. This should not include any resource extension.
    /// </summary>
    /// <param name="handler">External handler that is handling this resource.</param>
    /// <param name="savable">Savable to generate a resource name for</param>
    /// <returns>Generated resource name</returns>
    public delegate string GetResourceNameDelegate(IExternalReferenceHandler handler, ISavable savable);

    /// <summary>
    /// Interface for a handler that processes savable objects that need to be written externally to some
    /// resource file.
    /// </summary>
    public interface IExternalReferenceHandler
    {
        /// <summary>
        /// Gets the resource file that this handler processes child resource files for. The resources
        /// that are created/updated are relative to this resource file.
        /// </summary>
        IResourceFile ParentResourceFile { get; }

        /// <summary>
        /// Gets the write flags that specify certain behaviors during serialization.
        /// </summary>
        SavableWriteFlags WriteFlags { get; }

        /// <summary>
        /// Registers an external writer that handles a specific savable type.
        /// </summary>
        /// <param name="writer">External savable writer</param>
        void RegisterWriter(IExternalReferenceWriter writer);

        /// <summary>
        /// Removes an external writer based on its target savable type it was registered to.
        /// </summary>
        /// <typeparam name="T">Target savable type</typeparam>
        /// <returns>True if the writer was removed, false otherwise</returns>
        bool RemoveWriter<T>() where T : ISavable;

        /// <summary>
        /// Gets a registered external writer based on its target savable type it was registered to.
        /// </summary>
        /// <typeparam name="T">Target savable type</typeparam>
        /// <returns>The external savable writer, or null if a writer is not registered to the type.</returns>
        IExternalReferenceWriter GetWriter<T>() where T : ISavable;

        /// <summary>
        /// Gets all external writers registered to this handler.
        /// </summary>
        /// <returns>Collection of external savable writers.</returns>
        IEnumerable<IExternalReferenceWriter> GetWriters();

        /// <summary>
        /// Adds a delegate that returns a unique resource name for a given savable, registered to a specific
        /// savable type. This allows for custom name transformations for output files.
        /// </summary>
        /// <typeparam name="T">Target savable type</typeparam>
        /// <param name="getResourceNameDelegate">Get resource name delegate</param>
        void SetGetResourceNameDelegate<T>(GetResourceNameDelegate getResourceNameDelegate) where T : ISavable;

        /// <summary>
        /// Checks if the path/resource name is unique of all the references that the handler has processed so far. 
        /// </summary>
        /// <param name="resourcePath">Resource path (without extension)</param>
        /// <returns>True if the name is unique, false otherwise.</returns>
        bool IsUniqueResourcePath(string resourcePath);

        /// <summary>
        /// Processes a savable into an external reference.
        /// </summary>
        /// <typeparam name="T">Savable type</typeparam>
        /// <param name="value">Savable value</param>
        /// <returns>External reference to the savable</returns>
        ExternalReference ProcessSavable<T>(T value) where T : ISavable;

        /// <summary>
        /// Flushes any remaining data that needs to be written.
        /// </summary>
        void Flush();

        /// <summary>
        /// Clears cached external file references.
        /// </summary>
        void Clear();
    }
}
