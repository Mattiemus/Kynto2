namespace Spark.UI.Data
{
    using System.Collections;
    using System.Collections.Specialized;

    public interface ICollectionView : IEnumerable, INotifyCollectionChanged
    {
        IEnumerable SourceCollection { get; }
    }
}
