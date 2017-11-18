namespace Spark.Utilities
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Simple cyclic reference detector to be used when processing parent-child or nested relationships in order to
    /// detect cycles in the relationship. The object is added as a reference, processed, then removed as a reference. 
    /// If the object is added as a reference again, an exception is thrown, or it can be queried if it has already been announced.
    /// </summary>
    public sealed class CyclicReferenceDetector
    {
        private readonly Dictionary<object, bool> _detector;

        /// <summary>
        /// Initializes a new instance of the <see cref="CyclicReferenceDetector"/> class.
        /// </summary>
        public CyclicReferenceDetector()
        {
            _detector = new Dictionary<object, bool>(new ReferenceEqualityComparer<object>());
        }

        /// <summary>
        /// Adds a reference to the detector.
        /// </summary>
        /// <param name="value">The object reference.</param>
        public void AddReference(object value)
        {
            if (_detector.ContainsKey(value))
            {
                throw new SparkException("Cyclical reference has been detected");
            }
            
            _detector.Add(value, true);
        }

        /// <summary>
        /// Removes a reference from the detector.
        /// </summary>
        /// <param name="value">The object reference.</param>
        public void RemoveReference(object value)
        {
            _detector.Remove(value);
        }

        /// <summary>
        /// Checks if the reference has already been added to the detector.
        /// </summary>
        /// <param name="value">The object reference.</param>
        /// <returns>True if the reference has been seen by the detector, false otherwise.</returns>
        public bool IsCyclicReference(object value)
        {
            return _detector.ContainsKey(value);
        }
    }
}
