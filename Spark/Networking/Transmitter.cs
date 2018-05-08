namespace Spark.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Net;
    using System.Net.Sockets;
    using System.Diagnostics;

    using Messages;

    public class Transmitter
    {
        private Socket _udpSocket;
        private byte[] _receiveBuffer = new byte[NetworkConstants.MaxPacketSize];
        private Thread _receiveThread;
        private Thread _networkThread;
        private bool _requestThreadExit;
        private Queue<Session> _incomingPendingSessions = new Queue<Session>();
        private Dictionary<uint, Session> _outIdSessionDictionary = new Dictionary<uint, Session>();
        private Dictionary<SessionKey, Session> _inKeySessionDictionary = new Dictionary<SessionKey, Session>();
        private List<Session> _sessionsToRemove = new List<Session>();
        private object _sessionManagementLock = new object();
        private uint _sessionIdCounter;
        private ulong _lastBytesReceived;
        private ulong _lastBytesSent;

        public Transmitter()
        {
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            AcceptIncomingSessions = false;
            SendRateUpdateTime = DateTime.Now;
            SendRateTimeWindow = 0.2;
            ReceiveRateUpdateTime = DateTime.Now;
            ReceiveRateTimeWindow = 0.2;
            MaxSendRate = 100000;
        }

        public Transmitter(int port)
        {
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            AcceptIncomingSessions = true;
            SendRateUpdateTime = DateTime.Now;
            SendRateTimeWindow = 0.2;
            ReceiveRateUpdateTime = DateTime.Now;
            ReceiveRateTimeWindow = 0.2;
            MaxSendRate = 100000;
        }

        public bool IsAlive => _networkThread != null && _networkThread.IsAlive && _receiveThread != null && _receiveThread.IsAlive;

        public ulong PacketsSent { get; private set; }

        public ulong PacketsReceived { get; private set; }

        public ulong BytesSent { get; private set; }

        public ulong BytesReceived { get; private set; }

        public double MaxSendRate { get; set; }

        public double SendRate { get; private set; }

        public DateTime SendRateUpdateTime { get; private set; }

        public double SendRateTimeWindow { get; private set; }

        public double ReceiveRate { get; private set; }

        public DateTime ReceiveRateUpdateTime { get; private set; }

        public double ReceiveRateTimeWindow { get; private set; }

        public bool AcceptIncomingSessions { get; set; }

        public int PendingSessionCount => _incomingPendingSessions.Count;

        public void Startup()
        {
            if (_udpSocket == null)
            {
                throw new SparkNetworkException("Restart after stop is not supported.");
            }

            if (_networkThread != null)
            {
                throw new SparkNetworkException("Network thread already executing.");
            }

            if (_receiveThread != null)
            {
                throw new SparkNetworkException("Receiver thread already executing.");
            }

            _requestThreadExit = false;

            _networkThread = new Thread(new ThreadStart(NetworkThread));
            _networkThread.Name = "Network";
            _networkThread.Start();

            _receiveThread = new Thread(new ThreadStart(ReceiveThread));
            _receiveThread.Name = "NetworkReceive";
            _receiveThread.Start();
        }

        public void Shutdown()
        {
            _requestThreadExit = true;

            if (_networkThread != null)
            {
                _networkThread.Join();
            }
            _udpSocket.Close();

            if (_receiveThread != null)
            {
                _receiveThread.Join();
            }
            _udpSocket.Dispose();

            _networkThread = null;
            _receiveThread = null;
            _udpSocket = null;
        }

        private void ReceiveThread()
        {
            try
            {
                while (!_requestThreadExit)
                {
                    try
                    {
                        ReceiveMessages();

                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
            finally
            {
                Trace.WriteLine("Network receive thread exited.");
            }
        }

        private void ReceiveMessages()
        {
            IPEndPoint remoteEndPoint;
            int bytesReceived;
            try
            {
                EndPoint tempEndPoint = new IPEndPoint(IPAddress.Any, 0);
                bytesReceived = _udpSocket.ReceiveFrom(_receiveBuffer, NetworkConstants.MaxPacketSize, SocketFlags.None, ref tempEndPoint);
                remoteEndPoint = (IPEndPoint)tempEndPoint;
            }
            catch (SocketException)
            {
                return;
            }

            DateTime now = DateTime.Now;
            PacketsReceived++;
            BytesReceived += (ulong)bytesReceived;

            double receiveRateUpdateTimeDelta = now.Subtract(ReceiveRateUpdateTime).TotalSeconds;
            if (receiveRateUpdateTimeDelta > ReceiveRateTimeWindow)
            {
                ReceiveRate = (BytesReceived - _lastBytesReceived) / receiveRateUpdateTimeDelta;
                ReceiveRateUpdateTime = now;
                _lastBytesReceived = BytesReceived;
            }

            // TODO: drop packets with lowwer id when signal.
            // TODO: retain order with guaranteed packets.

            ByteReader reader = new ByteReader(_receiveBuffer, bytesReceived);
            Packet packet = new Packet();
            packet.Read(reader);
            Session session = GetSession(remoteEndPoint, packet.SessionId, packet);
            if (session == null)
            {
                return;
            }

            session.BytesReceived += (ulong)bytesReceived;
            double sessionReceiveRateUpdateTimeDelta = now.Subtract(session.ReceiveRateUpdateTime).TotalSeconds;
            if (sessionReceiveRateUpdateTimeDelta > session.ReceiveRateTimeWindow)
            {
                session.ReceiveRate = (session.BytesReceived - session._lastBytesReceived) / sessionReceiveRateUpdateTimeDelta;
                session.ReceiveRateUpdateTime = now;
                session._lastBytesReceived = BytesReceived;
            }

            if (session.ReceivedPackets.Contains(packet.PacketId))
            {
                Trace.WriteLine("Packet has been already received: " + packet.PacketId);
                session.AcknowledgePacket(packet.PacketId);
                return;
            }
            session.ReceivedPackets.Add(packet.PacketId);

            if (packet.Flags.HasFlag(PacketFlags.Guaranteed))
            {
                session.AcknowledgePacket(packet.PacketId);
            }

            foreach (Frame frame in packet.Frames)
            {
                MessageEntry messageEntry = session.GetPartialInboundMessage(frame.MessageId, frame.MessageType, frame.FrameCount);
                if (messageEntry.Frames[frame.FrameIndex] == null)
                {
                    messageEntry.Frames[frame.FrameIndex] = frame;
                    messageEntry.FramesCompleted++;

                    if (messageEntry.FramesCompleted == messageEntry.FramesCount)
                    {
                        session.CompletePartialInboundMessage(frame.MessageId, messageEntry);
                    }
                }
            }
        }

        private void NetworkThread()
        {
            try
            {
                while (!_requestThreadExit)
                {
                    try
                    {
                        ProcessControlMessages();
                        SendMessages();
                        CleanSessions();

                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
            finally
            {
                Trace.WriteLine("Network thread exited.");
            }
        }

        private void ProcessControlMessages()
        {
            foreach (Session session in _outIdSessionDictionary.Values)
            {
                DateTime now = DateTime.Now;

                if (session.IsConnected && now.Subtract(session.LastSendTime).TotalSeconds > NetworkConstants.MaxIdleTimeSeconds)
                {
                    session.Send(new KeepAliveMessage());
                }

                if (now.Subtract(session.LastReceiveTime).TotalSeconds > NetworkConstants.TimeOutSeconds)
                {
                    session.SessionState = SessionState.Disconnected;
                }

                if (session.AvailableControlMessages > 0)
                {
                    Message nextControlMessage = session.ReceiveControlMessage();
                    switch (nextControlMessage.Type)
                    {
                        case MessageTypes.Acknowledge:
                            AcknowledgeMessage acknowledgeMessage = (AcknowledgeMessage)nextControlMessage;
                            foreach (uint ackedPacketId in acknowledgeMessage.Acknowledges)
                            {
                                session.RemovePacketWaitingAcknowledge(ackedPacketId);
                            }
                            break;

                        case MessageTypes.Disconnect:
                            session.SessionState = SessionState.Disconnected;
                            break;

                        case MessageTypes.KeepAlive:
                            break;

                        default:
                            throw new SparkNetworkException($"Unknown control message type: {nextControlMessage.Type}");
                    }
                }
            }
        }

        private void SendMessages()
        {
            foreach (Session session in _outIdSessionDictionary.Values)
            {
                DateTime now = DateTime.Now;

                foreach (Packet packetToAck in session.PacketsWaitingAcknowledge)
                {
                    if (now.Subtract(packetToAck.LastSendTime).TotalMilliseconds > NetworkConstants.MaxAcknowledgeWaitTimeMilliSeconds)
                    {
                        if ((packetToAck.ResendCount + 1) > NetworkConstants.MaxResendCount)
                        {
                            session.SessionState = SessionState.Disconnected;
                            goto sendEnd;
                        }
                        else
                        {
                            packetToAck.LastSendTime = now;
                            packetToAck.ResendCount++;

                            ByteWriter writer = new ByteWriter(packetToAck.Size);
                            packetToAck.Write(writer);
                            _udpSocket.SendTo(writer.Bytes, packetToAck.Size, SocketFlags.None, session.RemoteEndPoint);

                            PacketsSent++;
                            BytesSent += (ulong)packetToAck.Size;
                            session.BytesSent += (ulong)packetToAck.Size;

                            Trace.WriteLine("Resending packet: " + packetToAck.PacketId + " for " + packetToAck.ResendCount + " time.");
                        }
                    }
                }

                double sendRateUpdateTimeDelta = now.Subtract(SendRateUpdateTime).TotalSeconds;
                if (sendRateUpdateTimeDelta > SendRateTimeWindow)
                {
                    SendRate = (BytesSent - _lastBytesSent) / sendRateUpdateTimeDelta;
                    SendRateUpdateTime = now;
                    _lastBytesSent = BytesSent;
                }

                double sessionSendRateUpdateTimeDelta = now.Subtract(session.SendRateUpdateTime).TotalSeconds;
                if (sendRateUpdateTimeDelta > session.SendRateTimeWindow)
                {
                    session.SendRate = (session.BytesSent - session._lastBytesSent) / sessionSendRateUpdateTimeDelta;
                    session.SendRateUpdateTime = now;
                    session._lastBytesSent = session.BytesSent;
                }

                if ((session.OutboundMessages > 0) || (session.PartiallySentMessageCount > 0))
                {
                    if (SendRate > (MaxSendRate * SendRateTimeWindow) ||
                        session.SendRate > (session.MaxSendRate * session.SendRateTimeWindow))
                    {
                        Trace.WriteLine("Exceeding transmission limit. Dropping unguaranteed messages.");
                        session.DropUnguaranteedOutboundMessages(10);
                        break;
                    }

                    // TODO: channels.
                    // TODO: round robin in channels.
                    // TODO: congestion avoidance; adjust bandwidth for each session.
                    // TODO: signal to guaranteed packet send ratio.
                    // TODO: only send either guaranteed or signal messages in each packet.

                    var packet = new Packet
                    {
                        PacketId = session._packetIdCounter++,
                        SessionId = session.OutgoingSessionId,
                        ChannelId = 0,
                        FirstSendTime = DateTime.Now,
                        LastSendTime = DateTime.Now,
                        ResendCount = 0
                    };

                    if (session._firstPacketId == 0)
                    {
                        session._firstPacketId = packet.PacketId;
                    }

                    while ((session.OutboundMessages > 0 || session.PartiallySentMessageCount > 0) && packet.Size < NetworkConstants.MaxPacketSize)
                    {
                        MessageEntry entry = session.GetPartialOutboundMessage();
                        Frame frame = entry.Frames[entry.FramesCompleted];
                        if (entry == null)
                        {
                            break;
                        }

                        if ((packet.Size + frame.Size) < NetworkConstants.MaxPacketSize)
                        {
                            packet.Frames.Add(frame);
                            packet.Flags |= entry.Message.Flags.HasFlag(MessageFlags.Guaranteed) ? PacketFlags.Guaranteed : packet.Flags;

                            entry.FramesCompleted++;

                            if (entry.FramesCompleted == entry.FramesCount)
                            {
                                session.CompletePartialOutboundMessage(entry);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    ByteWriter writer = new ByteWriter(packet.Size);
                    packet.Write(writer);
                    _udpSocket.SendTo(writer.Bytes, packet.Size, SocketFlags.None, session.RemoteEndPoint);

                    PacketsSent++;
                    BytesSent += (ulong)packet.Size;
                    session.BytesSent += (ulong)packet.Size;

                    if (packet.Flags.HasFlag(PacketFlags.Guaranteed))
                    {
                        session.AddPacketWaitingAcknowledge(packet);
                    }
                }

                sendEnd:
                continue;
            }
        }

        private void CleanSessions()
        {
            foreach (Session session in _outIdSessionDictionary.Values)
            {
                DateTime now = DateTime.Now;

                if (session.IsConnecting && DateTime.Now.Subtract(session.CreationTime).TotalSeconds > NetworkConstants.TimeOutSeconds)
                {
                    session.SessionState = SessionState.Disconnected;
                    Trace.WriteLine("Session timed out: " + session.IncomingSessionId + " (" + (session.IsIncoming ? "from" : "to") + " " + session.RemoteEndPoint.Address + ":" + session.RemoteEndPoint.Port + ")");
                }

                if (session.IsDisconnected)
                {
                    _sessionsToRemove.Add(session);
                    Trace.WriteLine("Session destructed: " + session.IncomingSessionId + " (" + (session.IsIncoming ? "from" : "to") + " " + session.RemoteEndPoint.Address + ":" + session.RemoteEndPoint.Port + ")");
                }
            }

            foreach (Session sessionToRemove in _sessionsToRemove)
            {
                _outIdSessionDictionary.Remove(sessionToRemove.OutgoingSessionId);
                _inKeySessionDictionary.Remove(new SessionKey(sessionToRemove.RemoteEndPoint, sessionToRemove.IncomingSessionId));
            }
            _sessionsToRemove.Clear();
        }

        public Session AcceptPendingSession()
        {
            lock (_sessionManagementLock)
            {
                return _incomingPendingSessions.Dequeue();
            }
        }

        public Session OpenSession(string hostname, int port)
        {
            return GetSession(new IPEndPoint(Dns.GetHostAddresses(hostname)[0], port), 0, null);
        }

        public Session GetSession(IPEndPoint remoteEndPoint, uint incomingSessionId, Packet incomingPacket)
        {
            SessionKey sessionKey = new SessionKey(remoteEndPoint, incomingSessionId);
            lock (_sessionManagementLock)
            {
                if (!_inKeySessionDictionary.ContainsKey(sessionKey))
                {
                    // Checking whether this incoming packet is connection request.
                    bool isConnectionRequest = false;
                    if (incomingPacket != null)
                    {
                        MessageTypes messageType = incomingPacket.Frames[0].MessageType;
                        if (messageType == MessageTypes.JoinRequest || messageType == MessageTypes.AttachRequest)
                        {
                            isConnectionRequest = true;
                        }
                    }

                    if (isConnectionRequest)
                    {
                        // Construct new incoming session
                        Session session = new Session(remoteEndPoint, true);
                        session.IncomingSessionId = incomingSessionId;
                        session.OutgoingSessionId = ++_sessionIdCounter;

                        _inKeySessionDictionary.Add(sessionKey, session);
                        _outIdSessionDictionary.Add(session.OutgoingSessionId, session);

                        _incomingPendingSessions.Enqueue(session);

                        Trace.WriteLine("Incoming session constructed: " + session.IncomingSessionId + " (" + (session.IsIncoming ? "from" : "to") + " " + session.RemoteEndPoint.Address + ":" + session.RemoteEndPoint.Port + ")");

                        return session;
                    }
                    else
                    {
                        // This is either response to outgoing session request or new outgoing session request.
                        if (incomingPacket != null)
                        {
                            // This is first response and needs to be bind to outgoing pending session.
                            // Extracting manually message type which should be 1 i.e. ack.
                            MessageTypes messageType = incomingPacket.Frames[0].MessageType;
                            if (messageType != MessageTypes.Acknowledge)
                            {
                                Trace.WriteLine("Incoming packet could not be tied to session - incomingSessionId: " + incomingSessionId + " message type:" + messageType);
                                return null;
                            }

                            // Extracting manually connect request packet id from ack fragment.
                            uint ackedPacketId = BitConverter.ToUInt32(incomingPacket.Frames[0].FrameData, 1);

                            Session session = null;
                            foreach (Session sessionCandidate in _outIdSessionDictionary.Values)
                            {
                                if (sessionCandidate._firstPacketId == ackedPacketId && sessionCandidate.SessionState == SessionState.Connecting)
                                {
                                    session = sessionCandidate;
                                }
                            }

                            if (session != null)
                            {
                                session.IncomingSessionId = incomingSessionId;
                                _inKeySessionDictionary.Add(sessionKey, session);
                                Trace.WriteLine("Session id received from peer: " + session.IncomingSessionId + " (" + (session.IsIncoming ? "from" : "to") + " " + session.RemoteEndPoint.Address + ":" + session.RemoteEndPoint.Port + ")");
                                return session;
                            }
                            else
                            {
                                Trace.WriteLine("Incoming ack packet could not be tied to session. - incomingSessionId: " + incomingSessionId + " message type:" + messageType);
                                return null;
                            }
                        }
                        else
                        {
                            // Construct new outgoing session
                            var session = new Session(remoteEndPoint, false);
                            session.OutgoingSessionId = _sessionIdCounter++;
                            _outIdSessionDictionary.Add(session.OutgoingSessionId, session);

                            Trace.WriteLine("Outgoing session constructed: " + session.IncomingSessionId + " (" + (session.IsIncoming ? "from" : "to") + " " + session.RemoteEndPoint.Address + ":" + session.RemoteEndPoint.Port + ")");

                            return session;
                        }
                    }
                }
                else
                {
                    return _inKeySessionDictionary[sessionKey];
                }
            }
        }
    }
}
