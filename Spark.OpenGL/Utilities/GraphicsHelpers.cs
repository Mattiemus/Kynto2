namespace Spark.OpenGL.Utilities
{
    using Spark.Graphics;

    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Helper methods for rendering
    /// </summary>
    public static class GraphicsHelpers
    {
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
                case PrimitiveType.Points:
                    return OGL.PrimitiveType.Points;
                case PrimitiveType.Lines:
                    return OGL.PrimitiveType.Lines;
                case PrimitiveType.LineLoop:
                    return OGL.PrimitiveType.LineLoop;
                case PrimitiveType.LineStrip:
                    return OGL.PrimitiveType.LineStrip;
                case PrimitiveType.Triangles:
                    return OGL.PrimitiveType.Triangles;
                case PrimitiveType.TriangleStrip:
                    return OGL.PrimitiveType.TriangleStrip;
                case PrimitiveType.TriangleFan:
                    return OGL.PrimitiveType.TriangleFan;
                case PrimitiveType.Quads:
                    return OGL.PrimitiveType.Quads;
                default:
                    throw new SparkGraphicsException($"Cannot cast {value.GetType()} value to native value");
            }
        }
    }
}
