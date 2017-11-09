namespace Spark.UI
{
    using System;
    using System.Threading;
    using System.Collections.Generic;

    public sealed class DependencyProperty
    {
        public static readonly object UnsetValue = new object();

        private static int _indexCounter = 0;
        private static List<DependencyProperty> _registeredProperties = new List<DependencyProperty>();
        private static List<DependencyProperty> _attachedProperties = new List<DependencyProperty>();
        
        private DependencyProperty(int globalIndex, string propertyName, Type propertyType, Type ownerType, PropertyMetadata metadata, bool isReadOnly)
        {
            GlobalIndex = globalIndex;
            Name = propertyName;
            PropertyType = propertyType;
            OwnerType = ownerType;
            DefaultMetadata = metadata;
            ReadOnly = isReadOnly;
        }

        public int GlobalIndex { get; private set; }
        
        public string Name { get; private set; }

        public Type PropertyType { get; private set; }

        public Type OwnerType { get; private set; }

        public PropertyMetadata DefaultMetadata { get; private set; }
        
        public bool ReadOnly { get; private set; }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, null);
        }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            DependencyProperty prop = new DependencyProperty(GetNextGlobalIndex(), name, propertyType, ownerType, metadata, false);
            _registeredProperties.Add(prop);
            return prop;
        }

        public static DependencyPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            DependencyProperty prop = new DependencyProperty(GetNextGlobalIndex(), name, propertyType, ownerType, metadata, true);
            _registeredProperties.Add(prop);
            return new DependencyPropertyKey(prop);
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType)
        {
            return RegisterAttached(name, propertyType, ownerType, null);
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            DependencyProperty prop = new DependencyProperty(GetNextGlobalIndex(), name, propertyType, ownerType, metadata, false);
            _attachedProperties.Add(prop);
            return prop;
        }

        public static DependencyProperty RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            DependencyProperty prop = new DependencyProperty(GetNextGlobalIndex(), name, propertyType, ownerType, metadata, true);
            _attachedProperties.Add(prop);
            return prop;
        }

        internal static bool IsRegisteredProperty(string propertyName, Type ownerType)
        {
            foreach (DependencyProperty prop in _registeredProperties)
            {
                if (prop.OwnerType == ownerType && prop.Name == propertyName)
                {
                    return true;
                }
            }

            return false;
        }
        
        private static int GetNextGlobalIndex()
        {
            return Interlocked.Increment(ref _indexCounter);
        }
    }
}
