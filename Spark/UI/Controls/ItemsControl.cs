namespace Spark.UI.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Markup;

    using Controls.Primitives;
    using Media;

    [ContentProperty(nameof(Items))]
    public class ItemsControl : Control
    {
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                nameof(DisplayMemberPath),
                typeof(string),
                typeof(ItemsControl),
                new PropertyMetadata(null, new PropertyChangedCallback(DisplayMemberPathChanged)));

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                nameof(Items),
                typeof(ItemCollection),
                typeof(ItemsControl));

        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register(
                nameof(ItemsPanel),
                typeof(ItemsPanelTemplate),
                typeof(ItemsControl),
                new PropertyMetadata(new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)))));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(ItemsControl),
                new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourceChanged)));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(ItemsControl),
                new PropertyMetadata(ItemTemplateChanged));

        private DataTemplate _displayMemberTemplate;
        private bool _itemsIsDataBound;
        private ItemsPresenter _itemsPresenter;

        public ItemsControl()
        {
            DefaultStyleKey = typeof(ItemsControl);
            ItemContainerGenerator = new ItemContainerGenerator(this);
            ItemContainerGenerator.ItemsChanged += OnItemContainerGeneratorChanged;

            ItemCollection items = new ItemCollection();
            ((INotifyCollectionChanged)items).CollectionChanged += InvokeItemsChanged;
            _itemsIsDataBound = false;
            SetValue(ItemsProperty, items);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public ItemCollection Items => (ItemCollection)GetValue(ItemsProperty);

        public string SelectedValuePath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public ItemsPanelTemplate ItemsPanel
        {
            get => (ItemsPanelTemplate)GetValue(ItemsPanelProperty);
            set => SetValue(ItemsPanelProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set
            {
                if (!_itemsIsDataBound && Items.Count > 0)
                {
                    throw new InvalidOperationException("Items collection must be empty before using ItemsSource.");
                }

                SetValue(ItemsSourceProperty, value);
            }
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public ItemContainerGenerator ItemContainerGenerator
        {
            get;
            private set;
        }

        internal Panel Panel => _itemsPresenter == null ? null : _itemsPresenter.Child;

        private DataTemplate DisplayMemberTemplate
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SelectedValuePath) && _displayMemberTemplate == null)
                {
                    MemoryStream s = new MemoryStream(
                        Encoding.UTF8.GetBytes(@"
                            <DataTemplate xmlns=""https://github.com/Mattiemus/Spark""
                                          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                <TextBlock Text=""{Binding " + SelectedValuePath + @"}"" />
                            </DataTemplate>"));

                    _displayMemberTemplate = (DataTemplate)XamlReader.Load(s);
                }

                return _displayMemberTemplate;
            }
        }

        public static ItemsControl GetItemsOwner(DependencyObject element)
        {
            Panel panel = element as Panel;

            if (panel != null && panel.IsItemsHost)
            {
                ItemsPresenter itemsPresenter = VisualTreeHelper.GetParent(panel) as ItemsPresenter;

                if (itemsPresenter != null)
                {
                    return (ItemsControl)itemsPresenter.TemplatedParent;
                }
            }

            return null;
        }

        public static ItemsControl ItemsControlFromItemContainer(DependencyObject container)
        {
            var e = container as FrameworkElement;

            if (e != null)
            {
                var itctl = e.Parent as ItemsControl;

                if (itctl == null)
                {
                    return GetItemsOwner(e.Parent);
                }

                if (itctl.IsItemItsOwnContainer(e))
                {
                    return itctl;
                }
            }

            return null;
        }

        public override void OnApplyTemplate()
        {
            ItemsPresenter itemsPresenter =
                VisualTreeHelper.GetDescendents<ItemsPresenter>(this).FirstOrDefault();

            if (itemsPresenter != null)
            {
                itemsPresenter.Child = (Panel)ItemsPanel.CreateVisualTree(itemsPresenter);
                itemsPresenter.Child.IsItemsHost = true;
                SetItemsPresenter(itemsPresenter);
            }
        }

        internal void ClearContainerForItem(DependencyObject element, object item)
        {
            ClearContainerForItemOverride(element, item);
        }

        internal DependencyObject GetContainerForItem()
        {
            return GetContainerForItemOverride();
        }

        internal bool IsItemItsOwnContainer(object item)
        {
            return IsItemItsOwnContainerOverride(item);
        }

        internal virtual void OnItemsSourceChanged(IEnumerable oldSource, IEnumerable newSource)
        {
            if (newSource != null)
            {
                _itemsIsDataBound = true;
                Items.SetSource(newSource);

                // Setting itemsIsDataBound to true prevents normal notifications from propagating, so do it manually here
                OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                _itemsIsDataBound = false;
                Items.SetSource(null);
            }
        }

        internal virtual void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            int count = Items.Count;

            for (int i = 0; i < count; i++)
            {
                UpdateContentTemplateOnContainer(
                    ItemContainerGenerator.ContainerFromIndex(i),
                    Items[i]);
            }
        }

        internal void PrepareContainerForItem(DependencyObject element, object item)
        {
            PrepareContainerForItemOverride(element, item);
        }

        internal void SetItemsPresenter(ItemsPresenter presenter)
        {
            if (_itemsPresenter != null)
            {
                _itemsPresenter.Child.InternalChildren.Clear();
            }

            _itemsPresenter = presenter;
            AddItemsToPresenter(new GeneratorPosition(-1, 1), Items.Count);
        }

        protected virtual void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            // nothing to undo by default (since nothing was prepared)
        }

        protected virtual DependencyObject GetContainerForItemOverride()
        {
            return new ContentPresenter();
        }

        protected virtual bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrameworkElement;
        }

        protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        protected virtual void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (SelectedValuePath != null && ItemTemplate != null)
            {
                throw new InvalidOperationException("Cannot set 'DisplayMemberPath' and 'ItemTemplate' simultaneously.");
            }

            UpdateContentTemplateOnContainer(element, item);
        }

        private static void DisplayMemberPathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)o).OnDisplayMemberPathChanged(
                e.OldValue as string,
                e.NewValue as string);
        }

        private static void ItemTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)sender).OnItemTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        private static void ItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)o).OnItemsSourceChanged(
                e.OldValue as IEnumerable,
                e.NewValue as IEnumerable);
        }

        private void InvokeItemsChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    SetLogicalParent(this, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    SetLogicalParent(null, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    SetLogicalParent(null, e.OldItems);
                    SetLogicalParent(this, e.NewItems);
                    break;
            }

            ItemContainerGenerator.OnOwnerItemsItemsChanged(o, e);

            if (!_itemsIsDataBound)
            {
                OnItemsChanged(e);
            }
        }

        private void OnDisplayMemberPathChanged(string oldPath, string newPath)
        {
            // refresh the display member template.
            _displayMemberTemplate = null;
            var newTemplate = DisplayMemberTemplate;

            int count = Items.Count;
            for (int i = 0; i < count; i++)
            {
                UpdateContentTemplateOnContainer(ItemContainerGenerator.ContainerFromIndex(i), Items[i]);
            }
        }

        private void OnItemContainerGeneratorChanged(object sender, ItemsChangedEventArgs e)
        {
            if (_itemsPresenter == null || _itemsPresenter.Child is VirtualizingPanel)
            {
                return;
            }

            Panel panel = _itemsPresenter.Child;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    // The list has gone away, so clear the children of the panel
                    if (panel.InternalChildren.Count > 0)
                    {
                        RemoveItemsFromPresenter(new GeneratorPosition(0, 0), panel.InternalChildren.Count);
                    }
                    break;

                case NotifyCollectionChangedAction.Add:
                    AddItemsToPresenter(e.Position, e.ItemCount);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveItemsFromPresenter(e.Position, e.ItemCount);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveItemsFromPresenter(e.Position, e.ItemCount);
                    AddItemsToPresenter(e.Position, e.ItemCount);
                    break;
            }
        }

        private void SetLogicalParent(FrameworkElement parent, IList items)
        {
            foreach (DependencyObject o in items.OfType<DependencyObject>())
            {
                FrameworkElement p = LogicalTreeHelper.GetParent(o) as FrameworkElement;
                if (p != null)
                {
                    p.RemoveLogicalChild(o);
                }
            }

            if (parent != null)
            {
                foreach (object o in items)
                {
                    parent.AddLogicalChild(o);
                }
            }
        }

        private void AddItemsToPresenter(GeneratorPosition position, int count)
        {
            if (_itemsPresenter == null ||
                _itemsPresenter.Child == null ||
                _itemsPresenter.Child is VirtualizingPanel)
            {
                return;
            }

            Panel panel = _itemsPresenter.Child;
            int newIndex = ItemContainerGenerator.IndexFromGeneratorPosition(position);

            using (IDisposable p = ItemContainerGenerator.StartAt(position, GeneratorDirection.Forward, true))
            {
                for (int i = 0; i < count; i++)
                {
                    object item = Items[newIndex + i];
                    DependencyObject container = null;

                    bool fresh;
                    container = ItemContainerGenerator.GenerateNext(out fresh);

                    FrameworkElement f = container as FrameworkElement;

                    if (f != null && !(item is FrameworkElement))
                    {
                        f.DataContext = item;
                    }

                    panel.InternalChildren.Insert(newIndex + i, (UIElement)container);
                    ItemContainerGenerator.PrepareItemContainer(container);
                }
            }
        }

        private void RemoveItemsFromPresenter(GeneratorPosition position, int count)
        {
            if (_itemsPresenter == null ||
                _itemsPresenter.Child == null ||
                _itemsPresenter.Child is VirtualizingPanel)
            {
                return;
            }

            Panel panel = _itemsPresenter.Child;

            while (count-- > 0)
            {
                panel.InternalChildren.RemoveAt(position.Index);
            }
        }

        private void UpdateContentTemplateOnContainer(DependencyObject element, object item)
        {
            if (element == item)
            {
                return;
            }

            ContentPresenter presenter = element as ContentPresenter;
            ContentControl control = element as ContentControl;

            DataTemplate template = null;

            if (!(item is UIElement))
            {
                template = ItemTemplate;

                if (template == null)
                {
                    template = DisplayMemberTemplate;
                }
            }

            if (presenter != null)
            {
                presenter.ContentTemplate = template;
                presenter.Content = item;
            }
            else if (control != null)
            {
                control.ContentTemplate = template;
                control.Content = item;
            }
        }
    }
}
