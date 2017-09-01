namespace Spark.Content
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Utilities for resource management
    /// </summary>
    public static class ContentHelper
    {
        /// <summary>
        /// Gets the directory location of the application
        /// </summary>
        public static string AppLocation
        {
            get
            {
                Assembly assemb = Assembly.GetEntryAssembly();
                if (assemb == null)
                {
                    assemb = Assembly.GetExecutingAssembly();
                }

                return Path.GetDirectoryName(assemb.Location);
            }
        }

        /// <summary>
        /// Parses a resource path that may optionally have a subresource name. A double colon identifies the separator between
        /// the resource file path and the sub resource name. A resource file may contain several resources that may want to be loaded up
        /// singular and not have the rest of the content be loaded up.
        /// </summary>
        /// <remarks>
        /// Format is: Path/To/Resource/ResourceFileNameWithExtension::subresourceName.
        /// </remarks>
        /// <param name="resourcePathWithName">Resource path with an optional sub resource name.</param>
        /// <param name="path">Resource path without the sub resource name.</param>
        /// <param name="subresourceName">Sub resource name, or an empty string if no name was specified.</param>
        public static void ParseSubresourceName(string resourcePathWithName, out string path, out string subresourceName)
        {
            path = resourcePathWithName;
            subresourceName = string.Empty;

            if (string.IsNullOrEmpty(resourcePathWithName))
            {
                return;
            }

            int indexOfFirst = -1;
            for (int i = 0; i < resourcePathWithName.Length; i++)
            {
                if (resourcePathWithName[i] == ':')
                {
                    int nextIndex = i + 1;
                    if (nextIndex < resourcePathWithName.Length && resourcePathWithName[nextIndex] == ':')
                    {
                        indexOfFirst = i;
                        break;
                    }
                }
            }

            if (indexOfFirst == -1)
            {
                return;
            }

            path = (indexOfFirst == 0) ? string.Empty : resourcePathWithName.Substring(0, indexOfFirst);
            subresourceName = resourcePathWithName.Substring(indexOfFirst + 2, resourcePathWithName.Length - (indexOfFirst + 2));
        }
    }
}
