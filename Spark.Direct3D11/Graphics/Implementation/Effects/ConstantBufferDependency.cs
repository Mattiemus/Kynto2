namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Math;

    using D3D11 = SharpDX.Direct3D11;

    internal sealed class ConstantBufferDependency : ShaderResourceDependency
    {
        private readonly D3D11.Buffer[] _buffers;

        public ConstantBufferDependency(D3D11EffectResourceManager manager, Range slotRange, ResourceLink[] resourceLinks)
            : base(manager, slotRange, resourceLinks)
        {
            _buffers = new D3D11.Buffer[SlotRange.Length];
        }

        public override void Apply(D3D11RenderContext renderContext, D3D11ShaderStage stage)
        {
            D3D11.DeviceContext deviceContext = renderContext.D3DDeviceContext;

            // Need to iterate over the buffers anyways to update them before setting
            for (int i = 0, index = 0; i < ResourceLinks.Length; i++)
            {
                ResourceLink link = ResourceLinks[i];
                for (int j = link.Range.Start; j < link.Range.End; j++, index++)
                {
                    var cb = ResourceManager.ConstantBuffers[j] as D3D11EffectConstantBuffer;
                    cb.Update(deviceContext);

                    _buffers[index] = cb.D3DBuffer;
                }
            }

            stage.SetConstantBuffers(SlotRange.Start, _buffers);

            Array.Clear(_buffers, 0, _buffers.Length);
        }

        public override ShaderResourceDependency Clone(D3D11EffectResourceManager manager)
        {
            return new ConstantBufferDependency(manager, SlotRange, ResourceLinks);
        }
    }
}
