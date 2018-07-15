namespace Spark.UI.Controls
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;

    using Data;

    public sealed class ItemCollection : CollectionView
    {
        private ObservableCollection<object> _internalList;

        internal ItemCollection()
            : base(new ObservableCollection<object>())
        {
            _internalList = (ObservableCollection<object>)base.SourceCollection;
        }

        internal ItemCollection(IEnumerable source)
            : base(source)
        {
        }

        public override IEnumerable SourceCollection => (_internalList != null) ? this : base.SourceCollection;

        public object this[int index]
        {
            get => ((IList)base.SourceCollection)[index];
            set => ((IList)base.SourceCollection)[index] = value;
        }

        public int Add(object newItem)
        {
            ThrowIfNotWritable();
            _internalList.Add(newItem);
            return Count - 1;
        }

        internal new void SetSource(IEnumerable source)
        {
            if (source != null)
            {
                _internalList = null;
                base.SetSource(source);
            }
            else if (_internalList == null)
            {
                _internalList = new ObservableCollection<object>();
                base.SetSource(_internalList);
            }
        }

        private void ThrowIfNotWritable()
        {
            if (_internalList == null)
            {
                throw new InvalidOperationException("Cannot modify the Items collection while ItemsSource is set.");
            }
        }
    }
}
