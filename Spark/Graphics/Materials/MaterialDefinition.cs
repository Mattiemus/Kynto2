namespace Spark.Graphics.Materials
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    using Core;
    using Content;
    using Graphics.Renderer;

    /// <summary>
    /// Defines a group of materials that are used to draw geometry. Each material has a shader effect and parameters, and is
    /// associated with a render bucket. An object may require multiple materials to achieve a complete rendering (e.g. one for
    /// shadows, one for opaque, one for special effects like emissive glow, etc), and all those materials will be contained in this
    /// material definition.
    /// </summary>
    public class MaterialDefinition : ISavable, INamable, IReadOnlyDictionary<RenderBucketId, Material>
    {
        private readonly Dictionary<RenderBucketId, Material> _materials;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDefinition"/> class.
        /// </summary>
        public MaterialDefinition() 
            : this(null, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDefinition"/> class.
        /// </summary>
        /// <param name="name">Name of the material definition</param>
        public MaterialDefinition(string name) 
            : this(name, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDefinition"/> class.
        /// </summary>
        /// <param name="name">Name of the material definition</param>
        /// <param name="initialCapacity">Initial capacity of the collection.</param>
        public MaterialDefinition(string name, int initialCapacity)
        {
            Name = (string.IsNullOrEmpty(name)) ? "MaterialDefinition" : name;
            ScriptFileName = string.Empty;
            _materials = new Dictionary<RenderBucketId, Material>(initialCapacity, new RenderBucketIdEqualityComparer());
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the script file, for serialization to a "TEMD" script.
        /// </summary>
        public string ScriptFileName { get; set; }

        /// <summary>
        /// Gets the number of materials contained.
        /// </summary>
        public int Count => _materials.Count;

        /// <summary>
        /// Gets all the bucket IDs associated with materials.
        /// </summary>
        public Dictionary<RenderBucketId, Material>.KeyCollection Keys => _materials.Keys;

        /// <summary>
        /// Gets all the materials.
        /// </summary>
        public Dictionary<RenderBucketId, Material>.ValueCollection Values => _materials.Values;

        /// <summary>
        /// Gets all the bucket IDs associated with materials.
        /// </summary>
        IEnumerable<RenderBucketId> IReadOnlyDictionary<RenderBucketId, Material>.Keys => _materials.Keys;

        /// <summary>
        /// Gets all the materials.
        /// </summary>
        IEnumerable<Material> IReadOnlyDictionary<RenderBucketId, Material>.Values => _materials.Values;

        /// <summary>
        /// Gets a material associated with the render bucket ID.
        /// </summary>
        /// <param name="key">Render bucket ID.</param>
        /// <returns>The associated material.</returns>
        public Material this[RenderBucketId key] => _materials[key];

        /// <summary>
        /// Queries if all materials are valid in the definition. If a material is null, then it is considered valid.
        /// </summary>
        /// <returns>True if all materials are valid, false if otherwise.</returns>
        public bool AreMaterialsValid()
        {
            foreach (KeyValuePair<RenderBucketId, Material> kv in _materials)
            {
                // A null material does not mean it is invalid
                if (kv.Value != null && !kv.Value.IsValid)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds a material to the definition which will be used for drawing with the associated render bucket. Null values
        /// are permitted.
        /// </summary>
        /// <param name="bucketID">Bucket the material will be used in.</param>
        /// <param name="material">Material that will be applied when an object is drawn, it can safely be null.</param>
        /// <returns>True if the material was scuccessfully added, false if another entry in the definition is associated with the bucket id.</returns>
        public bool Add(RenderBucketId bucketId, Material material)
        {
            // Allow null
            if (!_materials.ContainsKey(bucketId))
            {
                _materials.Add(bucketId, material);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Queries a material associated with <see cref="RenderBucketID.Opaque"/> and if there isn't one, then the first material
        /// defined in the collection.
        /// </summary>
        /// <returns>The material, or null if no materials are present. Possibly may be null for a null material.</returns>
        public Material GetOpaqueOrFirstMaterial()
        {
            if (_materials.Count == 0)
            {
                return null;
            }
            
            if (_materials.TryGetValue(RenderBucketId.Opaque, out Material mat))
            {
                return mat;
            }

            return _materials.FirstOrDefault().Value;
        }

        /// <summary>
        /// Moves the assignment of a material from one render bucket to another
        /// </summary>
        /// <param name="oldBucketId">Older render bucket id</param>
        /// <param name="newBucketId">New render bucket id</param>
        /// <returns>True if the material was found and moved, false otherwise</returns>
        public bool ReAssign(RenderBucketId oldBucketId, RenderBucketId newBucketId)
        {
            if (_materials.TryGetValue(oldBucketId, out Material mat))
            {
                _materials.Remove(oldBucketId);
                if (_materials.ContainsKey(newBucketId))
                {
                    _materials.Remove(newBucketId);
                }

                _materials.Add(newBucketId, mat);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the material assigned to a given render bucket
        /// </summary>
        /// <param name="bucketId">Render bucket id</param>
        /// <returns>True if the material was found and removed</returns>
        public bool Remove(RenderBucketId bucketId)
        {
            return _materials.Remove(bucketId);
        }

        /// <summary>
        /// Determines if the material definition contains a value for the given render bucket id
        /// </summary>
        /// <param name="key">Render bucket id</param>
        /// <returns>True if a material exists for the given bucket id</returns>
        public bool ContainsKey(RenderBucketId key)
        {
            return _materials.ContainsKey(key);
        }

        /// <summary>
        /// Determines if a given material is contained within the definition
        /// </summary>
        /// <param name="material">Material to search for</param>
        /// <returns>True if the material is contained within the definition</returns>
        public bool ContainsValue(Material material)
        {
            return _materials.ContainsValue(material);
        }

        /// <summary>
        /// Attempts to get a value from the material definition
        /// </summary>
        /// <param name="key">Render bucket id</param>
        /// <param name="value">Material for the render bucket</param>
        /// <returns>True if a material was found, false otherwise</returns>
        public bool TryGetValue(RenderBucketId key, out Material value)
        {
            return _materials.TryGetValue(key, out value);
        }

        /// <summary>
        /// Clears the material definition
        /// </summary>
        public void Clear()
        {
            _materials.Clear();
        }

        /// <summary>
        /// Creates a copy of the material definition
        /// </summary>
        /// <returns>Copy of the material definition</returns>
        public MaterialDefinition Clone()
        {
            MaterialDefinition matDef = new MaterialDefinition(Name, _materials.Count);
            matDef.ScriptFileName = ScriptFileName;

            foreach (KeyValuePair<RenderBucketId, Material> kv in _materials)
            {
                matDef.Add(kv.Key, (kv.Value != null) ? kv.Value.Clone() : null);
            }

            return matDef;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="Dictionary{RenderBucketId,Material}.Enumerator" /> that can be used to iterate through the collection.</returns>
        public Dictionary<RenderBucketId, Material>.Enumerator GetEnumerator()
        {
            return _materials.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{KeyValuePair{RenderBucketId,Material}}" /> that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<RenderBucketId, Material>> IEnumerable<KeyValuePair<RenderBucketId, Material>>.GetEnumerator()
        {
            return _materials.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator" /> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _materials.GetEnumerator();
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            int count = input.BeginReadGroup();

            for (int i = 0; i < count; i++)
            {
                String bucketName = input.ReadString();
                Material mat = input.ReadSavable<Material>();

                _materials.Add(RenderBucketId.QueryId(bucketName), mat);
            }

            input.EndReadGroup();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.BeginWriteGroup("Materials", _materials.Count);

            foreach (KeyValuePair<RenderBucketId, Material> kv in _materials)
            {
                output.Write("RenderBucket", kv.Key.Name);
                output.WriteSavable("Material", kv.Value);
            }

            output.EndWriteGroup();
        }        
    }
}
