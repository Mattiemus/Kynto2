namespace Spark.UI.Input
{
    using System.Linq;
    using System.Collections.Generic;

    public sealed class InputManager
    {
        private readonly Stack<InputEventArgs> _stack;
        private bool _processing;

        public InputManager()
        {
            _stack = new Stack<InputEventArgs>();
        }

        static InputManager()
        {
            Current = new InputManager();
        }

        public event PreProcessInputEventHandler PreProcessInput;

        public static InputManager Current { get; }

        public void ProcessInput(InputEventArgs input)
        {
            _stack.Push(input);
            ProcessStack();
        }

        private void ProcessStack()
        {
            if (_processing)
            {
                return;
            }

            _processing = true;

            while (_stack.Count > 0)
            {
                InputEventArgs input = _stack.Pop();

                try
                {
                    PreProcessInputEventArgs e = new PreProcessInputEventArgs(input);

                    input.OriginalSource = input.Device.Target;

                    if (PreProcessInput != null)
                    {
                        foreach (var handler in PreProcessInput.GetInvocationList().Reverse())
                        {
                            handler.DynamicInvoke(this, e);
                        }
                    }

                    if (!e.Canceled)
                    {
                        UIElement uiElement = input.OriginalSource as UIElement;

                        if (uiElement != null)
                        {
                            uiElement.RaiseEvent(input);
                        }
                    }
                }
                catch
                {
                }
            }

            _processing = false;
        }
    }
}
