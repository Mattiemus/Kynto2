namespace Spark.Direct3D11.Graphics
{
    public sealed class D3DBoundResource
    { 
        public D3DBoundResource(D3DResourceBinding binding)
        {
            Name = binding.Name;
            InputFlags = binding.InputFlags;
            ReturnType = binding.ReturnType;
            ResourceType = binding.ResourceType;
            ResourceDimension = binding.ResourceDimension;
            SampleCount = binding.SampleCount;
        }

        public string Name { get; set; }

        public D3DResourceReturnType ReturnType { get; set; }

        public D3DShaderInputType ResourceType { get; set; }

        public D3DResourceDimension ResourceDimension { get; set; }

        public D3DShaderInputFlags InputFlags { get; set; }

        public int SampleCount { get; set; }

        public bool IsConstantBuffer => ResourceType == D3DShaderInputType.ConstantBuffer || ResourceType == D3DShaderInputType.TextureBuffer;

        public bool AreSame(D3DBoundResource resourceVariable)
        {
            if (!Name.Equals(resourceVariable.Name) || 
                InputFlags != resourceVariable.InputFlags || 
                ReturnType != resourceVariable.ReturnType || 
                ResourceType != resourceVariable.ResourceType || 
                ResourceDimension != resourceVariable.ResourceDimension ||
                SampleCount != resourceVariable.SampleCount)
            {
                return false;
            }

            return true;
        }
    }
}
