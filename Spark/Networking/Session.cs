//namespace Spark.Networking
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Net;

//    using Messages;

//    public class Session
//    {
//        private Queue<Message> _inboundMessages = new Queue<Message>();
//        private Queue<Message> _inboundControlMessages = new Queue<Message>();
//        private Queue<Message> _outboundMessages = new Queue<Message>();
//        private Dictionary<uint, Packet> _packetsWaitingAcknowledge = new Dictionary<uint, Packet>();
//        private Dictionary<uint, MessageEntry> _partialInboundMessages = new Dictionary<uint, MessageEntry>();
//        private LinkedList<MessageEntry> _partialOutboundMessages = new LinkedList<MessageEntry>();
//        private LinkedListNode<MessageEntry> _currentPartialOutboundMessage;
//        private SessionState _sessionState = SessionState.Connecting;
//        private uint _messageIdCounter = 0;
//        private HashSet<uint> _receivedPackets = new HashSet<uint>();
//        internal uint _packetIdCounter = 0;
//        internal ulong _lastBytesReceived = 0;
//        internal ulong _lastBytesSent = 0;
//        internal uint _firstPacketId;

//        public Session(IPEndPoint remoteEndPoint, bool isIncomming)
//        {
//            RemoteEndPoint = remoteEndPoint;
//            SendRateUpdateTime = DateTime.Now;
//            SendRateTimeWindow = 0.2;
//            ReceiveRateUpdateTime = DateTime.Now;
//            ReceiveRateTimeWindow = 0.2;
//            IsIncoming = isIncomming;
//            CreationTime = DateTime.Now;
//            ConnectTime = DateTime.MinValue;
//            DisconnectTime = DateTime.MinValue;
//            LastSendTime = DateTime.Now;
//            LastReceiveTime = DateTime.Now;
//            MaxSendRate = 100000;
//        }
        
//        public IPEndPoint RemoteEndPoint { get; }

//        public uint OutgoingSessionId { get; set; }

//        public uint IncomingSessionId { get; set; }

//        public ICollection<Packet> PacketsWaitingAcknowledge => _packetsWaitingAcknowledge.Values;

//        public ulong MessagesSent { get; private set; }

//        public ulong MessagesReceived { get; private set; }

//        public ulong BytesSent { get; set; }

//        public ulong BytesReceived { get; set; }

//        public double MaxSendRate { get; set; }

//        public double SendRate { get; set; }

//        public DateTime SendRateUpdateTime { get; set; }

//        public double SendRateTimeWindow { get; }

//        public double ReceiveRate { get; set; }

//        public DateTime ReceiveRateUpdateTime { get; set; }

//        public double ReceiveRateTimeWindow { get; }

//        public bool IsIncoming { get; }

//        public HashSet<uint> ReceivedPackets
//        {
//            get
//            {
//                // TODO: Is it likely that id counter overflows. If that happens all received packets are ignored as duplicates.
//                // TODO: This is also memory leak. Replace with container which has limited size and oldest entry is removed first when filled. (Cache)
//                return _receivedPackets;
//            }
//        }

//        public int PartiallyReceivedMessageCount
//        {
//            get
//            {
//                lock (_partialInboundMessages)
//                {
//                    return _partialInboundMessages.Count;
//                }
//            }
//        }

//        public int PartiallySentMessageCount
//        {
//            get
//            {
//                lock (_partialOutboundMessages)
//                {
//                    return _partialOutboundMessages.Count;
//                }
//            }
//        }

//        public int OutboundMessages
//        {
//            get
//            {
//                lock (_outboundMessages)
//                {
//                    return _outboundMessages.Count;
//                }
//            }
//        }

//        public int AvailableMessages
//        {
//            get
//            {
//                lock (_inboundMessages)
//                {
//                    return _inboundMessages.Count;
//                }
//            }
//        }

//        public int AvailableControlMessages
//        {
//            get
//            {
//                lock (_inboundControlMessages)
//                {
//                    return _inboundControlMessages.Count;
//                }
//            }
//        }

//        public DateTime CreationTime { get; }

//        public DateTime ConnectTime { get; private set; }

//        public DateTime DisconnectTime { get; private set; }

//        public DateTime LastSendTime { get; private set; }

