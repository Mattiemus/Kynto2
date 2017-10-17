namespace Kynto.Input
{
    using System;

    /// <summary>
    /// Abstract manager for handling cursor changes. Implementations can hook into the cursor change event to do the
    /// actual cursor setup, identified by named values.
    /// </summary>
    public static class Cursors
    {
        private static string _activeCursor;

        /// <summary>
        /// Default cursor name. Usually a pointer.
        /// </summary>
        public static readonly string Default = "Default";

        /// <summary>
        /// Arrow cursor name.
        /// </summary>
        public static readonly string Arrow = "Arrow";

        /// <summary>
        /// Cross cursor name.
        /// </summary>
        public static readonly string Cross = "Cross";

        /// <summary>
        /// Open hand cursor name.
        /// </summary>
        public static readonly string Hand = "Hand";

        /// <summary>
        /// Closed hand cursor name.
        /// </summary>
        public static readonly string ClosedHand = "ClosedHand";

        /// <summary>
        /// Horizontal split cursor name.
        /// </summary>
        public static readonly string HSplit = "HSplit";

        /// <summary>
        /// Vertical split cursor name.
        /// </summary>
        public static readonly string VSplit = "VSplit";

        /// <summary>
        /// Pan cursor name.
        /// </summary>
        public static readonly string Pan = "Pan";

        /// <summary>
        /// Pan east cursor name.
        /// </summary>
        public static readonly string PanEast = "PanEast";

        /// <summary>
        /// Pan north-east cursor name.
        /// </summary>
        public static readonly string PanNE = "PanNE";

        /// <summary>
        /// Pan north cursor name.
        /// </summary>
        public static readonly string PanNorth = "PanNorth";

        /// <summary>
        /// Pan north-west cursor name.
        /// </summary>
        public static readonly string PanNW = "PanNW";

        /// <summary>
        /// Pan south-east cursor name.
        /// </summary>
        public static readonly string PanSE = "PanSE";

        /// <summary>
        /// Pan south cursor name.
        /// </summary>
        public static readonly string PanSouth = "PanSouth";

        /// <summary>
        /// Pan south-west cursor name.
        /// </summary>
        public static readonly string PanSW = "PanSW";

        /// <summary>
        /// Pan west cursor name.
        /// </summary>
        public static readonly string PanWest = "PanWest";

        /// <summary>
        /// Pan size all cursor name.
        /// </summary>
        public static readonly string SizeAll = "SizeAll";

        /// <summary>
        /// Wait cursor name.
        /// </summary>
        public static readonly string Wait = "Wait";

        /// <summary>
        /// Rotate cursor name.
        /// </summary>
        public static readonly string Rotate = "Rotate";

        /// <summary>
        /// Occurs when the cursor name changes.
        /// </summary>
        public static event EventHandler<string> CursorChanged;

        /// <summary>
        /// Get or set the current cursor name.
        /// </summary>
        public static string Cursor
        {
            get => _activeCursor;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _activeCursor = Default;
                }
                else
                {
                    _activeCursor = value;
                }

                CursorChanged?.Invoke(null, _activeCursor);
            }
        }
    }
}
