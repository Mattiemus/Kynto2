namespace Spark.UI.Content
{
    using System.IO;
    using Spark.Content;

    using Media;

    public sealed class XamlResourceDictionaryResourceImporter : BaseXamlResourceImporter<ResourceDictionary>
    {
    }

    public sealed class XamUIElementResourceImporter : BaseXamlResourceImporter<UIElement>
    {
    }

    public abstract class BaseXamlResourceImporter<T> : ResourceImporter<T> where T : class
    {
        protected BaseXamlResourceImporter()
            : base(".xaml")
        {
        }

        public override T Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters)
        {
            using (Stream str = resourceFile.OpenRead())
            {
                return LoadInternal(str, contentManager, parameters, resourceFile.Name);
            }
        }

        public override T Load(Stream input, ContentManager contentManager, ImporterParameters parameters)
        {
            return LoadInternal(input, contentManager, parameters, "Interface");
        }

        private T LoadInternal(Stream input, ContentManager contentManager, ImporterParameters parameters, string fileName)
        {
            return (T)XamlReader.Load(input);
        }
    }
}
