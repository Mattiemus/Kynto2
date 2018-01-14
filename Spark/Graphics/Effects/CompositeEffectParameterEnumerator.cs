namespace Spark.Graphics
{
    using System.Collections;
    using System.Collections.Generic;

    using Spark.Utilities;

    /// <summary>
    /// Composite enumerator to enumerate constant buffer effect parameter collections and a non-constant buffer parameter collection.
    /// </summary>
    public struct CompositeEffectParameterEnumerator : IEnumerator<IEffectParameter>
    {
        private readonly IReadOnlyList<IEffectConstantBuffer> _constantBuffers;
        private readonly IReadOnlyList<IEffectParameter> _resourceVariables;
        private int _currentCollectionIndex;
        private int _currentIndexInCollection;
        private int _countOfCurrentCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeEffectParameterEnumerator"/> struct.
        /// </summary>
        /// <param name="constantBuffers">The constant buffers collection.</param>
        /// <param name="resourceVariables">The resource variable collection.</param>
        public CompositeEffectParameterEnumerator(IReadOnlyList<IEffectConstantBuffer> constantBuffers, IReadOnlyList<IEffectParameter> resourceVariables)
        {
            Guard.Against.NullArgument(constantBuffers, nameof(constantBuffers));
            Guard.Against.NullArgument(resourceVariables, nameof(resourceVariables));

            _constantBuffers = constantBuffers;
            _resourceVariables = resourceVariables;
            _currentCollectionIndex = -1;
            _currentIndexInCollection = -1;
            _countOfCurrentCollection = 0;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public IEffectParameter Current
        {
            get
            {
                if (_currentCollectionIndex < 0)
                {
                    return null;
                }

                if (_currentCollectionIndex < _constantBuffers.Count)
                {
                    return _constantBuffers[_currentCollectionIndex].Parameters[_currentIndexInCollection];
                }
                else if (_currentCollectionIndex == _constantBuffers.Count)
                {
                    return _resourceVariables[_currentIndexInCollection];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        object IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            int currIndex = _currentIndexInCollection + 1;

            if (currIndex < _countOfCurrentCollection)
            {
                _currentIndexInCollection = currIndex;
                return true;
            }
            else if (_currentCollectionIndex < _constantBuffers.Count)
            {
                int currCollIndex = _currentCollectionIndex + 1;
                int count = GetCountOfCollection(currCollIndex);

                if (count > 0)
                {
                    _currentCollectionIndex = currCollIndex;
                    _countOfCurrentCollection = count;
                    _currentIndexInCollection = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _currentCollectionIndex = -1;
            _currentIndexInCollection = -1;
            _countOfCurrentCollection = 0;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // No-op
        }

        private int GetCountOfCollection(int collectionIndex)
        {
            if (collectionIndex < 0)
            {
                return 0;
            }

            if (collectionIndex < _constantBuffers.Count)
            {
                return _constantBuffers[collectionIndex].Parameters.Count;
            }
            else if (collectionIndex == _constantBuffers.Count)
            {
                return _resourceVariables.Count;
            }

            return 0;
        }
    }
}
