namespace Spark.Input.Utilities
{
    using System;

    using Math;

    /// <summary>
    /// Builder helper for constructing mouse states.
    /// </summary>
    public sealed class MouseStateBuilder
    {
        private static int _numberOfButtons = Enum.GetValues(typeof(MouseButton)).Length;

        private readonly ButtonState[] _pressedButtons;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseStateBuilder"/> class.
        /// </summary>
        public MouseStateBuilder()
        {
            _pressedButtons = new ButtonState[_numberOfButtons];
            WheelValue = 0;
            CursorPosition = Int2.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseStateBuilder"/> class.
        /// </summary>
        /// <param name="state">Initial mouse state to populate from.</param>
        public MouseStateBuilder(MouseState state)
        {
            _pressedButtons = new ButtonState[_numberOfButtons];
            SetButtonState(MouseButton.Left, state.LeftButton);
            SetButtonState(MouseButton.Middle, state.MiddleButton);
            SetButtonState(MouseButton.Right, state.RightButton);
            SetButtonState(MouseButton.XButton1, state.XButton1);
            SetButtonState(MouseButton.XButton2, state.XButton2);

            WheelValue = state.ScrollWheelValue;
            CursorPosition = state.PositionInt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseStateBuilder"/> class.
        /// </summary>
        /// <param name="from">Other mouse state builder to populate from.</param>
        public MouseStateBuilder(MouseStateBuilder from)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            _pressedButtons = from._pressedButtons.Clone() as ButtonState[];
            WheelValue = from.WheelValue;
            CursorPosition = from.CursorPosition;
        }

        /// <summary>
        /// Gets or sets the current cursor position.
        /// </summary>
        public Int2 CursorPosition { get; set; }

        /// <summary>
        /// Gets or sets the current mouse wheel value.
        /// </summary>
        public int WheelValue { get; set; }

        /// <summary>
        /// Constructs a new <see cref="MouseState"/> from the state the builder is maintaining.
        /// </summary>
        /// <returns>Mouse state</returns>
        public MouseState ConstructState()
        {
            return new MouseState(CursorPosition.X, CursorPosition.Y, WheelValue, 
                _pressedButtons[(int)MouseButton.Left],
                _pressedButtons[(int)MouseButton.Right], 
                _pressedButtons[(int)MouseButton.Middle], 
                _pressedButtons[(int)MouseButton.XButton1],
                _pressedButtons[(int)MouseButton.XButton2]);
        }

        /// <summary>
        /// Constructs a new <see cref="MouseState"/> from the state the builder is maintaining.
        /// </summary>
        /// <param name="state">Mouse state</param>
        public void ConstructState(out MouseState state)
        {
            state = new MouseState(CursorPosition.X, CursorPosition.Y, WheelValue, 
                _pressedButtons[(int)MouseButton.Left],
                _pressedButtons[(int)MouseButton.Right], 
                _pressedButtons[(int)MouseButton.Middle], 
                _pressedButtons[(int)MouseButton.XButton1],
                _pressedButtons[(int)MouseButton.XButton2]);
        }

        /// <summary>
        /// Clears the mouse state.
        /// </summary>
        public void Clear()
        {
            CursorPosition = Int2.Zero;
            WheelValue = 0;
            Array.Clear(_pressedButtons, 0, _pressedButtons.Length);
        }

        /// <summary>
        /// Sets the current button state for the specified mouse button.
        /// </summary>
        /// <param name="mouseButton">Mouse button to set.</param>
        /// <param name="state">Button state.</param>
        public void SetButtonState(MouseButton mouseButton, ButtonState state)
        {
            _pressedButtons[(int)mouseButton] = state;
        }

        /// <summary>
        /// Adds the number of pressed buttons to the builder's pressed state.
        /// </summary>
        /// <param name="mouseButtons">Mouse buttons to add as currently pressed.</param>
        public void AddPressedButtons(params MouseButton[] mouseButtons)
        {
            if (mouseButtons == null)
            {
                return;
            }

            foreach (MouseButton button in mouseButtons)
            {
                AddPressedButton(button);
            }
        }

        /// <summary>
        /// Adds the button to the builder's pressed state.
        /// </summary>
        /// <param name="mouseButton">Mouse button to add as currently pressed.</param>
        public void AddPressedButton(MouseButton mouseButton)
        {
            _pressedButtons[(int)mouseButton] = ButtonState.Pressed;
        }

        /// <summary>
        /// Removes the number of pressed buttons from the builder's pressed state.
        /// </summary>
        /// <param name="mouseButtons">Mouse buttons to remove as currently pressed.</param>
        public void RemovePressedButtons(params MouseButton[] mouseButtons)
        {
            if (mouseButtons == null)
            {
                return;
            }

            foreach (MouseButton button in mouseButtons)
            {
                RemovePressedButton(button);
            }
        }

        /// <summary>
        /// Removes the button from the builder's pressed state.
        /// </summary>
        /// <param name="mouseButton">Mouse button to remove as currently pressed.</param>
        public void RemovePressedButton(MouseButton mouseButton)
        {
            _pressedButtons[(int)mouseButton] = ButtonState.Released;
        }
    }
}
