namespace Spark.Direct3D11.Graphics
{
    using System;

    using Spark.Math;
    using Spark.Graphics;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;
    using DXGI = SharpDX.DXGI;
    using SD = System.Drawing;
    using SDX = SharpDX;
    using SDXM = SharpDX.Mathematics.Interop;

    public static class Direct3DHelper
    {

        #region Helper Methods

        public static bool IsFlagSet(int flag, int mask)
        {
            return ((flag & mask) == mask);
        }

        public static T Cast<T>(object obj) where T : class
        {
            T cast = obj as T;
            if (cast == null)
            {
                throw new InvalidCastException("Cannot cast object");
            }

            return cast;
        }

        public static D3D11.ShaderResourceView GetD3DShaderResourceView(IShaderResource shaderResource)
        {
            // Check if cast directly
            ID3D11ShaderResourceView srv = shaderResource as ID3D11ShaderResourceView;

            // Check if a GraphicsResource
            if (srv == null && shaderResource is GraphicsResource)
            {
                srv = (shaderResource as GraphicsResource).Implementation as ID3D11ShaderResourceView;
            }

            if (srv != null)
            {
                return srv.D3DShaderResourceView;
            }

            return null;
        }

        public static D3D11.RenderTargetView GetD3DRenderTargetView(IRenderTarget renderTarget)
        {
            // Check if cast directly
            ID3D11RenderTargetView rtv = renderTarget as ID3D11RenderTargetView;

            // Check if a GraphicsResource
            if (rtv == null && renderTarget is GraphicsResource)
            {
                rtv = (renderTarget as GraphicsResource).Implementation as ID3D11RenderTargetView;
            }

            if (rtv != null)
            {
                return rtv.D3DRenderTargetView;
            }

            return null;
        }

        public static D3D11.DepthStencilView GetD3DDepthStencilView(IDepthStencilBuffer depthBuffer)
        {
            // Check if cast directly
            ID3D11DepthStencilView dsv = depthBuffer as ID3D11DepthStencilView;

            //Forgo GraphicsResource check, since we create depth buffers indirectly
            if (dsv != null)
            {
                return dsv.D3DDepthStencilView;
            }

            return null;
        }

        public static D3D11.SamplerState GetD3DSamplerState(SamplerState samplerState)
        {
            if (samplerState == null)
            {
                return null;
            }

            return (samplerState.Implementation as ID3D11SamplerState).D3DSamplerState;
        }

        public static D3D11.BlendState GetD3DBlendState(BlendState blendState)
        {
            if (blendState == null)
            {
                return null;
            }

            return (blendState.Implementation as ID3D11BlendState).D3DBlendState;
        }

        public static D3D11.RasterizerState GetD3DRasterizerState(RasterizerState rasterizerState)
        {
            if (rasterizerState == null)
            {
                return null;
            }

            return (rasterizerState.Implementation as ID3D11RasterizerState).D3DRasterizerState;
        }

        public static D3D11.DepthStencilState GetD3DDepthStencilState(DepthStencilState depthStencilState)
        {
            if (depthStencilState == null)
            {
                return null;
            }

            return (depthStencilState.Implementation as ID3D11DepthStencilState).D3DDepthStencilState;
        }

        public static D3D11.DeviceContext GetD3DDeviceContext(IRenderContext renderContext)
        {
            D3D11RenderContext d3d11Context = renderContext as D3D11RenderContext;
            if (d3d11Context == null)
            {
                throw new SparkGraphicsException("Must be a Direct3D11 render context");
            }

            return d3d11Context.D3DDeviceContext;
        }

        #endregion

        #region Object Checkers

        public static void CheckDisposed(IDisposable disposable)
        {
            if (disposable == null)
            {
                throw new ObjectDisposedException(nameof(disposable));
            }
        }

        public static void CheckDataBufferNull(IReadOnlyDataBuffer data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer cannot be null");
            }
        }

