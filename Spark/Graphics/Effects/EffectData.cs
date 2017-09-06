namespace Spark.Graphics.Effects
{
    using System;
    using System.IO;

    using Content;

    /// <summary>
    /// 
    /// </summary>
    public sealed class EffectData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectByteCode"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="effectByteCode"></param>
        /// <returns></returns>
        public static EffectData Read(Stream effectByteCode)
        {
            if (effectByteCode == null || !effectByteCode.CanRead)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectData"></param>
        /// <param name="compressionMode"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="effectData"></param>
        /// <param name="output"></param>
        /// <param name="compressionMode"></param>
        /// <returns></returns>
        public static bool Write(EffectData effectData, Stream output, EffectCompressionMode compressionMode = EffectCompressionMode.None)
        {
            if (effectData == null || output == null || !output.CanWrite)
            {
                return false;
            }
            
            throw new NotImplementedException();
        }
    }
}
