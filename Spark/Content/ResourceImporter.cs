namespace Spark.Content
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// A base resource importer.
    /// </summary>
    /// <typeparam name="T">Content target type</typeparam>
    public abstract class ResourceImporter<T> : IResourceImporter where T : class
    {
        private readonly string[] _extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceImporter{T}"/> class.
        /// </summary>
        /// <param name="extensions">Format extensions that the importer can import</param>
        protected ResourceImporter(params string[] extensions)
        {
            _extensions = extensions;
            TargetType = typeof(T);

            if (extensions == null || extensions.Length == 0)
            {
                throw new ArgumentNullException(nameof(extensions));
            }
        }

        /// <summary>
        /// Gets the format extensions that this importer supports.
        /// </summary>
        public IEnumerable<string> Extensions => _extensions;

        /// <summary>
        /// Gets the content target type this importer serves.
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// Loads content from the specified resource as the target runtime type.
        /// </summary>
        /// <param name="resourceFile">Resource file to read from</param>
        /// <param name="contentManager">Calling content manager</param>
        /// <param name="parameters">Optional loading parameters</param>
        /// <returns>The loaded object or null if it could not be loaded</returns>
        public abstract T Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters);

        /// <summary>
        /// Loads content from the specified stream as the target runtime type.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="contentManager">Calling content manager.</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <returns>The loaded object or null if it could not be loaded.</returns>
        public abstract T Load(Stream input, ContentManager contentManager, ImporterParameters parameters);

        /// <summary>
        /// Loads content from the specified resource.
        /// </summary>
        /// <param name="resourceFile">Resource file to read from</param>
        /// <param name="contentManager">Calling content manager</param>
        /// <param name="parameters">Optional loading parameters</param>
        /// <returns>The loaded object or null if it could not be loaded</returns>
        object IResourceImporter.Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters)
        {
            return Load(resourceFile, contentManager, parameters);
        }

        /// <summary>
        /// Loads content from the specified stream.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="contentManager">Calling content manager.</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <returns>The loaded object or null if it could not be loaded.</returns>
        object IResourceImporter.Load(Stream input, ContentManager contentManager, ImporterParameters parameters)
        {
            return Load(input, contentManager, parameters);
        }

        /// <summary>
        /// Returns true if the specified type can be handled by this importer, false otherwise.
        /// </summary>
        /// <param name="contentType">Content type to be loaded.</param>
        /// <returns>True if the type can be loaded by this importer, false otherwise.</returns>
        public virtual bool CanLoadType(Type contentType)
        {
            if (contentType == null)
            {
                return false;
            }

            // Most importers will have a target type that will allow downcasting to more concrete types (e.g. Texture -> Texture2D)
            // but upcasting is also valid (Texture -> ISavable), but that may be iffy because then a collision may occur with another
            // extension-importer pair (how this importer is stored in the content manager)
            return TargetType.IsAssignableFrom(contentType) || contentType.IsAssignableFrom(TargetType);
        }

        /// <summary>
        /// Validates input parameters - whether the resource file exists or if the importer can handle its extension. Be aware that
        /// the format of the data is not validated, just the file extension.
        /// </summary>
        /// <param name="resourceFile">The resource file to load from</param>
        /// <param name="contentManager">The calling content manager</param>
        /// <param name="parameters">Optional parameters, if null <see cref="ImporterParameters.None"/> is set.</param>
        protected void ValidateParameters(IResourceFile resourceFile, ContentManager contentManager, ref ImporterParameters parameters)
        {
            if (resourceFile == null || !resourceFile.Exists)
            {
                throw new ArgumentNullException(nameof(resourceFile), "Resource file does not exist");
            }

            ThrowIfBadExtension(resourceFile);

            if (contentManager == null)
            {
                throw new ArgumentNullException(nameof(contentManager));
            }

            if (parameters == null)
            {
                parameters = ImporterParameters.None;
            }

            string reason;
            if (!parameters.Validate(out reason))
            {
                throw new SparkContentException($"Invalid importer parameters: {reason}");
            }
        }

        /// <summary>
        /// Validates input parameters - if we can read from the stream and the like. Be aware that the format of the data is
        /// not validated.
        /// </summary>
        /// <param name="input">The stream to load from</param>
        /// <param name="contentManager">The calling content manager</param>
        /// <param name="parameters">Optional parameters, if null <see cref="ImporterParameters.None"/> is set.</param>
        protected void ValidateParameters(Stream input, ContentManager contentManager, ref ImporterParameters parameters)
        {
            if (input == null || !input.CanRead)
            {
                throw new ArgumentNullException(nameof(input), "Cannot read from stream");
            }

            if (contentManager == null)
            {
                throw new ArgumentNullException(nameof(contentManager));
            }

            if (parameters == null)
            {
                parameters = ImporterParameters.None;
            }

            string reason;
            if (!parameters.Validate(out reason))
            {
                throw new SparkContentException($"Invalid importer parameters: {reason}");
            }
        }

        /// <summary>
        /// Throws an exception if the resource file cannot be loaded by this importer
        /// </summary>
        /// <param name="resourceFile">Resource file</param>
        private void ThrowIfBadExtension(IResourceFile resourceFile)
        {
            bool canHandle = false;
            foreach (string extension in _extensions)
            {
                if (resourceFile.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    canHandle = true;
                    break;
                }
            }

            if (!canHandle)
            {
                throw new ArgumentException($"Importer cannot load a file of extension {resourceFile.Extension}");
            }
        }
    }
}
