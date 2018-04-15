namespace Spark.Networking.Messages
{
    using Content;

    public abstract class Message : IPrimitiveValue
    {
        public uint MessageId { get; set; }

        public abstract MessageTypes Type { get; }

        public MessageTypes MessageType { get; set; }
        
        public MessageFlags Flags { get; set; }

        public virtual int Size => 0;

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public virtual void Read(IPrimitiveReader input)
        {
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public virtual void Write(IPrimitiveWriter output)
        {
        }
    }
}
