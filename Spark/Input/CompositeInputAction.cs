namespace Spark.Input
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents a composite input action where all actions are performed when an input condition evaluates to true. This is paired with an InputCondition
    /// inside an InputTrigger. Actions and conditions can vary independently from one another, allowing for their reuse.
    /// </summary>
    public sealed class CompositeInputAction : InputAction
    {
        private readonly List<InputAction> _actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeInputAction"/> class.
        /// </summary>
        /// <param name="actions">Input actions.</param>
        public CompositeInputAction(params InputAction[] actions)
        {
            if (actions == null || actions.Length == 0)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            _actions = new List<InputAction>(actions.Length);
            for (int i = 0; i < actions.Length; i++)
            {
                InputAction action = actions[i];
                if (action == null)
                {
                    throw new ArgumentNullException(nameof(actions));
                }

                _actions.Add(action);
            }
        }

        /// <summary>
        /// Gets a list of contained actions.
        /// </summary>
        public IReadOnlyList<InputAction> Actions => _actions;

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        public override void Perform(IGameTime time)
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                _actions[i].Perform(time);
            }
        }
    }
}
