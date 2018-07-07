namespace Spark.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class GradientStopCollection : IList<GradientStop>
    {
        private readonly List<GradientStop> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStopCollection"/> class.
        /// </summary>
        public GradientStopCollection()
        {
            _list = new List<GradientStop>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStopCollection"/> class.
        /// </summary>
        /// <param name="parameters">Gradient stops.</param>
        public GradientStopCollection(params GradientStop[] parameters)
        {
            _list = new List<GradientStop>(parameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStopCollection"/> class.
        /// </summary>
        /// <param name="parameters">Gradient stops.</param>
        public GradientStopCollection(IEnumerable<GradientStop> parameters)
        {
            _list = new List<GradientStop>(parameters);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        internal Action BrushInvalidate { get; set; }

        public GradientStop this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                InvalidateParentBrush();
            }
        }

        public void Add(GradientStop item)
        {
            _list.Add(item);
            InvalidateParentBrush();
        }

        public void Clear()
        {
            _list.Clear();
            InvalidateParentBrush();
        }

        public bool Contains(GradientStop item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(GradientStop[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(GradientStop item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, GradientStop item)
        {
            _list.Insert(index, item);
            InvalidateParentBrush();
        }

        public bool Remove(GradientStop item)
        {
            bool removed = _list.Remove(item);
            if (removed)
            {
                InvalidateParentBrush();
            }

            return removed;
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            InvalidateParentBrush();
        }

        public IEnumerator<GradientStop> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private void InvalidateParentBrush()
        {
            if (BrushInvalidate != null)
            {
                BrushInvalidate();
            }
        }
    }
}
