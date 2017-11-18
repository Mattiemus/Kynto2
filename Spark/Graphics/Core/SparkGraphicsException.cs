namespace Spark.Graphics
{
    using System;
    using System.Runtime.Serialization;
    
    /// <summary>
    /// Engine exception for an error related to the graphics system
    /// </summary>
    [Serializable]
    public class SparkGraphicsException : SparkException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkGraphicsException"/> class.
        /// </summary>
        public SparkGraphicsException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkGraphicsException"/> class.
        /// </summary>
        /// <param name="msg">Error message that explains the reason for the exception</param>
        public SparkGraphicsException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkGraphicsException"/> class.
        /// </summary>
        /// <param name="paramName">Parameter name that caused the exception.</param>
        /// <param name="msg">Error message that explains the reason for the exception.</param>
        public SparkGraphicsException(string paramName, string msg)
            : base(paramName, msg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkGraphicsException"/> class.
        /// </summary>
        /// <param name="msg">Error message that explains the reason for the exception.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown, generally a more specific exception.</param>
        public SparkGraphicsException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkGraphicsException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        public SparkGraphicsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
