namespace Spark.OpenGL.Graphics
{
    using Spark.Graphics;

    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Helper methods for rendering
    /// </summary>
    public static class OpenGLHelper
    {
        /// <summary>
        /// Converts a value to its native type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Native version of the input value</returns>
        public static OGL.ShaderType ToNative(ShaderStage value)
        {
            switch (value)
            {
                case ShaderStage.VertexShader:
                    return OGL.ShaderType.VertexShader;
                case ShaderStage.GeometryShader:
                    return OGL.ShaderType.GeometryShader;
                case ShaderStage.PixelShader:
                    return OGL.ShaderType.FragmentShader;
                case ShaderStage.ComputeShader:
                    return OGL.ShaderType.ComputeShader;
                default:
                    throw new SparkGraphicsException($"Cannot cast {value.GetType()} value to native value");
            }
        }
        
        /// <summary>
        /// Converts a value to its native type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Native version of the input value</returns>
        public static OGL.ClearBufferMask ToNative(ClearOptions value)
        {
            OGL.ClearBufferMask result = OGL.ClearBufferMask.None;
            if (value.HasFlag(ClearOptions.Depth))
            {
                result |= OGL.ClearBufferMask.DepthBufferBit;
            }
            if (value.HasFlag(ClearOptions.Stencil))
            {
                result |= OGL.ClearBufferMask.StencilBufferBit;
            }
            if (value.HasFlag(ClearOptions.Target))
            {
                result |= OGL.ClearBufferMask.ColorBufferBit;
            }

            return result;
        }

        /// <summary>
        /// Converts a value to its native type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Native version of the input value</returns>
        public static OGL.PrimitiveType ToNative(PrimitiveType value)
        {
            switch (value)
            {
                case PrimitiveType.TriangleList:
                    return OGL.PrimitiveType.Triangles;
                case PrimitiveType.TriangleStrip:
                    return OGL.PrimitiveType.TriangleStrip;
                case PrimitiveType.LineList:
                    return OGL.PrimitiveType.Lines;
                case PrimitiveType.LineStrip:
                    return OGL.PrimitiveType.LineStrip;
                case PrimitiveType.PointList:
                    return OGL.PrimitiveType.Points;
                default:
                    throw new SparkGraphicsException($"Cannot cast {value.GetType()} value to native value");
            }
        }

        /// <summary>
        /// Converts a value to its native type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Native version of the input value</returns>
        public static OGL.BlendEquationMode ToNative(BlendFunction value)
        {
            switch (value)
            {
                case BlendFunction.Add:
                    return OGL.BlendEquationMode.FuncAdd;
                case BlendFunction.Subtract:
                    return OGL.BlendEquationMode.FuncSubtract;
                case BlendFunction.ReverseSubtract:
                    return OGL.BlendEquationMode.FuncReverseSubtract;
                case BlendFunction.Minimum:
                    return OGL.BlendEquationMode.Min;
                case BlendFunction.Maximum:
                    return OGL.BlendEquationMode.Max;
                default:
                    throw new SparkGraphicsException($"Cannot cast {value.GetType()} value to native value");
            }
        }

        /// <summary>
        /// Converts a value to its native type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Native version of the input value</returns>
        public static OGL.BlendingFactorSrc ToNativeSource(Blend value)
        {
            switch (value)
            {
                case Blend.Zero:
                    return OGL.BlendingFactorSrc.Zero;
                case Blend.One:
                    return OGL.BlendingFactorSrc.One;
                case Blend.SourceColor:
                    return OGL.BlendingFactorSrc.SrcColor;
                case Blend.InverseSourceColor:
                    return OGL.BlendingFactorSrc.OneMinusSrcColor;
                case Blend.SourceAlpha:
                    return OGL.BlendingFactorSrc.SrcAlpha;
                case Blend.InverseSourceAlpha:
                    return OGL.BlendingFactorSrc.OneMinusSrcAlpha;
                case Blend.DestinationColor:
                    return OGL.BlendingFactorSrc.DstColor;
                case Blend.InverseDestinationColor:
                    return OGL.BlendingFactorSrc.OneMinusDstColor;
                case Blend.DestinationAlpha:
                    return OGL.BlendingFactorSrc.DstAlpha;
                case Blend.InverseDestinationAlpha:
                    return OGL.BlendingFactorSrc.OneMinusSrc1Alpha;
                case Blend.SourceAlphaSaturation:
                    return OGL.BlendingFactorSrc.SrcAlphaSaturate;
                default:
                    throw new SparkGraphicsException($"Cannot cast {value.GetType()} value to native value");
            }
        }

        /// <summary>
        /// Converts a value to its native type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Native version of the input value</returns>
        public static OGL.BlendingFactorDest ToNativeDest(Blend value)
        {
            switch (value)
            {
                case Blend.Zero:
                    return OGL.BlendingFactorDest.Zero;
                case Blend.One:
                    return OGL.BlendingFactorDest.One;
                case Blend.SourceColor:
                    return OGL.BlendingFactorDest.SrcColor;
                case Blend.InverseSourceColor:
                    return OGL.BlendingFactorDest.OneMinusSrcColor;
                case Blend.SourceAlpha:
                    return OGL.BlendingFactorDest.SrcAlpha;
                case Blend.InverseSourceAlpha:
                    return OGL.BlendingFactorDest.OneMinusSrcAlpha;
                case Blend.DestinationColor:
                    return OGL.BlendingFactorDest.DstColor;
                case Blend.InverseDestinationColor:
                    return OGL.BlendingFactorDest.OneMinusDstColor;
                case Blend.DestinationAlpha:
                    return OGL.BlendingFactorDest.DstAlpha;
                case Blend.InverseDestinationAlpha:
                    return OGL.BlendingFactorDest.OneMinusSrc1Alpha;
                case Blend.SourceAlphaSaturation:
                    return OGL.BlendingFactorDest.SrcAlphaSaturate;
                default:
                    throw new SparkGraphicsException($"Cannot cast {value.GetType()} value to native value");
            }
        }
    }
}
