namespace Spark.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public delegate bool ValidateValueCallback(object value);

    [TypeConverter(typeof(DependencyPropertyConverter))]
    public sealed class DependencyProperty
    {
        public static readonly object UnsetValue = new object();

        private Dictionary<Type, PropertyMetadata> _metadataByType = new Dictionary<Type, PropertyMetadata>();

        private DependencyProperty(
            bool isAttached,
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            if (defaultMetadata == null)
            {
                throw new ArgumentNullException(nameof(defaultMetadata));
            }

            IsAttached = isAttached;
            DefaultMetadata = defaultMetadata;
            Name = name;
            OwnerType = ownerType;
            PropertyType = propertyType;
            ValidateValueCallback = validateValueCallback;
        }

        public bool ReadOnly { get; private set; }

        public PropertyMetadata DefaultMetadata { get; }

        public string Name { get; }

        public Type OwnerType { get; }

        public Type PropertyType { get; }

        public ValidateValueCallback ValidateValueCallback { get; }

        public int GlobalIndex
        {
            get { throw new NotImplementedException(); }
        }

        internal bool IsAttached { get; set; }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, null, null);
        }

        public static DependencyProperty Register(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata)
        {
            return Register(name, propertyType, ownerType, typeMetadata, null);
        }

        public static DependencyProperty Register(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            PropertyMetadata defaultMetadata;

            if (typeMetadata == null)
            {
                defaultMetadata = typeMetadata = new PropertyMetadata();
            }
            else
            {
                defaultMetadata = new PropertyMetadata(typeMetadata.DefaultValue);
            }

            DependencyProperty dp = new DependencyProperty(
                false,
                name,
                propertyType,
                ownerType,
                defaultMetadata,
                validateValueCallback);

            DependencyObject.Register(ownerType, dp);

            dp.OverrideMetadata(ownerType, typeMetadata);

            return dp;
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType)
        {
            return RegisterAttached(name, propertyType, ownerType, null, null);
        }

        public static DependencyProperty RegisterAttached(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata)
        {
            return RegisterAttached(name, propertyType, ownerType, defaultMetadata, null);
        }

        public static DependencyProperty RegisterAttached(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            if (defaultMetadata == null)
            {
                defaultMetadata = new PropertyMetadata();
            }

            DependencyProperty dp = new DependencyProperty(
                true,
                name,
                propertyType,
                ownerType,
                defaultMetadata,
                validateValueCallback);

            DependencyObject.Register(ownerType, dp);
            return dp;
        }

        public static DependencyPropertyKey RegisterAttachedReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata)
        {
            throw new NotImplementedException();
        }

        public static DependencyPropertyKey RegisterAttachedReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            throw new NotImplementedException();
        }

        public static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata)
        {
            return RegisterReadOnly(name, propertyType, ownerType, typeMetadata, null);
        }

        public static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            DependencyProperty prop = Register(name, propertyType, ownerType, typeMetadata, validateValueCallback);
            prop.ReadOnly = true;
            return new DependencyPropertyKey(prop);
        }

        public DependencyProperty AddOwner(Type ownerType)
        {
            return AddOwner(ownerType, null);
        }

        public DependencyProperty AddOwner(Type ownerType, PropertyMetadata typeMetadata)
        {
            if (typeMetadata == null)
            {
                typeMetadata = new PropertyMetadata();
            }

            OverrideMetadata(ownerType, typeMetadata);
            DependencyObject.Register(ownerType, this);

            // MS seems to always return the same DependencyProperty
            return this;
        }

        public PropertyMetadata GetMetadata(Type forType)
        {
            Type type = forType;

            while (type != null)
            {
                if (_metadataByType.TryGetValue(type, out PropertyMetadata result))
                {
                    return result;
                }

                type = type.BaseType;
            }

            return DefaultMetadata;
        }

        public PropertyMetadata GetMetadata(DependencyObject dependencyObject)
        {
            return GetMetadata(dependencyObject.GetType());
        }

        public PropertyMetadata GetMetadata(DependencyObjectType dependencyObjectType)
        {
            return GetMetadata(dependencyObjectType.SystemType);
        }

        public bool IsValidType(object value)
        {
            if (value == UnsetValue)
            {
                return true;
            }

            if (value == null)
            {
                return !PropertyType.IsValueType || Nullable.GetUnderlyingType(PropertyType) != null;
            }

            return PropertyType.IsInstanceOfType(value);
        }

        public bool IsValidValue(object value)
        {
            if (value == UnsetValue)
            {
                return true;
            }

            if (!IsValidType(value))
            {
                return false;
            }

            if (ValidateValueCallback == null)
            {
                return true;
            }

            return ValidateValueCallback(value);
        }

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
        {
            if (forType == null)
            {
                throw new ArgumentNullException(nameof(forType));
            }

            if (typeMetadata == null)
            {
                throw new ArgumentNullException(nameof(typeMetadata));
            }

            if (ReadOnly)
            {
                throw new InvalidOperationException($"Cannot override metadata on readonly property '{Name}' without using a DependencyPropertyKey");
            }

            typeMetadata.Merge(DefaultMetadata, this, forType);
            _metadataByType.Add(forType, typeMetadata);
        }

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata, DependencyPropertyKey key)
        {
            if (forType == null)
            {
                throw new ArgumentNullException(nameof(forType));
            }

            if (typeMetadata == null)
            {
                throw new ArgumentNullException(nameof(typeMetadata));
            }

            // TODO: further checking? should we check key.DependencyProperty == this?
            typeMetadata.Merge(DefaultMetadata, this, forType);
            _metadataByType.Add(forType, typeMetadata);
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ PropertyType.GetHashCode() ^ OwnerType.GetHashCode();
        }
    }
}
