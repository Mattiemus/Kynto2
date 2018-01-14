namespace Spark.Direct3D11.Graphics
{
    using System;

    public enum D3D11FeatureLevel
    {
        Level_10_0 = 40960,

        Level_10_1 = 41216,

        Level_11_0 = 45056
    }

    public enum D3DShaderVariableClass
    {
        Scalar = 0,
        Vector = 1,
        MatrixRows = 2,
        MatrixColumns = 3,
        Object = 4,
        Struct = 5,
        InterfaceClass = 6,
        InterfacePointer = 7
    }

    public enum D3DShaderVariableType
    {
        AppendStructuredBuffer = 50,
        Blend = 24,
        Bool = 1,
        Buffer = 25,
        ByteaddressBuffer = 46,
        Computeshader = 38,
        Constantbuffer = 26,
        ConsumeStructuredBuffer = 51,
        Depthstencil = 23,
        Depthstencilview = 31,
        Domainshader = 36,
        Double = 39,
        Float = 3,
        Geometryshader = 21,
        Hullshader = 35,
        Int = 2,
        InterfacePointer = 37,
        Pixelfragment = 17,
        Pixelshade = 15,
        Rasterizer = 22,
        Rendertargetview = 30,
        RWBuffer = 45,
        RWByteAddressBuffer = 47,
        RwstructuredBuffer = 49,
        RWTexture1D = 40,
        RWTexture1DArray = 41,
        RWTexture2D = 42,
        RWTexture2DArray = 43,
        RWTexture3D = 44,
        Sampler = 10,
        Sampler1d = 11,
        Sampler2d = 12,
        Sampler3d = 13,
        Samplercube = 14,
        String = 4,
        StructuredBuffer = 48,
        Texture = 5,
        Texture1D = 6,
        Texture1DArray = 28,
        Texture2D = 7,
        Texture2DArray = 29,
        Texture2DMultisampled = 32,
        Texture2DMultisampledArray = 33,
        Texture3D = 8,
        TextureBuffer = 27,
        TextureCube = 9,
        TextureCubeArray = 34,
        UInt = 19,
        UInt8 = 20,
        Vertexfragment = 18,
        Vertexshader = 16,
        Void = 0
    }

    public enum D3DConstantBufferType
    {
        ConstantBuffer = 0,
        TextureBuffer = 1,
        InterfacePointers = 2,
        ResourceBindInformation = 3
    }

    [Flags]
    public enum D3DComponentMaskFlags
    {
        None = 0,
        ComponentX = 1,
        ComponentY = 2,
        ComponentZ = 4,
        ComponentW = 8,
        All = 15
    }

    public enum D3DComponentType
    {
        Unknown = 0,
        UInt32 = 1,
        SInt32 = 2,
        Float32 = 3
    }

    public enum D3DSystemValueType
    {
        ClipDistance = 2,
        Coverage = 66,
        CullDistance = 3,
        Depth = 65,
        DepthGreaterEqual = 67,
        DepthLessEqual = 68,
        FinalLineDensityTessFactor = 16,
        FinalLineDetailTessFactor = 15,
        FinalQuadEdgeTessFactor = 11,
        FinalQuadInsideTessFactor = 12,
        FinalTriangleEdgeTessFactor = 13,
        FinalTriangleInsideTessFactor = 14,
        InstanceId = 8,
        IsFrontFace = 9,
        Position = 1,
        PrimitiveId = 7,
        RenderTargetArrayIndex = 4,
        SampleIndex = 10,
        Target = 64,
        Undefined = 0,
        VertexId = 6,
        ViewportArrayIndex = 5,
    }

    public enum D3DResourceReturnType
    {
        UNorm = 1,
        SNorm = 2,
        SInt = 3,
        UInt = 4,
        Float = 5,
        Mixed = 6,
        Double = 7,
        Continued = 8
    }

    public enum D3DResourceDimension
    {
        Buffer = 1,
        Texture1D = 2,
        Texture1DArray = 3,
        Texture2D = 4,
        Texture2DArray = 5,
        Texture2DMultisampled = 6,
        Texture2DMultiSampledArray = 7,
        Texture3D = 8,
        TextureCube = 9,
        TextureCubeArray = 10,
        ExtendedBuffer = 11
    }

    [Flags]
    public enum D3DShaderInputFlags
    {
        None = 0,
        UserPacked = 1,
        ComparisonSampler = 2,
        TextureComponent0 = 4,
        TextureComponent1 = 8,
        Texturecomponents = 12
    }

    public enum D3DShaderInputType
    {
        ConstantBuffer = 0,
        TextureBuffer = 1,
        Texture = 2,
        Sampler = 3,
        RWTyped = 4,
        Structured = 5,
        RWStructured = 6,
        ByteAddress = 7,
        RWByteAddress = 8,
        AppendStructured = 9,
        ConsumeStructured = 10,
        RWStructuredWithCounter = 11
    }

    [Flags]
    public enum D3DShaderVariableFlags
    {
        None = 0,
        UserPacked = 1,
        Used = 2,
        InterfacePointer = 4,
        InterfaceParameter = 8
    }

    public enum D3DTessellatorDomain
    {
        Undefined = 0,
        Isoline = 1,
        Triangle = 2,
        Quad = 3
    }

    public enum D3DPrimitiveTopology
    {
        Undefined = 0,
        PointList = 1,
        LineList = 2,
        LineStrip = 3,
        TriangleList = 4,
        TriangleStrip = 5,
        LineListWithAdjacency = 10,
        LineStripWithAdjacency = 11,
        TriangleListWithAdjacency = 12,
        TriangleStripWithAdjacency = 13,
        PatchListWith1ControlPoint = 33,
        PatchListWith2ControlPoints = 34,
        PatchListWith3ControlPoints = 35,
        PatchListWith4ControlPoints = 36,
        PatchListWith5ControlPoints = 37,
        PatchListWith6ControlPoints = 38,
        PatchListWith7ControlPoints = 39,
        PatchListWith8ControlPoints = 40,
        PatchListWith9ControlPoints = 41,
        PatchListWith10ControlPoints = 42,
        PatchListWith11ControlPoints = 43,
        PatchListWith12ControlPoints = 44,
        PatchListWith13ControlPoints = 45,
        PatchListWith14ControlPoints = 46,
        PatchListWith15ControlPoints = 47,
        PatchListWith16ControlPoints = 48,
        PatchListWith17ControlPoints = 49,
        PatchListWith18ControlPoints = 50,
        PatchListWith19ControlPoints = 51,
        PatchListWith20ControlPoints = 52,
        PatchListWith21ControlPoints = 53,
        PatchListWith22ControlPoints = 54,
        PatchListWith23ControlPoints = 55,
        PatchListWith24ControlPoints = 56,
        PatchListWith25ControlPoints = 57,
        PatchListWith26ControlPoints = 58,
        PatchListWith27ControlPoints = 59,
        PatchListWith28ControlPoints = 60,
        PatchListWith29ControlPoints = 61,
        PatchListWith30ControlPoints = 62,
        PatchListWith31ControlPoints = 63,
        PatchListWith32ControlPoints = 64
    }

    public enum D3DInputPrimitive
    {
        Undefined = 0,
        Point = 1,
        Line = 2,
        Triangle = 3,
        LineWithAdjacency = 6,
        TriangleWithAdjacency = 7,
        PatchWith1ControlPoint = 8,
        PatchWith2ControlPoints = 9,
        PatchWith3ControlPoints = 10,
        PatchWith4ControlPoints = 11,
        PatchWith5ControlPoints = 12,
        PatchWith6ControlPoints = 13,
        PatchWith7ControlPoints = 14,
        PatchWith8ControlPoints = 15,
        PatchWith9ControlPoints = 16,
        PatchWith10ControlPoints = 17,
        PatchWith11ControlPoints = 18,
        PatchWith12ControlPoints = 19,
        PatchWith13ControlPoints = 20,
        PatchWith14ControlPoints = 21,
        PatchWith15ControlPoints = 22,
        PatchWith16ControlPoints = 23,
        PatchWith17ControlPoints = 24,
        PatchWith18ControlPoints = 25,
        PatchWith19ControlPoints = 26,
        PatchWith20ControlPoints = 28,
        PatchWith21ControlPoints = 29,
        PatchWith22ControlPoints = 30,
        PatchWith23ControlPoints = 31,
        PatchWith24ControlPoints = 32,
        PatchWith25ControlPoints = 33,
        PatchWith26ControlPoints = 34,
        PatchWith27ControlPoints = 35,
        PatchWith28ControlPoints = 36,
        PatchWith29ControlPoints = 37,
        PatchWith30ControlPoints = 38,
        PatchWith31ControlPoints = 39,
        PatchWith32ControlPoints = 40,
    }

    public enum D3DTessellatorOutputPrimitive
    {
        Undefined = 0,
        Point = 1,
        Line = 2,
        TriangleClockwise = 3,
        TriangleCounterClockwise = 4
    }

    public enum D3DTessellatorPartitioning
    {
        Undefined = 0,
        Integer = 1,
        PowerOfTwo = 2,
        FractionalOdd = 3,
        FractionalEven = 4
    }

    public enum D3DShaderVersion
    {
        PixelShader = 0,
        VertexShader = 1,
        GeometryShader = 2,
        HullShader = 3,
        DomainShader = 4,
        ComputeShader = 5
    }
}
