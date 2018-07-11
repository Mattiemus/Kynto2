namespace Spark.UI.Data
{
    using System.Collections;

    public class ListCollectionView : CollectionView
    {
        public ListCollectionView(IList list)
            : base(list)
        {
        }
    }
}
