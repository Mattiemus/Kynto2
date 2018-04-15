namespace Spark.Networking.Messages
{
    using System;

    [Flags]
    public enum PacketFlags
    {
        None = 0,
        Guaranteed = 1
    }
}
