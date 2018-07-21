namespace Spark.Graphics.Materials
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Content;
    using Core.Collections;

    /// <summary>
    /// A collection for materials, typically representing a set of materials found in a single TEM script file.
    /// </summary>
    [DebuggerDisplay("ScriptName = {ScriptFileName}, Count = {Count}")]
    public sealed class MaterialScriptCollection : NamedList<Material>, ISavable, IContentCastable
    {
        private string _scriptFileName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScriptCollection"/> class.
        /// </summary>
        public MaterialScriptCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScriptCollection"/> class.
        /// </summary>
        /// <param name="capacity">Initial capacity of the collection.</param>
        public MaterialScriptCollection(int capacity) 
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScriptCollection"/> class.
        /// </summary>
        /// <param name="materials">Collection of materials to add.</param>
        public MaterialScriptCollection(IEnumerable<Material> materials) 
            : base(materials)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialScriptCollection"/> class.
        /// </summary>
        /// <param name="materials">Collection of materials to add.</param>
        public MaterialScriptCollection(params Material[] materials) 
            : base(materials)
        {
        }

        /// <summary>
        /// Gets or sets the material script file name.
        /// </summary>
        public string ScriptFileName
        {
            get => _scriptFileName;
            set
            {
                _scriptFileName = value;
                if (string.IsNullOrEmpty(value))
                {
                    _scriptFileName = string.Empty;
                }
            }
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            int count = input.ReadInt32();
            Capacity = count;

            for (int i = 0; i < count; i++)
            {
                Material mat = input.ReadSavable<Material>();
                if (mat != null)
                {
                    Add(mat);
                }
            }
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("Count", Count);

            for (int i = 0; i < Count; i++)
            {
                output.WriteSavable("Material", this[i]);
            }
        }

        /// <summary>
        /// Attempts to cast the content item to another type.
        /// </summary>
        /// <param name="targetType">Type to cast to.</param>
        /// <param name="subresourceName">Optional subresource name.</param>
        /// <returns>Casted type or null if the type could not be converted.</returns>
        public object CastTo(Type targetType, string subresourceName)
        {
            if (targetType == typeof(Material))
            {
                if (Count > 0)
                {
                    // If has a subresource name, search for that material (may be null), otherwise return the first material
                    if (!string.IsNullOrEmpty(subresourceName))
                    {
                        return this[subresourceName];
                    }

                    return this[0];
                }

                return null;
            }

            return this;
        }
    }
}
