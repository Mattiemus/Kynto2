namespace Kynto.Input.Triggers
{
    using Spark.Core;
    using Spark.Input;

    /// <summary>
    /// Input action that applies a cursor at the start of the action and restores the previous cursor at the end.
    /// </summary>
    public abstract class CursorInputAction : InputAction
    {
        private string _cursor;
        private string _prevCursor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorInputAction"/> class.
        /// </summary>
        protected CursorInputAction()
        {
            _cursor = Cursors.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorInputAction"/> class.
        /// </summary>
        /// <param name="action">Input action delegate</param>
        protected CursorInputAction(InputActionDelegate action)
            : base(action)
        {
            _cursor = Cursors.Default;
        }

        /// <summary>
        /// Gets or sets the cursor name that will be applied.
        /// </summary>
        public string Cursor
        {
            get => _cursor;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _cursor = Cursors.Default;
                }
                else
                {
                    _cursor = value;
                }
            }
        }

        /// <summary>
        /// Called by an input trigger when the condition first succeeds, allowing the action to do first-time setup.
        /// </summary>
        /// <param name="time"></param>
        public override void OnBegin(IGameTime time)
        {
            SetCursor();
        }

        /// <summary>
        /// Called by an input trigger when the condition fails, allowing the action to do last-time cleanup.
        /// </summary>
        /// <param name="time"></param>
        public override void OnEnd(IGameTime time)
        {
            ResetCursor();
        }

        /// <summary>
        /// Applies the cursor.
        /// </summary>
        protected void SetCursor()
        {
            _prevCursor = Cursors.Cursor;
            Cursors.Cursor = _cursor;
        }

        /// <summary>
        /// Restores the previous cursor state.
        /// </summary>
        protected void ResetCursor()
        {
            Cursors.Cursor = _prevCursor;
        }
    }
}
