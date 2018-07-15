namespace Spark.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class RangeCollection : ICloneable, ICollection<int>
    {
        private const int MinCapacity = 16;

        private Range[] _ranges;
        private int _rangeCount;
        private int _indexCount;
        private int _generation;

        public RangeCollection()
        {
            Clear();
        }

        public Range[] Ranges
        {
            get
            {
                Range[] copy = new Range[_rangeCount];
                Array.Copy(_ranges, copy, _rangeCount);
                return copy;
            }
        }

        public int RangeCount => _rangeCount;

        public int Count => _indexCount;

        public bool IsReadOnly => false;

        public int this[int index]
        {
            get
            {
                for (int i = 0, cumlCount = 0; i < _rangeCount && index >= 0; i++)
                {
                    if (index < (cumlCount += _ranges[i].Count))
                    {
                        return _ranges[i].End - (cumlCount - index) + 1;
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public int FindRangeIndexForValue(int value)
        {
            int min = 0;
            int max = _rangeCount - 1;

            while (min <= max)
            {
                int mid = min + ((max - min) / 2);
                Range range = _ranges[mid];
                if (value >= range.Start && value <= range.End)
                {
                    return mid;    // In Range
                }

                if (value < range.Start)
                {
                    max = mid - 1; // Below Range
                }
                else
                {
                    min = mid + 1; // Above Range
                }
            }

            return ~min;
        }

        public int IndexOf(int value)
        {
            int offset = 0;

            foreach (Range range in _ranges)
            {
                if (value >= range.Start && value <= range.End)
                {
                    return offset + (value - range.Start);
                }

                offset += range.End - range.Start + 1;
            }

            return -1;
        }

        public bool Add(int value)
        {
            if (!Contains(value))
            {
                _generation++;
                InsertRange(new Range(value, value));
                _indexCount++;
                return true;
            }

            return false;
        }

        public bool Remove(int value)
        {
            _generation++;
            return RemoveIndexFromRange(value);
        }

        public void Clear()
        {
            _rangeCount = 0;
            _indexCount = 0;
            _generation++;
            _ranges = new Range[MinCapacity];
        }

        public bool Contains(int value)
        {
            return FindRangeIndexForValue(value) >= 0;
        }

        public void CopyTo(int[] array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < _rangeCount; i++)
            {
                for (int j = _ranges[i].Start; j <= _ranges[i].End; j++)
                {
                    yield return j;
                }
            }
        }

        void ICollection<int>.Add(int value)
        {
            Add(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static int CompareRanges(Range a, Range b)
        {
            return (a.Start + (a.End - a.Start)).CompareTo(b.Start + (b.End - b.Start));
        }

        private void Shift(int start, int delta)
        {
            if (delta < 0)
            {
                start -= delta;
            }

            if (start < _rangeCount)
            {
                Array.Copy(_ranges, start, _ranges, start + delta, _rangeCount - start);
            }

            _rangeCount += delta;
        }

        private void EnsureCapacity(int growBy)
        {
            int newCapacity = _ranges.Length == 0 ? 1 : _ranges.Length;
            int minCapacity = _ranges.Length == 0 ? MinCapacity : _ranges.Length + growBy;

            while (newCapacity < minCapacity)
            {
                newCapacity <<= 1;
            }

            Array.Resize(ref _ranges, newCapacity);
        }

        private void Insert(int position, Range range)
        {
            if (_rangeCount == _ranges.Length)
            {
                EnsureCapacity(1);
            }

            Shift(position, 1);
            _ranges[position] = range;
        }

        private void RemoveAt(int position)
        {
            Shift(position, -1);
            Array.Clear(_ranges, _rangeCount, 1);
        }

        private bool RemoveIndexFromRange(int index)
        {
            int rangeIndex = FindRangeIndexForValue(index);

            if (rangeIndex < 0)
            {
                return false;
            }

            Range range = _ranges[rangeIndex];
            if (range.Start == index && range.End == index)
            {
                RemoveAt(rangeIndex);
            }
            else
            {
                if (range.Start == index)
                {
                    _ranges[rangeIndex].Start++;
                }
                else
                {
                    if (range.End == index)
                    {
                        _ranges[rangeIndex].End--;
                    }
                    else
                    {
                        Range splitRange = new Range(index + 1, range.End);
                        _ranges[rangeIndex].End = index - 1;
                        Insert(rangeIndex + 1, splitRange);
                    }
                }
            }

            _indexCount--;
            return true;
        }

        private void InsertRange(Range range)
        {
            int position = FindInsertionPosition(range);
            bool mergedLeft = MergeLeft(range, position);
            bool mergedRight = MergeRight(range, position);

            if (!mergedLeft && !mergedRight)
            {
                Insert(position, range);
            }
            else
            {
                if (mergedLeft && mergedRight)
                {
                    _ranges[position - 1].End = _ranges[position].End;
                    RemoveAt(position);
                }
            }
        }

        private bool MergeLeft(Range range, int position)
        {
            int left = position - 1;
            if (left >= 0 && _ranges[left].End + 1 == range.Start)
            {
                _ranges[left].End = range.Start;
                return true;
            }

            return false;
        }

        private bool MergeRight(Range range, int position)
        {
            if (position < _rangeCount && _ranges[position].Start - 1 == range.End)
            {
                _ranges[position].Start = range.End;
                return true;
            }

            return false;
        }

        private int FindInsertionPosition(Range range)
        {
            int min = 0;
            int max = _rangeCount - 1;

            while (min <= max)
            {
                int mid = min + ((max - min) / 2);
                int cmp = CompareRanges(_ranges[mid], range);

                if (cmp == 0)
                {
                    return mid;
                }
                else
                {
                    if (cmp > 0)
                    {
                        if (mid > 0 && CompareRanges(_ranges[mid - 1], range) < 0)
                        {
                            return mid;
                        }

                        max = mid - 1;
                    }
                    else
                    {
                        min = mid + 1;
                    }
                }
            }

            return min;
        }

        public struct Range
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Range"/> struct.
            /// </summary>
            public Range(int start, int end)
            {
                Start = start;
                End = end;
            }

            public int Start { get; set; }

            public int End { get; set; }

            public int Count => End - Start + 1;

            public override string ToString()
            {
                return string.Format("{0}-{1} ({2})", Start, End, Count);
            }
        }
    }
}
