namespace Spark.Graphics
{
    using System.Collections;
    using System.Collections.Generic;

    using Content;

    /// <summary>
    /// Read only collection for render properties
    /// </summary>
    public sealed class RenderPropertyCollection : IReadOnlyDictionary<RenderPropertyId, RenderProperty>, ISavable
    {
        private readonly Dictionary<RenderPropertyId, RenderProperty> _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPropertyCollection"/> class.
        /// </summary>
        public RenderPropertyCollection() 
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPropertyCollection"/> class.
        /// </summary>
        /// <param name="initialCapacity">Initial capacity of the collection</param>
        public RenderPropertyCollection(int initialCapacity)
        {
            _properties = new Dictionary<RenderPropertyId, RenderProperty>(initialCapacity, new RenderPropertyIdEqualityComparer());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPropertyCollection"/> class.
        /// </summary>
        /// <param name="properties">Initial enumeration of properties to copy into the collection</param>
        public RenderPropertyCollection(IEnumerable<RenderProperty> properties)
        {
            _properties = new Dictionary<RenderPropertyId, RenderProperty>(new RenderPropertyIdEqualityComparer());

            if (properties != null)
            {
                foreach (RenderProperty prop in properties)
                {
                    if (prop != null)
                    {
                        _properties.Add(prop.Id, prop);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of render properties in the collection
        /// </summary>
        public int Count => _properties.Count;

        /// <summary>
        /// Gets the keys contained in the property collection
        /// </summary>
        public IEnumerable<RenderPropertyId> Keys => _properties.Keys;

        /// <summary>
        /// Gets the values contained in the property collection
        /// </summary>
        public IEnumerable<RenderProperty> Values => _properties.Values;

        /// <summary>
        /// Gets or sets a render property based on its id
        /// </summary>
        /// <param name="key">Render property id</param>
        /// <returns>Render property with the given id</returns>
        public RenderProperty this[RenderPropertyId key]
        {
            get
            {
                if (key == RenderPropertyId.Invalid)
                {
                    return null;
                }

                return _properties[key];
            }
            set
            {
                if (key == RenderPropertyId.Invalid || value == null)
                {
                    return;
                }

                _properties[key] = value;
            }
        }

        /// <summary>
        /// Adds a render property to the collection
        /// </summary>
        /// <param name="property">Render property to add</param>
        /// <returns>True if the property was added, false otherwise</returns>
        public bool Add(RenderProperty property)
        {
            if (property == null || property.Id == RenderPropertyId.Invalid)
            {
                return false;
            }

            _properties.Add(property.Id, property);

            return true;
        }

        /// <summary>
        /// Removes a render property by its id
        /// </summary>
        /// <param name="propId">Render property id</param>
        /// <returns>True if the property was removed, false otherwise</returns>
        public bool Remove(RenderPropertyId propId)
        {
            if (propId == RenderPropertyId.Invalid)
            {
                return false;
            }

            return _properties.Remove(propId);
        }

        /// <summary>
        /// Determines if a render property with the given id is contained within the collection
        /// </summary>
        /// <param name="key">Render property id</param>
        /// <returns>True if a render property with the given key is contained within the collection</returns>
        public bool ContainsKey(RenderPropertyId key)
        {
            if (key == RenderPropertyId.Invalid)
            {
                return false;
            }

            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// Attempts to get the render property with the given key 
        /// </summary>
        /// <param name="key">Render property key</param>
        /// <param name="value">Render property with the given id</param>
        /// <returns>True if a render property was found with the given id, false otherwise</returns>
        public bool TryGetValue(RenderPropertyId key, out RenderProperty value)
        {
            if (key == RenderPropertyId.Invalid)
            {
                value = null;
                return false;
            }

            return _properties.TryGetValue(key, out value);
        }

        /// <summary>
        /// Tries to get a render property of the given type
        /// </summary>
        /// <typeparam name="T">Type of render property to get</typeparam>
        /// <param name="value">Render property</param>
        /// <returns>True if a render property was found</returns>
        public bool TryGet<T>(out T value) where T : RenderProperty
        {
            RenderPropertyId key = RenderProperty.GetPropertyId<T>();
            value = null;

            if (key == RenderPropertyId.Invalid)
            {
                return false;
            }
            
            bool foundIt = _properties.TryGetValue(key, out RenderProperty prop);
            value = prop as T;

            return foundIt;
        }

        /// <summary>
        /// Clears all render properties from the collection
        /// </summary>
        public void Clear()
        {
            _properties.Clear();
        }

        /// <summary>
        /// Creates a copy of the render property collection
        /// </summary>
        /// <returns></returns>
        public RenderPropertyCollection Clone()
        {
            int canCloneCount = GetCanCloneCount();
            RenderPropertyCollection clone = new RenderPropertyCollection(canCloneCount);

            if (canCloneCount > 0)
            {
                foreach (KeyValuePair<RenderPropertyId, RenderProperty> kv in _properties)
                {
                    RenderProperty oldProp = kv.Value;
                    if (oldProp.CanClone)
                    {
                        RenderProperty newProp = oldProp.Clone();
                        if (newProp != null)
                        {
                            clone._properties.Add(newProp.Id, newProp);
                        }
                    }
                }
            }

            return clone;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="Dictionary{RenderPropertyId,RenderProperty}.Enumerator" /> that can be used to iterate through the collection.</returns>
        public Dictionary<RenderPropertyId, RenderProperty>.Enumerator GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{KeyValuePair{RenderPropertyId,RenderProperty}}" /> that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<RenderPropertyId, RenderProperty>> IEnumerable<KeyValuePair<RenderPropertyId, RenderProperty>>.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator" /> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            int count = input.ReadInt32();
            if (count > 0)
            {
                ISavable[] savables = input.ReadSavableArray<ISavable>();
                for (int i = 0; i < savables.Length; i++)
                {
                    RenderProperty prop = savables[i] as RenderProperty;
                    if (prop != null)
                    {
                        _properties.Add(prop.Id, prop);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            int savableCount = GetSavableCount();

            output.Write("Count", savableCount);

            if (savableCount > 0)
            {
                ISavable[] savables = new ISavable[savableCount];
                int index = 0;
                foreach (KeyValuePair<RenderPropertyId, RenderProperty> kv in _properties)
                {
                    if (kv.Value is ISavable)
                    {
                        savables[index] = kv.Value as ISavable;

                        index++;
                    }
                }

                output.WriteSavable("Properties", savables);
            }
        }

        /// <summary>
        /// Gets the number of <see cref="ISavable"/> values in the collection
        /// </summary>
        /// <returns>Number of <see cref="ISavable"/> values in the collection</returns>
        private int GetSavableCount()
        {
            int numSavable = 0;
            foreach (KeyValuePair<RenderPropertyId, RenderProperty> kv in _properties)
            {
                if (kv.Value is ISavable)
                {
                    numSavable++;
                }
            }

            return numSavable;
        }

        /// <summary>
        /// Gets the number of properties in the collection that can be cloned
        /// </summary>
        /// <returns>Number of properties in the collection that can be cloned</returns>
        private int GetCanCloneCount()
        {
            int cloneCount = 0;
            foreach (KeyValuePair<RenderPropertyId, RenderProperty> kv in _properties)
            {
                if (kv.Value.CanClone)
                {
                    cloneCount++;
                }
            }

            return cloneCount;
        }
    }
}
