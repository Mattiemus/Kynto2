namespace Spark.Input
{
    using System;
    
    /// <summary>
    /// Groups a single input condition with a single action that is only triggered when the condition is true.
    /// </summary>
    public class InputTrigger : INamable
    {
        private string _name;
        private bool _prevWasSuccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputTrigger"/> class.
        /// </summary>
        /// <param name="condition">The input condition to check.</param>
        /// <param name="action">The input action to perform when the condition is satisfied.</param>
        public InputTrigger(InputCondition condition, InputAction action) 
            : this(string.Empty, condition, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputTrigger"/> class.
        /// </summary>
        /// <param name="name">The name of the trigger, optional.</param>
        /// <param name="condition">The input condition to check.</param>
        /// <param name="action">The input action to perform when the condition is satisfied.</param>
        public InputTrigger(string name, InputCondition condition, InputAction action)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _name = string.IsNullOrEmpty(name) ? string.Empty : name;
            Condition = condition;
            Action = action;
            IsEnabled = true;
        }

        /// <summary>
        /// Gets or sets the (optional) name of the trigger. Useful for key mappings.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        /// <summary>
        /// Gets or sets the condition of this trigger.
        /// </summary>
        public InputCondition Condition { get; set; }
        /// <summary>
        /// Gets or sets the action of this trigger.
        /// </summary>
        public InputAction Action { get; set; }

        /// <summary>
        /// Gets or sets if this trigger is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Checks if the condition is true and performs the action if so.
        /// </summary>
        /// <param name="time">Represents current time snapshot.</param>
        /// <returns>True if the action successfully was performed, false otherwise.</returns>
        public bool CheckAndPerform(IGameTime time)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (Condition.Check(time))
            {
                if (!_prevWasSuccess)
                {
                    _prevWasSuccess = true;
                    Action.OnBegin(time);
                }

                Action.Perform(time);
                return true;
            }

            if (_prevWasSuccess)
            {
                _prevWasSuccess = false;
                Action.OnEnd(time);
            }

            return false;
        }
    }
}
