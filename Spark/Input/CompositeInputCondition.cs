namespace Spark.Input
{
    using System;
    using System.Collections.Generic;

    using Core;

    /// <summary>
    /// Represents a composite input condition where all conditions must be satisfied by some input. If the condition is true,
    /// then the action can be performed. This is paired with an InputAction inside an InputTrigger. Actions and conditions can 
    /// vary independently from one another, allowing for their reuse.
    /// </summary>
    public sealed class CompositeInputCondition : InputCondition
    {
        private readonly List<InputCondition> _conditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeInputCondition"/> class.
        /// </summary>
        /// <param name="conditions">Input conditions.</param>
        public CompositeInputCondition(params InputCondition[] conditions)
        {
            if (conditions == null || conditions.Length == 0)
            {
                throw new ArgumentNullException(nameof(conditions));
            }

            _conditions = new List<InputCondition>(conditions.Length);
            for (int i = 0; i < conditions.Length; i++)
            {
                InputCondition cond = conditions[i];
                if (cond == null)
                {
                    throw new ArgumentNullException(nameof(conditions));
                }

                _conditions.Add(cond);
            }
        }

        /// <summary>
        /// Gets the list of contained conditions.
        /// </summary>
        public IReadOnlyList<InputCondition> Conditions => _conditions;

        /// <summary>
        /// Checks if the condition has been satisfied or not.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        /// <returns>True if the condition has been satisfied, false otherwise.</returns>
        public override bool Check(IGameTime time)
        {
            for (int i = 0; i < _conditions.Count; i++)
            {
                if (!_conditions[i].Check(time))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
