namespace Spark.Graphics.Renderer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using Graphics.Materials;

    /// <summary>
    /// Encapsulates a method to draw the contents of a single render bucket.
    /// </summary>
    /// <param name="renderContext">Render context.</param>
    /// <param name="bucket">Render bucket.</param>
    public delegate void DrawRenderBucket(IRenderContext renderContext, RenderBucket bucket);

    /// <summary>
    /// A render queue that contains a collection of buckets that renderables will be placed and sorted in when a scene is processed for rendering. 
    /// The order of the buckets will be based on the order they were added to the render queue.
    /// </summary>
    ///<remarks>
    /// Each bucket entry consists of a renderable (geometry) paired with a material. Generally renderables are sorted based on material or other criteria, allowing for rendering to be 
    /// batched together to reduce state switching. A renderable may contain multiple materials for different render passes (e.g. shadows, opaque, glow) and thus may have an entry 
    /// in multiple buckets. So you can think of each bucket representing the input for a render pass. The logic for handling one or more render passes is implemented in a 
    /// corresponding <see cref="IRenderStage"/>.
    ///</remarks>
    public sealed class RenderQueue : IReadOnlyList<RenderBucket>
    {
        private readonly List<RenderBucket> _listofBuckets;
        private readonly Dictionary<RenderBucketId, RenderBucket> _renderBuckets;
        private readonly Dictionary<MarkId, IRenderable> _markedRenderables;
        private int _version;

        /// <summary>
        /// Constructs a new instance of the <see cref="RenderQueue"/> class.
        /// </summary>
        public RenderQueue()
        {
            _listofBuckets = new List<RenderBucket>();
            _renderBuckets = new Dictionary<RenderBucketId, RenderBucket>();
            _markedRenderables = new Dictionary<MarkId, IRenderable>();
        }

        /// <summary>
        /// Gets the number of render buckets present in the queue.
        /// </summary>
        public int Count => _renderBuckets.Count;

        /// <summary>
        /// Gets the <see cref="RenderBucket"/> with the specified bucket identifier.
        /// </summary>
        /// <param name="bucketID">The bucket identifier.</param>
        public RenderBucket this[RenderBucketId bucketID]
        {
            get
            {
                if (bucketID.IsValid && _renderBuckets.TryGetValue(bucketID, out RenderBucket bucket))
                {
                    return bucket;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="RenderBucket"/> with the specified zero-based index.
        /// </summary>
        public RenderBucket this[int index]
        {
            get
            {
                if (index < 0 || index >= _listofBuckets.Count)
                {
                    return null;
                }

                return _listofBuckets[index];
            }
        }

        #region Bucket management

        /// <summary>
        /// Adds a render bucket to the collection.
        /// </summary>
        /// <param name="bucket">Bucket to add.</param>
        /// <returns>True if the bucket was added, false otherwise.</returns>
        public bool AddBucket(RenderBucket bucket)
        {
            if (bucket == null || !bucket.BucketId.IsValid)
            {
                return false;
            }

            if (_renderBuckets.ContainsKey(bucket.BucketId))
            {
                return false;
            }

            _renderBuckets.Add(bucket.BucketId, bucket);
            _listofBuckets.Add(bucket);

            _version++;

            return true;
        }

        /// <summary>
        /// Removes a render bucket from the collection.
        /// </summary>
        /// <param name="bucketId">Bucket to remove.</param>
        /// <returns>True if the bucket was removed, false otherwise.</returns>
        public bool RemoveBucket(RenderBucketId bucketId)
        {
            if (!bucketId.IsValid)
            {
                return false;
            }

            if (_renderBuckets.TryGetValue(bucketId, out RenderBucket bucket))
            {
                _renderBuckets.Remove(bucketId);
                _listofBuckets.Remove(bucket);

                _version++;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all render buckets from the collection.
        /// </summary>
        public void RemoveAllBuckets()
        {
            _renderBuckets.Clear();
            _listofBuckets.Clear();
            _version++;
        }

        /// <summary>
        /// Tries to get the render bucket corresponding to the specified ID.
        /// </summary>
        /// <param name="bucketId">Bucket ID.</param>
        /// <param name="bucket">The render bucket, or null if it was not contained in the collection.</param>
        /// <returns>True if the bucket was found, false if otherwise.</returns>
        public bool TryGetBucket(RenderBucketId bucketId, out RenderBucket bucket)
        {
            bucket = null;

            if (!bucketId.IsValid)
            {
                return false;
            }

            return _renderBuckets.TryGetValue(bucketId, out bucket);
        }

        #endregion

        #region Clear buckets

        /// <summary>
        /// Clears all render buckets.
        /// </summary>
        public void ClearBuckets(bool notifyMarked = true)
        {
            ClearBuckets(notifyMarked, null);
        }

        /// <summary>
        /// Clears the specified render buckets.
        /// </summary>
        /// <param name="bucketIds">IDs of buckets whose contents should be cleared.</param>
        public void ClearBuckets(bool notifyMarked = true, params RenderBucketId[] bucketIds)
        {
            if (bucketIds == null)
            {
                for (int i = 0; i < _listofBuckets.Count; i++)
                {
                    _listofBuckets[i].Clear();
                }
            }
            else
            {
                for (int i = 0; i < bucketIds.Length; i++)
                {
                    RenderBucketId id = bucketIds[i];
                    if (TryGetBucket(id, out RenderBucket bucket))
                    {
                        bucket.Clear();
                    }
                }
            }

            ClearMarks(notifyMarked);
        }

        #endregion

        #region Sort buckets

        /// <summary>
        /// Sorts all render buckets.
        /// </summary>
        /// <param name="cam">Camera to use for sorting.</param>
        public void SortBuckets(Camera cam)
        {
            SortBuckets(cam, null);
        }

        /// <summary>
        /// Sorts the specified render buckets.
        /// </summary>
        /// <param name="cam">Camera used for sorting.</param>
        /// <param name="bucketIds">IDs of buckets whose contents should be sorted.</param>
        public void SortBuckets(Camera cam, params RenderBucketId[] bucketIds)
        {
            if (bucketIds == null)
            {
                for (int i = 0; i < _listofBuckets.Count; i++)
                {
                    RenderBucket bucket = _listofBuckets[i];
                    IRenderBucketEntryComparer comparer = bucket.BucketComparer;
                    comparer.SetCamera(cam);
                    bucket.Sort();
                    comparer.SetCamera(null);
                }
            }
            else
            {
                for (int i = 0; i < bucketIds.Length; i++)
                {
                    RenderBucketId id = bucketIds[i];
                    if (TryGetBucket(id, out RenderBucket bucket))
                    {
                        IRenderBucketEntryComparer comparer = bucket.BucketComparer;
                        comparer.SetCamera(cam);
                        bucket.Sort();
                        comparer.SetCamera(null);
                    }
                }
            }
        }

        #endregion

        #region Render buckets

        /// <summary>
        /// Draws all renderables in all render buckets.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        public void RenderBuckets(IRenderContext renderContext)
        {
            RenderBuckets(renderContext, true, null);
        }

        /// <summary>
        /// Draws all renderables in all render buckets.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="applyMaterials">True if materials should be applied, otherwise no state is set by the buckets during drawing.</param>
        public void RenderBuckets(IRenderContext renderContext, bool applyMaterials)
        {
            RenderBuckets(renderContext, applyMaterials, null);
        }

        /// <summary>
        /// Draws all renderables in the specified render buckets.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="bucketIDs">IDs of buckets whose contents should be drawn.</param>
        public void RenderBuckets(IRenderContext renderContext, params RenderBucketId[] bucketIDs)
        {
            RenderBuckets(renderContext, true, bucketIDs);
        }

        /// <summary>
        /// Draws all renderables in the specified render buckets.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="applyMaterials">True if materials should be applied, otherwise no state is set by the buckets during drawing.</param>
        /// <param name="bucketIds">IDs of buckets whose contents should be drawn.</param>
        public void RenderBuckets(IRenderContext renderContext, bool applyMaterials, params RenderBucketId[] bucketIds)
        {
            if (bucketIds == null)
            {
                for (int i = 0; i < _listofBuckets.Count; i++)
                {
                    _listofBuckets[i].DrawAll(renderContext, applyMaterials);
                }
            }
            else
            {
                for (int i = 0; i < bucketIds.Length; i++)
                {
                    RenderBucketId id = bucketIds[i];
                    if (TryGetBucket(id, out RenderBucket bucket))
                    {
                        bucket.DrawAll(renderContext, applyMaterials);
                    }
                }
            }
        }

        /// <summary>
        /// Draws all renderables in all render buckets using the supplied draw function.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="drawMethod">Method for drawing renderables.</param>
        public void RenderBuckets(IRenderContext renderContext, DrawRenderBucket drawMethod)
        {
            RenderBuckets(renderContext, drawMethod, null);
        }

        /// <summary>
        /// Draws all renderables in the specified render buckets using the supplied draw function.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="drawMethod">Method for drawing renderables.</param>
        /// <param name="bucketIds">IDs of buckets whose contents should be drawn.</param>
        public void RenderBuckets(IRenderContext renderContext, DrawRenderBucket drawMethod, params RenderBucketId[] bucketIds)
        {
            if (bucketIds == null)
            {
                for (int i = 0; i < _listofBuckets.Count; i++)
                {
                    drawMethod(renderContext, _listofBuckets[i]);
                }
            }
            else
            {
                for (int i = 0; i < bucketIds.Length; i++)
                {
                    RenderBucketId id = bucketIds[i];
                    if (TryGetBucket(id, out RenderBucket bucket))
                    {
                        drawMethod(renderContext, bucket);
                    }
                }
            }
        }

        #endregion

        #region Renderable processing

        /// <summary>
        /// Adds the renderable to the queue.
        /// </summary>
        /// <param name="renderable">Renderable to queue for drawing.</param>
        /// <returns>True if the renderable was successfully added, false otherwise.</returns>
        public bool Enqueue(IRenderable renderable)
        {
            if (renderable == null)
            {
                return false;
            }

            bool addedOne = false;
            foreach (KeyValuePair<RenderBucketId, Material> kv in renderable.MaterialDefinition)
            {
                RenderBucket bucket;
                if (kv.Key.IsValid && _renderBuckets.TryGetValue(kv.Key, out bucket))
                    addedOne |= bucket.Add(renderable, kv.Value);
            }

            return addedOne;
        }

        /// <summary>
        /// Adds a list of renderables to the queue.
        /// </summary>
        /// <param name="renderables">Renderables to queue for drawing.</param>
        /// <returns>True if one or more renderable were successfully added, false if otherwise.</returns>
        public bool Enqueue(IReadOnlyList<IRenderable> renderables)
        {
            if (renderables == null)
            {
                return false;
            }

            bool addedOne = false;
            for (int i = 0; i < renderables.Count; i++)
            {
                IRenderable renderable = renderables[i];
                if (renderable == null)
                {
                    continue;
                }

                foreach (KeyValuePair<RenderBucketId, Material> kv in renderable.MaterialDefinition)
                {
                    if (kv.Key.IsValid && _renderBuckets.TryGetValue(kv.Key, out RenderBucket bucket))
                    {
                        addedOne |= bucket.Add(renderable, kv.Value);
                    }
                }
            }

            return addedOne;
        }

        /// <summary>
        /// Removes the renderable from the queue.
        /// </summary>
        /// <param name="renderable">Renderable to remove from the queue.</param>
        /// <returns>True if the renderable was successfully removed, false otherwise.</returns>
        public bool Dequeue(IRenderable renderable)
        {
            if (renderable == null)
            {
                return false;
            }

            bool removedOne = false;

            foreach (KeyValuePair<RenderBucketId, Material> kv in renderable.MaterialDefinition)
            {
                if (kv.Key.IsValid && _renderBuckets.TryGetValue(kv.Key, out RenderBucket bucket))
                {
                    removedOne |= bucket.Remove(renderable);
                }
            }

            return removedOne;
        }

        /// <summary>
        /// Removes the renderables from the queue.
        /// </summary>
        /// <param name="renderables">Renderables to remove from the queue.</param>
        /// <returns>True if the renderable was successfully removed, false otherwise.</returns>
        public bool Dequeue(IReadOnlyList<IRenderable> renderables)
        {
            if (renderables == null)
            {
                return false;
            }

            bool removedOne = false;
            for (int i = 0; i < renderables.Count; i++)
            {
                IRenderable renderable = renderables[i];
                if (renderable == null)
                {
                    continue;
                }

                foreach (KeyValuePair<RenderBucketId, Material> kv in renderable.MaterialDefinition)
                {
                    if (kv.Key.IsValid && _renderBuckets.TryGetValue(kv.Key, out RenderBucket bucket))
                    {
                        removedOne |= bucket.Remove(renderable);
                    }
                }
            }

            return removedOne;
        }

        #endregion

        #region Mark API

        /// <summary>
        /// Marks the renderable. This is useful during processing to track a renderable.
        /// </summary>
        /// <param name="id">Mark ID to identify the renderable.</param>
        /// <param name="renderable">Renderable to mark.</param>
        /// <returns>True if the renderable was added to the marked list, false if it is invalid or already marked.</returns>
        public bool Mark(MarkId id, IRenderable renderable)
        {
            if (!id.IsValid || renderable == null || _markedRenderables.ContainsKey(id))
            {
                return false;
            }

            _markedRenderables.Add(id, renderable);
            return true;
        }

        /// <summary>
        /// Unmarks the renderable identified by the ID.
        /// </summary>
        /// <param name="id">Mark ID to identify the renderable.</param>
        /// <returns>True if the renderable was removed from the marked list, false otherwise.</returns>
        public bool Unmark(MarkId id)
        {
            if (!id.IsValid)
            {
                return false;
            }
            
            if (_markedRenderables.TryGetValue(id, out IRenderable rnd))
            {
                IMarkedRenderable mkRnd = rnd as IMarkedRenderable;
                if (mkRnd != null)
                {
                    mkRnd.OnMarkCleared(id, this);
                }

                _markedRenderables.Remove(id);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Queries if the renderable identified by the ID has been marked by this queue.
        /// </summary>
        /// <param name="id">Mark ID to identify the renerable.</param>
        public bool IsMarked(MarkId id)
        {
            if (!id.IsValid)
            {
                return false;
            }

            return _markedRenderables.ContainsKey(id);
        }

        /// <summary>
        /// Gets the marked renderable as identified by the ID. If the renderable is not present then null is returned.
        /// </summary>
        /// <param name="id">Mark ID to identify the renderable.</param>
        /// <returns>The marked renderable, or null if it was not found.</returns>
        public IRenderable GetMarked(MarkId id)
        {
            if (!id.IsValid)
            {
                return null;
            }
            
            _markedRenderables.TryGetValue(id, out IRenderable rnd);

            return rnd;
        }

        /// <summary>
        /// Clears all marked renderables from the queue.
        /// </summary>
        public void ClearMarks(bool notifyMarked = true)
        {
            if (notifyMarked)
            {
                foreach (KeyValuePair<MarkId, IRenderable> kv in _markedRenderables)
                {
                    IMarkedRenderable mkRend = kv.Value as IMarkedRenderable;
                    if (mkRend != null)
                    {
                        mkRend.OnMarkCleared(kv.Key, this);
                    }
                }
            }

            _markedRenderables.Clear();
        }

        #endregion

        /// <summary>
        /// Copies the contents of this render queue into the other render queue.
        /// </summary>
        /// <param name="renderqueue">Render queue to copy contents to.</param>
        public void CopyTo(RenderQueue renderqueue)
        {
            if (renderqueue == null)
            {
                return;
            }

            for (int i = 0; i < _listofBuckets.Count; i++)
            {
                RenderBucket bucket = _listofBuckets[i];
                if (renderqueue.TryGetBucket(bucket.BucketId, out RenderBucket other))
                {
                    bucket.CopyTo(other);
                }
            }

            foreach (KeyValuePair<MarkId, IRenderable> kv in _markedRenderables)
            {
                if (!renderqueue._markedRenderables.ContainsKey(kv.Key))
                {
                    renderqueue._markedRenderables.Add(kv.Key, kv.Value);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public RenderQueueEnumerator GetEnumerator()
        {
            return new RenderQueueEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        IEnumerator<RenderBucket> IEnumerable<RenderBucket>.GetEnumerator()
        {
            return new RenderQueueEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RenderQueueEnumerator(this);
        }

        #region Enumerator

        /// <summary>
        /// Enumerates elements of a <see cref="RenderQueue"/>.
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RenderQueueEnumerator : IEnumerator<RenderBucket>
        {
            private int _index;
            private RenderBucket _current;
            private readonly RenderQueue _queue;
            private readonly List<RenderBucket> _buckets;
            private readonly int _count;
            private readonly int _version;

            /// <summary>
            /// Initializes a new instance of the <see cref="RenderQueueEnumerator"/> struct.
            /// </summary>
            /// <param name="queue">Render queue to enumerate</param>
            internal RenderQueueEnumerator(RenderQueue queue)
            {
                _queue = queue;
                _buckets = queue._listofBuckets;
                _count = _buckets.Count;
                _index = 0;
                _version = queue._version;
                _current = null;
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public RenderBucket Current => _current;

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
                    _current = _buckets[_index];
                    _index++;
                    return true;
                }
                else
                {
                    _index++;
                    _current = null;
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
                _current = null;
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
                if (_version != _queue._version)
                {
                    throw new InvalidOperationException("Collection modified during enumeration");
                }
            }
        }

        #endregion
    }
}
