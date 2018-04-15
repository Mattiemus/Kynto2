namespace Spark.Networking.Messages
{
    public class MessageEntry
    {
        public MessageEntry(Message message, Frame[] frames)
        {
            Message = message;
            FramesCount = (ushort)frames.Length;
            Frames = frames;
        }

        public Message Message { get; }

        public ushort FramesCompleted { get; set; }

        public ushort FramesCount { get; }

        public Frame[] Frames { get; }
    }
}
