namespace Spark.Graphics
{
    using System.Collections;
    using System.Collections.Generic;

    using Spark.Utilities;

    /// <summary>
    /// Represents an enumerable collection of parameters from a collection of constant buffers and resource variables.
    /// </summary>
    public struct CompositeEffectParameterCollection : IEnumerable<IEffectParameter>
    {
        private readonly IReadOnlyList<IEffectConstantBuffer> _constantBuffers;
        private readonly IReadOnlyList<IEffectParameter> _resourceVariables;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeEffectParameterCollection"/> struct.
        /// </summary>
        /// <param name="constantBuffers">The constant buffers.</param>
        /// <param name="resourceVariables">The resource variables.</param>
        public CompositeEffectParameterCollection(IReadOnlyList<IEffectConstantBuffer> constantBuffers, IReadOnlyList<IEffectParameter> resourceVariables)
        {
            Guard.Against.NullArgument(constantBuffers, nameof(constantBuffers));
            Guard.Against.NullArgument(resourceVariables, nameof(resourceVariables));
            
            _constantBuffers = constantBuffers;
            _resourceVariables = resourceVariables;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public CompositeEffectParameterEnumerator GetEnumerator()
        {
            return new CompositeEffectParameterEnumerator(_constantBuffers, _resourceVariables);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        IEnumerator<IEffectParameter> IEnumerable<IEffectParameter>.GetEnumerator()
        {
            return new CompositeEffectParameterEnumerator(_constantBuffers, _resourceVariables);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CompositeEffectParameterEnumerator(_constantBuffers, _resourceVariables);
        }
    }
}
