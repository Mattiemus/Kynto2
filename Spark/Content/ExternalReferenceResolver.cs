namespace Spark.Content
{
    using System;

    /// <summary>
    /// Generic external resolver that is able to resolve an external reference path to a savable object.
    /// </summary>
    public class ExternalReferenceResolver : IExternalReferenceResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalReferenceResolver"/> class.
        /// </summary>
        /// <param name="contentManager">Content manager that loads the resource.</param>
        /// <param name="parentResourceFile">Resource file to load external references relative to.</param>
        public ExternalReferenceResolver(ContentManager contentManager, IResourceFile parentResourceFile)
        {
            if (contentManager == null)
            {
                throw new ArgumentNullException(nameof(contentManager));
            }

            if (parentResourceFile == null)
            {
                throw new ArgumentNullException(nameof(parentResourceFile));
            }

            ContentManager = contentManager;
            ParentResourceFile = parentResourceFile;
        }

        /// <summary>
        /// Gets the content manager that can load the external resource.
        /// </summary>
        public ContentManager ContentManager { get; }

        /// <summary>
        /// Gets the resource file this handler resolves references relative to.
        /// </summary>
        public IResourceFile ParentResourceFile { get; }

        /// <summary>
        /// Resolves an external reference and loads the savable using the content manager.
        /// </summary>
        /// <typeparam name="T">Savable type</typeparam>
        /// <param name="externalReference">External reference</param>
        /// <returns>The read savable</returns>
        public T ResolveSavable<T>(ExternalReference externalReference) where T : ISavable
        {
            if (externalReference == null || externalReference == ExternalReference.NullReference)
            {
                return default(T);
            }

            return (T)ContentManager.LoadRelativeTo<ISavable>(externalReference.ResourcePath, ParentResourceFile, ImporterParameters.None);
        }
    }
}
