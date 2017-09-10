namespace Spark.Graphics.Effects
{
    using System;
    using System.IO;

    using Core;
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
        /// Reads an effect data block from a byte stream
        /// </summary>
        /// <param name="effectByteCode">Effect data bytes</param>
        /// <returns>Parsed effect data</returns>
        public static EffectData Read(byte[] effectByteCode)
        {
            if (effectByteCode == null)
            {
                return null;
            }

            using (MemoryStream stream = new MemoryStream(effectByteCode))
            {
                return Read(stream);
            }
        }

        /// <summary>
        /// Reads an effect data block from a byte stream
        /// </summary>
        /// <param name="input">Effect data byte stream</param>
        /// <returns>Parsed effect data</returns>
        public static EffectData Read(Stream input)
        {
            if (input == null || !input.CanRead)
            {
                return null;
            }

            // TODO: deal with compression
            
            if (!Engine.IsInitialized)
            {
                throw new SparkGraphicsException("Engine is not initialized");
            }

            using (BinarySavableReader binaryReader = new BinarySavableReader(Engine.Instance.Services, input))
            {
                return new EffectData
                {
                    EffectName = binaryReader.ReadString(),
                    VertexShader = binaryReader.ReadString(),
                    PixelShader = binaryReader.ReadString()
                };
            }
        }

        /// <summary>
        /// Writes an effect data block to a stream of bytes
        /// </summary>
        /// <param name="effectData">Effect data</param>
        /// <param name="compressionMode">Compression mode</param>
        /// <returns>Array of bytes representing the effect data</returns>
        public static byte[] Write(EffectData effectData, EffectCompressionMode compressionMode = EffectCompressionMode.None)
        {
            if (effectData == null)
            {
                return null;
            }

            using (MemoryStream memStream = new MemoryStream())
            {
                Write(effectData, memStream);
                return memStream.ToArray();
            }
        }

        /// <summary>
        /// Writes an effect data block to a stream of bytes
        /// </summary>
        /// <param name="effectData">Effect data</param>
        /// <param name="output">Stream to write to</param>
        /// <param name="compressionMode">Compression mode</param>
        /// <returns>True if the write was successful, false otherwise</returns>
        public static bool Write(EffectData effectData, Stream output, EffectCompressionMode compressionMode = EffectCompressionMode.None)
        {
            if (effectData == null || output == null || !output.CanWrite)
            {
                return false;
            }

            // TODO: deal with compression
            
            using (BinarySavableWriter binaryWriter = new BinarySavableWriter(output, true))
            {
                binaryWriter.Write("EffectName", effectData.EffectName);
                binaryWriter.Write("VertexShader", effectData.VertexShader);
                binaryWriter.Write("PixelShader", effectData.PixelShader);
            }

            return true;
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
