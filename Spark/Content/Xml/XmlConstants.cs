namespace Spark.Content.Xml
{
    /// <summary>
    /// Static xml reader and writer constants
    /// </summary>
    public static class XmlConstants
    {
        /// <summary>
        /// Savable version number
        /// </summary>
        public static readonly short VERSION_NUMBER = 8;

        /// <summary>
        /// Name given to a shared resource section of the xml document
        /// </summary>
        public static readonly string BASE_SHAREDRESOURCE_NAME = "#SharedResource";

        /// <summary>
        /// Name given to a shared resource section of the xml document
        /// </summary>
        public static readonly string BASE_EXTERNALREFERENCE_NAME = "#ExternalReference";
    }
}
