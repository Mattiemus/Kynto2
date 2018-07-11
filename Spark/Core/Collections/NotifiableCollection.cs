namespace Spark.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public class NotifiableCollection<T> : IList<T>, IList
    {
        private readonly List<T> _items;
        private bool _isReadOnly;

        protected NotifiableCollection()
        {
            _items = new List<T>();
        }

        protected NotifiableCollection(IEnumerable<T> items)
        {
            _items = new List<T>(items);
        }

        protected NotifiableCollection(int capacity)
        {
            _items = new List<T>(capacity);
        }

        internal event EventHandler Clearing;

        internal event NotifyCollectionChangedEventHandler ItemsChanged;

        public int Capacity
        {
            get => _items.Capacity;
            set => _items.Capacity = value;
        }

        public int Count => _items.Count;

        public bool IsReadOnly => _isReadOnly;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        bool IList.IsFixedSize => false;

        public T this[int index]
        {
            get => _items[index];
            set
            {
                CheckReadOnly();
                SetItemInternal(index, value);
            }
        }

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        public int Add(T value)
        {
            CheckReadOnly();
            return AddInternal(value);
        }

        public void Clear()
        {
            CheckReadOnly();
            ClearInternal();
        }

        public bool Contains(T value)
        {
            return _items.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            _items.ToArray().CopyTo(array, index);
        }

        public void CopyTo(T[] array, int index)
        {
            _items.CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(T value)
        {
            return _items.IndexOf(value);
        }

        public void Insert(int index, T value)
        {
            CheckReadOnly();
            InsertInternal(index, value);
        }

        public bool Remove(T value)
        {
            CheckReadOnly();
            return RemoveInternal(value);
        }

        public void RemoveAt(int index)
        {
            CheckReadOnly();
            RemoveAtInternal(index);
        }

        public void RemoveRange(int index, int count)
        {
            _items.RemoveRange(index, count);
        }

        void ICollection<T>.Add(T value)
        {
            _items.Add(value);
        }

        int IList.Add(object value)
        {
            return Add((T)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal int AddInternal(T value)
        {
            int index = _items.Count;
            _items.Add(value);
            RaiseItemsChanged(NotifyCollectionChangedAction.Add, value, index);
            return index;
        }

        internal void ClearInternal()
        {
            OnClearing();
            _items.Clear();
            RaiseItemsChanged(NotifyCollectionChangedAction.Reset);
        }

        internal void InsertInternal(int index, T value)
        {
            _items.Insert(index, value);
            RaiseItemsChanged(NotifyCollectionChangedAction.Add, value, index);
        }

        internal bool RemoveInternal(T value)
        {
            int index = _items.IndexOf(value);

            if (index != -1)
            {
                _items.RemoveAt(index);
                RaiseItemsChanged(NotifyCollectionChangedAction.Remove, value, index);
                return true;
            }

            return false;
        }

        internal void RemoveAtInternal(int index)
        {
            object value = _items[index];
            _items.RemoveAt(index);
            RaiseItemsChanged(NotifyCollectionChangedAction.Remove, value, index);
        }

        internal virtual void SetIsReadOnly(bool readOnly)
        {
            _isReadOnly = readOnly;
        }

        internal void SetItemInternal(int index, T value)
        {
            T old = _items[index];

            if (!old.Equals(value))
            {
                _items[index] = value;
                RaiseItemsChanged(NotifyCollectionChangedAction.Replace, value, old, index);
            }
        }

        protected virtual void OnClearing()
        {
            Clearing?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            ItemsChanged?.Invoke(this, e);
        }

        private void CheckReadOnly()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("The collection is readonly.");
            }
        }

        private void RaiseItemsChanged(NotifyCollectionChangedAction action)
        {
            OnItemsChanged(new NotifyCollectionChangedEventArgs(action));
        }

        private void RaiseItemsChanged(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            OnItemsChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index));
        }

        private void RaiseItemsChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            OnItemsChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }
    }
}
