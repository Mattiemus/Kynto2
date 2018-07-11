namespace Spark.UI.Media
{
    using System;

    using Math;

    /// <summary>
    /// Base class for objects in the visual tree.
    /// </summary>
    public class Visual : DependencyObject
    {
        /// <summary>
        /// Gets the number of child controls in the visual tree.
        /// </summary>
        protected internal virtual int VisualChildrenCount => 0;

        /// <summary>
        /// Gets the position of the visual within it's parent visual.
        /// </summary>
        protected internal Vector2 VisualOffset { get; protected set; }

        /// <summary>
        /// Gets the parent control in the visual tree.
        /// </summary>
        protected internal DependencyObject VisualParent { get; protected set; }

        /// <summary>
        /// Gets or sets a transform that can be applied to the visual.
        /// </summary>
        protected internal Transform VisualTransform { get; protected set; }

        /// <summary>
        /// Converts a point in this Visual to screen coordinates.
        /// </summary>
        /// <param name="point">The client coordinate.</param>
        /// <returns>The screen coordinate.</returns>
        public Vector2 PointToScreen(Vector2 point)
        {
            foreach (Visual v in VisualTreeHelper.GetAncestors(this))
            {
                point += v.VisualOffset;

                ITopLevelElement topLevelElement = v as ITopLevelElement;

                if (topLevelElement != null)
                {
                    point += (Vector2)topLevelElement.PresentationSource.PointToScreen(Vector2.Zero);
                }
            }

            return point;
        }

        /// <summary>
        /// When overridden in a derived class, returns the bounds of the control for use in hit testing.
        /// </summary>
        /// <returns>The hit test bounds.</returns>
        internal virtual RectangleF GetHitTestBounds()
        {
            return RectangleF.Empty;
        }

        /// <summary>
        /// Gets the child in the visual tree with the requested index.
        /// </summary>
        /// <param name="index">The index. Should be from 0 to <see cref="VisualChildrenCount"/>.</param>
        /// <returns>The child visual.</returns>
        protected internal virtual Visual GetVisualChild(int index)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        /// <summary>
        /// Adds a child <see cref="Visual"/> to the visual tree.
        /// </summary>
        /// <param name="child">The child visual.</param>
        protected internal void AddVisualChild(Visual child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            DependencyObject oldParent = child.VisualParent;
            child.DependencyParent = this;
            child.VisualParent = this;
            child.OnVisualParentChanged(oldParent);
        }

        /// <summary>
        /// Called when the visual parent changes.
        /// </summary>
        /// <param name="oldParent">The old visual parent.</param>
        protected internal virtual void OnVisualParentChanged(DependencyObject oldParent)
        {
        }

        /// <summary>
        /// Removes a child <see cref="Visual"/> from the visual tree.
        /// </summary>
        /// <param name="child">The child visual.</param>
        protected internal void RemoveVisualChild(Visual child)
        {
            child.VisualParent = null;
            child.DependencyParent = null;
        }

        /// <summary>
        /// Determines whether a point is within the bounds of the visual.
        /// </summary>
        /// <param name="hitTestParameters">The point to test.</param>
        /// <returns>
        /// A <see cref="PointHitTestResult"/> if the point was within the bounds; otherwise null.
        /// </returns>
        protected virtual HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return GetHitTestBounds().Contains(hitTestParameters.HitPoint) ? new PointHitTestResult(this, hitTestParameters.HitPoint) : null;
        }
    }
}
