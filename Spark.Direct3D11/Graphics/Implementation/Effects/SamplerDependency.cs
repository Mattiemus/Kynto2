namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Math;
    using Spark.Graphics;
    
    internal sealed class SamplerDependency : ShaderResourceDependency
    {
        private readonly SamplerState[] _samplers;

        public SamplerDependency(D3D11EffectResourceManager manager, Range slotRange, ResourceLink[] resourceLinks)
            : base(manager, slotRange, resourceLinks)
        {
            _samplers = new SamplerState[slotRange.Length];
        }

        public override void Apply(D3D11RenderContext renderContext, D3D11ShaderStage stage)
        {
            // Gather from manager, because these are set by the user
            for (int i = 0, index = 0; i < ResourceLinks.Length; i++)
            {
                ResourceLink link = ResourceLinks[i];
                for (int j = link.Range.Start; j < link.Range.End; j++, index++)
                {
                    _samplers[index] = ResourceManager.Resources[j] as SamplerState;
                }
            }

            stage.SetSamplers(SlotRange.Start, _samplers);

            // Clear so we don't hold onto them for GC
            Array.Clear(_samplers, 0, _samplers.Length);
        }

        public override ShaderResourceDependency Clone(D3D11EffectResourceManager manager)
        {
            return new SamplerDependency(manager, SlotRange, ResourceLinks);
        }
    }
}
