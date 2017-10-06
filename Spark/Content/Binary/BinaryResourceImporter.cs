namespace Spark.Content.Binary
{
    using System.IO;

    /// <summary>
    /// A resource importer that can load <see cref="ISavable"/> objects from the SPBO (Spark Binary Object) format.
    /// </summary>
    public sealed class BinaryResourceImporter : ResourceImporter<ISavable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryResourceImporter"/> class. This queries the engine instance
        /// for the render system.
        /// </summary>
        public BinaryResourceImporter() 
            : base(".spbo")
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
            using (BinarySavableReader reader = new BinarySavableReader(contentManager.ServiceProvider, stream, resolver))
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

            using (BinarySavableReader reader = new BinarySavableReader(contentManager.ServiceProvider, input, true))
            {
                return reader.ReadSavable<ISavable>();
            }
        }
    }
}
