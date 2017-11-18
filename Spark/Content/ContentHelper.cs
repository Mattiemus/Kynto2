namespace Spark.Content
{
    using System.IO;
    using System.Reflection;
    
    /// <summary>
    /// Utilities for resource management
    /// </summary>
    public static class ContentHelper
    {
        private static char[] _pathTrimChars = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };

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
        /// Gets a resource name for the object.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="resource">Resource object</param>
        /// <returns>Resource name</returns>
        public static string GetResourceName<T>(T resource)
        {
            if (resource == null)
            {
                return string.Empty;
            }

            if (resource is INamed)
            {
                return (resource as INamed).Name;
            }

            return typeof(T).Name;
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

        /// <summary>
        /// Normalizes the path separators in the file path. This will return a path with the same
        /// separators used by <see cref="Path.GetDirectoryName"/>.
        /// </summary>
        /// <param name="filePath">File path to normalize.</param>
        /// <returns>Normalized file path.</returns>
        public static string NormalizePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return filePath;
            }

            return Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
        }

        /// <summary>
        /// Trims the rooted path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string TrimRootedPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !Path.IsPathRooted(filePath))
            {
                return filePath;
            }

            int length = filePath.Length;

            if (length >= 1)
            {
                if (filePath[0] == Path.DirectorySeparatorChar || filePath[0] == Path.AltDirectorySeparatorChar)
                {
                    return filePath.TrimStart(_pathTrimChars);
                }
            }

            if (length >= 2)
            {
                // Ignore the volume letter
                if (filePath[1] == Path.VolumeSeparatorChar)
                {
                    return filePath.Substring(1).TrimStart(_pathTrimChars);
                }
            }

            return filePath;
        }
    }
}
