namespace Spark.Direct3D11.Graphics.Implementation
{
    using System.Collections.Generic;
    using System.Threading;
    
    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;

    internal sealed class ShaderManager
    {
        private int _referenceCount;
        private readonly Dictionary<int, HashedShader> _shaderMap;

        public ShaderManager()
        {
            _shaderMap = new Dictionary<int, HashedShader>();
            AddRef();
        }

        public HashedShader GetOrCreateShader(D3D11.Device device, EffectData.Shader shaderdata)
        {
            // No need to lock, because all the shaders will be created when the effect is fully initialized. After that, the shader map
            // effectively becomes read only. Release, on the other hand, may still happen in whatever last effect instance is out there.
            HashedShader shader;

            ShaderStage type = shaderdata.ShaderType;
            int hashCode = shaderdata.HashCode;
            byte[] byteCode = shaderdata.ShaderByteCode;

            if (_shaderMap.TryGetValue(hashCode, out shader))
            {
                return shader;
            }

            switch (type)
            {
                case ShaderStage.VertexShader:
                    shader = new HashedShader(new D3D11.VertexShader(device, byteCode), hashCode, type);
                    break;
                case ShaderStage.PixelShader:
                    shader = new HashedShader(new D3D11.PixelShader(device, byteCode), hashCode, type);
                    break;
                case ShaderStage.GeometryShader:
                    shader = new HashedShader(new D3D11.GeometryShader(device, byteCode), hashCode, type);

                    var soCache = new GSWithSOCache(device);
                    soCache.SetShader(shaderdata);
                    shader.Shader.Tag = soCache;
                    break;
                case ShaderStage.DomainShader:
                    shader = new HashedShader(new D3D11.DomainShader(device, byteCode), hashCode, type);
                    break;
                case ShaderStage.HullShader:
                    shader = new HashedShader(new D3D11.HullShader(device, byteCode), hashCode, type);
                    break;
                case ShaderStage.ComputeShader:
                    shader = new HashedShader(new D3D11.ComputeShader(device, byteCode), hashCode, type);
                    break;
            }

            _shaderMap.Add(hashCode, shader);

            return shader;
        }

        public void AddRef()
        {
            Interlocked.Increment(ref _referenceCount);
        }

        public void Release()
        {
            Interlocked.Decrement(ref _referenceCount);

            // Whoever hits zero first is the one to dispose.
            if (_referenceCount == 0)
            {
                foreach (var kv in _shaderMap)
                {
                    D3D11.DeviceChild shader = kv.Value.Shader;
                    shader?.Dispose();
                }

                _shaderMap.Clear();
            }
        }
    }
}
