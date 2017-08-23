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
    }
}
