namespace Spark.UI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    using Resources;
    using Media;

    public static class Themes
    {
        static Themes()
        {
            RegisterDependencyProperties();
            GenericTheme = new ResourceDictionary();
            LoadComponent(GenericTheme, new Uri("/Spark;component/UI/Themes/Generic.xaml", UriKind.Relative));
        }
        
        internal static ResourceDictionary GenericTheme { get; }

        public static StreamResourceInfo GetResourceStream(Uri uriResource)
        {
            if (uriResource == null)
            {
                throw new ArgumentNullException(nameof(uriResource));
            }

            if (uriResource.OriginalString == null)
            {
                throw new ArgumentNullException(nameof(uriResource), "OriginalString is null.");
            }

            if (uriResource.IsAbsoluteUri)
            {
                if (uriResource.Scheme == "pack")
                {
                    throw new NotSupportedException("pack: resources not yet supported.");
                }

                throw new ArgumentException("uriResource is not relative and doesn't use the pack: scheme.");
            }

            PackUri pack = new PackUri(uriResource);
            string assemblyName = pack.Assembly;
            Assembly assembly = (assemblyName != null) ? Assembly.Load(assemblyName) : Assembly.GetEntryAssembly();
            string resourceName = assembly.GetName().Name + ".g";
            ResourceManager manager = new ResourceManager(resourceName, assembly);

            using (ResourceSet resourceSet = manager.GetResourceSet(CultureInfo.CurrentCulture, true, true))
            {
                Stream s = (Stream)resourceSet.GetObject(pack.GetAbsolutePath(), true);

                if (s == null)
                {
                    throw new IOException($"The requested resource could not be found: {uriResource.OriginalString}");
                }

                return new StreamResourceInfo(s, null);
            }
        }

        public static void LoadComponent(object component, Uri resourceLocator)
        {
            if (!resourceLocator.IsAbsoluteUri)
            {
                StreamResourceInfo sri = GetResourceStream(resourceLocator);
                XamlReader.Load(sri.Stream, component);
            }
            else
            {
                throw new ArgumentException("Cannot use absolute URI.");
            }
        }

        private static void RegisterDependencyProperties()
        {
            IEnumerable<Type> types = from type in Assembly.GetCallingAssembly().GetTypes()
                                      where typeof(DependencyObject).IsAssignableFrom(type)
                                      select type;

            BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

            foreach (Type type in types)
            {
                FieldInfo firstStaticField = type.GetFields(flags).FirstOrDefault();

                if (firstStaticField != null)
                {
                    object o = firstStaticField.GetValue(null);
                }
            }
        }
    }
}
