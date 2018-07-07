namespace Spark.UI
{
    using System;
    using System.Collections.Generic;

    public sealed class DependencyProperty
    {
        public static readonly object UnsetValue = new object();

        private static readonly List<DependencyProperty> RegisteredProperties;
        private static readonly List<DependencyProperty> AttachedProperties;

        private DependencyProperty(string propertyName, Type propertyType, Type ownerType)
        {
            Name = propertyName;
            PropertyType = propertyType;
            OwnerType = ownerType;
        }

        private DependencyProperty(string propertyName, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            Name = propertyName;
            PropertyType = propertyType;
            OwnerType = ownerType;
            DefaultMetadata = metadata;
        }

        static DependencyProperty()
        {
            RegisteredProperties = new List<DependencyProperty>();
            AttachedProperties = new List<DependencyProperty>();
        }

        public string Name { get; }

        public Type PropertyType { get; }

        public Type OwnerType { get; }

        public PropertyMetadata DefaultMetadata { get; }
        
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
            foreach (var prop in RegisteredProperties)
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
