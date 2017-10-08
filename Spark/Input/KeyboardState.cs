namespace Spark.Input
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the state of the keyboard, e.g. which keys are pressed.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardState : IEquatable<KeyboardState>
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
        /// Initializes a new instance of the <see cref="KeyboardState"/> struct with the specified keys
        /// pressed.
        /// </summary>
        /// <param name="keys">Keys that are pressed</param>
        public KeyboardState(params Keys[] keys)
        {
            _keyState0 = 0;
            _keyState1 = 0;
            _keyState2 = 0;
            _keyState3 = 0;
            _keyState4 = 0;
            _keyState5 = 0;
            _keyState6 = 0;
            _keyState7 = 0;

            if (keys != null)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    AddPressedKey(keys[i]);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardState"/> struct with the specified key group masks. Each key is a bit in each mask (32 bits x 8 = 256 keys)
        /// </summary>
        /// <param name="group0">First group</param>
        /// <param name="group1">Second group</param>
        /// <param name="group2">Third group</param>
        /// <param name="group3">Fourth group</param>
        /// <param name="group4">Fifth group</param>
        /// <param name="group5">Sixth group</param>
        /// <param name="group6">Seventh group</param>
        /// <param name="group7">Eighth group</param>
        internal KeyboardState(uint group0, uint group1, uint group2, uint group3, uint group4, uint group5, uint group6, uint group7)
        {
            _keyState0 = group0;
            _keyState1 = group1;
            _keyState2 = group2;
            _keyState3 = group3;
            _keyState4 = group4;
            _keyState5 = group5;
            _keyState6 = group6;
            _keyState7 = group7;
        }

        /// <summary>
        /// Gets the internal group key state masks.
        /// </summary>
        /// <param name="group0">First group</param>
        /// <param name="group1">Second group</param>
        /// <param name="group2">Third group</param>
        /// <param name="group3">Fourth group</param>
        /// <param name="group4">Fifth group</param>
        /// <param name="group5">Sixth group</param>
        /// <param name="group6">Seventh group</param>
        /// <param name="group7">Eighth group</param>
        internal void GetKeyStates(out uint group0, out uint group1, out uint group2, out uint group3, out uint group4, out uint group5, out uint group6, out uint group7)
        {
            group0 = _keyState0;
            group1 = _keyState1;
            group2 = _keyState2;
            group3 = _keyState3;
            group4 = _keyState4;
            group5 = _keyState5;
            group6 = _keyState6;
            group7 = _keyState7;
        }

        /// <summary>
        /// Query the keystate for the specified key.
        /// </summary>
        /// <param name="key">Specified key</param>
        /// <returns>The keystate</returns>
        public KeyState this[Keys key]
        {
            get
            {
                uint groupState = 0;
                int grouping = ((int)key) >> 5;
                switch (grouping)
                {
                    case 0:
                        groupState = _keyState0;
                        break;
                    case 1:
                        groupState = _keyState1;
                        break;
                    case 2:
                        groupState = _keyState2;
                        break;
                    case 3:
                        groupState = _keyState3;
                        break;
                    case 4:
                        groupState = _keyState4;
                        break;
                    case 5:
                        groupState = _keyState5;
                        break;
                    case 6:
                        groupState = _keyState6;
                        break;
                    case 7:
                        groupState = _keyState7;
                        break;
                }

                uint keyState = ((uint)1) << (int)key;
                if ((groupState & keyState) == 0)
                {
                    return KeyState.Up;
                }
                else
                {
                    return KeyState.Down;
                }
            }
        }

        /// <summary>
        /// Queries if multiple keys are currently pressed down.
        /// </summary>
        /// <returns>True if two or more keys are currently pressed, false otherwise.</returns>
        public bool AreMultipleKeysDown()
        {
            int count = 0;

            count += CheckAtMostTwoKeys(_keyState0);
            if (count >= 2)
            {
                return true;
            }

            count += CheckAtMostTwoKeys(_keyState1);
            if (count >= 2)
            {
                return true;
            }

            count += CheckAtMostTwoKeys(_keyState2);
            if (count >= 2)
            {
                return true;
            }

            count += CheckAtMostTwoKeys(_keyState3);
            if (count >= 2)
            {
                return true;
            }

            count += CheckAtMostTwoKeys(_keyState4);
            if (count >= 2)
            {
                return true;
            }

            count += CheckAtMostTwoKeys(_keyState5);
            if (count >= 2)
            {
                return true;
            }

            count += CheckAtMostTwoKeys(_keyState6);
            if (count >= 2)
            {
                return true;
            }

            count += CheckAtMostTwoKeys(_keyState7);

            return count >= 2;
        }

        /// <summary>
        /// Queries if any key is currently down.
        /// </summary>
        /// <returns>True if any key is pressed, false otherwise.</returns>
        public bool IsAnyKeyDown()
        {
            return _keyState0 != 0 || 
                   _keyState1 != 0 || 
                   _keyState2 != 0 || 
                   _keyState3 != 0 || 
                   _keyState4 != 0 || 
                   _keyState5 != 0 || 
                   _keyState6 != 0 || 
                   _keyState7 != 0;
        }

        /// <summary>
        /// Query if the specified key is down.
        /// </summary>
        /// <param name="key">Key to query</param>
        /// <returns>True if the key is pressed, false otherwise</returns>
        public bool IsKeyDown(Keys key)
        {
            return this[key] == KeyState.Down;
        }

        /// <summary>
        /// Query if the specified key is up.
        /// </summary>
        /// <param name="key">Key to query</param>
        /// <returns>True if the key is not pressed, false otherwise</returns>
        public bool IsKeyUp(Keys key)
        {
            return this[key] == KeyState.Up;
        }

        /// <summary>
        /// Adds a pressed key to the state
        /// </summary>
        /// <param name="key">Key to add</param>
        private void AddPressedKey(Keys key)
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
        /// Gets all the pressed keys in the current state.
        /// </summary>
        /// <param name="pressedKeys">List to add pressed keys to.</param>
        public void GetPressedKeys(IList<Keys> pressedKeys)
        {
            if (pressedKeys == null)
            {
                return;
            }

            CheckPressedKeys(_keyState0, 0, pressedKeys);
            CheckPressedKeys(_keyState1, 1, pressedKeys);
            CheckPressedKeys(_keyState2, 2, pressedKeys);
            CheckPressedKeys(_keyState3, 3, pressedKeys);
            CheckPressedKeys(_keyState4, 4, pressedKeys);
            CheckPressedKeys(_keyState5, 5, pressedKeys);
            CheckPressedKeys(_keyState6, 6, pressedKeys);
            CheckPressedKeys(_keyState7, 7, pressedKeys);
        }

        /// <summary>
        /// Gets the list of pressed keys
        /// </summary>
        /// <param name="keyState">Key state bitmap</param>
        /// <param name="grouping">key grouping</param>
        /// <param name="pressedKeys">List of pressed keys</param>
        private void CheckPressedKeys(uint keyState, int grouping, IList<Keys> pressedKeys)
        {
            // If the state is zero, we have nothing pressed
            if (keyState != 0)
            {
                // For each key, check to see if its pressed
                for (int i = 0; i < 32; i++)
                {
                    // If its pressed, get the keycode
                    if ((keyState & (((uint)1) << i)) != 0)
                    {
                        pressedKeys.Add((Keys)((grouping * 32) + i));
                    }
                }
            }
        }

        /// <summary>
        /// Checks that at most two keys are pressed in a given key state
        /// </summary>
        /// <param name="keyState">Key state map</param>
        /// <returns>Number of keys that are pressed, up to a maximum of 2</returns>
        private int CheckAtMostTwoKeys(uint keyState)
        {
            int count = 0;
            if (keyState != 0)
            {
                for (int i = 0; i < 32; i++)
                {
                    if ((keyState & (((uint)1) << i)) != 0)
                    {
                        count++;
                    }

                    if (count >= 2)
                    {
                        return 2;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = (hash * 31) + _keyState0.GetHashCode();
                hash = (hash * 31) + _keyState1.GetHashCode();
                hash = (hash * 31) + _keyState2.GetHashCode();
                hash = (hash * 31) + _keyState3.GetHashCode();
                hash = (hash * 31) + _keyState4.GetHashCode();
                hash = (hash * 31) + _keyState5.GetHashCode();
                hash = (hash * 31) + _keyState6.GetHashCode();
                hash = (hash * 31) + _keyState7.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is KeyboardState)
            {
                return Equals((KeyboardState)obj);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(KeyboardState other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ref KeyboardState other)
        {
            return (_keyState0 == other._keyState0) && 
                   (_keyState1 == other._keyState1) && 
                   (_keyState2 == other._keyState2) && 
                   (_keyState3 == other._keyState3) && 
                   (_keyState4 == other._keyState4) &&
                   (_keyState5 == other._keyState5) && 
                   (_keyState6 == other._keyState6) && 
                   (_keyState7 == other._keyState7);
        }

        /// <summary>
        /// Tests if two KeyboardStates are equal
        /// </summary>
        /// <param name="a">First KeyboardState</param>
        /// <param name="b">Second KeyboardState</param>
        /// <returns>True if equal, false otherwise</returns>
        public static bool operator ==(KeyboardState a, KeyboardState b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests if two KeyboardStates are not equal
        /// </summary>
        /// <param name="a">First KeyboardState</param>
        /// <param name="b">Second KeyboardState</param>
        /// <returns>True if not equal, false otherwise</returns>
        public static bool operator !=(KeyboardState a, KeyboardState b)
        {
            return !a.Equals(ref b);
        }
    }
}
