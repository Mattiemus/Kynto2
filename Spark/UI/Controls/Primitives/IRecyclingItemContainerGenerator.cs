namespace Spark.UI.Controls.Primitives
{
    public interface IRecyclingItemContainerGenerator : IItemContainerGenerator
    {
        void Recycle(GeneratorPosition position, int count);
    }
}
