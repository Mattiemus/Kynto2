namespace Spark.Networking
{
    using System;
    using System.Net;

    public sealed class SessionKey : IEquatable<SessionKey>
    {
        public SessionKey(IPEndPoint remoteEndPoint, uint incomingSessionId)
        {
            RemoteEndPoint = remoteEndPoint;
            IncomingSessionId = incomingSessionId;
        }

        public SessionKey(SessionKey Key)
        {
            RemoteEndPoint = Key.RemoteEndPoint;
            IncomingSessionId = Key.IncomingSessionId;
        }

        public IPEndPoint RemoteEndPoint { get; }

        public uint IncomingSessionId { get; }

        public override int GetHashCode()
        {
            return RemoteEndPoint.GetHashCode() + IncomingSessionId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SessionKey other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(SessionKey other)
        {
            if (other == null)
            {
                return false;
            }

            return RemoteEndPoint.Equals(other.RemoteEndPoint) && IncomingSessionId == other.IncomingSessionId;
        }
    }
}
