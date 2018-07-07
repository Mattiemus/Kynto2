namespace Spark.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class UIElementCollection : IList<UIElement>
    {
        private readonly UIElement _collectionOwner;
        private readonly List<UIElement> _elements;

        public UIElementCollection()
        {
            _elements = new List<UIElement>();
        }

        public UIElementCollection(UIElement owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            _elements = new List<UIElement>();
            _collectionOwner = owner;
        }

        public int Count => _elements.Count;

        public bool IsReadOnly => false;

        public int IndexOf(UIElement item)
        {
            return _elements.IndexOf(item);
        }

        public void Insert(int index, UIElement item)
        {
            if (_collectionOwner != null)
            {
                item.Parent = _collectionOwner;
            }

            _elements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _elements.RemoveAt(index);
        }

        public UIElement this[int index]
        {
            get => _elements[index];
            set
            {
                if (_collectionOwner != null)
                {
                    value.Parent = _collectionOwner;
                }

                _elements[index] = value;
            }
        }

        public void Add(UIElement item)
        {
            if (_collectionOwner != null)
            {
                item.Parent = _collectionOwner;
            }

            _elements.Add(item);
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public bool Contains(UIElement item)
        {
            return _elements.Contains(item);
        }

        public void CopyTo(UIElement[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public bool Remove(UIElement item)
        {
            return _elements.Remove(item);
        }

        public IEnumerator<UIElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
