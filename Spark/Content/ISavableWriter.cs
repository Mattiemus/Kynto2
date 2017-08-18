namespace Spark.Content
{
    /// <summary>
    /// Interface that defines a writer that can write objects that implement the <see cref="ISavable"/> interface to an output.
    /// </summary>
    public interface ISavableWriter : IPrimitiveWriter
    {
        /// <summary>
        /// Gets the external handler that controls how savables are to be written externally from the output. If no handler is set,
        /// then external savables are always handled as shared references.
        /// </summary>
        IExternalReferenceHandler ExternalHandler { get; }

        /// <summary>
        /// Gets the write flags that specify certain behaviors during serialization.
        /// </summary>
        SavableWriteFlags WriteFlags { get; }

        /// <summary>
        /// Writes a savable object to the output.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Savable object</param>
        void WriteSavable<T>(string name, T value) where T : ISavable;

        /// <summary>
        /// Writes an array of savable objects to the output.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of savables</param>
        void WriteSavable<T>(string name, T[] values) where T : ISavable;

        /// <summary>
        /// Writes a savable object to the output as a shared resource. This ensures an object that is used
        /// in multiple places is only ever written once to the output. This behavior may be overridden where the savable is instead written
        /// as an external reference.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Savable object</param>
        void WriteSharedSavable<T>(string name, T value) where T : ISavable;

        /// <summary>
        /// Writes aan array of savable object to the output where each item is a shared resource. This ensures an object that is used
        /// in multiple places is only ever written once to the output. This behavior may be overridden where each individual savable is instead written
        /// as an external reference.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of savables</param>
        void WriteSharedSavable<T>(string name, T[] values) where T : ISavable;

        /// <summary>
        /// Writes a savable object to an external output that possibly is a different format. This behavior may be overridden where the savable is 
        /// instead written as a shared object.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Savable object</param>
        void WriteExternalSavable<T>(string name, T value) where T : ISavable;

        /// <summary>
        /// Writes an array of savable objects to an external output that possibly is a different format. This behavior may be overridden where the savable is
        /// instead written as a shared object.
        /// </summary>
        /// <typeparam name="T">Type of object to write</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Array of savables</param>
        void WriteExternalSavable<T>(string name, T[] values) where T : ISavable;
    }
}
