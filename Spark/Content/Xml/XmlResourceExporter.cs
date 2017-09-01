namespace Spark.Content.Xml
{
    using System;
    using System.IO;

    using Binary;

    /// <summary>
    /// A resource exporter that can save <see cref="ISavable"/> objects to an XML format.
    /// </summary>
    public sealed class XmlResourceExporter : ResourceExporter<ISavable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResourceExporter"/> class.
        /// </summary>
        public XmlResourceExporter() 
            : base(".xml")
        {
        }

        /// <summary>
        /// Helper method to create a new external reference handler that has its default external writer set to a xml external writer.
        /// </summary>
        /// <param name="resourceFile">Resource file that is the parent to the files the handler will handle</param>
        /// <param name="writeFlags">Write flags</param>
        /// <returns>A new external reference handler.</returns>
        public static IExternalReferenceHandler CreateXmlExternalReferenceHandler(IResourceFile resourceFile, SavableWriteFlags writeFlags)
        {
            return new ExternalReferenceHandler(resourceFile, writeFlags, new XmlExternalWriter());
        }

        /// <summary>
        /// Writes the specified content to a resource file, as dictated by the user supplied external handler. Use this method
        /// if custom content naming and external writers are required. The savable write flags are also dictated by those on the external
        /// handler.
        /// </summary>
        /// <param name="externalHandler">User-specified external handler</param>
        /// <param name="content">Content to write</param>
        public void Save(IExternalReferenceHandler externalHandler, ISavable content)
        {
            if (externalHandler == null)
            {
                throw new ArgumentNullException(nameof(externalHandler));
            }

            ValidateParameters(externalHandler.ParentResourceFile, content);

            Stream output = externalHandler.ParentResourceFile.OpenWrite();
            using (XmlSavableWriter writer = new XmlSavableWriter(output, externalHandler.WriteFlags, externalHandler))
            {
                writer.WriteSavable("Object", content);
            }
        }

        /// <summary>
        /// Writes the specified content to a resource file, as dictated by the user supplied external handler. Use this method
        /// if custom content naming and external writers are required.
        /// </summary>
        /// <param name="externalHandler">User-specified external handler</param>
        /// <param name="content">Content to write</param>
        /// <param name="writeFlagsForPrimaryObject">Write flags that may differ from the external handler write flags, for the primary content object.</param>
        public void Save(IExternalReferenceHandler externalHandler, ISavable content, SavableWriteFlags writeFlagsForPrimaryObject)
        {
            if (externalHandler == null)
            {
                throw new ArgumentNullException(nameof(externalHandler));
            }

            ValidateParameters(externalHandler.ParentResourceFile, content);

            Stream output = externalHandler.ParentResourceFile.OpenWrite();
            using (XmlSavableWriter writer = new XmlSavableWriter(output, writeFlagsForPrimaryObject, externalHandler))
            {
                writer.WriteSavable("Object", content);
            }
        }

        /// <summary>
        /// Writes the specified content to a resource file.
        /// </summary>
        /// <param name="resourceFile">Resource file to write to</param>
        /// <param name="content">Content to write</param>
        /// <param name="writeFlags">Savable write flags</param>
        public void Save(IResourceFile resourceFile, ISavable content, SavableWriteFlags writeFlags)
        {
            ValidateParameters(resourceFile, content);

            ExternalReferenceHandler handler = new ExternalReferenceHandler(resourceFile, writeFlags, new BinaryExternalWriter());

            Stream output = resourceFile.OpenWrite();
            using (XmlSavableWriter writer = new XmlSavableWriter(output, writeFlags, handler))
            {
                writer.WriteSavable("Object", content);
            }
        }

        /// <summary>
        /// Writes the specified content to a stream.
        /// </summary>
        /// <param name="output">Output stream to write to</param>
        /// <param name="content">Content to write</param>
        /// <param name="writeFlags">Savable write flags</param>
        public void Save(Stream output, ISavable content, SavableWriteFlags writeFlags)
        {
            ValidateParameters(output, content);

            using (XmlSavableWriter writer = new XmlSavableWriter(output, writeFlags, true))
            {
                writer.WriteSavable("Object", content);
            }
        }

        /// <summary>
        /// Writes the specified content to a resource file.
        /// </summary>
        /// <param name="resourceFile">Resource file to write to</param>
        /// <param name="content">Content to write</param>
        public override void Save(IResourceFile resourceFile, ISavable content)
        {
            SavableWriteFlags writeFlags = SavableWriteFlags.OverwriteExistingResource | SavableWriteFlags.UseCompression;
            Save(resourceFile, content, writeFlags);
        }

        /// <summary>
        /// Writes the specified content to a stream.
        /// </summary>
        /// <param name="output">Output stream to write to</param>
        /// <param name="content">Content to write</param>
        public override void Save(Stream output, ISavable content)
        {
            SavableWriteFlags writeFlags = SavableWriteFlags.OverwriteExistingResource | SavableWriteFlags.UseCompression;
            Save(output, content, writeFlags);
        }
    }
}