//        public DateTime LastReceiveTime { get; private set; }

//        public SessionState SessionState
//        {
//            get => _sessionState;
//            set
//            {
//                if (value == _sessionState)
//                {
//                    return;
//                }

//                if ((_sessionState == SessionState.Connecting) && value == SessionState.Connected)
//                {
//                    _sessionState = SessionState.Connected;
//                    ConnectTime = DateTime.Now;
//                }
//                else if ((_sessionState == SessionState.Connecting || _sessionState == SessionState.Connected) && value == SessionState.Disconnected)
//                {
//                    _sessionState = SessionState.Disconnected;
//                    DisconnectTime = DateTime.Now;
//                }
//                else
//                {
//                    throw new SparkNetworkException("Session can only change state to connected from connecting or to disconnected from connecting or connected.");
//                }
//            }
//        }

//        public bool IsConnecting => _sessionState == SessionState.Connecting;

//        public bool IsConnected => _sessionState == SessionState.Connected;

//        public bool IsDisconnected => _sessionState == SessionState.Disconnected;

//        public void Disconnect()
//        {
//            Send(new DisconnectMessage());
//        }

//        public void Send(Message message)
//        {
//            lock (_outboundMessages)
//            {
//                message.MessageId = _messageIdCounter++;
//                _outboundMessages.Enqueue(message);
//            }
//        }

//        public Message Receive()
//        {
//            lock (_inboundMessages)
//            {
//                if (_inboundMessages.Count > 0)
//                {
//                    return _inboundMessages.Dequeue();
//                }

//                return null;
//            }
//        }

//        public Message ReceiveControlMessage()
//        {
//            lock (_inboundControlMessages)
//            {
//                if (_inboundControlMessages.Count > 0)
//                {
//                    return _inboundControlMessages.Dequeue();
//                }
//                else
//                {
//                    return null;
//                }
//            }
//        }

//        public void DropUnguaranteedOutboundMessages(int messagesToDequeue)
//        {
//            for (int i = 0; i < messagesToDequeue; i++)
//            {
//                Message message = null;
//                lock (_outboundMessages)
//                {
//                    if (_outboundMessages.Count == 0)
//                    {
//                        break;
//                    }
//                    message = _outboundMessages.Dequeue();
//                }

//                // TODO: this can be made better
//                if (message.Flags.HasFlag(MessageFlags.Guaranteed) || message.Flags.HasFlag(MessageFlags.Control))
//                {
//                    lock (_partialOutboundMessages)
//                    {
//                        MessageEntry entry = WriteMessageToEntry(message);
//                        _partialOutboundMessages.AddLast(entry);
//                    }
//                }
//            }
//        }

//        public void AcknowledgePacket(uint packetId)
//        {
//            bool toSend = false;
//            AcknowledgeMessage ackMessage;
//            lock (_outboundMessages)
//            {
//                ackMessage = _outboundMessages.OfType<AcknowledgeMessage>().FirstOrDefault();
//                if (ackMessage == null)
//                {
//                    ackMessage = new AcknowledgeMessage();
//                    ackMessage.Acknowledges.Add(packetId);
//                    toSend = true;
//                }
//                else
//                {
//                    ackMessage.Acknowledges.Add(packetId);
//                }
//            }

//            if (toSend)
//            {
//                Send(ackMessage);
//            }
//        }

//        public void AddPacketWaitingAcknowledge(Packet packet)
//        {
//            if (_packetsWaitingAcknowledge.Count > NetworkConstants.MaxPacketsWaitingAcknowledge)
//            {
//                SessionState = SessionState.Disconnected;
//            }
//            _packetsWaitingAcknowledge.Add(packet.PacketId, packet);
//        }

//        public void RemovePacketWaitingAcknowledge(uint packetId)
//        {
//            if (_packetsWaitingAcknowledge.ContainsKey(packetId))
//            {
//                _packetsWaitingAcknowledge.Remove(packetId);
//            }
//        }

//        public MessageEntry GetPartialInboundMessage(uint messageId, MessageTypes messageType, ushort frameCount)
//        {
//            lock (_partialInboundMessages)
//            {
//                LastReceiveTime = DateTime.Now;
//                if (_partialInboundMessages.ContainsKey(messageId))
//                {
//                    return _partialInboundMessages[messageId];
//                }
//                else
//                {
//                    var entry = new MessageEntry(messageType, frameCount);
//                    entry.Message.MessageId = messageId;
//                    _partialInboundMessages.Add(messageId, entry);
//                    return entry;
//                }
//            }
//        }

