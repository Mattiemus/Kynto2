namespace Spark.UI
{
    using System.Collections.ObjectModel;

    public sealed class PropertyPath
    {
        public PropertyPath(string path)
        {
            Path = path;
            PathParameters = new Collection<object>();
        }

        public PropertyPath(string path, params object[] pathParameters)
        {
            Path = path;
            PathParameters = new Collection<object>(pathParameters);
        }

        public string Path { get; }

        public Collection<object> PathParameters { get; }
    }
}
