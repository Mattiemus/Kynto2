namespace Spark.Direct3D11.Graphics
{
    using System;

    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;

    public sealed class D3D11ShaderStage : IShaderStage
    {
        private readonly int _maxSamplerSlots;

        private readonly SamplerState[] _samplers;
        private readonly IShaderResource[] _resources;
        private readonly D3D11.Buffer[] _nativeConstantBuffers;
        private readonly D3D11.CommonShaderStage _nativeShaderStage;
        private HashedShader _hashedShader;

        // Temp arrays for passing data since SharpDX always reads the input array from index zero.
        private readonly D3D11.SamplerState[] _tempSamplers;
        private readonly D3D11.ShaderResourceView[] _tempResources;
        private readonly D3D11.Buffer[] _tempConstantBuffers;

        public D3D11ShaderStage(D3D11.CommonShaderStage d3dShaderStage, ShaderStage stageType, int maxSamplerSlots, int maxResourceSlots, int maxConstantBufferSlots)
        {
            _maxSamplerSlots = maxSamplerSlots;
            MaximumResourceSlots = maxResourceSlots;
            MaximumConstantBufferSlots = maxConstantBufferSlots;

            _samplers = new SamplerState[maxSamplerSlots];
            _resources = new IShaderResource[maxResourceSlots];
            _nativeConstantBuffers = new D3D11.Buffer[maxConstantBufferSlots];
            StageType = stageType;
            _nativeShaderStage = d3dShaderStage;
            _hashedShader = new HashedShader(null, 0, StageType);

            _tempSamplers = new D3D11.SamplerState[maxSamplerSlots];
            _tempResources = new D3D11.ShaderResourceView[maxResourceSlots];
            _tempConstantBuffers = new D3D11.Buffer[maxConstantBufferSlots];
        }

        public int MaximumSamplerSlots => _samplers.Length;

        public int MaximumResourceSlots { get; }

        public int MaximumConstantBufferSlots { get; }

        public ShaderStage StageType { get; }

        #region Samplers

        public void SetSampler(SamplerState sampler)
        {
            if (sampler == null)
            {
                sampler = SamplerState.PointClamp;
            }

            if (!sampler.IsSameState(_samplers[0]))
            {
                _samplers[0] = sampler;
                _nativeShaderStage.SetSampler(0, (sampler.Implementation as ID3D11SamplerState).D3DSamplerState);
            }
        }

        public void SetSampler(int slotIndex, SamplerState sampler)
        {
            if (slotIndex < 0 || slotIndex >= _maxSamplerSlots)
            {
                return;
            }

            if (sampler == null)
            {
                sampler = SamplerState.PointClamp;
            }

            if (!sampler.IsSameState(_samplers[slotIndex]))
            {
                _samplers[slotIndex] = sampler;
                _nativeShaderStage.SetSampler(slotIndex, (sampler.Implementation as ID3D11SamplerState).D3DSamplerState);
            }
        }

        public void SetSamplers(params SamplerState[] samplers)
        {
            SetSamplers(0, samplers);
        }

        public void SetSamplers(int startSlotIndex, params SamplerState[] samplers)
        {
            int count = (samplers != null) ? samplers.Length : 0;
            if (samplers == null || count == 0 || count > _maxSamplerSlots)
            {
                return;
            }

            int applyCount = 0;
            for (int i = 0; i < count; i++)
            {
                int slotIndex = startSlotIndex + i;
                SamplerState sampler = samplers[i];
                if (sampler == null)
                {
                    sampler = SamplerState.PointClamp;
                }

                // Set to temporary array
                _tempSamplers[slotIndex] = (sampler.Implementation as ID3D11SamplerState).D3DSamplerState;

                // Check if the sampler is already set at the index, if so then we may -not- have to apply. But we need to keep checking.
                // If at least one sampler isn't set, we need to reset all. If all are already there, we can ignore setting (but still need to
                // clear the temp array)
                if (!sampler.IsSameState(_samplers[slotIndex]))
                {
                    _samplers[slotIndex] = sampler;
                    applyCount++;
                }
            }

            if (applyCount > 0)
            {
                _nativeShaderStage.SetSamplers(startSlotIndex, count, _tempSamplers);
            }

            Array.Clear(_tempSamplers, startSlotIndex, count);
        }

        public SamplerState[] GetSamplers(int startSlotIndex, int count)
        {
            if (startSlotIndex < 0 || count <= 0 || (startSlotIndex + count) > _maxSamplerSlots)
            {
                return new SamplerState[0];
            }

            var samplers = new SamplerState[count];
            Array.Copy(_samplers, startSlotIndex, samplers, 0, count);

            return samplers;
        }

        #endregion

        #region Resources

        public void SetShaderResource(IShaderResource resource)
        {
            if (!ReferenceEquals(_resources[0], resource))
            {
                _resources[0] = resource;
                _nativeShaderStage.SetShaderResource(0, Direct3DHelper.GetD3DShaderResourceView(resource));
            }
        }

        public void SetShaderResource(int slotIndex, IShaderResource resource)
        {
            if (slotIndex < 0 || slotIndex >= MaximumResourceSlots)
            {
                return;
            }

            if (!ReferenceEquals(_resources[slotIndex], resource))
            {
                _resources[slotIndex] = resource;
                _nativeShaderStage.SetShaderResource(slotIndex, Direct3DHelper.GetD3DShaderResourceView(resource));
            }
        }

        public void SetShaderResources(params IShaderResource[] resources)
        {
            SetShaderResources(0, resources);
        }

        public void SetShaderResources(int startSlotIndex, params IShaderResource[] resources)
        {
            int count = (resources != null) ? resources.Length : 0;
            if (resources == null || count == 0 || startSlotIndex < 0 || (startSlotIndex + count) > MaximumResourceSlots)
            {
                return;
            }

            int applyCount = 0;
            for (int i = 0; i < count; i++)
            {
                int slotIndex = startSlotIndex + i;
                IShaderResource resource = resources[i];

                // Set to temporary array
                _tempResources[slotIndex] = Direct3DHelper.GetD3DShaderResourceView(resource);

                // Check if the sampler is already set at the index, if so then we may -not- have to apply. But we need to keep checking.
                // If at least one sampler isn't set, we need to reset all. If all are already there, we can ignore setting (but still need to
                // clear the temp array)
                if (!ReferenceEquals(_resources[slotIndex], resource))
                {
                    _resources[slotIndex] = resource;
                    applyCount++;
                }
            }

            if (applyCount > 0)
            {
                _nativeShaderStage.SetShaderResources(startSlotIndex, count, _tempResources);
            }

            Array.Clear(_tempResources, startSlotIndex, count);
        }

        public IShaderResource[] GetShaderResources(int startSlotIndex, int count)
        {
            if (startSlotIndex < 0 || count <= 0 || (startSlotIndex + count) > MaximumResourceSlots)
            {
                return new IShaderResource[0];
            }

            var resources = new IShaderResource[count];
            Array.Copy(_resources, startSlotIndex, resources, 0, count);

            return resources;
        }

        #endregion

        #region Constant Buffers

        public void SetConstantBuffer(int slotIndex, D3D11.Buffer constantBuffer)
        {
            if (slotIndex < 0 || slotIndex >= MaximumConstantBufferSlots)
            {
                return;
            }

            if (!ReferenceEquals(_nativeConstantBuffers[slotIndex], constantBuffer))
            {
                _nativeConstantBuffers[slotIndex] = constantBuffer;
                _nativeShaderStage.SetConstantBuffer(slotIndex, constantBuffer);
            }
        }

        public void SetConstantBuffers(int startSlotIndex, params D3D11.Buffer[] constantBuffers)
        {
            int count = (constantBuffers != null) ? constantBuffers.Length : 0;
            if (constantBuffers == null || count == 0 || startSlotIndex < 0 || (count + startSlotIndex) > MaximumConstantBufferSlots)
            {
                return;
            }

            int applyCount = 0;
            for (int i = 0; i < count; i++)
            {
                int slotIndex = startSlotIndex + i;
                D3D11.Buffer cb = constantBuffers[i];

                // Set to temporary array
                _tempConstantBuffers[slotIndex] = cb;

                // Check if the CB is already set at the index, if so then we may -not- have to apply. But we need to keep checking.
                // If at least one CB isn't set, we need to reset all. If all are already there, we can ignore setting (but still need to
                // clear the temp array)
                if (!ReferenceEquals(_nativeConstantBuffers[slotIndex], cb))
                {
                    _nativeConstantBuffers[slotIndex] = cb;
                    applyCount++;
                }
            }

            if (applyCount > 0)
            {
                _nativeShaderStage.SetConstantBuffers(startSlotIndex, count, _tempConstantBuffers);
            }

            Array.Clear(_tempConstantBuffers, startSlotIndex, count);
        }

        public D3D11.Buffer[] GetConstantBuffers(int startSlotIndex, int count)
        {
            if (startSlotIndex < 0 || count <= 0 || (startSlotIndex + count) > MaximumConstantBufferSlots)
            {
                return new D3D11.Buffer[0];
            }

            var constantBuffers = new D3D11.Buffer[count];
            Array.Copy(_nativeConstantBuffers, startSlotIndex, constantBuffers, 0, count);

            return constantBuffers;
        }

        #endregion

        #region Shader Setting

        public void SetShader(HashedShader shader)
        {
            if (StageType == shader.ShaderType && _hashedShader.HashCode != shader.HashCode)
            {
                _hashedShader = shader;

                switch (StageType)
                {
                    case ShaderStage.VertexShader:
                        (_nativeShaderStage as D3D11.CommonShaderStage<D3D11.VertexShader>).Set(shader.Shader as D3D11.VertexShader);
                        break;
                    case ShaderStage.PixelShader:
                        (_nativeShaderStage as D3D11.CommonShaderStage<D3D11.PixelShader>).Set(shader.Shader as D3D11.PixelShader);
                        break;
                    case ShaderStage.GeometryShader:
                        (_nativeShaderStage as D3D11.CommonShaderStage<D3D11.GeometryShader>).Set(shader.Shader as D3D11.GeometryShader);
                        break;
                    case ShaderStage.HullShader:
                        (_nativeShaderStage as D3D11.CommonShaderStage<D3D11.HullShader>).Set(shader.Shader as D3D11.HullShader);
                        break;
                    case ShaderStage.DomainShader:
                        (_nativeShaderStage as D3D11.CommonShaderStage<D3D11.DomainShader>).Set(shader.Shader as D3D11.DomainShader);
                        break;
                    case ShaderStage.ComputeShader:
                        (_nativeShaderStage as D3D11.CommonShaderStage<D3D11.ComputeShader>).Set(shader.Shader as D3D11.ComputeShader);
                        break;
                }
            }
        }

        #endregion

        public void ClearState()
        {
            Array.Clear(_samplers, 0, _samplers.Length);
            Array.Clear(_resources, 0, _resources.Length);
            Array.Clear(_nativeConstantBuffers, 0, _nativeConstantBuffers.Length);

            Array.Clear(_tempSamplers, 0, _tempSamplers.Length);
            Array.Clear(_tempResources, 0, _tempResources.Length);
            Array.Clear(_tempConstantBuffers, 0, _tempConstantBuffers.Length);

            _hashedShader = new HashedShader(null, 0, StageType);
        }
    }
}
