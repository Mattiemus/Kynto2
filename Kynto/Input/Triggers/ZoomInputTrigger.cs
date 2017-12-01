namespace Kynto.Input.Triggers
{
    using System;

    using Spark;
    using Spark.Input;
    using Spark.Application;

    /// <summary>
    /// Input trigger that responds to input and performs a <see cref="ZoomInputAction"/>.
    /// </summary>
    public class ZoomInputTrigger : InputTrigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomInputTrigger"/> class.
        /// </summary>
        /// <param name="zoomCmd">Zoom action delegate.</param>
        public ZoomInputTrigger(Action<int, IGameTime> zoomCmd)
            : this("Zoom", zoomCmd)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomInputTrigger"/> class.
        /// </summary>
        /// <param name="name">Trigger name.</param>
        /// <param name="zoomCmd">Zoom action delegate.</param>
        public ZoomInputTrigger(string name, Action<int, IGameTime> zoomCmd)
            : base(name, new MouseWheelScrollCondition(), new ZoomInputAction(zoomCmd))
        {
        }
    }

    /// <summary>
    /// Zoom input action where mouse wheel movements determine how much a camera should zoom in/out (usually translated along a direction vector).
    /// </summary>
    public class ZoomInputAction : CursorInputAction
    {
        private int _prevWheelValue;
        private readonly Action<int, IGameTime> _zoomCmd;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomInputAction"/> class.
        /// </summary>
        /// <param name="zoomCmd">Zoom action delegate.</param>
        public ZoomInputAction(Action<int, IGameTime> zoomCmd)
        {
            if (zoomCmd == null)
            {
                throw new ArgumentNullException(nameof(zoomCmd));
            }

            _zoomCmd = zoomCmd;
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        public override void Perform(IGameTime time)
        {
            Mouse.GetMouseState(out MouseState ms);

            int wheelValue = ms.ScrollWheelValue;
            int delta = wheelValue - _prevWheelValue;

            if (delta != 0)
            {
                _zoomCmd(delta, time);
            }

            _prevWheelValue = wheelValue;
        }
    }
}
