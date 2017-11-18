namespace Spark.Graphics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    using Math;
    using Content;

    /// <summary>
    /// Collection of lights which has some global light properties such as ambient color and can let shaders know if data has changed.
    /// </summary>
    public class LightCollection : IList<Light>, IReadOnlyList<Light>, ISavable
    {        
        private List<Light> _lights;
        private Color _globalAmbient;
        private bool _updateShader;
        private bool _isEnabled;
        private bool _monitorLightChanges;
        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightCollection"/> class.
        /// </summary>
        public LightCollection() 
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightCollection"/> class.
        /// </summary>
        /// <param name="capacity">Initial capacity of the collection.</param>
        public LightCollection(int capacity)
        {
            _globalAmbient = Color.White;
            _monitorLightChanges = false;
            _updateShader = true;
            _isEnabled = true;
            _lights = new List<Light>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightCollection"/> class.
        /// </summary>
        /// <param name="collection">Collection of lights to copy.</param>
        public LightCollection(IEnumerable<Light> collection)
        {
            _globalAmbient = Color.White;
            _monitorLightChanges = false;
            _updateShader = true;
            _isEnabled = true;
            _lights = new List<Light>(collection);
        }

        /// <summary>
        /// Occurs when lights are added, removed, or cleared from the collection.
        /// </summary>
        public event TypedEventHandler<LightCollection> CollectionModified;

        /// <summary>
        /// Gets or sets the light at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public Light this[int index]
        {
            get
            {
                if (index < 0 || index >= _lights.Count)
                {
                    return null;
                }

                return _lights[index];
            }
            set
            {
                if (index < 0 || index >= _lights.Count || value == null)
                {
                    return;
                }

                if (_monitorLightChanges)
                {
                    value.LightChanged += OnLightChanged;
                    Light oldLight = _lights[index];
                    if (oldLight != null)
                    {
                        oldLight.LightChanged -= OnLightChanged;
                    }
                }

                _lights[index] = value;
                _version++;
            }
        }

        /// <summary>
        /// Gets the number of lights contained in the collection.
        /// </summary>
        public int Count => _lights.Count;

        /// <summary>
        /// Gets or sets the global ambient color.
        /// </summary>
        public Color GlobalAmbient
        {
            get => _globalAmbient;
            set
            {
                _globalAmbient = value;
                _updateShader = true;
            }
        }

        /// <summary>
        /// Gets or sets if the light collection has changed in any way that may interest the shader.
        /// </summary>
        public bool UpdateShader
        {
            get => _updateShader;
            set
            {
                _updateShader = value;
            }
        }

        /// <summary>
        /// Gets or sets if the light collection is enabled. This is a hint to the shader that no data should be uploaded.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                _updateShader = true;
            }
        }

        /// <summary>
        /// Gets or sets if the collection should monitor lights contained in it for when they change. If set to true, then when light properties
        /// change, the collection's <see cref="UpdateShader"/> property is set to true.
        /// </summary>
        public bool MonitorLightChanges
        {
            get => _monitorLightChanges;
            set
            {
                if (_monitorLightChanges != value)
                {

                    _monitorLightChanges = value;

                    if (_monitorLightChanges)
                    {
                        AddEvents();
                    }
                    else
                    {
                        RemoveEvents();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly => false;
        
        /// <summary>
        /// Adds the light to the collection.
        /// </summary>
        /// <param name="item">Light to add.</param>
        public void Add(Light item)
        {
            if (item == null)
            {
                return;
            }

            _lights.Add(item);

            if (_monitorLightChanges)
            {
                item.LightChanged += OnLightChanged;
            }

            _updateShader = true;
            _version++;
            OnModified();
        }

        /// <summary>
        /// Adds a number of lights to the collection.
        /// </summary>
        /// <param name="items">Lights to add.</param>
        public void AddRange(IEnumerable<Light> items)
        {
            if (items == null)
            {
                return;
            }

            IList<Light> coll = items as IList<Light>;

            if (coll == null)
            {
                foreach (Light l in items)
                {
                    if (l != null)
                    {
                        _lights.Add(l);

                        if (_monitorLightChanges)
                        {
                            l.LightChanged += OnLightChanged;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < coll.Count; i++)
                {
                    Light l = coll[i];
                    if (l != null)
                    {
                        _lights.Add(l);

                        if (_monitorLightChanges)
                        {
                            l.LightChanged += OnLightChanged;
                        }
                    }
                }
            }

            _updateShader = true;
            _version++;
            OnModified();
        }

        /// <summary>
        /// Inserts a light at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The light to insert into the collection.</param>
        public void Insert(int index, Light item)
        {
            if (index < 0 || index > _lights.Count || item == null)
            {
                return;
            }

            _lights.Insert(index, item);

            if (_monitorLightChanges)
            {
                item.LightChanged += OnLightChanged;
            }

            _updateShader = true;
            _version++;
            OnModified();
        }

        /// <summary>
        /// Removes all lights from the collection.
        /// </summary>
        public void Clear()
        {
            RemoveEvents();
            _lights.Clear();
            _version++;
            OnModified();
        }

        /// <summary>
        /// Queries if the specified light is contained in the collection.
        /// </summary>
        /// <param name="item">Light to determine if it is contained in the collection.</param>
        /// <returns>True if the light is contained in the collection, false otherwise.</returns>
        public bool Contains(Light item)
        {
            if (item == null)
            {
                return false;
            }

            return _lights.Contains(item);
        }

        /// <summary>
        /// Copies the entire contents of the collection to the array, starting at the specified array index.
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Index of the array to start copying to.</param>
        /// from the array index to the end of the destination array.</exception>
        public void CopyTo(Light[] array, int arrayIndex)
        {
            _lights.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the light at the specified index in the collection.
        /// </summary>
        /// <param name="index">Index of light to remove at.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _lights.Count)
            {
                return;
            }

            if (_monitorLightChanges)
            {
                Light l = _lights[index];
                l.LightChanged -= OnLightChanged;
            }

            _lights.RemoveAt(index);
            _version++;
            OnModified();
        }

        /// <summary>
        /// Removes the specified light from the collection.
        /// </summary>
        /// <param name="item">Light to remove.</param>
        /// <returns>True if the light was successfully removed from the collection, false otherwise.</returns>
        public bool Remove(Light item)
        {
            bool removed = _lights.Remove(item);

            if (removed)
            {
                _version++;

                if (_monitorLightChanges)
                {
                    item.LightChanged -= OnLightChanged;
                    OnModified();
                }
            }

            return removed;
        }

        /// <summary>
        /// Queries for the index in the collection of the specified light.
        /// </summary>
        /// <param name="item">Light to locate in the collection.</param>
        /// <returns>The zero-based index of the first light within the entire collection, if not found then -1.</returns>
        public int IndexOf(Light item)
        {
            return _lights.IndexOf(item);
        }

        /// <summary>
        /// Sorts the collection using a comparer.
        /// </summary>
        /// <param name="comparer">Comparer used to sort the contents of the collection.</param>
        public void Sort(IComparer<Light> comparer)
        {
            _lights.Sort(comparer);
            _version++;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public virtual void Read(ISavableReader input)
        {
            _globalAmbient = input.Read<Color>();
            _monitorLightChanges = input.ReadBoolean();
            _isEnabled = input.ReadBoolean();

            Light[] lightArray = input.ReadSharedSavableArray<Light>();
            _lights = (lightArray != null) ? new List<Light>(lightArray) : new List<Light>();
            _updateShader = true;

            if (_monitorLightChanges)
            {
                AddEvents();
            }
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public virtual void Write(ISavableWriter output)
        {
            output.Write("GlobalAmbient", ref _globalAmbient);
            output.Write("MonitorLightChanges", _monitorLightChanges);
            output.Write("IsEnabled", _isEnabled);
            output.WriteSharedSavable("Lights", _lights.ToArray());
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>Light collection enumerator.</returns>
        public LightCollectionEnumerator GetEnumerator()
        {
            return new LightCollectionEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        IEnumerator<Light> IEnumerable<Light>.GetEnumerator()
        {
            return new LightCollectionEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LightCollectionEnumerator(this);
        }

        /// <summary>
        /// Invoked when a light property is changed
        /// </summary>
        /// <param name="light">Light that was changed</param>
        /// <param name="e">Event arguments</param>
        private void OnLightChanged(Light light, EventArgs e)
        {
            _updateShader = true;
        }

        /// <summary>
        /// Removes all light changed handlers from all lights in the collection
        /// </summary>
        private void RemoveEvents()
        {
            for (int i = 0; i < _lights.Count; i++)
            {
                Light l = _lights[i];
                if (l != null)
                {
                    l.LightChanged += OnLightChanged;
                }
            }
        }

        /// <summary>
        /// Hooks all light changed events for all lights in the collection
        /// </summary>
        private void AddEvents()
        {
            for (int i = 0; i < _lights.Count; i++)
            {
                Light l = _lights[i];
                if (l != null)
                {
                    l.LightChanged -= OnLightChanged;
                }
            }
        }

        /// <summary>
        /// Invoked when the collection is modified
        /// </summary>
        private void OnModified()
        {
            CollectionModified?.Invoke(this, EventArgs.Empty);
        }

        #region Enumerator

        /// <summary>
        /// Enumerates elements of a <see cref="LightCollection"/>.
        /// </summary>
        public struct LightCollectionEnumerator : IEnumerator<Light>
        {
            private readonly int _version;
            private readonly LightCollection _collection;
            private readonly List<Light> _lights;
            private int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="LightCollectionEnumerator"/> struct
            /// </summary>
            /// <param name="lightCollection">Collection of lights</param>
            internal LightCollectionEnumerator(LightCollection lightCollection)
            {
                _collection = lightCollection;
                _lights = lightCollection._lights;
                _index = 0;
                _version = lightCollection._version;
                Current = null;
            }

            /// <summary>
            /// Gets the current light.
            /// </summary>
            public Light Current { get; private set; }

            /// <summary>
            /// Gets the current light.
            /// </summary>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                ThrowIfChanged();

                if (_index < _lights.Count)
                {
                    Current = _lights[_index];
                    _index++;
                    return true;
                }
                else
                {
                    _index++;
                    Current = null;
                    return false;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                ThrowIfChanged();
                _index = 0;
                Current = null;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // No-op
            }

            /// <summary>
            /// Throws a <see cref="InvalidOperationException"/> if the collection is changed while we are enumerating
            /// </summary>
            private void ThrowIfChanged()
            {
                if (_version != _collection._version)
                {
                    throw new InvalidOperationException("Collection modified during enumeration");
                }
            }
        }

        #endregion
    }
}
