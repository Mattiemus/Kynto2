namespace Spark
{
    using System.Collections.Generic;

    /// <summary>
    /// Implements a List structure as a cyclical list where indices are wrapped.
    /// </summary>
    /// <typeparam name="T">The Type to hold in the list.</typeparam>
    public class CyclicalList<T> : List<T>
    {
        public CyclicalList()
        {
        }

        public CyclicalList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public new T this[int index]
        {
            get
            {
                while (index < 0)
                {
                    index = Count + index;
                }

                if (index >= Count)
                {
                    index %= Count;
                }

                return base[index];
            }
            set
            {
                while (index < 0)
                {
                    index = Count + index;
                }

                if (index >= Count)
                {
                    index %= Count;
                }

                base[index] = value;
            }
        }

        public new void RemoveAt(int index)
        {
            Remove(this[index]);
        }
    }
}
