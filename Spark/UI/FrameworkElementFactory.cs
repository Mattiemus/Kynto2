namespace Spark.UI
{
    using System;

    public class FrameworkElementFactory
    {
        public FrameworkElementFactory()
        {
        }

        public FrameworkElementFactory(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }

        internal FrameworkElement Load()
        {
            if (Type != null)
            {
                if (typeof(FrameworkElement).IsAssignableFrom(Type))
                {
                    return (FrameworkElement)Activator.CreateInstance(Type);
                }

                throw new InvalidOperationException("FrameworkElementFactory.Type must be a FrameworkElement.");
            }

            throw new InvalidOperationException("FrameworkElementFactory.Type not set.");
        }
    }
}
