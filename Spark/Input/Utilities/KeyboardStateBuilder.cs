namespace Spark.Input.Utilities
{
    using System;

    /// <summary>
    /// Builder helper for constructing keyboard states.
    /// </summary>
    public sealed class KeyboardStateBuilder
    {
        private static uint _maskValue = uint.MaxValue;

        private uint _keyState0;
        private uint _keyState1;
        private uint _keyState2;
        private uint _keyState3;
        private uint _keyState4;
        private uint _keyState5;
        private uint _keyState6;
        private uint _keyState7;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardStateBuilder"/> class.
        /// </summary>
        public KeyboardStateBuilder()
        {
            Clear();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardStateBuilder"/> class.
        /// </summary>
        /// <param name="state">Initial keyboard state to populate from.</param>
        public KeyboardStateBuilder(KeyboardState state)
        {
            state.GetKeyStates(out _keyState0, 
                               out _keyState1, 
                               out _keyState2, 
                               out _keyState3, 
                               out _keyState4, 
                               out _keyState5, 
                               out _keyState6, 
                               out _keyState7);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardStateBuilder"/> class.
        /// </summary>
        /// <param name="from">Other keyboard state builder to populate from.</param>
        public KeyboardStateBuilder(KeyboardStateBuilder from)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            _keyState0 = from._keyState0;
            _keyState1 = from._keyState1;
            _keyState2 = from._keyState2;
            _keyState3 = from._keyState3;
            _keyState4 = from._keyState4;
            _keyState5 = from._keyState5;
            _keyState6 = from._keyState6;
            _keyState7 = from._keyState7;
        }

        /// <summary>
        /// Initializes a new <see cref="KeyboardState"/> from the state the builder is maintaining.
        /// </summary>
        /// <returns>Keyboard state</returns>
        public KeyboardState ConstructState()
        {
            return new KeyboardState(_keyState0, 
                                     _keyState1, 
                                     _keyState2, 
                                     _keyState3, 
                                     _keyState4, 
                                     _keyState5, 
                                     _keyState6, 
                                     _keyState7);
        }

        /// <summary>
        /// Initializes a new <see cref="KeyboardState"/> from the state the builder is maintaining.
        /// </summary>
        /// <param name="state">Keyboard state</param>
        public void ConstructState(out KeyboardState state)
        {
            state = new KeyboardState(_keyState0, 
                                      _keyState1, 
                                      _keyState2, 
                                      _keyState3, 
                                      _keyState4, 
                                      _keyState5, 
                                      _keyState6, 
                                      _keyState7);
        }

        /// <summary>
        /// Clears the keyboard state.
        /// </summary>
        public void Clear()
        {
            _keyState0 = 0;
            _keyState1 = 0;
            _keyState2 = 0;
            _keyState3 = 0;
            _keyState4 = 0;
            _keyState5 = 0;
            _keyState6 = 0;
            _keyState7 = 0;
        }

        /// <summary>
        /// Sets the current keystate for the specified key.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="state">Key state.</param>
        public void SetKeyState(Keys key, KeyState state)
        {
            if (state == KeyState.Up)
            {
                RemovePressedKey(key);
            }
            else
            {
                AddPressedKey(key);
            }
        }

        /// <summary>
        /// Adds the number of pressed keys to the builder's pressed state.
        /// </summary>
        /// <param name="keys">Keys to add as currently pressed.</param>
        public void AddPressedKeys(params Keys[] keys)
        {
            if (keys == null)
            {
                return;
            }

            foreach (Keys key in keys)
            {
                AddPressedKey(key);
            }
        }

        /// <summary>
        /// Adds the key to the builder's pressed state.
        /// </summary>
        /// <param name="key">Key to add as currently pressed.</param>
        public void AddPressedKey(Keys key)
        {
            uint place = ((uint)1) << (int)key;
            int grouping = (int)key >> 5;
            switch (grouping)
            {
                case 0:
                    _keyState0 |= place & _maskValue;
                    return;
                case 1:
                    _keyState1 |= place & _maskValue;
                    return;
                case 2:
                    _keyState2 |= place & _maskValue;
                    return;
                case 3:
                    _keyState3 |= place & _maskValue;
                    return;
                case 4:
                    _keyState4 |= place & _maskValue;
                    return;
                case 5:
                    _keyState5 |= place & _maskValue;
                    return;
                case 6:
                    _keyState6 |= place & _maskValue;
                    return;
                case 7:
                    _keyState7 |= place & _maskValue;
                    return;
            }
        }

        /// <summary>
        /// Removes the number of pressed keys from the builder's pressed state.
        /// </summary>
        /// <param name="keys">Keys to remove as currently pressed.</param>
        public void RemovePressedKeys(params Keys[] keys)
        {
            if (keys == null)
            {
                return;
            }

            foreach (Keys key in keys)
            {
                RemovePressedKey(key);
            }
        }

        /// <summary>
        /// Removes the key from the builder's pressed state.
        /// </summary>
        /// <param name="key">Key to remove as currently pressed.</param>
        public void RemovePressedKey(Keys key)
        {
            uint place = ((uint)1) << (int)key;
            int grouping = (int)key >> 5;
            switch (grouping)
            {
                case 0:
                    _keyState0 &= ~(place & _maskValue);
                    return;
                case 1:
                    _keyState1 &= ~(place & _maskValue);
                    return;
                case 2:
                    _keyState2 &= ~(place & _maskValue);
                    return;
                case 3:
                    _keyState3 &= ~(place & _maskValue);
                    return;
                case 4:
                    _keyState4 &= ~(place & _maskValue);
                    return;
                case 5:
                    _keyState5 &= ~(place & _maskValue);
                    return;
                case 6:
                    _keyState6 &= ~(place & _maskValue);
                    return;
                case 7:
                    _keyState7 &= ~(place & _maskValue);
                    return;
            }
        }
    }
}
