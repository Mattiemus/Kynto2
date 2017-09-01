namespace Spark.Content.Xml
{
    using System;
    using System.IO;

    /// <summary>
    /// Default external reference writer for XML files.
    /// </summary>
    public class XmlExternalWriter : IExternalReferenceWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlExternalWriter"/> class.
        /// </summary>
        public XmlExternalWriter()
        {
            TargetType = typeof(ISavable);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlExternalWriter"/> class.
        /// </summary>
        /// <param name="type">Resource type to write externally.</param>
        public XmlExternalWriter(Type type)
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
        public string ResourceExtension => ".xml";

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
            using (XmlSavableWriter xmlWriter = new XmlSavableWriter(output, externalHandler.WriteFlags, externalHandler))
            {
                xmlWriter.WriteSavable<ISavable>("Object", value);
            }
        }
    }
}
