namespace Spark.Input
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using Math;

    /// <summary>
    /// Represents the state of the mouse, e.g. where the cursor is located on the screen (client coordinates), and what buttons are pressed.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseState : IEquatable<MouseState>
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="MouseState"/> struct.
        /// </summary>
        /// <param name="x">The x (horizontal) position of the mouse. Typically these are client coordinates.</param>
        /// <param name="y">The y (verticaly) position of the mouse. Typically these are client coordinates.</param>
        /// <param name="scrollWheel">The scroll wheel value.</param>
        /// <param name="leftButton">The left button state.</param>
        /// <param name="rightButton">The right button state.</param>
        /// <param name="middleButton">The middle button state.</param>
        /// <param name="xButton1">The XButton1 state.</param>
        /// <param name="xButton2">The XButton2 state.</param>
        public MouseState(int x, int y, int scrollWheel, ButtonState leftButton, ButtonState rightButton,
            ButtonState middleButton, ButtonState xButton1, ButtonState xButton2)
        {
            X = x;
            Y = y;
            ScrollWheelValue = scrollWheel;
            LeftButton = leftButton;
            RightButton = rightButton;
            MiddleButton = middleButton;
            XButton1 = xButton1;
            XButton2 = xButton2;
        }

        /// <summary>
        /// Gets the position of the mouse as a float vector.
        /// </summary>
        public Vector2 Position => new Vector2(X, Y);

        /// <summary>
        /// Gets the position of the mouse as an int vector.
        /// </summary>
        public Int2 PositionInt => new Int2(X, Y);

        /// <summary>
        /// Gets the x (horizontal) coordinate of the mouse position.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the y (vertical) coordinate of the mouse position.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the mouse scroll wheel value.
        /// </summary>
        public int ScrollWheelValue { get; }

        /// <summary>
        /// Gets the state of the left mouse button.
        /// </summary>
        public ButtonState LeftButton { get; }

        /// <summary>
        /// Gets the state of the right mouse button.
        /// </summary>
        public ButtonState RightButton { get; }

        /// <summary>
        /// Gets the state of the middle mouse button.
        /// </summary>
        public ButtonState MiddleButton { get; }

        /// <summary>
        /// Gets the state of the XButton1.
        /// </summary>
        public ButtonState XButton1 { get; }

        /// <summary>
        /// Gets the state of the XButton2.
        /// </summary>
        public ButtonState XButton2 { get; }

        /// <summary>
        /// Query the button state for the specified mouse button.
        /// </summary>
        /// <param name="button">Specified mouse button</param>
        /// <returns>Button state</returns>
        public ButtonState this[MouseButton button]
        {
            get
            {
                switch (button)
                {
                    case MouseButton.Left:
                        return LeftButton;
                    case MouseButton.Middle:
                        return MiddleButton;
                    case MouseButton.Right:
                        return RightButton;
                    case MouseButton.XButton1:
                        return XButton1;
                    case MouseButton.XButton2:
                        return XButton2;
                    default:
                        return ButtonState.Released;
                }
            }
        }

        /// <summary>
        /// Gets the number of buttons that are pressed
        /// </summary>
        public int PressedButtonCount
        {
            get
            {
                int count = 0;
                if (LeftButton == ButtonState.Pressed)
                {
                    count++;
                }

                if (RightButton == ButtonState.Pressed)
                {
                    count++;
                }

                if (MiddleButton == ButtonState.Pressed)
                {
                    count++;
                }

                if (XButton1 == ButtonState.Pressed)
                {
                    count++;
                }

                if (XButton2 == ButtonState.Pressed)
                {
                    count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Queries if any button is currently pressed.
        /// </summary>
        /// <returns>True if any button is pressed, false otherwise.</returns>
        public bool IsAnyButtonPressed()
        {
            return LeftButton == ButtonState.Pressed || 
                   RightButton == ButtonState.Pressed || 
                   MiddleButton == ButtonState.Pressed || 
                   XButton1 == ButtonState.Pressed || 
                   XButton2 == ButtonState.Pressed;
        }

        /// <summary>
        /// Query if the specified button is pressed.
        /// </summary>
        /// <param name="button">Mousebutton to query</param>
        /// <returns>True if the button is pressed, false otherwise.</returns>
        public bool IsButtonPressed(MouseButton button)
        {
            return this[button] == ButtonState.Pressed;
        }

        /// <summary>
        /// Query if the specified button is released.
        /// </summary>
        /// <param name="button">Mousebutton to query</param>
        /// <returns>True if the button is released, false otherwise.</returns>
        public bool IsButtonReleased(MouseButton button)
        {
            return this[button] == ButtonState.Released;
        }

        /// <summary>
        /// Gets all the pressed buttons in the current state.
        /// </summary>
        /// <param name="pressedKeys">List to add pressed buttons to.</param>
        public void GetPressedButtons(IList<MouseButton> pressedButtons)
        {
            if (pressedButtons == null)
            {
                return;
            }

            if (LeftButton == ButtonState.Pressed)
            {
                pressedButtons.Add(MouseButton.Left);
            }

            if (RightButton == ButtonState.Pressed)
            {
                pressedButtons.Add(MouseButton.Right);
            }

            if (MiddleButton == ButtonState.Pressed)
            {
                pressedButtons.Add(MouseButton.Middle);
            }

            if (XButton1 == ButtonState.Pressed)
            {
                pressedButtons.Add(MouseButton.XButton1);
            }

            if (XButton2 == ButtonState.Pressed)
            {
                pressedButtons.Add(MouseButton.XButton2);
            }
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

                hash = (hash * 31) + X.GetHashCode();
                hash = (hash * 31) + Y.GetHashCode();
                hash = (hash * 31) + ScrollWheelValue.GetHashCode();
                hash = (hash * 31) + LeftButton.GetHashCode();
                hash = (hash * 31) + MiddleButton.GetHashCode();
                hash = (hash * 31) + RightButton.GetHashCode();
                hash = (hash * 31) + XButton1.GetHashCode();
                hash = (hash * 31) + XButton2.GetHashCode();
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
            if (obj is MouseState)
            {
                return Equals((MouseState)obj);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(MouseState other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ref MouseState other)
        {
            return (X == other.X) && 
                   (Y == other.Y) && 
                   (ScrollWheelValue == other.ScrollWheelValue) && 
                   (LeftButton == other.LeftButton) && 
                   (MiddleButton == other.MiddleButton) && 
                   (RightButton == other.RightButton) && 
                   (XButton1 == other.XButton1) && 
                   (XButton2 == other.XButton2);
        }

        /// <summary>
        /// Tests if two mouse states are equal.
        /// </summary>
        /// <param name="a">First mouse state</param>
        /// <param name="b">Second mouse state</param>
        /// <returns>True if equal, false otherwise</returns>
        public static bool operator ==(MouseState a, MouseState b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests if two mouse states are not equal.
        /// </summary>
        /// <param name="a">First mouse state</param>
        /// <param name="b">Second mouse state</param>
        /// <returns>True if not equal, false otherwise</returns>
        public static bool operator !=(MouseState a, MouseState b)
        {
            return !a.Equals(ref b);
        }
    }
}
