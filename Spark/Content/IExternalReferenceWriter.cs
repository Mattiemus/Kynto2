namespace Spark.Content
{
    using System;

    /// <summary>
    /// Interface for a writer that knows how to write a resource that is external to another resource.
    /// </summary>
    public interface IExternalReferenceWriter
    {
        /// <summary>
        /// Gets the target type of the savable object that this writer can output.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Gets the resource extension of the output resource file this writer can create.
        /// </summary>
        string ResourceExtension { get; }

        /// <summary>
        /// Writes a savable object to a resource file.
        /// </summary>
        /// <typeparam name="T">Savable type</typeparam>
        /// <param name="outputResourceFile">Resource file to write to</param>
        /// <param name="externalHandler">The external handler that is calling the writer.</param>
        /// <param name="value">Savable to write</param>
        void WriteSavable<T>(IResourceFile outputResourceFile, IExternalReferenceHandler externalHandler, T value) where T : ISavable;
    }
}
