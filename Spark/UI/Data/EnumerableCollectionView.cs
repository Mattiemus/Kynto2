namespace Spark.UI.Data
{
    using System.Collections;

    public class EnumerableCollectionView : CollectionView
    {
        public EnumerableCollectionView(IEnumerable collection)
            : base(collection)
        {
        }
    }
}
