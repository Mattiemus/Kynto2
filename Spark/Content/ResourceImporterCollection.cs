namespace Spark.Content
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Core.Collections;

    /// <summary>
    /// Represents a collection of resource importers that can be queried by extension and target type. The collection
    /// allows for multiple importers to be registered to a format extension, but there can only be one importer per format extension
    /// of any given type.
    /// </summary>
    public sealed class ResourceImporterCollection : ICollection<IResourceImporter>
    {
        private readonly MultiKeyDictionary<string, Type, IResourceImporter> _extensionToImporters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceImporterCollection"/> class.
        /// </summary>
        public ResourceImporterCollection()
        {
            _extensionToImporters = new MultiKeyDictionary<string, Type, IResourceImporter>(StringComparer.InvariantCultureIgnoreCase, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceImporterCollection"/> class.
        /// </summary>
        /// <param name="importers">Importers to add to the collection</param>
        public ResourceImporterCollection(IEnumerable<IResourceImporter> importers)
        {
            _extensionToImporters = new MultiKeyDictionary<string, Type, IResourceImporter>(StringComparer.InvariantCultureIgnoreCase, null);

            if (importers != null)
            {
                foreach (IResourceImporter importer in importers)
                {
                    Add(importer);
                }
            }
        }

        /// <summary>
        /// Gets the number of importers in the collection
        /// </summary>
        int ICollection<IResourceImporter>.Count => _extensionToImporters.Count;

        /// <summary>
        /// Gets a value indicating whether the collection is read only
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a resource importer registered to the extension and target type.
        /// </summary>
        /// <param name="extension">Format extension that the importer handles</param>
        /// <param name="targetType">Target type that the importer handles</param>
        /// <returns>The resource importer, or null if it is not found</returns>
        public IResourceImporter this[string extension, Type targetType]
        {
            get
            {
                if (string.IsNullOrEmpty(extension) || targetType == null)
                {
                    return null;
                }

                if (_extensionToImporters.TryGetValue(extension, targetType, out IResourceImporter importer))
                {
                    return importer;
                }

                return null;
            }
        }

        /// <summary>
        /// Adds the importer to the collection.
        /// </summary>
        /// <param name="item">The importer to add</param>
        public void Add(IResourceImporter item)
        {
            if (item == null)
            {
                return;
            }

            IEnumerable<string> extensions = item.Extensions;
            foreach (string ext in extensions)
            {
                _extensionToImporters.Add(ext, item.TargetType, item);
            }
        }

        /// <summary>
        /// Removes a resource importer from this collection
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True if the importer was removed, false if it could not be found.</returns>
        public bool Remove(IResourceImporter item)
        {
            if (item == null)
            {
                return false;
            }

            bool anyRemoved = false;
            foreach (string extension in item.Extensions)
            {
                anyRemoved |= Remove(extension, item.TargetType);
            }

            return anyRemoved;
        }

        /// <summary>
        /// Removes an importer of the extension and target type.
        /// </summary>
        /// <param name="extension">Format extension that the importer handles</param>
        /// <param name="targetType">Target type that the importer handles</param>
        /// <returns>True if the importer was removed, false if it could not be found.</returns>
        public bool Remove(string extension, Type targetType)
        {
            if (string.IsNullOrEmpty(extension) || targetType == null)
            {
                return false;
            }

            return _extensionToImporters.Remove(extension, targetType);
        }

        /// <summary>
        /// Clears the entire collection of importers.
        /// </summary>
        public void Clear()
        {
            _extensionToImporters.Clear();
        }

        /// <summary>
        /// Clears all importers that are registered to the specified format extension.
        /// </summary>
        /// <param name="extension">Format extension that the importers handle.</param>
        public void Clear(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return;
            }

            _extensionToImporters.ClearMajorKeys(extension);
        }

        /// <summary>
        /// Clears all importers that handle the specified target type, this clears the importer from all
        /// format extension buckets.
        /// </summary>
        /// <param name="targetType">Target type that the importer handles</param>
        public void Clear(Type targetType)
        {
            _extensionToImporters.ClearMinorKeys(targetType);
        }

        /// <summary>
        /// Determines if the collection contains the resource importer
        /// </summary>
        /// <param name="item">Resource importer to search for</param>
        /// <returns>True if a resource importer was found, false if it could not be.</returns>
        public bool Contains(IResourceImporter item)
        {
            if (item == null)
            {
                return false;
            }

            return _extensionToImporters.ContainsValue(item);
        }

        /// <summary>
        /// Queries if the collection contains a resource importer that is registered to the format extension and target type.
        /// </summary>
        /// <param name="extension">Format extension that the importer handles</param>
        /// <param name="targetType">Target type that the importer handles</param>
        /// <returns>True if a resource importer was found, false if it could not be.</returns>
        public bool Contains(string extension, Type targetType)
        {
            if (string.IsNullOrEmpty(extension) || targetType == null)
            {
                return false;
            }

            return _extensionToImporters.ContainsKey(extension, targetType);
        }

        /// <summary>
        /// Queries the number of importers registered to the specified format extension.
        /// </summary>
        /// <param name="extension">Format extension</param>
        /// <returns>The number of importers registered to the extension.</returns>
        public int Count(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return 0;
            }

            return _extensionToImporters.QueryMinorKeyCount(extension);
        }

        /// <summary>
        /// Queries the number of importers that can handle the specified target type.
        /// </summary>
        /// <param name="targetType">Target type</param>
        /// <returns>The number of importers that can handle the target type.</returns>
        public int Count(Type targetType)
        {
            return _extensionToImporters.QueryMajorKeyCount(targetType);
        }

        /// <summary>
        /// Gets the set of all importers contained in the collection.
        /// </summary>
        /// <returns>Set of importers</returns>
        public HashSet<IResourceImporter> GetImporters()
        {
            HashSet<IResourceImporter> importers = new HashSet<IResourceImporter>();
            foreach (KeyValuePair<MultiKey<string, Type>, IResourceImporter> kv in _extensionToImporters)
            {
                importers.Add(kv.Value);
            }

            return importers;
        }

        /// <summary>
        /// Gets the set of importers registered to the specified format extension.
        /// </summary>
        /// <param name="extension">Format extension</param>
        /// <returns>Set of importers</returns>
        public HashSet<IResourceImporter> GetImporters(string extension)
        {
            HashSet<IResourceImporter> importers = new HashSet<IResourceImporter>();
            if (!string.IsNullOrEmpty(extension))
            {
                MultiKeyDictionary<string, Type, IResourceImporter>.MinorKeyValueEnumerator enumerator = _extensionToImporters.GetMinorKeyValueEnumerator(extension);
                while (enumerator.MoveNext())
                {
                    KeyValuePair<Type, IResourceImporter> kv = enumerator.Current;
                    importers.Add(kv.Value);
                }
            }

            return importers;
        }

        /// <summary>
        /// Gets the set of importers that handle the specified target type.
        /// </summary>
        /// <param name="targetType">Target type</param>
        /// <returns>Enumerable collection of importers</returns>
        public HashSet<IResourceImporter> GetImporters(Type targetType)
        {
            return GetImporters(targetType, false);
        }

        /// <summary>
        /// Gets the set of importers that handle the specified target type.
        /// </summary>
        /// <param name="targetType">Target type</param>
        /// <param name="getOnlyRegisteredToType">Only return importers specifically registered to the type if true. If false, then importers
        /// that pass the <see cref="IResourceImporter.CanLoadType(Type)"/> check will be returned as well.</param>
        /// <returns>Set of importers</returns>
        public HashSet<IResourceImporter> GetImporters(Type targetType, bool getOnlyRegisteredToType)
        {
            HashSet<IResourceImporter> importers = new HashSet<IResourceImporter>();
            if (targetType != null)
            {
                foreach (KeyValuePair<MultiKey<string, Type>, IResourceImporter> kv in _extensionToImporters)
                {
                    bool add = kv.Value.TargetType == targetType || (!getOnlyRegisteredToType && kv.Value.CanLoadType(targetType));
                    if (add)
                    {
                        importers.Add(kv.Value);
                    }
                }
            }

            return importers;
        }

        /// <summary>
        /// Finds a resource importer that can potentially load the format into the specified target type. The target type
        /// does not necessarily have to match the resource importer's, if the type to load can be casted to the importer's target type.
        /// </summary>
        /// <param name="extension">Format extension that the importer handles</param>
        /// <param name="targetType">The type that is expected to be loaded</param>
        /// <returns>A resource importer that can potentially load this type of resource, or null if none could be found.</returns>
        public IResourceImporter FindSuitableImporter(string extension, Type targetType)
        {
            if (string.IsNullOrEmpty(extension) || targetType == null)
            {
                return null;
            }

            // Query if we have a importer registered to the type
            if (_extensionToImporters.TryGetValue(extension, targetType, out IResourceImporter importer))
            {
                return importer;
            }

            // If that wasn't successful, search the collection to see if we can find one that can handle the type (first one)
            MultiKeyDictionary<string, Type, IResourceImporter>.MinorKeyValueEnumerator enumerator = _extensionToImporters.GetMinorKeyValueEnumerator(extension);
            while (enumerator.MoveNext())
            {
                KeyValuePair<Type, IResourceImporter> kv = enumerator.Current;
                if (kv.Value.CanLoadType(targetType))
                {
                    return kv.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Copies all elements in the collection to an array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Starting index to copy from</param>
        public void CopyTo(IResourceImporter[] array, int arrayIndex)
        {
            _extensionToImporters.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        IEnumerator<IResourceImporter> IEnumerable<IResourceImporter>.GetEnumerator()
        {
            foreach (IResourceImporter importer in _extensionToImporters.Values)
            {
                yield return importer;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (IResourceImporter importer in _extensionToImporters.Values)
            {
                yield return importer;
            }
        }
    }
}
