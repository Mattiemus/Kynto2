namespace Spark.UI.Content
{
    using System.IO;
    using Spark.Content;

    using Media;

    public sealed class XamlResourceImporter : ResourceImporter<object>
    {
        public XamlResourceImporter()
            : base(".xaml")
        {
        }

        public override object Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters)
        {
            using (Stream str = resourceFile.OpenRead())
            {
                return LoadInternal(str, contentManager, parameters, resourceFile.Name);
            }
        }

        public override object Load(Stream input, ContentManager contentManager, ImporterParameters parameters)
        {
            return LoadInternal(input, contentManager, parameters, "Interface");
        }

        private object LoadInternal(Stream input, ContentManager contentManager, ImporterParameters parameters, string fileName)
        {
            return XamlReader.Load(input);
        }
    }
}