//        public void CompletePartialInboundMessage(uint id, MessageEntry entry)
//        {
//            MessagesReceived++;

//            lock (_partialInboundMessages)
//            {
//                _partialInboundMessages.Remove(entry.Message.MessageId);
//            }

//            var data = new byte[entry.FramesCount * NetworkConstants.MaxFrameDataSize];
//            int currentPos = 0;
//            foreach (var frame in entry.Frames)
//            {
//                Buffer.BlockCopy(frame.FrameData, 0, data, currentPos, frame.FrameLength);
//                currentPos += frame.FrameLength;
//            }
//            ByteReader reader = new ByteReader(data, currentPos);
//            entry.Message.Read(reader);

//            if (entry.Message.Flags.HasFlag(MessageFlags.Control))
//            {
//                lock (_inboundControlMessages)
//                {
//                    _inboundControlMessages.Enqueue(entry.Message);
//                }
//            }
//            else
//            {
//                lock (_inboundMessages)
//                {
//                    _inboundMessages.Enqueue(entry.Message);
//                }
//            }
//        }

//        public MessageEntry GetPartialOutboundMessage()
//        {
//            int partialOutboundMessageCount = 0;
//            lock (_partialOutboundMessages)
//            {
//                partialOutboundMessageCount = _partialOutboundMessages.Count;
//            }

//            if (partialOutboundMessageCount < NetworkConstants.MaxPacketsBeingFragmented)
//            {
//                lock (_outboundMessages)
//                {
//                    if (_outboundMessages.Count > 0)
//                    {
//                        var message = _outboundMessages.Dequeue();
//                        var entry = WriteMessageToEntry(message);
//                        _partialOutboundMessages.AddLast(entry);
//                    }
//                }
//            }

//            lock (_partialOutboundMessages)
//            {
//                if (_partialOutboundMessages.Count == 0)
//                {
//                    return null;
//                }

//                if (_currentPartialOutboundMessage == null)
//                {
//                    _currentPartialOutboundMessage = _partialOutboundMessages.First;
//                }

//                LastSendTime = DateTime.Now;
//                MessageEntry Message = _currentPartialOutboundMessage.Value;
//                _currentPartialOutboundMessage = _currentPartialOutboundMessage.Next;

//                return Message;
//            }
//        }

//        public void CompletePartialOutboundMessage(MessageEntry entry)
//        {
//            lock (_partialOutboundMessages)
//            {
//                MessagesSent++;
//                _partialOutboundMessages.Remove(entry);
//                if (_currentPartialOutboundMessage != null && _currentPartialOutboundMessage.Value == entry)
//                {
//                    _currentPartialOutboundMessage = null;
//                }
//            }

//            if (entry.Message.Type == MessageTypes.Disconnect)
//            {
//                SessionState = SessionState.Disconnected;
//            }
//        }

//        private MessageEntry WriteMessageToEntry(Message message)
//        {
//            int messageSize = (int)message.Size;

//            ByteWriter writer = new ByteWriter(messageSize);
//            message.Write(writer);

//            var framesCount = (int)Math.Ceiling(messageSize / (double)NetworkConstants.MaxFrameDataSize);
//            var frames = new Frame[framesCount];
//            int position = 0;
//            for (int i = 0; i < framesCount; i++)
//            {
//                int frameLength = Math.Min(NetworkConstants.MaxFrameDataSize, messageSize - (i * NetworkConstants.MaxFrameDataSize));

//                var frame = new Frame
//                {
//                    MessageId = message.MessageId,
//                    MessageType = message.Type,
//                    FrameCount = (ushort)framesCount,
//                    FrameIndex = (ushort)i,
//                    FrameLength = (byte)frameLength
//                };
//                Buffer.BlockCopy(writer.Bytes, position, frame.FrameData, 0, frameLength);
//                position += frameLength;

//                frames[i] = frame;
//            }

//            return new MessageEntry(message, frames);
//        }
//    }
//}
