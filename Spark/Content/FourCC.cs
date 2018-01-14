namespace Spark.Content
{
    using System;
    using System.Runtime.InteropServices;
    
    /// <summary>
    /// Represents a four character code (32-bit unsigned integer), usually used as a "magic number" to identify the contents of a file format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct FourCC : IEquatable<FourCC>, IPrimitiveValue
    {
        private uint _packedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FourCC"/> struct.
        /// </summary>
        /// <param name="fourCharacterCode">The string representation of a four character code.</param>
        public FourCC(string fourCharacterCode)
        {
            if (fourCharacterCode != null)
            {
                if (fourCharacterCode.Length != 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(fourCharacterCode), "FourCC string must be exactly four characters in length");
                }

                _packedValue = (uint)((fourCharacterCode[3] << 24) | (fourCharacterCode[2] << 16) | (fourCharacterCode[1] << 8) | fourCharacterCode[0]);
            }
            else
            {
                _packedValue = 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FourCC"/> struct.
        /// </summary>
        /// <param name="first">First character</param>
        /// <param name="second">Second character</param>
        /// <param name="third">Third character</param>
        /// <param name="fourth">Fourth character</param>
        public FourCC(char first, char second, char third, char fourth)
        {
            _packedValue = (uint)((((fourth << 24) | (third << 16)) | (second << 8)) | first);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FourCC"/> struct.
        /// </summary>
        /// <param name="packedValue">Packed value represent the four character code.</param>
        public FourCC(uint packedValue)
        {
            _packedValue = packedValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FourCC"/> struct.
        /// </summary>
        /// <param name="packedValue">Packed value represent the four character code.</param>
        public FourCC(int packedValue)
        {
            _packedValue = (uint)packedValue;
        }

        /// <summary>
        /// Gets the empty (a value of zero) four character code.
        /// </summary>
        public static FourCC Empty => new FourCC(0);

        /// <summary>
        /// Gets the size of the <see cref="FourCC"/> structure in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<FourCC>();

        /// <summary>
        /// Gets the first character.
        /// </summary>
        public char First => (char)(_packedValue & 255);

        /// <summary>
        /// Gets the second character.
        /// </summary>
        public char Second => (char)((_packedValue >> 8) & 255);

        /// <summary>
        /// Gets the third character.
        /// </summary>
        public char Third => (char)((_packedValue >> 16) & 255);

        /// <summary>
        /// Gets the fourth character.
        /// </summary>
        public char Fourth => (char)((_packedValue >> 24) & 255);

        /// <summary>
        /// Implicitly converts the <see cref="FourCC"/> instance to an unsigned integer.
        /// </summary>
        /// <param name="fourCharacterCode">Character code</param>
        /// <returns>Unsigned integer representation.</returns>
        public static implicit operator uint(FourCC fourCharacterCode)
        {
            return fourCharacterCode._packedValue;
        }

        /// <summary>
        /// Implicitly converts the <see cref="FourCC"/> instance to an integer.
        /// </summary>
        /// <param name="fourCharacterCode">Character code</param>
        /// <returns>Integer representation</returns>
        public static implicit operator int(FourCC fourCharacterCode)
        {
            return (int)fourCharacterCode._packedValue;
        }

        /// <summary>
        /// Implicitly converts the <see cref="FourCC"/> instance to a String.
        /// </summary>
        /// <param name="fourCharacterCode">Character code</param>
        /// <returns>String representation</returns>
        public static implicit operator string(FourCC fourCharacterCode)
        {
            return new string(new char[] { fourCharacterCode.First, fourCharacterCode.Second, fourCharacterCode.Third, fourCharacterCode.Fourth });
        }

        /// <summary>
        /// Implicitly converts an unsigned integer to a <see cref="FourCC"/> instance.
        /// </summary>
        /// <param name="packedValue">Packed value representing the four character code.</param>
        /// <returns>The FourCC instance.</returns>
        public static implicit operator FourCC(uint packedValue)
        {
            return new FourCC(packedValue);
        }

        /// <summary>
        /// Implicitly converts an integer to a <see cref="FourCC"/> instance.
        /// </summary>
        /// <param name="packedValue">Packed value representing the four character code.</param>
        /// <returns>The FourCC instance.</returns>
        public static implicit operator FourCC(int packedValue)
        {
            return new FourCC(packedValue);
        }

        /// <summary>
        /// Implicitly converts a String to a <see cref="FourCC"/> instance.
        /// </summary>
        /// <param name="fourCharacterCode">String representing the four character code.</param>
        /// <returns>The FourCC instance.</returns>
        public static implicit operator FourCC(String fourCharacterCode)
        {
            return new FourCC(fourCharacterCode);
        }

        /// <summary>
        /// Tests equality between two character codes.
        /// </summary>
        /// <param name="a">First character code</param>
        /// <param name="b">Second character code</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(FourCC a, FourCC b)
        {
            return a._packedValue == b._packedValue;
        }

        /// <summary>
        /// Tests inequality between two character codes.
        /// </summary>
        /// <param name="a">First character code</param>
        /// <param name="b">Second character code</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(FourCC a, FourCC b)
        {
            return a._packedValue != b._packedValue;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is FourCC)
            {
                return Equals((FourCC)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between this character code and another.
        /// </summary>
        /// <param name="other">Other character code</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(FourCC other)
        {
            return _packedValue == other._packedValue;
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            _packedValue = input.ReadUInt32();
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("FourCC", _packedValue);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (int)_packedValue;
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return new string(new char[] { First, Second, Third, Fourth });
        }
    }
}
