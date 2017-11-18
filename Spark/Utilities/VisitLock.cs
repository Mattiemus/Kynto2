namespace Spark.Utilities
{
    using System.Threading;

    /// <summary>
    /// A lock that is useful to guarantee an object is processed only once when being visited by multiple threads (e.g. first one wins, the rest see the lock as already marked and so ignore the object). 
    /// This requires each "round" of processing to have a globally unique sequence number (e.g. a render frame # or update # in a game). The sequence number that was used to take the lock is saved, so it is important to avoid copying-by-value of the lock.
    /// </summary>
    public struct VisitLock
    {
        private int _beingVisited;
        private int _lastVisitSequence;

        /// <summary>
        /// Determines if the lock has been visited during the current sequence. 
        /// </summary>
        /// <param name="currentVisitSequence">The current globally unique visit number.</param>
        /// <returns>True if this is the first time the lock has been visited, false if otherwise.</returns>
        public bool IsFirstVisit(int currentVisitSequence)
        {
            // Take the "lock", if it's already been taken it's being visited so we don't need to do anything
            if (Interlocked.Exchange(ref _beingVisited, 1) == 0)
            {
                // If this has been visited during this visitation run, then we don't do anything
                bool isFirstVisit = false;
                if (_lastVisitSequence != currentVisitSequence)
                {
                    // If not, then set the last visit run to the current
                    isFirstVisit = true;
                    _lastVisitSequence = currentVisitSequence;
                }

                // Release the "lock"
                Interlocked.Exchange(ref _beingVisited, 0);
                return isFirstVisit;
            }

            return false;
        }
    }
}
