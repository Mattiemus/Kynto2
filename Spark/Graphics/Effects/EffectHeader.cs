namespace Spark.Graphics
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using Content;

    /// <summary>
    /// Represents the header of the effect file format. The format is a binary blob
    /// representing a set of techniques and compiled shaders that determines how the rendering pipeline is configured.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EffectHeader : IEquatable<EffectHeader>, IPrimitiveValue
    {
        /// <summary>
        /// The MagicNumber that identifies the format, this is always a four character code of "SPFX".
        /// </summary>
        public FourCC MagicNumber;

        /// <summary>
        /// The runtime ID denoting which effect system the data is compatible with.
        /// </summary>
        public byte RuntimeID;

        /// <summary>
        /// The Version of the format.
        /// </summary>
        public short Version;

        /// <summary>
        /// Reserved.
        /// </summary>
        public int Reserved;

        /// <summary>
        /// Flags denoting compression of the data, if any.
        /// </summary>
        public EffectCompressionMode CompressionMode;

        /// <summary>
        /// Size of the data in bytes.
        /// </summary>
        public uint DataSizeInBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectHeader"/> struct.
        /// </summary>
        /// <param name="runtimeID">Runtime ID of the effects system that the data is compatible with.</param>
        /// <param name="version">Format version</param>
        /// <param name="compressionMode">Compression mode of the data, if any.</param>
        /// <param name="dataSizeInBytes">The size of the data, in bytes.</param>
        public EffectHeader(byte runtimeID, short version, EffectCompressionMode compressionMode, uint dataSizeInBytes)
        {
            MagicNumber = ExpectedMagicNumber;
            RuntimeID = runtimeID;
            Version = version;
            Reserved = 0;
            CompressionMode = compressionMode;
            DataSizeInBytes = dataSizeInBytes;
        }

        /// <summary>
        /// Gets the "SPFX" magic number four character code identifying the format.
        /// </summary>
        public static FourCC ExpectedMagicNumber => new FourCC('S', 'P', 'F', 'X');

        /// <summary>
        /// Gets the size in bytes of the <see cref="EffectHeader"/> structure in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<EffectHeader>();

        /// <summary>
        /// Tests equality between two effect headers.
        /// </summary>
        /// <param name="a">First effect header</param>
        /// <param name="b">Second effect header</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(EffectHeader a, EffectHeader b)
        {
            return a.MagicNumber == b.MagicNumber &&
                  a.RuntimeID == b.RuntimeID && 
                  a.Version == b.Version && 
                  a.Reserved == b.Reserved && 
                  a.CompressionMode == b.CompressionMode && 
                  a.DataSizeInBytes == b.DataSizeInBytes;
        }

        /// <summary>
        /// Tests inequality between two effect headers.
        /// </summary>
        /// <param name="a">First effect header</param>
        /// <param name="b">Second effect header</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(EffectHeader a, EffectHeader b)
        {
            return a.MagicNumber != b.MagicNumber || 
                   a.RuntimeID != b.RuntimeID || 
                   a.Version != b.Version || 
                   a.Reserved != b.Reserved || 
                   a.CompressionMode != b.CompressionMode || 
                   a.DataSizeInBytes != b.DataSizeInBytes;
        }

        /// <summary>
        /// Writes the effect header to a stream.
        /// </summary>
        /// <param name="header">Effect header to write.</param>
        /// <param name="stream">Stream to write to.</param>
        /// <returns>True if writing to the stream was successful.</returns>
        public static bool WriteHeader(EffectHeader header, Stream stream)
        {
            if (stream == null || !stream.CanWrite)
            {
                return false;
            }

            WriteInt32(header.MagicNumber, stream);
            stream.WriteByte(header.RuntimeID);
            WriteInt16(header.Version, stream);
            WriteInt32(header.Reserved, stream);
            stream.WriteByte((byte)header.CompressionMode);
            WriteInt32((int)header.DataSizeInBytes, stream);

            return true;
        }

        /// <summary>
        /// Reads a effect header from the stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>The read TEFX header or null, if there was an error reading from the stream, or if the header data was not present.</returns>
        public static EffectHeader? ReadHeader(Stream stream)
        {
            if (stream == null || !stream.CanRead)
            {
                return null;
            }

            int magicNum, reserved, dataSizeInbytes;
            short version;
            byte runtimeID, compressionMode;

            // Check if we can read a magic number in fully, and once we have it, if its as expected
            if (!ReadInt32(stream, out magicNum) || magicNum != ExpectedMagicNumber)
            {
                return null;
            }

            // Otherwise, we should be good to go with reading the rest
            runtimeID = (byte)stream.ReadByte();
            ReadInt16(stream, out version);
            ReadInt32(stream, out reserved);
            compressionMode = (byte)stream.ReadByte();
            ReadInt32(stream, out dataSizeInbytes);

            return new EffectHeader(runtimeID, version, (EffectCompressionMode)compressionMode, (uint)dataSizeInbytes);
        }
        
        private static bool ReadInt32(Stream stream, out int value)
        {
            value = 0;

            int b1 = stream.ReadByte();
            int b2 = stream.ReadByte();
            int b3 = stream.ReadByte();
            int b4 = stream.ReadByte();

            if (b1 < 0 || b2 < 0 || b3 < 0 || b4 < 0)
            {
                return false;
            }

            value = b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
            return true;
        }

        private static void WriteInt32(int value, Stream stream)
        {
            stream.WriteByte((byte)value);
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 24));
        }

        private static bool ReadInt16(Stream stream, out short value)
        {
            value = 0;

            int b1 = stream.ReadByte();
            int b2 = stream.ReadByte();

            if (b1 < 0 || b2 < 0)
            {
                return false;
            }

            value = (short)(b1 | (b2 << 8));
            return true;
        }

        private static void WriteInt16(short value, Stream stream)
        {
            stream.WriteByte((byte)value);
            stream.WriteByte((byte)(value >> 8));
        }
        
        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            MagicNumber = input.Read<FourCC>();
            RuntimeID = input.ReadByte();
            Version = input.ReadInt16();
            Reserved = input.ReadInt32();
            CompressionMode = input.ReadEnum<EffectCompressionMode>();
            DataSizeInBytes = input.ReadUInt32();
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("MagicNumber", MagicNumber);
            output.Write("RuntimeID", RuntimeID);
            output.Write("Version", Version);
            output.Write("Reserved", Reserved);
            output.WriteEnum("CompressionMode", CompressionMode);
            output.Write("DataSizeInBytes", DataSizeInBytes);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is EffectHeader)
            {
                return Equals((EffectHeader)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between this instance and another.
        /// </summary>
        /// <param name="other">Other effect header instance.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(EffectHeader other)
        {
            return MagicNumber == other.MagicNumber && 
                   RuntimeID == other.RuntimeID && 
                   Version == other.Version && 
                   Reserved == other.Reserved && 
                   CompressionMode == other.CompressionMode && 
                   DataSizeInBytes == other.DataSizeInBytes;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return MagicNumber.GetHashCode() + 
                       RuntimeID.GetHashCode() + 
                       Version.GetHashCode() + 
                       Reserved.GetHashCode() +
                       CompressionMode.GetHashCode() + 
                       DataSizeInBytes.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return string.Format("RuntimeID: {0}, Version: {1}, CompressionMode: {2}, DataSizeInBytes: {3}", new string[] { RuntimeID.ToString(), Version.ToString(), CompressionMode.ToString(), DataSizeInBytes.ToString() });
        }
    }
}
