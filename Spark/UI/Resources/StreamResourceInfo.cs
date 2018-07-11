namespace Spark.UI.Resources
{
    using System.IO;

    public class StreamResourceInfo
    {
        public StreamResourceInfo()
        {
        }

        public StreamResourceInfo(Stream stream, string contentType)
        {
            ContentType = contentType;
            Stream = stream;
        }

        public string ContentType { get; private set; }

        public Stream Stream { get; private set; }
    }
}
