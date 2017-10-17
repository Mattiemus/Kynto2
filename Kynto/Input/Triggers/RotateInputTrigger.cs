namespace Kynto.Input.Triggers
{
    using System;

    using Spark.Core;
    using Spark.Math;
    using Spark.Input;

    /// <summary>
    /// Input trigger that responds to input and performs a <see cref="RotateInputAction"/>.
    /// </summary>
    public class RotateInputTrigger : InputTrigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateInputTrigger"/> class.
        /// </summary>
        /// <param name="binding">Button binding.</param>
        /// <param name="rotateCmd">Rotate action delegate.</param>
        public RotateInputTrigger(KeyOrMouseButton binding, Action<Int2, IGameTime> rotateCmd)
            : this("Rotate", binding, rotateCmd)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateInputTrigger"/> class.
        /// </summary>
        /// <param name="name">Trigger name.</param>
        /// <param name="binding">Button binding.</param>
        /// <param name="rotateCmd">Rotate action delegate.</param>
        public RotateInputTrigger(string name, KeyOrMouseButton binding, Action<Int2, IGameTime> rotateCmd)
            : base(name, KeyOrMouseButton.CreateInputCondition(binding, false, true), new RotateInputAction(rotateCmd))
        {
        }
    }

    /// <summary>
    /// Rotate input action where mouse movements determine how much a camera should be rotated.
    /// </summary>
    public class RotateInputAction : CursorInputAction
    {
        private Int2 _prevScreenPt;
        private readonly Action<Int2, IGameTime> _rotateCmd;

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateInputAction"/> class.
        /// </summary>
        /// <param name="rotateCmd">Rotate action delegate.</param>
        public RotateInputAction(Action<Int2, IGameTime> rotateCmd)
        {
            if (rotateCmd == null)
            {
                throw new ArgumentNullException(nameof(rotateCmd));
            }

            _rotateCmd = rotateCmd;
            Cursor = Cursors.Rotate;
        }

        /// <summary>
        /// Called by an input trigger when the condition first succeeds, allowing the action to do first-time setup.
        /// </summary>
        /// <param name="time"></param>
        public override void OnBegin(IGameTime time)
        {
            base.OnBegin(time);
            
            Mouse.GetMouseState(out MouseState ms);

            _prevScreenPt = ms.PositionInt;
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        public override void Perform(IGameTime time)
        {
            Mouse.GetMouseState(out MouseState ms);

            Int2 screenPt = ms.PositionInt;
            Int2.Subtract(ref screenPt, ref _prevScreenPt, out Int2 delta);

            if (delta != Int2.Zero)
            {
                _rotateCmd(delta, time);
            }

            _prevScreenPt = screenPt;
        }
    }
}
