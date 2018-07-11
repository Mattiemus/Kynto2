namespace Spark.UI
{
    using Graphics;

    internal interface ITopLevelElement
    {
        PresentationSource PresentationSource { get; }

        void DoLayoutPass();
    }
}
