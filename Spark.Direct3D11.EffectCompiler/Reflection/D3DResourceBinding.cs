namespace Spark.Direct3D11.Graphics
{
    using D3D = SharpDX.D3DCompiler;

    public class D3DResourceBinding
    {
        internal D3DResourceBinding(D3D.InputBindingDescription resourceBinding)
        {
            Name = resourceBinding.Name;
            InputFlags = (D3DShaderInputFlags)resourceBinding.Flags;
            ReturnType = (D3DResourceReturnType)resourceBinding.ReturnType;
            ResourceType = (D3DShaderInputType)resourceBinding.Type;
            ResourceDimension = (D3DResourceDimension)resourceBinding.Dimension;
            SampleCount = resourceBinding.NumSamples;
            BindPoint = resourceBinding.BindPoint;
            BindCount = resourceBinding.BindCount;
        }

        public string Name { get; }

        public D3DShaderInputType ResourceType { get; }

        public D3DResourceDimension ResourceDimension { get; }

        public D3DResourceReturnType ReturnType { get; }

        public D3DShaderInputFlags InputFlags { get; }

        public int SampleCount { get; }

        public int BindPoint { get; }

        public int BindCount { get; }

        public bool IsConstantBuffer => ResourceType == D3DShaderInputType.ConstantBuffer || ResourceType == D3DShaderInputType.TextureBuffer;

        internal bool AreSame(D3DResourceBinding resourceBinding)
        {
            if (!Name.Equals(resourceBinding.Name) || 
                InputFlags != resourceBinding.InputFlags || 
                ReturnType != resourceBinding.ReturnType || 
                ResourceType != resourceBinding.ResourceType || 
                ResourceDimension != resourceBinding.ResourceDimension || 
                SampleCount != resourceBinding.SampleCount || 
                BindPoint != resourceBinding.BindPoint || 
                BindCount != resourceBinding.BindCount)
            {
                return false;
            }

            return true;
        }
    }
}
