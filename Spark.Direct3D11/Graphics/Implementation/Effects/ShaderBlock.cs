namespace Spark.Direct3D11.Graphics.Implementation
{
    using System.Collections.Generic;

    using Spark.Math;
    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;

    internal sealed class ShaderBlock
    {
        private static readonly ShaderResourceDependency[] EmptyDependencyArray = new ShaderResourceDependency[0];
        
        private readonly GSWithSOCache _gsHelper;

        private ShaderBlock(ShaderBlock toCloneFrom, D3D11EffectResourceManager resourceManager)
        {
            Shader = toCloneFrom.Shader;
            Dependencies = EmptyDependencyArray;
            _gsHelper = toCloneFrom._gsHelper;

            if (toCloneFrom.Dependencies.Length > 0)
            {
                Dependencies = new ShaderResourceDependency[toCloneFrom.Dependencies.Length];
                for (int i = 0; i < Dependencies.Length; i++)
                {
                    Dependencies[i] = toCloneFrom.Dependencies[i].Clone(resourceManager);
                }
            }
        }

        public ShaderBlock(D3D11.Device device, EffectData.Shader shader, D3D11EffectResourceManager resourceManager)
        {
            Shader = resourceManager.ShaderManager.GetOrCreateShader(device, shader);
            Dependencies = EmptyDependencyArray;
            _gsHelper = Shader.Shader.Tag as GSWithSOCache;

            if (shader.BoundResources != null && shader.BoundResources.Length > 0)
            {
                EffectData.BoundResource[] boundResources = shader.BoundResources;

                if (boundResources.Length == 1)
                {
                    var firstResource = boundResources[0];
                    var slotRange = new Range(firstResource.BindPoint, firstResource.BindPoint + firstResource.BindCount);
                    var inputType = firstResource.ResourceType;

                    Dependencies = new ShaderResourceDependency[]
                    {
                        CreateDependency(resourceManager, inputType, slotRange, new ResourceLink[]
                        {
                            CreateResourceLink(ref firstResource)
                        })
                    };
                }
                else
                {
                    var dependencies = new List<ShaderResourceDependency>();
                    var tempLinks = new List<ResourceLink>();

                    var firstResource = boundResources[0];
                    var slotRange = new Range(firstResource.BindPoint, firstResource.BindPoint + firstResource.BindCount);
                    var inputType = firstResource.ResourceType;
                    tempLinks.Add(CreateResourceLink(ref firstResource));

                    for (int i = 1; i < boundResources.Length; i++)
                    {
                        var nextResource = boundResources[i];

                        if (slotRange.End == nextResource.BindPoint && IsCompatibleType(inputType, nextResource.ResourceType))
                        {
                            slotRange.Expand(nextResource.BindCount);
                            tempLinks.Add(CreateResourceLink(ref nextResource));
                        }
                        else
                        {
                            dependencies.Add(CreateDependency(resourceManager, inputType, slotRange, tempLinks.ToArray()));
                            tempLinks.Clear();

                            slotRange = new Range(nextResource.BindPoint, nextResource.BindPoint + nextResource.BindCount);
                            inputType = nextResource.ResourceType;
                            tempLinks.Add(CreateResourceLink(ref nextResource));
                        }
                    }

                    if (tempLinks.Count > 0)
                    {
                        dependencies.Add(CreateDependency(resourceManager, inputType, slotRange, tempLinks.ToArray()));
                    }

                    Dependencies = dependencies.ToArray();
                }
            }
        }

        public HashedShader Shader { get; set; }

        public ShaderResourceDependency[] Dependencies { get; set; }

        public void Apply(D3D11RenderContext renderContext)
        {
            var stage = renderContext.GetShaderStage(Shader.ShaderType) as D3D11ShaderStage;
            for (int i = 0; i < Dependencies.Length; i++)
            {
                Dependencies[i].Apply(renderContext, stage);
            }

            if (Shader.ShaderType == ShaderStage.GeometryShader && _gsHelper != null)
            {
                // If a geometry shader, determine if we have stream outputs...if so, then we need to create (or get a cached) geometry shader
                // that was created with Stream Out.
                StreamOutputBufferBinding[] soTargets = renderContext.GetBoundSOWithoutCopy(out int numSO);
                HashedShader? shaderWithSO = _gsHelper.GetGeometryShaderWithSO(soTargets, numSO);

                stage.SetShader((shaderWithSO.HasValue) ? shaderWithSO.Value : Shader);
            }
            else
            {
                stage.SetShader(Shader);
            }
        }

        public ShaderBlock Clone(D3D11EffectResourceManager manager)
        {
            return new ShaderBlock(this, manager);
        }

        private ResourceLink CreateResourceLink(ref EffectData.BoundResource boundResource)
        {
            var range = new Range(boundResource.ResourceIndex, boundResource.ResourceIndex + boundResource.BindCount);
            bool isConstantBuffer = boundResource.ResourceType == D3DShaderInputType.ConstantBuffer || boundResource.ResourceType == D3DShaderInputType.TextureBuffer;

            return new ResourceLink(range, isConstantBuffer);
        }

        private ShaderResourceDependency CreateDependency(D3D11EffectResourceManager manager, D3DShaderInputType type, Range slotRange, ResourceLink[] resourceLinks)
        {
            switch (type)
            {
                case D3DShaderInputType.Sampler:
                    return new SamplerDependency(manager, slotRange, resourceLinks);
                case D3DShaderInputType.ConstantBuffer:
                    return new ConstantBufferDependency(manager, slotRange, resourceLinks);
                default:
                    return new ResourceDependency(manager, slotRange, resourceLinks);
            }
        }

        private bool IsCompatibleType(D3DShaderInputType prevType, D3DShaderInputType nextType)
        {
            if (prevType == nextType)
            {
                return true;
            }

            switch (prevType)
            {
                //Bit of a TODO, for UAVs I guess
                //case D3DShaderInputType.AppendStructured:
                //case D3DShaderInputType.ByteAddress:
                ///case D3DShaderInputType.ConsumeStructured:
                //case D3DShaderInputType.RWByteAddress:
                //case D3DShaderInputType.RWStructured:
                //case D3DShaderInputType.RWStructuredWithCounter:
                //case D3DShaderInputType.RWTyped:
                //case D3DShaderInputType.Structured:
                case D3DShaderInputType.Texture:
                case D3DShaderInputType.TextureBuffer:
                    switch (nextType)
                    {
                        //case D3DShaderInputType.AppendStructured:
                        //case D3DShaderInputType.ByteAddress:
                        //case D3DShaderInputType.ConsumeStructured:
                        //case D3DShaderInputType.RWByteAddress:
                        //case D3DShaderInputType.RWStructured:
                        //case D3DShaderInputType.RWStructuredWithCounter:
                        //case D3DShaderInputType.RWTyped:
                        //case D3DShaderInputType.Structured:
                        case D3DShaderInputType.Texture:
                        case D3DShaderInputType.TextureBuffer:
                            return true;
                        default:
                            return false;
                    }
                case D3DShaderInputType.ConstantBuffer:
                    switch (nextType)
                    {
                        case D3DShaderInputType.ConstantBuffer:
                            return true;
                        default:
                            return false;
                    }
                case D3DShaderInputType.Sampler:
                    switch (nextType)
                    {
                        case D3DShaderInputType.Sampler:
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }
    }
}
