namespace Spark.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a unique value for identifying components.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct ComponentTypeId : IEquatable<ComponentTypeId>, IComparable<ComponentTypeId>
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="ComponentTypeId"/> struct.
        /// </summary>
        /// <param name="idValue">The integer Type ID value.</param>
        internal ComponentTypeId(int idValue)
        {
            Value = idValue;
        }

        /// <summary>
        /// Gets the null type ID value.
        /// </summary>
        public static ComponentTypeId NullTypeId => new ComponentTypeId(0);

        /// <summary>
        /// Gets the integer value of the Type ID.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets the <see cref="ComponentTypeId"/> associated with the <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        /// <returns>Type id of component.</returns>
        public static ComponentTypeId GetTypeId<T>()
        {
            return ComponentTypeIdHolder<T>.TypeId;
        }

        /// <summary>
        /// Gets the <see cref="ComponentTypeId"/> associated with the <see cref="Type"/>.
        /// </summary>
        /// <param name="typeOf">Type of component.</param>
        /// <returns>Type id of component.</returns>
        public static ComponentTypeId GetTypeId(Type typeOf)
        {
            if (typeOf == null)
            {
                return NullTypeId;
            }

            return ComponentTypeIdGenerator.GetId(typeOf);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> associated with the <see cref="ComponentTypeId"/>.
        /// </summary>
        /// <param name="typeId">Type id of component.</param>
        /// <returns>Type of component.</returns>
        public static Type GetTypeOf(ComponentTypeId typeId)
        {
            if (typeId == NullTypeId)
            {
                return typeof(NullComponent);
            }

            return ComponentTypeIdGenerator.GetType(typeId);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Value;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>True</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ComponentTypeId)
            {
                return ((ComponentTypeId)obj).Value == Value;
            }

            return false;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.</returns>
        public int CompareTo(ComponentTypeId other)
        {
            if (Value < other.Value)
            {
                return -1;
            }

            if (Value > other.Value)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ComponentTypeId other)
        {
            return other.Value == Value;
        }

        /// <summary>
        /// Implicitly converts the Type id to an integer value.
        /// </summary>
        /// <param name="id">Type id.</param>
        /// <returns>Integer value.</returns>
        public static implicit operator int(ComponentTypeId id)
        {
            return id.Value;
        }

        /// <summary>
        /// Implicitly converts the integer value to a Type id.
        /// </summary>
        /// <param name="idValue">Integer value.</param>
        /// <returns>Render property id.</returns>
        public static implicit operator ComponentTypeId(int idValue)
        {
            return new ComponentTypeId(idValue);
        }

        /// <summary>
        /// Checks inequality between two Type ids.
        /// </summary>
        /// <param name="a">First id.</param>
        /// <param name="b">Second id.</param>
        /// <returns>True if the two values are not the same, false otherwise.</returns>
        public static bool operator !=(ComponentTypeId a, ComponentTypeId b)
        {
            return a.Value != b.Value;
        }

        /// <summary>
        /// Checks equality between two Type IDs.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the two values are the same, false otherwise.</returns>
        public static bool operator ==(ComponentTypeId a, ComponentTypeId b)
        {
            return a.Value == b.Value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return ToString();
            }

            return Value.ToString(formatProvider);
        }
    }

    /// <summary>
    /// Holder of a component id value
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    internal static class ComponentTypeIdHolder<T>
    {
        /// <summary>
        /// Component type id value
        /// </summary>
        public static readonly ComponentTypeId TypeId;

        /// <summary>
        /// Static initializer of the <see cref="ComponentTypeIdHolder{T}"/> class
        /// </summary>
        static ComponentTypeIdHolder()
        {
            TypeId = ComponentTypeIdGenerator.GetId(typeof(T));
        }
    }

    /// <summary>
    /// Generates component ids
    /// </summary>
    internal static class ComponentTypeIdGenerator
    {
        private const uint DefaultPolynomial = 0xedb88320;
        private const uint DefaultSeed = 0xffffffff;

        private static Dictionary<Type, ComponentTypeId> _typesToIds = new Dictionary<Type, ComponentTypeId>();
        private static Dictionary<ComponentTypeId, Type> _idsToTypes = new Dictionary<ComponentTypeId, Type>();
        private static uint[] _defaultTable;

        /// <summary>
        /// Static initializer of the <see cref="ComponentTypeIdGenerator"/> class
        /// </summary>
        static ComponentTypeIdGenerator()
        {
            _defaultTable = InitializeTable(DefaultPolynomial);

            _typesToIds = new Dictionary<Type, ComponentTypeId>();
            _idsToTypes = new Dictionary<ComponentTypeId, Type>();
        }

        /// <summary>
        /// Initializes the generation table
        /// </summary>
        /// <param name="polynomial">Default polynomial value</param>
        /// <returns>Generated table</returns>
        private static uint[] InitializeTable(uint polynomial)
        {
            var createTable = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                var entry = (uint)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((entry & 1) == 1)
                    {
                        entry = (entry >> 1) ^ polynomial;
                    }
                    else
                    {
                        entry >>= 1;
                    }
                }

                createTable[i] = entry;
            }

            return createTable;
        }

        /// <summary>
        /// CRC-32 algorithm to generate a consistent hash for a type. The hash is generated based on the assembly qualified name
        /// </summary>
        /// <param name="type">Component type</param>
        /// <returns>Type id for the given component type</returns>
        public static ComponentTypeId GenerateId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            string name = type.AssemblyQualifiedName;

            uint crc = DefaultSeed;
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];

                var lower = (byte)c;
                var upper = (byte)(c << 8);

                crc = (crc >> 8) ^ _defaultTable[lower ^ crc & 0xff];
                crc = (crc >> 8) ^ _defaultTable[upper ^ crc & 0xff];
            }

            var typeId = new ComponentTypeId((int)~crc);

            System.Diagnostics.Debug.Assert(typeId != ComponentTypeId.NullTypeId);
            return typeId;
        }

        /// <summary>
        /// Gets the type id for a given component type
        /// </summary>
        /// <param name="type">Component type</param>
        /// <returns>Type id for the given component type</returns>
        public static ComponentTypeId GetId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_typesToIds.TryGetValue(type, out ComponentTypeId typeId))
            {
                return typeId;
            }

            lock (_typesToIds)
            {
                typeId = (type == typeof(NullComponent)) ? ComponentTypeId.NullTypeId : GenerateId(type);
                _typesToIds.Add(type, typeId);
                _idsToTypes.Add(typeId, type);

                return typeId;
            }
        }

        /// <summary>
        /// Gets the type associated with a given component type
        /// </summary>
        /// <param name="typeId">Component type id</param>
        /// <returns>Type of the component represented by the given type id</returns>
        public static Type GetType(ComponentTypeId typeId)
        {
            if (typeId == ComponentTypeId.NullTypeId)
            {
                return typeof(NullComponent);
            }
            
            if (_idsToTypes.TryGetValue(typeId, out Type type))
            {
                return type;
            }

            return null;
        }
    }
}
