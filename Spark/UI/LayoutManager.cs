namespace Spark.UI
{
    using Media;
    using System.Collections.Generic;

    internal class LayoutManager
    {
        private readonly List<UIElement> _entries = new List<UIElement>();
        private bool _layoutPassQueued;

        static LayoutManager()
        {
            Instance = new LayoutManager();
        }

        public static LayoutManager Instance { get; }

        public void QueueMeasure(UIElement e)
        {
            if (!_entries.Contains(e))
            {
                _entries.Add(e);
            }

            QueueLayoutPass();
        }

        public void QueueArrange(UIElement e)
        {
            QueueMeasure(e);
        }

        private void QueueLayoutPass()
        {
            if (!_layoutPassQueued)
            {
                DoLayout();
                _layoutPassQueued = true;
            }
        }

        private void DoLayout()
        {
            List<ITopLevelElement> topLevelElements = new List<ITopLevelElement>();

            foreach (UIElement entry in _entries)
            {
                ITopLevelElement topLevelElement = VisualTreeHelper.GetTopLevelElement(entry);

                if (topLevelElement != null && !topLevelElements.Contains(topLevelElement))
                {
                    topLevelElements.Add(topLevelElement);
                }
            }

            foreach (ITopLevelElement topLevelElement in topLevelElements)
            {
                topLevelElement.DoLayoutPass();
            }

            _entries.Clear();
            _layoutPassQueued = false;
        }
    }
}
