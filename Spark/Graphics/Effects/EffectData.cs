namespace Spark.Graphics
{
    using System.IO;
    
    using Content;
    using Content.Binary;

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
        /// Reads an effect data instance from a byte array
        /// </summary>
        /// <param name="bytes">Bytes to read from</param>
        /// <returns>Parsed effect data</returns>
        public static EffectData FromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinarySavableReader(SparkEngine.Instance.Services, stream))
                {
                    var data = new EffectData();
                    data.Read(reader);
                    return data;
                }
            }
        }

        /// <summary>
        /// Writes an effect data instance to a byte array
        /// </summary>
        /// <param name="data">Data to write</param>
        /// <returns>Data represented as a byte array</returns>
        public static byte[] ToBytes(EffectData data)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinarySavableWriter(stream, true))
                {
                    data.Write(writer);
                }

                return stream.ToArray();
            }            
        }

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
