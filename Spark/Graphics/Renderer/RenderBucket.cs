namespace Spark.Graphics.Renderer
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using Graphics.Materials;

    /// <summary>
    /// A render bucket is a collection of renderables that are to be drawn, generally sorted in some order to achieve a rendering effect
    /// or to minimize overdraw and state switching.
    /// </summary>
    public sealed class RenderBucket : IReadOnlyList<RenderBucketEntry>
    {
        /// <summary>
        /// Threshold at which to stop using insertion sort, and start using merge sort
        /// </summary>
        private const int INSERTION_SORT_THRESHOLD = 7;

        private RenderBucketEntry[] _tempList;
        private RenderBucketEntry[] _renderables;
        private int _count;
        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderBucket"/> class.
        /// </summary>
        /// <param name="bucketId">Id that corresponds to this bucket.</param>
        /// <param name="comparer">Comparer used to sort the bucket.</param>
        /// <param name="initialSize">Initial capacity of the bucket, when exceeded the bucket will double this value.</param>
        public RenderBucket(RenderBucketId bucketId, IRenderBucketEntryComparer comparer, int initialSize)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (initialSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialSize));
            }

            BucketId = bucketId;
            BucketComparer = comparer;

            _count = 0;
            _renderables = new RenderBucketEntry[initialSize];
        }

        /// <summary>
        /// Gets the id corresponding to this render bucket.
        /// </summary>
        public RenderBucketId BucketId { get; }

        /// <summary>
        /// Gets the comparer used to sort the renderables.
        /// </summary>
        public IRenderBucketEntryComparer BucketComparer { get; }

        /// <summary>
        /// Gets the number of renderables in the collection.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Gets the renderable at the specified index in the bucket.
        /// </summary>
        /// <param name="index">The index of the renderable in the bucket.</param>
        public RenderBucketEntry this[int index]
        {
            get
            {
                if (index < 0 || index > _count)
                {
                    throw new ArgumentNullException(nameof(index));
                }

                return _renderables[index];
            }
        }

        /// <summary>
        /// Adds a renderable to the bucket.
        /// </summary>
        /// <param name="renderable">Renderable to add.</param>
        /// <param name="material">Optional material to be used with the renderable.</param>
        /// <returns>True if the renderable was added to the bucket, false if otherwise.</returns>
        public bool Add(IRenderable renderable, Material material)
        {
            if (renderable == null || !renderable.IsValidForDraw)
            {
                return false;
            }

            EnsureCapacity(_count + 1);
            
            _renderables[_count++] = new RenderBucketEntry(material, renderable);
            _version++;
            return true;
        }

        /// <summary>
        /// Removes a renderable from the bucket.
        /// </summary>
        /// <param name="renderable">Renderable to remove.</param>
        /// <returns>True if the renderable was removed from the bucket, false if otherwise.</returns>
        public bool Remove(IRenderable renderable)
        {
            if (renderable == null)
            {
                return false;
            }

            for (int i = 0; i < _count; i++)
            {
                if (_renderables[i].Renderable == renderable)
                {
                    // Remove and shift all the subsequent renderables down by one
                    _count--;
                    if (i < _count)
                    {
                        Array.Copy(_renderables, i + 1, _renderables, i, _count - i);
                    }

                    _renderables[_count] = new RenderBucketEntry();
                    _version++;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears the bucket of all renderables.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_renderables, 0, _count);

            _count = 0;
            _version++;
        }

        /// <summary>
        /// Draws all renderables in the bucket.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="applyMaterials">True to apply the materials of the renderables, false if just to setup draw calls.</param>
        public void DrawAll(IRenderContext renderContext, bool applyMaterials)
        {
            if (_count == 0)
            {
                return;
            }

            if (applyMaterials)
            {
                DrawAllWithMaterials(renderContext);
            }
            else
            {
                // Just execute the draw calls for each renderable
                for (int i = 0; i < _count; i++)
                {
                    _renderables[i].Renderable.SetupDrawCall(renderContext, BucketId, null);
                }
            }
        }

        /// <summary>
        /// Sorts the renderables in the bucket.
        /// </summary>
        public void Sort()
        {
            if (_count <= 1)
            {
                return;
            }

            // If null or not the same length, then need to resize/create a new array
            if (_tempList == null || _tempList.Length < _renderables.Length)
            {
                _tempList = _renderables.Clone() as RenderBucketEntry[];
            }
            else
            {
                // Otherwise copy up to the count
                Array.Copy(_renderables, _tempList, _count);
            }

            // Merge sort
            MergeInsertionSort(_tempList, _renderables, 0, _count, BucketComparer);

            // Clear temp list
            Array.Clear(_tempList, 0, _count);

            _version++;
        }

        /// <summary>
        /// Copies the content of this render bucket to another.
        /// </summary>
        /// <param name="bucket">Other bucket to copy content to.</param>
        public void CopyTo(RenderBucket bucket)
        {
            if (bucket == null)
            {
                return;
            }

            bucket.EnsureCapacity(bucket.Count + _count);

            Array.Copy(_renderables, 0, bucket._renderables, bucket._count, _count);
            bucket._count += _count;
            bucket._version++;
        }

        /// <summary>
        /// Ensures the render bucket can contain at least the given number of elements
        /// </summary>
        /// <param name="min">Minimum number of elements</param>
        private void EnsureCapacity(int min)
        {
            if (_renderables.Length < min)
            {
                int newCapacity = Math.Max((_renderables.Length == 0) ? 4 : _renderables.Length * 2, min);

                RenderBucketEntry[] newArray = new RenderBucketEntry[newCapacity];

                if (_count > 0)
                {
                    Array.Copy(_renderables, 0, newArray, 0, _count);
                }

                _renderables = newArray;
            }
        }

        /// <summary>
        /// Draws each renderable with their materials.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        private void DrawAllWithMaterials(IRenderContext renderContext)
        {
            for (int i = 0; i < _count; i++)
            {
                RenderBucketEntry currRenderBucketEntry = _renderables[i];
                IRenderable renderable = currRenderBucketEntry.Renderable;
                Material currMat = currRenderBucketEntry.Material;
                if (currMat == null)
                {
                    // If the material is null, and the renderable was valid for draw, most likely
                    // the renderable is doing some custom state setting in its setup draw call. We don't know enough information to do anything else.
                    renderable.SetupDrawCall(renderContext, BucketId, null);
                }
                else
                {
                    // The renderable has a material, now let's try to optimize by rendering in "chunks". For this material, let's see if there are other renderables
                    // ahead of us that use the same material, or share the same shaders. If so, then we can group the objects by pass -- set shaders once and
                    // some of the resources for each pass, then render all the objects before moving to the next pass, and so on.
                    int toIndex = GetEndIndex(i, currMat);
                    MaterialPassCollection passes = currMat.Passes;
                    int passCount = passes.Count;

                    // If same index, then no common materials found, so render this object normally (foreach pass, do draw).
                    if (toIndex == i)
                    {
                        currMat.ApplyMaterial(renderContext, renderable.RenderProperties);

                        for (int passIndex = 0; passIndex < passCount; passIndex++)
                        {
                            MaterialPass pass = passes[passIndex];
                            pass.Apply(renderContext);

                            renderable.SetupDrawCall(renderContext, BucketId, pass);
                        }
                    }
                    else
                    {
                        // If more than one found, render this chunk by applying one pass, render all objects, apply next pass, render all objects, etc.
                        for (int passIndex = 0; passIndex < passCount; passIndex++)
                        {
                            for (int j = i; j <= toIndex; j++)
                            {
                                currRenderBucketEntry = _renderables[j];
                                renderable = currRenderBucketEntry.Renderable;
                                currMat = currRenderBucketEntry.Material;

                                // For every object, we want its material to process data before we render the first pass exactly once,
                                // so all the constant buffers get filled and are ready to be bound
                                if (passIndex == 0)
                                {
                                    currMat.ApplyMaterial(renderContext, renderable.RenderProperties);
                                }

                                // Applying each pass will bind every resource as necessary, and because these materials share quite a lot
                                // in common, redundant resources should be filtered out. GPU resources like constant buffers will be unique
                                // between effect instances, but the pass shaders should all be the same resource, which will reduce state switching.
                                MaterialPass pass = currMat.Passes[passIndex];
                                pass.Apply(renderContext);

                                // Execute the draw calls
                                renderable.SetupDrawCall(renderContext, BucketId, pass);
                            }
                        }

                        // Set i to the index of the last renderable we drew, on next loop step it will be the index of the next different materialed
                        // renderable
                        i = toIndex;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return BucketId.Value;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return BucketId.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public RenderBucketEnumerator GetEnumerator()
        {
            return new RenderBucketEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        IEnumerator<RenderBucketEntry> IEnumerable<RenderBucketEntry>.GetEnumerator()
        {
            return new RenderBucketEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RenderBucketEnumerator(this);
        }

        /// <summary>
        /// Merge sorts an array using a comparer
        /// </summary>
        /// <typeparam name="T">Type of array elements</typeparam>
        /// <param name="aux">Auxhillary array</param>
        /// <param name="data">Array of elements to sort</param>
        /// <param name="low">Starting point to sort from</param>
        /// <param name="high">End point to sort to</param>
        /// <param name="comp">Comparer</param>
        private static void MergeInsertionSort<T>(T[] aux, T[] data, int low, int high, IComparer<T> comp)
        {
            // Use insertion sort on small arrays
            int length = high - low;
            if (length < INSERTION_SORT_THRESHOLD)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && comp.Compare(data[j - 1], data[j]) > 0; j--)
                    {
                        T temp = data[j];
                        data[j] = data[j - 1];
                        data[j - 1] = temp;
                    }
                }

                return;
            }

            // Merge sort: Recursively sort each half of dest into src
            int destLow = low;
            int destHight = high;

            int mid = (low + high) >> 1;
            MergeInsertionSort(data, aux, low, mid, comp);
            MergeInsertionSort(data, aux, mid, high, comp);

            // If list is already sorted, copy from src to dest
            if (comp.Compare(aux[mid - 1], aux[mid]) <= 0)
            {
                Array.Copy(aux, low, data, destLow, length);
            }

            // Merge sorted halves (now in src) into dest
            for (int i = destLow, p = low, q = mid; i < destHight; i++)
            {
                if (q >= high || p < mid && comp.Compare(aux[p], aux[q]) <= 0)
                {
                    data[i] = aux[p++];
                }
                else
                {
                    data[i] = aux[q++];
                }
            }
        }

        /// <summary>
        /// Returns the index of the final renderable that uses the given material
        /// </summary>
        /// <param name="startIndex">Index to start searching</param>
        /// <param name="currMat">Current material</param>
        /// <returns>Index of the final renderable that uses the given material</returns>
        private int GetEndIndex(int startIndex, Material currMat)
        {
            if (currMat == null)
            {
                return startIndex;
            }

            for (int i = startIndex + 1; i < _count; i++)
            {
                Material nextMat = _renderables[i].Material;

                // Compare curr material to the next, it may be null or invalid or a different set of shaders, or even just different number of passes. For any
                // of those cases, non-zero will be returned.
                if (currMat.CompareTo(nextMat) != 0)
                {
                    return i - 1;
                }
            }

            // Made it to the end without a switch, so use the last index
            return _count - 1;
        }
        
        #region RenderBucket Enumerator

        /// <summary>
        /// Enumerates elements of a <see cref="RenderBucket"/>.
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RenderBucketEnumerator : IEnumerator<RenderBucketEntry>
        {
            private int _index;
            private RenderBucketEntry _current;
            private readonly RenderBucket _bucket;
            private readonly RenderBucketEntry[] _list;
            private readonly int _count;
            private readonly int _version;

            /// <summary>
            /// Initializes a new instance of the <see cref="RenderBucketEnumerator"/> class.
            /// </summary>
            /// <param name="bucket">Render bucket to enumerate</param>
            internal RenderBucketEnumerator(RenderBucket bucket)
            {
                _bucket = bucket;
                _list = bucket._renderables;
                _count = bucket._count;
                _index = 0;
                _version = bucket._version;
                _current = new RenderBucketEntry();
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public RenderBucketEntry Current => _current;

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
                ThrowIfChanged();

                if (_index < _count)
                {
                    _current = _list[_index];
                    _index++;
                    return true;
                }
                else
                {
                    _index++;
                    _current = new RenderBucketEntry();
                    return false;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                ThrowIfChanged();
                _index = 0;
                _current = new RenderBucketEntry();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // No-op
            }

            /// <summary>
            /// Throws an exception if the collection is modified while it is being enumerated
            /// </summary>
            private void ThrowIfChanged()
            {
                if (_version != _bucket._version)
                {
                    throw new InvalidOperationException("Collection modified during enumeration");
                }
            }
        }

        #endregion
    }
}
