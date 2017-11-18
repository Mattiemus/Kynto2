namespace Spark.Graphics
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents a collection of effect constant buffers.
    /// </summary>
    public sealed class EffectConstantBufferCollection : ReadOnlyNamedListFast<IEffectConstantBuffer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectConstantBufferCollection"/> class.
        /// </summary>
        /// <param name="constantBuffers">Effect constant buffers.</param>
        public EffectConstantBufferCollection(params IEffectConstantBuffer[] constantBuffers) 
            : base(constantBuffers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectConstantBufferCollection"/> class.
        /// </summary>
        /// <param name="constantBuffers">Effect constant buffers.</param>
        public EffectConstantBufferCollection(IEnumerable<IEffectConstantBuffer> constantBuffers) 
            : base(constantBuffers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectConstantBufferCollection"/> class.
        /// </summary>
        private EffectConstantBufferCollection()
        {
        }

        /// <summary>
        /// Empty constant buffer collection.
        /// </summary>
        public static EffectConstantBufferCollection EmptyCollection => new EffectConstantBufferCollection();
    }
}
