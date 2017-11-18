namespace Spark.Input
{
    using System;
    
    /// <summary>
    /// Delegate for logic that checks some condition and evaluates true or false.
    /// </summary>
    /// <param name="time">Time elapsed since the last update</param>
    /// <returns>True if the condition has been satisfied, false otherwise.</returns>
    public delegate bool InputConditionDelegate(IGameTime time);

    /// <summary>
    /// Represents the condition that must be satisfied by some input. If the condition is true, then an
    /// action can be performed. This is paired with an InputAction inside an InputTrigger. Actions and conditions can 
    /// vary independently from one another, allowing for their reuse.
    /// </summary>
    public class InputCondition
    {
        private readonly InputConditionDelegate _condition;

        /// <summary>
        /// Initializes a newinstance of the <see cref="InputCondition"/> class.
        /// </summary>
        protected InputCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputCondition"/> class with the specified condition delegate.
        /// </summary>
        /// <param name="condition">Input condition delegate</param>
        public InputCondition(InputConditionDelegate condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            _condition = condition;
        }

        /// <summary>
        /// Checks if the condition has been satisfied or not.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        /// <returns>True if the condition has been satisfied, false otherwise.</returns>
        public virtual bool Check(IGameTime time)
        {
            if (_condition == null)
            {
                return false;
            }

            return _condition(time);
        }
    }
}
