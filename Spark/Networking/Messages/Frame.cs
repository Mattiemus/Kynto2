namespace Spark.Networking.Messages
{
    using System;

    using Content;

    public class Frame : IPrimitiveValue
    {
        public Frame()
        {
            FrameData = new byte[255];
            FrameLength = 0;
        }

        public Frame(int length)
        {
            FrameData = new byte[255];
            FrameLength = (byte)length;
        }

        public Frame(byte[] data)
        {
            FrameData = new byte[255];
            Buffer.BlockCopy(data, 0, FrameData, 0, data.Length);
            FrameLength = (byte)data.Length;
        }

        public Frame(byte[] data, int start, int count)
        {
            FrameData = new byte[255];
            Buffer.BlockCopy(data, start, FrameData, 0, count);
            FrameLength = (byte)count;
        }

        public int Size
        {
            get
            {
                return 4 + // MessageId
                       2 + // MessageType
                       2 + // FrameCount
                       2 + // FrameIndex
                       1 + // FrameLength
                       FrameLength; // FrameData
            }
        }

        public uint MessageId { get; set; }

        public MessageTypes MessageType { get; set; }

        public ushort FrameCount { get; set; }

        public ushort FrameIndex { get; set; }

        public byte FrameLength { get; set; }

        public byte[] FrameData { get; }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            MessageId = input.ReadUInt32();
            MessageType = (MessageTypes)input.ReadUInt16();
            FrameCount = input.ReadUInt16();
            FrameIndex = input.ReadUInt16();
            FrameLength = input.ReadByte();
            for (int i = 0; i < FrameLength; i++)
            {
                FrameData[i] = input.ReadByte();
            }
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("MessageId", MessageId);
            output.Write("MessageType", (ushort)MessageType);
            output.Write("FrameCount", FrameCount);
            output.Write("FrameIndex", FrameIndex);
            output.Write("FrameLength", FrameLength);
            for(int i = 0; i < FrameLength; i++)
            {
                output.Write($"FrameData{i}", FrameData[i]);
            }
        }
    }
}
