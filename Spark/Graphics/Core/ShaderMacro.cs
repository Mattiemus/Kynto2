namespace Spark.Graphics
{
    using Content;

    /// <summary>
    /// Represents a shader macro that defines a value that gets used during shader compilation.
    /// </summary>
    public sealed class ShaderMacro : INamable, ISavable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderMacro"/> class.
        /// </summary>
        /// <param name="name">Name of the macro.</param>
        /// <param name="definition">The macro value.</param>
        public ShaderMacro(string name, object definition)
        {
            Name = name;
            Definition = (definition == null) ? string.Empty : definition.ToString();
        }

        /// <summary>
        /// Gets or sets the name of the macro.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the macro definition.
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            Name = input.ReadString();
            Definition = input.ReadString();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("Name", Name);
            output.Write("Definition", Definition);
        }
    }
}
