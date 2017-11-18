namespace Spark.Graphics
{
    /// <summary>
    /// A collection of passes in a material. Each pass contains all the state information for the rendering pipeline that is applied before a draw call is issued.
    /// </summary>
    public sealed class MaterialPassCollection : ReadOnlyNamedListFast<MaterialPass>
    {
        private readonly Material _material;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPassCollection"/> class.
        /// </summary>
        /// <param name="material">Parent material</param>
        internal MaterialPassCollection(Material material) 
            : this(material, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPassCollection"/> class.
        /// </summary>
        /// <param name="material">Parent material</param>
        /// <param name="initialCapacity">Initial capaacity of the collection</param>
        internal MaterialPassCollection(Material material, int initialCapacity)
            : base(initialCapacity)
        {
            _material = material;
        }

        /// <summary>
        /// Adds a new pass to the collection.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="shaderGroupName">Name of the shader group from the effect to be used for the pass.</param>
        /// <returns>The created pass. This may be null if the pass was failed to be created.</returns>
        public MaterialPass Add(string passName, string shaderGroupName)
        {
            if (string.IsNullOrEmpty(passName))
            {
                return null;
            }

            IEffectShaderGroup shaderGroup = _material.Effect.ShaderGroups[shaderGroupName];
            if (shaderGroup == null)
            {
                return null;
            }

            MaterialPass pass = new MaterialPass(_material, shaderGroup, passName);
            pass.PassIndex = Count;
            _list.Add(pass);

            PopulateFastLookupTable();

            return pass;
        }

        /// <summary>
        /// Adds a new pass to the collection.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="shaderGroup">Shader group from the effect to be used for the pass.</param>
        /// <returns>The created pass. This may be null if the pass was failed to be created.</returns>
        public MaterialPass Add(string passName, IEffectShaderGroup shaderGroup)
        {
            if (string.IsNullOrEmpty(passName) || shaderGroup == null || !shaderGroup.IsPartOf(_material.Effect))
            {
                return null;
            }

            MaterialPass pass = new MaterialPass(_material, shaderGroup, passName);
            pass.PassIndex = Count;
            _list.Add(pass);

            PopulateFastLookupTable();

            return pass;
        }

        /// <summary>
        /// Removes the pass from the collection.
        /// </summary>
        /// <param name="passName">Name of the pass to remove.</param>
        /// <returns>True if successfully removed, false if it could not be found.</returns>
        public bool Remove(string passName)
        {
            if (string.IsNullOrEmpty(passName))
            {
                return false;
            }

            for (int i = 0; i < _list.Count; i++)
            {
                MaterialPass pass = _list[i];
                if (passName.Equals(pass.Name))
                {
                    _list.RemoveAt(i);
                    pass.PassIndex = -1;
                    PopulateFastLookupTable();
                    ReIndexPasses(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the pass from the collection.
        /// </summary>
        /// <param name="pass">Pass to remove.</param>
        /// <returns>True if successfully removed, false if not (e.g. the pass does not belong to this material).</returns>
        public bool Remove(MaterialPass pass)
        {
            if (pass == null || pass.Parent != _material)
            {
                return false;
            }

            for (int i = 0; i < _list.Count; i++)
            {
                MaterialPass p = _list[i];
                if (pass == p)
                {
                    _list.RemoveAt(i);
                    pass.PassIndex = -1;
                    PopulateFastLookupTable();
                    ReIndexPasses(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears the collection of passes.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            PopulateFastLookupTable();
        }

        /// <summary>
        /// Updates the fast lookup table
        /// </summary>
        internal void UpdateFastLookup()
        {
            PopulateFastLookupTable();
        }

        /// <summary>
        /// Copies all passes from this collection to the specified collection
        /// </summary>
        /// <param name="collection">Collection to copy passes into</param>
        internal void CopyPassesOver(MaterialPassCollection collection)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                MaterialPass oldPass = _list[i];
                MaterialPass newPass = new MaterialPass(collection._material, collection._material.Effect.ShaderGroups[oldPass.ShaderGroup.ShaderGroupIndex], oldPass.Name);

                if (oldPass.RenderStatesToApply.HasFlag(EnforcedRenderState.BlendState))
                {
                    newPass.BlendState = oldPass.BlendState;
                }

                if (oldPass.RenderStatesToApply.HasFlag(EnforcedRenderState.RasterizerState))
                {
                    newPass.RasterizerState = oldPass.RasterizerState;
                }

                if (oldPass.RenderStatesToApply.HasFlag(EnforcedRenderState.DepthStencilState))
                {
                    newPass.DepthStencilState = oldPass.DepthStencilState;
                }

                collection._list.Add(newPass);
            }

            collection.PopulateFastLookupTable();
        }

        /// <summary>
        /// Reindexes all the passes
        /// </summary>
        /// <param name="startIndex">Start index to reindex from</param>
        private void ReIndexPasses(int startIndex)
        {
            for (int i = startIndex; i < _list.Count; i++)
            {
                MaterialPass pass = _list[i];
                pass.PassIndex = i;
            }
        }
    }
}
