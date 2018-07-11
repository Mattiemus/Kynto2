using Spark.Input;

namespace Spark.UI.Input
{
    using System.Linq;
    using System.Collections.Generic;

    using Media;

    using SparkKeyboard = Spark.Input.Keyboard;

    public sealed class KeyboardDevice : InputDevice
    {
        private UIElement _target;
        private KeyboardState _keyboardState;

        public KeyboardDevice()
        {
            _keyboardState = SparkKeyboard.GetKeyboardState();

            InputManager.Current.PreProcessInput += PreProcessMouseInput;
        }

        public IInputElement FocusedElement => _target;

        public ModifierKeys Modifiers
        {
            get
            {
                ModifierKeys result = 0;

                if (_keyboardState[Keys.LeftAlt] == KeyState.Down ||
                    _keyboardState[Keys.RightAlt] == KeyState.Down)
                {
                    result |= ModifierKeys.Alt;
                }

                if (_keyboardState[Keys.LeftControl] == KeyState.Down ||
                    _keyboardState[Keys.RightControl] == KeyState.Down)
                {
                    result |= ModifierKeys.Control;
                }

                if (_keyboardState[Keys.LeftShift] == KeyState.Down ||
                    _keyboardState[Keys.RightShift] == KeyState.Down)
                {
                    result |= ModifierKeys.Shift;
                }

                if (_keyboardState[Keys.LeftWindows] == KeyState.Down ||
                    _keyboardState[Keys.RightWindows] == KeyState.Down)
                {
                    result |= ModifierKeys.Windows;
                }

                return result;
            }
        }

        public override IInputElement Target => _target ?? (IInputElement)ActiveSource.RootVisual;

        public IInputElement Focus(IInputElement element)
        {
            UIElement focusElement = FindFocusable(element);

            if (focusElement != null)
            {
                SetFocus(focusElement);
            }

            return _target;
        }

        public void Update()
        {
            KeyboardState oldState = _keyboardState;
            KeyboardState newState = SparkKeyboard.GetKeyboardState();
            
            List<Keys> oldKeys = new List<Keys>();
            oldState.GetPressedKeys(oldKeys);

            List<Keys> newKeys = new List<Keys>();
            newState.GetPressedKeys(newKeys);

            foreach (Keys key in newKeys.Except(oldKeys))
            {
                InputManager.Current.ProcessInput(new KeyEventArgs(this, key)
                {
                    RoutedEvent = UIElement.KeyDownEvent
                });
            }
                
            _keyboardState = newState;
        }

        private void PreProcessMouseInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.Input.Device == Mouse.PrimaryDevice)
            {
                if (e.Input.RoutedEvent == Mouse.MouseLeftButtonDownEvent)
                {
                    IInputElement element = e.Input.OriginalSource as IInputElement;

                    if (element != null)
                    {
                        Focus(element);
                    }
                }
            }
        }

        private UIElement FindFocusable(object o)
        {
            UIElement ui = o as UIElement;

            while (ui != null)
            {
                if (ui.IsVisible && ui.Focusable)
                {
                    return ui;
                }

                ui = VisualTreeHelper.GetAncestor<UIElement>(ui);
            }

            return null;
        }

        private void SetFocus(UIElement element)
        {
            if (element != _target)
            {
                if (_target != null)
                {
                    _target.SetValue(UIElement.IsKeyboardFocusedProperty, false);

                    KeyboardFocusChangedEventArgs e = new KeyboardFocusChangedEventArgs();
                    e.OriginalSource = e.Source = _target;
                    e.RoutedEvent = UIElement.LostKeyboardFocusEvent;
                    _target.RaiseEvent(e);
                }

                if (element != null)
                {
                    element.SetValue(UIElement.IsKeyboardFocusedProperty, true);

                    KeyboardFocusChangedEventArgs e = new KeyboardFocusChangedEventArgs();
                    e.OriginalSource = e.Source = element;
                    e.RoutedEvent = UIElement.GotKeyboardFocusEvent;
                    element.RaiseEvent(e);
                }

                _target = element;
            }
        }
    }
}
