namespace Spark.Networking.Messages
{
    using System;

    [Flags]
    public enum MessageFlags
    {
        None = 0,
        Guaranteed = 1,
        Control = 2,
    }
}
