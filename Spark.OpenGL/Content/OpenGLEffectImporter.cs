namespace Spark.Content
{
    using System;
    using System.IO;

    using Graphics;

    /// <summary>
    /// A resource importer that can load <see cref="Effect"/> objects from a Spark effect file
    /// </summary>
    public sealed class OpenGLEffectImporter : ResourceImporter<Effect>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLEffectImporter"/> class.
        /// </summary>
        public OpenGLEffectImporter()
            : base(".sfx")
        {
        }

        /// <summary>
        /// Loads content from the specified resource as the target runtime type.
        /// </summary>
        /// <param name="resourceFile">Resource file to read from</param>
        /// <param name="contentManager">Calling content manager</param>
        /// <param name="parameters">Optional loading parameters</param>
        /// <returns>The loaded object or null if it could not be loaded</returns>
        public override Effect Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters)
        {
            ValidateParameters(resourceFile, contentManager, ref parameters);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads content from the specified stream as the target runtime type.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="contentManager">Calling content manager.</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <returns>The loaded object or null if it could not be loaded.</returns>
        public override Effect Load(Stream input, ContentManager contentManager, ImporterParameters parameters)
        {
            ValidateParameters(input, contentManager, ref parameters);

            throw new NotImplementedException();
        }
    }
}
