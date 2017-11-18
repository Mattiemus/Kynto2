namespace Spark.Input
{
    using Math;
    
    /// <summary>
    /// Checks if the mouse has moved since the last update.
    /// </summary>
    public sealed class MouseMovedCondition : InputCondition
    {
        private Int2 _pos;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMovedCondition"/> class.
        /// </summary>
        public MouseMovedCondition() 
            : this(MoveDirection.XAndY)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMovedCondition"/> class.
        /// </summary>
        /// <param name="moveDir">Move direction to check.</param>
        public MouseMovedCondition(MoveDirection moveDir)
        {
            _pos = Mouse.GetMouseState().PositionInt;
            Direction = moveDir;
        }

        /// <summary>
        /// Sets the valid move direction.
        /// </summary>
        public MoveDirection Direction { get; set; }

        /// <summary>
        /// Checks if the condition has been satisfied or not.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        /// <returns>True if the condition has been satisfied, false otherwise.</returns>
        public override bool Check(IGameTime time)
        {
            MouseState currState = Mouse.GetMouseState();

            bool hasMoved = (_pos.X != currState.X && Direction != MoveDirection.YOnly) || (_pos.Y != currState.Y && Direction != MoveDirection.XOnly);
            _pos.X = currState.X;
            _pos.Y = currState.Y;

            return hasMoved;
        }
    }
}
