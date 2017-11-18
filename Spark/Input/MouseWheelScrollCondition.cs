namespace Spark.Input
{
    /// <summary>
    /// Checks if the mouse wheel has scrolled since the last update.
    /// </summary>
    public sealed class MouseWheelScrollCondition : InputCondition
    {
        private int _prevWheelValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseWheelScrollCondition"/> class.
        /// </summary>
        public MouseWheelScrollCondition() 
            : this(ScrollDirection.Both)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseWheelScrollCondition"/> class.
        /// </summary>
        /// <param name="moveDir">Move direction to check.</param>
        public MouseWheelScrollCondition(ScrollDirection moveDir)
        {
            _prevWheelValue = Mouse.GetMouseState().ScrollWheelValue;
            Direction = moveDir;
        }

        /// <summary>
        /// Sets the valid move direction.
        /// </summary>
        public ScrollDirection Direction { get; set; }

        /// <summary>
        /// Checks if the condition has been satisfied or not.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        /// <returns>True if the condition has been satisfied, false otherwise.</returns>
        public override bool Check(IGameTime time)
        {
            MouseState currState = Mouse.GetMouseState();

            bool hasMoved = currState.ScrollWheelValue != _prevWheelValue;
            bool movedValidDir = (currState.ScrollWheelValue > _prevWheelValue && Direction != ScrollDirection.Backward) || (currState.ScrollWheelValue < _prevWheelValue && Direction != ScrollDirection.Forward);
            _prevWheelValue = currState.ScrollWheelValue;

            return hasMoved && movedValidDir;
        }
    }
}
