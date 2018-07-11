namespace Spark.UI.Resources
{
    using System;

    public class PackUri
    {
        private readonly Uri _uri;

        public PackUri(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                throw new NotSupportedException("Absolute pack URIs not yet supported.");
            }

            _uri = uri;
        }

        public string Assembly
        {
            get
            {
                string s = _uri.OriginalString;
                int component = s.IndexOf(";component", StringComparison.InvariantCulture);

                if (component == -1)
                {
                    return null;
                }

                if (s.StartsWith("/", StringComparison.InvariantCulture))
                {
                    return s.Substring(1, component - 1);
                }

                throw new InvalidOperationException("Not a valid pack URI.");
            }
        }

        public string Path
        {
            get
            {
                string s = _uri.OriginalString;
                int component = s.IndexOf(";component", StringComparison.InvariantCulture);

                if (component == -1)
                {
                    return s;
                }

                if (s.StartsWith("/", StringComparison.InvariantCulture))
                {
                    return s.Substring(component + 10);
                }

                throw new InvalidOperationException("Not a valid pack URI.");
            }
        }

        public string GetAbsolutePath()
        {
            string path = Path;
            if (path.StartsWith("/", StringComparison.InvariantCulture))
            {
                return path.Substring(1);
            }

            return path;
        }
    }
}
