namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Math;

    internal abstract class ShaderResourceDependency
    {
        protected ShaderResourceDependency(D3D11EffectResourceManager manager, Range slotRange, ResourceLink[] resourceLinks)
        {
            ResourceManager = manager;
            SlotRange = slotRange;
            ResourceLinks = resourceLinks;
        }

        public D3D11EffectResourceManager ResourceManager { get; set; }

        /// <summary>
        /// Gets or sets contigious range over the shader stage slots (Start is the first slot index, and length is the bind count)
        /// </summary>
        public Range SlotRange { get; set; }

        /// <summary>
        /// Gets or sets indices to either the Resources or ConstantBuffers collections in the manager
        /// </summary>
        public ResourceLink[] ResourceLinks { get; set; }

        public abstract void Apply(D3D11RenderContext renderContext, D3D11ShaderStage stage);
        public abstract ShaderResourceDependency Clone(D3D11EffectResourceManager manager);
    }
}
