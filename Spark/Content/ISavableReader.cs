namespace Spark.Content
{
    using System;

    /// <summary>
    /// Interface that defines a reader that can read objects that implement the <see cref="ISavable"/> interface from an input.
    /// </summary>
    public interface ISavableReader : IPrimitiveReader
    {
        /// <summary>
        /// Gets the service provider, used to locate any services that may be needed during content loading.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the external reference resolver of the reader.
        /// </summary>
        IExternalReferenceResolver ExternalResolver { get; }

        /// <summary>
        /// Reads a savable object from the input.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Object read</returns>
        T ReadSavable<T>() where T : ISavable;

        /// <summary>
        /// Reads an array of savable objects from the input.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Array of objects read</returns>
        T[] ReadSavableArray<T>() where T : ISavable;

        /// <summary>
        /// Reads a shared savable object from the input.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Object read</returns>
        T ReadSharedSavable<T>() where T : ISavable;

        /// <summary>
        /// Reads an array of shared savable objects from the input.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Array of objects read</returns>
        T[] ReadSharedSavableArray<T>() where T : ISavable;

        /// <summary>
        /// Reads a savable object that is external from the input, e.g. a separate file.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Object read</returns>
        T ReadExternalSavable<T>() where T : ISavable;

        /// <summary>
        /// Reads an array of savable objects that is external from the input, e.g. a separate file.
        /// </summary>
        /// <typeparam name="T">Type of object to read</typeparam>
        /// <returns>Array of savables</returns>
        T[] ReadExternalSavableArray<T>() where T : ISavable;
    }
}
