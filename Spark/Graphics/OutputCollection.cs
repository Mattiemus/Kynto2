namespace Spark.Graphics
{
    using System.Collections.Generic;

    using Core.Collections;

    /// <summary>
    /// A read-only collection of outputs.
    /// </summary>
    public sealed class OutputCollection : ReadOnlyList<Output>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCollection"/> class.
        /// </summary>
        /// <param name="outputs">Outputs to initialize the collection with.</param>
        public OutputCollection(IEnumerable<Output> outputs) 
            : base(outputs)
        {
        }
    }
}
