namespace Spark.UI.Controls
{
    using System.Collections.Specialized;

    using Core.Collections;

    public class UIElementCollection : NotifiableCollection<UIElement>
    {
        private readonly UIElement _visualParent;
        private readonly FrameworkElement _logicalParent;

        public UIElementCollection(UIElement visualParent, FrameworkElement logicalParent)
        {
            _visualParent = visualParent;
            _logicalParent = logicalParent;
        }

        protected void ClearLogicalParent(UIElement element)
        {
            if (_logicalParent != null)
            {
                _logicalParent.RemoveLogicalChild(element);
            }

            if (_visualParent != null)
            {
                _visualParent.RemoveVisualChild(element);
            }
        }

        protected void SetLogicalParent(UIElement element)
        {
            if (_logicalParent != null)
            {
                _logicalParent.AddLogicalChild(element);
            }

            if (_visualParent != null)
            {
                _visualParent.AddVisualChild(element);
            }
        }

        protected override void OnClearing()
        {
            foreach (UIElement item in this)
            {
                ClearLogicalParent(item);
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (UIElement element in e.NewItems)
                    {
                        SetLogicalParent(element);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (UIElement element in e.OldItems)
                    {
                        ClearLogicalParent(element);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (UIElement element in e.OldItems)
                    {
                        ClearLogicalParent(element);
                    }

                    foreach (UIElement element in e.NewItems)
                    {
                        SetLogicalParent(element);
                    }

                    break;
            }

            base.OnItemsChanged(e);
        }
    }
}
