namespace Spark.Networking.Messages
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Content;

    public class Packet : IPrimitiveValue
    {
        public Packet()
        {
            Frames = new List<Frame>();
            FirstSendTime = DateTime.MinValue;
            LastSendTime = DateTime.MinValue;
        }

        public uint PacketId { get; set; }

        public uint SessionId { get; set; }

        public uint ChannelId { get; set; }

        public PacketFlags Flags { get; set; }

        public DateTime FirstSendTime { get; set; }

        public DateTime LastSendTime { get; set; }

        public byte ResendCount { get; set; }

        public List<Frame> Frames { get; }

        public int Size
        {
            get
            {
                return 4 + // PacketId
                       4 + // SessionId
                       4 + // ChannelId
                       1 + // FramesCount
                       1 + // Flags;
                       Frames.Count == 0 ? 0 : Frames.Sum(frame => frame.Size); // Frames
            }
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            PacketId = input.ReadUInt32();
            SessionId = input.ReadUInt32();
            ChannelId = input.ReadUInt32();
            byte framesCount = input.ReadByte();
            Flags = (PacketFlags)input.ReadByte();

            for (int i = 0; i < framesCount; i++)
            {
                var frame = new Frame();
                frame.Read(input);
                Frames.Add(frame);
            }
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("PacketId", PacketId);
            output.Write("SessionId", SessionId);
            output.Write("ChannelId", ChannelId);
            output.Write("FrameCount", (byte)Frames.Count);
            output.Write("Flags", (byte)Flags);
            foreach (Frame frame in Frames)
            {
                frame.Write(output);
            }
        }
    }
}
