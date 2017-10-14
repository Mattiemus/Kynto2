namespace Spark.Graphics.Renderer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Collection of render stages.
    /// </summary>
    public class RenderStageCollection : IReadOnlyList<IRenderStage>
    {
        private int _version;
        private readonly List<IRenderStage> _stages;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderStageCollection"/> class.
        /// </summary>
        public RenderStageCollection()
        {
            _stages = new List<IRenderStage>();
        }

        /// <summary>
        /// Gets the number of render stages in the collection.
        /// </summary>
        public int Count => _stages.Count;

        /// <summary>
        /// Gets the <see cref="IRenderStage"/> with the specified zero-based index.
        /// </summary>
        public IRenderStage this[int index]
        {
            get
            {
                if (index < 0 || index >= _stages.Count)
                {
                    return null;
                }

                return _stages[index];
            }
        }

        /// <summary>
        /// Adds a render stage to the collection.
        /// </summary>
        /// <param name="stage">Render stage to add.</param>
        /// <returns>True if the stage wass added, false otherwise.</returns>
        public bool AddStage(IRenderStage stage)
        {
            if (stage == null)
            {
                return false;
            }

            _stages.Add(stage);
            _version++;

            return true;
        }

        /// <summary>
        /// Inserts the render stage at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which to inser the stage into the collection.</param>
        /// <param name="stage">Render stage to add.</param>
        /// <returns>True if the stage was inserted, false otherwise.</returns>
        public bool InsertStage(int index, IRenderStage stage)
        {
            if (index < 0 || index >= _stages.Count || stage == null)
            {
                return false;
            }

            _stages.Insert(index, stage);
            _version++;

            return true;
        }

        /// <summary>
        /// Removes a render stage from the collection.
        /// </summary>
        /// <param name="stage">Render stage to remove.</param>
        /// <returns>True if the stage was removed, false otherwise.</returns>
        public bool RemoveStage(IRenderStage stage)
        {
            if (stage == null)
            {
                return false;
            }

            bool removed = _stages.Remove(stage);
            if (removed)
            {
                _version++;
            }

            return removed;
        }

        /// <summary>
        /// Removes a render stage at the specified index in the collection.
        /// </summary>
        /// <param name="index">Zero-based index of the render stage.</param>
        /// <returns>True if the stage was removed, false if otherwise.</returns>
        public bool RemoveStageAt(int index)
        {
            if (index < 0 || index >= _stages.Count)
            {
                return false;
            }

            _stages.RemoveAt(index);
            _version++;

            return true;
        }

        /// <summary>
        /// Removes all render stages from the collection.
        /// </summary>
        public void RemoveAllStages()
        {
            _stages.Clear();
            _version++;
        }

        /// <summary>
        /// Executes all the render stages in the collection.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="queue">Render queue of objects to be processed.</param>
        public void ExecuteStages(IRenderContext renderContext, RenderQueue queue)
        {
            for (int i = 0; i < _stages.Count; i++)
            {
                _stages[i].Execute(renderContext, queue);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public RenderStageCollectionEnumerator GetEnumerator()
        {
            return new RenderStageCollectionEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<IRenderStage> IEnumerable<IRenderStage>.GetEnumerator()
        {
            return new RenderStageCollectionEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RenderStageCollectionEnumerator(this);
        }

        #region Enumerator

        /// <summary>
        /// Enumerates elements of a <see cref="RenderStageCollection"/>.
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RenderStageCollectionEnumerator : IEnumerator<IRenderStage>
        {
            private int _index;
            private IRenderStage _current;
            private readonly RenderStageCollection _renderStages;
            private readonly List<IRenderStage> _stages;
            private readonly int _count;
            private readonly int _version;

            /// <summary>
            /// Initializes a new instance of the <see cref="RenderStageCollectionEnumerator"/> struct.
            /// </summary>
            /// <param name="renderStages">Render stage collection to enumerate</param>
            internal RenderStageCollectionEnumerator(RenderStageCollection renderStages)
            {
                _renderStages = renderStages;
                _stages = renderStages._stages;
                _index = 0;
                _count = _stages.Count;
                _version = renderStages._version;
                _current = null;
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public IRenderStage Current => _current;

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
                    _current = _stages[_index];
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
                if (_version != _renderStages._version)
                {
                    throw new InvalidOperationException("Collection modified during enumeration");
                }
            }
        }

        #endregion
    }
}
