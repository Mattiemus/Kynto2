namespace Spark.Networking
{
    public static class NetworkConstants
    {
        public const int MaxPacketSize = 1500;

        public const int MaxFrameDataSize = 255;

        public const byte ProtocolMajorVersion = 0;

        public const byte ProtocolMinorVersion = 1;

        public const uint ProtocolSourceRevision = 5;

        public const int TimeOutSeconds = 5;

        public const int MaxIdleTimeSeconds = 3;

        public const int MaxAcknowledgeWaitTimeMilliSeconds = 500;

        public const int MaxResendCount = 10;

        public const int MaxPacketsWaitingAcknowledge = 10000;

        public const int MaxPacketsBeingFragmented = 100;

        public const int RegionToRegionConnectDelaySeconds = 3;

        public const int RegionToRegionDisconnectDelaySeconds = 3;

        public const int DefaultServerPort = 1253;

        public const int DefaultHubPort = 1254;
    }
}
