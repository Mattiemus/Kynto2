namespace Spark.Graphics
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Describes how a stream output buffer is to be bound to the graphics pipeline.
    /// </summary>
    public struct StreamOutputBufferBinding : IEquatable<StreamOutputBufferBinding>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamOutputBufferBinding"/> struct.
        /// </summary>
        /// <param name="buffer">The stream output buffer to bind.</param>
        public StreamOutputBufferBinding(StreamOutputBuffer buffer)
        {
            StreamOutputBuffer = buffer;
            VertexOffset = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamOutputBufferBinding"/> struct.
        /// </summary>
        /// <param name="buffer">The stream output buffer to bind.</param>
        /// <param name="vertexOffset">The vertex offset from the start of the buffer to the first vertex that will be used.</param>
        public StreamOutputBufferBinding(StreamOutputBuffer buffer, int vertexOffset)
        {
            StreamOutputBuffer = buffer;
            VertexOffset = vertexOffset;
        }

        /// <summary>
        /// Gets the buffer to be bound.
        /// </summary>
        public StreamOutputBuffer StreamOutputBuffer { get; }

        /// <summary>
        /// Gets the vertex offset, indicating the first vertex to be used in the buffer (from the start of the buffer). This value is in whole indices, not bytes.
        /// </summary>
        public int VertexOffset { get; }

        /// <summary>
        /// Implicitly converts a stream output buffer to the binding with offset of zero.
        /// </summary>
        /// <param name="buffer">The stream output buffer to bind.</param>
        /// <returns>The stream output buffer binding.</returns>
        public static implicit operator StreamOutputBufferBinding(StreamOutputBuffer buffer)
        {
            return new StreamOutputBufferBinding(buffer);
        }
        
        /// <summary>
        /// Implicitly converts a stream output buffer binding to a stream output buffer.
        /// </summary>
        /// <param name="binding">The stream output buffer binding.</param>
        public static implicit operator StreamOutputBuffer(StreamOutputBufferBinding binding)
        {
            return binding.StreamOutputBuffer;
        }

        /// <summary>
        /// Tests equality between two stream output buffer bindings.
        /// </summary>
        /// <param name="a">First binding</param>
        /// <param name="b">Second binding</param>
        /// <returns>True if the two are equal, false otherwise.</returns>
        public static bool operator ==(StreamOutputBufferBinding a, StreamOutputBufferBinding b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two stream output buffer bindings.
        /// </summary>
        /// <param name="a">First binding</param>
        /// <param name="b">Second binding</param>
        /// <returns>True if the two are not equal, false otherwise.</returns>
        public static bool operator !=(StreamOutputBufferBinding a, StreamOutputBufferBinding b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = (hash * 31) + VertexOffset.GetHashCode();
                hash = (hash * 31) + StreamOutputBuffer.VertexLayout.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is StreamOutputBufferBinding)
            {
                return Equals((StreamOutputBufferBinding)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between this stream output buffer binding and another, that is if they both bind the same buffer and have the same binding configuration.
        /// </summary>
        /// <param name="other">Other stream output buffer binding to compare against.</param>
        /// <returns>True if the two bindings are equal, false otherwise.</returns>
        public bool Equals(StreamOutputBufferBinding other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between this stream output buffer binding and another, that is if they both bind the same buffer and have the same binding configuration.
        /// </summary>
        /// <param name="other">Other stream output buffer binding to compare against.</param>
        /// <returns>True if the two bindings are equal, false otherwise.</returns>
        public bool Equals(ref StreamOutputBufferBinding other)
        {
            return (other.VertexOffset == VertexOffset) && ReferenceEquals(other.StreamOutputBuffer, StreamOutputBuffer);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "VertexOffset: {0}, Buffer: {1}", new object[] { VertexOffset.ToString(), (StreamOutputBuffer == null) ? "null" : StreamOutputBuffer.ToString() });
        }
    }
}
