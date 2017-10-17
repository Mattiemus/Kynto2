namespace Kynto.Input.Triggers
{
    using System;

    using Spark.Core;
    using Spark.Math;
    using Spark.Input;

    /// <summary>
    /// Input trigger that responds to input and performs a <see cref="PanInputAction"/>.
    /// </summary>
    public class PanInputTrigger : InputTrigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PanInputTrigger"/> class.
        /// </summary>
        /// <param name="binding">Button binding.</param>
        /// <param name="panCmd">Pan action delegate.</param>
        public PanInputTrigger(KeyOrMouseButton binding, Action<Int2, IGameTime> panCmd)
            : this("Pan", binding, panCmd)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PanInputTrigger"/> class.
        /// </summary>
        /// <param name="name">Trigger name.</param>
        /// <param name="binding">Button binding.</param>
        /// <param name="panCmd">Pan action delegate.</param>
        public PanInputTrigger(string name, KeyOrMouseButton binding, Action<Int2, IGameTime> panCmd)
            : base(name, KeyOrMouseButton.CreateInputCondition(binding, false, true), new PanInputAction(panCmd))
        {
        }
    }

    /// <summary>
    /// Pan input action where mouse movements determine how much a camera should be translated across the pan axes.
    /// </summary>
    public class PanInputAction : CursorInputAction
    {
        private readonly Action<Int2, IGameTime> _panCmd;
        private Int2 _prevScreenPt;

        /// <summary>
        /// Initializes a new instance of the <see cref="PanInputAction"/> class.
        /// </summary>
        /// <param name="panCmd">Pan action delegate</param>
        public PanInputAction(Action<Int2, IGameTime> panCmd)
        {
            if (panCmd == null)
            {
                throw new ArgumentNullException(nameof(panCmd));
            }

            _panCmd = panCmd;
            Cursor = Cursors.ClosedHand;
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
                _panCmd(delta, time);
            }

            _prevScreenPt = screenPt;
        }
    }
}
