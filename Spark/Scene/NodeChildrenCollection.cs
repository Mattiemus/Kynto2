namespace Spark.Scene
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections;
    using System.Collections.Generic;

    public class NodeChildrenCollection : IList<Spatial>, IReadOnlyList<Spatial>
    {
        private int _version;
        private readonly List<Spatial> _children;
        private bool _suspendPropogateDirty;
        
        public NodeChildrenCollection(Node parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;
            _children = new List<Spatial>();
        }

        public NodeChildrenCollection(Node parent, int initialCapacity)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;
            _children = new List<Spatial>(initialCapacity);
        }

        public Node Parent { get; }

        public int Capacity
        {
            get => _children.Capacity;
            set => _children.Capacity = value;
        }

        public int Count => _children.Count;

        public Spatial this[int index]
        {
            get
            {
                if (index < 0 || index >= _children.Count)
                {
                    return null;
                }

                return _children[index];
            }
            set
            {
                if (index < 0 || index >= _children.Count)
                {
                    return;
                }

                if (value == null)
                {
                    RemoveAt(index);
                    return;
                }

                Spatial old = _children[index];
                old.SetParent(null);
                value.AttachToParent(Parent);
                _children[index] = value;

                Parent.PropagateDirtyDown(DirtyMark.All);
                Parent.PropagateDirtyUp(DirtyMark.Bounding);
                _version++;
            }
        }

        public Spatial this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }

                for (int i = 0; i < _children.Count; i++)
                {
                    Spatial child = _children[i];
                    if (name.Equals(child.Name))
                    {
                        return child;
                    }
                }

                return null;
            }
        }

        public bool IsReadOnly => false;

        internal bool SuspendPropogateDirty
        {
            get => _suspendPropogateDirty;
            set => _suspendPropogateDirty = value;
        }

        public void Add(Spatial spatial)
        {
            if (spatial == null || spatial.Parent == Parent)
            {
                return;
            }

            spatial.AttachToParent(Parent);
            _children.Add(spatial);

            if (!_suspendPropogateDirty)
            {
                Parent.PropagateDirtyDown(DirtyMark.All);
                Parent.PropagateDirtyUp(DirtyMark.Bounding);
            }

            _version++;
        }

        public void AddRange(IEnumerable<Spatial> spatials)
        {
            if (spatials == null)
            {
                return;
            }

            bool addedOne = false;

            foreach (Spatial s in spatials)
            {
                if (s != null && s.Parent != Parent)
                {
                    s.AttachToParent(Parent);
                    _children.Add(s);
                    addedOne = true;
                }
            }

            if (addedOne)
            {
                if (!_suspendPropogateDirty)
                {
                    Parent.PropagateDirtyDown(DirtyMark.All);
                    Parent.PropagateDirtyUp(DirtyMark.Bounding);
                }

                _version++;
            }
        }

        public int IndexOf(Spatial spatial)
        {
            if (spatial == null || spatial.Parent != Parent)
            {
                return -1;
            }

            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] == spatial)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Remove(Spatial spatial)
        {
            if (spatial == null || spatial.Parent != Parent)
            {
                return false;
            }

            for (int i = 0; i < _children.Count; i++)
            {
                Spatial child = _children[i];
                if (child == spatial)
                {
                    child.SetParent(null);
                    _children.RemoveAt(i);

                    if (!_suspendPropogateDirty)
                    {
                        Parent.PropagateDirtyUp(DirtyMark.Bounding);
                    }

                    _version++;
                    return true;
                }
            }

            return false;
        }

        public bool Remove(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            for (int i = 0; i < _children.Count; i++)
            {
                Spatial child = _children[i];
                if (name.Equals(child.Name))
                {
                    child.SetParent(null);
                    _children.RemoveAt(i);

                    if (!_suspendPropogateDirty)
                    {
                        Parent.PropagateDirtyUp(DirtyMark.Bounding);
                    }

                    _version++;
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                return;
            }

            Spatial child = _children[index];
            child.SetParent(null);
            _children.RemoveAt(index);

            if (!_suspendPropogateDirty)
            {
                Parent.PropagateDirtyUp(DirtyMark.Bounding);
            }

            _version++;
        }

        public void Clear()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].SetParent(null);
            }

            _children.Clear();
            _version++;
        }

        public void Insert(int index, Spatial item)
        {
            if (index < 0 || item == null || item.Parent == Parent)
            {
                return;
            }

            item.AttachToParent(Parent);
            _children.Insert(index, item);

            if (!_suspendPropogateDirty)
            {
                Parent.PropagateDirtyDown(DirtyMark.All);
                Parent.PropagateDirtyUp(DirtyMark.Bounding);
            }

            _version++;
        }

        public bool Contains(Spatial item)
        {
            if (item == null)
            {
                return false;
            }

            if (item.Parent == Parent)
            {
                return true;
            }

            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] == item)
                {
                    return true;
                }
            }

            return false;
        }

        public Spatial[] ToArray()
        {
            return _children.ToArray();
        }

        public void CopyTo(Spatial[] array)
        {
            _children.CopyTo(array);
        }

        public void CopyTo(Spatial[] array, int arrayIndex)
        {
            _children.CopyTo(array, arrayIndex);
        }

        public void Sort(IComparer<Spatial> comparer)
        {
            _children.Sort(comparer);
        }

        public NodeChildrenEnumerator GetEnumerator()
        {
            return new NodeChildrenEnumerator(this);
        }

        IEnumerator<Spatial> IEnumerable<Spatial>.GetEnumerator()
        {
            return new NodeChildrenEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NodeChildrenEnumerator(this);
        }

        #region Enumerator

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct NodeChildrenEnumerator : IEnumerator<Spatial>
        {
            private readonly int _version;
            private readonly NodeChildrenCollection _collection;
            private readonly List<Spatial> _children;
            private int _index;
            private Spatial _current;

            public Spatial Current => _current;

            object IEnumerator.Current => Current;

            internal NodeChildrenEnumerator(NodeChildrenCollection children)
            {
                _collection = children;
                _children = children._children;
                _index = 0;
                _version = children._version;
                _current = null;
            }

            public bool MoveNext()
            {
                ThrowIfChanged();

                if (_index < _children.Count)
                {
                    _current = _children[_index];
                    _index++;
                    return true;
                }
                else
                {
                    _index++;
                    _current = null;
                    return false;
                }
            }

            public void Reset()
            {
                ThrowIfChanged();
                _index = 0;
                _current = null;
            }

            public void Dispose()
            {
                // No-op
            }

            private void ThrowIfChanged()
            {
                if (_version != _collection._version)
                {
                    throw new InvalidOperationException("Collection modified during enumeration");
                }
            }
        }

        #endregion
    }
}
