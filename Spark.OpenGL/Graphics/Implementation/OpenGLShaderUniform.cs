namespace Spark.Graphics.Implementation
{
    using System;
    using System.Linq;

    using OGL = OpenTK.Graphics.OpenGL;

    using Math;

    /// <summary>
    /// Uniform shader variable
    /// </summary>
    public sealed class OpenGLShaderUniform
    {
        private readonly OGL.ActiveUniformType _type;
        private readonly int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLShaderUniform"/> class.
        /// </summary>
        /// <param name="location">Uniform location</param>
        /// <param name="name">Uniform name</param>
        /// <param name="type">Uniform type</param>
        /// <param name="count">Number of values</param>
        public OpenGLShaderUniform(int location, string name, OGL.ActiveUniformType type, int count)
        {
            _type = type;
            _count = count;

            Location = location;
            Name = name;
        }

        /// <summary>
        /// Gets the location of the uniform
        /// </summary>
        public int Location { get; }

        /// <summary>
        /// Gets the name of the uniform
        /// </summary>
        public string Name { get; }

        public void SetValue(int value)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.Int, 1);
            OGL.GL.Uniform1(Location, value);
        }

        public void SetValue(int[] values)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.Int, values.Length);
            OGL.GL.Uniform1(Location, values.Length, values);
        }

        public void SetValue(uint value)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.UnsignedInt, 1);
            OGL.GL.Uniform1(Location, value);
        }

        public void SetValue(uint[] values)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.UnsignedInt, values.Length);
            OGL.GL.Uniform1(Location, values.Length, values);
        }

        public void SetValue(float value)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.Float, 1);
            OGL.GL.Uniform1(Location, value);
        }

        public void SetValue(float[] values)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.Float, values.Length);
            OGL.GL.Uniform1(Location, values.Length, values);
        }

        public void SetValue(Vector2 value)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.FloatVec2, 1);
            OGL.GL.Uniform2(Location, value.X, value.Y);
        }

        public void SetValue(Vector2[] values)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.FloatVec2, values.Length);
            OGL.GL.Uniform2(Location, values.Length, values.SelectMany(v => new[] { v.X, v.Y }).ToArray());
        }

        public void SetValue(Vector3 value)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.FloatVec3, 1);
            OGL.GL.Uniform3(Location, value.X, value.Y, value.Z);
        }

        public void SetValue(Vector3[] values)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.FloatVec3, values.Length);
            OGL.GL.Uniform3(Location, values.Length, values.SelectMany(v => new[] { v.X, v.Y, v.Z }).ToArray());
        }

        public void SetValue(Vector4 value)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.FloatVec4, 1);
            OGL.GL.Uniform4(Location, value.X, value.Y, value.Z, value.W);
        }

        public void SetValue(Vector4[] values)
        {
            ThrowIfInvalid(OGL.ActiveUniformType.FloatVec4, values.Length);
            OGL.GL.Uniform4(Location, values.Length, values.SelectMany(v => new[] { v.X, v.Y, v.Z, v.W }).ToArray());
        }

        /// <summary>
        /// Throws an exception if the expect type and input count doesnt match the uniform type information
        /// </summary>
        /// <param name="expectedType">Type being set to the uniform</param>
        /// <param name="inputCount">Number of values being set</param>
        private void ThrowIfInvalid(OGL.ActiveUniformType expectedType, int inputCount)
        {
            if (expectedType != _type)
            {
                throw new InvalidOperationException($"Cannot set uniform value of type {_type} with value of type {expectedType}");
            }

            if (inputCount != _count)
            {
                throw new InvalidOperationException($"Cannot set {inputCount} value(s) to a uniform which expects {_count} values");
            }
        }
    }
}
