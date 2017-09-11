namespace Spark.Graphics.Effects
{
    using Content;

    /// <summary>
    /// Effect data object
    /// </summary>
    public sealed class EffectData : ISavable
    {
        /// <summary>
        /// Gets or sets the name of the effect
        /// </summary>
        public string EffectName { get; set; }

        /// <summary>
        /// Gets or sets the vertex shader portion of the effect
        /// </summary>
        public string VertexShader { get; set; }

        /// <summary>
        /// Gets or sets the pixel shader portion of the effect
        /// </summary>
        public string PixelShader { get; set; }
        
        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            EffectName = input.ReadString();
            VertexShader = input.ReadString();
            PixelShader = input.ReadString();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("EffectName", EffectName);
            output.Write("VertexShader", VertexShader);
            output.Write("PixelShader", PixelShader);
        }
    }
}
