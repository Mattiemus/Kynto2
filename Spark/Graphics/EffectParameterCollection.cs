namespace Spark.Graphics
{
    using System.Collections.Generic;

    using Core.Collections;

    /// <summary>
    /// Represents a collection of effect parameters.
    /// </summary>
    public sealed class EffectParameterCollection : ReadOnlyNamedListFast<IEffectParameter>
    {
        /// <summary>
        /// Empty parameter collection.
        /// </summary>
        public static readonly EffectParameterCollection EmptyCollection = new EffectParameterCollection();

        /// <summary>
        /// Prevents a default instance of the <see cref="EffectParameterCollection"/> class from being created.
        /// </summary>
        private EffectParameterCollection()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="EffectParameterCollection"/> class.
        /// </summary>
        /// <param name="parameters">Effect parameters.</param>
        public EffectParameterCollection(params IEffectParameter[] parameters)
            : base(parameters)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="EffectParameterCollection"/> class.
        /// </summary>
        /// <param name="parameters">Effect parameters.</param>
        public EffectParameterCollection(IEnumerable<IEffectParameter> parameters) 
            : base(parameters)
        {
        }
    }
}
