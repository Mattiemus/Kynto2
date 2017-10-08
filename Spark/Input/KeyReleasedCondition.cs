namespace Spark.Input
{
    using Core;

    /// <summary>
    /// Checks if a key has been released since the last update, that is, moved from a down to up state.
    /// </summary>
    public sealed class KeyReleasedCondition : InputCondition, IKeyInputBinding
    {
        private KeyOrMouseButton _binding;
        private KeyboardState _oldKeyState;
        private MouseState _oldMouseState;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyReleasedCondition"/> class.
        /// </summary>
        /// <param name="binding">Input binding.</param>
        public KeyReleasedCondition(KeyOrMouseButton binding) 
            : this(binding, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyReleasedCondition"/> class.
        /// </summary>
        /// <param name="binding">Input binding.</param>
        /// <param name="allowMultipleButtons">Allow if the condition should evaluate to true even if multiple buttons are currently pressed, false if the condition
        /// should evaluate true as long as other buttons are not held down when the specified button is released.</param>
        public KeyReleasedCondition(KeyOrMouseButton binding, bool allowMultipleButtons)
        {
            _binding = binding;
            AllowMultipleButtons = allowMultipleButtons;

            Reset();
        }

        /// <summary>
        /// Gets or sets the input binding.
        /// </summary>
        public KeyOrMouseButton InputBinding
        {
            get => _binding;
            set
            {
                _binding = value;
                Reset();
            }
        }

        /// <summary>
        /// Gets or sets if the condition can be true even if the previous state had multiple key presses. If true, then as long as the specified button is released
        /// then the condition will evaluate as true, if false then the condition will only be true if the specified button is released and the previous state had no other key press.
        /// </summary>
        public bool AllowMultipleButtons { get; set; }

        /// <summary>
        /// Resets the condition's state.
        /// </summary>
        public void Reset()
        {
            if (_binding.IsMouseButton)
            {
                _oldKeyState = new KeyboardState();
                _oldMouseState = Mouse.GetMouseState();
            }
            else
            {
                _oldKeyState = Keyboard.GetKeyboardState();
                _oldMouseState = new MouseState();
            }
        }

        /// <summary>
        /// Checks if the condition has been satisfied or not.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        /// <returns>True if the condition has been satisfied, false otherwise.</returns>
        public override bool Check(IGameTime time)
        {
            bool hasChanged = false;

            if (_binding.IsMouseButton)
            {
                MouseState currState = Mouse.GetMouseState();
                MouseButton button = _binding.MouseButton;

                bool ignoreIfOtherKeysPressed = !AllowMultipleButtons && _oldMouseState.PressedButtonCount > 1;
                if (!ignoreIfOtherKeysPressed && _oldMouseState.IsButtonPressed(button) && currState.IsButtonReleased(button))
                {
                    hasChanged = true;
                }

                _oldMouseState = currState;
            }
            else
            {
                KeyboardState currState = Keyboard.GetKeyboardState();
                Keys key = _binding.Key;

                bool ignoreIfOtherKeysPressed = !AllowMultipleButtons && _oldKeyState.AreMultipleKeysDown();
                if (!ignoreIfOtherKeysPressed && _oldKeyState.IsKeyDown(key) && currState.IsKeyUp(key))
                {
                    hasChanged = true;
                }

                _oldKeyState = currState;
            }

            return hasChanged;
        }
    }
}
