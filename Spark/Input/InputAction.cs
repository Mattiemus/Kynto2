namespace Spark.Input
{
    using System;
    
    /// <summary>
    /// Delegate for logic that is executed by an InputAction, used in leui of a subclass.
    /// </summary>
    /// <param name="time">Time elapsed since the last update.</param>
    public delegate void InputActionDelegate(IGameTime time);

    /// <summary>
    /// Represents the logic that should be performed when an input condition evaluates to true. This is paired with an InputCondition
    /// inside an InputTrigger. Actions and conditions can vary independently from one another, allowing for their reuse.
    /// </summary>
    public class InputAction
    {
        private readonly InputActionDelegate _action;

        /// <summary>
        /// Iniitializes a new instance of the <see cref="InputAction"/> class.
        /// </summary>
        protected InputAction()
        {
        }

        /// <summary>
        /// Iniitializes a new instance of the <see cref="InputAction"/> class with the specified action delegate.
        /// </summary>
        /// <param name="action">Input action delegate</param>
        public InputAction(InputActionDelegate action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _action = action;
        }

        /// <summary>
        /// Called by an input trigger when the condition first succeeds, allowing the action to do first-time setup.
        /// </summary>
        /// <param name="time"></param>
        public virtual void OnBegin(IGameTime time)
        {
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        public virtual void Perform(IGameTime time)
        {
            if (_action == null)
            {
                return;
            }

            _action(time);
        }

        /// <summary>
        /// Called by an input trigger when the condition fails, allowing the action to do last-time cleanup.
        /// </summary>
        public virtual void OnEnd(IGameTime time)
        {
        }
    }
}
