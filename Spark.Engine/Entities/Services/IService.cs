namespace Spark.Engine
{
    using System;

    public interface IService : IDisposable
    {
        string Name { get; }

        bool IsInitialized { get; }

        void Initialize();
    }
}
