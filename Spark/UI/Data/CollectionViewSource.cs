namespace Spark.UI.Data
{
    using System.Collections;
    using System.Runtime.CompilerServices;

    public class CollectionViewSource : DependencyObject
    {
        private static ConditionalWeakTable<object, ICollectionView> DefaultViews =
            new ConditionalWeakTable<object, ICollectionView>();

        public static ICollectionView GetDefaultView(object source)
        {
            ICollectionView result = null;
            if (source != null && !DefaultViews.TryGetValue(source, out result))
            {
                IList list = source as IList;
                IEnumerable enumerable = source as IEnumerable;

                if (list != null)
                {
                    result = new ListCollectionView(list);
                }
                else if (enumerable != null)
                {
                    result = new EnumerableCollectionView(enumerable);
                }

                if (result != null)
                {
                    DefaultViews.Add(source, result);
                }
            }

            return result;
        }
    }
}
