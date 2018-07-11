namespace Spark.UI.Data
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    public class CollectionView : ICollectionView, INotifyPropertyChanged
    {
        private IEnumerable _sourceCollection;
        private object _currentItem;
        private int _currentPosition;
        private NotifyCollectionChangedEventHandler _collectionChanged;
        private PropertyChangedEventHandler _propertyChanged;

        public CollectionView(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _sourceCollection = collection;

            if (collection.Cast<object>().Any())
            {
                _currentItem = collection.Cast<object>().First();
                _currentPosition = 0;
            }
            else
            {
                _currentPosition = -1;
                IsCurrentAfterLast = IsCurrentBeforeFirst = true;
            }

            INotifyCollectionChanged incc = collection as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += OnCollectionChanged;
            }
        }

        public virtual event EventHandler CurrentChanged;

        public virtual event CurrentChangingEventHandler CurrentChanging;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { _collectionChanged += value; }
            remove { _collectionChanged -= value; }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        public virtual bool CanFilter => true;

        public virtual bool CanGroup => false;

        public virtual bool CanSort => false;

        public virtual IComparer Comparer => null;

        public virtual int Count
        {
            get
            {
                ICollection collection = _sourceCollection as ICollection;
                return (collection != null) ?
                    collection.Count :
                    _sourceCollection.Cast<object>().Count();
            }
        }

        public virtual CultureInfo Culture { get; set; }

        public virtual object CurrentItem => _currentItem;

        public virtual int CurrentPosition => _currentPosition;

        public virtual Predicate<object> Filter { get; set; }

        public virtual ObservableCollection<GroupDescription> GroupDescriptions => null;

        public virtual ReadOnlyObservableCollection<object> Groups => null;

        public virtual bool IsCurrentAfterLast { get; private set; }

        public virtual bool IsCurrentBeforeFirst { get; private set; }

        public virtual IEnumerable SourceCollection => _sourceCollection;

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Filter == null)
            {
                return _sourceCollection.GetEnumerator();
            }

            return _sourceCollection
                .Cast<object>()
                .Where(x => Filter(x))
                .GetEnumerator();
        }

        public virtual bool Contains(object item)
        {
            return _sourceCollection.Cast<object>().Contains(item);
        }

        public virtual int IndexOf(object item)
        {
            IComparer comparer = Comparer;
            int i = 0;

            foreach (object o in _sourceCollection)
            {
                if ((comparer != null && comparer.Compare(item, o) == 0) || o == item)
                {
                    return i;
                }

                ++i;
            }

            return -1;
        }

        public virtual bool MoveCurrentToFirst()
        {
            return MoveCurrentToPosition(0);
        }

        public virtual bool MoveCurrentToLast()
        {
            return MoveCurrentToPosition(Count - 1);
        }

        public virtual bool MoveCurrentToNext()
        {
            return MoveCurrentToPosition(_currentPosition + 1);
        }

        public virtual bool MoveCurrentToPosition(int position)
        {
            int count = Count;
            if (position < -1 || position > count)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            if (OKToChangeCurrent())
            {
                bool result;

                if (position == -1 || (position == 0 && count == 0))
                {
                    IsCurrentBeforeFirst = true;
                    IsCurrentAfterLast = false;
                    _currentPosition = -1;
                    _currentItem = null;
                    result = false;
                }
                else if (position == count)
                {
                    IsCurrentBeforeFirst = false;
                    IsCurrentAfterLast = true;
                    _currentPosition = position;
                    _currentItem = null;
                    result = false;
                }
                else
                {
                    IsCurrentBeforeFirst = IsCurrentAfterLast = false;
                    _currentPosition = position;
                    _currentItem = _sourceCollection.Cast<object>().ElementAt(position);
                    result = true;
                }

                OnCurrentChanged();

                return result;
            }

            return true;
        }

        public virtual bool MoveCurrentToPrevious()
        {
            if (_currentPosition > -1)
            {
                return MoveCurrentToPosition(_currentPosition - 1);
            }

            return false;
        }

        internal void SetSource(IEnumerable collection)
        {
            INotifyCollectionChanged incc = _sourceCollection as INotifyCollectionChanged;

            if (incc != null)
            {
                incc.CollectionChanged -= OnCollectionChanged;
            }

            _sourceCollection = collection;
            incc = _sourceCollection as INotifyCollectionChanged;

            if (incc != null)
            {
                incc.CollectionChanged += OnCollectionChanged;
            }
        }

        protected bool OKToChangeCurrent()
        {
            CurrentChangingEventArgs e = new CurrentChangingEventArgs();
            OnCurrentChanging(e);
            return !e.Cancel;
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            ProcessCollectionChanged(args);
            OnCollectionChanged(args);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            _collectionChanged?.Invoke(this, args);
        }

        protected virtual void OnCurrentChanged()
        {
            CurrentChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void OnCurrentChanging()
        {
            _currentPosition = -1;
            OnCurrentChanging(new CurrentChangingEventArgs(false));
        }

        protected virtual void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            CurrentChanging?.Invoke(this, args);
        }

        protected virtual void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
        }
    }
}
