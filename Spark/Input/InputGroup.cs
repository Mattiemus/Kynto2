namespace Spark.Input
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Core;

    /// <summary>
    /// Manages a group of <see cref="InputTrigger"/> objects. Generally triggers are grouped
    /// logically so they can all be enabled or checked together.
    /// </summary>
    public sealed class InputGroup : IReadOnlyList<InputTrigger>, INamable
    {
        private string _name;
        private readonly List<InputTrigger> _triggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputGroup"/> class.
        /// </summary>
        public InputGroup() 
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputGroup"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public InputGroup(string name)
        {
            _name = string.IsNullOrEmpty(name) ? string.Empty : name;
            _triggers = new List<InputTrigger>();
            IsEnabled = true;
        }

        /// <summary>
        /// Gets or sets the (optional) name of the input group.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        /// <summary>
        /// Gets or sets if the input group is enabled. If disabled then no triggers will be checked during update.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the number of input triggers in the group.
        /// </summary>
        public int Count => _triggers.Count;

        /// <summary>
        /// Gets the input trigger at the specified index.
        /// </summary>
        /// <param name="index">Zero based index.</param>
        /// <returns>Trigger at the specified index, or null if index is out of bounds.</returns>
        public InputTrigger this[int index]
        {
            get
            {
                if (index < 0 || index >= _triggers.Count)
                {
                    return null;
                }

                return _triggers[index];
            }
        }

        /// <summary>
        /// Gets the input trigger corresponding to the specified name.
        /// </summary>
        /// <param name="name">Name of the input trigger.</param>
        /// <returns>Trigger corresponding to the name, or null if it does not exist.</returns>
        public InputTrigger this[string name]
        {
            get
            {
                if (name == null)
                {
                    return null;
                }

                for (int i = 0; i < _triggers.Count; i++)
                {
                    InputTrigger trigger = _triggers[i];
                    if (trigger.Name.Equals(name))
                    {
                        return trigger;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Checks each trigger in the group if its condition is true, then performs
        /// the trigger's action.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        public void CheckAndPerformTriggers(IGameTime time)
        {
            if (!IsEnabled)
            {
                return;
            }

            for (int i = 0; i < _triggers.Count; i++)
            {
                _triggers[i].CheckAndPerform(time);
            }
        }

        /// <summary>
        /// Adds the specified input trigger to the group. If the value is null, it will not be added.
        /// </summary>
        /// <param name="item">Input trigger to add.</param>
        public void Add(InputTrigger item)
        {
            if (item == null)
            {
                return;
            }

            _triggers.Add(item);
        }

        /// <summary>
        /// Inserts the specified input trigger into the group. If the value is null or the index is out of
        /// range, it will not be added.
        /// </summary>
        /// <param name="index">Zero based index at which the item will be inserted.</param>
        /// <param name="item">Input trigger to insert.</param>
        public void Insert(int index, InputTrigger item)
        {
            if (index < 0 || index > _triggers.Count || item == null)
            {
                return;
            }

            _triggers.Insert(index, item);
        }

        /// <summary>
        /// Queries if the specific input trigger is in the group.
        /// </summary>
        /// <param name="item">Input trigger to check.</param>
        /// <returns>True if the trigger is contained in the group, false otherwise.</returns>
        public bool Contains(InputTrigger item)
        {
            if (item == null)
            {
                return false;
            }

            return _triggers.Contains(item);
        }

        /// <summary>
        /// Queries if there exists an input trigger corresponding to the specified name.
        /// </summary>
        /// <param name="itemName">Name of input trigger.</param>
        /// <returns>True if an input trigger with that name exists, false otherwise.</returns>
        public bool Contains(String itemName)
        {
            if (itemName == null)
            {
                return false;
            }

            for (int i = 0; i < _triggers.Count; i++)
            {
                if (_triggers[i].Name.Equals(itemName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the index of the specified input trigger in the group.
        /// </summary>
        /// <param name="item">Input trigger to get the index of in the group.</param>
        /// <returns>Zero based index. If the input trigger was not present, -1 is returned.</returns>
        public int IndexOf(InputTrigger item)
        {
            if (item == null)
            {
                return -1;
            }

            return _triggers.IndexOf(item);
        }

        /// <summary>
        /// Gets the index of the input trigger corresponding to the specified name.
        /// </summary>
        /// <param name="itemName">Name of the input trigger.</param>
        /// <returns>Zero based index. If the input trigger was not present, -1 is returned.</returns>
        public int IndexOf(String itemName)
        {
            if (itemName == null)
            {
                return -1;
            }

            for (int i = 0; i < _triggers.Count; i++)
            {
                if (_triggers[i].Name.Equals(itemName))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Enables or disables an input trigger at the specified index.
        /// </summary>
        /// <param name="index">Zero based index of the input trigger in the group.</param>
        /// <param name="isEnabled">True if the trigger should be enabled, false if disabled.</param>
        /// <returns>True if the trigger was succesfully modified, false if it was not found or the index was out of range.</returns>
        public bool EnableAt(int index, bool isEnabled)
        {
            if (index < 0 || index >= _triggers.Count)
            {
                return false;
            }

            InputTrigger trigger = _triggers[index];
            if (trigger != null)
            {
                trigger.IsEnabled = isEnabled;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all input triggers from the group.
        /// </summary>
        public void Clear()
        {
            _triggers.Clear();
        }

        /// <summary>
        /// Removes the specified input trigger from the group.
        /// </summary>
        /// <param name="item">Input trigger to remove.</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        public bool Remove(InputTrigger item)
        {
            return _triggers.Remove(item);
        }

        /// <summary>
        /// Removes the input trigger corresponding to the specified name, if it exists.
        /// </summary>
        /// <param name="itemName">Name of input trigger to remove.</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        public bool Remove(String itemName)
        {
            if (itemName == null)
            {
                return false;
            }

            for (int i = 0; i < _triggers.Count; i++)
            {
                if (_triggers[i].Name.Equals(itemName))
                {
                    _triggers.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the input trigger at the specified index.
        /// </summary>
        /// <param name="index">Zero based index.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _triggers.Count)
            {
                return;
            }

            _triggers.RemoveAt(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<InputTrigger> GetEnumerator()
        {
            return _triggers.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _triggers.GetEnumerator();
        }
    }
}
