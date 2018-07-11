namespace Spark.UI
{
    using System;

    public class ResourceReferenceKeyNotFoundException : Exception
    {
        public ResourceReferenceKeyNotFoundException(string message, object key)
            : base(message)
        {
            Key = key;
        }

        public object Key { get; private set; }
    }
}
