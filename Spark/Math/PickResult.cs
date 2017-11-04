namespace Spark.Math
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A picking query result for a single object, which contains two components: a bounding intersection and a variable number of primitive intersections. Depending on
    /// the picking query, a result may just contain the bounding intersection result.
    /// </summary>
    public sealed class PickResult : IReadOnlyList<Tuple<LineIntersectionResult, Triangle?>>
    {
        private readonly List<Tuple<LineIntersectionResult, Triangle?>> _results;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickResult"/> class.
        /// </summary>
        public PickResult()
        {
            _results = new List<Tuple<LineIntersectionResult, Triangle?>>();
            BoundingIntersection = new BoundingIntersectionResult();
            Target = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PickResult"/> class.
        /// </summary>
        /// <param name="target">The picking target.</param>
        /// <param name="boundingResult">Bounding intersection result.</param>
        public PickResult(IPickable target, BoundingIntersectionResult boundingResult) : this(target, boundingResult, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PickResult"/> class.
        /// </summary>
        /// <param name="target">The picking target.</param>
        /// <param name="boundingResult">Bounding intersection result.</param>
        /// <param name="results">Optional list of primitive intersections.</param>
        public PickResult(IPickable target, BoundingIntersectionResult boundingResult, IEnumerable<Tuple<LineIntersectionResult, Triangle?>> results)
        {
            if (results != null)
            {
                _results = new List<Tuple<LineIntersectionResult, Triangle?>>(results);
            }
            else
            {
                _results = new List<Tuple<LineIntersectionResult, Triangle?>>();
            }

            Target = target;
            BoundingIntersection = boundingResult;
            _results.Sort(new Comparer());
        }

        /// <summary>
        /// Gets the number of primitive intersections contained in the result. If the count is zero, the pick result will
        /// still carry a bounding intersection result.
        /// </summary>
        public int Count => _results.Count;

        /// <summary>
        /// Gets the target of the result.
        /// </summary>
        public IPickable Target { get; private set; }

        /// <summary>
        /// Gets the bounding intersection result.
        /// </summary>
        public BoundingIntersectionResult BoundingIntersection { get; private set; }

        /// <summary>
        /// Gets the closest primitive intersection.
        /// </summary>
        public Tuple<LineIntersectionResult, Triangle?> ClosestIntersection
        {
            get
            {
                GetIntersection(0, out LineIntersectionResult result, out Triangle? triangle);
                return new Tuple<LineIntersectionResult, Triangle?>(result, triangle);
            }
        }

        /// <summary>
        /// Gets the farthest primitive intersection.
        /// </summary>
        public Tuple<LineIntersectionResult, Triangle?> FarthestIntersection
        {
            get
            {
                GetIntersection(_results.Count - 1, out LineIntersectionResult result, out Triangle?  triangle);
                return new Tuple<LineIntersectionResult, Triangle?>(result, triangle);
            }
        }

        /// <summary>
        /// Gets the primitive intersection at the specified index.
        /// </summary>
        public Tuple<LineIntersectionResult, Triangle?> this[int index]
        {
            get
            {
                GetIntersection(index, out LineIntersectionResult result, out Triangle? triangle);
                return new Tuple<LineIntersectionResult, Triangle?>(result, triangle);
            }
        }
        
        /// <summary>
        /// Gets the bounding intersection result.
        /// </summary>
        /// <param name="result">Bounding intersection result.</param>
        public void GetBoundingIntersection(out BoundingIntersectionResult result)
        {
            result = BoundingIntersection;
        }

        /// <summary>
        /// Gets the closest primitive intersection.
        /// </summary>
        /// <param name="result">Closest primitive intersection result.</param>
        /// <returns>True if the intersection exists, false if not.</returns>
        public bool GetClosestIntersection(out LineIntersectionResult result)
        {
            return GetIntersection(0, out result, out Triangle? triangle);
        }

        /// <summary>
        /// Gets the closest primitive intersection.
        /// </summary>
        /// <param name="result">Closest primitive intersection result.</param>
        /// <param name="triangle">Optional triangle that was picked.</param>
        /// <returns>True if the intersection exists, false if not.</returns>
        public bool GetClosestIntersection(out LineIntersectionResult result, out Triangle? triangle)
        {
            return GetIntersection(0, out result, out triangle);
        }

        /// <summary>
        /// Gets the farthest primitive intersection.
        /// </summary>
        /// <param name="result">Farthest primitive intersection result.</param>
        /// <returns>True if the intersection exists, false if not.</returns>
        public bool GetFarthestIntersection(out LineIntersectionResult result)
        {
            return GetIntersection(_results.Count - 1, out result, out Triangle? triangle);
        }

        /// <summary>
        /// Gets the farthest primitive intersection.
        /// </summary>
        /// <param name="result">Farthest primitive intersection result.</param>
        /// <param name="triangle">Optional triangle that was picked.</param>
        /// <returns>True if the intersection exists, false if not.</returns>
        public bool GetFarthestIntersection(out LineIntersectionResult result, out Triangle? triangle)
        {
            return GetIntersection(_results.Count - 1, out result, out triangle);
        }

        /// <summary>
        /// Gets the primitive intersection at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the primitive intersection.</param>
        /// <param name="result">Farthest primitive intersection result.</param>
        /// <returns>True if the intersection exists, false if not.</returns>
        public bool GetIntersection(int index, out LineIntersectionResult result)
        {
            return GetIntersection(index, out result, out Triangle? triangle);
        }

        /// <summary>
        /// Gets the primitive intersection at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the primitive intersection.</param>
        /// <param name="result">Farthest primitive intersection result.</param>
        /// <param name="triangle">Optional triangle that was picked.</param>
        /// <returns>True if the intersection exists, false if not.</returns>
        public bool GetIntersection(int index, out LineIntersectionResult result, out Triangle? triangle)
        {
            if (index < 0 || index >= _results.Count)
            {
                result = new LineIntersectionResult();
                triangle = null;
                return false;
            }

            Tuple<LineIntersectionResult, Triangle?> pair = _results[index];

            result = pair.Item1;
            triangle = pair.Item2;

            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the pick results.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the pick results.</returns>
        public List<Tuple<LineIntersectionResult, Triangle?>>.Enumerator GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<Tuple<LineIntersectionResult, Triangle?>> IEnumerable<Tuple<LineIntersectionResult, Triangle?>>.GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _results.GetEnumerator();
        }
        
        /// <summary>
        /// Comparer for sorting primitive intersection results
        /// </summary>
        private sealed class Comparer : IComparer<Tuple<LineIntersectionResult, Triangle?>>
        {
            /// <summary>
            /// Compares two intersection results
            /// </summary>
            /// <param name="x">First intersection result</param>
            /// <param name="y">Second intersection result</param>
            /// <returns>0 if both values are identical, -1 if x is closer than y, 1 if y is closer than x</returns>
            public int Compare(Tuple<LineIntersectionResult, Triangle?> x, Tuple<LineIntersectionResult, Triangle?> y)
            {
                float distx = x.Item1.Distance;
                float disty = y.Item1.Distance;
                
                if (MathHelper.IsApproxEquals(distx, disty))
                {
                    return 0;
                }

                if (distx < disty)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
