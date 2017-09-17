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
    }
}
