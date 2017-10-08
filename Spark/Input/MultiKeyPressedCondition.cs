namespace Spark.Input
{
    using System.Collections.Generic;

    using Core;

    /// <summary>
    /// Checks if a series of input bindings have been pressed since the last update. It can also account for key repeats,
    /// where the keys or buttons are continually pressed between multiple updates.
    /// </summary>
    public sealed class MultiKeyPressedCondition : InputCondition, IMultiKeyInputBinding
    {
        private bool _hasKey;
        private bool _hasMouse;
        private bool _allPressedPreviously;
        private readonly List<KeyOrMouseButton> _bindings;
        private readonly List<bool> _keysThatAreDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyPressedCondition"/> class.
        /// </summary>
        /// <param name="allowRepeat">Allow for repeating</param>
        /// <param name="bindings">Input bindings.</param>
        public MultiKeyPressedCondition(bool allowRepeat, params KeyOrMouseButton[] bindings)
        {
            _bindings = new List<KeyOrMouseButton>(bindings.Length);
            _keysThatAreDown = new List<bool>(bindings.Length);
            AllowRepeat = allowRepeat;
            _allPressedPreviously = false;

            SetInputBindings(bindings);
        }

        /// <summary>
        /// Gets the input bindings.
        /// </summary>
        public IReadOnlyList<KeyOrMouseButton> InputBindings => _bindings;

        /// <summary>
        /// Gets or sets if the condition should allow key repeats or not.
        /// </summary>
        public bool AllowRepeat { get; set; }

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

            return EvaluatePressedCondition();
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
        /// Evalutes the input condition, checking that all keys are pressed
        /// </summary>
        /// <returns>True if all keys are pressed, false otherwise</returns>
        private bool EvaluatePressedCondition()
        {
            bool areAllKeysDown = AreAllKeysDown();
            if (_allPressedPreviously)
            {
                // If there was a previous state where all keys were down and if they're down now, then the condition evaluates to true only if repeats are allowed
                if (areAllKeysDown)
                {
                    return AllowRepeat;
                }
                else
                {
                    // If not all keys are pressed, condition is false but reset the previous state
                    _allPressedPreviously = false;
                    return false;
                }
            }
            else
            {
                // If there wasn't a previous state where all keys were down and they are down now, then the condition evaluates to true
                if (areAllKeysDown)
                {
                    _allPressedPreviously = true;
                    return true;
                }
            }

            return false;
        }
    }
}
