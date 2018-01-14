namespace Spark.Graphics
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the complete source code for an <see cref="Effect"/>. This is run through an effect compiler
    /// to produce a compiled SPFX file that can be consumed by a render system.
    /// </summary>
    public sealed class EffectContent : IEnumerable<EffectContent.ShaderGroupContent>
    {
        private readonly List<ShaderGroupContent> _shaderGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectContent"/> class.
        /// </summary>
        public EffectContent()
        {
            FileName = string.Empty;
            _shaderGroups = new List<ShaderGroupContent>();
        }

        /// <summary>
        /// Gets the file name of the effect content.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the shader group content at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the technique content.</param>
        /// <returns>Shader group content contained, or null if out of range.</returns>
        public ShaderGroupContent this[int index]
        {
            get
            {
                if (index < 0 || index > _shaderGroups.Count)
                {
                    return null;
                }

                return _shaderGroups[index];
            }
        }

        /// <summary>
        /// Gets the shader group content corresponding to the specified name.
        /// </summary>
        /// <param name="shaderGrpName">Name of the shader group to get.</param>
        /// <returns>Shader group content contained, or null if a shader group corresponding to the name could not be found.</returns>
        public ShaderGroupContent this[string shaderGrpName]
        {
            get
            {
                if (string.IsNullOrEmpty(shaderGrpName))
                {
                    return null;
                }

                for (int i = 0; i < _shaderGroups.Count; i++)
                {
                    ShaderGroupContent grp = _shaderGroups[i];
                    if (grp.Name.Equals(shaderGrpName))
                    {
                        return grp;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the number of shader groups contained in the effect.
        /// </summary>
        public int ShaderGroupCount => _shaderGroups.Count;

        /// <summary>
        /// Adds a shader group to the effect.
        /// </summary>
        /// <param name="shaderGrp">ShaderGroup content to add</param>
        /// <returns>True if successfully added, false otherwise.</returns>
        public bool AddShaderGroup(ShaderGroupContent shaderGrp)
        {
            if (shaderGrp != null)
            {
                _shaderGroups.Add(shaderGrp);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a shader group, specified by a zero-based index, from the effect.
        /// </summary>
        /// <param name="index">Zero-based index of the pass content.</param>
        /// <returns>True if the shader group was removed, false otherwise.</returns>
        public bool RemoveShaderGroup(int index)
        {
            if (index < 0 || index > _shaderGroups.Count)
            {
                return false;
            }

            _shaderGroups.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes a shader group, specified by its name, from the effect. The first technique that
        /// matches the name exactly is removed.
        /// </summary>
        /// <param name="shaderGrpName">Name of the shader group content to remove.</param>
        /// <returns>True if the shader group was removed, false otherwise.</returns>
        public bool RemoveShaderGroup(string shaderGrpName)
        {
            if (string.IsNullOrEmpty(shaderGrpName))
            {
                return false;
            }

            for (int i = 0; i < _shaderGroups.Count; i++)
            {
                ShaderGroupContent shaderGrp = _shaderGroups[i];
                if (shaderGrp.Name.Equals(shaderGrpName))
                {
                    _shaderGroups.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a shader group from the effect.
        /// </summary>
        /// <param name="shaderGrp">ShaderGroup content to remove.</param>
        /// <returns>True if the shader group was removed, false otherwise.</returns>
        public bool RemoveShaderGroup(ShaderGroupContent shaderGrp)
        {
            if (shaderGrp == null)
            {
                return false;
            }

            return _shaderGroups.Remove(shaderGrp);
        }

        /// <summary>
        /// Clears all shader groups currently contained in the effect.
        /// </summary>
        public void Clear()
        {
            _shaderGroups.Clear();
        }

        /// <summary>
        /// Gets all shader groups contained in this effect as a list.
        /// </summary>
        /// <returns>A list of shader groups contained in this effect.</returns>
        public IList<ShaderGroupContent> GetShaderGroups()
        {
            return new List<ShaderGroupContent>(_shaderGroups);
        }

        /// <summary>
        /// Queries if the effect contains a shader group corresponding to the name.
        /// </summary>
        /// <param name="shaderGrpName">Name of the shader group.</param>
        /// <returns>True if the shader group content was found, false otherwise.</returns>
        public bool IsShaderGroupPresent(string shaderGrpName)
        {
            if (string.IsNullOrEmpty(shaderGrpName))
            {
                return false;
            }

            for (int i = 0; i < _shaderGroups.Count; i++)
            {
                ShaderGroupContent tech = _shaderGroups[i];
                if (tech.Name.Equals(shaderGrpName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<ShaderGroupContent> GetEnumerator()
        {
            return _shaderGroups.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _shaderGroups.GetEnumerator();
        }
        
        /// <summary>
        /// Represents a group of shaders defined in the effect. Each group can have exactly one of each type of shader.
        /// </summary>
        public sealed class ShaderGroupContent : INamable, IEnumerable<ShaderContent>
        {
            private readonly Dictionary<ShaderStage, ShaderContent> _shaders;
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderGroupContent"/> class.
            /// </summary>
            public ShaderGroupContent()
            {
                Name = "ShaderGroup";
                _shaders = new Dictionary<ShaderStage, ShaderContent>();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderGroupContent"/> class.
            /// </summary>
            /// <param name="name">Name of the shader group</param>
            public ShaderGroupContent(string name)
            {
                Name = (string.IsNullOrEmpty(name)) ? "ShaderGroup" : name;
                _shaders = new Dictionary<ShaderStage, ShaderContent>();
            }


            /// <summary>
            /// Gets the name of the shader group.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets the shader content corresponding to the shader type, if it is present in the group.
            /// </summary>
            /// <param name="shaderType">Shader type to query</param>
            /// <returns>The contained shader content, or null if content of the specified type is not present.</returns>
            public ShaderContent this[ShaderStage shaderType]
            {
                get => _shaders[shaderType];
                set => _shaders[shaderType] = value;
            }

            /// <summary>
            /// Gets the number of shaders contained in this group.
            /// </summary>
            public int ShaderCount => _shaders.Count;

            /// <summary>
            /// Gets all shader content contained in this group as a list.
            /// </summary>
            /// <returns>A list of shader content contained in this group.</returns>
            public IList<ShaderContent> GetShaders()
            {
                var shaders = new List<ShaderContent>(_shaders.Count);
                foreach (KeyValuePair<ShaderStage, ShaderContent> kv in _shaders)
                {
                    if (kv.Value != null)
                    {
                        shaders.Add(kv.Value);
                    }
                }

                return shaders;
            }

            /// <summary>
            /// Clears all shaders currently contained in the group.
            /// </summary>
            public void Clear()
            {
                _shaders.Clear();
            }

            /// <summary>
            /// Queries if the group contains shader content corresponding to the specified type.
            /// </summary>
            /// <param name="shaderType">Type of shader</param>
            /// <returns>True if shader content is contained, or false if not.</returns>
            public bool IsShaderPresent(ShaderStage shaderType)
            {
                return _shaders.ContainsKey(shaderType);
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
            public IEnumerator<ShaderContent> GetEnumerator()
            {
                return _shaders.Values.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return _shaders.Values.GetEnumerator();
            }
        }

        /// <summary>
        /// Represents source code for a single shader.
        /// </summary>
        public sealed class ShaderContent
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderContent"/> class.
            /// </summary>
            public ShaderContent()
            {
                ShaderType = ShaderStage.VertexShader;
                SourceCode = string.Empty;
                EntryPoint = string.Empty;
                ShaderProfile = string.Empty;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderContent"/> class.
            /// </summary>
            /// <param name="shaderType">Type of shader.</param>
            /// <param name="sourceCode">Source code.</param>
            /// <param name="entryPoint">Entry function name.</param>
            /// <param name="shaderProfile">Shader profile.</param>
            public ShaderContent(ShaderStage shaderType, string sourceCode, string entryPoint, string shaderProfile)
            {
                ShaderType = shaderType;
                SourceCode = sourceCode;
                EntryPoint = entryPoint;
                ShaderProfile = shaderProfile;
            }

            /// <summary>
            /// Gets the type of shader the content represents.
            /// </summary>
            public ShaderStage ShaderType { get; set; }

            /// <summary>
            /// Gets the shader source code.
            /// </summary>
            public string SourceCode { get; set; }

            /// <summary>
            /// Gets the function name that serves as an entry point to the shader.
            /// </summary>
            public string EntryPoint { get; set; }

            /// <summary>
            /// Gets the shader profile the shader should be compiled against.
            /// </summary>
            public string ShaderProfile { get; set; }
        }
    }
}
