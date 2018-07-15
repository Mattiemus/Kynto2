namespace Spark.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Math;
    using Media;

    public class Grid : Panel
    {
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached(
                "Column",
                typeof(int),
                typeof(Grid));

        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached(
                "ColumnSpan",
                typeof(int),
                typeof(Grid),
                new PropertyMetadata(1));

        public static readonly DependencyProperty IsSharedSizeScopeProperty =
            DependencyProperty.RegisterAttached(
                "IsSharedSizeScope",
                typeof(bool),
                typeof(Grid));

        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached(
                "Row",
                typeof(int),
                typeof(Grid));

        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached(
                "RowSpan",
                typeof(int),
                typeof(Grid),
                new PropertyMetadata(1));

        private Segment[,] _rowMatrix;
        private Segment[,] _colMatrix;

        public Grid()
        {
            ColumnDefinitions = new ColumnDefinitionCollection();
            RowDefinitions = new RowDefinitionCollection();
        }

        public ColumnDefinitionCollection ColumnDefinitions { get; private set; }

        public RowDefinitionCollection RowDefinitions { get; private set; }

        public static int GetColumn(UIElement element)
        {
            return (int)element.GetValue(ColumnProperty);
        }

        public static int GetColumnSpan(UIElement element)
        {
            return (int)element.GetValue(ColumnSpanProperty);
        }

        public static int GetRow(UIElement element)
        {
            return (int)element.GetValue(RowProperty);
        }

        public static int GetRowSpan(UIElement element)
        {
            return (int)element.GetValue(RowSpanProperty);
        }

        public static void SetColumn(UIElement element, int value)
        {
            element.SetValue(ColumnProperty, value);
        }

        public static void SetColumnSpan(UIElement element, int value)
        {
            element.SetValue(ColumnSpanProperty, value);
        }

        public static void SetRow(UIElement element, int value)
        {
            element.SetValue(RowProperty, value);
        }

        public static void SetRowSpan(UIElement element, int value)
        {
            element.SetValue(RowSpanProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size totalSize = constraint;
            int colCount = ColumnDefinitions.Count;
            int rowCount = RowDefinitions.Count;
            Size totalStars = new Size(0, 0);

            bool emptyRows = rowCount == 0;
            bool emptyCols = colCount == 0;
            bool hasChildren = InternalChildren.Count > 0;

            if (emptyRows)
            {
                rowCount = 1;
            }

            if (emptyCols)
            {
                colCount = 1;
            }

            CreateMatrices(rowCount, colCount);

            if (emptyRows)
            {
                _rowMatrix[0, 0] = new Segment(0.0f, 0.0f, float.PositiveInfinity, GridUnitType.Star);
                _rowMatrix[0, 0].Stars = 1.0f;
                totalStars.Height += 1.0f;
            }
            else
            {
                for (int i = 0; i < rowCount; i++)
                {
                    RowDefinition rowdef = RowDefinitions[i];
                    GridLength height = rowdef.Height;

                    rowdef.ActualHeight = float.PositiveInfinity;
                    _rowMatrix[i, i] = new Segment(0.0f, rowdef.MinHeight, rowdef.MaxHeight, height.GridUnitType);

                    if (height.GridUnitType == GridUnitType.Pixel)
                    {
                        _rowMatrix[i, i].OfferedSize = MathHelper.Clamp(height.Value, _rowMatrix[i, i].Min, _rowMatrix[i, i].Max);
                        _rowMatrix[i, i].DesiredSize = _rowMatrix[i, i].OfferedSize;
                        rowdef.ActualHeight = _rowMatrix[i, i].OfferedSize;
                    }
                    else if (height.GridUnitType == GridUnitType.Star)
                    {
                        _rowMatrix[i, i].Stars = height.Value;
                        totalStars.Height += height.Value;
                    }
                    else if (height.GridUnitType == GridUnitType.Auto)
                    {
                        _rowMatrix[i, i].OfferedSize = MathHelper.Clamp(0.0f, _rowMatrix[i, i].Min, _rowMatrix[i, i].Max);
                        _rowMatrix[i, i].DesiredSize = _rowMatrix[i, i].OfferedSize;
                    }
                }
            }

            if (emptyCols)
            {
                _colMatrix[0, 0] = new Segment(0.0f, 0.0f, float.PositiveInfinity, GridUnitType.Star);
                _colMatrix[0, 0].Stars = 1.0f;
                totalStars.Width += 1.0f;
            }
            else
            {
                for (int i = 0; i < colCount; i++)
                {
                    ColumnDefinition coldef = ColumnDefinitions[i];
                    GridLength width = coldef.Width;

                    coldef.ActualWidth = float.PositiveInfinity;
                    _colMatrix[i, i] = new Segment(0.0f, coldef.MinWidth, coldef.MaxWidth, width.GridUnitType);

                    if (width.GridUnitType == GridUnitType.Pixel)
                    {
                        _colMatrix[i, i].OfferedSize = MathHelper.Clamp(width.Value, _colMatrix[i, i].Min, _colMatrix[i, i].Max);
                        _colMatrix[i, i].DesiredSize = _colMatrix[i, i].OfferedSize;
                        coldef.ActualWidth = _colMatrix[i, i].OfferedSize;
                    }
                    else if (width.GridUnitType == GridUnitType.Star)
                    {
                        _colMatrix[i, i].Stars = width.Value;
                        totalStars.Width += width.Value;
                    }
                    else if (width.GridUnitType == GridUnitType.Auto)
                    {
                        _colMatrix[i, i].OfferedSize = MathHelper.Clamp(0.0f, _colMatrix[i, i].Min, _colMatrix[i, i].Max);
                        _colMatrix[i, i].DesiredSize = _colMatrix[i, i].OfferedSize;
                    }
                }
            }

            List<GridNode> sizes = new List<GridNode>();
            GridNode node;
            GridNode separator = new GridNode(null, 0, 0, 0.0f);
            int separatorIndex;

            sizes.Add(separator);

            // Pre-process the grid children so that we know what types of elements we have so
            // we can apply our special measuring rules.
            GridWalker gridWalker = new GridWalker(this, _rowMatrix, _colMatrix);

            for (int i = 0; i < 6; i++)
            {
                // These bools tell us which grid element type we should be measuring. i.e.
                // 'star/auto' means we should measure elements with a star row and auto col
                bool autoAuto = i == 0;
                bool starAuto = i == 1;
                bool autoStar = i == 2;
                bool starAutoAgain = i == 3;
                bool nonStar = i == 4;
                bool remainingStar = i == 5;

                if (hasChildren)
                {
                    ExpandStarCols(totalSize);
                    ExpandStarRows(totalSize);
                }

                foreach (UIElement child in VisualTreeHelper.GetChildren(this))
                {
                    int col, row;
                    int colspan, rowspan;
                    Size childSize = new Size(0, 0);
                    bool starCol = false;
                    bool starRow = false;
                    bool autoCol = false;
                    bool autoRow = false;

                    col = Math.Min(GetColumn(child), colCount - 1);
                    row = Math.Min(GetRow(child), rowCount - 1);
                    colspan = Math.Min(GetColumnSpan(child), colCount - col);
                    rowspan = Math.Min(GetRowSpan(child), rowCount - row);

                    for (int r = row; r < row + rowspan; r++)
                    {
                        starRow |= _rowMatrix[r, r].Type == GridUnitType.Star;
                        autoRow |= _rowMatrix[r, r].Type == GridUnitType.Auto;
                    }

                    for (int c = col; c < col + colspan; c++)
                    {
                        starCol |= _colMatrix[c, c].Type == GridUnitType.Star;
                        autoCol |= _colMatrix[c, c].Type == GridUnitType.Auto;
                    }

                    // This series of if statements checks whether or not we should measure
                    // the current element and also if we need to override the sizes
                    // passed to the Measure call. 

                    // If the element has Auto rows and Auto columns and does not span Star
                    // rows/cols it should only be measured in the auto_auto phase.
                    // There are similar rules governing auto/star and star/auto elements.
                    // NOTE: star/auto elements are measured twice. The first time with
                    // an override for height, the second time without it.
                    if (autoRow && autoCol && !starRow && !starCol)
                    {
                        if (!autoAuto)
                        {
                            continue;
                        }

                        childSize.Width = float.PositiveInfinity;
                        childSize.Height = float.PositiveInfinity;
                    }
                    else if (starRow && autoCol && !starCol)
                    {
                        if (!(starAuto || starAutoAgain))
                        {
                            continue;
                        }

                        if (starAuto && gridWalker.HasAutoStar)
                        {
                            childSize.Height = float.PositiveInfinity;
                        }

                        childSize.Width = float.PositiveInfinity;
                    }
                    else if (autoRow && starCol && !starRow)
                    {
                        if (!autoStar)
                        {
                            continue;
                        }

                        childSize.Height = float.PositiveInfinity;
                    }
                    else if ((autoRow || autoCol) && !(starRow || starCol))
                    {
                        if (!nonStar)
                        {
                            continue;
                        }

                        if (autoRow)
                        {
                            childSize.Height = float.PositiveInfinity;
                        }

                        if (autoCol)
                        {
                            childSize.Width = float.PositiveInfinity;
                        }
                    }
                    else if (!(starRow || starCol))
                    {
                        if (!nonStar)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!remainingStar)
                        {
                            continue;
                        }
                    }

                    for (int r = row; r < row + rowspan; r++)
                    {
                        childSize.Height += _rowMatrix[r, r].OfferedSize;
                    }

                    for (int c = col; c < col + colspan; c++)
                    {
                        childSize.Width += _colMatrix[c, c].OfferedSize;
                    }

                    child.Measure(childSize);
                    Size desired = child.DesiredSize;

                    // Elements distribute their height based on two rules:
                    // 1) Elements with rowspan/colspan == 1 distribute their height first
                    // 2) Everything else distributes in a LIFO manner.
                    // As such, add all UIElements with rowspan/colspan == 1 after the separator in
                    // the list and everything else before it. Then to process, just keep popping
                    // elements off the end of the list.
                    if (!starAuto)
                    {
                        node = new GridNode(_rowMatrix, row + rowspan - 1, row, desired.Height);
                        separatorIndex = sizes.IndexOf(separator);
                        sizes.Insert(node.Row == node.Column ? separatorIndex + 1 : separatorIndex, node);
                    }

                    node = new GridNode(_colMatrix, col + colspan - 1, col, desired.Width);

                    separatorIndex = sizes.IndexOf(separator);
                    sizes.Insert(node.Row == node.Column ? separatorIndex + 1 : separatorIndex, node);
                }

                sizes.Remove(separator);

                while (sizes.Count > 0)
                {
                    node = sizes.Last();
                    node.Matrix[node.Row, node.Column].DesiredSize = Math.Max(node.Matrix[node.Row, node.Column].DesiredSize, node.Size);
                    AllocateDesiredSize(rowCount, colCount);
                    sizes.Remove(node);
                }

                sizes.Add(separator);
            }

            // Once we have measured and distributed all sizes, we have to store
            // the results. Every time we want to expand the rows/cols, this will
            // be used as the baseline.
            SaveMeasureResults();

            sizes.Remove(separator);

            Size gridSize = new Size(0.0f, 0.0f);

            for (int c = 0; c < colCount; c++)
            {
                gridSize.Width += _colMatrix[c, c].DesiredSize;
            }

            for (int r = 0; r < rowCount; r++)
            {
                gridSize.Height += _rowMatrix[r, r].DesiredSize;
            }

            return gridSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int colCount = ColumnDefinitions.Count;
            int rowCount = RowDefinitions.Count;
            int colMatrixDim = _colMatrix.GetUpperBound(0) + 1;
            int rowMatrixDim = _rowMatrix.GetUpperBound(0) + 1;

            RestoreMeasureResults();

            Size totalConsumed = new Size(0.0f, 0.0f);

            for (int c = 0; c < colMatrixDim; c++)
            {
                _colMatrix[c, c].OfferedSize = _colMatrix[c, c].DesiredSize;
                totalConsumed.Width += _colMatrix[c, c].OfferedSize;
            }

            for (int r = 0; r < rowMatrixDim; r++)
            {
                _rowMatrix[r, r].OfferedSize = _rowMatrix[r, r].DesiredSize;
                totalConsumed.Height += _rowMatrix[r, r].OfferedSize;
            }

            if (!MathHelper.IsApproxEquals(totalConsumed.Width, finalSize.Width))
            {
                ExpandStarCols(finalSize);
            }

            if (!MathHelper.IsApproxEquals(totalConsumed.Height, finalSize.Height))
            {
                ExpandStarRows(finalSize);
            }

            for (int c = 0; c < colCount; c++)
            {
                ColumnDefinitions[c].ActualWidth = _colMatrix[c, c].OfferedSize;
            }

            for (int r = 0; r < rowCount; r++)
            {
                RowDefinitions[r].ActualHeight = _rowMatrix[r, r].OfferedSize;
            }

            foreach (UIElement child in VisualTreeHelper.GetChildren(this))
            {
                int col = Math.Min(GetColumn(child), colMatrixDim - 1);
                int row = Math.Min(GetRow(child), rowMatrixDim - 1);
                int colspan = Math.Min(GetColumnSpan(child), colMatrixDim - col);
                int rowspan = Math.Min(GetRowSpan(child), rowMatrixDim - row);

                RectangleF childFinal = new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);

                for (int c = 0; c < col; c++)
                {
                    childFinal.X += _colMatrix[c, c].OfferedSize;
                }

                for (int c = col; c < col + colspan; c++)
                {
                    childFinal.Width += _colMatrix[c, c].OfferedSize;
                }

                for (int r = 0; r < row; r++)
                {
                    childFinal.Y += _rowMatrix[r, r].OfferedSize;
                }

                for (int r = row; r < row + rowspan; r++)
                {
                    childFinal.Height += _rowMatrix[r, r].OfferedSize;
                }

                child.Arrange(childFinal);
            }

            return finalSize;
        }

        private void CreateMatrices(int rowCount, int colCount)
        {
            if (_rowMatrix == null || _colMatrix == null ||
                _rowMatrix.GetUpperBound(0) != rowCount - 1 ||
                _colMatrix.GetUpperBound(0) != colCount - 1)
            {
                _rowMatrix = new Segment[rowCount, rowCount];
                _colMatrix = new Segment[colCount, colCount];
            }
        }

        private void ExpandStarCols(Size availableSize)
        {
            int columnsCount = ColumnDefinitions.Count;

            for (int i = 0; i < _colMatrix.GetUpperBound(0) + 1; i++)
            {
                if (_colMatrix[i, i].Type == GridUnitType.Star)
                {
                    _colMatrix[i, i].OfferedSize = 0;
                }
                else
                {
                    availableSize.Width = Math.Max(availableSize.Width - _colMatrix[i, i].OfferedSize, 0);
                }
            }

            float width = availableSize.Width;
            AssignSize(_colMatrix, 0, _colMatrix.GetUpperBound(0), ref width, GridUnitType.Star, false);
            availableSize.Width = Math.Max(0.0f, width);

            if (columnsCount > 0)
            {
                for (int i = 0; i < _colMatrix.GetUpperBound(0) + 1; i++)
                {
                    if (_colMatrix[i, i].Type == GridUnitType.Star)
                    {
                        ColumnDefinitions[i].ActualWidth = _colMatrix[i, i].OfferedSize;
                    }
                }
            }
        }

        private void ExpandStarRows(Size availableSize)
        {
            int rowCount = RowDefinitions.Count;

            // When expanding star rows, we need to zero out their height before
            // calling AssignSize. AssignSize takes care of distributing the 
            // available size when there are Mins and Maxs applied.
            for (int i = 0; i < _rowMatrix.GetUpperBound(0) + 1; i++)
            {
                if (_rowMatrix[i, i].Type == GridUnitType.Star)
                {
                    _rowMatrix[i, i].OfferedSize = 0.0f;
                }
                else
                {
                    availableSize.Height = Math.Max(availableSize.Height - _rowMatrix[i, i].OfferedSize, 0);
                }
            }

            float height = availableSize.Height;
            AssignSize(_rowMatrix, 0, _rowMatrix.GetUpperBound(0), ref height, GridUnitType.Star, false);
            availableSize.Height = height;

            if (rowCount > 0)
            {
                for (int i = 0; i < _rowMatrix.GetUpperBound(0) + 1; i++)
                {
                    if (_rowMatrix[i, i].Type == GridUnitType.Star)
                    {
                        RowDefinitions[i].ActualHeight = _rowMatrix[i, i].OfferedSize;
                    }
                }
            }
        }

        private void AssignSize(
            Segment[,] matrix,
            int start,
            int end,
            ref float size,
            GridUnitType type,
            bool desiredSize)
        {
            float count = 0;
            bool assigned;

            // Count how many segments are of the correct type. If we're measuring Star rows/cols
            // we need to count the number of stars instead.
            for (int i = start; i <= end; i++)
            {
                float segmentSize = desiredSize ? matrix[i, i].DesiredSize : matrix[i, i].OfferedSize;
                if (segmentSize < matrix[i, i].Max)
                {
                    count += type == GridUnitType.Star ? matrix[i, i].Stars : 1;
                }
            }

            do
            {
                float contribution = size / count;

                assigned = false;

                for (int i = start; i <= end; i++)
                {
                    float segmentSize = desiredSize ? matrix[i, i].DesiredSize : matrix[i, i].OfferedSize;

                    if (!(matrix[i, i].Type == type && segmentSize < matrix[i, i].Max))
                    {
                        continue;
                    }

                    float newsize = segmentSize;
                    newsize += contribution * (type == GridUnitType.Star ? matrix[i, i].Stars : 1);
                    newsize = Math.Min(newsize, matrix[i, i].Max);
                    assigned |= newsize > segmentSize;
                    size -= newsize - segmentSize;

                    if (desiredSize)
                    {
                        matrix[i, i].DesiredSize = newsize;
                    }
                    else
                    {
                        matrix[i, i].OfferedSize = newsize;
                    }
                }
            }
            while (assigned);
        }

        private void AllocateDesiredSize(int rowCount, int colCount)
        {
            // First allocate the heights of the RowDefinitions, then allocate
            // the widths of the ColumnDefinitions.
            for (int i = 0; i < 2; i++)
            {
                Segment[,] matrix = i == 0 ? _rowMatrix : _colMatrix;
                int count = i == 0 ? rowCount : colCount;

                for (int row = count - 1; row >= 0; row--)
                {
                    for (int col = row; col >= 0; col--)
                    {
                        bool spansStar = false;
                        for (int j = row; j >= col; j--)
                        {
                            spansStar |= matrix[j, j].Type == GridUnitType.Star;
                        }

                        // This is the amount of pixels which must be available between the grid rows
                        // at index 'col' and 'row'. i.e. if 'row' == 0 and 'col' == 2, there must
                        // be at least 'matrix [row][col].size' pixels of height allocated between
                        // all the rows in the range col -> row.
                        float current = matrix[row, col].DesiredSize;

                        // Count how many pixels have already been allocated between the grid rows
                        // in the range col -> row. The amount of pixels allocated to each grid row/column
                        // is found on the diagonal of the matrix.
                        float totalAllocated = 0.0f;

                        for (int k = row; k >= col; k--)
                        {
                            totalAllocated += matrix[k, k].DesiredSize;
                        }

                        // If the size requirement has not been met, allocate the additional required
                        // size between 'pixel' rows, then 'star' rows, finally 'auto' rows, until all
                        // height has been assigned.
                        if (totalAllocated < current)
                        {
                            float additional = current - totalAllocated;

                            if (spansStar)
                            {
                                AssignSize(matrix, col, row, ref additional, GridUnitType.Star, true);
                            }
                            else
                            {
                                AssignSize(matrix, col, row, ref additional, GridUnitType.Pixel, true);
                                AssignSize(matrix, col, row, ref additional, GridUnitType.Auto, true);
                            }
                        }
                    }
                }
            }

            for (int r = 0; r < _rowMatrix.GetUpperBound(0) + 1; r++)
            {
                _rowMatrix[r, r].OfferedSize = _rowMatrix[r, r].DesiredSize;
            }

            for (int c = 0; c < _colMatrix.GetUpperBound(0) + 1; c++)
            {
                _colMatrix[c, c].OfferedSize = _colMatrix[c, c].DesiredSize;
            }
        }

        private void SaveMeasureResults()
        {
            for (int i = 0; i < _rowMatrix.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < _rowMatrix.GetUpperBound(0) + 1; j++)
                {
                    _rowMatrix[i, j].OriginalSize = _rowMatrix[i, j].OfferedSize;
                }
            }

            for (int i = 0; i < _colMatrix.GetUpperBound(0); i++)
            {
                for (int j = 0; j < _colMatrix.GetUpperBound(0); j++)
                {
                    _colMatrix[i, j].OriginalSize = _colMatrix[i, j].OfferedSize;
                }
            }
        }

        private void RestoreMeasureResults()
        {
            for (int i = 0; i < _rowMatrix.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < _rowMatrix.GetUpperBound(0) + 1; j++)
                {
                    _rowMatrix[i, j].OfferedSize = _rowMatrix[i, j].OriginalSize;
                }
            }

            for (int i = 0; i < _colMatrix.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < _colMatrix.GetUpperBound(0) + 1; j++)
                {
                    _colMatrix[i, j].OfferedSize = _colMatrix[i, j].OriginalSize;
                }
            }
        }

        private struct Segment
        {
            public float OriginalSize;
            public float Max;
            public float Min;
            public float DesiredSize;
            public float OfferedSize;
            public float Stars;
            public GridUnitType Type;

            public Segment(float offeredSize, float min, float max, GridUnitType type)
            {
                OriginalSize = 0.0f;
                Min = min;
                Max = max;
                DesiredSize = 0.0f;
                OfferedSize = offeredSize;
                Stars = 0.0f;
                Type = type;
            }

            public void Init(float offeredSize, float min, float max, GridUnitType type)
            {
                OfferedSize = offeredSize;
                Min = min;
                Max = max;
                Type = type;
            }
        }

        private struct GridNode
        {
            public int Row;
            public int Column;
            public float Size;
            public Segment[,] Matrix;

            public GridNode(Segment[,] matrix, int row, int col, float size)
            {
                Matrix = matrix;
                Row = row;
                Column = col;
                Size = size;
            }
        }

        private class GridWalker
        {
            public GridWalker(Grid grid, Segment[,] rowMatrix, Segment[,] colMatrix)
            {
                foreach (UIElement child in VisualTreeHelper.GetChildren(grid))
                {
                    bool starCol = false;
                    bool starRow = false;
                    bool autoCol = false;
                    bool autoRow = false;

                    int col = Math.Min(GetColumn(child), colMatrix.GetUpperBound(0));
                    int row = Math.Min(GetRow(child), rowMatrix.GetUpperBound(0));
                    int colspan = Math.Min(GetColumnSpan(child), colMatrix.GetUpperBound(0));
                    int rowspan = Math.Min(GetRowSpan(child), rowMatrix.GetUpperBound(0));

                    for (int r = row; r < row + rowspan; r++)
                    {
                        starRow |= rowMatrix[r, r].Type == GridUnitType.Star;
                        autoRow |= rowMatrix[r, r].Type == GridUnitType.Auto;
                    }

                    for (int c = col; c < col + colspan; c++)
                    {
                        starCol |= colMatrix[c, c].Type == GridUnitType.Star;
                        autoCol |= colMatrix[c, c].Type == GridUnitType.Auto;
                    }

                    HasAutoAuto |= autoRow && autoCol && !starRow && !starCol;
                    HasStarAuto |= starRow && autoCol;
                    HasAutoStar |= autoRow && starCol;
                }
            }

            public bool HasAutoAuto { get; private set; }

            public bool HasStarAuto { get; private set; }

            public bool HasAutoStar { get; private set; }
        }
    }
}
