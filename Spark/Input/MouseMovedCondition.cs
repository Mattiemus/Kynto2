namespace Spark.Input
{
    using Core;
    using Math;
    
    /// <summary>
    /// Checks if the mouse has moved since the last update.
    /// </summary>
    public sealed class MouseMovedCondition : InputCondition
    {
        private Int2 m_pos;

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
            m_pos = Mouse.GetMouseState().PositionInt;
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

            bool hasMoved = (m_pos.X != currState.X && Direction != MoveDirection.YOnly) || (m_pos.Y != currState.Y && Direction != MoveDirection.XOnly);
            m_pos.X = currState.X;
            m_pos.Y = currState.Y;

            return hasMoved;
        }
    }
}
