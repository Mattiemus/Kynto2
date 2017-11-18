namespace Spark.Graphics
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents a collection of effect parameters.
    /// </summary>
    public sealed class EffectParameterCollection : ReadOnlyNamedListFast<IEffectParameter>
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="EffectParameterCollection"/> class from being created.
        /// </summary>
        private EffectParameterCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectParameterCollection"/> class.
        /// </summary>
        /// <param name="parameters">Effect parameters.</param>
        public EffectParameterCollection(params IEffectParameter[] parameters)
            : base(parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectParameterCollection"/> class.
        /// </summary>
        /// <param name="parameters">Effect parameters.</param>
        public EffectParameterCollection(IEnumerable<IEffectParameter> parameters) 
            : base(parameters)
        {
        }

        /// <summary>
        /// Empty parameter collection.
        /// </summary>
        public static EffectParameterCollection EmptyCollection => new EffectParameterCollection();
    }
}
