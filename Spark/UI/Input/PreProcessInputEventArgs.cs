namespace Spark.UI.Input
{
    using System;

    public delegate void PreProcessInputEventHandler(object sender, PreProcessInputEventArgs e);

    public class PreProcessInputEventArgs : EventArgs
    {
        private bool _canceled;

        public PreProcessInputEventArgs(InputEventArgs input)
        {
            Input = input;
        }

        public bool Canceled => _canceled;

        public InputEventArgs Input { get; }

        public void Cancel()
        {
            _canceled = true;
        }
    }
}
