namespace Spark.Input
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Represents a button that can either be a keyboard key or a mouse button.
    /// </summary>
    public struct KeyOrMouseButton : IEquatable<KeyOrMouseButton>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyOrMouseButton"/> struct.
        /// </summary>
        /// <param name="key">Key.</param>
        public KeyOrMouseButton(Keys key)
        {
            Key = key;
            MouseButton = MouseButton.Left;
            IsMouseButton = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyOrMouseButton"/> struct.
        /// </summary>
        /// <param name="button">Mouse button.</param>
        public KeyOrMouseButton(MouseButton button)
        {
            Key = Keys.A;
            MouseButton = button;
            IsMouseButton = true;
        }

        /// <summary>
        /// gets the key.
        /// </summary>
        public Keys Key { get; }

        /// <summary>
        /// Gets the mouse button.
        /// </summary>
        public MouseButton MouseButton { get; }

        /// <summary>
        /// Gets if the button is a <see cref="MouseButton"/>, if false then
        /// it is a <see cref="Key"/>.
        /// </summary>
        public bool IsMouseButton { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Keys"/> to <see cref="KeyOrMouseButton"/>.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator KeyOrMouseButton(Keys key)
        {
            return new KeyOrMouseButton(key);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MouseButton"/> to <see cref="KeyOrMouseButton"/>.
        /// </summary>
        /// <param name="button">Mouse button.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator KeyOrMouseButton(MouseButton button)
        {
            return new KeyOrMouseButton(button);
        }

        /// <summary>
        /// Queries if the set of bindings has at least one mouse binding.
        /// </summary>
        /// <param name="bindings">Mouse bindings.</param>
        /// <returns>True if the set has a mouse binding, false if otherwise.</returns>
        public bool HasMouse(params KeyOrMouseButton[] bindings)
        {
            if (bindings == null || bindings.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < bindings.Length; i++)
            {
                if (bindings[i].IsMouseButton)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a default input condition based on the input binding.
        /// </summary>
        /// <param name="binding">Key or mouse binding.</param>
        /// <param name="onRelease">True if the condition should be on released, false if on pressed.</param>
        /// <returns>Input condition.</returns>
        public static InputCondition CreateInputCondition(KeyOrMouseButton binding, bool onRelease)
        {
            return CreateInputCondition(binding, onRelease, false, true);
        }

        /// <summary>
        /// Creates a default input condition based on the input binding.
        /// </summary>
        /// <param name="binding">Key or mouse binding.</param>
        /// <param name="onRelease">True if the condition should be on released, false if on pressed.</param>
        /// <param name="allowRepeats">True if the condition should allow repeats, false otherwise. Only valid if the condition is on pressed.</param>
        /// <returns>Input condition.</returns>
        public static InputCondition CreateInputCondition(KeyOrMouseButton binding, bool onRelease, bool allowRepeats)
        {
            return CreateInputCondition(binding, onRelease, allowRepeats, true);
        }

        /// <summary>
        /// Creates a default input condition based on the input binding.
        /// </summary>
        /// <param name="binding">Key or mouse binding.</param>
        /// <param name="onRelease">True if the condition should be on released, false if on pressed.</param>
        /// <param name="allowRepeats">True if the condition should allow repeats, false otherwise. Only valid if the condition is on pressed.</param>
        /// <param name="allowMultipleButtons">True if the condition can evaluate true even if other buttons are pressed, false if the condition can only evaluate true
        /// if only the specified button is pressed.</param>
        /// <returns>Input condition.</returns>
        public static InputCondition CreateInputCondition(KeyOrMouseButton binding, bool onRelease, bool allowRepeats, bool allowMultipleButtons)
        {
            if (onRelease)
            {
                return new KeyReleasedCondition(binding, allowMultipleButtons);
            }
            else
            {
                return new KeyPressedCondition(binding, allowRepeats, allowMultipleButtons);
            }
        }

        /// <summary>
        /// Creates a default input condition based on the input binding.
        /// </summary>
        /// <param name="onRelease">True if the condition should be on released, false if on pressed.</param>
        /// <param name="allowRepeats">True if the condition should allow repeats, false otherwise. Only valid if the condition is on pressed.</param>
        /// <param name="bindings">Key or mouse binding.</param>
        /// <returns>Input condition.</returns>
        public static InputCondition CreateInputCondition(bool onRelease, bool allowRepeats, params KeyOrMouseButton[] bindings)
        {
            if (bindings == null || bindings.Length == 0)
            {
                return null;
            }

            if (bindings.Length == 1)
            {
                return CreateInputCondition(bindings[0], onRelease, allowRepeats);
            }

            if (onRelease)
            {
                return new MultiKeyReleasedCondition(bindings);
            }
            else
            {
                return new MultiKeyPressedCondition(allowRepeats, bindings);
            }
        }

        /// <summary>
        /// Creates an input condition based on a map of key bindings, if the entry exists, otherwise create a condition based on the default binding.
        /// </summary>
        /// <param name="keyBindings">Collection of input bindings.</param>
        /// <param name="bindingName">Name of the binding to search in the dictionary.</param>
        /// <param name="defaultKey">Default key or mouse binding.</param>
        /// <param name="onRelease">True if the condition should be on released, false if on pressed.</param>
        /// <param name="allowRepeats">True if the condition should allow repeats, false otherwise. Only valid if the condition is on pressed.</param>
        /// <returns>Input condition.</returns>
        public static InputCondition CreateInputCondition(IReadOnlyDictionary<String, KeyOrMouseButton[]> keyBindings, String bindingName, KeyOrMouseButton defaultKey, bool onRelease, bool allowRepeats)
        {
            if (keyBindings == null || String.IsNullOrEmpty(bindingName))
            {
                return CreateInputCondition(defaultKey, onRelease, allowRepeats);
            }
            
            if (!keyBindings.TryGetValue(bindingName, out KeyOrMouseButton[] binding))
            {
                return CreateInputCondition(defaultKey, onRelease, allowRepeats);
            }

            return CreateInputCondition(onRelease, allowRepeats, binding);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>True</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is KeyOrMouseButton)
            {
                return Equals((KeyOrMouseButton)obj);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(KeyOrMouseButton other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ref KeyOrMouseButton other)
        {
            if (IsMouseButton == other.IsMouseButton)
            {
                if (IsMouseButton)
                {
                    return MouseButton == other.MouseButton;
                }
                else
                {
                    return Key == other.Key;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (IsMouseButton) ? MouseButton.GetHashCode() : Key.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            if (IsMouseButton)
            {
                return string.Format(info, "MouseButton: {0}", MouseButton.ToString());
            }
            else
            {
                return string.Format(info, "Key: {0}", Key.ToString());
            }
        }
    }
}
