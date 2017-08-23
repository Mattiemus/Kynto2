namespace Spark.Content
{
    using System;

    /// <summary>
    /// Represents a reference to a resource that is external to some other resource.
    /// </summary>
    public sealed class ExternalReference
    {
        /// <summary>
        /// Gets a null external reference.
        /// </summary>
        public static ExternalReference NullReference { get; }

        /// <summary>
        /// Gets or sets the runtime target type of the reference.
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Gets or sets the resource path which should include the resource extension.
        /// </summary>
        public string ResourcePath { get; private set; }

        /// <summary>
        /// Static constructor for the <see cref="ExternalReference"/> class
        /// </summary>
        static ExternalReference()
        {
            NullReference = new ExternalReference();
            NullReference.TargetType = typeof(ISavable);
            NullReference.ResourcePath = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalReference"/> class.
        /// </summary>
        private ExternalReference()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalReference"/> class.
        /// </summary>
        /// <param name="targetType">Runtime target type</param>
        /// <param name="resourcePath">Path to the external resource file.</param>
        public ExternalReference(Type targetType, string resourcePath)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType), "Type cannot be null");
            }

            if (string.IsNullOrEmpty(resourcePath))
            {
                throw new ArgumentNullException(nameof(resourcePath), "Resource path cannot be null");
            }

            ResourcePath = resourcePath;
            TargetType = targetType;
        }
    }
}
