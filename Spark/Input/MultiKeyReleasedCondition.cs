namespace Spark.Input
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Checks if a series of input bindings have been released since the last update.
    /// </summary>
    public sealed class MultiKeyReleasedCondition : InputCondition, IMultiKeyInputBinding
    {
        private bool _hasKey;
        private bool _hasMouse;
        private bool _allPressedPreviously;
        private readonly List<KeyOrMouseButton> _bindings;
        private readonly List<bool> _keysThatAreDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyReleasedCondition"/> class.
        /// </summary>
        /// <param name="bindings">Input bindings.</param>
        public MultiKeyReleasedCondition(params KeyOrMouseButton[] bindings)
        {
            _bindings = new List<KeyOrMouseButton>(bindings.Length);
            _keysThatAreDown = new List<bool>(bindings.Length);
            _allPressedPreviously = false;

            SetInputBindings(bindings);
        }

        /// <summary>
        /// Gets the input bindings.
        /// </summary>
        public IReadOnlyList<KeyOrMouseButton> InputBindings => _bindings;

        /// <summary>
        /// Resets the condition's state.
        /// </summary>
        public void Reset()
        {
            _allPressedPreviously = false;
            for (int i = 0; i < _keysThatAreDown.Count; i++)
            {
                _keysThatAreDown[i] = false;
            }
        }

        /// <summary>
        /// Sets the input binding combination of the condition.
        /// </summary>
        /// <param name="bindings">Input bindings.</param>
        public void SetInputBindings(params KeyOrMouseButton[] bindings)
        {
            _bindings.Clear();
            _keysThatAreDown.Clear();
            _hasKey = false;
            _hasMouse = false;

            if (bindings == null)
            {
                return;
            }

            _bindings.AddRange(bindings);

            for (int i = 0; i < bindings.Length; i++)
            {
                _keysThatAreDown.Add(false);
                if (bindings[i].IsMouseButton)
                {
                    _hasMouse = true;
                }
                else
                {
                    _hasKey = true;
                }
            }

            Reset();
        }

        /// <summary>
        /// Checks if the condition has been satisfied or not.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        /// <returns>True if the condition has been satisfied, false otherwise.</returns>
        public override bool Check(IGameTime time)
        {
            KeyboardState currKeyState = (_hasKey) ? Keyboard.GetKeyboardState() : new KeyboardState();
            MouseState currMouseState = (_hasMouse) ? Mouse.GetMouseState() : new MouseState();

            GatherPressedKeys(ref currKeyState, ref currMouseState);

            return EvaluateReleasedCondition();
        }

        /// <summary>
        /// Collects all currently pressed keys
        /// </summary>
        /// <param name="keyState">Current keyboard state</param>
        /// <param name="mouseState">Current mouse state</param>
        private void GatherPressedKeys(ref KeyboardState keyState, ref MouseState mouseState)
        {
            for (int i = 0; i < _bindings.Count; i++)
            {
                KeyOrMouseButton binding = _bindings[i];
                if (binding.IsMouseButton)
                {
                    if (mouseState.IsButtonPressed(binding.MouseButton))
                    {
                        _keysThatAreDown[i] = true;
                    }
                    else
                    {
                        _keysThatAreDown[i] = false;
                    }
                }
                else
                {
                    if (keyState.IsKeyDown(binding.Key))
                    {
                        _keysThatAreDown[i] = true;
                    }
                    else
                    {
                        _keysThatAreDown[i] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if all watched keys are pressed
        /// </summary>
        /// <returns>True if all watched keys are pressed, false otherwise</returns>
        private bool AreAllKeysDown()
        {
            for (int i = 0; i < _keysThatAreDown.Count; i++)
            {
                if (!_keysThatAreDown[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if all watched keys are released
        /// </summary>
        /// <returns>True if all watched keys are released, false otherwise</returns>
        private bool AreAllKeysUp()
        {
            for (int i = 0; i < _keysThatAreDown.Count; i++)
            {
                if (_keysThatAreDown[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Evalutes the input condition, checking that all keys are released
        /// </summary>
        /// <returns>True if all keys are released, false otherwise</returns>
        private bool EvaluateReleasedCondition()
        {
            if (_allPressedPreviously)
            {
                // If all were pressed and we're releasing some, then evaluate to true and reset state
                if (AreAllKeysUp())
                {
                    _allPressedPreviously = false;
                    return true;
                }

                // If any keys are up, don't reset the state - else you'd have to time the release perfectly
            }
            else
            {
                bool areAllKeysDown = AreAllKeysDown();

                // If there wasn't a previous state where all keys were down, the condition evaluates to false but we take note that all keys are now down
                if (areAllKeysDown)
                {
                    _allPressedPreviously = true;
                }
            }

            return false;
        }
    }
}
