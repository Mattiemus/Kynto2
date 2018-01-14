namespace Spark.Direct3D11.Graphics
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    using Spark.Graphics;

    /// <summary>
    /// Effect bytecode provider for D3D11 effects.
    /// </summary>
    public sealed class D3D11EffectByteCodeProvider : StandardEffectLibrary.BaseProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11EffectByteCodeProvider"/> class.
        /// </summary>
        public D3D11EffectByteCodeProvider()
            : base(string.Empty)
        {
        }

        /// <summary>
        /// Called to preload all the effect byte code buffers.
        /// </summary>
        /// <param name="effectByteCodes">Cache that will hold the effect byte code buffers.</param>
        protected override void Preload(Dictionary<string, byte[]> effectByteCodes)
        {
            // Key should always be a string (name of the file) and value should always be a byte[]
            foreach (DictionaryEntry kv in StandardEffects.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false))
            {
                effectByteCodes.Add((string)kv.Key, (byte[])kv.Value);
            }
        }
    }
}
