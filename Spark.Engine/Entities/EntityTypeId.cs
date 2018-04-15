namespace Spark.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a unique value for identifying entities.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct EntityTypeId : IEquatable<EntityTypeId>, IComparable<EntityTypeId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTypeId"/> struct.
        /// </summary>
        /// <param name="idValue">The integer Type ID value.</param>
        internal EntityTypeId(int idValue)
        {
            Value = idValue;
        }
        
        /// <summary>
        /// Gets the integer value of the Type ID.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets the <see cref="EntityTypeId"/> associated with the <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <returns>Type id of entity.</returns>
        public static EntityTypeId GetTypeId<T>()
        {
            return EntityTypeIdHolder<T>.TypeId;
        }

        /// <summary>
        /// Gets the <see cref="EntityTypeId"/> associated with the <see cref="Type"/>.
        /// </summary>
        /// <param name="typeOf">Type of entity.</param>
        /// <returns>Type id of entity.</returns>
        public static EntityTypeId GetTypeId(Type typeOf)
        {
            if (typeOf == null)
            {
                throw new ArgumentNullException(nameof(typeOf));
            }

            return EntityTypeIdGenerator.GetId(typeOf);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> associated with the <see cref="EntityTypeId"/>.
        /// </summary>
        /// <param name="typeId">Type id of entity.</param>
        /// <returns>Type of entity.</returns>
        public static Type GetTypeOf(EntityTypeId typeId)
        {
            return EntityTypeIdGenerator.GetType(typeId);
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
            if (obj is EntityTypeId)
            {
                return ((EntityTypeId)obj).Value == Value;
            }

            return false;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.</returns>
        public int CompareTo(EntityTypeId other)
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
        public bool Equals(EntityTypeId other)
        {
            return other.Value == Value;
        }

        /// <summary>
        /// Implicitly converts the Type id to an integer value.
        /// </summary>
        /// <param name="id">Type id.</param>
        /// <returns>Integer value.</returns>
        public static implicit operator int(EntityTypeId id)
        {
            return id.Value;
        }

        /// <summary>
        /// Implicitly converts the integer value to a Type id.
        /// </summary>
        /// <param name="idValue">Integer value.</param>
        /// <returns>Render property id.</returns>
        public static implicit operator EntityTypeId(int idValue)
        {
            return new EntityTypeId(idValue);
        }

        /// <summary>
        /// Checks inequality between two Type ids.
        /// </summary>
        /// <param name="a">First id.</param>
        /// <param name="b">Second id.</param>
        /// <returns>True if the two values are not the same, false otherwise.</returns>
        public static bool operator !=(EntityTypeId a, EntityTypeId b)
        {
            return a.Value != b.Value;
        }

        /// <summary>
        /// Checks equality between two Type IDs.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the two values are the same, false otherwise.</returns>
        public static bool operator ==(EntityTypeId a, EntityTypeId b)
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
    /// Holder of a entity id value
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    internal static class EntityTypeIdHolder<T>
    {
        /// <summary>
        /// Component type id value
        /// </summary>
        public static readonly EntityTypeId TypeId;

        /// <summary>
        /// Static initializer of the <see cref="EntityTypeIdHolder{T}"/> class
        /// </summary>
        static EntityTypeIdHolder()
        {
            TypeId = EntityTypeIdGenerator.GetId(typeof(T));
        }
    }

    /// <summary>
    /// Generates entity ids
    /// </summary>
    internal static class EntityTypeIdGenerator
    {
        private const uint DefaultPolynomial = 0xedb88320;
        private const uint DefaultSeed = 0xffffffff;

        private static Dictionary<Type, EntityTypeId> _typesToIds = new Dictionary<Type, EntityTypeId>();
        private static Dictionary<EntityTypeId, Type> _idsToTypes = new Dictionary<EntityTypeId, Type>();
        private static uint[] _defaultTable;

        /// <summary>
        /// Static initializer of the <see cref="EntityTypeIdGenerator"/> class
        /// </summary>
        static EntityTypeIdGenerator()
        {
            _defaultTable = InitializeTable(DefaultPolynomial);

            _typesToIds = new Dictionary<Type, EntityTypeId>();
            _idsToTypes = new Dictionary<EntityTypeId, Type>();
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
        /// <returns>Type id for the given entity type</returns>
        public static EntityTypeId GenerateId(Type type)
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

            var typeId = new EntityTypeId((int)~crc);
            
            return typeId;
        }

        /// <summary>
        /// Gets the type id for a given entity type
        /// </summary>
        /// <param name="type">Component type</param>
        /// <returns>Type id for the given entity type</returns>
        public static EntityTypeId GetId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_typesToIds.TryGetValue(type, out EntityTypeId typeId))
            {
                return typeId;
            }

            lock (_typesToIds)
            {
                typeId = GenerateId(type);
                _typesToIds.Add(type, typeId);
                _idsToTypes.Add(typeId, type);

                return typeId;
            }
        }

        /// <summary>
        /// Gets the type associated with a given entity type
        /// </summary>
        /// <param name="typeId">Component type id</param>
        /// <returns>Type of the entity represented by the given type id</returns>
        public static Type GetType(EntityTypeId typeId)
        {            
            if (_idsToTypes.TryGetValue(typeId, out Type type))
            {
                return type;
            }

            return null;
        }
    }
}
