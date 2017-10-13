namespace Spark.Graphics.Materials
{
    using System;
    using System.Collections.Generic;

    using Core;
    using Content;
    using Graphics.Renderer;

    /// <summary>
    /// Defines the way the surface of a piece of geometry is rendered
    /// </summary>
    public class Material : ISavable, IComparable<Material>, INamable
    {
        private string _name;
        private string _scriptFileName;
        private readonly Dictionary<string, MaterialParameterBinding> _parameterBindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        protected Material()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        /// <param name="effect">Material effect</param>
        public Material(Effect effect)
            : this("Material", effect)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        /// <param name="name">Material name</param>
        /// <param name="effect">Material effect</param>
        public Material(string name, Effect effect)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            Name = string.IsNullOrEmpty(name) ? "Material" : name;
            ScriptFileName = string.Empty;
            Effect = effect;

            Passes = new MaterialPassCollection(this);
            _parameterBindings = new Dictionary<string, MaterialParameterBinding>();
            //m_scriptParameterBindings = new Dictionary<String, ScriptParameterBinding>();

            ShadowMode = ShadowMode.None;
            TransparencyMode = TransparencyMode.OneSided;
        }

        /// <summary>
        /// Gets or sets the name of the material
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                if (string.IsNullOrEmpty(value))
                {
                    _name = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the file name of the material script that this <see cref="Material"/> was constructed from
        /// </summary>
        public string ScriptFileName
        {
            get
            {
                return _scriptFileName;
            }
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
        /// Gets the material effect file
        /// </summary>
        public Effect Effect { get; }

        /// <summary>
        /// Get the collection of material passed
        /// </summary>
        public MaterialPassCollection Passes { get; }

        /// <summary>
        /// Gets a value indicating whether the material is valid
        /// </summary>
        public bool IsValid => Passes.Count > 0 && !Effect.IsDisposed;

        /// <summary>
        /// Gets the enumeration of parameter bindings
        /// </summary>
        public IEnumerable<MaterialParameterBinding> ParameterBindings => _parameterBindings.Values;

        /// <summary>
        /// Gets the number of parameter bindings
        /// </summary>
        public int ParameterBindingCount => _parameterBindings.Count;

        /// <summary>
        /// Gets or sets the shadowing mode the material uses
        /// </summary>
        public ShadowMode ShadowMode { get; set; }

        /// <summary>
        /// Gets or sets the transparency mode the material uses
        /// </summary>
        public TransparencyMode TransparencyMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public bool SetParameterBinding(string parameterName, IParameterBindingProvider provider)
        {
            if (string.IsNullOrEmpty(parameterName) || provider == null)
                return false;

            if (_parameterBindings.ContainsKey(parameterName))
                return false;

            IEffectParameter effectParam = Effect.Parameters[parameterName];
            if (effectParam == null)
                return false;

            MaterialParameterBinding binding = new MaterialParameterBinding(parameterName, provider, effectParam);
            bool isValid = binding.IsValid;

            if (isValid)
                _parameterBindings.Add(parameterName, binding);

            return isValid;
        }

        /// <summary>
        /// Applies the material
        /// </summary>
        /// <param name="renderContext">Render context to apply the material to</param>
        /// <param name="properties">Render property container</param>
        public void ApplyMaterial(IRenderContext renderContext, RenderPropertyCollection properties)
        {
            if (renderContext == null)
            {
                return;
            }

            foreach (KeyValuePair<string, MaterialParameterBinding> kv in _parameterBindings)
            {
                MaterialParameterBinding binding = kv.Value;
                if (binding.IsValid)
                {
                    object localState = binding.LocalState;
                    binding.Provider.UpdateParameter(renderContext, properties, this, binding.Parameter, ref localState);
                    binding.LocalState = localState;
                }
            }
        }
                
        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.</returns>
        public int CompareTo(Material other)
        {
            if (other == null || !other.IsValid)
            {
                return -1;
            }

            if (other == this)
            {
                return 0;
            }

            int otherSortKey = other.Effect.SortKey;
            int thisSortKey = Effect.SortKey;

            if (thisSortKey < otherSortKey)
            {
                return -1;
            }
            else if (thisSortKey > otherSortKey)
            {
                return 1;
            }
            else
            {
                // If same effects, look at what shader groups are used for each pass
                MaterialPassCollection otherPasses = other.Passes;
                for (int i = 0; i < Passes.Count; i++)
                {
                    if (i < otherPasses.Count)
                    {
                        int thisShaderIndex = Passes[i].ShaderGroup.ShaderGroupIndex;
                        int otherShaderIndex = otherPasses[i].ShaderGroup.ShaderGroupIndex;

                        // Compare the shaders used by each pass
                        if (thisShaderIndex < otherShaderIndex)
                        {
                            return -1;
                        }
                        else if (thisShaderIndex > otherShaderIndex)
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        // If each pass has been equal so far, but "this" material has more passes (and therefore more state changes), then sort "other" material as less than
                        return 1;
                    }
                }
            }

            // If got to this point, then materials are equal in what shaders they use
            return 0;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            throw new NotImplementedException();
        }
    }
}
