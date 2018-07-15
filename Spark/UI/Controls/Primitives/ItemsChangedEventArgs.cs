namespace Spark.UI.Controls.Primitives
{
    using System;
    using System.Collections.Specialized;

    public delegate void ItemsChangedEventHandler(object sender, ItemsChangedEventArgs e);

    public class ItemsChangedEventArgs : EventArgs
    {
        internal ItemsChangedEventArgs()
        {
        }

        public NotifyCollectionChangedAction Action { get; internal set; }

        public int ItemCount { get; internal set; }

        public int ItemUICount { get; internal set; }

        public GeneratorPosition OldPosition { get; internal set; }

        public GeneratorPosition Position { get; internal set; }
    }
}