        public static void CheckResourceBounds(IReadOnlyDataBuffer data, int index, int count)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer cannot be null");
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to zero");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            if (index + count > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index and count out of range");
            }
        }

        public static void CheckMipLevels(int mipLevel, int mipMapCount)
        {
            if (mipLevel < 0 || mipLevel >= mipMapCount)
            {
                throw new ArgumentOutOfRangeException(nameof(mipLevel), "Mip level is out of range");
            }
        }

        public static void CheckArraySlice(int arraySlice, int arrayCount)
        {
            if (arraySlice < 0 || arraySlice >= arrayCount)
            {
                throw new ArgumentOutOfRangeException(nameof(arraySlice), "Array slice is out of range");
            }
        }

        public static void CheckIfImmutable(ResourceUsage resourceUsage)
        {
            if (resourceUsage == ResourceUsage.Immutable)
            {
                throw new SparkGraphicsException("Cannot write to immutable resource");
            }
        }

        public static void CheckFormatSize(int formatSizeInBytes, int elementSizeInBytes)
        {
            if (formatSizeInBytes != elementSizeInBytes && ((formatSizeInBytes <= elementSizeInBytes) || (formatSizeInBytes % elementSizeInBytes) != 0))
            {
                throw new ArgumentOutOfRangeException(nameof(elementSizeInBytes), "Invalid element data size");
            }
        }

        public static void CheckSubresourceDataSizeToCopy(SurfaceFormat format, ref int formatSizeInBytes, ref int width, ref int height, int elementSizeInBytes, int elementCount)
        {
            if (format.IsCompressedFormat())
            {
                Texture.CalculateCompressedDimensions(format, ref width, ref height, out formatSizeInBytes);
            }

            int totalSize = width * height * formatSizeInBytes;
            if ((elementSizeInBytes * elementCount) != totalSize)
            {
                throw new ArgumentOutOfRangeException(nameof(elementCount), "Invalid size of data to copy");
            }
        }

        #endregion

        #region Struct conversions

        public static void ConvertRectangle(ref SDXM.RawRectangle rectangle, out Rectangle result)
        {
            result = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Right - rectangle.Left, rectangle.Bottom - rectangle.Top);
        }

        public static void ConvertRectangle(ref SD.Rectangle rectangle, out Rectangle result)
        {
            result = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static void ConvertColor(ref Color color, out SDXM.RawColor4 sdxColor)
        {
            Vector4 clamped = color.ToVector4();
            sdxColor = new SDXM.RawColor4(clamped.X, clamped.Y, clamped.Z, clamped.W);
        }

        public static void ConvertViewport(ref Viewport vp, out SDXM.RawViewport result)
        {            
            result = new SDXM.RawViewport();
            result.X = vp.X;
            result.Y = vp.Y;
            result.Width = vp.Width;
            result.Height = vp.Height;
            result.MinDepth = vp.MinDepth;
            result.MaxDepth = vp.MaxDepth;
        }

        public static void ConvertRenderTargetBlendDescription(ref RenderTargetBlendDescription blendDesc, out D3D11.RenderTargetBlendDescription d3dBlendDesc)
        {
            d3dBlendDesc.AlphaBlendOperation = ToD3DBlendOperation(blendDesc.AlphaBlendFunction);
            d3dBlendDesc.SourceAlphaBlend = ToD3DBlendOption(blendDesc.AlphaSourceBlend);
            d3dBlendDesc.DestinationAlphaBlend = ToD3DBlendOption(blendDesc.AlphaDestinationBlend);

            d3dBlendDesc.BlendOperation = ToD3DBlendOperation(blendDesc.ColorBlendFunction);
            d3dBlendDesc.SourceBlend = ToD3DBlendOption(blendDesc.ColorSourceBlend);
            d3dBlendDesc.DestinationBlend = ToD3DBlendOption(blendDesc.ColorDestinationBlend);

            d3dBlendDesc.IsBlendEnabled = blendDesc.BlendEnable;
            d3dBlendDesc.RenderTargetWriteMask = ToD3DColorWriteMaskFlags(blendDesc.WriteChannels);
        }

        #endregion

        #region Enum Conversions

        #region To/From D3D Vertex Semantic

        public static String ToD3DVertexSemantic(VertexSemantic semantic)
        {
            switch (semantic)
            {
                case VertexSemantic.Position:
                    return "POSITION";
                case VertexSemantic.Color:
                    return "COLOR";
                case VertexSemantic.TextureCoordinate:
                    return "TEXCOORD";
                case VertexSemantic.Normal:
                    return "NORMAL";
                case VertexSemantic.Tangent:
                    return "TANGENT";
                case VertexSemantic.Bitangent:
                    return "BITANGENT";
                case VertexSemantic.BlendIndices:
                    return "BLENDINDICES";
                case VertexSemantic.BlendWeight:
                    return "BLENDWEIGHT";
                case VertexSemantic.UserDefined:
                    return "USERDEFINED";
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static VertexSemantic FromD3DVertexSemantic(string semantic)
        {
            switch (semantic)
            {
                case "POSITION":
                    return VertexSemantic.Position;
                case "COLOR":
                    return VertexSemantic.Color;
                case "TEXCOORD":
                    return VertexSemantic.TextureCoordinate;
                case "NORMAL":
                    return VertexSemantic.Normal;
                case "TANGENT":
                    return VertexSemantic.Tangent;
                case "BITANGENT":
                    return VertexSemantic.Bitangent;
                case "BLENDINDICES":
                    return VertexSemantic.BlendIndices;
                case "BLENDWEIGHT":
                    return VertexSemantic.BlendWeight;
                case "USERDEFINED":
                    return VertexSemantic.UserDefined;
                default:
                    throw new SparkGraphicsException("Invalid semantic name");
            }
        }

        #endregion

        #region To/From D3D Vertex Format

        public static DXGI.Format ToD3DVertexFormat(VertexFormat format)
        {
            switch (format)
            {
                case VertexFormat.Color:
                    return DXGI.Format.R8G8B8A8_UNorm;
                case VertexFormat.UShort:
                    return DXGI.Format.R16_UInt;
                case VertexFormat.UShort2:
                    return DXGI.Format.R16G16_UInt;
                case VertexFormat.UShort4:
                    return DXGI.Format.R16G16B16A16_UInt;
                case VertexFormat.NormalizedUShort:
                    return DXGI.Format.R16_UNorm;
                case VertexFormat.NormalizedUShort2:
                    return DXGI.Format.R16G16_UNorm;
                case VertexFormat.NormalizedUShort4:
                    return DXGI.Format.R16G16B16A16_UNorm;
                case VertexFormat.Short:
                    return DXGI.Format.R16_SInt;
                case VertexFormat.Short2:
                    return DXGI.Format.R16G16_SInt;
                case VertexFormat.Short4:
                    return DXGI.Format.R16G16B16A16_SInt;
                case VertexFormat.NormalizedShort:
                    return DXGI.Format.R16_SNorm;
                case VertexFormat.NormalizedShort2:
                    return DXGI.Format.R16G16_SNorm;
                case VertexFormat.NormalizedShort4:
                    return DXGI.Format.R16G16B16A16_SNorm;
                case VertexFormat.UInt:
                    return DXGI.Format.R32_UInt;
                case VertexFormat.UInt2:
                    return DXGI.Format.R32G32_UInt;
                case VertexFormat.UInt3:
                    return DXGI.Format.R32G32B32_UInt;
                case VertexFormat.UInt4:
                    return DXGI.Format.R32G32B32A32_UInt;
                case VertexFormat.Int:
                    return DXGI.Format.R32_SInt;
                case VertexFormat.Int2:
                    return DXGI.Format.R32G32_SInt;
                case VertexFormat.Int3:
                    return DXGI.Format.R32G32B32_SInt;
                case VertexFormat.Int4:
                    return DXGI.Format.R32G32B32A32_SInt;
                case VertexFormat.Half:
                    return DXGI.Format.R16_Float;
                case VertexFormat.Half2:
                    return DXGI.Format.R16G16_Float;
                case VertexFormat.Half4:
                    return DXGI.Format.R16G16B16A16_Float;
                case VertexFormat.Float:
                    return DXGI.Format.R32_Float;
                case VertexFormat.Float2:
                    return DXGI.Format.R32G32_Float;
                case VertexFormat.Float3:
                    return DXGI.Format.R32G32B32_Float;
                case VertexFormat.Float4:
                    return DXGI.Format.R32G32B32A32_Float;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static VertexFormat FromD3DVertexFormat(DXGI.Format format)
        {
            switch (format)
            {
                case DXGI.Format.R8G8B8A8_UNorm:
                    return VertexFormat.Color;
                case DXGI.Format.R16_UInt:
                    return VertexFormat.UShort;
                case DXGI.Format.R16G16_UInt:
                    return VertexFormat.UShort2;
                case DXGI.Format.R16G16B16A16_UInt:
                    return VertexFormat.UShort4;
                case DXGI.Format.R16_UNorm:
                    return VertexFormat.NormalizedUShort;
                case DXGI.Format.R16G16_UNorm:
                    return VertexFormat.NormalizedUShort2;
                case DXGI.Format.R16G16B16A16_UNorm:
                    return VertexFormat.NormalizedUShort4;
                case DXGI.Format.R16_SInt:
                    return VertexFormat.Short;
                case DXGI.Format.R16G16_SInt:
                    return VertexFormat.Short2;
                case DXGI.Format.R16G16B16A16_SInt:
                    return VertexFormat.Short4;
                case DXGI.Format.R16_SNorm:
                    return VertexFormat.NormalizedShort;
                case DXGI.Format.R16G16_SNorm:
                    return VertexFormat.NormalizedShort2;
                case DXGI.Format.R16G16B16A16_SNorm:
                    return VertexFormat.NormalizedShort4;
                case DXGI.Format.R32_UInt:
                    return VertexFormat.UInt;
                case DXGI.Format.R32G32_UInt:
                    return VertexFormat.UInt2;
                case DXGI.Format.R32G32B32_UInt:
                    return VertexFormat.UInt3;
                case DXGI.Format.R32G32B32A32_UInt:
                    return VertexFormat.UInt4;
                case DXGI.Format.R32_SInt:
                    return VertexFormat.Int;
                case DXGI.Format.R32G32_SInt:
                    return VertexFormat.Int2;
                case DXGI.Format.R32G32B32_SInt:
                    return VertexFormat.Int3;
                case DXGI.Format.R32G32B32A32_SInt:
                    return VertexFormat.Int4;
                case DXGI.Format.R16_Float:
                    return VertexFormat.Half;
                case DXGI.Format.R16G16_Float:
                    return VertexFormat.Half2;
                case DXGI.Format.R16G16B16A16_Float:
                    return VertexFormat.Half4;
                case DXGI.Format.R32_Float:
                    return VertexFormat.Float;
                case DXGI.Format.R32G32_Float:
                    return VertexFormat.Float2;
                case DXGI.Format.R32G32B32_Float:
                    return VertexFormat.Float3;
                case DXGI.Format.R32G32B32A32_Float:
                    return VertexFormat.Float4;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Index Format

        public static DXGI.Format ToD3DIndexFormat(IndexFormat format)
        {
            switch (format)
            {
                case IndexFormat.SixteenBits:
                    return DXGI.Format.R16_UInt;
                case IndexFormat.ThirtyTwoBits:
                    return DXGI.Format.R32_UInt;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static IndexFormat FromD3DIndexFormat(DXGI.Format format)
        {
            switch (format)
            {
                case DXGI.Format.R16_UInt:
                    return IndexFormat.SixteenBits;
                case DXGI.Format.R32_UInt:
                    return IndexFormat.ThirtyTwoBits;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Shader Variable Type

        public static D3DShaderVariableType ToD3DShaderVariableType(EffectParameterType variableType)
        {
            switch (variableType)
            {
                case EffectParameterType.Bool:
                    return D3DShaderVariableType.Bool;
                case EffectParameterType.Int32:
                    return D3DShaderVariableType.Int;
                case EffectParameterType.Single:
                    return D3DShaderVariableType.Float;
                case EffectParameterType.SamplerState:
                    return D3DShaderVariableType.Sampler;
                case EffectParameterType.String:
                    return D3DShaderVariableType.String;
                case EffectParameterType.Texture:
                    return D3DShaderVariableType.Texture;
                case EffectParameterType.Texture1D:
                    return D3DShaderVariableType.Texture1D;
                case EffectParameterType.Texture1DArray:
                    return D3DShaderVariableType.Texture1DArray;
                case EffectParameterType.Texture2D:
                    return D3DShaderVariableType.Texture2D;
                case EffectParameterType.Texture2DArray:
                    return D3DShaderVariableType.Texture2DArray;
                case EffectParameterType.Texture2DMS:
                    return D3DShaderVariableType.Texture2DMultisampled;
                case EffectParameterType.Texture2DMSArray:
                    return D3DShaderVariableType.Texture2DMultisampledArray;
                case EffectParameterType.Texture3D:
                    return D3DShaderVariableType.Texture3D;
                case EffectParameterType.TextureCube:
                    return D3DShaderVariableType.TextureCube;
                case EffectParameterType.Void:
                    return D3DShaderVariableType.Void;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static EffectParameterType FromD3DShaderVariableType(D3DShaderVariableType variableType)
        {
            switch (variableType)
            {
                case D3DShaderVariableType.Bool:
                    return EffectParameterType.Bool;
                case D3DShaderVariableType.Int:
                    return EffectParameterType.Int32;
                case D3DShaderVariableType.Float:
                    return EffectParameterType.Single;
                case D3DShaderVariableType.Sampler:
                case D3DShaderVariableType.Sampler1d:
                case D3DShaderVariableType.Sampler2d:
                case D3DShaderVariableType.Sampler3d:
                case D3DShaderVariableType.Samplercube:
                    return EffectParameterType.SamplerState;
                case D3DShaderVariableType.String:
                    return EffectParameterType.String;
                case D3DShaderVariableType.Texture:
                    return EffectParameterType.Texture;
                case D3DShaderVariableType.RWTexture1D:
                case D3DShaderVariableType.Texture1D:
                    return EffectParameterType.Texture1D;
                case D3DShaderVariableType.RWTexture1DArray:
                case D3DShaderVariableType.Texture1DArray:
                    return EffectParameterType.Texture1DArray;
                case D3DShaderVariableType.RWTexture2D:
                case D3DShaderVariableType.Texture2D:
                    return EffectParameterType.Texture2D;
                case D3DShaderVariableType.RWTexture2DArray:
                case D3DShaderVariableType.Texture2DArray:
                    return EffectParameterType.Texture2DArray;
                case D3DShaderVariableType.Texture2DMultisampled:
                    return EffectParameterType.Texture2DMS;
                case D3DShaderVariableType.Texture2DMultisampledArray:
                    return EffectParameterType.Texture2DMSArray;
                case D3DShaderVariableType.RWTexture3D:
                case D3DShaderVariableType.Texture3D:
                    return EffectParameterType.Texture3D;
                case D3DShaderVariableType.TextureCube:
                    return EffectParameterType.TextureCube;
                case D3DShaderVariableType.Void:
                    return EffectParameterType.Void;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Resource Dimension

        public static D3DResourceDimension ToD3DResourceDimension(EffectParameterType variableType)
        {
            switch (variableType)
            {
                case EffectParameterType.Texture1D:
                    return D3DResourceDimension.Texture1D;
                case EffectParameterType.Texture1DArray:
                    return D3DResourceDimension.Texture1DArray;
                case EffectParameterType.Texture2D:
                    return D3DResourceDimension.Texture2D;
                case EffectParameterType.Texture2DArray:
                    return D3DResourceDimension.Texture2DArray;
                case EffectParameterType.Texture2DMS:
                    return D3DResourceDimension.Texture2DMultisampled;
                case EffectParameterType.Texture2DMSArray:
                    return D3DResourceDimension.Texture2DMultiSampledArray;
                case EffectParameterType.Texture3D:
                    return D3DResourceDimension.Texture3D;
                case EffectParameterType.TextureCube:
                    return D3DResourceDimension.TextureCube;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static EffectParameterType FromD3DResourceDimension(D3DResourceDimension resourceDim)
        {
            switch (resourceDim)
            {
                case D3DResourceDimension.Texture1D:
                    return EffectParameterType.Texture1D;
                case D3DResourceDimension.Texture1DArray:
                    return EffectParameterType.Texture1DArray;
                case D3DResourceDimension.Texture2D:
                    return EffectParameterType.Texture2D;
                case D3DResourceDimension.Texture2DArray:
                    return EffectParameterType.Texture2DArray;
                case D3DResourceDimension.Texture2DMultisampled:
                    return EffectParameterType.Texture2DMS;
                case D3DResourceDimension.Texture2DMultiSampledArray:
                    return EffectParameterType.Texture2DMSArray;
                case D3DResourceDimension.Texture3D:
                    return EffectParameterType.Texture3D;
                case D3DResourceDimension.TextureCube:
                    return EffectParameterType.TextureCube;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Shader Variable Class

        public static D3DShaderVariableClass ToD3DShaderVariableClass(EffectParameterClass variableClass)
        {
            switch (variableClass)
            {
                case EffectParameterClass.MatrixColumns:
                    return D3DShaderVariableClass.MatrixColumns;
                case EffectParameterClass.MatrixRows:
                    return D3DShaderVariableClass.MatrixRows;
                case EffectParameterClass.Object:
                    return D3DShaderVariableClass.Object;
                case EffectParameterClass.Scalar:
                    return D3DShaderVariableClass.Scalar;
                case EffectParameterClass.Struct:
                    return D3DShaderVariableClass.Struct;
                case EffectParameterClass.Vector:
                    return D3DShaderVariableClass.Vector;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static EffectParameterClass FromD3DShaderVariableClass(D3DShaderVariableClass variableClass)
        {
            switch (variableClass)
            {
                case D3DShaderVariableClass.MatrixColumns:
                    return EffectParameterClass.MatrixColumns;
                case D3DShaderVariableClass.MatrixRows:
                    return EffectParameterClass.MatrixRows;
                case D3DShaderVariableClass.Object:
                    return EffectParameterClass.Object;
                case D3DShaderVariableClass.Scalar:
                    return EffectParameterClass.Scalar;
                case D3DShaderVariableClass.Struct:
                    return EffectParameterClass.Struct;
                case D3DShaderVariableClass.Vector:
                    return EffectParameterClass.Vector;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Blend Option

        public static D3D11.BlendOption ToD3DBlendOption(Blend blend)
        {
            switch (blend)
            {
                case Blend.Zero:
                    return D3D11.BlendOption.Zero;
                case Blend.SourceColor:
                    return D3D11.BlendOption.SourceColor;
                case Blend.SourceAlphaSaturation:
                    return D3D11.BlendOption.SourceAlphaSaturate;
                case Blend.SourceAlpha:
                    return D3D11.BlendOption.SourceAlpha;
                case Blend.One:
                    return D3D11.BlendOption.One;
                case Blend.InverseSourceColor:
                    return D3D11.BlendOption.InverseSourceColor;
                case Blend.InverseSourceAlpha:
                    return D3D11.BlendOption.InverseSourceAlpha;
                case Blend.InverseDestinationColor:
                    return D3D11.BlendOption.InverseDestinationColor;
                case Blend.InverseDestinationAlpha:
                    return D3D11.BlendOption.InverseDestinationAlpha;
                case Blend.InverseBlendFactor:
                    return D3D11.BlendOption.InverseBlendFactor;
                case Blend.DestinationColor:
                    return D3D11.BlendOption.DestinationColor;
                case Blend.DestinationAlpha:
                    return D3D11.BlendOption.DestinationAlpha;
                case Blend.BlendFactor:
                    return D3D11.BlendOption.BlendFactor;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static Blend FromD3DBlendOption(D3D11.BlendOption blend)
        {
            switch (blend)
            {
                case D3D11.BlendOption.Zero:
                    return Blend.Zero;
                case D3D11.BlendOption.SourceColor:
                    return Blend.SourceColor;
                case D3D11.BlendOption.SourceAlphaSaturate:
                    return Blend.SourceAlphaSaturation;
                case D3D11.BlendOption.SourceAlpha:
                    return Blend.SourceAlpha;
                case D3D11.BlendOption.One:
                    return Blend.One;
                case D3D11.BlendOption.InverseSourceColor:
                    return Blend.InverseSourceColor;
                case D3D11.BlendOption.InverseSourceAlpha:
                    return Blend.InverseSourceAlpha;
                case D3D11.BlendOption.InverseDestinationColor:
                    return Blend.InverseDestinationColor;
                case D3D11.BlendOption.InverseDestinationAlpha:
                    return Blend.InverseDestinationAlpha;
                case D3D11.BlendOption.InverseBlendFactor:
                    return Blend.InverseBlendFactor;
                case D3D11.BlendOption.DestinationColor:
                    return Blend.DestinationColor;
                case D3D11.BlendOption.DestinationAlpha:
                    return Blend.DestinationAlpha;
                case D3D11.BlendOption.BlendFactor:
                    return Blend.BlendFactor;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Blend Operation

        public static D3D11.BlendOperation ToD3DBlendOperation(BlendFunction func)
        {
            switch (func)
            {
                case BlendFunction.Add:
                    return D3D11.BlendOperation.Add;
                case BlendFunction.Maximum:
                    return D3D11.BlendOperation.Maximum;
                case BlendFunction.Minimum:
                    return D3D11.BlendOperation.Minimum;
                case BlendFunction.ReverseSubtract:
                    return D3D11.BlendOperation.ReverseSubtract;
                case BlendFunction.Subtract:
                    return D3D11.BlendOperation.Subtract;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static BlendFunction FromD3DBlendOperation(D3D11.BlendOperation func)
        {
            switch (func)
            {
                case D3D11.BlendOperation.Add:
                    return BlendFunction.Add;
                case D3D11.BlendOperation.Maximum:
                    return BlendFunction.Maximum;
                case D3D11.BlendOperation.Minimum:
                    return BlendFunction.Minimum;
                case D3D11.BlendOperation.ReverseSubtract:
                    return BlendFunction.ReverseSubtract;
                case D3D11.BlendOperation.Subtract:
                    return BlendFunction.Subtract;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Resource Usage

        public static D3D11.ResourceUsage ToD3DResourceUsage(ResourceUsage resourceUsage)
        {
            switch (resourceUsage)
            {
                case ResourceUsage.Static:
                    return D3D11.ResourceUsage.Default;
                case ResourceUsage.Immutable:
                    return D3D11.ResourceUsage.Immutable;
                case ResourceUsage.Dynamic:
                    return D3D11.ResourceUsage.Dynamic;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static ResourceUsage FromD3DResourceUsage(D3D11.ResourceUsage resourceUsage)
        {
            switch (resourceUsage)
            {
                case D3D11.ResourceUsage.Default:
                    return ResourceUsage.Static;
                case D3D11.ResourceUsage.Immutable:
                    return ResourceUsage.Immutable;
                case D3D11.ResourceUsage.Dynamic:
                    return ResourceUsage.Dynamic;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Color Write Mask Flags

        public static D3D11.ColorWriteMaskFlags ToD3DColorWriteMaskFlags(ColorWriteChannels writeChannels)
        {
            return (D3D11.ColorWriteMaskFlags)writeChannels;
        }

        public static ColorWriteChannels FromD3DColorWriteMaskFlags(D3D11.ColorWriteMaskFlags writeMaskFlags)
        {
            return (ColorWriteChannels)writeMaskFlags;
        }

        #endregion

        #region To/From D3D Comparison

        public static D3D11.Comparison ToD3DComparison(ComparisonFunction func)
        {
            switch (func)
            {
                case ComparisonFunction.Always:
                    return D3D11.Comparison.Always;
                case ComparisonFunction.Equal:
                    return D3D11.Comparison.Equal;
                case ComparisonFunction.Greater:
                    return D3D11.Comparison.Greater;
                case ComparisonFunction.GreaterEqual:
                    return D3D11.Comparison.GreaterEqual;
                case ComparisonFunction.Less:
                    return D3D11.Comparison.Less;
                case ComparisonFunction.LessEqual:
                    return D3D11.Comparison.LessEqual;
                case ComparisonFunction.Never:
                    return D3D11.Comparison.Never;
                case ComparisonFunction.NotEqual:
                    return D3D11.Comparison.NotEqual;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static ComparisonFunction FromD3DComparison(D3D11.Comparison func)
        {
            switch (func)
            {
                case D3D11.Comparison.Always:
                    return ComparisonFunction.Always;
                case D3D11.Comparison.Equal:
                    return ComparisonFunction.Equal;
                case D3D11.Comparison.Greater:
                    return ComparisonFunction.Greater;
                case D3D11.Comparison.GreaterEqual:
                    return ComparisonFunction.GreaterEqual;
                case D3D11.Comparison.Less:
                    return ComparisonFunction.Less;
                case D3D11.Comparison.LessEqual:
                    return ComparisonFunction.LessEqual;
                case D3D11.Comparison.Never:
                    return ComparisonFunction.Never;
                case D3D11.Comparison.NotEqual:
                    return ComparisonFunction.NotEqual;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Stencil Operation

        public static D3D11.StencilOperation ToD3DStencilOperation(StencilOperation operation)
        {
            switch (operation)
            {
                case StencilOperation.Zero:
                    return D3D11.StencilOperation.Zero;
                case StencilOperation.Replace:
                    return D3D11.StencilOperation.Replace;
                case StencilOperation.Keep:
                    return D3D11.StencilOperation.Keep;
                case StencilOperation.Invert:
                    return D3D11.StencilOperation.Invert;
                case StencilOperation.IncrementAndClamp:
                    return D3D11.StencilOperation.IncrementAndClamp;
                case StencilOperation.Increment:
                    return D3D11.StencilOperation.Increment;
                case StencilOperation.DecrementAndClamp:
                    return D3D11.StencilOperation.DecrementAndClamp;
                case StencilOperation.Decrement:
                    return D3D11.StencilOperation.Decrement;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static StencilOperation FromD3DStencilOperation(D3D11.StencilOperation operation)
        {
            switch (operation)
            {
                case D3D11.StencilOperation.Zero:
                    return StencilOperation.Zero;
                case D3D11.StencilOperation.Replace:
                    return StencilOperation.Replace;
                case D3D11.StencilOperation.Keep:
                    return StencilOperation.Keep;
                case D3D11.StencilOperation.Invert:
                    return StencilOperation.Invert;
                case D3D11.StencilOperation.IncrementAndClamp:
                    return StencilOperation.IncrementAndClamp;
                case D3D11.StencilOperation.Increment:
                    return StencilOperation.Increment;
                case D3D11.StencilOperation.DecrementAndClamp:
                    return StencilOperation.DecrementAndClamp;
                case D3D11.StencilOperation.Decrement:
                    return StencilOperation.Decrement;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Primitive Topology

        public static D3D.PrimitiveTopology ToD3DPrimitiveTopology(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.TriangleList:
                    return D3D.PrimitiveTopology.TriangleList;
                case PrimitiveType.TriangleStrip:
                    return D3D.PrimitiveTopology.TriangleStrip;
                case PrimitiveType.LineList:
                    return D3D.PrimitiveTopology.LineList;
                case PrimitiveType.LineStrip:
                    return D3D.PrimitiveTopology.LineList;
                case PrimitiveType.PointList:
                    return D3D.PrimitiveTopology.PointList;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static PrimitiveType FromD3DPrimitiveTopology(D3D.PrimitiveTopology type)
        {
            switch (type)
            {
                case D3D.PrimitiveTopology.TriangleList:
                    return PrimitiveType.TriangleList;
                case D3D.PrimitiveTopology.TriangleStrip:
                    return PrimitiveType.TriangleStrip;
                case D3D.PrimitiveTopology.LineList:
                    return PrimitiveType.LineList;
                case D3D.PrimitiveTopology.LineStrip:
                    return PrimitiveType.LineStrip;
                case D3D.PrimitiveTopology.PointList:
                    return PrimitiveType.PointList;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Fill Mode

        public static D3D11.FillMode ToD3DFillMode(FillMode mode)
        {
            switch (mode)
            {
                case FillMode.Solid:
                    return D3D11.FillMode.Solid;
                case FillMode.WireFrame:
                    return D3D11.FillMode.Wireframe;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static FillMode FromD3DFillMode(D3D11.FillMode mode)
        {
            switch (mode)
            {
                case D3D11.FillMode.Solid:
                    return FillMode.Solid;
                case D3D11.FillMode.Wireframe:
                    return FillMode.WireFrame;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Cull Mode

        public static D3D11.CullMode ToD3DCullMode(CullMode mode)
        {
            switch (mode)
            {
                case CullMode.Back:
                    return D3D11.CullMode.Back;
                case CullMode.Front:
                    return D3D11.CullMode.Front;
                case CullMode.None:
                    return D3D11.CullMode.None;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static CullMode FromD3DCullMode(D3D11.CullMode mode)
        {
            switch (mode)
            {
                case D3D11.CullMode.Back:
                    return CullMode.Back;
                case D3D11.CullMode.Front:
                    return CullMode.Front;
                case D3D11.CullMode.None:
                    return CullMode.None;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Filter

        public static D3D11.Filter ToD3DFilter(TextureFilter filter)
        {
            switch (filter)
            {
                case TextureFilter.Anisotropic:
                    return D3D11.Filter.Anisotropic;
                case TextureFilter.Linear:
                    return D3D11.Filter.MinMagMipLinear;
                case TextureFilter.LinearMipPoint:
                    return D3D11.Filter.MinMagLinearMipPoint;
                case TextureFilter.Point:
                    return D3D11.Filter.MinMagMipPoint;
                case TextureFilter.PointMipLinear:
                    return D3D11.Filter.MinMagPointMipLinear;
                case TextureFilter.MinLinearMagPointMipLinear:
                    return D3D11.Filter.MinLinearMagPointMipLinear;
                case TextureFilter.MinLinearMagPointMipPoint:
                    return D3D11.Filter.MinLinearMagMipPoint;
                case TextureFilter.MinPointMagLinearMipLinear:
                    return D3D11.Filter.MinPointMagMipLinear;
                case TextureFilter.MinPointMagLinearMipPoint:
                    return D3D11.Filter.MinPointMagLinearMipPoint;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static TextureFilter FromD3DFilter(D3D11.Filter filter)
        {
            switch (filter)
            {
                case D3D11.Filter.Anisotropic:
                    return TextureFilter.Anisotropic;
                case D3D11.Filter.MinMagMipLinear:
                    return TextureFilter.Linear;
                case D3D11.Filter.MinMagLinearMipPoint:
                    return TextureFilter.LinearMipPoint;
                case D3D11.Filter.MinMagMipPoint:
                    return TextureFilter.Point;
                case D3D11.Filter.MinMagPointMipLinear:
                    return TextureFilter.PointMipLinear;
                case D3D11.Filter.MinLinearMagPointMipLinear:
                    return TextureFilter.MinLinearMagPointMipLinear;
                case D3D11.Filter.MinLinearMagMipPoint:
                    return TextureFilter.MinLinearMagPointMipPoint;
                case D3D11.Filter.MinPointMagMipLinear:
                    return TextureFilter.MinPointMagLinearMipLinear;
                case D3D11.Filter.MinPointMagLinearMipPoint:
                    return TextureFilter.MinPointMagLinearMipPoint;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Depth Format

        public static D3D11.TextureAddressMode ToD3DTextureAddressMode(TextureAddressMode mode)
        {
            switch (mode)
            {
                case TextureAddressMode.Clamp:
                    return D3D11.TextureAddressMode.Clamp;
                case TextureAddressMode.Wrap:
                    return D3D11.TextureAddressMode.Wrap;
                case TextureAddressMode.Border:
                    return D3D11.TextureAddressMode.Border;
                case TextureAddressMode.Mirror:
                    return D3D11.TextureAddressMode.Mirror;
                case TextureAddressMode.MirrorOnce:
                    return D3D11.TextureAddressMode.MirrorOnce;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static TextureAddressMode FromD3DTextureAddressMode(D3D11.TextureAddressMode mode)
        {
            switch (mode)
            {
                case D3D11.TextureAddressMode.Clamp:
                    return TextureAddressMode.Clamp;
                case D3D11.TextureAddressMode.Wrap:
                    return TextureAddressMode.Wrap;
                case D3D11.TextureAddressMode.Border:
                    return TextureAddressMode.Border;
                case D3D11.TextureAddressMode.Mirror:
                    return TextureAddressMode.Mirror;
                case D3D11.TextureAddressMode.MirrorOnce:
                    return TextureAddressMode.MirrorOnce;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Surface Format

        public static DXGI.Format ToD3DSurfaceFormat(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Color:
                    return DXGI.Format.R8G8B8A8_UNorm;
                case SurfaceFormat.BGRColor:
                    return DXGI.Format.B8G8R8A8_UNorm;
                case SurfaceFormat.BGRA5551:
                    return DXGI.Format.B5G5R5A1_UNorm;
                case SurfaceFormat.BGR565:
                    return DXGI.Format.B5G6R5_UNorm;
                case SurfaceFormat.RGBA1010102:
                    return DXGI.Format.R10G10B10A2_UNorm;
                case SurfaceFormat.RG32:
                    return DXGI.Format.R16G16_UNorm;
                case SurfaceFormat.RGBA64:
                    return DXGI.Format.R16G16B16A16_UNorm;
                case SurfaceFormat.DXT1:
                    return DXGI.Format.BC1_UNorm;
                case SurfaceFormat.DXT3:
                    return DXGI.Format.BC2_UNorm;
                case SurfaceFormat.DXT5:
                    return DXGI.Format.BC3_UNorm;
                case SurfaceFormat.Alpha8:
                    return DXGI.Format.A8_UNorm;
                case SurfaceFormat.Single:
                    return DXGI.Format.R32_Float;
                case SurfaceFormat.Vector2:
                    return DXGI.Format.R32G32_Float;
                case SurfaceFormat.Vector3:
                    return DXGI.Format.R32G32B32_Float;
                case SurfaceFormat.Vector4:
                    return DXGI.Format.R32G32B32A32_Float;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static SurfaceFormat FromD3DSurfaceFormat(DXGI.Format format)
        {
            switch (format)
            {
                case DXGI.Format.R8G8B8A8_UNorm:
                    return SurfaceFormat.Color;
                case DXGI.Format.B8G8R8A8_UNorm:
                    return SurfaceFormat.BGRColor;
                case DXGI.Format.B5G5R5A1_UNorm:
                    return SurfaceFormat.BGRA5551;
                case DXGI.Format.B5G6R5_UNorm:
                    return SurfaceFormat.BGR565;
                case DXGI.Format.R10G10B10A2_UNorm:
                    return SurfaceFormat.RGBA1010102;
                case DXGI.Format.R16G16_UNorm:
                    return SurfaceFormat.RG32;
                case DXGI.Format.R16G16B16A16_UNorm:
                    return SurfaceFormat.RGBA64;
                case DXGI.Format.BC1_UNorm:
                    return SurfaceFormat.DXT1;
                case DXGI.Format.BC2_UNorm:
                    return SurfaceFormat.DXT3;
                case DXGI.Format.BC3_UNorm:
                    return SurfaceFormat.DXT5;
                case DXGI.Format.A8_UNorm:
                    return SurfaceFormat.Alpha8;
                case DXGI.Format.R32_Float:
                    return SurfaceFormat.Single;
                case DXGI.Format.R32G32_Float:
                    return SurfaceFormat.Vector2;
                case DXGI.Format.R32G32B32_Float:
                    return SurfaceFormat.Vector3;
                case DXGI.Format.R32G32B32A32_Float:
                    return SurfaceFormat.Vector4;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Depth Format

        public static DXGI.Format ToD3DDepthFormat(DepthFormat format)
        {
            switch (format)
            {
                case DepthFormat.Depth32Stencil8:
                    return DXGI.Format.D32_Float_S8X24_UInt;
                case DepthFormat.Depth32:
                    return DXGI.Format.D32_Float;
                case DepthFormat.Depth24Stencil8:
                    return DXGI.Format.D24_UNorm_S8_UInt;
                case DepthFormat.Depth16:
                    return DXGI.Format.D16_UNorm;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static DepthFormat FromD3DDepthFormat(DXGI.Format format)
        {
            switch (format)
            {
                case DXGI.Format.D32_Float_S8X24_UInt:
                    return DepthFormat.Depth32Stencil8;
                case DXGI.Format.D32_Float:
                    return DepthFormat.Depth32;
                case DXGI.Format.D24_UNorm_S8_UInt:
                    return DepthFormat.Depth24Stencil8;
                case DXGI.Format.D16_UNorm:
                    return DepthFormat.Depth16;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static DXGI.Format ToD3DShaderResourceFormatFromDepthFormat(DepthFormat format)
        {
            switch (format)
            {
                case DepthFormat.Depth32Stencil8:
                    return DXGI.Format.R32_Float_X8X24_Typeless;
                case DepthFormat.Depth32:
                    return DXGI.Format.R32_Float;
                case DepthFormat.Depth24Stencil8:
                    return DXGI.Format.R24_UNorm_X8_Typeless;
                case DepthFormat.Depth16:
                    return DXGI.Format.R16_Float;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static DXGI.Format ToD3DTextureFormatFromDepthFormat(DepthFormat format)
        {
            switch (format)
            {
                case DepthFormat.Depth32Stencil8:
                    return DXGI.Format.R32G8X24_Typeless;
                case DepthFormat.Depth32:
                    return DXGI.Format.R32_Typeless;
                case DepthFormat.Depth24Stencil8:
                    return DXGI.Format.R24G8_Typeless;
                case DepthFormat.Depth16:
                    return DXGI.Format.R16_Typeless;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D Map Mode

        public static D3D11.MapMode ToD3DMapMode(DataWriteOptions writeOptions)
        {
            switch (writeOptions)
            {
                case DataWriteOptions.None:
                    return D3D11.MapMode.Write;
                case DataWriteOptions.Discard:
                    return D3D11.MapMode.WriteDiscard;
                case DataWriteOptions.NoOverwrite:
                    return D3D11.MapMode.WriteNoOverwrite;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        public static DataWriteOptions FromD3DMapMode(D3D11.MapMode mapMode)
        {
            switch (mapMode)
            {
                case D3D11.MapMode.Write:
                    return DataWriteOptions.None;
                case D3D11.MapMode.WriteDiscard:
                    return DataWriteOptions.Discard;
                case D3D11.MapMode.WriteNoOverwrite:
                    return DataWriteOptions.NoOverwrite;
                default:
                    throw new SparkGraphicsException("Invalid enum value");
            }
        }

        #endregion

        #region To/From D3D ShaderResourceViewDimension

        public static D3D.ShaderResourceViewDimension ToD3DShaderResourceViewDimension(ShaderResourceType resourceType)
        {
            switch (resourceType)
            {
                case ShaderResourceType.Buffer:
                    return D3D.ShaderResourceViewDimension.Buffer;
                case ShaderResourceType.Texture1D:
                    return D3D.ShaderResourceViewDimension.Texture1D;
                case ShaderResourceType.Texture1DArray:
                    return D3D.ShaderResourceViewDimension.Texture1DArray;
                case ShaderResourceType.Texture2D:
                    return D3D.ShaderResourceViewDimension.Texture2D;
                case ShaderResourceType.Texture2DArray:
                    return D3D.ShaderResourceViewDimension.Texture2DArray;
                case ShaderResourceType.Texture2DMS:
                    return D3D.ShaderResourceViewDimension.Texture2DMultisampled;
                case ShaderResourceType.Texture2DMSArray:
                    return D3D.ShaderResourceViewDimension.Texture2DMultisampledArray;
                case ShaderResourceType.Texture3D:
                    return D3D.ShaderResourceViewDimension.Texture3D;
                case ShaderResourceType.TextureCube:
                    return D3D.ShaderResourceViewDimension.TextureCube;
                case ShaderResourceType.TextureCubeArray:
                    return D3D.ShaderResourceViewDimension.TextureCubeArray;
                default:
                    return D3D.ShaderResourceViewDimension.Unknown;
            }
        }

        public static ShaderResourceType FromD3DShaderResourceViewDimension(D3D.ShaderResourceViewDimension srvDim)
        {
            switch (srvDim)
            {
                case D3D.ShaderResourceViewDimension.Buffer:
                    return ShaderResourceType.Buffer;
                case D3D.ShaderResourceViewDimension.Texture1D:
                    return ShaderResourceType.Texture1D;
                case D3D.ShaderResourceViewDimension.Texture1DArray:
                    return ShaderResourceType.Texture1DArray;
                case D3D.ShaderResourceViewDimension.Texture2D:
                    return ShaderResourceType.Texture2D;
                case D3D.ShaderResourceViewDimension.Texture2DArray:
                    return ShaderResourceType.Texture2DArray;
                case D3D.ShaderResourceViewDimension.Texture2DMultisampled:
                    return ShaderResourceType.Texture2DMS;
                case D3D.ShaderResourceViewDimension.Texture2DMultisampledArray:
                    return ShaderResourceType.Texture2DMSArray;
                case D3D.ShaderResourceViewDimension.Texture3D:
                    return ShaderResourceType.Texture3D;
                case D3D.ShaderResourceViewDimension.TextureCube:
                    return ShaderResourceType.TextureCube;
                case D3D.ShaderResourceViewDimension.TextureCubeArray:
                    return ShaderResourceType.TextureCubeArray;
                default:
                    return ShaderResourceType.Unknown;
            }
        }

        #endregion

        #endregion
    }
}
