namespace Spark.UI.Input
{
    using Math;

    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    public class MouseEventArgs : InputEventArgs
    {
        public MouseEventArgs(MouseDevice mouse)
            : base(mouse)
        {
        }

        public MouseDevice MouseDevice => (MouseDevice)Device;

        public Vector2 GetPosition(IInputElement relativeTo)
        {
            return MouseDevice.GetPosition(relativeTo);
        }
    }
}
