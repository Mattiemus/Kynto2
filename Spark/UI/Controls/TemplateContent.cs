namespace Spark.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Xaml;

    public class TemplateContent
    {
        private readonly XamlNodeList _nodeList;
        private readonly Dictionary<string, Type> _typesByName;

        internal TemplateContent(XamlNodeList nodeList, Dictionary<string, Type> typesByName)
        {
            _nodeList = nodeList;
            _typesByName = typesByName;
        }

        internal Type GetTypeForName(string name)
        {
            if (!_typesByName.TryGetValue(name, out Type result))
            {
                throw new KeyNotFoundException($"Element '{name}' not found in TemplateContent.");
            }

            return result;
        }

        internal object Load()
        {
            return Media.XamlReader.Load(_nodeList.GetReader());
        }
    }
}
