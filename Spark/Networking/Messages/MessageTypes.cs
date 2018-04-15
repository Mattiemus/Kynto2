namespace Spark.Networking.Messages
{
    public enum MessageTypes : ushort
    {
        Disconnect = 0x01,
        Acknowledge = 0x02,
        KeepAlive = 0x03,
        Throttle = 0x04,
    }
}
