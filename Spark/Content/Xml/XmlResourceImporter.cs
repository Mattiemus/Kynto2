namespace Spark.Content.Xml
{
    using System.IO;

    /// <summary>
    /// A resource importer that can load <see cref="ISavable"/> objects from a XML format.
    /// </summary>
    public sealed class XmlResourceImporter : ResourceImporter<ISavable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResourceImporter"/> class
        /// </summary>
        public XmlResourceImporter() 
            : base(".xml")
        {
        }

        /// <summary>
        /// Loads content from the specified resource as the target runtime type.
        /// </summary>
        /// <param name="resourceFile">Resource file to read from</param>
        /// <param name="contentManager">Calling content manager</param>
        /// <param name="parameters">Optional loading parameters</param>
        /// <returns>The loaded object or null if it could not be loaded</returns>
        public override ISavable Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters)
        {
            ValidateParameters(resourceFile, contentManager, ref parameters);

            ExternalReferenceResolver resolver = new ExternalReferenceResolver(contentManager, resourceFile);
            Stream stream = resourceFile.OpenRead();

            using (XmlSavableReader reader = new XmlSavableReader(contentManager.ServiceProvider, stream, resolver))
            {
                return reader.ReadSavable<ISavable>();
            }
        }

        /// <summary>
        /// Loads content from the specified stream as the target runtime type.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="contentManager">Calling content manager.</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <returns>The loaded object or null if it could not be loaded.</returns>
        public override ISavable Load(Stream input, ContentManager contentManager, ImporterParameters parameters)
        {
            ValidateParameters(input, contentManager, ref parameters);

            using (XmlSavableReader reader = new XmlSavableReader(contentManager.ServiceProvider, input, true))
            {
                return reader.ReadSavable<ISavable>();
            }
        }
    }
}
