namespace Spark.UI
{
    using System;
    using System.Collections.Generic;

    public sealed class DependencyProperty
    {
        public static readonly object UnsetValue = new object();

        private static List<DependencyProperty> RegisteredProperties = new List<DependencyProperty>();
        private static List<DependencyProperty> AttachedProperties = new List<DependencyProperty>();
        
        private DependencyProperty(string propertyName, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            Name = propertyName;
            PropertyType = propertyType;
            OwnerType = ownerType;
            DefaultMetadata = metadata;
        }
        
        public string Name { get; private set; }

        public Type PropertyType { get; private set; }

        public Type OwnerType { get; private set; }

        public PropertyMetadata DefaultMetadata { get; private set; }
        
        public static DependencyProperty Register(string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, null);
        }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            DependencyProperty prop = new DependencyProperty(name, propertyType, ownerType, metadata);
            RegisteredProperties.Add(prop);
            return prop;
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType)
        {
            return RegisterAttached(name, propertyType, ownerType, null);
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            DependencyProperty prop = new DependencyProperty(name, propertyType, ownerType, metadata);
            AttachedProperties.Add(prop);
            return prop;
        }

        internal static bool IsRegisteredProperty(string propertyName, Type ownerType)
        {
            foreach (DependencyProperty prop in RegisteredProperties)
            {
                if (prop.OwnerType == ownerType && prop.Name == propertyName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
