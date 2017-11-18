namespace Spark.Graphics
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Describes how a vertex buffer is to be bound to the graphics pipeline.
    /// </summary>
    public struct VertexBufferBinding : IEquatable<VertexBufferBinding>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBufferBinding"/> struct.
        /// </summary>
        /// <param name="buffer">The vertex buffer to bind.</param>
        public VertexBufferBinding(VertexBuffer buffer)
        {
            VertexBuffer = buffer;
            VertexOffset = 0;
            InstanceFrequency = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBufferBinding"/> struct.
        /// </summary>
        /// <param name="buffer">The vertex buffer to bind.</param>
        /// <param name="vertexOffset">The vertex offset from the start of the buffer to the first vertex that will be used.</param>
        public VertexBufferBinding(VertexBuffer buffer, int vertexOffset)
        {
            VertexBuffer = buffer;
            VertexOffset = vertexOffset;
            InstanceFrequency = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBufferBinding"/> struct.
        /// </summary>
        /// <param name="buffer">The vertex buffer to bind.</param>
        /// <param name="vertexOffset">The vertex offset from the start of the buffer to the first vertex that will be used.</param>
        /// <param name="instanceFrequency">Instance frequency (step rate), which specifies how many times to draw the instance before stepping
        /// one unit forward. If no instancing, this should be zero.</param>
        public VertexBufferBinding(VertexBuffer buffer, int vertexOffset, int instanceFrequency)
        {
            VertexBuffer = buffer;
            VertexOffset = vertexOffset;
            InstanceFrequency = Math.Max(0, instanceFrequency);
        }

        /// <summary>
        /// Gets the buffer to be bound.
        /// </summary>
        public VertexBuffer VertexBuffer { get; }

        /// <summary>
        /// Gets the vertex offset, indicating the first vertex to be used in the buffer (from the start of the buffer). This
        /// value is in whole indices, not bytes.
        /// </summary>
        public int VertexOffset { get; }

        /// <summary>
        /// Gets the instance frequency (step rate), which specifies how many times to draw the instance before stepping
        /// one unit forward. If no instancing, this should be zero.
        /// </summary>
        public int InstanceFrequency { get; }

        /// <summary>
        /// Implicitly converts a vertex buffer to the binding with offset of zero and instance frequency of zero.
        /// </summary>
        /// <param name="buffer">The vertex buffer to bind.</param>
        /// <returns>The vertex buffer binding.</returns>
        public static implicit operator VertexBufferBinding(VertexBuffer buffer)
        {
            return new VertexBufferBinding(buffer);
        }

        /// <summary>
        /// Implicitly converts a vertex buffer binding to a vertex buffer.
        /// </summary>
        /// <param name="binding">The vertex buffer binding.</param>
        /// <returns>The vertex buffer.</returns>
        public static implicit operator VertexBuffer(VertexBufferBinding binding)
        {
            return binding.VertexBuffer;
        }

        /// <summary>
        /// Tests equality between two vertex buffer bindings.
        /// </summary>
        /// <param name="a">First binding</param>
        /// <param name="b">Second binding</param>
        /// <returns>True if the two are equal, false otherwise.</returns>
        public static bool operator ==(VertexBufferBinding a, VertexBufferBinding b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Tests inequality between two vertex buffer bindings.
        /// </summary>
        /// <param name="a">First binding</param>
        /// <param name="b">Second binding</param>
        /// <returns>True if the two are not equal, false otherwise.</returns>
        public static bool operator !=(VertexBufferBinding a, VertexBufferBinding b)
        {
            return !a.Equals(b);
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

                hash = (hash * 31) + InstanceFrequency.GetHashCode();
                hash = (hash * 31) + VertexOffset.GetHashCode();
                hash = (hash * 31) + VertexBuffer.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is VertexBufferBinding)
            {
                return Equals((VertexBufferBinding)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between this vertex buffer buffer binding and another, that is if they both bind the same buffer and have the same binding configuration.
        /// </summary>
        /// <param name="other">Other vertex buffer binding to compare against.</param>
        /// <returns>True if the two bindings are equal, false otherwise.</returns>
        public bool Equals(VertexBufferBinding other)
        {
            return (other.VertexOffset == VertexOffset) && (other.InstanceFrequency == InstanceFrequency) && ReferenceEquals(other.VertexBuffer, VertexBuffer);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "VertexOffset: {0}, InstanceFrequency: {1}, Buffer: {2}", 
                new object[] { VertexOffset.ToString(), InstanceFrequency.ToString(), (VertexBuffer == null) ? "null" : VertexBuffer.ToString() });
        }
    }
}
