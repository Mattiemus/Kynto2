namespace Spark.OpenGL.Graphics.Implementation
{
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Shader input attribute
    /// </summary>
    public sealed class OpenGLShaderAttribute
    {
        private readonly OGL.ActiveAttribType _type;
        private readonly int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLShaderAttribute"/> class.
        /// </summary>
        /// <param name="location">Attribute location</param>
        /// <param name="name">Attribute name</param>
        /// <param name="type">Attribute type</param>
        /// <param name="count">Number of values</param>
        public OpenGLShaderAttribute(int location, string name, OGL.ActiveAttribType type, int count)
        {
            _type = type;
            _count = count;

            Location = location;
            Name = name;
        }

        /// <summary>
        /// Gets the location of the attribute
        /// </summary>
        public int Location { get; }

        /// <summary>
        /// Gets the name of the attribute
        /// </summary>
        public string Name { get; }
    }
}
