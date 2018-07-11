namespace Spark.UI.Media
{
    using System;
    using System.Collections.Generic;

    using Math;

    public static class VisualTreeHelper
    {
        public static DependencyObject GetChild(DependencyObject reference, int childIndex)
        {
            return GetVisual(reference).GetVisualChild(childIndex);
        }

        public static int GetChildrenCount(DependencyObject reference)
        {
            return GetVisual(reference).VisualChildrenCount;
        }

        public static IEnumerable<DependencyObject> GetChildren(DependencyObject reference)
        {
            int count = GetChildrenCount(reference);
            for (int i = 0; i < count; ++i)
            {
                yield return GetChild(reference, i);
            }
        }

        public static Vector2 GetOffset(Visual reference)
        {
            return GetVisual(reference).VisualOffset;
        }

        public static DependencyObject GetParent(DependencyObject reference)
        {
            return GetVisual(reference).VisualParent;
        }

        public static T GetAncestor<T>(DependencyObject target) where T : DependencyObject
        {
            DependencyObject o = GetParent(target);

            while (o != null)
            {
                if (o is T)
                {
                    return (T)o;
                }

                o = GetParent(o);
            }

            return null;
        }

        public static IEnumerable<DependencyObject> GetAncestors(DependencyObject dependencyObject)
        {
            dependencyObject = GetParent(dependencyObject);

            while (dependencyObject != null)
            {
                yield return dependencyObject;
                dependencyObject = GetParent(dependencyObject);
            }
        }

        public static IEnumerable<T> GetDescendents<T>(DependencyObject reference) where T : DependencyObject
        {
            foreach (DependencyObject child in GetChildren(reference))
            {
                if (child is T)
                {
                    yield return (T)child;
                }

                foreach (T descendent in GetDescendents<T>(child))
                {
                    yield return descendent;
                }
            }
        }

        public static Transform GetTransform(Visual reference)
        {
            return GetVisual(reference).VisualTransform;
        }

        internal static ITopLevelElement GetTopLevelElement(DependencyObject target)
        {
            DependencyObject o = target;
            while (o != null)
            {
                if (o is ITopLevelElement topLevelElement)
                {
                    return topLevelElement;
                }

                o = GetParent(o);
            }

            return null;
        }

        private static Visual GetVisual(DependencyObject reference)
        {
            Visual visual = reference as Visual;
            if (visual == null)
            {
                throw new ArgumentException("Object is not a Visual.", nameof(reference));
            }

            return visual;
        }
    }
}
