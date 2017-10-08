namespace Spark.Input
{
    using Core;

    /// <summary>
    /// Checks if a key has been pressed since the last update, that is, moved from an up to down state. It can also account for
    /// key repeats, where the key is continually down between multiple updates.
    /// </summary>
    public sealed class KeyPressedCondition : InputCondition, IKeyInputBinding
    {
        private KeyOrMouseButton _binding;
        private KeyboardState _oldKeyState;
        private MouseState _oldMouseState;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPressedCondition"/> class.
        /// </summary>
        /// <param name="binding">Input binding.</param>
        /// <param name="allowRepeat">Allow for repeating</param>
        public KeyPressedCondition(KeyOrMouseButton binding, bool allowRepeat) 
            : this(binding, allowRepeat, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPressedCondition"/> class.
        /// </summary>
        /// <param name="binding">Input binding.</param>
        /// <param name="allowRepeat">Allow for repeating</param>
        /// <param name="allowMultipleButtons">Allow if the condition should evaluate to true even if multiple buttons are currently pressed, false if the condition
        /// should evaluate true as long as the specified button is pressed.</param>
        public KeyPressedCondition(KeyOrMouseButton binding, bool allowRepeat, bool allowMultipleButtons)
        {
            _binding = binding;
            AllowRepeat = allowRepeat;
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
        /// Gets or sets if the condition should allow key repeats or not.
        /// </summary>
        public bool AllowRepeat { get; set; }

        /// <summary>
        /// Gets or sets if the condition can be true even if there are multiple key presses. If true, then as long as the specified button is pressed
        /// then the condition will evaluate as true, if false then only the specified button pressed and no other will the condition be true.
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

                bool ignoreIfOtherKeysPressed = !AllowMultipleButtons && currState.PressedButtonCount > 1;
                if (AllowRepeat)
                {
                    if (!ignoreIfOtherKeysPressed && currState.IsButtonPressed(button))
                    {
                        hasChanged = true;
                    }
                }
                else
                {
                    if (!ignoreIfOtherKeysPressed && _oldMouseState.IsButtonReleased(button) && currState.IsButtonPressed(button))
                    {
                        hasChanged = true;
                    }
                }

                _oldMouseState = currState;
            }
            else
            {
                KeyboardState currState = Keyboard.GetKeyboardState();
                Keys key = _binding.Key;

                bool ignoreIfOtherKeysPressed = !AllowMultipleButtons && currState.AreMultipleKeysDown();
                if (AllowRepeat)
                {
                    if (!ignoreIfOtherKeysPressed && currState.IsKeyDown(key))
                    {
                        hasChanged = true;
                    }
                }
                else
                {
                    if (!ignoreIfOtherKeysPressed && _oldKeyState.IsKeyUp(key) && currState.IsKeyDown(key))
                    {
                        hasChanged = true;
                    }
                }

                _oldKeyState = currState;
            }

            return hasChanged;
        }
    }
}
