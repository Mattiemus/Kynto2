namespace Spark.UI
{
    using System;

    public delegate void RoutedEventHandler(object sender, RoutedEventArgs e);

    public class RoutedEventArgs : EventArgs
    {
        public RoutedEventArgs()
        {
        }

        public RoutedEventArgs(RoutedEvent routedEvent)
        {
            RoutedEvent = routedEvent;
        }

        public RoutedEventArgs(RoutedEvent routedEvent, object source)
        {
            RoutedEvent = routedEvent;
            Source = source;
        }

        public bool Handled { get; set; }

        public object OriginalSource { get; set; }

        public RoutedEvent RoutedEvent { get; set; }

        public object Source { get; set; }
    }
}
