namespace Spark.Direct3D11.Graphics.Implementation
{
    using System.Collections.Generic;
    
    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;
    
    internal sealed class D3D11EffectResourceManager
    {
        private static readonly IShaderResource[] EmptyResourceArray = new IShaderResource[0];

        public IShaderResource[] Resources;
        public EffectConstantBufferCollection ConstantBuffers;

        private readonly Dictionary<int, ShaderBlock> _shaderBlocks;
        private readonly EffectData _effectData;
        
        private D3D11EffectResourceManager(D3D11EffectResourceManager toCloneFrom)
        {
            _effectData = toCloneFrom._effectData;
            ShaderManager = toCloneFrom.ShaderManager;
            ShaderManager.AddRef();

            _shaderBlocks = new Dictionary<int, ShaderBlock>();

            // Clone shader blocks
            foreach (KeyValuePair<int, ShaderBlock> kv in toCloneFrom._shaderBlocks)
            {
                _shaderBlocks.Add(kv.Key, kv.Value.Clone(this));
            }

            if (_effectData.ResourceVariables != null && _effectData.ResourceVariables.Length > 0)
            {
                // Manager we're cloning from already knows the total resource count, and we want to preserve attached resources,
                // so just clone their array
                Resources = toCloneFrom.Resources.Clone() as IShaderResource[];
            }
            else
            {
                Resources = EmptyResourceArray;
            }

            ConstantBuffers = EffectConstantBufferCollection.EmptyCollection;
        }

        public D3D11EffectResourceManager(EffectData effectData)
        {
            _shaderBlocks = new Dictionary<int, ShaderBlock>();
            _effectData = effectData;
            ShaderManager = new ShaderManager();

            if (_effectData.ResourceVariables != null && _effectData.ResourceVariables.Length > 0)
            {
                int totalResourceCount = 0;
                for (int i = 0; i < _effectData.ResourceVariables.Length; i++)
                {
                    EffectData.ResourceVariable rvar = _effectData.ResourceVariables[i];
                    totalResourceCount += (rvar.ElementCount == 0) ? 1 : rvar.ElementCount;
                }

                Resources = new IShaderResource[totalResourceCount];
            }
            else
            {
                Resources = EmptyResourceArray;
            }

            ConstantBuffers = EffectConstantBufferCollection.EmptyCollection;
        }
        
        public ShaderManager ShaderManager { get; }

        public void SetConstantBuffers(EffectConstantBufferCollection constantBuffers)
        {
            if (constantBuffers == null || constantBuffers.Count == 0)
            {
                return;
            }

            ConstantBuffers = constantBuffers;
        }

        public ShaderBlock GetShaderBlock(D3D11.Device device, EffectData.Shader shader)
        {
            if (shader == null)
            {
                return null;
            }
            
            if (_shaderBlocks.TryGetValue(shader.HashCode, out ShaderBlock shaderBlock))
            {
                return shaderBlock;
            }

            shaderBlock = new ShaderBlock(device, shader, this);
            _shaderBlocks.Add(shader.HashCode, shaderBlock);

            return shaderBlock;
        }

        public D3D11EffectResourceManager Clone()
        {
            return new D3D11EffectResourceManager(this);
        }
    }
}
