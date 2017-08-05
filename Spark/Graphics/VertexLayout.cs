namespace Spark.Graphics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Content;

    /// <summary>
    /// Layout declaration that describes the structure of vertex data in a vertex buffer.
    /// </summary>
    public sealed class VertexLayout : IEquatable<VertexLayout>, IEnumerable<VertexElement>, ISavable
    {
        private VertexElement[] _elements;
        private int _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexLayout"/> class.
        /// </summary>
        private VertexLayout()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexLayout"/> class. The vertex stride automatically calculated from the elements.
        /// </summary>
        /// <param name="elements">Vertex elements that define the vertex layout.</param>
        public VertexLayout(params VertexElement[] elements)
            : this(CalculateOffsetsAndGetVertexStride(elements), elements)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="VertexLayout"/> class.
        /// </summary>
        /// <param name="vertexStride">Vertex stride of the vertex layout.</param>
        /// <param name="elements">Vertex elements that define the vertex layout.</param>
        public VertexLayout(int vertexStride, params VertexElement[] elements)
        {
            if (elements == null || elements.Length == 0)
            {
                throw new ArgumentNullException(nameof(elements), "Input array is null or empty");
            }

            _elements = (VertexElement[])elements.Clone();
            VertexStride = vertexStride;

            try
            {
                ValidateVertexElements(VertexStride, _elements);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Failed to validate vertex layout", e);
            }

            ComputeHashCode();
        }

        /// <summary>
        /// Gets the stride of a single vertex, which is the total size of a vertex in bytes (position, color, etc).
        /// </summary>
        public int VertexStride { get; private set; }

        /// <summary>
        /// Gets the number of vertex elements declared in the layout.
        /// </summary>
        public int ElementCount => _elements.Length;

        /// <summary>
        /// Gets the vertex element at the specified index in the layout.
        /// </summary>
        /// <param name="index">Zero-based index of the element</param>
        /// <returns>The vertex element</returns>
        /// <exception cref=" ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
        public VertexElement this[int index]
        {
            get
            {
                if (index < 0 || index >= _elements.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }

                return _elements[index];
            }
        }

        /// <summary>
        /// Computes the vertex stride from the specified vertex layout.
        /// </summary>
        /// <param name="elements">Vertex elements of the layout.</param>
        /// <returns>Vertex stride of the layout.</returns>
        public static int CalculateOffsetsAndGetVertexStride(params VertexElement[] elements)
        {
            int stride = 0;

            if (elements == null || elements.Length == 0)
            {
                return stride;
            }

            for (int i = 0; i < elements.Length; i++)
            {
                VertexElement elem = elements[i];
                stride = elem.Offset;
                stride += elem.Format.SizeInBytes();
            }

            return stride;
        }

        /// <summary>
        /// Validates the vertex elements and specified vertex stride.
        /// </summary>
        /// <param name="vertexStride">The vertex stride of the layout.</param>
        /// <param name="elements">The vertex elements of the layout.</param>
        public static void ValidateVertexElements(int vertexStride, params VertexElement[] elements)
        {
            if (elements == null || elements.Length == 0)
            {
                throw new ArgumentNullException(nameof(elements), "Input array is null or empty");
            }

            if (vertexStride <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexStride), "Vertex stride must be positive");
            }

            // Ensure stride is a multiple of four
            if ((vertexStride & 3) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexStride), "Vertex stride must be multiple of four");
            }

            int expectedStride = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                VertexElement elem = elements[i];
                int offset = elem.Offset;
                int elemSize = elem.Format.SizeInBytes();

                // Ensure offset is a multiple of four
                if ((offset & 3) != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(offset), "Vertex stride must be multiple of four");
                }

                // Check for overlap between this element and the previous
                if ((expectedStride + elemSize) != (offset + elemSize))
                {
                    throw new ArgumentException("Vertex elements overlap");
                }

                expectedStride = offset + elemSize;
            }
        }

        /// <summary>
        /// Gets a copy of the vertex elements in this declaration.
        /// </summary>
        /// <returns>Copy of the vertex elements.</returns>
        public VertexElement[] GetVertexElements()
        {
            return (VertexElement[])_elements.Clone();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<VertexElement> GetEnumerator()
        {
            return ((IEnumerable<VertexElement>)_elements).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is VertexLayout)
            {
                return (obj as VertexLayout)._hash == _hash;
            }

            return false;
        }

        /// <summary>
        /// Tests equality between this instance and another vertex layout.
        /// </summary>
        /// <param name="other">Other vertex declaration to compare to.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public bool Equals(VertexLayout other)
        {
            if (other != null)
            {
                return other._hash == _hash;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return _hash;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            VertexStride = input.ReadInt32();
            _elements = input.ReadArray<VertexElement>();

            // Precompute hash
            ComputeHashCode();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("VertexStride", VertexStride);
            output.Write("Elements", _elements);
        }

        /// <summary>
        /// Computes the hashcode for the vertex layout
        /// </summary>
        private void ComputeHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + VertexStride.GetHashCode();

                for (int i = 0; i < _elements.Length; i++)
                {
                    hash = (hash * 31) + _elements[i].GetHashCode();
                }

                _hash = hash;
            }
        }
    }
}
