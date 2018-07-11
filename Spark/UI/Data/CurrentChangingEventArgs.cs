namespace Spark.UI.Data
{
    using System;

    public delegate void CurrentChangingEventHandler(object sender, CurrentChangingEventArgs e);

    public class CurrentChangingEventArgs : EventArgs
    {
        public CurrentChangingEventArgs()
        {
            IsCancelable = true;
        }

        public CurrentChangingEventArgs(bool isCancelable)
        {
            IsCancelable = isCancelable;
        }

        public bool Cancel { get; set; }

        public bool IsCancelable { get; private set; }
    }
}
