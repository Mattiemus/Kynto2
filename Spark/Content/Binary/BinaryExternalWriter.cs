namespace Spark.Content.Binary
{
    using System;
    using System.IO;

    /// <summary>
    /// Default external reference writer for binary (TEBO) files.
    /// </summary>
    public sealed class BinaryExternalWriter : IExternalReferenceWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExternalWriter"/> class.
        /// </summary>
        public BinaryExternalWriter()
        {
            TargetType = typeof(ISavable);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExternalWriter"/> class.
        /// </summary>
        /// <param name="type">Resource type to write externally.</param>
        public BinaryExternalWriter(Type type)
        {
            TargetType = type ?? typeof(ISavable);
        }

        /// <summary>
        /// Gets the target type of the savable object that this writer can output.
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// Gets the resource extension of the output resource file this writer can create.
        /// </summary>
        public string ResourceExtension => ".spbo";

        /// <summary>
        /// Writes a savable object to a resource file.
        /// </summary>
        /// <param name="outputResourceFile">Resource file to write to</param>
        /// <param name="externalHandler">The external handler that is calling the writer.</param>
        /// <param name="value">Savable to write</param>
        public void WriteSavable<T>(IResourceFile outputResourceFile, IExternalReferenceHandler externalHandler, T value) where T : ISavable
        {
            if (outputResourceFile == null)
            {
                throw new ArgumentNullException(nameof(outputResourceFile));
            }

            if (externalHandler == null)
            {
                throw new ArgumentNullException(nameof(externalHandler));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!TargetType.IsInstanceOfType(value))
            {
                throw new InvalidCastException($"Could not match type {value.GetType().FullName} with type {TargetType.FullName}");
            }

            if (outputResourceFile.Extension != ResourceExtension)
            {
                throw new ArgumentException($"External writer cannot save a file of extension {outputResourceFile.Extension}");
            }

            Stream output = outputResourceFile.OpenWrite();
            using (BinarySavableWriter binaryWriter = new BinarySavableWriter(output, externalHandler.WriteFlags, externalHandler))
            {
                binaryWriter.WriteSavable<ISavable>("Object", value);
            }
        }
    }
}
