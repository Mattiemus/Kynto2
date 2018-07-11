namespace Spark.UI.Input
{
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    public class MouseButtonEventArgs : MouseEventArgs
    {
        public MouseButtonEventArgs(MouseDevice device)
            : base(device)
        {
        }
    }
}
