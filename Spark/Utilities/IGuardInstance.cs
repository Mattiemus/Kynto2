namespace Spark.Utilities
{
    public interface IGuardInstance
    {
        void NullArgument(object value, string argumentName);

        void NullOrEmptyArgument(string value, string argumentName);
    }
}
