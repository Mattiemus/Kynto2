namespace Spark
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Engine exception for general errors that occur during application execution.
    /// </summary>
    [Serializable]
    public class SparkException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkException"/> class.
        /// </summary>
        public SparkException() 
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkException"/> class.
        /// </summary>
        /// <param name="msg">Error message that explains the reason for the exception</param>
        public SparkException(string msg) 
            : base(msg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkException"/> class.
        /// </summary>
        /// <param name="paramName">Parameter name that caused the exception.</param>
        /// <param name="msg">Error message that explains the reason for the exception.</param>
        public SparkException(string paramName, string msg)
            : base($"Parameter: {paramName}\n\n{msg}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkException"/> class.
        /// </summary>
        /// <param name="msg">Error message that explains the reason for the exception.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown, generally a more specific exception.</param>
        public SparkException(string msg, Exception innerException) 
            : base(msg, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        public SparkException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
