namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Math;
    using Spark.Graphics;
    
    internal sealed class ResourceDependency : ShaderResourceDependency
    {
        private readonly IShaderResource[] _resources;

        public ResourceDependency(D3D11EffectResourceManager manager, Range slotRange, ResourceLink[] resourceLinks)
            : base(manager, slotRange, resourceLinks)
        {
            _resources = new IShaderResource[slotRange.Length];
        }

        public override void Apply(D3D11RenderContext renderContext, D3D11ShaderStage stage)
        {
            // Gather from manager, because these are set by the user (except for tbuffers)
            for (int i = 0, index = 0; i < ResourceLinks.Length; i++)
            {
                ResourceLink link = ResourceLinks[i];

                // Since tbuffers are constant buffers with SRVs, we have to determine which resource we need to get, from which array.
                for (int j = link.Range.Start; j < link.Range.End; j++, index++)
                {
                    _resources[index] = (link.IsConstantBuffer) ? ResourceManager.ConstantBuffers[j] as D3D11EffectConstantBuffer : ResourceManager.Resources[j];
                }
            }

            stage.SetShaderResources(SlotRange.Start, _resources);

            // Clear so we don't hold onto them for GC
            Array.Clear(_resources, 0, _resources.Length);
        }

        public override ShaderResourceDependency Clone(D3D11EffectResourceManager manager)
        {
            return new ResourceDependency(manager, SlotRange, ResourceLinks);
        }
    }
}
