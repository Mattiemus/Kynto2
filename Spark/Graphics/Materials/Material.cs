namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;
    
    using Math;
    using Content;

    /// <summary>
    /// Defines the way the surface of a piece of geometry is rendered
    /// </summary>
    public class Material : ISavable, IComparable<Material>, INamable
    {
        private string _name;
        private string _scriptFileName;
        private readonly Dictionary<string, MaterialParameterBinding> _parameterBindings;
        private readonly Dictionary<string, MaterialParameter> _parameters;

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
            _parameters = new Dictionary<string, MaterialParameter>();

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
        /// Creates a copy of the material
        /// </summary>
        /// <returns>Copy of the material</returns>
        public Material Clone()
        {
            Material clone = new Material(Name, Effect.Clone());

            clone.ScriptFileName = ScriptFileName;
            clone.ShadowMode = ShadowMode;
            clone.TransparencyMode = TransparencyMode;

            // Copy over passes
            Passes.CopyPassesOver(clone.Passes);

            // Just copy contents of each collection to the clone, everything should already be valid
            foreach (KeyValuePair<string, MaterialParameterBinding> kv in _parameterBindings)
            {
                MaterialParameterBinding oldBinding = kv.Value;
                MaterialParameterBinding newBinding = new MaterialParameterBinding(oldBinding.Name, oldBinding.Provider, clone.Effect.Parameters[oldBinding.Name]);
                clone._parameterBindings.Add(newBinding.Name, newBinding);
            }

            foreach (KeyValuePair<string, MaterialParameter> kv in _parameters)
            {
                MaterialParameter oldBinding = kv.Value;
                MaterialParameter newBinding = new MaterialParameter(oldBinding.Name, clone.Effect.Parameters[oldBinding.Name]);
                clone._parameters.Add(newBinding.Name, newBinding);
            }

            return clone;
        }

        /// <summary>
        /// Gets the binding for a parameter
        /// </summary>
        /// <param name="parameterName">Parameter name to get the binding for</param>
        /// <returns>Material parameter binding, or null if it could not be found</returns>
        public MaterialParameterBinding GetParameterBinding(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return null;
            }

            if (_parameterBindings.TryGetValue(parameterName, out MaterialParameterBinding binding))
            {
                return binding;
            }

            return null;
        }

        /// <summary>
        /// Sets a parameter to be bound to a provider
        /// </summary>
        /// <param name="parameterName">Parameter name to bind</param>
        /// <param name="provider">Binding provider</param>
        /// <returns>True if the parameter was bound, false otherwise</returns>
        public bool SetParameterBinding(string parameterName, IParameterBindingProvider provider)
        {
            if (string.IsNullOrEmpty(parameterName) || provider == null)
            {
                return false;
            }

            if (_parameterBindings.ContainsKey(parameterName))
            {
                return false;
            }

            IEffectParameter effectParam = Effect.Parameters[parameterName];
            if (effectParam == null)
            {
                return false;
            }

            MaterialParameterBinding binding = new MaterialParameterBinding(parameterName, provider, effectParam);
            bool isValid = binding.IsValid;

            if (isValid)
            {
                _parameterBindings.Add(parameterName, binding);
            }

            return isValid;
        }

        /// <summary>
        /// Removes a binding from a parameter
        /// </summary>
        /// <param name="parameterName">Paramemter name to remove the binding for</param>
        /// <returns>True if the parameter was found and removed, false otherwise</returns>
        public bool RemoveParameterBinding(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return false;
            }

            return _parameterBindings.Remove(parameterName);
        }

        /// <summary>
        /// Gets the value of a parameter
        /// </summary>
        /// <param name="parameterName">Name of the parameter to get</param>
        /// <returns>Material parameter value, or null if it could not be found</returns>
        public MaterialParameter GetParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return null;
            }

            if (_parameters.TryGetValue(parameterName, out MaterialParameter binding))
            {
                return binding;
            }

            return null;
        }

        /// <summary>
        /// Sets a parameter
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="dataType">Optional data type. Each parameter (non-structures) have a default .NET type, but some parameters may be associated with
        /// a different type. E.g. a parameter that is a 32-bit integer but is really a packed <see cref="Color"/>.</param>
        /// <returns>True if the parameter is valid, false if not.</returns>
        public bool SetParameter(string parameterName, Type dataType = null)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return false;
            }

            // If we already have the parameter name, then it was successfully added at one point. So don't do anything but report that we have it.
            if (_parameters.ContainsKey(parameterName))
            {
                return true;
            }

            IEffectParameter effectParam = Effect.Parameters[parameterName];
            if (effectParam == null)
            {
                return false;
            }

            MaterialParameter binding = new MaterialParameter(parameterName, effectParam);
            bool isValid = binding.IsValid;

            if (isValid)
            {
                _parameters.Add(parameterName, binding);
                if (dataType != null)
                {
                    binding.DataType = dataType;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Sets the value of a parameter
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">Value to set the parameter to</param>
        /// <returns>True if the parameter was set, false otherwise</returns>
        public bool SetParameter<T>(string parameterName, T value) where T : struct
        {
            bool success = SetParameter(parameterName, typeof(T));
            if (success)
            {
                Effect.Parameters[parameterName].SetValue(ref value);
            }

            return success;
        }

        /// <summary>
        /// Sets the value of a parameter
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">Value to set the parameter to</param>
        /// <param name="transpose">True if the matrix should be transposed, false otherwise</param>
        /// <returns>True if the parameter was set, false otherwise</returns>
        public bool SetParameter(string parameterName, Matrix4x4 value, bool transpose = false)
        {
            bool success = SetParameter(parameterName, typeof(Matrix4x4));
            if (success)
            {
                if (transpose)
                {
                    Effect.Parameters[parameterName].SetValueTranspose(ref value);
                }
                else
                {
                    Effect.Parameters[parameterName].SetValue(ref value);
                }
            }

            return success;
        }

        /// <summary>
        /// Sets the value of a parameter
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">Value to set the parameter to</param>
        /// <returns>True if the parameter was set, false otherwise</returns>
        public bool SetParameter(string parameterName, IShaderResource value)
        {
            bool success = SetParameter(parameterName, (value != null) ? value.GetType() : null);
            if (success)
            {
                Effect.Parameters[parameterName].SetResource(value);
            }

            return success;
        }

        /// <summary>
        /// Removes a parameter
        /// </summary>
        /// <param name="parameterName">Paramemter name to remove</param>
        /// <returns>True if the parameter was found and removed, false otherwise</returns>
        public bool RemoveParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return false;
            }

            return _parameters.Remove(parameterName);
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
