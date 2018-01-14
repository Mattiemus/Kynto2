namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;

    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;

    internal sealed class GSWithSOCache
    {
        private readonly D3D11.Device _device;
        private EffectData.Shader _shader;
        private HashedShader _gsWithSOShader;
        private readonly object _sync;
        private readonly int[] _stride;

        public GSWithSOCache(D3D11.Device device)
        {
            _device = device;
            _sync = new Object();
            _stride = new int[1];
        }

        public void SetShader(EffectData.Shader shader)
        {
            _shader = shader;
        }

        public HashedShader? GetGeometryShaderWithSO(StreamOutputBufferBinding[] soTargets, int numSOTargets)
        {
            // TODO: Only support a single Stream output target for now...
            if (soTargets == null || numSOTargets != 1)
            {
                return null;
            }

            lock (_sync)
            {
                if (_gsWithSOShader.Shader == null && _shader != null)
                {
                    var sig = _shader.OutputSignature;
                    var soEntries = new D3D11.StreamOutputElement[sig.Parameters.Length];

                    int hash = 17;
                    hash = (hash * 31) + _shader.HashCode;

                    for (int i = 0; i < sig.Parameters.Length; i++)
                    {
                        EffectData.SignatureParameter param = sig.Parameters[i];
                        byte numComp = 0;

                        if (param.UsageMask == D3DComponentMaskFlags.All)
                        {
                            numComp = 4;
                        }
                        else
                        {
                            if ((param.UsageMask & D3DComponentMaskFlags.ComponentX) == D3DComponentMaskFlags.ComponentX)
                            {
                                numComp++;
                            }

                            if ((param.UsageMask & D3DComponentMaskFlags.ComponentY) == D3DComponentMaskFlags.ComponentY)
                            {
                                numComp++;
                            }

                            if ((param.UsageMask & D3DComponentMaskFlags.ComponentZ) == D3DComponentMaskFlags.ComponentZ)
                            {
                                numComp++;
                            }

                            if ((param.UsageMask & D3DComponentMaskFlags.ComponentW) == D3DComponentMaskFlags.ComponentW)
                            {
                                numComp++;
                            }
                        }

                        soEntries[i] = new D3D11.StreamOutputElement(param.StreamIndex, param.SemanticName, param.SemanticIndex, 0, numComp, 0);

                        hash = (hash * 31) + param.StreamIndex.GetHashCode();
                        hash = (hash * 31) + param.SemanticName.GetHashCode();
                        hash = (hash * 31) + param.SemanticIndex.GetHashCode();
                        hash = (hash * 31) + numComp;
                    }

                    _stride[0] = soTargets[0].StreamOutputBuffer.VertexLayout.VertexStride;
                    var gs = new D3D11.GeometryShader(_device, _shader.ShaderByteCode, soEntries, _stride, -1);
                    _gsWithSOShader = new HashedShader(gs, hash, ShaderStage.GeometryShader);
                }
            }

            return _gsWithSOShader;
        }
    }
}
