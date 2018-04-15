namespace Spark.Math
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// Query object for finding and sorting pick intersections with objects. Both bounding and primitive (usually triangle) picking is supported.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IReadOnlyList{PickResult}" />
    public class PickQuery : IReadOnlyList<PickResult>
    {
        private Ray _ray;
        private readonly List<PickResult> _results;
        private bool _sorted;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickQuery"/> class.
        /// </summary>
        public PickQuery()
        {
            _results = new List<PickResult>();
            _sorted = false;
            Options = PickingOptions.None;
        }

        /// <summary>
        /// Gets a comparer that sorts based on the distance of bounding intersections. Smallest distance is the first result.
        /// </summary>
        public static IComparer<PickResult> BoundingPickDistanceComparer => new BoundingPickComparer();

        /// <summary>
        /// Gets a comparer that sorts absed on distance of primitive intersections (and if equal, falls back to bounding intersection). Smallest distance is the first result.
        /// </summary>
        public static IComparer<PickResult> PrimitivePickDistanceComparer => new PrimitivePickComparer();

        /// <summary>
        /// Gets or sets the pick ray used in the query.
        /// </summary>
        public Ray PickRay
        {
            get => _ray;
            set => _ray = value;
        }

        /// <summary>
        /// Gets or sets picking options.
        /// </summary>
        public PickingOptions Options { get; set; }

        /// <summary>
        /// Gets the number of results in the query.
        /// </summary>
        public int Count => _results.Count;

        /// <summary>
        /// Gets if the query results have been sorted.
        /// </summary>
        public bool IsSorted => _sorted;

        /// <summary>
        /// Gets the <see cref="PickResult"/> at the specified index.
        /// </summary>
        public PickResult this[int index]
        {
            get
            {
                if (index < 0 || index >= _results.Count)
                {
                    return null;
                }

                return _results[index];
            }
        }

        /// <summary>
        /// Gets the closest pick, if it exists (null may be returned).
        /// </summary>
        public PickResult ClosestPick
        {
            get
            {
                if (_results.Count > 0)
                {
                    return _results[0];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the farthest pick, if it exists (null may be returned).
        /// </summary>
        public PickResult FarthestPick
        {
            get
            {
                if (_results.Count > 0)
                {
                    return _results[_results.Count - 1];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the pick ray used in queries.
        /// </summary>
        /// <param name="pickRay">Pick ray used in the query.</param>
        public void GetPickRay(out Ray pickRay)
        {
            pickRay = _ray;
        }

        /// <summary>
        /// Adds a pick to the query
        /// </summary>
        /// <param name="pickable">Pickable object</param>
        /// <returns>True if the item intersects with the pick ray, false otherwise</returns>
        public bool AddPick(IPickable pickable)
        {
            if (pickable == null)
            {
                return false;
            }

            BoundingVolume bv = pickable.WorldBounding;
            BoundingIntersectionResult bvResult;

            // If doesn't intersect with bounding... no pick
            if (bv != null && bv.Intersects(ref _ray, out bvResult))
            {                
                // If no primitive picking just set the result and return true, otherwise
                // do primitive picking, if that succeeds set the result and return true
                if (Options.HasFlag(PickingOptions.PrimitivePick))
                {
                    var results = new List<Tuple<LineIntersectionResult, Triangle?>>();
                    if (pickable.IntersectsMesh(ref _ray, results, Options.HasFlag(PickingOptions.IgnoreBackfaces)))
                    {
                        var result = new PickResult(pickable, bvResult);
                        _results.Add(result);
                        return true;
                    }
                }
                else
                {
                    var result = new PickResult(pickable, bvResult);
                    _results.Add(result);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sorts the pick results contained in the query. If primitive picking is enabled, sorting is based on pick distances of the primitives, otherwise the distances
        /// of the bounding picks will be used.
        /// </summary>
        public void Sort()
        {
            if (_sorted)
            {
                return;
            }

            IComparer<PickResult> comparer = Options.HasFlag(PickingOptions.PrimitivePick) ? PrimitivePickDistanceComparer : BoundingPickDistanceComparer;
            _results.Sort(comparer);
            _sorted = true;
        }

        /// <summary>
        /// Sorts the pick results contained in the query using a custom comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public void Sort(IComparer<PickResult> comparer)
        {
            if (_sorted)
            {
                return;
            }

            if (comparer == null)
            {
                Sort();
            }

            _results.Sort(comparer);
            _sorted = true;
        }

        /// <summary>
        /// Clears the query of all results.
        /// </summary>
        public void Clear()
        {
            _results.Clear();
            _sorted = false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public List<PickResult>.Enumerator GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<PickResult> IEnumerable<PickResult>.GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        /// <summary>
        /// Sorts based on bounding intersections
        /// </summary>
        private sealed class BoundingPickComparer : IComparer<PickResult>
        {
            /// <summary>
            /// Compares two pick results
            /// </summary>
            /// <param name="x">First pick result</param>
            /// <param name="y">Second pick result</param>
            /// <returns>0 if both values are identical, -1 if x is closer than y, 1 if y is closer than x</returns>
            public int Compare(PickResult x, PickResult y)
            {
                x.GetBoundingIntersection(out BoundingIntersectionResult result1);
                y.GetBoundingIntersection(out BoundingIntersectionResult result2);

                if (result1.IntersectionCount > 0 && result2.IntersectionCount > 0)
                {
                    float dist1 = result1.ClosestIntersection.Value.Distance;
                    float dist2 = result2.ClosestIntersection.Value.Distance;

                    if (MathHelper.IsApproxEquals(dist1, dist2))
                    {
                        return 0;
                    }

                    if (dist1 < dist2)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (result1.IntersectionCount > 0)
                {
                    return -1;
                }
                else if (result2.IntersectionCount > 0)
                {
                    return 1;
                }

                return 0;
            }
        }

        /// <summary>
        /// Sorts based on the closest primitive intersection, or falls back to bounding pick comparison
        /// </summary>
        private sealed class PrimitivePickComparer : IComparer<PickResult>
        {
            /// <summary>
            /// Compares two pick results
            /// </summary>
            /// <param name="x">First pick result</param>
            /// <param name="y">Second pick result</param>
            /// <returns>0 if both values are identical, -1 if x is closer than y, 1 if y is closer than x</returns>
            public int Compare(PickResult x, PickResult y)
            {
                bool hasResult1 = x.GetClosestIntersection(out LineIntersectionResult result1);
                bool hasResult2 = y.GetClosestIntersection(out LineIntersectionResult result2);

                if (hasResult1 && hasResult2)
                {
                    float dist1 = result1.Distance;
                    float dist2 = result2.Distance;

                    if (MathHelper.IsApproxEquals(dist1, dist2))
                    {
                        return 0;
                    }

                    if (dist1 < dist2)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

                return BoundingPickDistanceComparer.Compare(x, y);
            }
        }
    }
}
