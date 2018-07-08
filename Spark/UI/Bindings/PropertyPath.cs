namespace Spark.UI
{
    using System.Collections.ObjectModel;

    public sealed class PropertyPath
    {
        private const string SingleStepPath = "(0)";

        public PropertyPath(object parameter) 
            : this(SingleStepPath, parameter)
        {
        }

        public PropertyPath(string path, params object[] pathParameters)
        {
            Path = path;

            if (pathParameters != null)
            {
                PathParameters = new Collection<object>(pathParameters);
            }
        }
        
        public string Path { get; set; }

        public Collection<object> PathParameters { get; }
    }
}
